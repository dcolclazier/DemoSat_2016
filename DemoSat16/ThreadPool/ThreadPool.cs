using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;

namespace DemoSat16 {
    public static class ThreadPool {

        //This is a list of all of the running threads in our pool - each thread does something different 
        //at effectively the same time.
        private static readonly ArrayList AvailableThreads = new ArrayList();

        //this is a queue (First in, first out, like a DMV line) of work items the flight computer 
        // submitted for execution.
        private static readonly Queue PendingWorkItems = new Queue();

        //The maximum number of threads our threadpool is allowed to have doing work at any given time..
        private const int MAX_THREADS = 3;

        // This is the method the flight computer calls when you, the programmer, execute a work item.
        static public void QueueWorkItem(WorkItem thingToQueue) {
            //first, add the work item to the queue of work items (if something came before, it comes first)
            lock (locker) {
                PendingWorkItems.Enqueue(thingToQueue);
            }

            //If we haven't reached our MAX_THREADS yet, go ahead and create one... 
            //    Note" this will only run the first MAX_THREADS (3) times we execute 
            //    a work item in the flight computer - then, we'll have our 3 threads, 
            //    and this if block will be skipped.
            if (AvailableThreads.Count < MAX_THREADS) {
                var thread = new Thread(ThreadWorker);
                AvailableThreads.Add(thread);
                thread.Start();
            }

            //Let our threadworkers know that there's something in the queue they should grab.
            lock (locker) {
                ThreadSync.Set();
            }
        }


        //This is the little gnome that sits around, waiting for a work item to be added to the queue so it can execute.
        private static void ThreadWorker() {
            while (true) {
                
                //Wait for a signal from the ThreadPool indicating that a work item has been added to the queue for execution.
                ThreadSync.WaitOne();

                //If we got here, we stopped waiting because there's a work item - let's grab it:
                WorkItem nextWorkItemToExecute = null;


                //lock this section of code, since we have multiple threads trying to access PendingWorkItems and ThreadSync concurrently
                //this just means that only one thread can run it at a time, so we don't get any weird "1+1=1" behaviour
                lock (locker) {

                    //If we're the first to check this and there's still something in the PendingWorkItems, grab the first thing in line.
                    if (PendingWorkItems.Count > 0)
                        nextWorkItemToExecute = PendingWorkItems.Dequeue() as WorkItem;
                    else {
                        //We weren't the first to check this; some other thread came and grabbed it, and there's no more pending work items..
                        //Let the ThreadPool know about this, so that all thread workers (including this one) start waiting for something to be queued.
                        ThreadSync.Reset();
                    }
                }

                //If we didn't get a work item, start over, and wait for another work item to appear.
                if (nextWorkItemToExecute == null) continue;

                //We got a work item! Let's try to take care of it.
                try {
                    //Execute the work item.
                    nextWorkItemToExecute.Action();

                    //If the work item needed to trigger an event, do so now:
                    if (nextWorkItemToExecute.EventType != FlightEventType.None) {
                        FlightComputer.Instance.TriggerEvent(nextWorkItemToExecute.EventType,
                            nextWorkItemToExecute.EventData);
                    }
                    //We finished the work item, and triggered its event. Now, we need to queue the work item again if it was marked as persistent...
                    if (nextWorkItemToExecute.IsPersistent) QueueWorkItem(nextWorkItemToExecute);
                }
                //If something went wrong, stick it in the debug - don't crash the program.
                catch (Exception e) {
                    Debug.Print("ThreadPool: Unhandled error executing action - " + e.Message + e.InnerException);
                    Debug.Print("StackTrace: " + e.StackTrace);
                }


            }
        }


        private static readonly ManualResetEvent ThreadSync = new ManualResetEvent(false);
        private static readonly object locker = new object();
    }
}