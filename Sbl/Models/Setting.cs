using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class Setting
    {
        public int Id { get; set; }

        //

        [Display(Name = "WebApp Name")]
        public string WebAppName { get; set; }


        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }


        [Display(Name = "Induction Code")]
        public string InductionCode { get; set; }


        [Display(Name = "Schedule Edit Days")]
        public int ScheduleEditDays { get; set; }

        //

        [Display(Name = "SBL Remittance Business Name")]
        public string SblRemittanceBusinessName { get; set; }


        [Display(Name = "SBL Remittance Business Address")]
        public string SblRemittanceBusinessAddress { get; set; }


        [Display(Name = "SBL Remittance Business City")]
        public string SblRemittanceBusinessCity { get; set; }


        [Display(Name = "SBL Remittance Business Postcode")]
        public string SblRemittanceBusinessPostcode { get; set; }


        [Display(Name = "SBL Remittance Business VAT Number")]
        public string SblRemittanceBusinessVatNumber { get; set; }


        [Display(Name = "SBL Remittance Thank You Message")]
        public string SblRemittanceThankYouMessage { get; set; }

        //

        [Display(Name = "Full Route Amazon")]
        public double FullRouteAmazon { get; set; }


        [Display(Name = "Half Route Amazon")]
        public double HalfRouteAmazon { get; set; }


        [Display(Name = "Remote Debrief Amazon")]
        public double RemoteDebriefAmazon { get; set; }


        [Display(Name = "Nursery Routes Level 1 Amazon")]
        public double NurseryRoutesLevel1Amazon { get; set; }


        [Display(Name = "Nursery Routes Level 2 Amazon")]
        public double NurseryRoutesLevel2Amazon { get; set; }


        [Display(Name = "Rescue 2 Hours Amazon")]
        public double Rescue2HoursAmazon { get; set; }


        [Display(Name = "Rescue 4 Hours Amazon")]
        public double Rescue4HoursAmazon { get; set; }


        [Display(Name = "Rescue 6 Hours Amazon")]
        public double Rescue6HoursAmazon { get; set; }


        [Display(Name = "Re-delivery 2 Hours Amazon")]
        public double ReDelivery2HoursAmazon { get; set; }


        [Display(Name = "Re-delivery 4 Hours Amazon")]
        public double ReDelivery4HoursAmazon { get; set; }


        [Display(Name = "Re-delivery 6 Hours Amazon")]
        public double ReDelivery6HoursAmazon { get; set; }


        [Display(Name = "Missort 2 Hours Amazon")]
        public double Missort2HoursAmazon { get; set; }


        [Display(Name = "Missort 4 Hours Amazon")]
        public double Missort4HoursAmazon { get; set; }


        [Display(Name = "Missort 6 Hours Amazon")]
        public double Missort6HoursAmazon { get; set; }


        [Display(Name = "Sameday Amazon")]
        public double SamedayAmazon { get; set; }


        [Display(Name = "Training Day Amazon")]
        public double TrainingDayAmazon { get; set; }


        [Display(Name = "Ride Along Amazon")]
        public double RideAlongAmazon { get; set; }


        [Display(Name = "Support AD1 Amazon")]
        public double SupportAd1Amazon { get; set; }


        [Display(Name = "Support AD2 Amazon")]
        public double SupportAd2Amazon { get; set; }


        [Display(Name = "Support AD3 Amazon")]
        public double SupportAd3Amazon { get; set; }


        [Display(Name = "Lead Driver Amazon")]
        public double LeadDriverAmazon { get; set; }


        [Display(Name = "Large Van Amazon")]
        public double LargeVanAmazon { get; set; }



        [Display(Name = "Congestion Charge Amazon")]
        public double CongestionChargeAmazon { get; set; }


        [Display(Name = "Late Payment Amazon")]
        public double LatePaymentAmazon { get; set; }



        [Display(Name = "Mileage (1 Mile) Amazon")]
        public double Mileage1MileAmazon { get; set; }


        [Display(Name = "BYOD Amazon")]
        public double BYODAmazon { get; set; }


        //



        [Display(Name = "Full Route SBL")]
        public double FullRouteSBL { get; set; }


        [Display(Name = "Half Route SBL")]
        public double HalfRouteSBL { get; set; }


        [Display(Name = "Remote Debrief SBL")]
        public double RemoteDebriefSBL { get; set; }


        [Display(Name = "Nursery Routes Level 1 SBL")]
        public double NurseryRoutesLevel1SBL { get; set; }


        [Display(Name = "Nursery Routes Level 2 SBL")]
        public double NurseryRoutesLevel2SBL { get; set; }


        [Display(Name = "Rescue 2 Hours SBL")]
        public double Rescue2HoursSBL { get; set; }


        [Display(Name = "Rescue 4 Hours SBL")]
        public double Rescue4HoursSBL { get; set; }


        [Display(Name = "Rescue 6 Hours SBL")]
        public double Rescue6HoursSBL { get; set; }


        [Display(Name = "Re-delivery 2 Hours SBL")]
        public double ReDelivery2HoursSBL { get; set; }


        [Display(Name = "Re-delivery 4 Hours SBL")]
        public double ReDelivery4HoursSBL { get; set; }


        [Display(Name = "Re-delivery 6 Hours SBL")]
        public double ReDelivery6HoursSBL { get; set; }


        [Display(Name = "Missort 2 Hours SBL")]
        public double Missort2HoursSBL { get; set; }


        [Display(Name = "Missort 4 Hours SBL")]
        public double Missort4HoursSBL { get; set; }


        [Display(Name = "Missort 6 Hours SBL")]
        public double Missort6HoursSBL { get; set; }


        [Display(Name = "Sameday SBL")]
        public double SamedaySBL { get; set; }


        [Display(Name = "Training Day SBL")]
        public double TrainingDaySBL { get; set; }


        [Display(Name = "Ride Along SBL")]
        public double RideAlongSBL { get; set; }


        [Display(Name = "Support AD1 SBL")]
        public double SupportAd1Sbl { get; set; }


        [Display(Name = "Support AD2 SBL")]
        public double SupportAd2Sbl { get; set; }


        [Display(Name = "Support AD3 SBL")]
        public double SupportAd3Sbl { get; set; }


        [Display(Name = "Lead Driver SBL")]
        public double LeadDriverSbl { get; set; }


        [Display(Name = "Large Van SBL")]
        public double LargeVanSbl { get; set; }


        [Display(Name = "Congestion Charge SBL")]
        public double CongestionChargeSbl { get; set; }


        [Display(Name = "Late Payment SBL")]
        public double LatePaymentSbl { get; set; }


        [Display(Name = "Mileage (1 Mile) SBL")]
        public double Mileage1MileSBL { get; set; }


        [Display(Name = "BYOD SBL")]
        public double BYODSbl { get; set; }


        //


        [Display(Name = "Own Vehicle Deduction")]
        public double OwnVehicleDeductionPrice { get; set; }


        [Display(Name = "Van Rental Price")]
        public double VanRentalPrice { get; set; }


        [Display(Name = "Insurance Price")]
        public double InsurancePrice { get; set; }


        [Display(Name = "Goods in Transit Price")]
        public double GoodsInTransitPrice { get; set; }


        [Display(Name = "Public Liability Price")]
        public double PublicLiabilityPrice { get; set; }



        // delete below?



        [Display(Name = "AD1 Amazon")]
        public double Ad1Amazon { get; set; }


        [Display(Name = "AD2 Amazon")]
        public double Ad2Amazon { get; set; }


        [Display(Name = "AD3 Amazon")]
        public double Ad3Amazon { get; set; }


        [Display(Name = "BYOD")]
        public double BYODPrice { get; set; }


        [Display(Name = "AD1 SBL")]
        public double Ad1Sbl { get; set; }


        [Display(Name = "AD2 SBL")]
        public double Ad2Sbl { get; set; }


        [Display(Name = "AD3 SBL")]
        public double Ad3Sbl { get; set; }


        [Display(Name = "Miles Per Litre")]
        public double MilesPerLitre { get; set; }


        [Display(Name = "Diesel Price")]
        public double DieselPrice { get; set; }


        //

    }
}