using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class RouteAllocation
    {
        public int Id { get; set; }

        //

        [Display(Name = "Associate")]
        public int? AssociateId { get; set; }


        public virtual Associate Associate { get; set; }



        [Display(Name = "Depot")]
        public int? DepotId { get; set; }


        public virtual Depot Depot { get; set; }

        //

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Route Date")]
        public DateTime? RouteDate { get; set; }

        //

        [Display(Name = "Route Code")]
        public string RouteCode1 { get; set; }


        [Display(Name = "Route Type")]
        public string RouteType1 { get; set; }


        [Display(Name = "Route Type Price")]
        public double RoutePrice1 { get; set; }

        //

        [Display(Name = "Route Code")]
        public string RouteCode2 { get; set; }


        [Display(Name = "Route Type")]
        public string RouteType2 { get; set; }


        [Display(Name = "Route Type Price")]
        public double RoutePrice2 { get; set; }

        //

        [Display(Name = "Route Code")]
        public string RouteCode3 { get; set; }


        [Display(Name = "Route Type")]
        public string RouteType3 { get; set; }


        [Display(Name = "Route Type Price")]
        public double RoutePrice3 { get; set; }

        //

        [Display(Name = "Route Code")]
        public string RouteCode4 { get; set; }


        [Display(Name = "Route Type")]
        public string RouteType4 { get; set; }


        [Display(Name = "Route Price")]
        public double RoutePrice4 { get; set; }

        //

        [Display(Name = "Route Code")]
        public string RouteCode5 { get; set; }


        [Display(Name = "Route Type")]
        public string RouteType5 { get; set; }


        [Display(Name = "Route Price")]
        public double RoutePrice5 { get; set; }

        //

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

        //

        [Display(Name = "Mileage")]
        public double Mileage { get; set; }


        [Display(Name = "Fuel Charge")]
        public double FuelChargePrice { get; set; }


        [Display(Name = "BYOD Price")]
        public double BYODPrice { get; set; }


        [Display(Name = "Congestion Charge Quantity")]
        public double CongestionChargeQuantity { get; set; }


        [Display(Name = "Congestion Charge Price")]
        public double CongestionChargePrice { get; set; }


        [Display(Name = "Late Payment Quantity")]
        public double LatePaymentQuantity { get; set; }


        [Display(Name = "Late Payment Price")]
        public double LatePaymentPrice { get; set; }


        //
        // delete
        public double Fuel { get; set; }
        public double Deduct { get; set; }

        //

        [Display(Name = "Start Time")]
        public string StartTime { get; set; }


        [Display(Name = "End Time")]
        public string EndTime { get; set; }


        [Display(Name = "Total Time")]
        public double TotalTime { get; set; }

        //


        [Display(Name = "Notes")]
        public string Notes { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Allocation Status")]
        public string AllocationStatus { get; set; }

        //
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }


        [Display(Name = "Active")]
        public bool Active { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Auth Poc")]
        public bool AuthPoc { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Auth Payroll")]
        public bool AuthPayroll { get; set; }


        [DefaultValue(false)]
        [Display(Name = "Auth Admin")]
        public bool AuthAdmin { get; set; }


        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }
    }
}