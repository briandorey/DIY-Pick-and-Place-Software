using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using AForge.Math;
using System.Data;
using System.Windows;

namespace PickandPlaceV2
{
    class VisionProcessing
    {
        private Bitmap currentImage;

        public VisionProcessing(Bitmap currentImage)
        {
            this.currentImage = currentImage;
        }

        public VisionProcessing()
        {
            this.currentImage = null;
        }

        public DataTable MakeDataTable(DataTable dt)
        {
            dt.Columns.Add("Loc1X", typeof(double));
            dt.Columns.Add("Loc1Y", typeof(double));
            dt.Columns.Add("Loc2X", typeof(double));
            dt.Columns.Add("Loc2Y", typeof(double));
            dt.Columns.Add("Loc3X", typeof(double));
            dt.Columns.Add("Loc3Y", typeof(double));
            dt.Columns.Add("Loc4X", typeof(double));
            dt.Columns.Add("Loc4Y", typeof(double));
            dt.Columns.Add("ItemWidth", typeof(double));
            dt.Columns.Add("ItemHeight", typeof(double));
            dt.Columns.Add("LocAngle", typeof(double));
            dt.Columns.Add("OffsetX", typeof(double));
            dt.Columns.Add("OffsetY", typeof(double));
            dt.Columns.Add("Text", typeof(string));

            return dt;
        }

