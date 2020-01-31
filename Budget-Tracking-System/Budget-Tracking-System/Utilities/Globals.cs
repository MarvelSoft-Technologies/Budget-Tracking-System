using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NPoco;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;


namespace Budget_Tracking_System
{
    public class Globals
    {
        public static IConfiguration configuration;
        public static IWebHostEnvironment environment;
        //public static Serilog.ILogger AppLogger { get; }

        
    /// <summary>
    /// BaseURL of this application
    /// </summary>
    public static string BaseURL;
        /// <summary>
        /// baseurl of the frontend implementation
        /// </summary>
        public static string FrontendURL;
        /// <summary>
        /// application physical path on disk
        /// </summary>
        public static string RootPath;

        public static string MoodleEndpoint;
        public static string Token;
        public static string InterSwitchPaymentBaseURL ;
        public static string PaycodeBaseUrl;
        public static string ElasticSearchURL = "";
        public static string ElasticSearchIndex;
        public static string SkillbaseConnectionString;
        public static string MoodleConnectionString;

        //NEW ADDITIONS
        public static string LogoURL;
        public static String WebPay_MerchantCode;
        public static String WebPay_payItemId;
        public static String WebPay_currency;
        public const String WebPay_display_mode = "PAGE";
        public static double Webpay_surcharge;
        public static String Webpay_ClientID;
        public static String WebPay_ClientSecret;

        public static String PAYCODE_ClientID;
        public static String PAYCODE_SecretKey;
        public static double CourseCacheDuration;
        public static double EmailTemplateCacheDuration;
        public static string PayCodeTokenLifetime;

        public static bool EnableDBLogging;
        public static bool EnableFileLogging;
        public static bool EnableElasticSearchLogging;

        public static void Init()
        {
            RootPath = environment.ContentRootPath;
            BaseURL = Environment.GetEnvironmentVariable("BaseURL");
            FrontendURL = Environment.GetEnvironmentVariable("FrontEndURL");
            MoodleEndpoint = Environment.GetEnvironmentVariable("MoodleEndpoint");
            Token = Environment.GetEnvironmentVariable("MoodleToken");
            ElasticSearchURL = Environment.GetEnvironmentVariable("ElasticSearchURL");
            ElasticSearchIndex = Environment.GetEnvironmentVariable("ELASTIC_SEARCH_INDEX");
            InterSwitchPaymentBaseURL = Environment.GetEnvironmentVariable("WebpayBaseURL");
            PaycodeBaseUrl = Environment.GetEnvironmentVariable("PaycodeBaseURL");
            SkillbaseConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Skillbase");

            //new additions
            LogoURL = Environment.GetEnvironmentVariable("LogoURL");
            WebPay_MerchantCode = Environment.GetEnvironmentVariable("WEBPAY_MERCHANT_CODE");
            WebPay_payItemId = Environment.GetEnvironmentVariable("WEBPAY_PAYITEM_ID");
            WebPay_currency = Environment.GetEnvironmentVariable("WEBPAY_CURRENCY_CODE");
            Webpay_surcharge = Convert.ToDouble(Environment.GetEnvironmentVariable("WEBPAY_SURCHARGE"));
            Webpay_ClientID = Environment.GetEnvironmentVariable("WEBPAY_CLIENTID");
            WebPay_ClientSecret = Environment.GetEnvironmentVariable("WEBPAY_SECRET");

            PAYCODE_ClientID = Environment.GetEnvironmentVariable("PAYCODE_CLIENTID");
            PAYCODE_SecretKey = Environment.GetEnvironmentVariable("PAYCODE_SECRET");

            //cache
            CourseCacheDuration = Convert.ToDouble(Environment.GetEnvironmentVariable("Course_Cache_Duration"));
            EmailTemplateCacheDuration = Convert.ToDouble(Environment.GetEnvironmentVariable("Email_Template_Cache_Duration"));
            PayCodeTokenLifetime = Environment.GetEnvironmentVariable("PayCode_Token_Lifetime");

            //logging
            EnableDBLogging = Convert.ToBoolean(Environment.GetEnvironmentVariable("LOGGING_ENABLE_DB"));
            EnableFileLogging = Convert.ToBoolean(Environment.GetEnvironmentVariable("LOGGING_ENABLE_FILE"));
            EnableElasticSearchLogging = Convert.ToBoolean(Environment.GetEnvironmentVariable("LOGGING_ENABLE_ELASTIC_SEARCH"));
            Logger.ConfigureNLog();


            //pull all email templates into cache
           
            
        }

        
        
        
           
    }


}
