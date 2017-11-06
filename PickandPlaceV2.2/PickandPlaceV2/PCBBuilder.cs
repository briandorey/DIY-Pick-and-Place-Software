using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using AForge.Video.DirectShow;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using AForge.Video;
using System.Windows.Threading;

namespace PickandPlaceV2
{
    public class PCBBuilder
    {
        public DataSet dsData = new DataSet();

        DataTable dtLog = new DataTable();
        string _LogFile = "";

        private Components comp = new Components();
        private UsbDevice usbController;
        private Kflop kf;
        public KflopLocation kfl = new KflopLocation(0,100,100,0,0,0,0,0,0,0,false, false);
        private System.Windows.Controls.Image img;

        // manual picker selector
        public int currentfeeder = 0;
        int MotorRunLoop = 20;
        // bed settings
        public double NeedleZHeight = 34.9;

        DataHelpers dh = new DataHelpers();


        public double dblPCBThickness = 1.6;
        public int FeedRate = 5000;

        public double ClearHeight = 15;

        // nozzle to camera offsets
        public double Nozzle1Xoffset = 0;
        public double Nozzle1Yoffset = 0;

        public double Nozzle2Xoffset = 0;
        public double Nozzle2Yoffset = 0;

        VideoCaptureDevice videoCam;
        private VisionProcessing visionLocation = new VisionProcessing();
        VisionLocationResult outstr;
        private bool VisionCorrectionNeeded = false;

        private readonly BackgroundWorker backgroundWorkerBuildPCB = new BackgroundWorker();

        public void SetupPCBBuilder(Kflop kflop, UsbDevice usb, DataSet ds, string LogFile,
            System.Windows.Controls.Image imgref, VideoCaptureDevice videoBaseCam)
        {
            _LogFile = LogFile;
            dsData = ds;
            kf = kflop;
            usbController = usb;
            img = imgref;
            dtLog.ReadXml(LogFile);
            videoCam = videoBaseCam;

            // set NewFrame event handler
            try
            {
                videoCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
            }
            catch { }

            // setup worker methods
            backgroundWorkerBuildPCB.DoWork += Worker_DoWork;
            backgroundWorkerBuildPCB.RunWorkerCompleted += Worker_RunWorkerCompleted;
            backgroundWorkerBuildPCB.WorkerSupportsCancellation = true;


        }


        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            BackgroundWorker worker = sender as BackgroundWorker;
            DataTable dtComponents = dsData.Tables["Components"];
            DataView dv = new DataView(dtComponents);
            dv.RowFilter = "Pick = 1";
            int currentrow = 0;
            int totalrows = dv.Count;

            Nozzle1Xoffset = dh.Nozzle1Xoffset;
            Nozzle1Yoffset = dh.Nozzle1Yoffset;
            Nozzle2Xoffset = dh.Nozzle2Xoffset;
            Nozzle2Yoffset = dh.Nozzle2Yoffset;

