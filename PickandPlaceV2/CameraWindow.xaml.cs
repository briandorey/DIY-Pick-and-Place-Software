using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PickandPlaceV2
{
    /// <summary>
    /// Interaction logic for CameraWindow.xaml
    /// </summary>
    public partial class CameraWindow : Window
    {
        private Mat cap;
        private Image<Bgr, Byte> frame;
        private VideoCapture capture = null;
        private DispatcherTimer timer;
        private bool isrunning = false;
        private Byte[] buffer = new Byte[1];
       
        public CameraWindow()
        {
            InitializeComponent();
            InitializeCameras();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(ProcessFrame);
            timer.Interval = new TimeSpan(0, 0, 0, 0,25);

           
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!isrunning)
            {

                timer.Start();
                button1.Content = "Stop Camera";

                isrunning = true;
            }
            else
            {
                timer.Stop();
                button1.Content = "Start Camera";

                isrunning = false;
            }
        }


        private void InitializeCameras()
        {
            if (capture == null)
            {
                try
                {
                    capture = new VideoCapture(0);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }

       
        private void ProcessFrame(object sender, EventArgs arg)
        {
           
            cap = capture.QueryFrame();
            int totalwidth = 0;
            int totalheight = 0;

            using (Image<Bgr, Byte> frame = capture.QueryFrame().ToImage<Bgr, Byte>())
            {
                if (frame != null)
                {

                    // add cross hairs to image
                    totalwidth = frame.Width;
                    totalheight = frame.Height;
                    PointF[] linepointshor = new PointF[] {
                    new PointF(0, totalheight/2),
                    new PointF(totalwidth, totalheight/2)

                };
                    PointF[] linepointsver = new PointF[] {
                    new PointF(totalwidth/2, 0),
                    new PointF(totalwidth/2, totalheight)

                };

                    frame.DrawPolyline(Array.ConvertAll<PointF, System.Drawing.Point>(linepointshor, System.Drawing.Point.Round), false, new Bgr(System.Drawing.Color.AntiqueWhite), 1);
                    frame.DrawPolyline(Array.ConvertAll<PointF, System.Drawing.Point>(linepointsver, System.Drawing.Point.Round), false, new Bgr(System.Drawing.Color.AntiqueWhite), 1);


                    CapturedImageBox.Source = BitmapSourceConvert.ToBitmapSource(frame);
                }
            }
          
        }
    }
    public static class BitmapSourceConvert
    {
        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }
    }
}
