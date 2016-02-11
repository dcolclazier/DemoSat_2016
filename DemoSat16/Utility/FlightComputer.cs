using DemoSat16.Event_Data;

namespace DemoSat16.Utility {
    //The flight computer is our very simple brain to our payload... It sits around waiting for stuff to execute and then does so. 
    //We'll look at an example of this later.
    public class FlightComputer {
        //Singleton OOP model - just makes sure there's only ever one Flight Computer.
        private static FlightComputer _instance;
        public static FlightComputer Instance => _instance ?? (_instance = new FlightComputer());

        //Manages the execution of a WorkItem that needs to be run.
        public void Execute(WorkItem workItem) {
            ThreadPool.QueueWorkItem(workItem);
        }

        //Manages responses to events - when an event is triggered, if anyone has subscribed to it, it will let them know so they can execute code
        //Keep in mind - I personify other classes and objects in code(anyone, them, they, he, she, etc) as it makes it a litte easier to talk about.
        public void TriggerEvent(FlightEventType eventType, IEventData eventData) {
            OnEventTriggered?.Invoke(eventType, eventData);            
        }


        //If you want to subscribe to a FlightComputer event, this is the syntax that the method/function HAS to be:
        public delegate void EventTriggered(FlightEventType eventType, IEventData eventData);
        
        //this is what is subscribed to by others who want to listen for Flight events.
        public static event EventTriggered OnEventTriggered;


    }
    
}