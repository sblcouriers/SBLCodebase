using Sbl.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;

namespace Sbl.Helpers
{
    public class EmailSender
    {

 
        public List<MsAttachment> MsAttachments { get; set; }
        public class MsAttachment
        {
            public MemoryStream Ms { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }



        public static void SendMessage(List<string> To, List<string> Cc, List<string> Bcc, string Subject, string Body, bool HasAttachmentFile, List<string> AttachmentFile, bool HasAttachmentMs, List<MsAttachment> AttachmentMs)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            string SMTPHost = "mail.overssl.net";
            string SMTPUsername = "smtp@smartappscode.com";
            string SMTPPassword = "letmein";
            int SMTPPort = 25;
            bool SMTPEnableSSL = false;

            string FromEmail = "smtp@smartappscode.com";
            string DisplayName = "SBL Couriers";
            string ReplyToEmail = "smtp@smartappscode.com";
            string BccEmail = "smtp@smartappscode.com";

            //

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(FromEmail, DisplayName);

            foreach (var to in To)
            {
                mail.To.Add(to);
            }

            foreach (var cc in Cc)
            {
                mail.CC.Add(cc);
            }

            foreach (var bcc in Bcc)
            {
                mail.Bcc.Add(bcc);
            }

            if (!String.IsNullOrEmpty(BccEmail))
            {
                mail.Bcc.Add(BccEmail);
            }

            mail.ReplyToList.Add(new MailAddress(ReplyToEmail, DisplayName));
            mail.Subject = Subject;
            mail.Body = Body;
            mail.IsBodyHtml = true;

            // attachments file
            if (HasAttachmentFile == true)
            {
                foreach (var attachmentfile in AttachmentFile)
                {
                    if (File.Exists(attachmentfile))
                    {
                        Attachment file = new Attachment(attachmentfile);
                        mail.Attachments.Add(file);
                    }
                }

                //file.Dispose();
            }

            // attachments ms
            if (HasAttachmentMs == true)
            {
                foreach (var attachmentms in AttachmentMs)
                {
                    mail.Attachments.Add(new Attachment(attachmentms.Ms, attachmentms.FileName, attachmentms.ContentType));
                }

                //file.Dispose();
            }

            SmtpClient smtpServer = new SmtpClient(SMTPHost);
            smtpServer.Port = SMTPPort;
            smtpServer.EnableSsl = SMTPEnableSSL;
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
            smtpServer.Send(mail);
            smtpServer.Dispose();
        }

 

        public static bool SendEmailReceiptToAssociate(string AssociateEmail, string EmailSubject, string Emailbody)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var emailsent = false;
            bool hasattachmentdb = false;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<MsAttachment> attachmentms = new List<MsAttachment>();

            // subject
            string subject = EmailSubject;

            // body
            StringBuilder sb = new StringBuilder();
            sb.Append(Emailbody);
            //sb.Append("<br /><br />");

            string body = sb.ToString();

            // add email 
            to.Add(AssociateEmail);

            // check there are emails
            if (to.Any() || cc.Any() || bcc.Any())
            {
                emailsent = true;
                SendMessage(to, cc, bcc, subject, body, false, null, hasattachmentdb, attachmentms);
            }

            return emailsent;
        }




        public static bool SendEmailRemittanceToAssociate(int AssociateRemittanceId, string AssociateEmail, string EmailSubject, string Emailbody)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var emailsent = false;
            bool hasattachmentdb = false;

            List<string> to = new List<string>();
            List<string> cc = new List<string>();
            List<string> bcc = new List<string>();
            List<MsAttachment> attachmentms = new List<MsAttachment>();

            // subject
            string subject = EmailSubject;

            // body
            StringBuilder sb = new StringBuilder();
            sb.Append(Emailbody);
            //sb.Append("<br /><br />");

            string body = sb.ToString();

            // add email 
            to.Add(AssociateEmail);


            // add remittance
            var remittance = db.AssociateRemittances.Where(x => x.Id == AssociateRemittanceId).FirstOrDefault();

            if (!String.IsNullOrEmpty(remittance.DataFileName))
            {
                byte[] att = remittance.DataFile;
                MemoryStream ms = new MemoryStream(att);

                attachmentms.Add(new MsAttachment
                {
                    Ms = ms,
                    FileName = remittance.DataFileName,
                    ContentType = remittance.DataFileContentType
                });

                hasattachmentdb = true;
            }

            // check there are emails
            if (to.Any() || cc.Any() || bcc.Any())
            {
                emailsent = true;
                SendMessage(to, cc, bcc, subject, body, false, null, hasattachmentdb, attachmentms);
            }

            return emailsent;
        }



        public static bool SendMail2(string toEmail, string emailSubject, string emailbody, bool isBodyHtml=true, MailPriority mailPriority=MailPriority.High, string toDisplayName=null)
        {
            return SendMail2(toEmail, string.Empty, emailSubject, emailbody, isBodyHtml, mailPriority, toDisplayName);
        }

         

        public static bool SendMail2(string toEmail,string fromEmail, string emailSubject, string emailbody, bool isBodyHtml = true, MailPriority mailPriority = MailPriority.High, string toDisplayName=null,string fromName="SBL")
        {
            bool isEmailSent = false;
            try
            {
                if(string.IsNullOrWhiteSpace(fromName))
                {
                    fromName = "SBL";
                }
                bool isEmailEnable = true; 
                if (isEmailEnable)
                {
                    SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                    string host = section.Network.Host;
                    int port = section.Network.Port;
                    bool enableSsl = section.Network.EnableSsl;
                    string smtpUser = section.Network.UserName;

                    string smtpPassword = section.Network.Password;
                    if (string.IsNullOrWhiteSpace(fromEmail))
                        fromEmail = section.From;

                    SmtpClient smtpClient = new SmtpClient();

                    MailAddress fromMail = new MailAddress(fromEmail, fromName);
                    MailAddress toMail = new MailAddress(toEmail);

                    if (!string.IsNullOrWhiteSpace(toDisplayName))
                    {
                        toMail = new MailAddress(toEmail, toDisplayName.Trim());
                    }
                    MailMessage  mailMessage = new MailMessage(fromMail, toMail);

                    mailMessage.Headers.Add("X-SEGMENT", fromName);
                   

                    mailMessage.Subject = emailSubject;
                    mailMessage.IsBodyHtml = isBodyHtml;
                    mailMessage.Body = emailbody;
                    mailMessage.Priority = mailPriority;
                    smtpClient.Host = host;
                    smtpClient.Port = port;
                    smtpClient.EnableSsl = enableSsl;
                    if (!string.IsNullOrWhiteSpace(smtpUser) && !string.IsNullOrWhiteSpace(smtpPassword))
                    {
                        if (smtpUser.Length > 0 && smtpPassword.Length > 0)
                        {
                            smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                        }
                    }
                  smtpClient.Send(mailMessage);
                   
                    isEmailSent= true;
                }
            }
            catch (Exception ex)
            {
                isEmailSent= false;
            }
            return isEmailSent;
        }



    }
    
    
}