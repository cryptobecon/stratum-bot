using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    // DTO for OHLC+ Limit Indicator
    class OHLCPlusItem
    {
        public decimal OpenMax { get; set; } // Max of Open цена в period периоде
        public decimal OpenMin { get; set; } // Min of Open цена в period периоде
        public decimal OpenAvg { get; set; } // openMax + openMin / 2

        public decimal HighMax { get; set; }
        public decimal HighMin { get; set; }
        public decimal HighAvg { get; set; }

        public decimal LowMax { get; set; }
        public decimal LowMin { get; set; }
        public decimal LowAvg { get; set; }

        public decimal CloseMax { get; set; }
        public decimal CloseMin { get; set; }
        public decimal CloseAvg { get; set; }

        public decimal Hl2Max { get; set; } // Median Price 
        public decimal Hl2Min { get; set; }
        public decimal Hl2Avg { get; set; }

        public decimal Hlc3Max { get; set; }
        public decimal Hlc3Min { get; set; }
        public decimal Hlc3Avg { get; set; }

        public decimal Ohlc4Max { get; set; }
        public decimal Ohlc4Min { get; set; }
        public decimal Ohlc4Avg { get; set; }
    }
}
