using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickandPlaceV2
{
    public class KflopLocation
    {
        public int PlacementNozzle { get; set; }
        public double PickSpeed { get; set; }
        public double PlaceSpeed { get; set; }

        public double FeederX { get; set; }
        public double FeederY { get; set; }
        public double FeederHeight { get; set; }

        public double PlaceX { get; set; }
        public double PlaceY { get; set; }
        public double PlaceHeight { get; set; }

        public double PlaceRotation { get; set; }

        public bool VerifyCamera { get; set; }
        public bool TapeFeeder { get; set; }


        public KflopLocation(int placementnozzle, double pickspeed,  double placespeed, double feederx, double feedery, 
            double feederheight, double placex, double placey, double placeheight,  double placerotation, bool verifycamera, bool tapefeeder)
        {
            PlacementNozzle = placementnozzle;
            PickSpeed = pickspeed;
            PlaceSpeed = placespeed;
            FeederX = feederx;
            FeederY = feedery;
            FeederHeight = feederheight;
            PlaceX = placex;
            PlaceY = placey;
            PlaceHeight = placeheight;
            PlaceRotation = placerotation;
            VerifyCamera = verifycamera;
            TapeFeeder = tapefeeder;
        }
    }

    public class KflopAxis
    {
        public double Accel { get; set; }
        public double Jerk { get; set; }
        public double MaxVel { get; set; }



        public KflopAxis(double accel, double jerk, double maxvel)
        {
            Accel = accel;
            Jerk = jerk;
            MaxVel = maxvel;
           
        }
    }
}
