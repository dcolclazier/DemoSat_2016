using System.Threading;

namespace DemoSat16 {

    //A work item is anything the Flight Computer needs to execute- could be a sensor update, a log message, writing to the SD card, or anything else really. 
    public class WorkItem {

        //A threadstart is just a thing built into C# that holds onto some method or function of code that needs to be run in a dedicated thread.
        public readonly ThreadStart Action = null;

        //The event type can indicate an event that some other sensor needs to know about.
        public readonly FlightEventType EventType;

        //The eventData could be a reference to a sensor, could be a reference to some data that needs to be logged, or anything else really.
        public readonly IEventData EventData;

        //If IsPersistent is true, this WorkItem executed repeatedly, as if it was in a loop
        public bool IsPersistent { get; }


        //This is the constructor for a WorkItem - the only required parameter is the threadstart itself, but you can optionally add additional parameters if you need.
        public WorkItem(ThreadStart action, bool isPersistent = false, FlightEventType type = FlightEventType.None, IEventData eventData = null) {
            Action = action;
            EventType = type;
            EventData = eventData;
            IsPersistent = isPersistent;
        }
    }

    //The different events that a Work Item can eventData upon completion.
}