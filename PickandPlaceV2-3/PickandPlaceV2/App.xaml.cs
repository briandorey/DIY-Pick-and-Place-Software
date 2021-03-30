using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PickandPlaceV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public UsbDevice usbController;
        public Kflop _kflop = new Kflop();
        public FilterInfoCollection videoDevices;
        public VideoCaptureDevice videoSource;

        public bool _hasHomed = false;

        void App_Startup(object sender, StartupEventArgs e)
        {
            _hasHomed = false;

            try
            {
                _kflop.InitDevice();
                usbController = new UsbDevice(0x04D8, 0x0042);
                usbController.usbEvent += new UsbDevice.usbEventsHandler(usbEvent_receiver);
                usbController.findTargetDevice();
                usbController.RunBoardInit(false, 250, 250);
            }
            catch {
                MessageBox.Show("Error connecting to controller");

            }

   
        }
        public bool checkHome()
        {
            return _hasHomed;

        }
        public void setHomed(bool status)
        {
            _hasHomed = status;

        }
        public Kflop GetKFlop()
        {

            return _kflop;
        }
        public UsbDevice GetUSBDevice()
        {
            return usbController;
        }
        public DataComponentFeeders cf = new DataComponentFeeders();
        public Components comp = new Components();

        public PCBBuilder pcbBuilder = new PCBBuilder();
        private void usbEvent_receiver(object o, EventArgs e)
        {
            // Check the status of the USB device and update the form accordingly
            if (usbController.isDeviceAttached)
            {
                // Device is currently attached


            }
            else
            {
                // Device is currently unattached

                // Update the status label

            }
        }
    }
}
