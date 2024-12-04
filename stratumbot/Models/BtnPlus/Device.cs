using Microsoft.Win32;
using System;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace stratumbot.Models
{
    public class Device
    {
        /// <summary>
        /// Property to getting device hash from anywhere
        /// </summary>
        private static string hash;
        public static string Hash
        {
            get
            {
                if(hash == null)
                    hash = GetHash();
                return hash;
            }
        }

        // TODO DELETE
        /// <summary>
        /// OLD METHOD
        /// Device Hash generation
        /// </summary>
        public static string GetHash__OLD()
        {
            string proc = GetProcessorID();
            string drive = GetDriveId();
            string mother = GetMotherId();
            string uuid = GetUUID();
            string macs = GetMACs();

            string secret = "stratum-bot" + proc + drive + mother + uuid + macs;

            return GetMD5(secret);
        }

        /// <summary>
        /// Device Hash generation
        /// </summary>
        public static string GetHash()
        {
            string proc = GetProcessorID();
            string drive = GetDriveId();
            string mother = GetMotherId();
            string uuid = GetUUID();
            string guid = GetGUID(); 

            string secret = "stratum-bot" + proc + drive + mother + uuid + guid;

            return GetMD5(secret);
        }

        private static string GetProcessorID()
        {
            try
            {
                var processorId = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

                foreach (ManagementObject obj in processorId.Get())
                {
                    return @"stratum-bot" + obj["ProcessorId"].ToString();
                }

                return "";

            } catch
            {
                throw new Exception("dex_1");
            }
        }

        private static string GetDriveId()
        {
            try
            {
                string drive = "C";
                var dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
                dsk.Get();
                return dsk["VolumeSerialNumber"].ToString();
            }
            catch
            {
                throw new Exception("dex_2");
            }
        }

        private static string GetMotherId()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM CIM_Card");

                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["SerialNumber"].ToString();
                }

                return "";
            }
            catch
            {
                throw new Exception("dex_3");
            }
        }

        private static string GetUUID()
        {
            try
            {
                var searcherUUid = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");

                foreach (ManagementObject obj in searcherUUid.Get())
                {
                    return obj["UUID"].ToString();
                }

                return "";
            }
            catch
            {
                throw new Exception("dex_4");
            }
        }

        private static string GetMACs()
        {
            try
            {
                /*string macs = "";

                foreach (System.Net.NetworkInformation.NetworkInterface ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Ethernet) continue;
                    macs += ni.GetPhysicalAddress().ToString();
                    var dd = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                }

                return macs;*/

                return (
                            from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                            where nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up
                            select nic.GetPhysicalAddress().ToString()
                        ).FirstOrDefault();
            }
            catch
            {
                throw new Exception("dex_5");
            }
        }

        private static string GetGUID()
        {
            try
            {
                return Registry.CurrentUser.OpenSubKey("SB3").GetValue("g").ToString();
            }
            catch
            {
                throw new Exception("dex_6");
            }
        }

        private static string GetMD5(string value)
        {
            byte[] encodedPassword = new UTF8Encoding().GetBytes(value);
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
    }
}
