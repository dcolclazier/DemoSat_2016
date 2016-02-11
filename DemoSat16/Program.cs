using Microsoft.SPOT;

namespace DemoSat16
{
    public class Program {
        private static Bno055 _bnoSensor;

        public static void Main()
        {
            //This communicates via I2C on the default address, clockrate, and mode.
            _bnoSensor = new Bno055();

            //create class that manages persistent work item that updates gyroscope
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

    //this is an example of using the flight computer to execute a persistent work item that
    //also needs to trigger an event upon completion.
    public class GyroUpdater {

        private readonly GyroData _gyroData;
        private readonly WorkItem _worker;
        private readonly Bno055 _sensor;

        public GyroUpdater(Bno055 sensor) {
            _sensor = sensor;
            _gyroData = new GyroData();

            _worker = new WorkItem(UpdateGyro, true, FlightEventType.Gyro, _gyroData);
        }

        public void Start() {
            FlightComputer.Instance.Execute(_worker);
        }

        public void UpdateGyro() {
            var test = _sensor.GetVector(Bno055.Bno055VectorType.Vector_Gyroscope);
            _gyroData.X = test.X; 
            _gyroData.Y = test.Y; 
            _gyroData.Z = test.Z;  
        }
    }
}
