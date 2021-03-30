
using System.Drawing;

namespace PickandPlaceV2
{
    public class VisionLocationResult
    {
        public double Loc1X { get; set; }
        public double Loc1Y { get; set; }
        public double Loc2X { get; set; }
        public double Loc2Y { get; set; }
        public double Loc3X { get; set; }
        public double Loc3Y { get; set; }
        public double Loc4X { get; set; }
        public double Loc4Y { get; set; }
        public double ItemWidth { get; set; }
        public double ItemHeight { get; set; }
        public double LocAngle { get; set; }
        public string LocText { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public Bitmap Image { get; set; }

        public VisionLocationResult(double loc1x, double loc1y, double loc2x, double loc2y, double loc3x, double loc3y, double loc4x, double loc4y, double itemwidth, double itemheight, double locangle, string loctext, double offsetx, double offsety, Bitmap image)
        {
            Loc1X = loc1x;
            Loc1Y = loc1y;
            Loc2X = loc2x;
            Loc2Y = loc2y;
            Loc3X = loc3x;
            Loc3Y = loc3y;
            Loc4X = loc4x;
            Loc4Y = loc4y;
            ItemWidth = itemwidth;
            ItemHeight = itemheight;
            LocAngle = locangle;
            LocText = loctext;
            OffsetX = offsetx;
            OffsetY = offsety;
            Image = image;
        }
    }
}
