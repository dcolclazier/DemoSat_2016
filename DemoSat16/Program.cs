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
            _bnoSensor = new Bno055();

            //create class that manages persistent work item that updates gyroscope data
            var Gyro = new GyroUpdater(_bnoSensor);
            Gyro.Start();
            
            //activate event class that responds to gyroscope update
            var gyroevent = new GyroEventTester();
            gyroevent.Start();
        }

    }


    //This is an example of a class that needs to respond to a Gyro event. It subscribes to the event 
    //system in the constructor, and then only has a single method that pronts the gyro data to the debug log,
    //every time the gyro updates. This assumes that the tester can run as fast as the gyro is updating.

    //this is an example of using the flight computer to execute a persistent work item that
    //also needs to trigger an event upon completion.
}
