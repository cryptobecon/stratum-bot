using Newtonsoft.Json.Linq;
using stratumbot.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    class License
    {
        public static string PaidTill { get; set; } // Оплачно до
        public static string LicenseHash { get; set; } // Лицензия

        public static bool Check()
        {
            return true;
        }

        private static string ReadLicenseFile()
        {
            if (File.Exists("license"))
                return File.ReadAllText("license");
            return "";
        }
    }
}
