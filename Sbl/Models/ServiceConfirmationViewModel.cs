using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class ServiceConfirmationViewModel
    {
        public int? DepotId { get; set; }

        public DateTime? DateStart { get; set; }

        public int? Days { get; set; }





        public bool HasWritePermision { get; set; }


        public List<Period> Periods { get; set; }
        public class Period
        {
            public int Year { get; set; }
            public int Week { get; set; }
            public DateTime? DateStart { get; set; }
            public DateTime? DateEnd { get; set; }
        }


        public List<Depot> Depots { get; set; }
        public class Depot
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        public List<Route> Routes { get; set; }
        public class Route
        {
            public int Id { get; set; }
            public DateTime? RouteDate { get; set; }
            public int? DriverId { get; set; }
            public string DriverName { get; set; }
            public int? DepotId { get; set; }
            public string DepotName { get; set; }
            public string RouteCode { get; set; }
            public string RouteType { get; set; }
            public double RoutePrice { get; set; }
            public double Ad1Quantity { get; set; }
            public double Ad1Price { get; set; }
            public double Ad2Quantity { get; set; }
            public double Ad2Price { get; set; }
            public double Ad3Quantity { get; set; }
            public double Ad3Price { get; set; }
            public double Fuel { get; set; }
            public double Mileage { get; set; }
            public double Deduct { get; set; }
            public string Notes { get; set; }
        }



        public List<Helpers.Functions.ServiceList> Day1 { get; set; }





        #region amazon week
        public double AmazonWeekFullAndHalfQuantity { get; set; }
        public double AmazonWeekFullAndHalfPrice { get; set; }
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
        public double AmazonWeekDeduct { get; set; }

        #endregion


        #region sbl week
        public double SblWeekFullAndHalfQuantity { get; set; }
        public double SblWeekFullAndHalfPrice { get; set; }
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
        public double SblWeekDeduct { get; set; }

        #endregion


        #region diff week
        public double DiffWeekFullAndHalfQuantity { get; set; }
        public double DiffWeekFullAndHalfPrice { get; set; }
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
        public double DiffWeekDeduct { get; set; }

        #endregion



        public List<DayTotal> DayTotals { get; set; }
        public class DayTotal
        {
            public int RouteAmazonId { get; set; }
            public DateTime? RouteDate { get; set; }
            public int? DepotId { get; set; }
            

            #region amazon day

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
            public double AmazonDayDeduct { get; set; }

            #endregion


            #region sbl day
            
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
            public double SblDayDeduct { get; set; }

            #endregion


            #region diff day

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
            public double DiffDayDeduct { get; set; }

            #endregion

        }


        // totals
        public double AmazonWeekTotalQuantity { get; set; }
        public double AmazonWeekTotalPrice { get; set; }
        public double SblWeekTotalQuantity { get; set; }
        public double SblWeekTotalPrice { get; set; }
        public double DiffWeekTotalQuantity { get; set; }
        public double DiffWeekTotalPrice { get; set; }

         



        public List<Total> Totals { get; set; }
        public class Total
        {
            // amazon day
            public int RouteAmazonId { get; set; }
            public DateTime? RouteDate { get; set; }
            public int? DepotId { get; set; }
            //
            public double AmazonDayFullAndHalfQuantity { get; set; }
            public double AmazonDayFullAndHalfPrice { get; set; }
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
            public double AmazonDayDeduct { get; set; }



            // amazon week
            //
            public double AmazonWeekFullAndHalfQuantity { get; set; }
            public double AmazonWeekFullAndHalfPrice { get; set; }
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
            public double AmazonWeekDeduct { get; set; }




            // sbl day
            //
            public double SblDayFullAndHalfQuantity { get; set; }
            public double SblDayFullAndHalfPrice { get; set; }
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
            public double SblDayDeduct { get; set; }



            // sbl week
            //
            public double SblWeekFullAndHalfQuantity { get; set; }
            public double SblWeekFullAndHalfPrice { get; set; }
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
            public double SblWeekDeduct { get; set; }





            // diff day
            //
            public double DiffDayFullAndHalfQuantity { get; set; }
            public double DiffDayFullAndHalfPrice { get; set; }
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
            public double DiffDayDeduct { get; set; }



            // diff week
            //
            public double DiffWeekFullAndHalfQuantity { get; set; }
            public double DiffWeekFullAndHalfPrice { get; set; }
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
            public double DiffWeekDeduct { get; set; }



            // totals
            public double AmazonWeekTotalQuantity { get; set; }
            public double AmazonWeekTotalPrice { get; set; }
            public double SblWeekTotalQuantity { get; set; }
            public double SblWeekTotalPrice { get; set; }
            public double DiffWeekTotalQuantity { get; set; }
            public double DiffWeekTotalPrice { get; set; }

        }



    }
}