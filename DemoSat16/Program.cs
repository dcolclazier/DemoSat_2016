using System.Threading;
using DemoSat16.Utility;
using DemoSat16.Work_Items;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Messaging;
using SecretLabs.NETMF.Hardware.Netduino;


namespace DemoSat16
{
    public class Program {
      

        public static void Main()
        {
            //var gopro = new GoPro(Pins.GPIO_PIN_D9);
            //gopro.Start();


            var tracker = new LightTracker(PWMChannels.PWM_PIN_D5, PWMChannels.PWM_PIN_D10,
                                        Cpu.AnalogChannel.ANALOG_0, Cpu.AnalogChannel.ANALOG_1,
                                        Cpu.AnalogChannel.ANALOG_2, Cpu.AnalogChannel.ANALOG_3);
            tracker.Start();



            //// ONE
            // ContServo bob = new ContServo(PWMChannels.PWM_PIN_D5);
            // for (int x = 0; x < 3; x++)
            // {
            //     bob.go_counterclockwise_one_tick();
            //     Debug.Print("Went counter clockwise a tick?");
            //     Thread.Sleep(200);

            // }
            // for (int x = 0; x < 3; x++)
            // {
            //     bob.go_clockwise_one_tick();
            //     Debug.Print("Went clockwise a tick?");
            //     Thread.Sleep(200);

            // }


            ////TWO
            //AnalogInput photoCellTL = new AnalogInput(Cpu.AnalogChannel.ANALOG_0);
            //AnalogInput photoCellTR = new AnalogInput(Cpu.AnalogChannel.ANALOG_1);
            //AnalogInput photoCellBL = new AnalogInput(Cpu.AnalogChannel.ANALOG_2);
            //AnalogInput photoCellBR = new AnalogInput(Cpu.AnalogChannel.ANALOG_3);

            //while (true) {
            //    double _topLeft = photoCellTL.Read();
            //    double _topRight = photoCellTR.Read();
            //    double _bottomLeft = photoCellBL.Read();
            //    double _bottomRight = photoCellBR.Read();

            //    Debug.Print("TopLeft" + _topLeft);
            //    Debug.Print("TopRight" + _topRight);
            //    Debug.Print("BottomLeft" + _bottomLeft);
            //    Debug.Print("BottomRight" + _bottomRight);

            //    Thread.Sleep(2000);
            //}

            //Thread.Sleep(500);
            //bob.go_clockwise_one_tick();
            //Debug.Print("Went clockwise a tick?");
        }

}
}
