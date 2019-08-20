using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class RouteAmazon
    {
        public int Id { get; set; }

        //

        [Display(Name = "Depot")]
        public int? DepotId { get; set; }


        public virtual Depot Depot { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Route Date")]
        public DateTime? RouteDate { get; set; }


        [Display(Name = "Full Quantity")]
        public double FullQuantity { get; set; }


        [Display(Name = "Full Price")]
        public double FullPrice { get; set; }


        [Display(Name = "Half Quantity")]
        public double HalfQuantity { get; set; }


        [Display(Name = "Half Price")]
        public double HalfPrice { get; set; }


        [Display(Name = "Remote Debrief Quantity")]
        public double RemoteDebriefQuantity { get; set; }


        [Display(Name = "Remote Debrief Price")]
        public double RemoteDebriefPrice { get; set; }


        [Display(Name = "Nursery Routes Level 1 Quantity")]
        public double NurseryRoutesLevel1Quantity { get; set; }


        [Display(Name = "Nursery Routes Level 1 Price")]
        public double NurseryRoutesLevel1Price { get; set; }


        [Display(Name = "Nursery Routes Level 2 Quantity")]
        public double NurseryRoutesLevel2Quantity { get; set; }


        [Display(Name = "Nursery Routes Level 2 Price")]
        public double NurseryRoutesLevel2Price { get; set; }


        [Display(Name = "Rescue 2 Hours Quantity")]
        public double Rescue2HoursQuantity { get; set; }


        [Display(Name = "Rescue 2 Hours Price")]
        public double Rescue2HoursPrice { get; set; }


        [Display(Name = "Rescue 4 Hours Quantity")]
        public double Rescue4HoursQuantity { get; set; }


        [Display(Name = "Rescue 4 Hours Price")]
        public double Rescue4HoursPrice { get; set; }


        [Display(Name = "Rescue 6 Hours Quantity")]
        public double Rescue6HoursQuantity { get; set; }


        [Display(Name = "Rescue 6 Hours Price")]
        public double Rescue6HoursPrice { get; set; }


        [Display(Name = "Re-delivery 2 Hours Quantity")]
        public double ReDelivery2HoursQuantity { get; set; }


        [Display(Name = "Re-delivery 2 Hours Price")]
        public double ReDelivery2HoursPrice { get; set; }


        [Display(Name = "Re-delivery 4 Hours Quantity")]
        public double ReDelivery4HoursQuantity { get; set; }


        [Display(Name = "Re-delivery 4 Hours Price")]
        public double ReDelivery4HoursPrice { get; set; }


        [Display(Name = "Re-delivery 6 Hours Quantity")]
        public double ReDelivery6HoursQuantity { get; set; }


        [Display(Name = "Re-delivery 6 Hours Price")]
        public double ReDelivery6HoursPrice { get; set; }


        [Display(Name = "Missort 2 Hours Quantity")]
        public double Missort2HoursQuantity { get; set; }


        [Display(Name = "Missort 2 Hours Price")]
        public double Missort2HoursPrice { get; set; }


        [Display(Name = "Missort 4 Hours Quantity")]
        public double Missort4HoursQuantity { get; set; }


        [Display(Name = "Missort 4 Hours Price")]
        public double Missort4HoursPrice { get; set; }


        [Display(Name = "Missort 6 Hours Quantity")]
        public double Missort6HoursQuantity { get; set; }


        [Display(Name = "Missort 6 Hours Price")]
        public double Missort6HoursPrice { get; set; }


        [Display(Name = "Same Day Quantity")]
        public double SameDayQuantity { get; set; }


        [Display(Name = "Same Day Price")]
        public double SameDayPrice { get; set; }


        [Display(Name = "Training Day Quantity")]
        public double TrainingDayQuantity { get; set; }


        [Display(Name = "Training Day Price")]
        public double TrainingDayPrice { get; set; }


        [Display(Name = "Ride Along Quantity")]
        public double RideAlongQuantity { get; set; }


        [Display(Name = "Ride Along Price")]
        public double RideAlongPrice { get; set; }


        [Display(Name = "Support AD1 Quantity")]
        public double SupportAd1Quantity { get; set; }


        [Display(Name = "Support AD1 Price")]
        public double SupportAd1Price { get; set; }


        [Display(Name = "Support AD2 Quantity")]
        public double SupportAd2Quantity { get; set; }


        [Display(Name = "Support AD2 Price")]
        public double SupportAd2Price { get; set; }


        [Display(Name = "Support AD3 Quantity")]
        public double SupportAd3Quantity { get; set; }


        [Display(Name = "Support AD3 Price")]
        public double SupportAd3Price { get; set; }


        [Display(Name = "Lead Driver Quantity")]
        public double LeadDriverQuantity { get; set; }


        [Display(Name = "Lead Driver Price")]
        public double LeadDriverPrice { get; set; }


        [Display(Name = "Large Van Quantity")]
        public double LargeVanQuantity { get; set; }


        [Display(Name = "Large Van Price")]
        public double LargeVanPrice { get; set; }


        [Display(Name = "Congestion Charge Quantity")]
        public double CongestionChargeQuantity { get; set; }


        [Display(Name = "Congestion Charge Price")]
        public double CongestionChargePrice { get; set; }


        [Display(Name = "Late Payment Quantity")]
        public double LatePaymentQuantity { get; set; }


        [Display(Name = "Late Payment Price")]
        public double LatePaymentPrice { get; set; }
        

        [Display(Name = "AD1 Quantity")]
        public double Ad1Quantity { get; set; }


        [Display(Name = "AD1 Price")]
        public double Ad1Price { get; set; }


        [Display(Name = "AD2 Quantity")]
        public double Ad2Quantity { get; set; }


        [Display(Name = "AD2 Price")]
        public double Ad2Price { get; set; }


        [Display(Name = "AD3 Quantity")]
        public double Ad3Quantity { get; set; }


        [Display(Name = "AD3 Price")]
        public double Ad3Price { get; set; }


        [Display(Name = "Fuel")]
        public double Fuel { get; set; }


        [Display(Name = "Mileage")]
        public double Mileage { get; set; }


        [Display(Name = "Deduct")]
        public double Deduct { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }


        [Display(Name = "Active")]
        public bool Active { get; set; }


        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }
    }
}