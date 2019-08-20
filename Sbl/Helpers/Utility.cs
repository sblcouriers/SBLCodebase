using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Sbl.Models;
using Sbl.Models.BO;

namespace Sbl.Helpers
{
    public static class Utility
    {
        public static string WebSiteUrl
        {
            get
            {
                string websiteUrl = ConfigurationManager.AppSettings["WebSiteUrl"];
                if (!string.IsNullOrWhiteSpace(websiteUrl) && websiteUrl.EndsWith(WebConstant.BackSlash))
                    websiteUrl = websiteUrl.TrimEnd(new[] { '/' });
                return websiteUrl;
            }
        }

        public static DateTime ConvertFromUnixTimestamp(double timeStamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddMilliseconds(timeStamp);
        }
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        /// <summary>
        /// Case insensitive version of String.Replace().
        /// </summary>
        /// <param name="s">String that contains patterns to replace</param>
        /// <param name="oldValue">Pattern to find</param>
        /// <param name="newValue">New pattern to replaces old</param>
        /// <param name="comparisonType">String comparison type</param>
        /// <returns></returns>
        public static string ReplaceStr(this string s, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (s == null)
                return null;

            if (String.IsNullOrEmpty(oldValue))
                return s;
            StringBuilder result = new StringBuilder(Math.Min(4096, s.Length));
            int pos = 0;
            while (true)
            {
                int i = s.IndexOf(oldValue, pos, comparisonType);
                if (i < 0)
                    break;

                result.Append(s, pos, i - pos);
                result.Append(newValue);

                pos = i + oldValue.Length;
            }
            result.Append(s, pos, s.Length - pos);

            return result.ToString();
        }

        public static FileStreamResult GetFileStreamResult(byte[] dataFileBytes, string dataFileContentType)
        {
            MemoryStream pdfStream = new MemoryStream();
            pdfStream.Write(dataFileBytes, 0, dataFileBytes.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, dataFileContentType);
        }

        //public static RouteCharges CalculateRouteCharges(RouteAllocation routeAllocation)
        //{
        //    return 0.0M;
        //}




        #region


            /*

        public static List<AssociateRemittanceViewModel.SubRental> RentalCharges(List<RouteAllocation> routeAllocations)
        {
            #region Deduct Related Calculation
            double totalVechicleDeductAmt = 0.0;
            var associateVechileRental = new List<AssociateRemittanceViewModel.SubRental>();
            foreach (var sub in routeAllocations)
            {
                if (sub.RouteDate != null)
                {
                    var weekNo = sub.RouteDate.Value.GetWeekNumberOfYear();
                    var subRentalCharges = 0.00;
                    if (!sub.Associate.OwnVehicle)
                    {
                        var subRentals = sub.Associate.SubRentals.Where(x => x.Active && x.Deleted == false).ToList();
                        foreach (var subRent in subRentals)
                        {
                            if (subRent != null)
                            {
                                var rentedWeek = subRent.DateRented.GetWeekNumberOfYear();
                                var alreadyAdded =
                                    associateVechileRental.FirstOrDefault(
                                        x => x.WeekNumber == weekNo && x.AssociateId == subRent.AssociateId);
                                if (alreadyAdded == null)
                                {
                                    if (subRent.DateReturned != null)
                                    {
                                        var returnWeek = subRent.DateReturned.Value.GetWeekNumberOfYear();
                                        if (returnWeek > weekNo && rentedWeek <= weekNo)
                                        {
                                            subRentalCharges = subRent.RentalPrice;
                                        }
                                    }
                                    else
                                    {
                                        if (weekNo >= rentedWeek)
                                        {
                                            subRentalCharges = subRent.RentalPrice;
                                        }
                                    }
                                    var rental = new AssociateRemittanceViewModel.SubRental();
                                    rental.Amount = subRentalCharges;
                                    rental.WeekNumber = weekNo;
                                    rental.AssociateId = subRent.AssociateId;
                                    associateVechileRental.Add(rental);
                                }
                            }
                        }
                        //If the associate “OwnVehicle” is true then it should deduct £5 from daily route
                    }
                }
            }
            return associateVechileRental;

            #endregion
        }

        */

        public static double TotalRouteCharges(List<RouteAllocation> routeAllocations, Setting settings)
        {
            var sblTotal = 0.0;
            if (routeAllocations != null)
            {
                sblTotal =
                      routeAllocations.Sum(
                          x =>
                              x.RoutePrice1 + (x.Ad1Price * x.Ad1Quantity) + (x.Ad2Price * x.Ad2Quantity) +
                              (x.Ad3Price * x.Ad3Quantity) + ((x.Mileage / settings.MilesPerLitre) * settings.DieselPrice) -
                              (x.Deduct + (x.Associate.OwnVehicle ? WebConstant.OwnVechicleDeductAmount : 0)));
            }
            return sblTotal;
        }




        #endregion




    }//end class
}//end namespace