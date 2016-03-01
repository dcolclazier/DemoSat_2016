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
        //private static Bno055 _bnoSensor;

        public static void Main()
        {
            //This communicates via I2C on the default address, clockrate, and mode.
            //All initialization is taken care of behind the scenes, simplifying the user-facing code.
            //_bnoSensor = new Bno055();

            //create class that manages persistent work item that updates gyroscope data
            // var Gyro = new GyroUpdater(_bnoSensor);
            //Gyro.Start();

            var tracker = new LightTracker(PWMChannels.PWM_PIN_D5, PWMChannels.PWM_PIN_D10,
                                        Cpu.AnalogChannel.ANALOG_0, Cpu.AnalogChannel.ANALOG_1,
                                        Cpu.AnalogChannel.ANALOG_2, Cpu.AnalogChannel.ANALOG_3);
            tracker.StartDemo();

            //var servo = new Servo(PWMChannels.PWM_PIN_D5,180);
            //servo.Degree = 180;

            //Thread.Sleep(5000);
            //driver.StopDemo();


            //activate event class that responds to gyroscope update
            //var gyroevent = new GyroEventTester();
            //gyroevent.Start();
        }

    }

    public class LightTracker {

        private const double Tolerance = 0.005;
        private const int DelayTime = 10;
        private double _lastX;
        private double _lastY;
        // private TestServo servo = null;
        private readonly WorkItem _trackAction;

        private readonly Servo _panServo;
        private readonly Servo _tiltServo;

        private readonly AnalogInput photoCellTL;
        private readonly AnalogInput photoCellTR;
        private readonly AnalogInput photoCellBL;
        private readonly AnalogInput photoCellBR;
        private readonly AnalogInput speedPot;

        public LightTracker(Cpu.PWMChannel panPin, Cpu.PWMChannel tiltPin, Cpu.AnalogChannel topLeftPhotocell, Cpu.AnalogChannel topRightPhotocell, Cpu.AnalogChannel bottomLeftPhotocell, Cpu.AnalogChannel bottomRightPhotocell) {


            _panServo = new Servo(panPin, 90) {Degree = 90};
            _lastX = _panServo.Degree;

            _tiltServo = new Servo(tiltPin, 90) {Degree = 90};
            _lastY = _tiltServo.Degree;


            photoCellTL = new AnalogInput(topLeftPhotocell);
            photoCellTR = new AnalogInput(topRightPhotocell);
            photoCellBL = new AnalogInput(bottomLeftPhotocell);
            photoCellBR = new AnalogInput(bottomRightPhotocell);

            speedPot = new AnalogInput(AnalogChannels.ANALOG_PIN_A5);

            _trackAction = new WorkItem(SearchForLight, true);
        }

        public void StartDemo() {
            FlightComputer.Instance.Execute(_trackAction);
        }

        public void StopDemo() {
            _trackAction.SetRepeat(false);
        }

        private void SearchForLight() {
           
            var topLeft = photoCellTL.Read();
            var topRight = photoCellTR.Read();
            var bottomLeft = photoCellBL.Read();
            var bottomRight = photoCellBR.Read();

            var topAvg = (topLeft + topRight) / 2;
            var bottomAvg = (bottomLeft + bottomRight) / 2;
            var leftAvg = (topLeft + bottomLeft) / 2;
            var rightAvg = (bottomRight + topRight) / 2;

            var dY = topAvg - bottomAvg;
            var dX = leftAvg - rightAvg;
            //Debug.Print("dX: " + dX);
            //Debug.Print("dY: " + dY);

            var speedPotValue = (int)(speedPot.Read() * 10);
            
            if (-1 * Tolerance > dY || dY > Tolerance)
            {

                if (topAvg > bottomAvg)
                {
                    //Debug.Print("Moved up 1 degree.");
                    _lastY--;
                }
                else if (topAvg < bottomAvg)
                {
                    //Debug.Print("Moved down 1 degree.");
                    _lastY++;
                }
                if(_lastY > 180) _lastY = 0; //if we went too far, start over
                if (_lastY < 0) _lastY = 180;
                _tiltServo.Degree = _lastY;
                Thread.Sleep(5);
                _tiltServo.disengage();
            }
            if (-1 * Tolerance > dX || dX > Tolerance)
            {
                if (leftAvg > rightAvg)
                {
                    Debug.Print("Moved left 1 degree.");
                    _lastX--;
                }
                else if (leftAvg < rightAvg)
                {
                    Debug.Print("Moved right 1 degree.");
                    _lastX++;
                }
                if (_lastX > 180) _lastX = 0;
                if (_lastX < 0) _lastX = 180;

                _panServo.Degree = _lastX;
                Thread.Sleep(5);
                _panServo.disengage();

                Debug.Print("Last Heading: " + _lastX);
            }
            
        }


    }
}