            if (totalrows > 0)
            {

                /* Components table columns
                    ComponentCode
                    ComponentName
                    PlacementX
                    PlacementY
                    PlacementRotate
                    PlacementNozzle
                 
                 * component list
                 * ComponentCode
                 * ComponentValue
                 * Package
                 * 
                 * PlacementHeight
                 * FeederHeight
                 * FeederX
                 * FeederY
                 * VerifywithCamera
                 * TapeFeeder
               
                  
                 * */
                double pcbHeight = double.Parse(dsData.Tables["BoardInfo"].Rows[0][1].ToString());

                double feedrate = 100;
               // double placerate = 20000;
               // double feederPosX = 0;
                //double feederPosY = 0;
                //double feederPosZ = 0;
                //double placePosX = 0;
                //double placePosY = 0;
                //double ComponentRotation = 0;
                //double PlacementHeight = 0;
                //int PlacementNozzle = 1;
                //double PlaceSpeed = 100;

               // bool TapeFeeder = false;
               // bool VerifyCamera = false;

                

                while (currentrow < totalrows)
                {

                    if (backgroundWorkerBuildPCB.CancellationPending)
                    {
                        e.Cancel = true;
                        dv.Dispose();
                        break;
                    }
                   
                   

                    kfl.PlacementNozzle = int.Parse(dv[currentrow]["PlacementNozzle"].ToString());
                    kfl.PickSpeed = feedrate;
                    kfl.PlaceSpeed = comp.GetPlaceSpeed(dv[currentrow]["ComponentCode"].ToString(), feedrate);

                    kfl.FeederX = CalcXLocation(comp.GetFeederX(dv[currentrow]["ComponentCode"].ToString()), kfl.PlacementNozzle);
                    kfl.FeederY = CalcYLocation(comp.GetFeederY(dv[currentrow]["ComponentCode"].ToString()), kfl.PlacementNozzle);
                    kfl.FeederHeight = comp.GetFeederHeight(dv[currentrow]["ComponentCode"].ToString());

                    kfl.PlaceHeight = comp.GetPlacementHeight(dv[currentrow]["ComponentCode"].ToString()) - pcbHeight;

                    kfl.TapeFeeder = comp.GetComponentTapeFeeder(dv[currentrow]["ComponentCode"].ToString());

                    kfl.PlaceX = CalcXLocation(double.Parse(dv[currentrow]["PlacementX"].ToString()), kfl.PlacementNozzle);
                    kfl.PlaceY = CalcYLocation(double.Parse(dv[currentrow]["PlacementY"].ToString()), kfl.PlacementNozzle);
                    kfl.PlaceRotation = double.Parse(dv[currentrow]["PlacementRotate"].ToString());

                    kfl.VerifyCamera = comp.GetComponentVerifywithCamera(dv[currentrow]["ComponentCode"].ToString());
                    
                    //feederPosX = CalcXLocation(comp.GetFeederX(dv[currentrow]["ComponentCode"].ToString()), PlacementNozzle);
                    //double feederPosY = CalcYLocation(comp.GetFeederY(dv[currentrow]["ComponentCode"].ToString()), PlacementNozzle);
                    //feederPosZ = comp.GetFeederHeight(dv[currentrow]["ComponentCode"].ToString());
                    // PlacementHeight = comp.GetPlacementHeight(dv[currentrow]["ComponentCode"].ToString()) - pcbHeight;
                    //TapeFeeder = comp.GetComponentTapeFeeder(dv[currentrow]["ComponentCode"].ToString());
                    // placePosX = CalcXLocation(double.Parse(dv[currentrow]["PlacementX"].ToString()), kfl.PlacementNozzle);
                    //placePosY = CalcYLocation(double.Parse(dv[currentrow]["PlacementY"].ToString()), kfl.PlacementNozzle);
                    //ComponentRotation = double.Parse(dv[currentrow]["PlacementRotate"].ToString());
                    //VerifyCamera = comp.GetComponentVerifywithCamera(dv[currentrow]["ComponentCode"].ToString());

                    if (currentrow == 0)
                    {
                        SetFeederOutputs(comp.GetFeederID(dv[currentrow]["ComponentCode"].ToString())); // send feeder to position
                    }
                    //MessageBox.Show(kfl.FeederY.ToString());
                    //kf.MoveSingleFeed(kfl.PickSpeed, 100, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);

                    kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);

                   
                    if (comp.GetComponentTapeFeeder(dv[currentrow]["ComponentCode"].ToString()))
                    {

                        while (!usbController.GetFeederReadyStatus())
                        {
                            Thread.Sleep(10);
                        }
                        Thread.Sleep(50);
                        if (kfl.PlacementNozzle == 1)
                        {
                            // use picker 1
                            kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, kfl.FeederHeight, ClearHeight, 0, 0);
                            Thread.Sleep(200);
                            // go down and turn on suction
                            usbController.SetVAC1(true);
                            Thread.Sleep(150);
                            kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);
                        } else
                        {
                            // nozzle 2 on tape feeder
                            kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, kfl.FeederHeight, 0, 0);
                            Thread.Sleep(200);
                            // go down and turn on suction
                            usbController.SetVAC2(true);
                            Thread.Sleep(150);
                            kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);
                        }

                    }
                    else
                    {
                        // use picker 2
                        while (usbController.CheckChipMotorRunning())
                        {
                            Thread.Sleep(10);
                        }
                        kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, kfl.FeederHeight, 0, 0);
                        Thread.Sleep(200);

                        usbController.SetVAC2(true);
                        Thread.Sleep(300);
                        kf.MoveSingleFeed(kfl.PickSpeed, kfl.FeederX, kfl.FeederY, ClearHeight, ClearHeight, 0, 0);
                    }
                    // send picker to pick next item
                    if (currentrow >= 0 && (currentrow + 1) < totalrows)
                    {
                        Thread.Sleep(100);
                        Thread.Sleep(100);


                        SetFeederOutputs(comp.GetFeederID(dv[currentrow + 1]["ComponentCode"].ToString())); // send feeder to position
                    }

                    
                    // check if place speed needs to go slower

                  
                    // rotate head and place component

                    //SetResultsLabelText("Placing Component");
                    if (comp.GetComponentTapeFeeder(dv[currentrow]["ComponentCode"].ToString()) && (kfl.PlacementNozzle == 1))
                    {


                        /*
                        if (kfl.VerifyCamera)
                        {
                            kfl = CheckWithCamera(kfl, kf, 1, usbController, img);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, ClearHeight, 0, kfl.PlaceRotation);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, kfl.PlaceHeight, ClearHeight, 0, kfl.PlaceRotation);
                            
                        } else
                        {
                        */
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, ClearHeight, 0, kfl.PlaceRotation);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, kfl.PlaceHeight, ClearHeight, 0, kfl.PlaceRotation);

                       // }
                         Thread.Sleep(200);
                            usbController.SetVAC1(false);
                            Thread.Sleep(50);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, ClearHeight, 0, kfl.PlaceRotation);

                    }
                    else
                    {
                        // use picker 2  CalcXwithNeedleSpacing
                        /*
                        if (kfl.VerifyCamera)
                        {
                            kfl = CheckWithCamera(kfl, kf, 2, usbController, img);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, ClearHeight, kfl.PlaceRotation, 0);
                            Thread.Sleep(200);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, kfl.PlaceHeight, kfl.PlaceRotation, 0);
                        }
                        else
                        {*/
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, ClearHeight, kfl.PlaceRotation, 0);
                            Thread.Sleep(200);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, kfl.PlaceHeight, kfl.PlaceRotation, 0);
                           
                        //}
                        
                         // go down and turn off suction
                            Thread.Sleep(300);
                            usbController.SetVAC2(false);
                            Thread.Sleep(200);
                            kf.MoveSingleFeed(kfl.PlaceSpeed, kfl.PlaceX, kfl.PlaceY, ClearHeight, ClearHeight, kfl.PlaceRotation, 0);

                    }
                   
                    currentrow++;

                }
                // move near to home point
                kf.MoveSingleFeed(kfl.PickSpeed, 10, 10, 10, 10, 0, 0);

            }
            else
            {
                MessageBox.Show("Board file not loaded");
            }
            backgroundWorkerBuildPCB.CancelAsync();
            usbController.SetResetFeeder();
            usbController.RunVibrationMotor(MotorRunLoop);
            kf.RunHomeAll();

            //dtLog.WriteXml(_LogFile);

            dv.Dispose();
            dtComponents.Dispose();

        }

        private void Worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
        }
        public KflopLocation CheckWithCamera(KflopLocation kfl, Kflop kf, int nozzle, UsbDevice usbController, System.Windows.Controls.Image imgref)
        {
            double CameraX = 295.4;
            double CameraY = 150.2;

            double CameraX2 = 263.5;
            double CameraY2 = 150.2;

            double PlaceX = kfl.PlaceX;
            double PlaceY = kfl.PlaceY;
            double PlaceRotation = kfl.PlaceRotation;


            int retrycounter = 0;


            img = imgref;
            outstr.Loc1X = 0;
            videoCam.Start();

            if (nozzle == 1)
            {
                kf.MoveSingleFeed(kfl.PlaceSpeed, CameraX, CameraY, ClearHeight, ClearHeight, 0, kfl.PlaceRotation);
                usbController.SetHeadCameraLED(true);
            }
            else
            {

                kf.MoveSingleFeed(kfl.PlaceSpeed, CameraX2, CameraY2, ClearHeight, ClearHeight, kfl.PlaceRotation, 0);
                usbController.SetHeadCameraLED(true);
            }
            Thread.Sleep(2000);
            // verify camera image if needed
            
            // loop until part has correct rotation
            while (VisionCorrectionNeeded && retrycounter < 100)
            {
                if (outstr.OffsetX < 0.5 && outstr.OffsetY < 0.5)
                {
                    PlaceX = PlaceX + outstr.OffsetX;
                    PlaceY = PlaceY + outstr.OffsetY;
                    PlaceRotation = PlaceRotation + outstr.LocAngle;

                    VisionCorrectionNeeded = true;
                } else
                {
                    if (nozzle == 1)
                    {
                        PlaceX = PlaceX + outstr.OffsetX;
                        PlaceY = PlaceY + outstr.OffsetY;
                        PlaceRotation = PlaceRotation + outstr.LocAngle;
                        CameraX = CameraX + outstr.OffsetX;
                        CameraY = CameraY + outstr.OffsetY;

                        kf.MoveSingleFeed(kfl.PlaceSpeed, CameraX, CameraY, ClearHeight, ClearHeight, 0, PlaceRotation);
                       
                    }
                    else
                    {
                        PlaceX = PlaceX + outstr.OffsetX;
                        PlaceY = PlaceY + outstr.OffsetY;
                        PlaceRotation = PlaceRotation + outstr.LocAngle;
                        CameraX2 = CameraX2 + outstr.OffsetX;
                        CameraX2 = CameraX2 + outstr.OffsetY;


                        kf.MoveSingleFeed(kfl.PlaceSpeed, CameraX2, CameraY2, ClearHeight, ClearHeight, kfl.PlaceRotation, 0);
                    }
                    VisionCorrectionNeeded = false;
                }
                outstr.Loc1X = 0;
                Thread.Sleep(1000);
                retrycounter++;
            }




            usbController.SetHeadCameraLED(false);
            if (videoCam != null && videoCam.IsRunning)
            {
                videoCam.SignalToStop();
            }
            return kfl;
        }

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                /*
                if (framecounter >= 30)
                {
                    if (videoCam != null && videoCam.IsRunning)
                    {
                        videoCam.SignalToStop();
                        // videoSource.WaitForStop();
                        //videoSource = null;
                    }
                    framecounter = 0;
                }
                framecounter++;
                */

                Bitmap image = (Bitmap)eventArgs.Frame.Clone();

                visionLocation.SetCurrentImage(image);
                visionLocation.ApplyConvertToGrayscale();
                //visionLocation.applySobelEdgeFilter();
                visionLocation.ApplyCannyEdgeDetector();
                outstr = visionLocation.LocateObjects();
               // data.Dispatcher.Invoke(
                //    new UpdateTextCallback(this.UpdateTable),
                //    new object[] { outstr.Loc1X, outstr.Loc1Y, outstr.Loc2X, outstr.Loc2Y, outstr.Loc3X, outstr.Loc3Y, //outstr.Loc4X, outstr.Loc4Y, outstr.ItemWidth, outstr.ItemHeight, outstr.LocAngle, outstr.LocText, //outstr.OffsetX, outstr.OffsetY }
               // );


                img.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    img.Source = MakeBS(visionLocation.GetCurrentImage());
                }));

                /*
                if (outstr != null && outstr.Loc1X > 0)
                {
                    if (videoCam != null && videoCam.IsRunning)
                    {
                        videoCam.SignalToStop();
                        // videoSource.WaitForStop();
                        //videoSource = null;
                    }
                    framecounter = 0;
                }
                */

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        public void UpdateComponents(string componentcode)
        {
            foreach (DataRow row in dtLog.Rows)
            {
                if (row["ComponentCode"].ToString() == componentcode) 
                {
                    row["Placed"] = Int32.Parse(row["Placed"].ToString()) + 1;
                }
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
                usbController.RunVibrationMotor(MotorRunLoop);
            }
        }



        public void ActivateBuildProcess(int motorruntime)
        {
            kf.SetAlltoZero();
            MotorRunLoop = motorruntime;
            if (backgroundWorkerBuildPCB.IsBusy != true)
            {
                backgroundWorkerBuildPCB.RunWorkerAsync();
            }
        }
        public void CancelBuildProcess()
        {
            backgroundWorkerBuildPCB.CancelAsync();

        }

        private BitmapSource MakeBS(Bitmap img)
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                //MemoryStream ms;
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();

                return bi;
            }
            catch
            {
                return null;
            }
        }
    }

}
