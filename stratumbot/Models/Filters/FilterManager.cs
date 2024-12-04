using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Filters
{
    class FilterManager
    {
        // Filter filter - объект фильтра без настроек, только общие.
        public static IFilter GetFilterObjectByBtnPlusId(JsonFilter filter, string filterSide)
        {
            // TODO This will not work as API is not available, so we need to store the filter settings locally

            var json = Client.GetJSON($"https://api.org/v2/filter?id={filter.Id}&hash={Device.Hash}");

            // TODO фильтр не неайден

            // H/L SMA
            if ((string)json["ID"] == "1")
            {
                var newFilter = new HLSMA((int)json["Mode"], (int)json["Period"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // H/L EMA
            if ((string)json["ID"] == "2")
            {
                var newFilter = new HLEMA((int)json["Mode"], (int)json["Period"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Price Change
            if ((string)json["ID"] == "3")
            {
                var newFilter = new PriceChange((string)json["Cur1"], (string)json["Cur2"], (int)json["Mode"], (int)json["Side"], (decimal)json["PriceValue"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }
            // Price Limit
            if ((string)json["ID"] == "4")
            {
                var newFilter = new PriceLimit((int)json["Mode"], (decimal)json["PriceLimitValue"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }
            // DOM Volume Value
            if ((string)json["ID"] == "5")
            {
                var newFilter = new DOMVolumeDiff((int)json["Mode"], (int)json["Side"], (int)json["VolumeValue"], (int)json["Period"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // H/L SMMA
            if ((string)json["ID"] == "6")
            {
                var newFilter = new HLSMMA((int)json["Mode"], (int)json["Period"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // NGA
            if ((string)json["ID"] == "7")
            {
                var newFilter = new NGA((int)json["Mode"], (int)json["Period"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }
            // OHLC+ Limit
            if ((string)json["ID"] == "8")
            {
                var newFilter = new OHLCPlusLimit((int)json["Mode"], (int)json["Source"], (int)json["PriceType"], (int)json["Period"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }
            // Cross
            if ((string)json["ID"] == "9")
            {
                var newFilter = new Cross((int)json["Mode"], (int)json["Line1"], (int)json["Period"], (int)json["Line2"], (int)json["Period2"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }
            // Bollinger Bands
            if ((string)json["ID"] == "10")
            {
                var newFilter = new BollingerBands((int)json["Mode"], (int)json["Period"], (decimal)json["Rate"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"], (string)json["DepthSide"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // RSI
            if ((string)json["ID"] == "11")
            {
                var newFilter = new RSI((int)json["Mode"], (decimal)json["PriceValue"], (int)json["Period"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Stoch
            if ((string)json["ID"] == "12")
            {
                var newFilter = new Stoch((int)json["Mode"], (decimal)json["PriceValue"], (int)json["Period"], (int)json["Period2"], (int)json["Period3"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Stoch
            if ((string)json["ID"] == "13")
            {
                var newFilter = new StochRSI((int)json["Mode"], (decimal)json["PriceValue"], (int)json["Period"], (int)json["Period2"], (int)json["Period3"], (int)json["Period4"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Email Notify
            if ((string)json["ID"] == "14")
            {
                var newFilter = new EmailNotify(Settings.GoogleLogin, Settings.GooglePassword, (string)json["Text"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // URL
            if ((string)json["ID"] == "15")
            {
                var newFilter = new URL((string)json["Text"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Spread
            if ((string)json["ID"] == "16")
            {
                var newFilter = new Spread((int)json["Mode"], (decimal)json["Diff"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }
            // MA Spread
            if ((string)json["ID"] == "17")
            {
                var newFilter = new MASpread((int)json["Mode"], (decimal)json["Diff"], (int)json["Line1"], (int)json["Period"], (int)json["Line2"], (int)json["Period2"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Price Change
            if ((string)json["ID"] == "18")
            {
                var newFilter = new CandlePriceChange((string)json["Cur1"], (string)json["Cur2"], (int)json["Mode"], (int)json["Side"], (decimal)json["PriceValue"], (int)json["Period"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // MFI
            if ((string)json["ID"] == "19")
            {
                var newFilter = new MFI((int)json["Mode"], (decimal)json["PriceValue"], (int)json["Period"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // CCI
            if ((string)json["ID"] == "20")
            {
                var newFilter = new CCI((int)json["Mode"], (decimal)json["PriceValue"], (int)json["Period"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Timer
            if ((string)json["ID"] == "21")
            {
                var newFilter = new Timer((int)json["Mode"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Candle Color
            if ((string)json["ID"] == "22")
            {
                var newFilter = new CandleColor((int)json["Mode"], (int)json["Side"], (int)json["Period"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Volume Limit
            if ((string)json["ID"] == "23")
            {
                var newFilter = new VolumeLimit((string)json["Cur1"], (string)json["Cur2"], (int)json["Mode"], (decimal)json["VolumeValue"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Bollinger Bands Width
            if ((string)json["ID"] == "24")
            {
                var newFilter = new BollingerBandsWidth((int)json["Mode"], (int)json["Period"], (decimal)json["Rate"], (string)json["TimeFrame"], (decimal)json["PriceValue"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // Keltner Channels
            if ((string)json["ID"] == "25")
            {
                var newFilter = new KeltnerChannels((int)json["Mode"], (int)json["Period"], (int)json["Period2"], (decimal)json["Rate"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // STARC Bands
            if ((string)json["ID"] == "26")
            {
                var newFilter = new STARCBands((int)json["Mode"], (int)json["Period"], (int)json["Period2"], (decimal)json["Rate"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            // MA Envelopes
            if ((string)json["ID"] == "27")
            {
                var newFilter = new MAEnvelopes((int)json["Mode"], (int)json["Period"], (int)json["Source"], (decimal)json["Rate"], (string)json["TimeFrame"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

			// Donchian Channel
			if ((string)json["ID"] == "28")
			{
				var newFilter = new DonchianChannel((int)json["Mode"], (int)json["Period"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"]);
				newFilter.MyName = filter.MyName;
				newFilter.Group = filter.Group;
				newFilter.Color = filter.Color;
				newFilter.Weight = filter.Weight;
				newFilter.FilterSide = filterSide;

				return newFilter;
			}

            // SuperTrend
            if ((string)json["ID"] == "29")
            {
                var newFilter = new SuperTrend((int)json["Mode"], (int)json["Period"], (decimal)json["Rate"], (string)json["TimeFrame"], (decimal)json["Indent"], (int)json["Duration"]);
                newFilter.MyName = filter.MyName;
                newFilter.Group = filter.Group;
                newFilter.Color = filter.Color;
                newFilter.Weight = filter.Weight;
                newFilter.FilterSide = filterSide;

                return newFilter;
            }

            throw new Exception("code 28"); // Выбран неизвестный фильтр
        }
    }
}
