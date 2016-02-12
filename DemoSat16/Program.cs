using DemoSat16.Event_Subscriptions;
using DemoSat16.Sensor_Libs;
using DemoSat16.Work_Items;

namespace DemoSat16
{
    public class Program {
        private static Bno055 _bnoSensor;

        public static void Main()
        {
            //This communicates via I2C on the default address, clockrate, and mode.
            //All initialization is taken care of behind the scenes, simplifying the user-facing code.
            _bnoSensor = new Bno055();

            //create class that manages persistent work item that updates gyroscope data
            var Gyro = new GyroUpdater(_bnoSensor);
            Gyro.Start();
            
            //activate event class that responds to gyroscope update
            var gyroevent = new GyroEventTester();
            gyroevent.Start();
        }

    }


    

    
}
