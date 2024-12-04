using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    /// <summary>
    /// Объект списка API ключей для биржи
    /// </summary>
    public class APITokens
    {
        [JsonProperty("Exchange")]
        public Exchange Exchange { get; set; }
        [JsonProperty("Tokens")]
        public List<Tokens> Tokens { get; set; }
    }

    public class Tokens
    {
        [JsonProperty("APIKey")]
        public string APIKey { get; set; }
        [JsonProperty("APISecret")]
        public string APISecret { get; set; }
    }
}
