using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sbl.Models.BO
{
    public class ResponseOutput
    {
        public int Id { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }//end class

    public class VechicleResponse
    {
        public int VehicleId { get; set; }
        public int AssociateId { get; set; }
        public string VehicleName { get; set; }
    }//end class

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string AuthToken { get; set; }
        public bool IsLogin { get; set; }
    } //end class

    public class SelectListResponse
    {
        public string Name { get; set; }
        public string Id { get; set; }
    } //end class
}//end namespace