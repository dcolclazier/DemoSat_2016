namespace DemoSat16 {
    //The flight computer is our very simple brain to our payload... It sits around waiting for stuff to execute and then does so. 
    //We'll look at an example of this later.
    public class FlightComputer {
        //Singleton OOP model
        private static FlightComputer _instance;
        public static FlightComputer Instance => _instance ?? (_instance = new FlightComputer());

        //Manages the execution of a WorkItem that needs to be run.
        public void Execute(WorkItem workItem) {
            ThreadPool.QueueWorkItem(workItem);
        }

        //Manages event triggers - when an event is triggered, if anyone has subscribed to it, it will let them know so they can execute code
        //Keep in mind - I personify other classes and objects in code(anyone, them, they, he, she, etc) as it makes it a litte easier to talk about.
        public void TriggerEvent(FlightEventType eventType, IEventData eventData) {
            OnEventTriggered?.Invoke(eventType, eventData);            
        }


        //If you want to subscribe to a FlightComputer event, this is the syntax that the method/function HAS to be:
        public delegate void EventTriggered(FlightEventType eventType, IEventData eventData);
        //EXAMPLE OF HOW TO USE THIS:
        // 
        //First, you write the function: this function would respond to gyro events and then log the data.
        //
        /* void GyroTriggerTest1 (FlightEventType eventType, object eventData)
               {
                  if(eventType != FlightEventType.Gyro) return;   // this makes sure that if the event wasn't a gyro event, we don't run this code. 
                                                                  //     In other words, we don't want to log gyro data if the event was a temperature update!
                  var gyroData = eventData as GyroData;             // this turns the eventData into the data we want it... since the event triggered was a Gyro Event,
                                                                  //     we know that the eventData object contains data that the gyroscope obtained.  If it was a temp update,
                                                                  //     we'd know that the eventData object contained data that the thermometer obtained. And so on...
                                                                   
                  GyroEventTester.log(gyroData)                            // A possible reason we might want to respond to a Gyroscope update!
               }
        */
        //Next, once the function/method is written, you have to subscribe it to the events:
        //              FlightComputer.OnEventTriggered += GyroTriggerTest1;
        //
        //That's IT!!! Every time a FlightEventType.Gyro event is triggered, this function will run, and log the data!

        public static event EventTriggered OnEventTriggered;


    }
    
}