using stratumbot.DTO;
using stratumbot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Indicators
{
    class BollingerBandsi : IIndicator
    {
        public string Name { get; set; } = "Bollinger Bands";

        public BollingerBandsItem Result(List<Quote> data, int period, decimal deviationsValue)
        {
            decimal middleBB = 0;
            decimal upperBB = 0;
            decimal lowerBB = 0;
            decimal standartDevitions = 0; // Стандартное отклонение
            decimal squaredSMA = 0; // SMA в квадрате

            foreach (var price in data)
            {
                middleBB += price.Close;
            }
            middleBB = middleBB / period; // SMA

            foreach (var price in data)
            {
                squaredSMA += ( price.Close - middleBB ) * (price.Close - middleBB); // TODO FUTURE не только Close но и TP и другие.
            }
            squaredSMA = squaredSMA / period;
            standartDevitions = (decimal)Math.Sqrt(Decimal.ToDouble(squaredSMA));

            upperBB = middleBB + (deviationsValue * standartDevitions);
            lowerBB = middleBB - (deviationsValue * standartDevitions);

            return new BollingerBandsItem
            {
                Upper = upperBB,
                Middle = middleBB,
                Lower = lowerBB
            };
        }
    }
}