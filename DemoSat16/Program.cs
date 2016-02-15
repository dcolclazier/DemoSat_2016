using System;
using System.Threading;
using DemoSat16.Event_Subscriptions;
using DemoSat16.Sensor_Libs;
using DemoSat16.Utility;
using DemoSat16.Work_Items;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace DemoSat16
{
    public class Program {
        private static Bno055 _bnoSensor;

        public static void Main()
        {
            //This communicates via I2C on the default address, clockrate, and mode.
            //All initialization is taken care of behind the scenes, simplifying the user-facing code.
            //_bnoSensor = new Bno055();

            //create class that manages persistent work item that updates gyroscope data
           // var Gyro = new GyroUpdater(_bnoSensor);
            //Gyro.Start();

            var driver = new MotorDriver(PWMChannels.PWM_PIN_D5,PWMChannels.PWM_PIN_D6);
            driver.StartDemo();
            
            //activate event class that responds to gyroscope update
            //var gyroevent = new GyroEventTester();
            //gyroevent.Start();
        }

    }

    public class MotorDriver {

        private readonly WorkItem _panAction;
        private readonly WorkItem _tiltAction;

        private Servo _panServo;
        private Servo _tiltServo;

        public MotorDriver(Cpu.PWMChannel panPin, Cpu.PWMChannel tiltPin) {
            _panServo = new Servo(panPin);
            _tiltServo = new Servo(tiltPin);
            _panAction = new WorkItem(DoSomePanning, true);
            _tiltAction = new WorkItem(DoSomeTilting, true);
        }

        public void StartDemo() {
            FlightComputer.Instance.Execute(_panAction);
            FlightComputer.Instance.Execute(_tiltAction);
        }

        public void StopDemo() {
            _panAction.SetRepeat(false);
            _tiltAction.SetRepeat(false);
        }

        private void DoSomePanning() {
            for (int i = 0; i <= 180; i++) {
                _panServo.Degree = i;
                //Debug.Print("Yep");
                Thread.Sleep(10);
            }
            _panServo.Inverted = !_panServo.Inverted;
        }

        private void DoSomeTilting() {
            for (int i = 0; i <= 180; i++) {
                _tiltServo.Degree = i;
                Thread.Sleep(10);
            }
            _tiltServo.Inverted = !_tiltServo.Inverted;
        }
    }
}
