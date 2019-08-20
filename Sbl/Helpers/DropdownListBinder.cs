using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sbl.Models.BO;


namespace Sbl.Helpers
{
    public static class DropdownListBinder
    {

        public static List<SelectListItem> PopulatWeeks()
        {
            List<SelectListItem> weekNoListItems = new List<SelectListItem>();
            int totalNoOfWeek = 52;
            for (int weekNo = 1; weekNo <= totalNoOfWeek; weekNo++)
            {
                weekNoListItems.Add(new SelectListItem { Text = string.Format("Week {0}", weekNo), Value = weekNo.ToString() });
            }
            return weekNoListItems;
        }
        public static List<SelectListItem> PopulatAccidentStatus()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            status.Add(new SelectListItem { Text = "Resolved", Value = "Resolved" });
            return status;
        }
        public static List<SelectListItem> PopulatAssociateStatus()
        {
            List<SelectListItem> associatestatus = new List<SelectListItem>();
            //associatestatus.Add(new SelectListItem { Text = "Inactive", Value = "Inactive" });
            associatestatus.Add(new SelectListItem { Text = "Active", Value = "Active" });
            associatestatus.Add(new SelectListItem { Text = "Deactivated", Value = "Deactivated" });
            return associatestatus;
        }

        public static List<SelectListItem> PopulatApplicationStatus()
        {
            List<SelectListItem> applicationstatus = new List<SelectListItem>();
            applicationstatus.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            applicationstatus.Add(new SelectListItem { Text = "Approved", Value = "Approved" });
            applicationstatus.Add(new SelectListItem { Text = "Rejected", Value = "Rejected" });
            return applicationstatus;
        }
        public static List<SelectListItem> PopulatInductionStatus()
        {
            List<SelectListItem> inductionStatus = new List<SelectListItem>();
            inductionStatus.Add(new SelectListItem { Text = "Pending", Value = "Pending" });
            inductionStatus.Add(new SelectListItem { Text = "Approved", Value = "Approved" });
            inductionStatus.Add(new SelectListItem { Text = "Rejected", Value = "Rejected" });
            return inductionStatus;
        }

        public static List<SelectListItem> PopulatSubRentalStatus()
        {
            List<SelectListItem> inductionStatus = new List<SelectListItem>();
            inductionStatus.Add(new SelectListItem { Text = "Rented", Value = "Rented" });
            inductionStatus.Add(new SelectListItem { Text = "Available", Value = "Available" });
            inductionStatus.Add(new SelectListItem { Text = "Repair", Value = "Repair" });
            inductionStatus.Add(new SelectListItem { Text = "Returned", Value = "Returned" });
            return inductionStatus;
        }
        public static List<SelectListItem> PopulatAssociateFileDescription()
        {
            List<SelectListItem> description = new List<SelectListItem>();
            description.Add(new SelectListItem { Text = "CRB Result", Value = "CRB Result" });
            description.Add(new SelectListItem { Text = "D&A Result", Value = "D&A Result" });
            description.Add(new SelectListItem { Text = "Driving Licence", Value = "Driving Licence" });
            description.Add(new SelectListItem { Text = "DVLA Check", Value = "DVLA Check" });
            description.Add(new SelectListItem { Text = "Passport/ID", Value = "Passport/ID" });
            description.Add(new SelectListItem { Text = "NiNo", Value = "NiNo" });
            description.Add(new SelectListItem { Text = "POA - 1", Value = "POA - 1" });
            description.Add(new SelectListItem { Text = "POA - 2", Value = "POA - 2" });
            description.Add(new SelectListItem { Text = "SBL Application", Value = "SBL Application" });
            description.Add(new SelectListItem { Text = "Amazon Forms", Value = "Amazon Forms" });
            description.Add(new SelectListItem { Text = "CRB Authorisation", Value = "CRB Authorisation" });
            description.Add(new SelectListItem { Text = "Induction Declaration", Value = "Induction Declaration" });
            description.Add(new SelectListItem { Text = "Casualty Loss", Value = "Casualty Loss" });
            description.Add(new SelectListItem { Text = "AVD", Value = "AVD" });
            return description;
        }
        public static List<SelectListItem> PopulatInspectionFileDescription()
        {
            var desscriptionList = FindInspectionFileDescriptionList();
            return desscriptionList.Select(tyreType => new SelectListItem { Text = tyreType.Name, Value = tyreType.Name }).ToList();
        }
        public static List<SelectListItem> PopulatTyreType()
        {
            var tyreTypeList = FindTyreTypeList();
            return tyreTypeList.Select(tyreType => new SelectListItem { Text = tyreType.Name, Value = tyreType.Name }).ToList();
        }

