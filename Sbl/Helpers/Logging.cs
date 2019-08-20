using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sbl.Models;

namespace Sbl.Helpers
{
    public class Logging
    {
        public static void LogEntry(string Method, string Message, bool IsError)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            Log log = new Log();
            log.Method = Method;
            log.Message = Message;
            log.IsError = IsError;
            log.DateCreated = DateTime.Now;
            db.Logs.Add(log);
            db.SaveChanges();
        }
    }
}