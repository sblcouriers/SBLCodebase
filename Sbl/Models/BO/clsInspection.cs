using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sbl.Models.BO
{
    public class ClsInspection
    {
        public int Id { get; set; }
        public int AssociateId { get; set; }
        public string AssociateName { get; set; }
        public string AssociateEmail { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? InspectionDueDate { get; set; }
    }//end class
    public class InspectionModel
    {
        public int PageNo { get; set; }
        public int PageLength { get; set; }
        public int TotalRecords { get; set; }
        public List<ClsInspection> InspectionList { get; set; }
    }//end class
}//end namespace