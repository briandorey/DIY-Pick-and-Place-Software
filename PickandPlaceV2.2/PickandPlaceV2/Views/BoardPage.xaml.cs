using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.Data;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace PickandPlaceV2.Pages
{
    /// <summary>
    /// Interaction logic for BoardPage.xaml
    /// </summary>
    public partial class BoardPage : Page
    {
        DataTable dtComponents = new DataTable();
        DataTable dtBoardInfo = new DataTable();

        DataTable dtLog = new DataTable();

        public DataSet dsData = new DataSet();
        DataHelpers dh = new DataHelpers();
        private PCBBuilder builder = new PCBBuilder();
        public App MyApplication = ((App)Application.Current);
        UsbDevice usbController;
        Kflop _kflop;
        public string LogFile = "";
        public int SelectedVideoMode = 0;

        // video processing 
        FilterInfoCollection videoDevices;
        
        VideoCaptureDevice videoHeadCam;
        VideoCaptureDevice videoBaseCam;


        public delegate void UpdateDataCallback(VisionLocationResult res);
        public bool isrunning = false;
        private Byte[] buffer = new Byte[1];
        public DataTable table = new DataTable();

        public BoardPage()
        {
            InitializeComponent();

            string savedir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//pickandplacelogs//";
            System.IO.Directory.CreateDirectory(savedir);

            LogFile = savedir + DateTime.Now.ToString("yyyyMMdd-HH-mm") + ".xml";

            dtLog.TableName = "boardcomponents";
            dtLog.Columns.Add("ComponentValue", typeof(string));
            dtLog.Columns.Add("Package", typeof(string));
            dtLog.Columns.Add("TapeFeeder", typeof(bool));
            dtLog.Columns.Add("FeederID", typeof(string));
            dtLog.Columns.Add("ComponentCode", typeof(int));
            dtLog.Columns.Add("Placed", typeof(int));
            dtLog.WriteXml(LogFile);


            SetupGridView(_dgComponents);

            SetupFeeders(_dgFeeders);

            CapturedImageBox.Source = new BitmapImage(new Uri("/Resources/testcard.png", UriKind.Relative));
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoHeadCam = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoHeadCam.NewFrame += new NewFrameEventHandler(ProcessFrame);

            try
            {
                videoBaseCam = new VideoCaptureDevice(videoDevices[1].MonikerString);
                videoBaseCam.NewFrame += new NewFrameEventHandler(ProcessFrame);
                videoBaseCam.SetCameraProperty(
                           CameraControlProperty.Exposure,
                           -9,
                           CameraControlFlags.Manual);
            }
            catch { }
            CaptureSelect.SelectedIndex = 0;

            App MyApplication = ((App)Application.Current);
            _kflop = MyApplication.GetKFlop();

            usbController = MyApplication.GetUSBDevice();

            builder.SetupPCBBuilder(_kflop, usbController, dsData, LogFile, CapturedImageBox, videoBaseCam);

            // video setup

        }
            private void SetResultsLabelText(string text)
            {

            }

            public void OnFragmentNavigation(FragmentNavigationEventArgs e)
            {
            }
            public void OnNavigatedFrom(NavigationEventArgs e)
            {
            }
            public void OnNavigatedTo(NavigationEventArgs e)
            {


            }
            public void OnNavigatingFrom(NavigatingCancelEventArgs e)
            {
            }



            private void Bt_HomeAll_Click(object sender, RoutedEventArgs e)
            {
                usbController.SetResetFeeder();
                _kflop.HomeAll();
            }

            private void Bt_Load_Click(object sender, RoutedEventArgs e)
            {

                while (dsData.Tables.Count > 0)
                {
                    DataTable table = dsData.Tables[0];
                    if (dsData.Tables.CanRemove(table))
                    {
                        dsData.Tables.Remove(table);
                    }
                }

                // Configure open file dialog box
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = ""; // Default file name
                dlg.DefaultExt = ".xml"; // Default file extension
                dlg.Filter = "Xml documents (.xml)|*.xml"; // Filter files by extension 

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    dsData.ReadXml(dlg.FileName);

                    dtComponents = dsData.Tables["Components"];

                    dtBoardInfo = dsData.Tables["BoardInfo"];


                    ItemTitle.Text = dsData.Tables["BoardInfo"].Rows[0][0].ToString();
                    csfeeder.Text = dsData.Tables["BoardInfo"].Rows[0][2].ToString();

                // populate feeders table
                Components comp = new Components();
                DataSet ds = comp.POPComponentsTable();

                DataView dvFeeders = new DataView(ds.Tables[0]);
                dvFeeders.Sort = "FeederID ASC";

                DataView dvFeederComponents = new DataView(dtComponents);



                



                for (int x = 0; x < dvFeeders.Count; x++)
                {
                    DataView dvfilter = new DataView(dtComponents);

                    dvfilter.RowFilter = "ComponentCode = " + dvFeeders[x]["ComponentCode"].ToString();
                    if (dvfilter.Count > 0)
                    {
                        dtLog.Rows.Add(dvFeeders[x]["ComponentValue"], dvFeeders[x]["Package"] , dvFeeders[x]["TapeFeeder"].ToString(), dvFeeders[x]["FeederID"], dvFeeders[x]["ComponentCode"], 0);
                    }
                    dvfilter.Dispose();


                }



                dtLog.WriteXml(LogFile);
               
                /*
                for (int x = 0; x < dvFeederComponents.Count; x++)
                {
                    DataView dvfilter = new DataView(ds.Tables[0]);

                    dvfilter.RowFilter = "ComponentCode = " + dvFeederComponents[x]["ComponentCode"].ToString();
                    if (dvfilter.Count > 0)
                    {
                        table.Rows.Add(dvfilter[0]["ComponentValue"], dvfilter[0]["Package"], dvfilter[0]["TapeFeeder"].ToString(), dvfilter[0]["FeederID"], dvfilter[0]["ComponentCode"]);
                    }
                    dvfilter.Dispose();


                }

    */
                //ComponentCode

                // ComponentValue
                //    FeederID
                //    TapeFeeder
                _dgFeeders.ItemsSource = dtLog.DefaultView;
            }




                _dgComponents.ItemsSource = dtComponents.DefaultView;

            }

            private void Bt_ChipFeeder_Click(object sender, RoutedEventArgs e)
            {
                usbController.RunVibrationMotor(int.Parse(csfeeder.Text));
            }

            private void Bt_CheckAll_Click(object sender, RoutedEventArgs e)
            {
                // toggle selection on datagrid checkboxes
                foreach (DataRow row in dsData.Tables["Components"].Rows)
                {

                    row["Pick"] = 1;
                }
                DataView dv = new DataView(dsData.Tables["Components"]);
                dv.RowFilter = "Pick = 1";
                lblInfo.Content = dv.Count.ToString() + " selected";
                dv.Dispose();
            }

            private void Bt_UnCheckAll_Click(object sender, RoutedEventArgs e)
            {
                // toggle selection on datagrid checkboxes
                foreach (DataRow row in dsData.Tables["Components"].Rows)
                {

                    row["Pick"] = 0;
                }
                DataView dv = new DataView(dsData.Tables["Components"]);
                dv.RowFilter = "Pick = 1";
                lblInfo.Content = dv.Count.ToString() + " selected";
                dv.Dispose();
            }



            private void Bt_eStop_Click(object sender, RoutedEventArgs e)
            {
                _kflop.EStop();
            }

            private void Bt_Stop_Click(object sender, RoutedEventArgs e)
            {
                builder.CancelBuildProcess();
            }

            private void Bt_Start_Click(object sender, RoutedEventArgs e)
            {
                if (MyApplication.checkHome())
                {
                    builder.ActivateBuildProcess(int.Parse(csfeeder.Text));
                }
                else
                {
                    MessageBox.Show("Home Error");
                }
                MyApplication.setHomed(false);
            }

            private void _dgComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)

            {
                DataView dv = new DataView(dsData.Tables["Components"]);
                dv.RowFilter = "Pick = 1";
                lblInfo.Content = dv.Count.ToString() + " selected";
                dv.Dispose();
            }


            private void SetupGridView(DataGrid dg)
            {

                dg.AutoGenerateColumns = false;
                dh.SetupTextColumn(dg, "RefDes", "ComponentName", false);
                dh.SetupTextColumn(dg, "PosX", "PlacementX", false);
                dh.SetupTextColumn(dg, "PosY", "PlacementY", false);
                dh.SetupTextColumn(dg, "Rotate", "PlacementRotate", false);
                dh.SetupTextColumn(dg, "Nozzle", "PlacementNozzle", false);
            
                dh.SetupCheckBoxColumn(dg, "Pick", "Pick", false);

            }
        private void SetupFeeders(DataGrid dg)
        {

            dg.AutoGenerateColumns = false;
            dh.SetupTextColumn(dg, "Feeder", "FeederID", false);
            dh.SetupTextColumn(dg, "Part", "ComponentValue", false);
            dh.SetupTextColumn(dg, "Package", "Package", false);
            
        }
     



            private void Bt_Save_Click(object sender, RoutedEventArgs e)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Xml file (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {

                    dsData.Tables["BoardInfo"].Rows[0][2] = csfeeder.Text;

                    System.IO.StreamWriter xmlSW = new System.IO.StreamWriter(saveFileDialog.FileName);
                    dsData.WriteXml(xmlSW, XmlWriteMode.WriteSchema);
                    xmlSW.Close();
                    MessageBox.Show("File Saved");
                }
            }

            public void SetlblActive(string val)
            {
                lblActive.Content = val;
            }

            private void Bt_ViewPCB_Click(object sender, RoutedEventArgs e)
            {
                pcbvisualiser pcb = new pcbvisualiser();
                pcb.LoadDataSet(dsData);
                pcb.Show();
            }

        // video code

        private void UpdateTable(double a, double b, double c, double d, double e)
        {

          
        }

        private void ProcessFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (videoBaseCam != null)
            {
                Bitmap image = (Bitmap)eventArgs.Frame.Clone();

                int totalwidth = 0;
                int totalheight = 0;
                Graphics g = Graphics.FromImage(image);

                if (image != null)
                {
                    // add cross hairs to image
                    totalwidth = image.Width;
                    totalheight = image.Height;
                    Pen whitepen = new Pen(Color.White, 1);
                    // draw center cross lines on image
                    g.DrawLine(whitepen, new System.Drawing.Point(0, totalheight / 2), new System.Drawing.Point(totalwidth, totalheight / 2));
                    g.DrawLine(whitepen, new System.Drawing.Point(totalwidth / 2, 0), new System.Drawing.Point(totalwidth / 2, totalheight));

                    CapturedImageBox.Dispatcher.BeginInvoke(new Action(() =>
                        CapturedImageBox.Source = MakeBS(image)));

                    whitepen.Dispose();
                }

            }
        }

        private BitmapSource MakeBS(Bitmap img)
        {
            //  try
            // {
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
            // }
            //  catch
            //  {
            //     return null;
            //  }
        }

        public void UpdateCam(VisionLocationResult res)
        {


        }
        public void RunUpdateCam(VisionLocationResult res)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                CapturedImageBox.Source = MakeBS(res.Image);
            }));
        }
      

        private void CaptureSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int itemselected = CaptureSelect.SelectedIndex;
            if (itemselected == 0)
            {
                if (videoBaseCam != null && videoBaseCam.IsRunning)
                {
                    videoBaseCam.SignalToStop();
                }
                if (videoHeadCam != null && videoHeadCam.IsRunning)
                {
                    videoHeadCam.SignalToStop();
                }
                isrunning = false;
                SelectedVideoMode = 0;
                CapturedImageBox.Source = new BitmapImage(new Uri("/Resources/testcard.png", UriKind.Relative));
            }
            if (itemselected == 1)
            {
                videoBaseCam.Start();
                isrunning = true;
                SelectedVideoMode = 1;
            }
            if (itemselected == 2)
            {
               videoHeadCam.Start();
                isrunning = true;
                SelectedVideoMode = 2;
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            
            if (videoBaseCam != null && videoBaseCam.IsRunning)
            {
                videoBaseCam.SignalToStop();
            }
            if (videoHeadCam != null && videoHeadCam.IsRunning)
            {
                videoHeadCam.SignalToStop();
            }

        }
    }
    }

