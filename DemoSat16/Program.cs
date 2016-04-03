using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace DemoSat16
{
    public class Program {
      

        public static void Main()
        {
            var gopro = new GoPro(Pins.GPIO_PIN_D9);
            gopro.Start();


            var tracker = new LightTracker(PWMChannels.PWM_PIN_D5, PWMChannels.PWM_PIN_D10,
                                        Cpu.AnalogChannel.ANALOG_0, Cpu.AnalogChannel.ANALOG_1,
                                        Cpu.AnalogChannel.ANALOG_2, Cpu.AnalogChannel.ANALOG_3);
            tracker.Start();

           
        }

    }
}
