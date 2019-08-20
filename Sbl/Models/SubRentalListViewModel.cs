using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models
{
    public class SubRentalListViewModel
    {

        public int? SelectedDepotId { get; set; }
        public int? SelectedAssociateId { get; set; }
        public int? SelectedVehicleId { get; set; }
        public string SelectedVehicleMake { get; set; }
        public string SelectedVehicleModel { get; set; }
        public string SelectedStatus { get; set; }



        public List<SubRental> SubRentals { get; set; }
        public class SubRental
        {
            public int Id { get; set; }
            public int AssociateId { get; set; }
            public string AssociateName { get; set; }
            public int VehicleId { get; set; }
            public string VehicleMake { get; set; }
            public string VehicleModel { get; set; }
            public string VehicleRegistration { get; set; }
            public double MileageStart { get; set; }
            public double MileageEnd { get; set; }
            public double VanRentalPrice { get; set; }
            public double InsurancePrice { get; set; }
            public double GoodsInTransitPrice { get; set; }
            public double PublicLiabilityPrice { get; set; }
            public double RentalPrice { get; set; }
            public DateTime DateRented { get; set; }
            public DateTime? DateReturned { get; set; }
            public string Status { get; set; }
            public DateTime DateCreated { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
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


        public List<Vehicle> Vehicles { get; set; }
        public class Vehicle
        {
            public int Id { get; set; }
            public string VehicleMake { get; set; }
            public string VehicleModel { get; set; }
            public string VehicleRegistration { get; set; }
        }


        public List<VehicleMake> VehicleMakes { get; set; }
        public class VehicleMake
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        public List<VehicleModel> VehicleModels { get; set; }
        public class VehicleModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


    }
}