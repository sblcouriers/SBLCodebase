using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace Sbl.Helpers
{
    public static class EmailTemplate
    {

        public const string MailTemplate = "mailTemplate:";
        public const string TemplateReplaceFormat = "[[{0}]]";
        private static string EmailTemplatePath
        {
            get
            {
                return System.Web.Hosting.HostingEnvironment.MapPath("~/assets/EmailTemplate/");
            }

        }

        // read the text in template file and return it as a string
        private static string ReadFileFrom(string templateName)
        {
            string filePath = Path.Combine(EmailTemplatePath, templateName);
            string body = File.ReadAllText(filePath);
            return body;
        }
        public static string ReadFileFromTemplatePath(string templatePathWithName)
        {
            string body = File.ReadAllText(templatePathWithName);
            return body;
        }
        // get the template body, cache it and return the text
        private static string GetMailBodyOfTemplate(string templateName)
        {
            string cacheKey = string.Concat(MailTemplate, templateName);
            string body=string.Empty;
            if (HttpContext.Current != null)
            {
                body = (string)HttpContext.Current.Cache[cacheKey];
            }
            if (string.IsNullOrWhiteSpace(body))
            {
                //read template file text
                body = ReadFileFrom(templateName);
                if (!string.IsNullOrEmpty(body))
                {
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Cache.Insert(cacheKey, body, null, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
                    }
                }
            }
            return body;
        }
        #region Replace the tokens from html template body with corresponding values
        /// <summary>
        ///  Replace the tokens from html template body with corresponding values
        /// </summary>
        /// <param name="templateName">Name of the Html Template </param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static string PrepareMailBodyWith(string templateName, params string[] pairs)
        {
            string body = GetMailBodyOfTemplate(templateName);
            return PrepareEMailBody(body, pairs);
        }
        #endregion

        #region Replace Tocken From the Email content
        /// <summary>
        /// Replace Tocken From the Email content
        /// </summary>
        /// <param name="emailBody">email body content</param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static string PrepareEMailBody(string emailBody, params string[] pairs)
        {
            if (!string.IsNullOrWhiteSpace(emailBody))
            {
                for (var i = 0; i < pairs.Length; i += 2)
                {
                    emailBody = emailBody.ReplaceStr(TemplateReplaceFormat.FormatWith(pairs[i]), pairs[i + 1], StringComparison.InvariantCultureIgnoreCase);
                }
                return emailBody;
            }
            return string.Empty;
        }
        public static string PrepareEMailBody(string emailBody, Dictionary<string, string> dicEmailTags)
        {
            if (!string.IsNullOrWhiteSpace(emailBody))
            {
                return dicEmailTags.Aggregate(emailBody, (current, dictEmailTag) => current.ReplaceStr(dictEmailTag.Key, dictEmailTag.Value, StringComparison.InvariantCultureIgnoreCase));
            }
            return string.Empty;
        }
        #endregion
        public static string FormatWith(this string target, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, target, args);
        }
    }//end class
}//end namespace