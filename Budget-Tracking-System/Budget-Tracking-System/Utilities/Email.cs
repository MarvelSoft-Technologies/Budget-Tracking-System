using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using RestSharp;
using System.Text;

namespace Budget_Tracking_System
{
    public class Email
    {
        public static void CacheAllTemplates()
        {
            try
            {
                var EmailTemplatesFolder = Globals.RootPath + "/EmailTemplates/";
                var emailTemplates = Directory.GetFiles(EmailTemplatesFolder);
                foreach (var template in emailTemplates)
                {
                    string HTMLTemplate = File.ReadAllText(template);
                    Cache.Add(Path.GetFileName(template), HTMLTemplate, DateTime.Now.AddSeconds(Globals.EmailTemplateCacheDuration));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static string ComposeFromTemplate(string TemplateName, List<KeyValuePair<string, string>> CustomValues)
        {
            try
            {
                string HTMLTemplate = (string)Cache.Get(TemplateName);
                if (HTMLTemplate == null)
                {
                    string TemplatePath = Globals.RootPath + "/EmailTemplates/" + TemplateName;
                    if (!File.Exists(TemplatePath))
                    {
                        Logger.Error("Email template not found at: " + TemplatePath);
                        throw new Exception("Email template not found at: " + TemplatePath);
                    }
                    HTMLTemplate = File.ReadAllText(TemplatePath);
                    //cache has expired, we just got the template from disk. we need to cache it again
                    Cache.Add(Path.GetFileName(TemplatePath), HTMLTemplate, DateTime.Now.AddSeconds(Globals.EmailTemplateCacheDuration));
                }
                StringBuilder stringBuilder = new StringBuilder(HTMLTemplate);
                foreach(KeyValuePair<string, string> CustomValue in CustomValues)
                {
                    stringBuilder.Replace("[[" + CustomValue.Key + "]]", CustomValue.Value);
                }

                //
                stringBuilder.Replace("[[Year]]", DateTime.Now.Year.ToString());
                stringBuilder.Replace("[[LogoURL]]", Globals.LogoURL);
                stringBuilder.Replace("[[Instagram]]", "https://www.linkedin.com/company/interswitch-limited");
                stringBuilder.Replace("[[Twitter]]", "https://twitter.com/InterswitchGRP");
                stringBuilder.Replace("[[Facebook]]", "https://www.facebook.com/InterswitchGroup");
                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static bool SendMessage(string EmailSubject, string EmailBody, string ToEmail)
        {
            string response = "";
            try
            {
                string MaildroidHandle = "skillbase";
                string MaildroidSecret = "bts.skillbase.123";
                string Hash = Encryptions.SHA512(MaildroidHandle + MaildroidSecret + ToEmail); //SHA512
                var client = new RestClient("https://mailrunnerapi.asset.bz/api/");
                var request = new RestRequest("Email/Queue", Method.POST);
                request.RequestFormat = DataFormat.Json;
                var body = new
                {
                    Handle = MaildroidHandle,
                    LogoURL = "",
                    EmailSubject = EmailSubject,
                    ContentHeading = "",
                    Body = EmailBody,
                    ToEmail = ToEmail,
                    ButtonText = "",
                    ButtonURL = ""
                };

                request.AddHeader("Hash", Hash);
                request.AddJsonBody(body);

                response = client.Execute(request).Content;
                if (response.Replace('"', ' ').Trim() == "0")
                {
                    return true;
                }
                else
                {
                    Logger.Error("Email service returned: " +response);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public static string Mask(string EmailAddress)
        {
            try
            {
                if (Utilities.IsValidEmail(EmailAddress))
                {
                    string[] EmailParts = EmailAddress.ToLower().Split('@');
                    decimal Part1Lenght = decimal.Parse(EmailParts[0].Length.ToString());
                    decimal LengthToExtract = Math.Round(Part1Lenght / 2);
                    EmailParts[0] = EmailParts[0].Substring(0, int.Parse(LengthToExtract.ToString()));
                    return EmailParts[0].PadRight(int.Parse(Part1Lenght.ToString()), '*') + "@" + EmailParts[1];
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return "";
        }

    }
}