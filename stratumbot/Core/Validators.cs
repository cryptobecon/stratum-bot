using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stratumbot.Models.Logs;
using System;

namespace stratumbot.Core
{
    /// <summary>
    /// Valid data checker
    /// </summary>
    public static class Validators
    {

        /// <summary>
        /// Is this JSON string valid
        /// </summary>
        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Logger.ToFile(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Logger.ToFile(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
