using Microsoft.AspNet.Identity;
using Sbl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sbl.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                var userid = User.Identity.GetUserId();
                var user = db.Users.FirstOrDefault(x => x.Id == userid);

                // user details
                Session["UserName"] = user.UserName;
                Session["UserEmail"] = user.Email;
                
                // dates
                var datenow = DateTime.Now.Date;
                var dateservicealert = DateTime.Now.Date.AddDays(7);

                // vehicle services
                if (db.VehicleServices.Any(x => x.Active == true && x.Deleted == false &&  dateservicealert >= x.ServiceDate))
                {
                    List<VehicleService> vehicleservices = db.VehicleServices.Where(x => x.Active == true && x.Deleted == false && dateservicealert >= x.ServiceDate).OrderBy(x => x.ServiceDate).ToList();
                    Session["VehicleServices"] = vehicleservices;
                }
                else
                {
                    Session["VehicleServices"] = null;
                }

                // associate files
                if (db.AssociateFiles.Any(x => x.Active == true && x.Deleted == false && dateservicealert >= x.DataFileExpiryDate))
                {
                    List<AssociateFile> associatefiles = db.AssociateFiles.Where(x => x.Active == true && x.Deleted == false && dateservicealert >= x.DataFileExpiryDate).OrderBy(x => x.DataFileExpiryDate).ToList();
                    Session["AssociateFiles"] = associatefiles;
                }
                else
                {
                    Session["AssociateFiles"] = null;
                }
            }
        }
    }//end class
}//end namespace