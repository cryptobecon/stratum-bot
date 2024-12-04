using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class DOMVolDiff : IIndicator
    {
        public string Name { get; set; } = "DOM Volume Diff";

        public Dictionary<string, decimal> Result(Dictionary<string, List<Depth>> DOM, int quantity)
        {
            decimal bidsVolume = 0;
            decimal asksVolume = 0;
            decimal wholeVolume = 0; // 100% volume
            decimal bidsVolumePercent = 0;
            decimal asksVolumePercent = 0;

            // Vol of BUY
            int i = 0;
            foreach (var depth in DOM["bids"])
            {
                bidsVolume += depth.Amount;
                if (i >= quantity)
                    break;
                i++;
            }
            // Vol of SELL
            i = 0;
            foreach (var depth in DOM["asks"])
            {
                asksVolume += depth.Amount;
                if (i >= quantity)
                    break;
                i++;
            }
            // 100%
            wholeVolume = bidsVolume + asksVolume;
            // sell % ?
            asksVolumePercent = Calc.PercentOfAmount(asksVolume, wholeVolume);
            // buy % ?
            bidsVolumePercent = Calc.PercentOfAmount(bidsVolume, wholeVolume);

            Dictionary<string, decimal> result = new Dictionary<string, decimal>();
            result["asksVolumePercent"] = asksVolumePercent;
            result["bidsVolumePercent"] = bidsVolumePercent;

            return result;
        }
    }
}
