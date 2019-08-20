using Sbl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Sbl.Helpers
{
    public class Functions
    {

        #region models


        public List<AmazonDay> AmazonDays { get; set; }
        public class AmazonDay
        {
            public double AmazonDayFullQuantity { get; set; }
            public double AmazonDayFullPrice { get; set; }
            public double AmazonDayHalfQuantity { get; set; }
            public double AmazonDayHalfPrice { get; set; }
            public double AmazonDayRemoteDebriefQuantity { get; set; }
            public double AmazonDayRemoteDebriefPrice { get; set; }
            public double AmazonDayNurseryRoutesLevel1Quantity { get; set; }
            public double AmazonDayNurseryRoutesLevel1Price { get; set; }
            public double AmazonDayNurseryRoutesLevel2Quantity { get; set; }
            public double AmazonDayNurseryRoutesLevel2Price { get; set; }
            public double AmazonDayRescue2HoursQuantity { get; set; }
            public double AmazonDayRescue2HoursPrice { get; set; }
            public double AmazonDayRescue4HoursQuantity { get; set; }
            public double AmazonDayRescue4HoursPrice { get; set; }
            public double AmazonDayRescue6HoursQuantity { get; set; }
            public double AmazonDayRescue6HoursPrice { get; set; }
            public double AmazonDayReDelivery2HoursQuantity { get; set; }
            public double AmazonDayReDelivery2HoursPrice { get; set; }
            public double AmazonDayReDelivery4HoursQuantity { get; set; }
            public double AmazonDayReDelivery4HoursPrice { get; set; }
            public double AmazonDayReDelivery6HoursQuantity { get; set; }
            public double AmazonDayReDelivery6HoursPrice { get; set; }
            public double AmazonDayMissort2HoursQuantity { get; set; }
            public double AmazonDayMissort2HoursPrice { get; set; }
            public double AmazonDayMissort4HoursQuantity { get; set; }
            public double AmazonDayMissort4HoursPrice { get; set; }
            public double AmazonDayMissort6HoursQuantity { get; set; }
            public double AmazonDayMissort6HoursPrice { get; set; }
            public double AmazonDaySameDayQuantity { get; set; }
            public double AmazonDaySameDayPrice { get; set; }
            public double AmazonDayTrainingDayQuantity { get; set; }
            public double AmazonDayTrainingDayPrice { get; set; }
            public double AmazonDayRideAlongQuantity { get; set; }
            public double AmazonDayRideAlongPrice { get; set; }

            public double AmazonDaySupportAd1Quantity { get; set; }
            public double AmazonDaySupportAd1Price { get; set; }
            public double AmazonDaySupportAd2Quantity { get; set; }
            public double AmazonDaySupportAd2Price { get; set; }
            public double AmazonDaySupportAd3Quantity { get; set; }
            public double AmazonDaySupportAd3Price { get; set; }
            public double AmazonDayLeadDriverQuantity { get; set; }
            public double AmazonDayLeadDriverPrice { get; set; }
            public double AmazonDayLargeVanQuantity { get; set; }
            public double AmazonDayLargeVanPrice { get; set; }


            public double AmazonDayCongestionChargeQuantity { get; set; }
            public double AmazonDayCongestionChargePrice { get; set; }
            public double AmazonDayLatePaymentQuantity { get; set; }
            public double AmazonDayLatePaymentPrice { get; set; }

            public double AmazonDayFuel { get; set; }
            public double AmazonDayMileage { get; set; }
        }


        public List<SblDay> SblDays { get; set; }
        public class SblDay
        {
            public double SblDayFullQuantity { get; set; }
            public double SblDayFullPrice { get; set; }
            public double SblDayHalfQuantity { get; set; }
            public double SblDayHalfPrice { get; set; }
            public double SblDayRemoteDebriefQuantity { get; set; }
            public double SblDayRemoteDebriefPrice { get; set; }
            public double SblDayNurseryRoutesLevel1Quantity { get; set; }
            public double SblDayNurseryRoutesLevel1Price { get; set; }
            public double SblDayNurseryRoutesLevel2Quantity { get; set; }
            public double SblDayNurseryRoutesLevel2Price { get; set; }
            public double SblDayRescue2HoursQuantity { get; set; }
            public double SblDayRescue2HoursPrice { get; set; }
            public double SblDayRescue4HoursQuantity { get; set; }
            public double SblDayRescue4HoursPrice { get; set; }
            public double SblDayRescue6HoursQuantity { get; set; }
            public double SblDayRescue6HoursPrice { get; set; }
            public double SblDayReDelivery2HoursQuantity { get; set; }
            public double SblDayReDelivery2HoursPrice { get; set; }
            public double SblDayReDelivery4HoursQuantity { get; set; }
            public double SblDayReDelivery4HoursPrice { get; set; }
            public double SblDayReDelivery6HoursQuantity { get; set; }
            public double SblDayReDelivery6HoursPrice { get; set; }
            public double SblDayMissort2HoursQuantity { get; set; }
            public double SblDayMissort2HoursPrice { get; set; }
            public double SblDayMissort4HoursQuantity { get; set; }
            public double SblDayMissort4HoursPrice { get; set; }
            public double SblDayMissort6HoursQuantity { get; set; }
            public double SblDayMissort6HoursPrice { get; set; }
            public double SblDaySameDayQuantity { get; set; }
            public double SblDaySameDayPrice { get; set; }
            public double SblDayTrainingDayQuantity { get; set; }
            public double SblDayTrainingDayPrice { get; set; }
            public double SblDayRideAlongQuantity { get; set; }
            public double SblDayRideAlongPrice { get; set; }

            public double SblDaySupportAd1Quantity { get; set; }
            public double SblDaySupportAd1Price { get; set; }
            public double SblDaySupportAd2Quantity { get; set; }
            public double SblDaySupportAd2Price { get; set; }
            public double SblDaySupportAd3Quantity { get; set; }
            public double SblDaySupportAd3Price { get; set; }
            public double SblDayLeadDriverQuantity { get; set; }
            public double SblDayLeadDriverPrice { get; set; }
            public double SblDayLargeVanQuantity { get; set; }
            public double SblDayLargeVanPrice { get; set; }


            public double SblDayCongestionChargeQuantity { get; set; }
            public double SblDayCongestionChargePrice { get; set; }
            public double SblDayLatePaymentQuantity { get; set; }
            public double SblDayLatePaymentPrice { get; set; }

            public double SblDayFuel { get; set; }
            public double SblDayMileage { get; set; }
        }


        public List<DiffDay> DiffDays { get; set; }
        public class DiffDay
        {
            public double DiffDayFullQuantity { get; set; }
            public double DiffDayFullPrice { get; set; }
            public double DiffDayHalfQuantity { get; set; }
            public double DiffDayHalfPrice { get; set; }
            public double DiffDayRemoteDebriefQuantity { get; set; }
            public double DiffDayRemoteDebriefPrice { get; set; }
            public double DiffDayNurseryRoutesLevel1Quantity { get; set; }
            public double DiffDayNurseryRoutesLevel1Price { get; set; }
            public double DiffDayNurseryRoutesLevel2Quantity { get; set; }
            public double DiffDayNurseryRoutesLevel2Price { get; set; }
            public double DiffDayRescue2HoursQuantity { get; set; }
            public double DiffDayRescue2HoursPrice { get; set; }
            public double DiffDayRescue4HoursQuantity { get; set; }
            public double DiffDayRescue4HoursPrice { get; set; }
            public double DiffDayRescue6HoursQuantity { get; set; }
            public double DiffDayRescue6HoursPrice { get; set; }
            public double DiffDayReDelivery2HoursQuantity { get; set; }
            public double DiffDayReDelivery2HoursPrice { get; set; }
            public double DiffDayReDelivery4HoursQuantity { get; set; }
            public double DiffDayReDelivery4HoursPrice { get; set; }
            public double DiffDayReDelivery6HoursQuantity { get; set; }
            public double DiffDayReDelivery6HoursPrice { get; set; }
            public double DiffDayMissort2HoursQuantity { get; set; }
            public double DiffDayMissort2HoursPrice { get; set; }
            public double DiffDayMissort4HoursQuantity { get; set; }
            public double DiffDayMissort4HoursPrice { get; set; }
            public double DiffDayMissort6HoursQuantity { get; set; }
            public double DiffDayMissort6HoursPrice { get; set; }
            public double DiffDaySameDayQuantity { get; set; }
            public double DiffDaySameDayPrice { get; set; }
            public double DiffDayTrainingDayQuantity { get; set; }
            public double DiffDayTrainingDayPrice { get; set; }
            public double DiffDayRideAlongQuantity { get; set; }
            public double DiffDayRideAlongPrice { get; set; }

            public double DiffDaySupportAd1Quantity { get; set; }
            public double DiffDaySupportAd1Price { get; set; }
            public double DiffDaySupportAd2Quantity { get; set; }
            public double DiffDaySupportAd2Price { get; set; }
            public double DiffDaySupportAd3Quantity { get; set; }
            public double DiffDaySupportAd3Price { get; set; }
            public double DiffDayLeadDriverQuantity { get; set; }
            public double DiffDayLeadDriverPrice { get; set; }
            public double DiffDayLargeVanQuantity { get; set; }
            public double DiffDayLargeVanPrice { get; set; }

            public double DiffDayCongestionChargeQuantity { get; set; }
            public double DiffDayCongestionChargePrice { get; set; }
            public double DiffDayLatePaymentQuantity { get; set; }
            public double DiffDayLatePaymentPrice { get; set; }

            public double DiffDayFuel { get; set; }
            public double DiffDayMileage { get; set; }

        }


        public List<AmazonWeek> AmazonWeeks { get; set; }
        public class AmazonWeek
        {
            public double AmazonWeekFullQuantity { get; set; }
            public double AmazonWeekFullPrice { get; set; }
            public double AmazonWeekHalfQuantity { get; set; }
            public double AmazonWeekHalfPrice { get; set; }
            public double AmazonWeekRemoteDebriefQuantity { get; set; }
            public double AmazonWeekRemoteDebriefPrice { get; set; }
            public double AmazonWeekNurseryRoutesLevel1Quantity { get; set; }
            public double AmazonWeekNurseryRoutesLevel1Price { get; set; }
            public double AmazonWeekNurseryRoutesLevel2Quantity { get; set; }
            public double AmazonWeekNurseryRoutesLevel2Price { get; set; }
            public double AmazonWeekRescue2HoursQuantity { get; set; }
            public double AmazonWeekRescue2HoursPrice { get; set; }
            public double AmazonWeekRescue4HoursQuantity { get; set; }
            public double AmazonWeekRescue4HoursPrice { get; set; }
            public double AmazonWeekRescue6HoursQuantity { get; set; }
            public double AmazonWeekRescue6HoursPrice { get; set; }
            public double AmazonWeekReDelivery2HoursQuantity { get; set; }
            public double AmazonWeekReDelivery2HoursPrice { get; set; }
            public double AmazonWeekReDelivery4HoursQuantity { get; set; }
            public double AmazonWeekReDelivery4HoursPrice { get; set; }
            public double AmazonWeekReDelivery6HoursQuantity { get; set; }
            public double AmazonWeekReDelivery6HoursPrice { get; set; }
            public double AmazonWeekMissort2HoursQuantity { get; set; }
            public double AmazonWeekMissort2HoursPrice { get; set; }
            public double AmazonWeekMissort4HoursQuantity { get; set; }
            public double AmazonWeekMissort4HoursPrice { get; set; }
            public double AmazonWeekMissort6HoursQuantity { get; set; }
            public double AmazonWeekMissort6HoursPrice { get; set; }
            public double AmazonWeekSameDayQuantity { get; set; }
            public double AmazonWeekSameDayPrice { get; set; }
            public double AmazonWeekTrainingDayQuantity { get; set; }
            public double AmazonWeekTrainingDayPrice { get; set; }
            public double AmazonWeekRideAlongQuantity { get; set; }
            public double AmazonWeekRideAlongPrice { get; set; }

            public double AmazonWeekSupportAd1Quantity { get; set; }
            public double AmazonWeekSupportAd1Price { get; set; }
            public double AmazonWeekSupportAd2Quantity { get; set; }
            public double AmazonWeekSupportAd2Price { get; set; }
            public double AmazonWeekSupportAd3Quantity { get; set; }
            public double AmazonWeekSupportAd3Price { get; set; }
            public double AmazonWeekLeadDriverQuantity { get; set; }
            public double AmazonWeekLeadDriverPrice { get; set; }
            public double AmazonWeekLargeVanQuantity { get; set; }
            public double AmazonWeekLargeVanPrice { get; set; }

            public double AmazonWeekCongestionChargeQuantity { get; set; }
            public double AmazonWeekCongestionChargePrice { get; set; }
            public double AmazonWeekLatePaymentQuantity { get; set; }
            public double AmazonWeekLatePaymentPrice { get; set; }

            public double AmazonWeekFuel { get; set; }
            public double AmazonWeekMileage { get; set; }
        }


        public List<SblWeek> SblWeeks { get; set; }
        public class SblWeek
        {
            public double SblWeekFullQuantity { get; set; }
            public double SblWeekFullPrice { get; set; }
            public double SblWeekHalfQuantity { get; set; }
            public double SblWeekHalfPrice { get; set; }
            public double SblWeekRemoteDebriefQuantity { get; set; }
            public double SblWeekRemoteDebriefPrice { get; set; }
            public double SblWeekNurseryRoutesLevel1Quantity { get; set; }
            public double SblWeekNurseryRoutesLevel1Price { get; set; }
            public double SblWeekNurseryRoutesLevel2Quantity { get; set; }
            public double SblWeekNurseryRoutesLevel2Price { get; set; }
            public double SblWeekRescue2HoursQuantity { get; set; }
            public double SblWeekRescue2HoursPrice { get; set; }
            public double SblWeekRescue4HoursQuantity { get; set; }
            public double SblWeekRescue4HoursPrice { get; set; }
            public double SblWeekRescue6HoursQuantity { get; set; }
            public double SblWeekRescue6HoursPrice { get; set; }
            public double SblWeekReDelivery2HoursQuantity { get; set; }
            public double SblWeekReDelivery2HoursPrice { get; set; }
            public double SblWeekReDelivery4HoursQuantity { get; set; }
            public double SblWeekReDelivery4HoursPrice { get; set; }
            public double SblWeekReDelivery6HoursQuantity { get; set; }
            public double SblWeekReDelivery6HoursPrice { get; set; }
            public double SblWeekMissort2HoursQuantity { get; set; }
            public double SblWeekMissort2HoursPrice { get; set; }
            public double SblWeekMissort4HoursQuantity { get; set; }
            public double SblWeekMissort4HoursPrice { get; set; }
            public double SblWeekMissort6HoursQuantity { get; set; }
            public double SblWeekMissort6HoursPrice { get; set; }
            public double SblWeekSameDayQuantity { get; set; }
            public double SblWeekSameDayPrice { get; set; }
            public double SblWeekTrainingDayQuantity { get; set; }
            public double SblWeekTrainingDayPrice { get; set; }
            public double SblWeekRideAlongQuantity { get; set; }
            public double SblWeekRideAlongPrice { get; set; }

            public double SblWeekSupportAd1Quantity { get; set; }
            public double SblWeekSupportAd1Price { get; set; }
            public double SblWeekSupportAd2Quantity { get; set; }
            public double SblWeekSupportAd2Price { get; set; }
            public double SblWeekSupportAd3Quantity { get; set; }
            public double SblWeekSupportAd3Price { get; set; }
            public double SblWeekLeadDriverQuantity { get; set; }
            public double SblWeekLeadDriverPrice { get; set; }
            public double SblWeekLargeVanQuantity { get; set; }
            public double SblWeekLargeVanPrice { get; set; }

            public double SblWeekCongestionChargeQuantity { get; set; }
            public double SblWeekCongestionChargePrice { get; set; }
            public double SblWeekLatePaymentQuantity { get; set; }
            public double SblWeekLatePaymentPrice { get; set; }

            public double SblWeekFuel { get; set; }
            public double SblWeekMileage { get; set; }
        }


        public List<DiffWeek> DiffWeeks { get; set; }
        public class DiffWeek
        {
            public double DiffWeekFullQuantity { get; set; }
            public double DiffWeekFullPrice { get; set; }
            public double DiffWeekHalfQuantity { get; set; }
            public double DiffWeekHalfPrice { get; set; }
            public double DiffWeekRemoteDebriefQuantity { get; set; }
            public double DiffWeekRemoteDebriefPrice { get; set; }
            public double DiffWeekNurseryRoutesLevel1Quantity { get; set; }
            public double DiffWeekNurseryRoutesLevel1Price { get; set; }
            public double DiffWeekNurseryRoutesLevel2Quantity { get; set; }
            public double DiffWeekNurseryRoutesLevel2Price { get; set; }
            public double DiffWeekRescue2HoursQuantity { get; set; }
            public double DiffWeekRescue2HoursPrice { get; set; }
            public double DiffWeekRescue4HoursQuantity { get; set; }
            public double DiffWeekRescue4HoursPrice { get; set; }
            public double DiffWeekRescue6HoursQuantity { get; set; }
            public double DiffWeekRescue6HoursPrice { get; set; }
            public double DiffWeekReDelivery2HoursQuantity { get; set; }
            public double DiffWeekReDelivery2HoursPrice { get; set; }
            public double DiffWeekReDelivery4HoursQuantity { get; set; }
            public double DiffWeekReDelivery4HoursPrice { get; set; }
            public double DiffWeekReDelivery6HoursQuantity { get; set; }
            public double DiffWeekReDelivery6HoursPrice { get; set; }
            public double DiffWeekMissort2HoursQuantity { get; set; }
            public double DiffWeekMissort2HoursPrice { get; set; }
            public double DiffWeekMissort4HoursQuantity { get; set; }
            public double DiffWeekMissort4HoursPrice { get; set; }
            public double DiffWeekMissort6HoursQuantity { get; set; }
            public double DiffWeekMissort6HoursPrice { get; set; }
            public double DiffWeekSameDayQuantity { get; set; }
            public double DiffWeekSameDayPrice { get; set; }
            public double DiffWeekTrainingDayQuantity { get; set; }
            public double DiffWeekTrainingDayPrice { get; set; }
            public double DiffWeekRideAlongQuantity { get; set; }
            public double DiffWeekRideAlongPrice { get; set; }

            public double DiffWeekSupportAd1Quantity { get; set; }
            public double DiffWeekSupportAd1Price { get; set; }
            public double DiffWeekSupportAd2Quantity { get; set; }
            public double DiffWeekSupportAd2Price { get; set; }
            public double DiffWeekSupportAd3Quantity { get; set; }
            public double DiffWeekSupportAd3Price { get; set; }
            public double DiffWeekLeadDriverQuantity { get; set; }
            public double DiffWeekLeadDriverPrice { get; set; }
            public double DiffWeekLargeVanQuantity { get; set; }
            public double DiffWeekLargeVanPrice { get; set; }

            public double DiffWeekCongestionChargeQuantity { get; set; }
            public double DiffWeekCongestionChargePrice { get; set; }
            public double DiffWeekLatePaymentQuantity { get; set; }
            public double DiffWeekLatePaymentPrice { get; set; }

            public double DiffWeekFuel { get; set; }
            public double DiffWeekMileage { get; set; }
        }


        public List<ServiceList> ServiceLists { get; set; }
        public class ServiceList
        {
            public DateTime? RouteDate { get; set; }
            public int? DepotId { get; set; }

            //

            public double AmazonDayFullQuantity { get; set; }
            public double AmazonDayFullPrice { get; set; }
            public double AmazonDayHalfQuantity { get; set; }
            public double AmazonDayHalfPrice { get; set; }
            public double AmazonDayRemoteDebriefQuantity { get; set; }
            public double AmazonDayRemoteDebriefPrice { get; set; }
            public double AmazonDayNurseryRoutesLevel1Quantity { get; set; }
            public double AmazonDayNurseryRoutesLevel1Price { get; set; }
            public double AmazonDayNurseryRoutesLevel2Quantity { get; set; }
            public double AmazonDayNurseryRoutesLevel2Price { get; set; }
            public double AmazonDayRescue2HoursQuantity { get; set; }
            public double AmazonDayRescue2HoursPrice { get; set; }
            public double AmazonDayRescue4HoursQuantity { get; set; }
            public double AmazonDayRescue4HoursPrice { get; set; }
            public double AmazonDayRescue6HoursQuantity { get; set; }
            public double AmazonDayRescue6HoursPrice { get; set; }
            public double AmazonDayReDelivery2HoursQuantity { get; set; }
            public double AmazonDayReDelivery2HoursPrice { get; set; }
            public double AmazonDayReDelivery4HoursQuantity { get; set; }
            public double AmazonDayReDelivery4HoursPrice { get; set; }
            public double AmazonDayReDelivery6HoursQuantity { get; set; }
            public double AmazonDayReDelivery6HoursPrice { get; set; }
            public double AmazonDayMissort2HoursQuantity { get; set; }
            public double AmazonDayMissort2HoursPrice { get; set; }
            public double AmazonDayMissort4HoursQuantity { get; set; }
            public double AmazonDayMissort4HoursPrice { get; set; }
            public double AmazonDayMissort6HoursQuantity { get; set; }
            public double AmazonDayMissort6HoursPrice { get; set; }
            public double AmazonDaySameDayQuantity { get; set; }
            public double AmazonDaySameDayPrice { get; set; }
            public double AmazonDayTrainingDayQuantity { get; set; }
            public double AmazonDayTrainingDayPrice { get; set; }
            public double AmazonDayRideAlongQuantity { get; set; }
            public double AmazonDayRideAlongPrice { get; set; }

            public double AmazonDaySupportAd1Quantity { get; set; }
            public double AmazonDaySupportAd1Price { get; set; }
            public double AmazonDaySupportAd2Quantity { get; set; }
            public double AmazonDaySupportAd2Price { get; set; }
            public double AmazonDaySupportAd3Quantity { get; set; }
            public double AmazonDaySupportAd3Price { get; set; }
            public double AmazonDayLeadDriverQuantity { get; set; }
            public double AmazonDayLeadDriverPrice { get; set; }
            public double AmazonDayLargeVanQuantity { get; set; }
            public double AmazonDayLargeVanPrice { get; set; }

            public double AmazonDayCongestionChargeQuantity { get; set; }
            public double AmazonDayCongestionChargePrice { get; set; }
            public double AmazonDayLatePaymentQuantity { get; set; }
            public double AmazonDayLatePaymentPrice { get; set; }

            public double AmazonDayFuel { get; set; }
            public double AmazonDayMileage { get; set; }

            //

            public double SblDayFullQuantity { get; set; }
            public double SblDayFullPrice { get; set; }
            public double SblDayHalfQuantity { get; set; }
            public double SblDayHalfPrice { get; set; }
            public double SblDayRemoteDebriefQuantity { get; set; }
            public double SblDayRemoteDebriefPrice { get; set; }
            public double SblDayNurseryRoutesLevel1Quantity { get; set; }
            public double SblDayNurseryRoutesLevel1Price { get; set; }
            public double SblDayNurseryRoutesLevel2Quantity { get; set; }
            public double SblDayNurseryRoutesLevel2Price { get; set; }
            public double SblDayRescue2HoursQuantity { get; set; }
            public double SblDayRescue2HoursPrice { get; set; }
            public double SblDayRescue4HoursQuantity { get; set; }
            public double SblDayRescue4HoursPrice { get; set; }
            public double SblDayRescue6HoursQuantity { get; set; }
            public double SblDayRescue6HoursPrice { get; set; }
            public double SblDayReDelivery2HoursQuantity { get; set; }
            public double SblDayReDelivery2HoursPrice { get; set; }
            public double SblDayReDelivery4HoursQuantity { get; set; }
            public double SblDayReDelivery4HoursPrice { get; set; }
            public double SblDayReDelivery6HoursQuantity { get; set; }
            public double SblDayReDelivery6HoursPrice { get; set; }
            public double SblDayMissort2HoursQuantity { get; set; }
            public double SblDayMissort2HoursPrice { get; set; }
            public double SblDayMissort4HoursQuantity { get; set; }
            public double SblDayMissort4HoursPrice { get; set; }
            public double SblDayMissort6HoursQuantity { get; set; }
            public double SblDayMissort6HoursPrice { get; set; }
            public double SblDaySameDayQuantity { get; set; }
            public double SblDaySameDayPrice { get; set; }
            public double SblDayTrainingDayQuantity { get; set; }
            public double SblDayTrainingDayPrice { get; set; }
            public double SblDayRideAlongQuantity { get; set; }
            public double SblDayRideAlongPrice { get; set; }


            public double SblDaySupportAd1Quantity { get; set; }
            public double SblDaySupportAd1Price { get; set; }
            public double SblDaySupportAd2Quantity { get; set; }
            public double SblDaySupportAd2Price { get; set; }
            public double SblDaySupportAd3Quantity { get; set; }
            public double SblDaySupportAd3Price { get; set; }
            public double SblDayLeadDriverQuantity { get; set; }
            public double SblDayLeadDriverPrice { get; set; }
            public double SblDayLargeVanQuantity { get; set; }
            public double SblDayLargeVanPrice { get; set; }

            public double SblDayCongestionChargeQuantity { get; set; }
            public double SblDayCongestionChargePrice { get; set; }
            public double SblDayLatePaymentQuantity { get; set; }
            public double SblDayLatePaymentPrice { get; set; }

            public double SblDayFuel { get; set; }
            public double SblDayMileage { get; set; }

            //

            public double DiffDayFullQuantity { get; set; }
            public double DiffDayFullPrice { get; set; }
            public double DiffDayHalfQuantity { get; set; }
            public double DiffDayHalfPrice { get; set; }
            public double DiffDayRemoteDebriefQuantity { get; set; }
            public double DiffDayRemoteDebriefPrice { get; set; }
            public double DiffDayNurseryRoutesLevel1Quantity { get; set; }
            public double DiffDayNurseryRoutesLevel1Price { get; set; }
            public double DiffDayNurseryRoutesLevel2Quantity { get; set; }
            public double DiffDayNurseryRoutesLevel2Price { get; set; }
            public double DiffDayRescue2HoursQuantity { get; set; }
            public double DiffDayRescue2HoursPrice { get; set; }
            public double DiffDayRescue4HoursQuantity { get; set; }
            public double DiffDayRescue4HoursPrice { get; set; }
            public double DiffDayRescue6HoursQuantity { get; set; }
            public double DiffDayRescue6HoursPrice { get; set; }
            public double DiffDayReDelivery2HoursQuantity { get; set; }
            public double DiffDayReDelivery2HoursPrice { get; set; }
            public double DiffDayReDelivery4HoursQuantity { get; set; }
            public double DiffDayReDelivery4HoursPrice { get; set; }
            public double DiffDayReDelivery6HoursQuantity { get; set; }
            public double DiffDayReDelivery6HoursPrice { get; set; }
            public double DiffDayMissort2HoursQuantity { get; set; }
            public double DiffDayMissort2HoursPrice { get; set; }
            public double DiffDayMissort4HoursQuantity { get; set; }
            public double DiffDayMissort4HoursPrice { get; set; }
            public double DiffDayMissort6HoursQuantity { get; set; }
            public double DiffDayMissort6HoursPrice { get; set; }
            public double DiffDaySameDayQuantity { get; set; }
            public double DiffDaySameDayPrice { get; set; }
            public double DiffDayTrainingDayQuantity { get; set; }
            public double DiffDayTrainingDayPrice { get; set; }
            public double DiffDayRideAlongQuantity { get; set; }
            public double DiffDayRideAlongPrice { get; set; }

            public double DiffDaySupportAd1Quantity { get; set; }
            public double DiffDaySupportAd1Price { get; set; }
            public double DiffDaySupportAd2Quantity { get; set; }
            public double DiffDaySupportAd2Price { get; set; }
            public double DiffDaySupportAd3Quantity { get; set; }
            public double DiffDaySupportAd3Price { get; set; }
            public double DiffDayLeadDriverQuantity { get; set; }
            public double DiffDayLeadDriverPrice { get; set; }
            public double DiffDayLargeVanQuantity { get; set; }
            public double DiffDayLargeVanPrice { get; set; }

            public double DiffDayCongestionChargeQuantity { get; set; }
            public double DiffDayCongestionChargePrice { get; set; }
            public double DiffDayLatePaymentQuantity { get; set; }
            public double DiffDayLatePaymentPrice { get; set; }

            public double DiffDayFuel { get; set; }
            public double DiffDayMileage { get; set; }
        }


        public List<WeekList> WeekLists { get; set; }
        public class WeekList
        {
            public DateTime? RouteDate { get; set; }
            public int? DepotId { get; set; }

            //

            public double AmazonWeekFullQuantity { get; set; }
            public double AmazonWeekFullPrice { get; set; }
            public double AmazonWeekHalfQuantity { get; set; }
            public double AmazonWeekHalfPrice { get; set; }
            public double AmazonWeekRemoteDebriefQuantity { get; set; }
            public double AmazonWeekRemoteDebriefPrice { get; set; }
            public double AmazonWeekNurseryRoutesLevel1Quantity { get; set; }
            public double AmazonWeekNurseryRoutesLevel1Price { get; set; }
            public double AmazonWeekNurseryRoutesLevel2Quantity { get; set; }
            public double AmazonWeekNurseryRoutesLevel2Price { get; set; }
            public double AmazonWeekRescue2HoursQuantity { get; set; }
            public double AmazonWeekRescue2HoursPrice { get; set; }
            public double AmazonWeekRescue4HoursQuantity { get; set; }
            public double AmazonWeekRescue4HoursPrice { get; set; }
            public double AmazonWeekRescue6HoursQuantity { get; set; }
            public double AmazonWeekRescue6HoursPrice { get; set; }
            public double AmazonWeekReDelivery2HoursQuantity { get; set; }
            public double AmazonWeekReDelivery2HoursPrice { get; set; }
            public double AmazonWeekReDelivery4HoursQuantity { get; set; }
            public double AmazonWeekReDelivery4HoursPrice { get; set; }
            public double AmazonWeekReDelivery6HoursQuantity { get; set; }
            public double AmazonWeekReDelivery6HoursPrice { get; set; }
            public double AmazonWeekMissort2HoursQuantity { get; set; }
            public double AmazonWeekMissort2HoursPrice { get; set; }
            public double AmazonWeekMissort4HoursQuantity { get; set; }
            public double AmazonWeekMissort4HoursPrice { get; set; }
            public double AmazonWeekMissort6HoursQuantity { get; set; }
            public double AmazonWeekMissort6HoursPrice { get; set; }
            public double AmazonWeekSameDayQuantity { get; set; }
            public double AmazonWeekSameDayPrice { get; set; }
            public double AmazonWeekTrainingDayQuantity { get; set; }
            public double AmazonWeekTrainingDayPrice { get; set; }
            public double AmazonWeekRideAlongQuantity { get; set; }
            public double AmazonWeekRideAlongPrice { get; set; }

            public double AmazonWeekSupportAd1Quantity { get; set; }
            public double AmazonWeekSupportAd1Price { get; set; }
            public double AmazonWeekSupportAd2Quantity { get; set; }
            public double AmazonWeekSupportAd2Price { get; set; }
            public double AmazonWeekSupportAd3Quantity { get; set; }
            public double AmazonWeekSupportAd3Price { get; set; }
            public double AmazonWeekLeadDriverQuantity { get; set; }
            public double AmazonWeekLeadDriverPrice { get; set; }
            public double AmazonWeekLargeVanQuantity { get; set; }
            public double AmazonWeekLargeVanPrice { get; set; }

            public double AmazonWeekCongestionChargeQuantity { get; set; }
            public double AmazonWeekCongestionChargePrice { get; set; }
            public double AmazonWeekLatePaymentQuantity { get; set; }
            public double AmazonWeekLatePaymentPrice { get; set; }

            public double AmazonWeekFuel { get; set; }
            public double AmazonWeekMileage { get; set; }

            //

            public double SblWeekFullQuantity { get; set; }
            public double SblWeekFullPrice { get; set; }
            public double SblWeekHalfQuantity { get; set; }
            public double SblWeekHalfPrice { get; set; }
            public double SblWeekRemoteDebriefQuantity { get; set; }
            public double SblWeekRemoteDebriefPrice { get; set; }
            public double SblWeekNurseryRoutesLevel1Quantity { get; set; }
            public double SblWeekNurseryRoutesLevel1Price { get; set; }
            public double SblWeekNurseryRoutesLevel2Quantity { get; set; }
            public double SblWeekNurseryRoutesLevel2Price { get; set; }
            public double SblWeekRescue2HoursQuantity { get; set; }
            public double SblWeekRescue2HoursPrice { get; set; }
            public double SblWeekRescue4HoursQuantity { get; set; }
            public double SblWeekRescue4HoursPrice { get; set; }
            public double SblWeekRescue6HoursQuantity { get; set; }
            public double SblWeekRescue6HoursPrice { get; set; }
            public double SblWeekReDelivery2HoursQuantity { get; set; }
            public double SblWeekReDelivery2HoursPrice { get; set; }
            public double SblWeekReDelivery4HoursQuantity { get; set; }
            public double SblWeekReDelivery4HoursPrice { get; set; }
            public double SblWeekReDelivery6HoursQuantity { get; set; }
            public double SblWeekReDelivery6HoursPrice { get; set; }
            public double SblWeekMissort2HoursQuantity { get; set; }
            public double SblWeekMissort2HoursPrice { get; set; }
            public double SblWeekMissort4HoursQuantity { get; set; }
            public double SblWeekMissort4HoursPrice { get; set; }
            public double SblWeekMissort6HoursQuantity { get; set; }
            public double SblWeekMissort6HoursPrice { get; set; }
            public double SblWeekSameDayQuantity { get; set; }
            public double SblWeekSameDayPrice { get; set; }
            public double SblWeekTrainingDayQuantity { get; set; }
            public double SblWeekTrainingDayPrice { get; set; }
            public double SblWeekRideAlongQuantity { get; set; }
            public double SblWeekRideAlongPrice { get; set; }

            public double SblWeekSupportAd1Quantity { get; set; }
            public double SblWeekSupportAd1Price { get; set; }
            public double SblWeekSupportAd2Quantity { get; set; }
            public double SblWeekSupportAd2Price { get; set; }
            public double SblWeekSupportAd3Quantity { get; set; }
            public double SblWeekSupportAd3Price { get; set; }
            public double SblWeekLeadDriverQuantity { get; set; }
            public double SblWeekLeadDriverPrice { get; set; }
            public double SblWeekLargeVanQuantity { get; set; }
            public double SblWeekLargeVanPrice { get; set; }

            public double SblWeekCongestionChargeQuantity { get; set; }
            public double SblWeekCongestionChargePrice { get; set; }
            public double SblWeekLatePaymentQuantity { get; set; }
            public double SblWeekLatePaymentPrice { get; set; }

            public double SblWeekFuel { get; set; }
            public double SblWeekMileage { get; set; }

            //

            public double DiffWeekFullQuantity { get; set; }
            public double DiffWeekFullPrice { get; set; }
            public double DiffWeekHalfQuantity { get; set; }
            public double DiffWeekHalfPrice { get; set; }
            public double DiffWeekRemoteDebriefQuantity { get; set; }
            public double DiffWeekRemoteDebriefPrice { get; set; }
            public double DiffWeekNurseryRoutesLevel1Quantity { get; set; }
            public double DiffWeekNurseryRoutesLevel1Price { get; set; }
            public double DiffWeekNurseryRoutesLevel2Quantity { get; set; }
            public double DiffWeekNurseryRoutesLevel2Price { get; set; }
            public double DiffWeekRescue2HoursQuantity { get; set; }
            public double DiffWeekRescue2HoursPrice { get; set; }
            public double DiffWeekRescue4HoursQuantity { get; set; }
            public double DiffWeekRescue4HoursPrice { get; set; }
            public double DiffWeekRescue6HoursQuantity { get; set; }
            public double DiffWeekRescue6HoursPrice { get; set; }
            public double DiffWeekReDelivery2HoursQuantity { get; set; }
            public double DiffWeekReDelivery2HoursPrice { get; set; }
            public double DiffWeekReDelivery4HoursQuantity { get; set; }
            public double DiffWeekReDelivery4HoursPrice { get; set; }
            public double DiffWeekReDelivery6HoursQuantity { get; set; }
            public double DiffWeekReDelivery6HoursPrice { get; set; }
            public double DiffWeekMissort2HoursQuantity { get; set; }
            public double DiffWeekMissort2HoursPrice { get; set; }
            public double DiffWeekMissort4HoursQuantity { get; set; }
            public double DiffWeekMissort4HoursPrice { get; set; }
            public double DiffWeekMissort6HoursQuantity { get; set; }
            public double DiffWeekMissort6HoursPrice { get; set; }
            public double DiffWeekSameDayQuantity { get; set; }
            public double DiffWeekSameDayPrice { get; set; }
            public double DiffWeekTrainingDayQuantity { get; set; }
            public double DiffWeekTrainingDayPrice { get; set; }
            public double DiffWeekRideAlongQuantity { get; set; }
            public double DiffWeekRideAlongPrice { get; set; }

            public double DiffWeekSupportAd1Quantity { get; set; }
            public double DiffWeekSupportAd1Price { get; set; }
            public double DiffWeekSupportAd2Quantity { get; set; }
            public double DiffWeekSupportAd2Price { get; set; }
            public double DiffWeekSupportAd3Quantity { get; set; }
            public double DiffWeekSupportAd3Price { get; set; }
            public double DiffWeekLeadDriverQuantity { get; set; }
            public double DiffWeekLeadDriverPrice { get; set; }
            public double DiffWeekLargeVanQuantity { get; set; }
            public double DiffWeekLargeVanPrice { get; set; }

            public double DiffWeekCongestionChargeQuantity { get; set; }
            public double DiffWeekCongestionChargePrice { get; set; }
            public double DiffWeekLatePaymentQuantity { get; set; }
            public double DiffWeekLatePaymentPrice { get; set; }

            public double DiffWeekFuel { get; set; }
            public double DiffWeekMileage { get; set; }
        }


        public List<MsAttachment> MsAttachments { get; set; }
        public class MsAttachment
        {
            public MemoryStream Ms { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }


        #endregion




        #region gets

        /*
        public static List<DayList> GetDayTotals(int DepotId, DateTime StartDate, int Days)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<DayList> list = new List<DayList>();

            for (int i = 0; i < Days; i++)
            {
                DateTime currentDate = StartDate.Date.AddDays(i);

                var amazonRoutes = db.RouteAmazons.Where(x => x.DepotId == DepotId && x.RouteDate == currentDate && x.Active == true && x.Deleted == false);
                var sblRoutes = db.RouteAllocations.Where(x => x.DepotId == DepotId && x.RouteDate == currentDate && x.Active == true && x.Deleted == false);

                list.Add(new DayList
                {
                    DepotId = DepotId,
                    RouteDate = currentDate,
                    
                    //

                    AmazonDayFullQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayFullPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayHalfQuantity = amazonRoutes.Sum(x => x.HalfQuantity),
                    AmazonDayHalfPrice = amazonRoutes.Sum(x => x.HalfQuantity * x.HalfPrice),

                    AmazonDayRemoteDebriefQuantity = amazonRoutes.Sum(x => x.RemoteDebriefQuantity),
                    AmazonDayRemoteDebriefPrice = amazonRoutes.Sum(x => x.RemoteDebriefQuantity * x.RemoteDebriefPrice),

                    AmazonDayNurseryRoutesLevel1Quantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayNurseryRoutesLevel1Price = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayNurseryRoutesLevel2Quantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayNurseryRoutesLevel2Price = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayRescue2HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayRescue2HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayRescue4HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayRescue4HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayRescue6HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayRescue6HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayReDelivery2HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayReDelivery2HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayReDelivery4HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayReDelivery4HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayReDelivery6HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayReDelivery6HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayMissort2HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayMissort2HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayMissort4HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayMissort4HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayMissort6HoursQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayMissort6HoursPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDaySameDayQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDaySameDayPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayTrainingDayQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayTrainingDayPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayRideAlongQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayRideAlongPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayFuel = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayMileage = amazonRoutes.Sum(x => x.FullQuantity),

                    AmazonDayCongestionChargeQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayCongestionChargePrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),

                    AmazonDayLatePaymentQuantity = amazonRoutes.Sum(x => x.FullQuantity),
                    AmazonDayLatePaymentPrice = amazonRoutes.Sum(x => x.FullQuantity * x.FullPrice),
                    
                    //
                    AmazonWeekFullQuantity = amazonWeekFullQuantity,
                    AmazonWeekFullPrice = amazonWeekFullPrice,
                    AmazonWeekHalfQuantity = amazonWeekHalfQuantity,
                    AmazonWeekHalfPrice = amazonWeekHalfPrice,
                    AmazonWeekRemoteDebriefQuantity = amazonWeekRemoteDebriefQuantity,
                    AmazonWeekRemoteDebriefPrice = amazonWeekRemoteDebriefPrice,
                    AmazonWeekNurseryRoutesLevel1Quantity = amazonWeekNurseryRoutesLevel1Quantity,
                    AmazonWeekNurseryRoutesLevel1Price = amazonWeekNurseryRoutesLevel1Price,
                    AmazonWeekNurseryRoutesLevel2Quantity = amazonWeekNurseryRoutesLevel2Quantity,
                    AmazonWeekNurseryRoutesLevel2Price = amazonWeekNurseryRoutesLevel2Price,
                    AmazonWeekRescue2HoursQuantity = amazonWeekRescue2HoursQuantity,
                    AmazonWeekRescue2HoursPrice = amazonWeekRescue2HoursPrice,
                    AmazonWeekRescue4HoursQuantity = amazonWeekRescue4HoursQuantity,
                    AmazonWeekRescue4HoursPrice = amazonWeekRescue4HoursPrice,
                    AmazonWeekRescue6HoursQuantity = amazonWeekRescue6HoursQuantity,
                    AmazonWeekRescue6HoursPrice = amazonWeekRescue6HoursPrice,
                    AmazonWeekReDelivery2HoursQuantity = amazonWeekReDelivery2HoursQuantity,
                    AmazonWeekReDelivery2HoursPrice = amazonWeekReDelivery2HoursPrice,
                    AmazonWeekReDelivery4HoursQuantity = amazonWeekReDelivery4HoursQuantity,
                    AmazonWeekReDelivery4HoursPrice = amazonWeekReDelivery4HoursPrice,
                    AmazonWeekReDelivery6HoursQuantity = amazonWeekReDelivery6HoursQuantity,
                    AmazonWeekReDelivery6HoursPrice = amazonWeekReDelivery6HoursPrice,
                    AmazonWeekMissort2HoursQuantity = amazonWeekMissort2HoursQuantity,
                    AmazonWeekMissort2HoursPrice = amazonWeekMissort2HoursPrice,
                    AmazonWeekMissort4HoursQuantity = amazonWeekMissort4HoursQuantity,
                    AmazonWeekMissort4HoursPrice = amazonWeekMissort4HoursPrice,
                    AmazonWeekMissort6HoursQuantity = amazonWeekMissort6HoursQuantity,
                    AmazonWeekMissort6HoursPrice = amazonWeekMissort6HoursPrice,
                    AmazonWeekSameDayQuantity = amazonWeekSameDayQuantity,
                    AmazonWeekSameDayPrice = amazonWeekSameDayPrice,
                    AmazonWeekTrainingDayQuantity = amazonWeekTrainingDayQuantity,
                    AmazonWeekTrainingDayPrice = amazonWeekTrainingDayPrice,
                    AmazonWeekRideAlongQuantity = amazonWeekRideAlongQuantity,
                    AmazonWeekRideAlongPrice = amazonWeekRideAlongPrice,
                    AmazonWeekCongestionChargeQuantity = amazonWeekCongestionChargeQuantity,
                    AmazonWeekCongestionChargePrice = amazonWeekCongestionChargePrice,
                    AmazonWeekLatePaymentQuantity = amazonWeekLatePaymentQuantity,
                    AmazonWeekLatePaymentPrice = amazonWeekLatePaymentPrice,
                    AmazonWeekFuel = amazonWeekFuel,
                    AmazonWeekMileage = amazonWeekMileage,
                    AmazonWeekDeduct = amazonWeekDeduct,
                    //
                    SblDayFullQuantity = sblDayFullQuantity,
                    SblDayFullPrice = sblDayFullPrice,
                    SblDayHalfQuantity = sblDayHalfQuantity,
                    SblDayHalfPrice = sblDayHalfPrice,
                    SblDayRemoteDebriefQuantity = sblDayRemoteDebriefQuantity,
                    SblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice,
                    SblDayNurseryRoutesLevel1Quantity = sblDayNurseryRoutesLevel1Quantity,
                    SblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price,
                    SblDayNurseryRoutesLevel2Quantity = sblDayNurseryRoutesLevel2Quantity,
                    SblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price,
                    SblDayRescue2HoursQuantity = sblDayRescue2HoursQuantity,
                    SblDayRescue2HoursPrice = sblDayRescue2HoursPrice,
                    SblDayRescue4HoursQuantity = sblDayRescue4HoursQuantity,
                    SblDayRescue4HoursPrice = sblDayRescue4HoursPrice,
                    SblDayRescue6HoursQuantity = sblDayRescue6HoursQuantity,
                    SblDayRescue6HoursPrice = sblDayRescue6HoursPrice,
                    SblDayReDelivery2HoursQuantity = sblDayReDelivery2HoursQuantity,
                    SblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice,
                    SblDayReDelivery4HoursQuantity = sblDayReDelivery4HoursQuantity,
                    SblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice,
                    SblDayReDelivery6HoursQuantity = sblDayReDelivery6HoursQuantity,
                    SblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice,
                    SblDayMissort2HoursQuantity = sblDayMissort2HoursQuantity,
                    SblDayMissort2HoursPrice = sblDayMissort2HoursPrice,
                    SblDayMissort4HoursQuantity = sblDayMissort4HoursQuantity,
                    SblDayMissort4HoursPrice = sblDayMissort4HoursPrice,
                    SblDayMissort6HoursQuantity = sblDayMissort6HoursQuantity,
                    SblDayMissort6HoursPrice = sblDayMissort6HoursPrice,
                    SblDaySameDayQuantity = sblDaySameDayQuantity,
                    SblDaySameDayPrice = sblDaySameDayPrice,
                    SblDayTrainingDayQuantity = sblDayTrainingDayQuantity,
                    SblDayTrainingDayPrice = sblDayTrainingDayPrice,
                    SblDayRideAlongQuantity = sblDayRideAlongQuantity,
                    SblDayRideAlongPrice = sblDayRideAlongPrice,
                    SblDayCongestionChargeQuantity = sblDayCongestionChargeQuantity,
                    SblDayCongestionChargePrice = sblDayCongestionChargePrice,
                    SblDayLatePaymentQuantity = sblDayLatePaymentQuantity,
                    SblDayLatePaymentPrice = sblDayLatePaymentPrice,
                    SblDayFuel = sblDayFuel,
                    SblDayMileage = sblDayMileage,
                    SblDayDeduct = sblDayDeduct,
                    //
                    SblWeekFullQuantity = sblWeekFullQuantity,
                    SblWeekFullPrice = sblWeekFullPrice,
                    SblWeekHalfQuantity = sblWeekHalfQuantity,
                    SblWeekHalfPrice = sblWeekHalfPrice,
                    SblWeekRemoteDebriefQuantity = sblWeekRemoteDebriefQuantity,
                    SblWeekRemoteDebriefPrice = sblWeekRemoteDebriefPrice,
                    SblWeekNurseryRoutesLevel1Quantity = sblWeekNurseryRoutesLevel1Quantity,
                    SblWeekNurseryRoutesLevel1Price = sblWeekNurseryRoutesLevel1Price,
                    SblWeekNurseryRoutesLevel2Quantity = sblWeekNurseryRoutesLevel2Quantity,
                    SblWeekNurseryRoutesLevel2Price = sblWeekNurseryRoutesLevel2Price,
                    SblWeekRescue2HoursQuantity = sblWeekRescue2HoursQuantity,
                    SblWeekRescue2HoursPrice = sblWeekRescue2HoursPrice,
                    SblWeekRescue4HoursQuantity = sblWeekRescue4HoursQuantity,
                    SblWeekRescue4HoursPrice = sblWeekRescue4HoursPrice,
                    SblWeekRescue6HoursQuantity = sblWeekRescue6HoursQuantity,
                    SblWeekRescue6HoursPrice = sblWeekRescue6HoursPrice,
                    SblWeekReDelivery2HoursQuantity = sblWeekReDelivery2HoursQuantity,
                    SblWeekReDelivery2HoursPrice = sblWeekReDelivery2HoursPrice,
                    SblWeekReDelivery4HoursQuantity = sblWeekReDelivery4HoursQuantity,
                    SblWeekReDelivery4HoursPrice = sblWeekReDelivery4HoursPrice,
                    SblWeekReDelivery6HoursQuantity = sblWeekReDelivery6HoursQuantity,
                    SblWeekReDelivery6HoursPrice = sblWeekReDelivery6HoursPrice,
                    SblWeekMissort2HoursQuantity = sblWeekMissort2HoursQuantity,
                    SblWeekMissort2HoursPrice = sblWeekMissort2HoursPrice,
                    SblWeekMissort4HoursQuantity = sblWeekMissort4HoursQuantity,
                    SblWeekMissort4HoursPrice = sblWeekMissort4HoursPrice,
                    SblWeekMissort6HoursQuantity = sblWeekMissort6HoursQuantity,
                    SblWeekMissort6HoursPrice = sblWeekMissort6HoursPrice,
                    SblWeekSameDayQuantity = sblWeekSameDayQuantity,
                    SblWeekSameDayPrice = sblWeekSameDayPrice,
                    SblWeekTrainingDayQuantity = sblWeekTrainingDayQuantity,
                    SblWeekTrainingDayPrice = sblWeekTrainingDayPrice,
                    SblWeekRideAlongQuantity = sblWeekRideAlongQuantity,
                    SblWeekRideAlongPrice = sblWeekRideAlongPrice,
                    SblWeekCongestionChargeQuantity = sblWeekCongestionChargeQuantity,
                    SblWeekCongestionChargePrice = sblWeekCongestionChargePrice,
                    SblWeekLatePaymentQuantity = sblWeekLatePaymentQuantity,
                    SblWeekLatePaymentPrice = sblWeekLatePaymentPrice,
                    SblWeekFuel = sblWeekFuel,
                    SblWeekMileage = sblWeekMileage,
                    SblWeekDeduct = sblWeekDeduct,
                    //
                    DiffDayFullQuantity = diffDayFullQuantity,
                    DiffDayFullPrice = diffDayFullPrice,
                    DiffDayHalfQuantity = diffDayHalfQuantity,
                    DiffDayHalfPrice = diffDayHalfPrice,
                    DiffDayRemoteDebriefQuantity = diffDayRemoteDebriefQuantity,
                    DiffDayRemoteDebriefPrice = diffDayRemoteDebriefPrice,
                    DiffDayNurseryRoutesLevel1Quantity = diffDayNurseryRoutesLevel1Quantity,
                    DiffDayNurseryRoutesLevel1Price = diffDayNurseryRoutesLevel1Price,
                    DiffDayNurseryRoutesLevel2Quantity = diffDayNurseryRoutesLevel2Quantity,
                    DiffDayNurseryRoutesLevel2Price = diffDayNurseryRoutesLevel2Price,
                    DiffDayRescue2HoursQuantity = diffDayRescue2HoursQuantity,
                    DiffDayRescue2HoursPrice = diffDayRescue2HoursPrice,
                    DiffDayRescue4HoursQuantity = diffDayRescue4HoursQuantity,
                    DiffDayRescue4HoursPrice = diffDayRescue4HoursPrice,
                    DiffDayRescue6HoursQuantity = diffDayRescue6HoursQuantity,
                    DiffDayRescue6HoursPrice = diffDayRescue6HoursPrice,
                    DiffDayReDelivery2HoursQuantity = diffDayReDelivery2HoursQuantity,
                    DiffDayReDelivery2HoursPrice = diffDayReDelivery2HoursPrice,
                    DiffDayReDelivery4HoursQuantity = diffDayReDelivery4HoursQuantity,
                    DiffDayReDelivery4HoursPrice = diffDayReDelivery4HoursPrice,
                    DiffDayReDelivery6HoursQuantity = diffDayReDelivery6HoursQuantity,
                    DiffDayReDelivery6HoursPrice = diffDayReDelivery6HoursPrice,
                    DiffDayMissort2HoursQuantity = diffDayMissort2HoursQuantity,
                    DiffDayMissort2HoursPrice = diffDayMissort2HoursPrice,
                    DiffDayMissort4HoursQuantity = diffDayMissort4HoursQuantity,
                    DiffDayMissort4HoursPrice = diffDayMissort4HoursPrice,
                    DiffDayMissort6HoursQuantity = diffDayMissort6HoursQuantity,
                    DiffDayMissort6HoursPrice = diffDayMissort6HoursPrice,
                    DiffDaySameDayQuantity = diffDaySameDayQuantity,
                    DiffDaySameDayPrice = diffDaySameDayPrice,
                    DiffDayTrainingDayQuantity = diffDayTrainingDayQuantity,
                    DiffDayTrainingDayPrice = diffDayTrainingDayPrice,
                    DiffDayRideAlongQuantity = diffDayRideAlongQuantity,
                    DiffDayRideAlongPrice = diffDayRideAlongPrice,
                    DiffDayCongestionChargeQuantity = diffDayCongestionChargeQuantity,
                    DiffDayCongestionChargePrice = diffDayCongestionChargePrice,
                    DiffDayLatePaymentQuantity = diffDayLatePaymentQuantity,
                    DiffDayLatePaymentPrice = diffDayLatePaymentPrice,
                    DiffDayFuel = diffDayFuel,
                    DiffDayMileage = diffDayMileage,
                    DiffDayDeduct = diffDayDeduct,
                    //
                    DiffWeekFullQuantity = diffWeekFullQuantity,
                    DiffWeekFullPrice = diffWeekFullPrice,
                    DiffWeekHalfQuantity = diffWeekHalfQuantity,
                    DiffWeekHalfPrice = diffWeekHalfPrice,
                    DiffWeekRemoteDebriefQuantity = diffWeekRemoteDebriefQuantity,
                    DiffWeekRemoteDebriefPrice = diffWeekRemoteDebriefPrice,
                    DiffWeekNurseryRoutesLevel1Quantity = diffWeekNurseryRoutesLevel1Quantity,
                    DiffWeekNurseryRoutesLevel1Price = diffWeekNurseryRoutesLevel1Price,
                    DiffWeekNurseryRoutesLevel2Quantity = diffWeekNurseryRoutesLevel2Quantity,
                    DiffWeekNurseryRoutesLevel2Price = diffWeekNurseryRoutesLevel2Price,
                    DiffWeekRescue2HoursQuantity = diffWeekRescue2HoursQuantity,
                    DiffWeekRescue2HoursPrice = diffWeekRescue2HoursPrice,
                    DiffWeekRescue4HoursQuantity = diffWeekRescue4HoursQuantity,
                    DiffWeekRescue4HoursPrice = diffWeekRescue4HoursPrice,
                    DiffWeekRescue6HoursQuantity = diffWeekRescue6HoursQuantity,
                    DiffWeekRescue6HoursPrice = diffWeekRescue6HoursPrice,
                    DiffWeekReDelivery2HoursQuantity = diffWeekReDelivery2HoursQuantity,
                    DiffWeekReDelivery2HoursPrice = diffWeekReDelivery2HoursPrice,
                    DiffWeekReDelivery4HoursQuantity = diffWeekReDelivery4HoursQuantity,
                    DiffWeekReDelivery4HoursPrice = diffWeekReDelivery4HoursPrice,
                    DiffWeekReDelivery6HoursQuantity = diffWeekReDelivery6HoursQuantity,
                    DiffWeekReDelivery6HoursPrice = diffWeekReDelivery6HoursPrice,
                    DiffWeekMissort2HoursQuantity = diffWeekMissort2HoursQuantity,
                    DiffWeekMissort2HoursPrice = diffWeekMissort2HoursPrice,
                    DiffWeekMissort4HoursQuantity = diffWeekMissort4HoursQuantity,
                    DiffWeekMissort4HoursPrice = diffWeekMissort4HoursPrice,
                    DiffWeekMissort6HoursQuantity = diffWeekMissort6HoursQuantity,
                    DiffWeekMissort6HoursPrice = diffWeekMissort6HoursPrice,
                    DiffWeekSameDayQuantity = diffWeekSameDayQuantity,
                    DiffWeekSameDayPrice = diffWeekSameDayPrice,
                    DiffWeekTrainingDayQuantity = diffWeekTrainingDayQuantity,
                    DiffWeekTrainingDayPrice = diffWeekTrainingDayPrice,
                    DiffWeekRideAlongQuantity = diffWeekRideAlongQuantity,
                    DiffWeekRideAlongPrice = diffWeekRideAlongPrice,
                    DiffWeekCongestionChargeQuantity = diffWeekCongestionChargeQuantity,
                    DiffWeekCongestionChargePrice = diffWeekCongestionChargePrice,
                    DiffWeekLatePaymentQuantity = diffWeekLatePaymentQuantity,
                    DiffWeekLatePaymentPrice = diffWeekLatePaymentPrice,
                    DiffWeekFuel = diffWeekFuel,
                    DiffWeekMileage = diffWeekMileage,
                    DiffWeekDeduct = diffWeekDeduct,
                    //
                    AmazonWeekTotalQuantity = amazonWeekTotalQuantity,
                    AmazonWeekTotalPrice = amazonWeekTotalPrice,
                    SblWeekTotalQuantity = sblWeekTotalQuantity,
                    SblWeekTotalPrice = sblWeekTotalPrice,
                    DiffWeekTotalQuantity = diffWeekTotalQuantity,
                    DiffWeekTotalPrice = diffWeekTotalPrice

                });



            }

            return list;
        }
        */

        public static string GetAssociateReceiptEmailSubject(int? AssociateId, DateTime? DateStart, int Days)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            StringBuilder sb = new StringBuilder();

            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            string subject = "";
            int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(DateStart.Value);

            if (associate != null)
            {
                subject = "SBL: Work Statement - Week " + week + " - " + associate.Name + " - " + String.Format("{0:dd/MM/yyyy}", DateStart) + " to " + String.Format("{0:dd/MM/yyyy}", DateStart.Value.AddDays(Days));
            }

            return subject;
        }



        public static string GetAssociateReceiptEmailBody(int? AssociateId, DateTime? Date, int Days)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            StringBuilder sb = new StringBuilder();

            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            if (associate != null)
            {
                // get settings
                var settings = db.Settings.FirstOrDefault();

                // focus date
                var focusDate = Date.Value.Date;

                // days and period end date
                var periodEndDate = focusDate.AddDays(Days);

                // week
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(Date.Value);

                // get lists from period
                var getroutes = db.RouteAllocations.Where(x => x.AssociateId == associate.Id && x.RouteDate >= focusDate && x.RouteDate <= periodEndDate && x.Active == true && x.Deleted == false);
                var getcharges = db.Charges.Where(x => x.AssociateId == associate.Id && x.Date >= focusDate && x.Date <= periodEndDate && x.Active == true && x.Deleted == false);
                var getsubrentals = db.SubRentals.Where(x => x.AssociateId == associate.Id && ((x.DateReturned >= focusDate && x.DateReturned <= periodEndDate) || (!x.DateReturned.HasValue)) && x.Active == true && x.Deleted == false);

                // routes
                var routes = (from x in getroutes
                              select new AssociateRemittanceViewModel.Route
                              {
                                  Id = x.Id,
                                  RouteDate = x.RouteDate,
                                  RouteType1 = x.RouteType1,
                                  RouteCode1 = x.RouteCode1,
                                  RouteExtra = (x.RouteType2 != "0" ?
                                                    x.RouteType2 == "SupportAd1" ||
                                                    x.RouteType2 == "SupportAd2" ||
                                                    x.RouteType2 == "SupportAd3" ? "Support" : x.RouteType2 : "") +
                                                (x.RouteType3 != "0" ? " / " +
                                                    x.RouteType3 == "SupportAd1" ||
                                                    x.RouteType3 == "SupportAd2" ||
                                                    x.RouteType3 == "SupportAd3" ? "Support" : x.RouteType3 : "") +
                                                (x.RouteType4 != "0" ? " / " +
                                                    x.RouteType4 == "SupportAd1" ||
                                                    x.RouteType4 == "SupportAd2" ||
                                                    x.RouteType4 == "SupportAd3" ? "Support" : x.RouteType4 : "") +
                                                (x.RouteType5 != "0" ? " / " +
                                                    x.RouteType5 == "SupportAd1" ||
                                                    x.RouteType5 == "SupportAd2" ||
                                                    x.RouteType5 == "SupportAd3" ? "Support" : x.RouteType5 : ""),
                                  //RouteExtra = (x.RouteType2 != "0" ? x.RouteType2 + " (" + x.RouteCode2 + ")" : "") + (x.RouteType3 != "0" ? " / " + x.RouteType3 + " (" + x.RouteCode3 + ")" : ""),
                                  Depot = x.Depot.Name,
                                  Mileage = x.Mileage,
                                  Byod = x.BYODPrice,
                                  RouteRate = x.RoutePrice1,
                                  RouteExtraRate = x.RoutePrice2 + x.RoutePrice3,
                                  FuelSupport = x.Mileage * settings.Mileage1MileSBL,
                                  SubTotal = (x.BYODPrice + x.RoutePrice1 + x.RoutePrice2 + x.RoutePrice3 + (x.Mileage * settings.Mileage1MileSBL)),
                                  //
                                  AllocationStatus = x.AllocationStatus,
                                  AuthPayroll = x.AuthPayroll,
                                  AuthPoc = x.AuthPoc,
                                  AuthAdmin = x.AuthAdmin,
                              }).OrderBy(x => x.RouteDate).ToList();


                // credits
                var credits = (from x in getcharges
                               where x.SetAsCredit == true
                               select new AssociateRemittanceViewModel.Credit
                               {
                                   CreditDate = x.Date,
                                   Description = x.Description,
                                   CreditAmount = x.Amount
                               }).ToList();


                // deductions
                var deductions = (from x in getcharges
                                  where x.SetAsCredit == false
                                  select new AssociateRemittanceViewModel.Deduction
                                  {
                                      DeductionDate = x.Date,
                                      Description = x.Description,
                                      DeductionAmount = x.Amount
                                  }).ToList();


                // subrentals
                var subrentals = (from x in getsubrentals
                                  select new AssociateRemittanceViewModel.SubRental
                                  {
                                      VanRentalDescription = "Van Rental: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      VanRentalPrice = x.VanRentalPrice,
                                      InsuranceDescription = "Insurance: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      InsurancePrice = x.InsurancePrice,
                                      GoodsInTransitDescription = "Goods in Transit: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      GoodsInTransitPrice = x.GoodsInTransitPrice,
                                      PublicLiabilityDescription = "Public Liability: " + x.Vehicle.Make + " - " + x.Vehicle.Model + " - " + x.Vehicle.Registration,
                                      PublicLiabilityPrice = x.PublicLiabilityPrice,
                                      SubRentalAmount = x.RentalPrice
                                  }).ToList();


                // intro
                sb.Append("Hi " + associate.Name + ",");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("Please see below work statement report for week " + week + ".");
                sb.Append("<br />");
                sb.Append("<br />");


                // routes
                if (routes.Any())
                {
                    sb.Append("<table style=\"border:1px solid #000000;\">");
                    sb.Append("<thead>");
                    sb.Append("<tr><th colspan=\"6\">Routes</th></tr>");
                    sb.Append("<tr>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Date</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Route Type</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Route Code</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Extra Services</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Site</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Miles</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    foreach (var route in routes)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy}", route.RouteDate) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + route.RouteType1 + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + route.RouteCode1 + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + route.RouteExtra + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + route.Depot + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + route.Mileage + "</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                }


                // credits
                if (credits.Any())
                {
                    sb.Append("<hr>");
                    sb.Append("<table style=\"border:1px solid #000000;\">");
                    sb.Append("<thead>");
                    sb.Append("<tr><th colspan=\"2\">Credits</th></tr>");
                    sb.Append("<tr>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Date</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Description</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    foreach (var credit in credits)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy}", credit.CreditDate) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + credit.Description + "</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    sb.Append("<hr>");
                }


                // deductions
                if (deductions.Any())
                {
                    sb.Append("<hr>");
                    sb.Append("<table style=\"border:1px solid #000000;\">");
                    sb.Append("<thead>");
                    sb.Append("<tr><th colspan=\"2\">Deductions</th></tr>");
                    sb.Append("<tr>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Date</th>");
                    sb.Append("<th style=\"border:1px solid #000000;padding:3px;\">Description</th>");
                    sb.Append("</tr>");
                    sb.Append("</thead>");
                    foreach (var subrental in subrentals)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", focusDate, focusDate.AddDays(6)) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + subrental.VanRentalDescription + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", focusDate, focusDate.AddDays(6)) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + subrental.InsuranceDescription + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", focusDate, focusDate.AddDays(6)) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + subrental.GoodsInTransitDescription + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", focusDate, focusDate.AddDays(6)) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + subrental.PublicLiabilityDescription + "</td>");
                        sb.Append("</tr>");
                    }
                    foreach (var deduction in deductions)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + String.Format("{0:dd/MM/yyyy}", deduction.DeductionDate) + "</td>");
                        sb.Append("<td style=\"border:1px solid #000000;padding:3px;\">" + deduction.Description + "</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                }


                // message
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("<span style=\"font-size:20px;font-weight:bold;\">In case of discrepancy please contact your supervisor (POC) immediately.</span>");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("If no amendment is requested within 72 hours of this email sent, We will assume the information above is correct and an invoice will be automatically generated on the payment week.");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("It's a pleasure to work with you!");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("This is a automated email service - please do not reply to this email.");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("<img src=\"http://" + settings.DomainName + "/img/logo-email.png\" style=\"margin:20px;\" width=\"200\" />");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("SILVA BROTHERS LOGISTICS LIMITED");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("Breach of Confidentiality.");
                sb.Append("<br />");
                sb.Append("This email and any files transmitted with it are confidential and intended solely for the use of the individual or entity to whom they are addressed.If you have received this email in error please notify the system manager.This message contains confidential information and is intended only for the individual named.If you are not the named addressee you should not disseminate, distribute or copy this e - mail.Please notify the sender immediately by e - mail if you have received this e - mail by mistake and delete this e - mail from your system.If you are not the intended recipient you are notified that disclosing, copying, distributing or taking any action in reliance on the contents of this information is strictly prohibited.");
                sb.Append("<br />");
                sb.Append("<br />");


            }

            return sb.ToString();
        }




        public static string GetAssociateRemittanceEmailSubject(int? AssociateId, DateTime? DateStart, int Days)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            StringBuilder sb = new StringBuilder();

            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            string subject = "";
            int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(DateStart.Value);

            if (associate != null)
            {
                subject = "SBL: Week " + week + " Remittance - " + associate.Name + " - " + String.Format("{0:dd/MM/yyyy}", DateStart) + " to " + String.Format("{0:dd/MM/yyyy}", DateStart.Value.AddDays(Days));
            }

            return subject;
        }



        public static string GetAssociateRemittanceEmailBody(int? AssociateId, DateTime? Date, int Days)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            StringBuilder sb = new StringBuilder();

            var associate = db.Associates.Where(x => x.Id == AssociateId).FirstOrDefault();

            if (associate != null)
            {
                // get settings
                var settings = db.Settings.FirstOrDefault();

                // focus date
                var focusDate = Date.Value.Date;

                // days and period end date
                var periodEndDate = focusDate.AddDays(Days);

                // week
                int week = Helpers.DateTimeExtensions.GetWeekNumberOfYear(Date.Value);

                // intro
                sb.Append("Hi " + associate.Name + ",");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("Please find attached your invoice for week " + week + ".");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("It's a pleasure to work with you!");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("This is a automated email service - please do not reply to this email.");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("<img src=\"http://" + settings.DomainName + "/img/logo-email.png\" style=\"margin:20px;\" width=\"200\" />");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("SILVA BROTHERS LOGISTICS LIMITED");
                sb.Append("<br />");
                sb.Append("<br />");
                sb.Append("Breach of Confidentiality.");
                sb.Append("<br />");
                sb.Append("This email and any files transmitted with it are confidential and intended solely for the use of the individual or entity to whom they are addressed.If you have received this email in error please notify the system manager.This message contains confidential information and is intended only for the individual named.If you are not the named addressee you should not disseminate, distribute or copy this e - mail.Please notify the sender immediately by e - mail if you have received this e - mail by mistake and delete this e - mail from your system.If you are not the intended recipient you are notified that disclosing, copying, distributing or taking any action in reliance on the contents of this information is strictly prohibited.");
                sb.Append("<br />");
                sb.Append("<br />");

            }

            return sb.ToString();
        }



        /*

        public static ServiceList GetServiceConfirmationDate(int DepotId, DateTime RouteDate, int Days)
        {
            double amazonDayFullQuantity = 0;
            double amazonDayFullPrice = 0;
            double amazonDayHalfQuantity = 0;
            double amazonDayHalfPrice = 0;
            double amazonDayRemoteDebriefQuantity = 0;
            double amazonDayRemoteDebriefPrice = 0;
            double amazonDayNurseryRoutesLevel1Quantity = 0;
            double amazonDayNurseryRoutesLevel1Price = 0;
            double amazonDayNurseryRoutesLevel2Quantity = 0;
            double amazonDayNurseryRoutesLevel2Price = 0;
            double amazonDayRescue2HoursQuantity = 0;
            double amazonDayRescue2HoursPrice = 0;
            double amazonDayRescue4HoursQuantity = 0;
            double amazonDayRescue4HoursPrice = 0;
            double amazonDayRescue6HoursQuantity = 0;
            double amazonDayRescue6HoursPrice = 0;
            double amazonDayReDelivery2HoursQuantity = 0;
            double amazonDayReDelivery2HoursPrice = 0;
            double amazonDayReDelivery4HoursQuantity = 0;
            double amazonDayReDelivery4HoursPrice = 0;
            double amazonDayReDelivery6HoursQuantity = 0;
            double amazonDayReDelivery6HoursPrice = 0;
            double amazonDayMissort2HoursQuantity = 0;
            double amazonDayMissort2HoursPrice = 0;
            double amazonDayMissort4HoursQuantity = 0;
            double amazonDayMissort4HoursPrice = 0;
            double amazonDayMissort6HoursQuantity = 0;
            double amazonDayMissort6HoursPrice = 0;
            double amazonDaySameDayQuantity = 0;
            double amazonDaySameDayPrice = 0;
            double amazonDayTrainingDayQuantity = 0;
            double amazonDayTrainingDayPrice = 0;
            double amazonDayRideAlongQuantity = 0;
            double amazonDayRideAlongPrice = 0;
            double amazonDayCongestionChargeQuantity = 0;
            double amazonDayCongestionChargePrice = 0;
            double amazonDayLatePaymentQuantity = 0;
            double amazonDayLatePaymentPrice = 0;
            double amazonDayMileage = 0;
            double amazonDayFuel = 0;
            //
            double sblDayFullQuantity = 0;
            double sblDayFullPrice = 0;
            double sblDayHalfQuantity = 0;
            double sblDayHalfPrice = 0;
            double sblDayRemoteDebriefQuantity = 0;
            double sblDayRemoteDebriefPrice = 0;
            double sblDayNurseryRoutesLevel1Quantity = 0;
            double sblDayNurseryRoutesLevel1Price = 0;
            double sblDayNurseryRoutesLevel2Quantity = 0;
            double sblDayNurseryRoutesLevel2Price = 0;
            double sblDayRescue2HoursQuantity = 0;
            double sblDayRescue2HoursPrice = 0;
            double sblDayRescue4HoursQuantity = 0;
            double sblDayRescue4HoursPrice = 0;
            double sblDayRescue6HoursQuantity = 0;
            double sblDayRescue6HoursPrice = 0;
            double sblDayReDelivery2HoursQuantity = 0;
            double sblDayReDelivery2HoursPrice = 0;
            double sblDayReDelivery4HoursQuantity = 0;
            double sblDayReDelivery4HoursPrice = 0;
            double sblDayReDelivery6HoursQuantity = 0;
            double sblDayReDelivery6HoursPrice = 0;
            double sblDayMissort2HoursQuantity = 0;
            double sblDayMissort2HoursPrice = 0;
            double sblDayMissort4HoursQuantity = 0;
            double sblDayMissort4HoursPrice = 0;
            double sblDayMissort6HoursQuantity = 0;
            double sblDayMissort6HoursPrice = 0;
            double sblDaySameDayQuantity = 0;
            double sblDaySameDayPrice = 0;
            double sblDayTrainingDayQuantity = 0;
            double sblDayTrainingDayPrice = 0;
            double sblDayRideAlongQuantity = 0;
            double sblDayRideAlongPrice = 0;
            double sblDayCongestionChargeQuantity = 0;
            double sblDayCongestionChargePrice = 0;
            double sblDayLatePaymentQuantity = 0;
            double sblDayLatePaymentPrice = 0;
            double sblDayMileage = 0;
            double sblDayFuel = 0;
            //
            double diffDayFullQuantity = 0;
            double diffDayFullPrice = 0;
            double diffDayHalfQuantity = 0;
            double diffDayHalfPrice = 0;
            double diffDayRemoteDebriefQuantity = 0;
            double diffDayRemoteDebriefPrice = 0;
            double diffDayNurseryRoutesLevel1Quantity = 0;
            double diffDayNurseryRoutesLevel1Price = 0;
            double diffDayNurseryRoutesLevel2Quantity = 0;
            double diffDayNurseryRoutesLevel2Price = 0;
            double diffDayRescue2HoursQuantity = 0;
            double diffDayRescue2HoursPrice = 0;
            double diffDayRescue4HoursQuantity = 0;
            double diffDayRescue4HoursPrice = 0;
            double diffDayRescue6HoursQuantity = 0;
            double diffDayRescue6HoursPrice = 0;
            double diffDayReDelivery2HoursQuantity = 0;
            double diffDayReDelivery2HoursPrice = 0;
            double diffDayReDelivery4HoursQuantity = 0;
            double diffDayReDelivery4HoursPrice = 0;
            double diffDayReDelivery6HoursQuantity = 0;
            double diffDayReDelivery6HoursPrice = 0;
            double diffDayMissort2HoursQuantity = 0;
            double diffDayMissort2HoursPrice = 0;
            double diffDayMissort4HoursQuantity = 0;
            double diffDayMissort4HoursPrice = 0;
            double diffDayMissort6HoursQuantity = 0;
            double diffDayMissort6HoursPrice = 0;
            double diffDaySameDayQuantity = 0;
            double diffDaySameDayPrice = 0;
            double diffDayTrainingDayQuantity = 0;
            double diffDayTrainingDayPrice = 0;
            double diffDayRideAlongQuantity = 0;
            double diffDayRideAlongPrice = 0;
            double diffDayCongestionChargeQuantity = 0;
            double diffDayCongestionChargePrice = 0;
            double diffDayLatePaymentQuantity = 0;
            double diffDayLatePaymentPrice = 0;
            double diffDayMileage = 0;
            double diffDayFuel = 0;

            ApplicationDbContext db = new ApplicationDbContext();

            for (int i = 0; i < Days; i++)
            {
                var focusdate = RouteDate.AddDays(i);
                var sblRoutes = db.RouteAllocations.Where(x => x.DepotId == DepotId && x.RouteDate == focusdate && x.Active == true && x.Deleted == false);
                var amazonRoutes = db.RouteAmazons.Where(x => x.DepotId == DepotId && x.RouteDate == focusdate && x.Active == true && x.Deleted == false);
                //
                amazonDayFullQuantity = amazonDayFullQuantity + amazonRoutes.Sum(s => (double?)(s.FullQuantity)) ?? 0;
                amazonDayFullPrice = amazonDayFullPrice + amazonRoutes.Sum(s => (double?)(s.FullPrice)) ?? 0;
                amazonDayHalfQuantity = amazonDayHalfQuantity + amazonRoutes.Sum(s => (double?)(s.HalfQuantity)) ?? 0;
                amazonDayHalfPrice = amazonDayHalfPrice + amazonRoutes.Sum(s => (double?)(s.HalfPrice)) ?? 0;
                amazonDayRemoteDebriefQuantity = amazonDayRemoteDebriefQuantity + amazonRoutes.Sum(s => (double?)(s.RemoteDebriefQuantity)) ?? 0;
                amazonDayRemoteDebriefPrice = amazonDayRemoteDebriefPrice + amazonRoutes.Sum(s => (double?)(s.RemoteDebriefPrice)) ?? 0;
                amazonDayNurseryRoutesLevel1Quantity = amazonDayNurseryRoutesLevel1Quantity + amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel1Quantity)) ?? 0;
                amazonDayNurseryRoutesLevel1Price = amazonDayNurseryRoutesLevel1Price + amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel1Price)) ?? 0;
                amazonDayNurseryRoutesLevel2Quantity = amazonDayNurseryRoutesLevel2Quantity + amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel2Quantity)) ?? 0;
                amazonDayNurseryRoutesLevel2Price = amazonDayNurseryRoutesLevel2Price + amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel2Price)) ?? 0;
                amazonDayRescue2HoursQuantity = amazonDayRescue2HoursQuantity + amazonRoutes.Sum(s => (double?)(s.Rescue2HoursQuantity)) ?? 0;
                amazonDayRescue2HoursPrice = amazonDayRescue2HoursPrice + amazonRoutes.Sum(s => (double?)(s.Rescue2HoursPrice)) ?? 0;
                amazonDayRescue4HoursQuantity = amazonDayRescue4HoursQuantity + amazonRoutes.Sum(s => (double?)(s.Rescue4HoursQuantity)) ?? 0;
                amazonDayRescue4HoursPrice = amazonDayRescue4HoursPrice + amazonRoutes.Sum(s => (double?)(s.Rescue4HoursPrice)) ?? 0;
                amazonDayRescue6HoursQuantity = amazonDayRescue6HoursQuantity + amazonRoutes.Sum(s => (double?)(s.Rescue6HoursQuantity)) ?? 0;
                amazonDayRescue6HoursPrice = amazonDayRescue6HoursPrice + amazonRoutes.Sum(s => (double?)(s.Rescue6HoursPrice)) ?? 0;
                amazonDayReDelivery2HoursQuantity = amazonDayReDelivery2HoursQuantity + amazonRoutes.Sum(s => (double?)(s.ReDelivery2HoursQuantity)) ?? 0;
                amazonDayReDelivery2HoursPrice = amazonDayReDelivery2HoursPrice + amazonRoutes.Sum(s => (double?)(s.ReDelivery2HoursPrice)) ?? 0;
                amazonDayReDelivery4HoursQuantity = amazonDayReDelivery4HoursQuantity + amazonRoutes.Sum(s => (double?)(s.ReDelivery4HoursQuantity)) ?? 0;
                amazonDayReDelivery4HoursPrice = amazonDayReDelivery4HoursPrice + amazonRoutes.Sum(s => (double?)(s.ReDelivery4HoursPrice)) ?? 0;
                amazonDayReDelivery6HoursQuantity = amazonDayReDelivery6HoursQuantity + amazonRoutes.Sum(s => (double?)(s.ReDelivery6HoursQuantity)) ?? 0;
                amazonDayReDelivery6HoursPrice = amazonDayReDelivery6HoursPrice + amazonRoutes.Sum(s => (double?)(s.ReDelivery6HoursPrice)) ?? 0;
                amazonDayMissort2HoursQuantity = amazonDayMissort2HoursQuantity + amazonRoutes.Sum(s => (double?)(s.Missort2HoursQuantity)) ?? 0;
                amazonDayMissort2HoursPrice = amazonDayMissort2HoursPrice + amazonRoutes.Sum(s => (double?)(s.Missort2HoursPrice)) ?? 0;
                amazonDayMissort4HoursQuantity = amazonDayMissort4HoursQuantity + amazonRoutes.Sum(s => (double?)(s.Missort4HoursQuantity)) ?? 0;
                amazonDayMissort4HoursPrice = amazonDayMissort4HoursPrice + amazonRoutes.Sum(s => (double?)(s.Missort4HoursPrice)) ?? 0;
                amazonDayMissort6HoursQuantity = amazonDayMissort6HoursQuantity + amazonRoutes.Sum(s => (double?)(s.Missort6HoursQuantity)) ?? 0;
                amazonDayMissort6HoursPrice = amazonDayMissort6HoursPrice + amazonRoutes.Sum(s => (double?)(s.Missort6HoursPrice)) ?? 0;
                amazonDaySameDayQuantity = amazonDaySameDayQuantity + amazonRoutes.Sum(s => (double?)(s.SameDayQuantity)) ?? 0;
                amazonDaySameDayPrice = amazonDaySameDayPrice + amazonRoutes.Sum(s => (double?)(s.SameDayPrice)) ?? 0;
                amazonDayTrainingDayQuantity = amazonDayTrainingDayQuantity + amazonRoutes.Sum(s => (double?)(s.TrainingDayQuantity)) ?? 0;
                amazonDayTrainingDayPrice = amazonDayTrainingDayPrice + amazonRoutes.Sum(s => (double?)(s.TrainingDayPrice)) ?? 0;
                amazonDayRideAlongQuantity = amazonDayRideAlongQuantity + amazonRoutes.Sum(s => (double?)(s.RideAlongQuantity)) ?? 0;
                amazonDayRideAlongPrice = amazonDayRideAlongPrice + amazonRoutes.Sum(s => (double?)(s.RideAlongPrice)) ?? 0;
                amazonDayCongestionChargeQuantity = amazonDayCongestionChargeQuantity + amazonRoutes.Sum(s => (double?)(s.CongestionChargeQuantity)) ?? 0;
                amazonDayCongestionChargePrice = amazonDayCongestionChargePrice + amazonRoutes.Sum(s => (double?)(s.CongestionChargePrice)) ?? 0;
                amazonDayLatePaymentQuantity = amazonDayLatePaymentQuantity + amazonRoutes.Sum(s => (double?)(s.LatePaymentQuantity)) ?? 0;
                amazonDayLatePaymentPrice = amazonDayLatePaymentPrice + amazonRoutes.Sum(s => (double?)(s.LatePaymentPrice)) ?? 0;
                amazonDayMileage = amazonDayMileage + amazonRoutes.Sum(s => (double?)(s.Mileage)) ?? 0;
                amazonDayFuel = amazonDayFuel + amazonRoutes.Sum(s => (double?)(s.Fuel)) ?? 0;
                //
                sblDayFullQuantity = sblDayFullQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Full" ? 1 : 0) + (x.RouteType2 == "Full" ? 1 : 0) + (x.RouteType3 == "Full" ? 1 : 0) + (x.RouteType4 == "Full" ? 1 : 0) + (x.RouteType5 == "Full" ? 1 : 0))) ?? 0;
                sblDayFullPrice = sblDayFullPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Full" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Full" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Full" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Full" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Full" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayHalfQuantity = sblDayHalfQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Half" ? 1 : 0) + (x.RouteType2 == "Half" ? 1 : 0) + (x.RouteType3 == "Half" ? 1 : 0) + (x.RouteType4 == "Half" ? 1 : 0) + (x.RouteType5 == "Half" ? 1 : 0))) ?? 0;
                sblDayHalfPrice = sblDayHalfPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Half" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Half" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Half" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Half" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Half" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayRemoteDebriefQuantity = sblDayRemoteDebriefQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "RemoteDebrief" ? 1 : 0) + (x.RouteType2 == "RemoteDebrief" ? 1 : 0) + (x.RouteType3 == "RemoteDebrief" ? 1 : 0) + (x.RouteType4 == "RemoteDebrief" ? 1 : 0) + (x.RouteType5 == "RemoteDebrief" ? 1 : 0))) ?? 0;
                sblDayRemoteDebriefPrice = sblDayRemoteDebriefPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "RemoteDebrief" ? x.RoutePrice1 : 0) + (x.RouteType2 == "RemoteDebrief" ? x.RoutePrice2 : 0) + (x.RouteType3 == "RemoteDebrief" ? x.RoutePrice3 : 0) + (x.RouteType4 == "RemoteDebrief" ? x.RoutePrice4 : 0) + (x.RouteType5 == "RemoteDebrief" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayNurseryRoutesLevel1Quantity = sblDayNurseryRoutesLevel1Quantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType2 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType3 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType4 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType5 == "NurseryRoutesLevel1" ? 1 : 0))) ?? 0;
                sblDayNurseryRoutesLevel1Price = sblDayNurseryRoutesLevel1Price + sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel1" ? x.RoutePrice1 : 0) + (x.RouteType2 == "NurseryRoutesLevel1" ? x.RoutePrice2 : 0) + (x.RouteType3 == "NurseryRoutesLevel1" ? x.RoutePrice3 : 0) + (x.RouteType4 == "NurseryRoutesLevel1" ? x.RoutePrice4 : 0) + (x.RouteType5 == "NurseryRoutesLevel1" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayNurseryRoutesLevel2Quantity = sblDayNurseryRoutesLevel2Quantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType2 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType3 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType4 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType5 == "NurseryRoutesLevel2" ? 1 : 0))) ?? 0;
                sblDayNurseryRoutesLevel2Price = sblDayNurseryRoutesLevel2Price + sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel2" ? x.RoutePrice1 : 0) + (x.RouteType2 == "NurseryRoutesLevel2" ? x.RoutePrice2 : 0) + (x.RouteType3 == "NurseryRoutesLevel2" ? x.RoutePrice3 : 0) + (x.RouteType4 == "NurseryRoutesLevel2" ? x.RoutePrice4 : 0) + (x.RouteType5 == "NurseryRoutesLevel2" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayRescue2HoursQuantity = sblDayRescue2HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue2Hours" ? 1 : 0) + (x.RouteType2 == "Rescue2Hours" ? 1 : 0) + (x.RouteType3 == "Rescue2Hours" ? 1 : 0) + (x.RouteType4 == "Rescue2Hours" ? 1 : 0) + (x.RouteType5 == "Rescue2Hours" ? 1 : 0))) ?? 0;
                sblDayRescue2HoursPrice = sblDayRescue2HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue2Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Rescue2Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Rescue2Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Rescue2Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Rescue2Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayRescue4HoursQuantity = sblDayRescue4HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue4Hours" ? 1 : 0) + (x.RouteType2 == "Rescue4Hours" ? 1 : 0) + (x.RouteType3 == "Rescue4Hours" ? 1 : 0) + (x.RouteType4 == "Rescue4Hours" ? 1 : 0) + (x.RouteType5 == "Rescue4Hours" ? 1 : 0))) ?? 0;
                sblDayRescue4HoursPrice = sblDayRescue4HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue4Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Rescue4Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Rescue4Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Rescue4Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Rescue4Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayRescue6HoursQuantity = sblDayRescue6HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue6Hours" ? 1 : 0) + (x.RouteType2 == "Rescue6Hours" ? 1 : 0) + (x.RouteType3 == "Rescue6Hours" ? 1 : 0) + (x.RouteType4 == "Rescue6Hours" ? 1 : 0) + (x.RouteType5 == "Rescue6Hours" ? 1 : 0))) ?? 0;
                sblDayRescue6HoursPrice = sblDayRescue6HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue6Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Rescue6Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Rescue6Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Rescue6Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Rescue6Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayReDelivery2HoursQuantity = sblDayReDelivery2HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType2 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType3 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType4 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType5 == "ReDelivery2Hours" ? 1 : 0))) ?? 0;
                sblDayReDelivery2HoursPrice = sblDayReDelivery2HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery2Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "ReDelivery2Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "ReDelivery2Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "ReDelivery2Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "ReDelivery2Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayReDelivery4HoursQuantity = sblDayReDelivery4HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType2 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType3 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType4 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType5 == "ReDelivery4Hours" ? 1 : 0))) ?? 0;
                sblDayReDelivery4HoursPrice = sblDayReDelivery4HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery4Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "ReDelivery4Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "ReDelivery4Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "ReDelivery4Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "ReDelivery4Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayReDelivery6HoursQuantity = sblDayReDelivery6HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType2 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType3 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType4 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType5 == "ReDelivery6Hours" ? 1 : 0))) ?? 0;
                sblDayReDelivery6HoursPrice = sblDayReDelivery6HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery6Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "ReDelivery6Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "ReDelivery6Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "ReDelivery6Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "ReDelivery6Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayMissort2HoursQuantity = sblDayMissort2HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort2Hours" ? 1 : 0) + (x.RouteType2 == "Missort2Hours" ? 1 : 0) + (x.RouteType3 == "Missort2Hours" ? 1 : 0) + (x.RouteType4 == "Missort2Hours" ? 1 : 0) + (x.RouteType5 == "Missort2Hours" ? 1 : 0))) ?? 0;
                sblDayMissort2HoursPrice = sblDayMissort2HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort2Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Missort2Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Missort2Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Missort2Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Missort2Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayMissort4HoursQuantity = sblDayMissort4HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort4Hours" ? 1 : 0) + (x.RouteType2 == "Missort4Hours" ? 1 : 0) + (x.RouteType3 == "Missort4Hours" ? 1 : 0) + (x.RouteType4 == "Missort4Hours" ? 1 : 0) + (x.RouteType5 == "Missort4Hours" ? 1 : 0))) ?? 0;
                sblDayMissort4HoursPrice = sblDayMissort4HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort4Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Missort4Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Missort4Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Missort4Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Missort4Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayMissort6HoursQuantity = sblDayMissort6HoursQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort6Hours" ? 1 : 0) + (x.RouteType2 == "Missort6Hours" ? 1 : 0) + (x.RouteType3 == "Missort6Hours" ? 1 : 0) + (x.RouteType4 == "Missort6Hours" ? 1 : 0) + (x.RouteType5 == "Missort6Hours" ? 1 : 0))) ?? 0;
                sblDayMissort6HoursPrice = sblDayMissort6HoursPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort6Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Missort6Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Missort6Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Missort6Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Missort6Hours" ? x.RoutePrice5 : 0))) ?? 0;
                sblDaySameDayQuantity = sblDaySameDayQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "SameDay" ? 1 : 0) + (x.RouteType2 == "SameDay" ? 1 : 0) + (x.RouteType3 == "SameDay" ? 1 : 0) + (x.RouteType4 == "SameDay" ? 1 : 0) + (x.RouteType5 == "SameDay" ? 1 : 0))) ?? 0;
                sblDaySameDayPrice = sblDaySameDayPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "SameDay" ? x.RoutePrice1 : 0) + (x.RouteType2 == "SameDay" ? x.RoutePrice2 : 0) + (x.RouteType3 == "SameDay" ? x.RoutePrice3 : 0) + (x.RouteType4 == "SameDay" ? x.RoutePrice4 : 0) + (x.RouteType5 == "SameDay" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayTrainingDayQuantity = sblDayTrainingDayQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "TrainingDay" ? 1 : 0) + (x.RouteType2 == "TrainingDay" ? 1 : 0) + (x.RouteType3 == "TrainingDay" ? 1 : 0) + (x.RouteType4 == "TrainingDay" ? 1 : 0) + (x.RouteType5 == "TrainingDay" ? 1 : 0))) ?? 0;
                sblDayTrainingDayPrice = sblDayTrainingDayPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "TrainingDay" ? x.RoutePrice1 : 0) + (x.RouteType2 == "TrainingDay" ? x.RoutePrice2 : 0) + (x.RouteType3 == "TrainingDay" ? x.RoutePrice3 : 0) + (x.RouteType4 == "TrainingDay" ? x.RoutePrice4 : 0) + (x.RouteType5 == "TrainingDay" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayRideAlongQuantity = sblDayRideAlongQuantity + sblRoutes.Sum(x => (double?)((x.RouteType1 == "RideAlong" ? 1 : 0) + (x.RouteType2 == "RideAlong" ? 1 : 0) + (x.RouteType3 == "RideAlong" ? 1 : 0) + (x.RouteType4 == "RideAlong" ? 1 : 0) + (x.RouteType5 == "RideAlong" ? 1 : 0))) ?? 0;
                sblDayRideAlongPrice = sblDayRideAlongPrice + sblRoutes.Sum(x => (double?)((x.RouteType1 == "RideAlong" ? x.RoutePrice1 : 0) + (x.RouteType2 == "RideAlong" ? x.RoutePrice2 : 0) + (x.RouteType3 == "RideAlong" ? x.RoutePrice3 : 0) + (x.RouteType4 == "RideAlong" ? x.RoutePrice4 : 0) + (x.RouteType5 == "RideAlong" ? x.RoutePrice5 : 0))) ?? 0;
                sblDayCongestionChargeQuantity = sblDayCongestionChargeQuantity + sblRoutes.Sum(x => (double?)(x.CongestionChargeQuantity)) ?? 0;
                sblDayCongestionChargePrice = sblDayCongestionChargePrice + sblRoutes.Sum(x => (double?)(x.CongestionChargePrice)) ?? 0;
                sblDayLatePaymentQuantity = sblDayLatePaymentQuantity + sblRoutes.Sum(x => (double?)(x.LatePaymentQuantity)) ?? 0;
                sblDayLatePaymentPrice = sblDayLatePaymentPrice + sblRoutes.Sum(x => (double?)(x.LatePaymentPrice)) ?? 0;
                sblDayMileage = sblDayMileage + sblRoutes.Sum(x => (double?)(x.Mileage)) ?? 0;
                sblDayFuel = sblDayFuel + sblRoutes.Sum(x => (double?)(x.FuelChargePrice)) ?? 0;
                //
                diffDayFullQuantity = diffDayFullQuantity + (amazonRoutes.Sum(s => (double?)(s.FullQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Full" ? 1 : 0) + (x.RouteType2 == "Full" ? 1 : 0) + (x.RouteType3 == "Full" ? 1 : 0) + (x.RouteType4 == "Full" ? 1 : 0) + (x.RouteType5 == "Full" ? 1 : 0))) ?? 0);
                diffDayFullPrice = diffDayFullPrice + (amazonRoutes.Sum(s => (double?)(s.FullPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Full" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Full" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Full" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Full" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Full" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayHalfQuantity = diffDayHalfQuantity + (amazonRoutes.Sum(s => (double?)(s.HalfQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Half" ? 1 : 0) + (x.RouteType2 == "Half" ? 1 : 0) + (x.RouteType3 == "Half" ? 1 : 0) + (x.RouteType4 == "Half" ? 1 : 0) + (x.RouteType5 == "Half" ? 1 : 0))) ?? 0);
                diffDayHalfPrice = diffDayHalfPrice + (amazonRoutes.Sum(s => (double?)(s.HalfPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Half" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Half" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Half" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Half" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Half" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayRemoteDebriefQuantity = diffDayRemoteDebriefQuantity + (amazonRoutes.Sum(s => (double?)(s.RemoteDebriefQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "RemoteDebrief" ? 1 : 0) + (x.RouteType2 == "RemoteDebrief" ? 1 : 0) + (x.RouteType3 == "RemoteDebrief" ? 1 : 0) + (x.RouteType4 == "RemoteDebrief" ? 1 : 0) + (x.RouteType5 == "RemoteDebrief" ? 1 : 0))) ?? 0);
                diffDayRemoteDebriefPrice = diffDayRemoteDebriefPrice + (amazonRoutes.Sum(s => (double?)(s.RemoteDebriefPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "RemoteDebrief" ? x.RoutePrice1 : 0) + (x.RouteType2 == "RemoteDebrief" ? x.RoutePrice2 : 0) + (x.RouteType3 == "RemoteDebrief" ? x.RoutePrice3 : 0) + (x.RouteType4 == "RemoteDebrief" ? x.RoutePrice4 : 0) + (x.RouteType5 == "RemoteDebrief" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayNurseryRoutesLevel1Quantity = diffDayNurseryRoutesLevel1Quantity + (amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel1Quantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType2 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType3 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType4 == "NurseryRoutesLevel1" ? 1 : 0) + (x.RouteType5 == "NurseryRoutesLevel1" ? 1 : 0))) ?? 0);
                diffDayNurseryRoutesLevel1Price = diffDayNurseryRoutesLevel1Price + (amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel1Price) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel1" ? x.RoutePrice1 : 0) + (x.RouteType2 == "NurseryRoutesLevel1" ? x.RoutePrice2 : 0) + (x.RouteType3 == "NurseryRoutesLevel1" ? x.RoutePrice3 : 0) + (x.RouteType4 == "NurseryRoutesLevel1" ? x.RoutePrice4 : 0) + (x.RouteType5 == "NurseryRoutesLevel1" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayNurseryRoutesLevel2Quantity = diffDayNurseryRoutesLevel2Quantity + (amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel2Quantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType2 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType3 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType4 == "NurseryRoutesLevel2" ? 1 : 0) + (x.RouteType5 == "NurseryRoutesLevel2" ? 1 : 0))) ?? 0);
                diffDayNurseryRoutesLevel2Price = diffDayNurseryRoutesLevel2Price + (amazonRoutes.Sum(s => (double?)(s.NurseryRoutesLevel2Price) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "NurseryRoutesLevel2" ? x.RoutePrice1 : 0) + (x.RouteType2 == "NurseryRoutesLevel2" ? x.RoutePrice2 : 0) + (x.RouteType3 == "NurseryRoutesLevel2" ? x.RoutePrice3 : 0) + (x.RouteType4 == "NurseryRoutesLevel2" ? x.RoutePrice4 : 0) + (x.RouteType5 == "NurseryRoutesLevel2" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayRescue2HoursQuantity = diffDayRescue2HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.Rescue2HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue2Hours" ? 1 : 0) + (x.RouteType2 == "Rescue2Hours" ? 1 : 0) + (x.RouteType3 == "Rescue2Hours" ? 1 : 0) + (x.RouteType4 == "Rescue2Hours" ? 1 : 0) + (x.RouteType5 == "Rescue2Hours" ? 1 : 0))) ?? 0);
                diffDayRescue2HoursPrice = diffDayRescue2HoursPrice + (amazonRoutes.Sum(s => (double?)(s.Rescue2HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue2Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Rescue2Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Rescue2Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Rescue2Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Rescue2Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayRescue4HoursQuantity = diffDayRescue4HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.Rescue4HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue4Hours" ? 1 : 0) + (x.RouteType2 == "Rescue4Hours" ? 1 : 0) + (x.RouteType3 == "Rescue4Hours" ? 1 : 0) + (x.RouteType4 == "Rescue4Hours" ? 1 : 0) + (x.RouteType5 == "Rescue4Hours" ? 1 : 0))) ?? 0);
                diffDayRescue4HoursPrice = diffDayRescue4HoursPrice + (amazonRoutes.Sum(s => (double?)(s.Rescue4HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue4Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Rescue4Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Rescue4Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Rescue4Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Rescue4Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayRescue6HoursQuantity = diffDayRescue6HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.Rescue6HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue6Hours" ? 1 : 0) + (x.RouteType2 == "Rescue6Hours" ? 1 : 0) + (x.RouteType3 == "Rescue6Hours" ? 1 : 0) + (x.RouteType4 == "Rescue6Hours" ? 1 : 0) + (x.RouteType5 == "Rescue6Hours" ? 1 : 0))) ?? 0);
                diffDayRescue6HoursPrice = diffDayRescue6HoursPrice + (amazonRoutes.Sum(s => (double?)(s.Rescue6HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Rescue6Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Rescue6Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Rescue6Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Rescue6Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Rescue6Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayReDelivery2HoursQuantity = diffDayReDelivery2HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.ReDelivery2HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType2 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType3 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType4 == "ReDelivery2Hours" ? 1 : 0) + (x.RouteType5 == "ReDelivery2Hours" ? 1 : 0))) ?? 0);
                diffDayReDelivery2HoursPrice = diffDayReDelivery2HoursPrice + (amazonRoutes.Sum(s => (double?)(s.ReDelivery2HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery2Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "ReDelivery2Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "ReDelivery2Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "ReDelivery2Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "ReDelivery2Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayReDelivery4HoursQuantity = diffDayReDelivery4HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.ReDelivery4HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType2 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType3 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType4 == "ReDelivery4Hours" ? 1 : 0) + (x.RouteType5 == "ReDelivery4Hours" ? 1 : 0))) ?? 0);
                diffDayReDelivery4HoursPrice = diffDayReDelivery4HoursPrice + (amazonRoutes.Sum(s => (double?)(s.ReDelivery4HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery4Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "ReDelivery4Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "ReDelivery4Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "ReDelivery4Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "ReDelivery4Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayReDelivery6HoursQuantity = diffDayReDelivery6HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.ReDelivery6HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType2 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType3 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType4 == "ReDelivery6Hours" ? 1 : 0) + (x.RouteType5 == "ReDelivery6Hours" ? 1 : 0))) ?? 0);
                diffDayReDelivery6HoursPrice = diffDayReDelivery6HoursPrice + (amazonRoutes.Sum(s => (double?)(s.ReDelivery6HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "ReDelivery6Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "ReDelivery6Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "ReDelivery6Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "ReDelivery6Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "ReDelivery6Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayMissort2HoursQuantity = diffDayMissort2HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.Missort2HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort2Hours" ? 1 : 0) + (x.RouteType2 == "Missort2Hours" ? 1 : 0) + (x.RouteType3 == "Missort2Hours" ? 1 : 0) + (x.RouteType4 == "Missort2Hours" ? 1 : 0) + (x.RouteType5 == "Missort2Hours" ? 1 : 0))) ?? 0);
                diffDayMissort2HoursPrice = diffDayMissort2HoursPrice + (amazonRoutes.Sum(s => (double?)(s.Missort2HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort2Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Missort2Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Missort2Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Missort2Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Missort2Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayMissort4HoursQuantity = diffDayMissort4HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.Missort4HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort4Hours" ? 1 : 0) + (x.RouteType2 == "Missort4Hours" ? 1 : 0) + (x.RouteType3 == "Missort4Hours" ? 1 : 0) + (x.RouteType4 == "Missort4Hours" ? 1 : 0) + (x.RouteType5 == "Missort4Hours" ? 1 : 0))) ?? 0);
                diffDayMissort4HoursPrice = diffDayMissort4HoursPrice + (amazonRoutes.Sum(s => (double?)(s.Missort4HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort4Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Missort4Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Missort4Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Missort4Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Missort4Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayMissort6HoursQuantity = diffDayMissort6HoursQuantity + (amazonRoutes.Sum(s => (double?)(s.Missort6HoursQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort6Hours" ? 1 : 0) + (x.RouteType2 == "Missort6Hours" ? 1 : 0) + (x.RouteType3 == "Missort6Hours" ? 1 : 0) + (x.RouteType4 == "Missort6Hours" ? 1 : 0) + (x.RouteType5 == "Missort6Hours" ? 1 : 0))) ?? 0);
                diffDayMissort6HoursPrice = diffDayMissort6HoursPrice + (amazonRoutes.Sum(s => (double?)(s.Missort6HoursPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "Missort6Hours" ? x.RoutePrice1 : 0) + (x.RouteType2 == "Missort6Hours" ? x.RoutePrice2 : 0) + (x.RouteType3 == "Missort6Hours" ? x.RoutePrice3 : 0) + (x.RouteType4 == "Missort6Hours" ? x.RoutePrice4 : 0) + (x.RouteType5 == "Missort6Hours" ? x.RoutePrice5 : 0))) ?? 0);
                diffDaySameDayQuantity = diffDaySameDayQuantity + (amazonRoutes.Sum(s => (double?)(s.SameDayQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "SameDay" ? 1 : 0) + (x.RouteType2 == "SameDay" ? 1 : 0) + (x.RouteType3 == "SameDay" ? 1 : 0) + (x.RouteType4 == "SameDay" ? 1 : 0) + (x.RouteType5 == "SameDay" ? 1 : 0))) ?? 0);
                diffDaySameDayPrice = diffDaySameDayPrice + (amazonRoutes.Sum(s => (double?)(s.SameDayPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "SameDay" ? x.RoutePrice1 : 0) + (x.RouteType2 == "SameDay" ? x.RoutePrice2 : 0) + (x.RouteType3 == "SameDay" ? x.RoutePrice3 : 0) + (x.RouteType4 == "SameDay" ? x.RoutePrice4 : 0) + (x.RouteType5 == "SameDay" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayTrainingDayQuantity = diffDayTrainingDayQuantity + (amazonRoutes.Sum(s => (double?)(s.TrainingDayQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "TrainingDay" ? 1 : 0) + (x.RouteType2 == "TrainingDay" ? 1 : 0) + (x.RouteType3 == "TrainingDay" ? 1 : 0) + (x.RouteType4 == "TrainingDay" ? 1 : 0) + (x.RouteType5 == "TrainingDay" ? 1 : 0))) ?? 0);
                diffDayTrainingDayPrice = diffDayTrainingDayPrice + (amazonRoutes.Sum(s => (double?)(s.TrainingDayPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "TrainingDay" ? x.RoutePrice1 : 0) + (x.RouteType2 == "TrainingDay" ? x.RoutePrice2 : 0) + (x.RouteType3 == "TrainingDay" ? x.RoutePrice3 : 0) + (x.RouteType4 == "TrainingDay" ? x.RoutePrice4 : 0) + (x.RouteType5 == "TrainingDay" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayRideAlongQuantity = diffDayRideAlongQuantity + (amazonRoutes.Sum(s => (double?)(s.RideAlongQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "RideAlong" ? 1 : 0) + (x.RouteType2 == "RideAlong" ? 1 : 0) + (x.RouteType3 == "RideAlong" ? 1 : 0) + (x.RouteType4 == "RideAlong" ? 1 : 0) + (x.RouteType5 == "RideAlong" ? 1 : 0))) ?? 0);
                diffDayRideAlongPrice = diffDayRideAlongPrice + (amazonRoutes.Sum(s => (double?)(s.RideAlongPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)((x.RouteType1 == "RideAlong" ? x.RoutePrice1 : 0) + (x.RouteType2 == "RideAlong" ? x.RoutePrice2 : 0) + (x.RouteType3 == "RideAlong" ? x.RoutePrice3 : 0) + (x.RouteType4 == "RideAlong" ? x.RoutePrice4 : 0) + (x.RouteType5 == "RideAlong" ? x.RoutePrice5 : 0))) ?? 0);
                diffDayCongestionChargeQuantity = diffDayCongestionChargeQuantity + (amazonRoutes.Sum(s => (double?)(s.CongestionChargeQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)(x.CongestionChargeQuantity)) ?? 0);
                diffDayCongestionChargePrice = diffDayCongestionChargePrice + (amazonRoutes.Sum(s => (double?)(s.CongestionChargePrice) ?? 0)) - (sblRoutes.Sum(x => (double?)(x.CongestionChargePrice)) ?? 0);
                diffDayLatePaymentQuantity = diffDayLatePaymentQuantity + (amazonRoutes.Sum(s => (double?)(s.LatePaymentQuantity) ?? 0)) - (sblRoutes.Sum(x => (double?)(x.LatePaymentQuantity)) ?? 0);
                diffDayLatePaymentPrice = diffDayLatePaymentPrice + (amazonRoutes.Sum(s => (double?)(s.LatePaymentPrice) ?? 0)) - (sblRoutes.Sum(x => (double?)(x.LatePaymentPrice)) ?? 0);
                diffDayMileage = diffDayMileage + (amazonRoutes.Sum(s => (double?)(s.Mileage) ?? 0)) - (sblRoutes.Sum(x => (double?)(x.Mileage)) ?? 0);
                diffDayFuel = diffDayFuel + (amazonRoutes.Sum(s => (double?)(s.Fuel) ?? 0)) - (sblRoutes.Sum(x => (double?)(x.FuelChargePrice)) ?? 0);
            }

          return 
        }

        */
     



        #endregion




        #region send emails


        public static void SendMessage(List<string> To, List<string> Cc, List<string> Bcc, string Subject, string Body, bool HasAttachmentFile, List<string> AttachmentFile, bool HasAttachmentMs, List<MsAttachment> AttachmentMs)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            string SMTPHost = "mail.overssl.net";
            string SMTPUsername = "smtp@smartappscode.com";
            string SMTPPassword = "letmein";
            int SMTPPort = 25;
            bool SMTPEnableSSL = false;

            string FromEmail = "smtp@smartappscode.com";
            string DisplayName = "SBL Couriers";
            string ReplyToEmail = "smtp@smartappscode.com";
            string BccEmail = "smtp@smartappscode.com";

            //

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(FromEmail, DisplayName);

            foreach (var to in To)
            {
                mail.To.Add(to);
            }

            foreach (var cc in Cc)
            {
                mail.CC.Add(cc);
            }

            foreach (var bcc in Bcc)
            {
                mail.Bcc.Add(bcc);
            }

            if (!String.IsNullOrEmpty(BccEmail))
            {
                mail.Bcc.Add(BccEmail);
            }

            mail.ReplyToList.Add(new MailAddress(ReplyToEmail, DisplayName));
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;

            // attachments file
            if (HasAttachmentFile == true)
            {
                foreach (var attachmentfile in AttachmentFile)
                {
                    if (File.Exists(attachmentfile))
                    {
                        Attachment file = new Attachment(attachmentfile);
                        mail.Attachments.Add(file);
                    }
                }

                //file.Dispose();
            }

            // attachments ms
            if (HasAttachmentMs == true)
            {
                foreach (var attachmentms in AttachmentMs)
                {
                    mail.Attachments.Add(new Attachment(attachmentms.Ms, attachmentms.FileName, attachmentms.ContentType));
                }

                //file.Dispose();
            }

            SmtpClient smtpServer = new SmtpClient(SMTPHost);
            smtpServer.Port = SMTPPort;
            smtpServer.EnableSsl = SMTPEnableSSL;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
            smtpServer.Send(mail);
            smtpServer.Dispose();
        }


        public static bool SendRemittanceToAssociate(string Email, string Subject, string Body)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var emailsent = false;
            bool hasattachmentdb = false;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<MsAttachment> attachmentms = new List<MsAttachment>();

            string subject = Subject;

            StringBuilder sb = new StringBuilder();
            sb.Append(Body);

            string body = sb.ToString();

            to.Add(Email);

            if (to.Any() || cc.Any() || bcc.Any())
            {
                emailsent = true;
                SendMessage(to, cc, bcc, subject, body, false, null, hasattachmentdb, attachmentms);
            }

            return emailsent;
        }


        #endregion



    }
}