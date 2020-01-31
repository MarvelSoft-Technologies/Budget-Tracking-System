using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Budget_Tracking_System
{
    public class StandardResponse
    {
        public string status { get; set; }
        public dynamic data { get; set; }

        public static StandardResponse TokenError()
        {
            return new StandardResponse() { status = "error", data = "token error" };
        }

        public static StandardResponse AccessDenied()
        {
            return new StandardResponse() { status = "error", data = "You do not have the privilege to access the requested resource or data" };
        }

        public static StandardResponse Error(dynamic data = null)
        {
            if (data == null)
            {
                data = "server error";
            }
            return new StandardResponse() { status = "error", data = data };
        }

        public static StandardResponse Success(dynamic data = null)
        {
            if (data == null)
            {
                data = "success";
            }
            return new StandardResponse() { status = "success", data = data };
        }
    }
}
