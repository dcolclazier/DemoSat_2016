using DemoSat16.Event_Data;
using DemoSat16.Utility;
using Microsoft.SPOT;

namespace DemoSat16.Event_Subscriptions {
    public class GyroEventTester {
        //this is the constructor - so that when we create one of these, it adds the necessary
        //trigger code (in this case the method TestGyro) to the 
        public GyroEventTester() {
            
        }

        public void Start() {
            FlightComputer.OnEventTriggered += TestGyro;
        }

        public void Stop() {
            FlightComputer.OnEventTriggered -= TestGyro;
        }
        //this is the code that runs when the Gyro event triggers!
        private void TestGyro(FlightEventType eventType, IEventData eventData) {
            if (eventType != FlightEventType.Gyro) return;

            GyroData data = eventData as GyroData;
            if (data == null) return;

            Debug.Print("Gyro X: " + data.X + "  Y: " + data.Y + " Z:" + data.Z);
            Debug.Print(data?.X.ToString());
            Debug.Print("Gyro Y: ");
            Debug.Print(data?.Y.ToString());
            Debug.Print("Gyro Z: ");
            Debug.Print(data?.Z.ToString());
            Debug.Print("\n");
        }
    }
}