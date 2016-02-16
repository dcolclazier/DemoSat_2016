﻿using System;
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

            var tracker = new LightTracker(PWMChannels.PWM_PIN_D6,PWMChannels.PWM_PIN_D5, 
                                        Cpu.AnalogChannel.ANALOG_0, Cpu.AnalogChannel.ANALOG_1, 
                                        Cpu.AnalogChannel.ANALOG_2, Cpu.AnalogChannel.ANALOG_3);
            tracker.StartDemo();
            //Thread.Sleep(5000);
            //driver.StopDemo();
            
            
            //activate event class that responds to gyroscope update
            //var gyroevent = new GyroEventTester();
            //gyroevent.Start();
        }

    }

    public class LightTracker {

        private const int Tolerance = 0;
        private const int DelayTime = 10;

        private readonly WorkItem _trackAction;

        private readonly Servo _panServo;
        private readonly Servo _tiltServo;

        private readonly AnalogInput photoCellTL;
        private readonly AnalogInput photoCellTR;
        private readonly AnalogInput photoCellBL;
        private readonly AnalogInput photoCellBR;

        public LightTracker(Cpu.PWMChannel panPin, Cpu.PWMChannel tiltPin, Cpu.AnalogChannel topLeftPhotocell, Cpu.AnalogChannel topRightPhotocell, Cpu.AnalogChannel bottomLeftPhotocell, Cpu.AnalogChannel bottomRightPhotocell) {

            _panServo = new Servo(panPin);
            _tiltServo = new Servo(tiltPin);

            photoCellTL = new AnalogInput(topLeftPhotocell);
            photoCellTR = new AnalogInput(topRightPhotocell);
            photoCellBL = new AnalogInput(bottomLeftPhotocell);
            photoCellBR = new AnalogInput(bottomRightPhotocell);

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

            var topAvg = (topLeft + topRight)/2;
            var bottomAvg = (bottomLeft + bottomRight)/2;
            var leftAvg = (topLeft + bottomLeft)/2;
            var rightAvg = (bottomRight + topRight)/2;

            var dZ = topAvg - bottomAvg;
            var dX = leftAvg - rightAvg;

            if (-1*Tolerance > dZ || dZ > Tolerance) {
                if (topAvg > bottomAvg) _tiltServo.Degree += 1;
                else if (topAvg < bottomAvg) _tiltServo.Degree -= 1;
            }
            if (-1*Tolerance > dX || dX > Tolerance) {
                if (leftAvg > rightAvg) _panServo.Degree += 1;
                else if (leftAvg < rightAvg) _panServo.Degree -= 1;
            }

            _panServo.disengage();
            _tiltServo.disengage();
            Thread.Sleep(DelayTime);
        }

        
    }
}
