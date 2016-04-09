using System.Threading;
using DemoSat16.Utility;
using Microsoft.SPOT.Hardware;

namespace DemoSat16.Work_Items {
    public class GoPro {

        private readonly WorkItem _cameraAction;
        private readonly OutputPort _snapPin;
        public GoPro(Cpu.Pin picturePin) {
            _snapPin = new OutputPort(picturePin, true);
            _cameraAction = new WorkItem(PictureTime);    
        }

        private void PictureTime() {
            takePicture();
            Thread.Sleep(1000);
        }

        private void takePicture() {
            _snapPin.Write(false);
            Thread.Sleep(250);
            _snapPin.Write(true);
        }
        public void Start() {
            FlightComputer.Instance.Execute(_cameraAction);
        }
    }
}