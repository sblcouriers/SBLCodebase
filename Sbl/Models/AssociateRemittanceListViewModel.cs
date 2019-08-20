using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Sbl.Models
{
    public class AssociateRemittanceListViewModel
    {


        public int? AssociateId { get; set; }

        public int? DepotId { get; set; }

        public DateTime? DateStart { get; set; }


        public string SelectedStatus { get; set; }

        //



        public List<Remittance> Remittances { get; set; }
        public class Remittance
        {
            public int AssociateId { get; set; }
            public string AssociateName { get; set; }

            public int? AssociateDepotId { get; set; }
            public string AssociateDepotName { get; set; }


            public int? RouteCount { get; set; }
            public double RoutePay { get; set; }
            public double AdditionalPay { get; set; }
            public double ByodPay { get; set; }
            public double DpmoBonusPay { get; set; }
            public double TotalMiles { get; set; }
            public double MileagePay { get; set; }
            public double OtherPay { get; set; }

            public double OtherDeductionsPay { get; set; }
            public double VanRentalPay { get; set; }
            public double VanInsurancePay { get; set; }
            public double DamageChargesPay { get; set; }
            public double VanFinesPay { get; set; }
            public double CoFuelUsedPay { get; set; }
            public double LoanPay { get; set; }

            public double NetPay { get; set; }
            public double SecuritaxPay { get; set; }
            public double GrossPay { get; set; }


            public bool AuthPoc { get; set; }
            public bool AuthPayroll { get; set; }
            public bool AuthAdmin { get; set; }

            public double Total { get; set; }

            public int ReceiptSent { get; set; }
            public int RemittanceSent { get; set; }


            public List<Route> Routes { get; set; }
        }




 
 


        // routes

        public double SumMileage { get; set; }
        public double SumByod { get; set; }
        public double SumRouteRate { get; set; }
        public double SumRouteExtraRate { get; set; }
        public double SumFuelSupport { get; set; }
        public double SumSubTotal { get; set; }

        public List<Route> Routes { get; set; }
        public class Route
        {
            public int Id { get; set; }
            public DateTime? RouteDate { get; set; }
            public string RouteType1 { get; set; }
            public string RouteCode1 { get; set; }
            public string RouteExtra { get; set; }
            public string Depot { get; set; }
            public double Mileage { get; set; }
            public double Byod { get; set; }
            public double RouteRate { get; set; }
            public double RouteExtraRate { get; set; }
            public double FuelSupport { get; set; }
            public double SubTotal { get; set; }
            // delete?
            public string AllocationStatus { get; set; }
            public bool AuthPoc { get; set; }
            public bool AuthPayroll { get; set; }
            public bool AuthAdmin { get; set; }

            public string Note { get; set; }

        }


        // credits

        public double SumCreditAmount { get; set; }

        public List<Credit> Credits { get; set; }
        public class Credit
        {
            public int Id { get; set; }
            public DateTime? CreditDate { get; set; }
            public string Description { get; set; }
            public double CreditAmount { get; set; }
        }



        // deductions

        public double SumDeductionAmount { get; set; }

        public List<Deduction> Deductions { get; set; }
        public class Deduction
        {
            public int Id { get; set; }
            public DateTime? DeductionDate { get; set; }
            public string Description { get; set; }
            public double DeductionAmount { get; set; }
        }


        // subrentals

        public double SumSubRentalAmount { get; set; }

        public List<SubRental> SubRentals { get; set; }
        public class SubRental
        {
            public DateTime? DateRented { get; set; }
            public DateTime? DateReturned { get; set; }
            public string VanName { get; set; }
            //
            public string VanRentalDates { get; set; }
            public int VanRentalDays { get; set; }
            public string VanRentalDescription { get; set; }
            public double VanRentalPrice { get; set; }
            public string InsuranceDescription { get; set; }
            public double InsurancePrice { get; set; }
            public string GoodsInTransitDescription { get; set; }
            public double GoodsInTransitPrice { get; set; }
            public string PublicLiabilityDescription { get; set; }
            public double PublicLiabilityPrice { get; set; }
            public double SubRentalAmount { get; set; }
        }




        // extra deductions

        public double SumExtraDeductionAmount { get; set; }

        public List<ExtraDeduction> ExtraDeductions { get; set; }
        public class ExtraDeduction
        {
            public int Id { get; set; }
            public DateTime? DeductionDate { get; set; }
            public string Description { get; set; }
            public double DeductionAmount { get; set; }
        }



        // charge claims

        public double SumChargeClaimAmount { get; set; }

        public List<ChargeClaim> ChargeClaims { get; set; }
        public class ChargeClaim
        {
            public int Id { get; set; }
            public DateTime? ChargeClaimDate { get; set; }
            public string Description { get; set; }
            public double ChargeClaimAmount { get; set; }
        }


        // charge pcn

        public double SumChargePcnAmount { get; set; }

        public List<ChargePcn> ChargePcns { get; set; }
        public class ChargePcn
        {
            public int Id { get; set; }
            public DateTime? ChargePcnDate { get; set; }
            public string Description { get; set; }
            public double ChargePcnAmount { get; set; }
        }





        // summary 

        public double TotalDeductions { get; set; }
        public double TotalCredits { get; set; }
        public double Total { get; set; }


        


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



        public List<Associate> Associates { get; set; }
        public class Associate
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


    }
}