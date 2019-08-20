using Sbl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Helpers
{
    public class Prices
    {


        public static double GetPrice(string Company, string RouteLabel)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            // get settings
            var settings = db.Settings.FirstOrDefault();

            double price = 0;




            if (Company == "SBL")
            {
                if (RouteLabel == "Full")
                {
                    price = settings.FullRouteSBL;
                }

                if (RouteLabel == "Half")
                {
                    price = settings.HalfRouteSBL;
                }

                if (RouteLabel == "RemoteDebrief")
                {
                    price = settings.RemoteDebriefSBL;
                }

                if (RouteLabel == "NurseryRoutesLevel1")
                {
                    price = settings.NurseryRoutesLevel1SBL;
                }

                if (RouteLabel == "NurseryRoutesLevel2")
                {
                    price = settings.NurseryRoutesLevel2SBL;
                }

                if (RouteLabel == "Rescue2Hours")
                {
                    price = settings.Rescue2HoursSBL;
                }

                if (RouteLabel == "Rescue4Hours")
                {
                    price = settings.Rescue4HoursSBL;
                }

                if (RouteLabel == "Rescue6Hours")
                {
                    price = settings.Rescue6HoursSBL;
                }

                if (RouteLabel == "ReDelivery2Hours")
                {
                    price = settings.ReDelivery2HoursSBL;
                }

                if (RouteLabel == "ReDelivery4Hours")
                {
                    price = settings.ReDelivery4HoursSBL;
                }

                if (RouteLabel == "ReDelivery6Hours")
                {
                    price = settings.ReDelivery6HoursSBL;
                }

                if (RouteLabel == "Missort2Hours")
                {
                    price = settings.Missort2HoursSBL;
                }

                if (RouteLabel == "Missort4Hours")
                {
                    price = settings.Missort4HoursSBL;
                }

                if (RouteLabel == "Missort6Hours")
                {
                    price = settings.Missort6HoursSBL;
                }

                if (RouteLabel == "SameDay")
                {
                    price = settings.SamedaySBL;
                }

                if (RouteLabel == "TrainingDay")
                {
                    price = settings.TrainingDaySBL;
                }

                if (RouteLabel == "RideAlong")
                {
                    price = settings.RideAlongSBL;
                }


                if (RouteLabel == "SupportAd1")
                {
                    price = settings.SupportAd1Sbl;
                }

                if (RouteLabel == "SupportAd2")
                {
                    price = settings.SupportAd2Sbl;
                }

                if (RouteLabel == "SupportAd3")
                {
                    price = settings.SupportAd3Sbl;
                }

                if (RouteLabel == "LeadDriver")
                {
                    price = settings.LeadDriverSbl;
                }

                if (RouteLabel == "LargeVan")
                {
                    price = settings.LargeVanSbl;
                }


                if (RouteLabel == "CongestionCharge")
                {
                    price = settings.CongestionChargeSbl;
                }

                if (RouteLabel == "LatePayment")
                {
                    price = settings.LatePaymentSbl;
                }

                if (RouteLabel == "Mileage")
                {
                    price = settings.Mileage1MileSBL;
                }

                if (RouteLabel == "Byod")
                {
                    price = settings.BYODSbl;
                }

            }





            if (Company == "Amazon")
            {
                if (RouteLabel == "Full")
                {
                    price = settings.FullRouteAmazon;
                }

                if (RouteLabel == "Half")
                {
                    price = settings.HalfRouteAmazon;
                }

                if (RouteLabel == "RemoteDebrief")
                {
                    price = settings.RemoteDebriefAmazon;
                }

                if (RouteLabel == "NurseryRoutesLevel1")
                {
                    price = settings.NurseryRoutesLevel1Amazon;
                }

                if (RouteLabel == "NurseryRoutesLevel2")
                {
                    price = settings.NurseryRoutesLevel2Amazon;
                }

                if (RouteLabel == "Rescue2Hours")
                {
                    price = settings.Rescue2HoursAmazon;
                }

                if (RouteLabel == "Rescue4Hours")
                {
                    price = settings.Rescue4HoursAmazon;
                }

                if (RouteLabel == "Rescue6Hours")
                {
                    price = settings.Rescue6HoursAmazon;
                }

                if (RouteLabel == "ReDelivery2Hours")
                {
                    price = settings.ReDelivery2HoursAmazon;
                }

                if (RouteLabel == "ReDelivery4Hours")
                {
                    price = settings.ReDelivery4HoursAmazon;
                }

                if (RouteLabel == "ReDelivery6Hours")
                {
                    price = settings.ReDelivery6HoursAmazon;
                }

                if (RouteLabel == "Missort2Hours")
                {
                    price = settings.Missort2HoursAmazon;
                }

                if (RouteLabel == "Missort4Hours")
                {
                    price = settings.Missort4HoursAmazon;
                }

                if (RouteLabel == "Missort6Hours")
                {
                    price = settings.Missort6HoursAmazon;
                }

                if (RouteLabel == "SameDay")
                {
                    price = settings.SamedayAmazon;
                }

                if (RouteLabel == "TrainingDay")
                {
                    price = settings.TrainingDayAmazon;
                }

                if (RouteLabel == "RideAlong")
                {
                    price = settings.RideAlongAmazon;
                }


                if (RouteLabel == "SupportAd1")
                {
                    price = settings.SupportAd1Amazon;
                }

                if (RouteLabel == "SupportAd2")
                {
                    price = settings.SupportAd2Amazon;
                }

                if (RouteLabel == "SupportAd3")
                {
                    price = settings.SupportAd3Amazon;
                }

                if (RouteLabel == "LeadDriver")
                {
                    price = settings.LeadDriverAmazon;
                }

                if (RouteLabel == "LargeVan")
                {
                    price = settings.LargeVanAmazon;
                }


                if (RouteLabel == "CongestionCharge")
                {
                    price = settings.CongestionChargeAmazon;
                }

                if (RouteLabel == "LatePayment")
                {
                    price = settings.LatePaymentAmazon;
                }

                if (RouteLabel == "Mileage")
                {
                    price = settings.Mileage1MileAmazon;
                }

                if (RouteLabel == "Byod")
                {
                    price = settings.BYODAmazon;
                }

            }



            return price;
        }

    }
}