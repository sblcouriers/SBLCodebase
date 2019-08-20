using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models.BO
{
    public class RouteCharges
    {
        public int RouteId { get; set; }
        public int AssociateId { get; set; }
        public int WeekNumber { get; set; }
        public decimal RouteAmount { get; set; }
      //  public int RouteCharges { get; set; }
    }//end class
}//end namespace