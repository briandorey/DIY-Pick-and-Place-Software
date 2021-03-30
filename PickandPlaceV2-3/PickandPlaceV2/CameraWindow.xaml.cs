using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace PickandPlaceV2
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class CameraWindow : Window
    {
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;

        
        private bool _captureInProgress = false;



        public CameraWindow()
        {
            InitializeComponent();


            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo VideoCaptureDe in videoDevices)
                {
                    camera.Items.Add(VideoCaptureDe.Name);
                }
                camera.SelectedIndex = 0;  
                
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }

        }

        private void ProcessFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (videoSource != null)
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

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
           
                if (_captureInProgress)
                {  //stop the capture
                    button1.Content = "Start Capture";
                    videoSource.SignalToStop();
                }
                else
                {
                    //start the capture
                    button1.Content = "Stop";
                    videoSource = new VideoCaptureDevice(videoDevices[camera.SelectedIndex].MonikerString);
                    //videoSource.SetCameraProperty(
                    //         CameraControlProperty.Exposure,
                    //        -9,
                    //        CameraControlFlags.Manual);
                    videoSource.NewFrame += new NewFrameEventHandler(ProcessFrame);
                    videoSource.Start();
                }

                _captureInProgress = !_captureInProgress;
            



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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                //     videoSource.WaitForStop();
                videoSource = null;
            }

        }

        private void Camera_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
            }
         

        }
    }

}