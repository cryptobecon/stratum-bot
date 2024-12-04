using Newtonsoft.Json.Linq;
using stratumbot.Models;
using stratumbot.Models.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    class Client
    {
        /// <summary>
        /// Сделать request и получить html ответ
        /// </summary>
        public static string Request(string _url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            var web = new WebClient();
            web.Headers.Add("ForF: stratum-bot");
            while(true)
            {
                try
                {
                    return web.DownloadString(_url);
                } catch (Exception ex)
                {
                    Logger.ToFile($"[exception] {ex.ToString()}");
                    Logger.ToFile("code 24"); // Неудалось соединиться с сервером бота
                    Thread.Sleep(Settings.FailedQueryTimeout);
                }
            }
        }

        /// <summary>
        /// Получить JSON объект по URL.
        /// </summary>
        public static JObject GetJSON(string _url)
        {
            string html = Request(_url);
            if (!html.IsValidJson())
            {
                // TODO FUTURE ERROR вылетает прога. ловить это искл
                Logger.ToFile($"[response] {_url} [request] {html}");
                throw new Exception("code 22"); // Invalid json                 
            }
            return JObject.Parse(html);
        }


    }
}
