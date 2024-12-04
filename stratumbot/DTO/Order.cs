using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    public enum OrderStatus
    {
        New = 0,
        PartiallyFilled = 1,
        Filled = 2,
        Canceled = 3
    }

    public enum OrderSide
    {
        BUY = 1,
        SELL = 2
    }

    public class Order
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        [JsonProperty("Cur1")]
        public string Cur1 { get; set; }
        [JsonProperty("Cur2")]
        public string Cur2 { get; set; }
        [JsonProperty("Time")]
        public string Time { get; set; }
        [JsonProperty("Price")]
        public decimal Price { get; set; }
        [JsonProperty("Amount")]
        public decimal Amount { get; set; } // Объем
        [JsonProperty("Filled")]
        public decimal Filled { get; set; } // Выполнено
        [JsonProperty("Remainder")]
        public decimal Remainder { get; set; } // Осталось выполнить
        [JsonProperty("Side")]
        public OrderSide Side { get; set; } // BUY SELL
        [JsonProperty("Status")]
        public OrderStatus Status { get; set; }
        // TODO take in account user profile
        /// <summary>
        /// Fee rate depends on order type (and user profile but not yet)
        /// </summary>
        [JsonProperty("FeeRate")]
        public decimal FeeRate { get; set; }
    }
}
