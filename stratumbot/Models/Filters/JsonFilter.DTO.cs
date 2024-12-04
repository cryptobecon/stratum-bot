using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace stratumbot.Models.Filters
{
    /// <summary>
    /// Filter JSON object for configs to save and read from file.
    /// </summary>
    public class JsonFilter
    {
        /// <summary>
        /// Filter identificator on website database
        /// </summary>
        [JsonProperty("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Filter weight
        /// </summary>
        [JsonProperty("Weight")]
        public int Weight { get; set; }

        /// <summary>
        /// Filter display name
        /// </summary>
        [JsonProperty("MyName")]
        public string MyName { get; set; }

        /// <summary>
        /// Filter group (A/B/C)
        /// </summary>
        [JsonProperty("Group")]
        public string Group { get; set; } // Группа

        /// <summary>
        /// Filter background color
        /// </summary>
        [JsonProperty("Color")]
        public Brush Color { get; set; } // Цвет
    }
}
