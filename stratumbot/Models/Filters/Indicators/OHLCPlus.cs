using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class OHLCPlus : IIndicator
    {
        public string Name { get; set; } = "OHLC+ Limit";

        public OHLCPlusItem Result(IEnumerable<Quote> data, int period)
        {
            decimal openMax = 0; // Max of Open цена в period периоде
            decimal openMin = 999999; // Min of Open цена в period периоде
            decimal openAvg = 0; // openMax + openMin / 2

            decimal highMax = 0;
            decimal highMin = 999999;
            decimal highAvg = 0;

            decimal lowMax = 0; 
            decimal lowMin = 999999;
            decimal lowAvg = 0;

            decimal closeMax = 0;
            decimal closeMin = 999999;
            decimal closeAvg = 0;

            decimal hl2 = 0; // hl2 ~ NGA (Median Price)
            decimal hl2Max = 0; 
            decimal hl2Min = 999999;
            decimal hl2Avg = 0;

            decimal hlc3 = 0; // Pivot Point (Typical Price)
            decimal hlc3Max = 0;
            decimal hlc3Min = 999999;
            decimal hlc3Avg = 0;

            decimal ohlc4 = 0; // Pivot Point
            decimal ohlc4Max = 0; 
            decimal ohlc4Min = 999999;
            decimal ohlc4Avg = 0;

            foreach (var ohlc in data)
            {
                // Open
                if (ohlc.Open < openMin)
                    openMin = ohlc.Open;
                if (ohlc.Open > openMax)
                    openMax = ohlc.Open;
                // High
                if (ohlc.High < highMin)
                    highMin = ohlc.High;
                if (ohlc.High > highMax)
                    highMax = ohlc.High;
                // Min
                if (ohlc.Low < lowMin)
                    lowMin = ohlc.Low;
                if (ohlc.Low > lowMax)
                    lowMax = ohlc.Low;
                // Close
                if (ohlc.Close < closeMin)
                    closeMin = ohlc.Close;
                if (ohlc.Close > closeMax)
                    closeMax = ohlc.Close;
                // hl2
                hl2 = (ohlc.High + ohlc.Low) / 2;
                if (hl2 < hl2Min)
                    hl2Min = hl2;
                if (hl2 > hl2Max)
                    hl2Max = hl2;
                // hlc3 Pivot Point (Typical Price)
                hlc3 = ((ohlc.High + ohlc.Low + ohlc.Close) / 3);
                if (hlc3 < hlc3Min)
                    hlc3Min = hlc3;
                if (hlc3 > hlc3Max)
                    hlc3Max = hlc3;
                // ohlc4 Pivot Point
                ohlc4 = ((ohlc.Open + ohlc.High + ohlc.Low + ohlc.Close) / 4);
                if (ohlc4 < ohlc4Min)
                    ohlc4Min = ohlc4;
                if (ohlc4 > ohlc4Max)
                    ohlc4Max = ohlc4;
            }

            openAvg = (openMax + openMin) / 2;
            highAvg = (highMax + highMin) / 2;
            lowAvg = (lowMax + lowMin) / 2;
            closeAvg = (closeMax + closeMin) / 2;
            hl2Avg = (hl2Max + hl2Min) / 2;
            hlc3Avg = (hlc3Max + hlc3Min) / 2;
            ohlc4Avg = (hlc3Max + hlc3Min) / 2;

            return new OHLCPlusItem()
            {
                OpenMax = openMax,
                OpenMin = openMin,
                OpenAvg = openAvg,
                HighMax = highMax,
                HighMin = highMin,
                HighAvg = highAvg,
                LowMax = lowMax,
                LowMin = lowMin,
                LowAvg = lowAvg,
                CloseMax = closeMax,
                CloseMin = closeMin,
                CloseAvg = closeAvg,
                Hl2Max = hl2Max,
                Hl2Min = hl2Min,
                Hl2Avg = hl2Avg,
                Hlc3Max = hlc3Max,
                Hlc3Min = hlc3Min,
                Hlc3Avg = hlc3Avg,
                Ohlc4Max = ohlc4Max,
                Ohlc4Min = ohlc4Min,
                Ohlc4Avg = ohlc4Avg
            };
        }
    }
}