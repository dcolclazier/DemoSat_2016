using System.Threading;
using DemoSat16.Event_Data;

namespace DemoSat16.Utility {

    //A work item is anything the Flight Computer needs to execute- could be a sensor update, a log message, writing to the SD card, or anything else really. 
    public class WorkItem {

        //A threadstart is just a thing built into C# that holds onto some method or function of code that needs to be run in a dedicated thread.
        //It will hold onto the code the FlightComputer needs to execute!
        public readonly ThreadStart Action = null;

        //The event type can indicate an event that some other sensor needs to know about.
        public readonly FlightEventType EventType;

        //The eventData could be a gyro data, a picture from the spectrometer, etc.
        public readonly IEventData EventData;

        //If IsPersistent is true, this WorkItem executed repeatedly, as if it was in a loop
        public bool IsPersistent { get; private set; }


        //This is the constructor for a WorkItem - the only required parameter is the threadstart itself, but you can optionally add additional parameters if you need.
        //If you don't provide an eventType or eventData, the workItem assumes it does not trigger an event.
        //If you don't tell the work item it is persistent when you create it, it will assume it only needs to be run once.
        public WorkItem(ThreadStart action, bool isPersistent = false, FlightEventType type = FlightEventType.None, IEventData eventData = null) {
            Action = action;
            EventType = type;
            EventData = eventData;
            IsPersistent = isPersistent;
        }

        public void SetRepeat(bool persistence) {
            IsPersistent = persistence;
        }
    }

    
}