        public bool ApplySobelEdgeFilter()
        {
            if (currentImage != null)
            {
                try
                {
                    SobelEdgeDetector filter = new SobelEdgeDetector();
                    filter.ApplyInPlace(currentImage);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            return false;
        }

        public bool ApplyCannyEdgeDetector()
        {
            if (currentImage != null)
            {
                try
                {
                    CannyEdgeDetector filter = new CannyEdgeDetector();
                    filter.ApplyInPlace(currentImage);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            return false;
        }


        public bool ApplyConvertToGrayscale()
        {
            if (currentImage != null)
            {
                try
                {
                    // create grayscale filter (BT709)
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    currentImage = filter.Apply(currentImage);
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("applyConvertToGrayscale" + e.ToString());
                }
            }
            return false;
        }

        public VisionLocationResult LocateObjects()
        {
            VisionLocationResult ld = new VisionLocationResult(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, null);
            int totalwidth = 0;
            int totalheight = 0;
            System.Drawing.Point centerpoint = new System.Drawing.Point();


            if (currentImage != null)
            {
                try
                {
                    Bitmap image = new Bitmap(this.currentImage);

                    totalwidth = image.Width;
                    totalheight = image.Height;

                    centerpoint.X = image.Width / 2;
                    centerpoint.Y = image.Height / 2;


                    // lock image
                    BitmapData bmData = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadWrite, image.PixelFormat);



                    // turn background to black
                    ColorFiltering cFilter = new ColorFiltering(red: new IntRange(0,64), green: new IntRange(0, 64), blue: new IntRange(0, 64));
                    //cFilter.Red = new IntRange(0, 64);
                    //cFilter.Green = new IntRange(0, 64);
                    //cFilter.Blue = new IntRange(0, 64);
                    cFilter.FillOutsideRange = false;
                    cFilter.ApplyInPlace(bmData);



                    // locate objects
                    BlobCounter bCounter = new BlobCounter();

                    bCounter.FilterBlobs = true;
                    bCounter.MinHeight = 10;
                    bCounter.MinWidth = 10;

                    bCounter.ProcessImage(bmData);

                    Blob[] baBlobs = bCounter.GetObjectsInformation();
                    image.UnlockBits(bmData);

                    // coloring objects
                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

                    Graphics g = Graphics.FromImage(image);
                    Pen bluePen = new Pen(Color.Blue, 2);
                    Pen whitepen = new Pen(Color.White, 1);


                    for (int i = 0, n = baBlobs.Length; i < n; i++)
                    {
                        List<IntPoint> edgePoints = bCounter.GetBlobsEdgePoints(baBlobs[i]);
                        Rectangle[] _rects = bCounter.GetObjectsRectangles();

                        List<IntPoint> corners;

                        // is triangle or quadrilateral
                        if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                        {
                            PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
                            Pen pen;
                            if (subType == PolygonSubType.Rectangle && corners.Count == 4)
                            {
                                pen = bluePen;
                                ld.Loc1X = corners[0].X;
                                ld.Loc1Y = corners[0].Y;
                                ld.Loc2X = corners[1].X;
                                ld.Loc2Y = corners[1].Y;
                                ld.Loc3X = corners[2].X;
                                ld.Loc3Y = corners[2].Y;
                                ld.Loc4X = corners[3].X;
                                ld.Loc4Y = corners[3].Y;

                                // find longest side of rectangle
                                double side1 = corners[0].DistanceTo(corners[1]);
                                double side2 = corners[1].DistanceTo(corners[2]);

                                double angleInRadians = 0;
                                double deltaY = 0;
                                double deltaX = 0;
                                int select1 = 0;
                                int select2 = 1;
                                if (side1 < side2)
                                {
                                    select1 = 1;
                                    select2 = 2;
                                    ld.ItemWidth = side2;
                                    ld.ItemHeight = side1;
                                }
                                else
                                {
                                    ld.ItemWidth = side1;
                                    ld.ItemHeight = side2;
                                }

                                // find angle of longest side
                                angleInRadians = Math.Atan2(corners[select1].Y, corners[select1].X) - Math.Atan2(corners[select2].Y, corners[select2].X);
                                deltaY = Math.Abs(corners[select2].Y - corners[select1].Y);
                                deltaX = Math.Abs(corners[select2].X - corners[select1].X);

                                double angleInDegrees = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;



                                ld.LocAngle = angleInDegrees;

                                ld.OffsetY = PixelsToMM((GetCenter(corners).X - (totalwidth / 2)) * -1);
                                ld.OffsetX = PixelsToMM(GetCenter(corners).Y - (totalheight / 2));

                                ld.LocText = "side1: " + side1.ToString() + Environment.NewLine + "side2: " + side2.ToString() + Environment.NewLine + "Center X: " + GetCenter(corners).X + "Center Y: " + GetCenter(corners).Y
                                 + Environment.NewLine + "Image Center: " + (totalwidth / 2).ToString() + " x " + (totalheight / 2).ToString();

                                System.Drawing.Point[] _coordinates = ToPointsArray(corners);
                                // draw outline and corners
                                g.DrawPolygon(pen, _coordinates);

                                // draw center cross lines on image
                                g.DrawLine(whitepen, new System.Drawing.Point(0, totalheight / 2), new System.Drawing.Point(totalwidth, totalheight / 2));
                                g.DrawLine(whitepen, new System.Drawing.Point(totalwidth / 2, 0), new System.Drawing.Point(totalwidth / 2, totalheight));
                                ld.Image = image;
                            }

                        }

                    }


                    whitepen.Dispose();
                    bluePen.Dispose();
                    g.Dispose();
                    this.currentImage = image;

                    return ld;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            return ld;
        }

        private double PixelsToMM(double pixels)
        {
            return pixels / 5;

        }

        private PointF GetCenter(List<IntPoint> poly)
        {
            float accumulatedArea = 0.0f;
            float centerX = 0.0f;
            float centerY = 0.0f;

            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                float temp = poly[i].X * poly[j].Y - poly[j].X * poly[i].Y;
                accumulatedArea += temp;
                centerX += (poly[i].X + poly[j].X) * temp;
                centerY += (poly[i].Y + poly[j].Y) * temp;
            }

            if (Math.Abs(accumulatedArea) < 1E-7f)
                return PointF.Empty;  // Avoid division by zero

            accumulatedArea *= 3f;
            return new PointF(centerX / accumulatedArea, centerY / accumulatedArea);
        }


        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }

        public void SetCurrentImage(Bitmap currentImage)
        {
            this.currentImage = currentImage;

        }

        public Bitmap GetCurrentImage()
        {
            return currentImage;
        }
    }
}