using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Threading;
using System;

namespace PickandPlaceV2.Views
{
    /// <summary>
    /// Interaction logic for ManualControl.xaml
    /// </summary>
    public partial class ManualControl : Page
    {
        UsbDevice usbController;
        Kflop _kflop;
        DataSet dt = new DataSet();
        Components comp = new Components();
        public ManualControl()
        {
            InitializeComponent();
            App MyApplication = ((App)Application.Current);
            _kflop = MyApplication.GetKFlop();

            usbController = MyApplication.GetUSBDevice();


            DataSet dsComponents = new DataSet();
            dsComponents = comp.POPComponentsTable();
            dd_ComponentSelect.DisplayMemberPath = "ComponentValue";
            dd_ComponentSelect.SelectedValuePath = "ComponentCode";
            dd_ComponentSelect.ItemsSource = dsComponents.Tables[0].DefaultView;
            dd_ComponentSelect.SelectedIndex = 0;
        }


        public void SetText(TextBox control, string text)
        {
            control.Text = text;
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            CameraWindow cam = new CameraWindow();

            cam.Show();
        }






        private void bt_MoveYMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("Y", -jogspeed);
        }


        private void bt_MoveYPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("Y", jogspeed);
        }


        private void bt_MoveXMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("X", -jogspeed);
        }


        private void bt_MoveXPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("X", jogspeed);
        }

        private void bt_MoveZMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("Z", -jogspeed);
        }


        private void bt_MoveZPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("Z", jogspeed);
        }

        private void bt_MoveAMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("A", -jogspeed);
        }


        private void bt_MoveAPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("A", jogspeed);
        }

        private void bt_MoveBMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("B", -jogspeed);
        }


        private void bt_MoveBPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("B", jogspeed);
        }

        private void bt_MoveCMinus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("C", -jogspeed);
        }


        private void bt_MoveCPlus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_distance.SelectedItem;
            string value = typeItem.Content.ToString();
            double jogspeed = double.Parse(typeItem.Content.ToString());
            _kflop.JogAxis("C", jogspeed);
        }

        private void bt_MoveStop(object sender, MouseButtonEventArgs e)
        {
            _kflop.JogAxis("X", 0);
            _kflop.JogAxis("Y", 0);
            _kflop.JogAxis("Z", 0);
            _kflop.JogAxis("A", 0);
            _kflop.JogAxis("B", 0);
            _kflop.JogAxis("C", 0);

        }

        private void UpdateDRO()
        {
            double _x = 0;
            double _y = 0;
            double _z = 0;
            double _a = 0;
            double _b = 0;
            double _c = 0;

            _kflop.GetDRO(out _x, out _y, out _z, out _a, out _b, out _c);
            txt_CameraX.Text = _x.ToString();
            txt_CameraY.Text = _y.ToString();
            txt_CameraZ.Text = _z.ToString();
            txt_CameraA.Text = _a.ToString();
            txt_CameraB.Text = _b.ToString();
            txt_CameraC.Text = _c.ToString();
        }

        private void bt_HomeAll_Click(object sender, RoutedEventArgs e)
        {
            _kflop.HomeAll();
            usbController.SetResetFeeder();
        }

        private void bt_GetDRO_Click(object sender, RoutedEventArgs e)
        {
            UpdateDRO();
        }

        // head led controller
        private void chk_HeadLED_Checked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetBaseCameraLED(true);
            }
        }

        private void chk_HeadLED_Unchecked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetBaseCameraLED(false);
            }
        }
        // base led controller
        private void chk_BaseLED_Checked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetHeadCameraLED(true);
            }
        }

        private void chk_BaseLED_Unchecked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetHeadCameraLED(false);
            }
        }
        // vac 1
        private void chk_Vac1_Checked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetVAC1(true);
            }
        }

        private void chk_Vac1_Unchecked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetVAC1(false);
            }
        }
        // vac 2
        private void chk_Vac2_Checked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetVAC2(true);
            }
        }

        private void chk_Vac2_Unchecked(object sender, RoutedEventArgs e)
        {
            if (usbController != null)
            {
                usbController.SetVAC2(false);
            }
        }

        private void bt_getFeeder_Click_1(object sender, RoutedEventArgs e)
        {
            ComboBoxItem typeItem = (ComboBoxItem)dd_FeederSelect.SelectedItem;
            string value = typeItem.Content.ToString();
            byte feeder = byte.Parse(typeItem.Content.ToString());
            usbController.SetGotoFeeder(feeder);
        }

        private void bt_eStop_Click(object sender, RoutedEventArgs e)
        {
            _kflop.EStop();
        }

        private void bt_ChipFeeder_Click(object sender, RoutedEventArgs e)
        {
            usbController.RunVibrationMotor(25);
        }

        private void bt_runto_Click(object sender, RoutedEventArgs e)
        {
            double newX = double.Parse(txt_goX.Text);
            double newY = double.Parse(txt_goY.Text);
            double newZ = double.Parse(txt_goZ.Text);
            double newA = double.Parse(txt_goA.Text);
            double newB = double.Parse(txt_goB.Text);
            double newC = double.Parse(txt_goC.Text);
            double newSpeed = double.Parse(txt_Speed.Text);


            ThreadStart starter = () => RunToPoint(newX, newY, newZ, newA, newB, newC, newSpeed);
            Thread runner = new Thread(starter);
            runner.Start();
        }

        private void RunToPoint(double newX, double newY, double newZ, double newA, double newB, double newC, double newSpeed)
        {
            _kflop.MoveSingleFeed(newSpeed, newX, newY, newZ, newA, newB, newC);
        }

        private void mainframe_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            int currentitem = dd_distance.SelectedIndex;
            int maaxitems = dd_distance.Items.Count;
            if (e.Delta > 0)
            {
                if (currentitem > 0)
                {
                    dd_distance.SelectedIndex = currentitem - 1;
                }
                else
                {
                    dd_distance.SelectedIndex = 0;
                }
            }
            else
            {
                if (currentitem < maaxitems)
                {
                    dd_distance.SelectedIndex = currentitem + 1;
                }
            }

        }

        public KflopLocation kfl = new KflopLocation(0, 100, 100, 0, 0, 0, 0, 0, 0, 0, false, false);
        public double Nozzle1Xoffset = 0;
        public double Nozzle1Yoffset = 0;

        public double Nozzle2Xoffset = 0;
        public double Nozzle2Yoffset = 0;
        public double ClearHeight = 15;


        // run to selected component
        private void bt_selcomp_Click(object sender, RoutedEventArgs e)
        {
            DataHelpers dh = new DataHelpers();
            Nozzle1Xoffset = dh.Nozzle1Xoffset;
            Nozzle1Yoffset = dh.Nozzle1Yoffset;
            Nozzle2Xoffset = dh.Nozzle2Xoffset;
            Nozzle2Yoffset = dh.Nozzle2Yoffset;


            Int32 ComponentCode = Int32.Parse(dd_ComponentSelect.SelectedValue.ToString());
            string ComponentName = comp.GetComponentValue(dd_ComponentSelect.SelectedValue.ToString());

            kfl.PlacementNozzle = ComponentCode;
            kfl.PickSpeed = 50;
            kfl.PlaceSpeed = comp.GetPlaceSpeed(ComponentCode.ToString(), 50);

            kfl.FeederX = CalcXLocation(comp.GetFeederX(ComponentCode.ToString()), kfl.PlacementNozzle);
            kfl.FeederY = CalcYLocation(comp.GetFeederY(ComponentCode.ToString()), kfl.PlacementNozzle);
            kfl.FeederHeight = comp.GetFeederHeight(ComponentCode.ToString());

            kfl.PlaceHeight = comp.GetPlacementHeight(ComponentCode.ToString());

            kfl.TapeFeeder = comp.GetComponentTapeFeeder(ComponentCode.ToString());

            kfl.PlaceX = 0;
            kfl.PlaceY = 0;
            kfl.PlaceRotation = 0;

            kfl.VerifyCamera = false;


            SetFeederOutputs(comp.GetFeederID(ComponentCode.ToString())); // send feeder to position

            _kflop.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);


            if (comp.GetComponentTapeFeeder(ComponentCode.ToString()))
            {

                while (!usbController.GetFeederReadyStatus())
                {
                    Thread.Sleep(10);
                }
                Thread.Sleep(50);
                if (kfl.PlacementNozzle == 1)
                {
                    // use picker 1
                    _kflop.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, kfl.FeederHeight, ClearHeight, 0, 0);
                   // Thread.Sleep(200);
                    // go down and turn on suction
                   // usbController.SetVAC1(true);
                   // Thread.Sleep(150);
                   // _kflop.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);
                }
                else
                {
                    // nozzle 2 on tape feeder
                    _kflop.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, kfl.FeederHeight, 0, 0);
                    Thread.Sleep(200);
                    // go down and turn on suction
                    //usbController.SetVAC2(true);
                    //Thread.Sleep(150);
                    //_kflop.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);
                }

            }
            else
            {
                // use picker 2
                while (usbController.CheckChipMotorRunning())
                {
                    Thread.Sleep(10);
                }
                _kflop.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, kfl.FeederHeight, 0, 0);
               // Thread.Sleep(200);

               // usbController.SetVAC2(true);
               // Thread.Sleep(300);
               // kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);
            }
            

        }

        public double CalcXLocation(double val, int nozzle)
        {
            if (nozzle.Equals(1))
            {
                return val - Nozzle1Xoffset;
            }
            else
            {
                return val - Nozzle2Xoffset;
            }
        }
        public double CalcYLocation(double val, int nozzle)
        {
            if (nozzle.Equals(1))
            {
                return val - Nozzle1Yoffset;
            }
            else
            {
                return val - Nozzle2Yoffset;
            }
        }

        public void SetFeederOutputs(int feedercommand)
        {
            usbController.SetGotoFeeder(Byte.Parse(feedercommand.ToString()));

            // check if on main feeder rack
            if (feedercommand == 98)
            {
                // command set, now toggle interupt pin
                usbController.SetResetFeeder();
            }
            // if (comp.GetFeederID(feedercommand.ToString()) > 20) {
            //     usbController.RunVibrationMotor(MotorRunLoop);
            // }
            if (feedercommand >= 20 && feedercommand < 30)
            {
                usbController.RunVibrationMotor(20);
            }
        }
    }


}
