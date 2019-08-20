using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models
{
    public class HandbookListViewModel
    {
        public int? VehicleId { get; set; }

        public int? DepotId { get; set; }

        public DateTime? DateStart { get; set; }

        public string SelectedStatus { get; set; }

        //


        public List<Vehicle> Vehicles { get; set; }

        public class Vehicle
        {
            public int VehicleId { get; set; }
            public string Make { get; set; }
            public string Model { get; set; }
            public string Registration { get; set; }
            //
            public List<Handbook> Handbooks { get; set; }
        }

        public class Handbook
        {
            public int HandbookId { get; set; }
            public int VehicleId { get; set; }
            public DateTime? BookDate { get; set; }
            public string Notes { get; set; }
            public string Status { get; set; }
        }

         


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