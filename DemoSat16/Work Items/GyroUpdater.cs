using DemoSat16.Event_Data;
using DemoSat16.Sensor_Libs;
using DemoSat16.Utility;

namespace DemoSat16.Work_Items {
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