        public static List<SelectListItem> PopulatRouteType()
        {
            List<SelectListItem> routeType = new List<SelectListItem>();
            routeType.Add(new SelectListItem { Text = "Select Type", Value = "0" });
            routeType.Add(new SelectListItem { Text = "Full", Value = "Full" });
            routeType.Add(new SelectListItem { Text = "Half", Value = "Half" });
            routeType.Add(new SelectListItem { Text = "Remote Debrief", Value = "RemoteDebrief" });
            routeType.Add(new SelectListItem { Text = "Nursery Routes Level 1", Value = "NurseryRoutesLevel1" });
            routeType.Add(new SelectListItem { Text = "Nursery Routes Level 2", Value = "NurseryRoutesLevel2" });
            routeType.Add(new SelectListItem { Text = "Rescue 2 Hours", Value = "Rescue2Hours" });
            routeType.Add(new SelectListItem { Text = "Rescue 4 Hours", Value = "Rescue4Hours" });
            routeType.Add(new SelectListItem { Text = "Rescue 6 Hours", Value = "Rescue6Hours" });
            routeType.Add(new SelectListItem { Text = "Re-delivery 2 Hours", Value = "ReDelivery2Hours" });
            routeType.Add(new SelectListItem { Text = "Re-delivery 4 Hours", Value = "ReDelivery4Hours" });
            routeType.Add(new SelectListItem { Text = "Re-delivery 6 Hours", Value = "ReDelivery6Hours" });
            routeType.Add(new SelectListItem { Text = "Missort 2 Hours", Value = "Missort2Hours" });
            routeType.Add(new SelectListItem { Text = "Missort 4 Hours", Value = "Missort4Hours" });
            routeType.Add(new SelectListItem { Text = "Missort 6 Hours", Value = "Missort6Hours" });
            routeType.Add(new SelectListItem { Text = "Same Day", Value = "SameDay" });
            routeType.Add(new SelectListItem { Text = "Training Day", Value = "TrainingDay" });
            routeType.Add(new SelectListItem { Text = "Ride Along", Value = "RideAlong" });
            routeType.Add(new SelectListItem { Text = "Support AD1", Value = "SupportAd1" });
            routeType.Add(new SelectListItem { Text = "Support AD2", Value = "SupportAd2" });
            routeType.Add(new SelectListItem { Text = "Support AD3", Value = "SupportAd3" });
            routeType.Add(new SelectListItem { Text = "Lead Driver", Value = "LeadDriver" });
            routeType.Add(new SelectListItem { Text = "Large Van", Value = "LargeVan" });
            return routeType;
        }

        public static List<SelectListResponse> FindTyreTypeList()
        {
            var tyreTypeList = new List<SelectListResponse>();
            tyreTypeList.Add(new SelectListResponse { Name = "Bridgestone", Id = "Bridgestone" });
            tyreTypeList.Add(new SelectListResponse { Name = "Continental", Id = "Continental" });
            tyreTypeList.Add(new SelectListResponse { Name = "Dunlop", Id = "Dunlop" });
            tyreTypeList.Add(new SelectListResponse { Name = "Form Kite", Id = "Form Kite" });
            tyreTypeList.Add(new SelectListResponse { Name = "Goodyear", Id = "Goodyear" });
            tyreTypeList.Add(new SelectListResponse { Name = "Pirelli", Id = "Pirelli" });
            tyreTypeList.Add(new SelectListResponse { Name = "Space Saver", Id = "Space Saver" });
            tyreTypeList.Add(new SelectListResponse { Name = "Other", Id = "Other" });
            return tyreTypeList;
        }

        public static List<SelectListResponse> FindInspectionFileDescriptionList()
        {
            var fileDescription = new List<SelectListResponse>();
            fileDescription.Add(new SelectListResponse { Name = "CRB Result", Id = "CRB Result" });
            fileDescription.Add(new SelectListResponse { Name = "D&A Result", Id = "D&A Result" });
            fileDescription.Add(new SelectListResponse { Name = "Driving Licence", Id = "Driving Licence" });
            fileDescription.Add(new SelectListResponse { Name = "DVLA Check", Id = "DVLA Check" });
            fileDescription.Add(new SelectListResponse { Name = "Passport/ID", Id = "Passport/ID" });
            fileDescription.Add(new SelectListResponse { Name = "NiNo", Id = "NiNo" });
            fileDescription.Add(new SelectListResponse { Name = "POA - 1", Id = "POA - 1" });
            fileDescription.Add(new SelectListResponse { Name = "POA - 2", Id = "POA - 2" });
            fileDescription.Add(new SelectListResponse { Name = "SBL Application", Id = "SBL Application" });
            fileDescription.Add(new SelectListResponse { Name = "Amazon Forms", Id = "Amazon Forms" });
            fileDescription.Add(new SelectListResponse { Name = "CRB Authorisation", Id = "CRB Authorisation" });
            fileDescription.Add(new SelectListResponse { Name = "Induction Declaration", Id = "Induction Declaration" });
            fileDescription.Add(new SelectListResponse { Name = "Casualty Loss", Id = "Casualty Loss" });
            fileDescription.Add(new SelectListResponse { Name = "AVD", Id = "AVD" });
            return fileDescription;
        }
    }//end class
}//end namespace