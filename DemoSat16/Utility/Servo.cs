using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace DemoSat16.Utility
{
    internal class Servo : IDisposable {

        private readonly PWM _servoPin;
        private readonly int[] range = new int[2];
        public bool Inverted = false;

        public Servo(Cpu.PWMChannel servoPin) {

            _servoPin = new PWM(servoPin, 50, 0, false);
            range[0] = 500;
            range[1] = 2200;
        }

        public void Dispose() {
            disengage();
            _servoPin.Dispose();
        }

        private void disengage() {
            _servoPin.DutyCycle = 0;
        }

        public void setRange(int fullLeft, int fullRight) {
            range[1] = fullLeft;
            range[0] = fullRight;
        }

        public double Degree {
            set {
                if (value > 180) value = 180;
                if (value < 0) value = 0;
                if (Inverted) value = 180 - value;
                _servoPin.Duration = (uint)map((long)value, 0, 180, range[0], range[1]);
                Debug.Print(_servoPin.Duration.ToString());
                _servoPin.Start();

            }
        }

        public long map(long x, long in_min, long in_max, long out_min, long out_max) {
            return (x - in_min)*(out_max - out_min)/(in_max - in_min) + out_min;
        }
}
}
