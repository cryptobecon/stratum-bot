using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.DTO
{
    /// <summary>
    /// DTO класс для сохранения разных временных точек (для индикатора Timer)
    /// Возвращаются значения в сколько прошло сек
    /// Задаются значения в Unix Timestamp (сек)
    /// </summary>
    public class Times
    {
        // Начало итерации
        private double startIteration;
        public double StartIteration
        {
            get
            {
                double sec = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - startIteration;
                return sec;
            }
            set
            {
                startIteration = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }

        // Первый BUY ордер
        public bool IsFirstBuySet { get; set; } = false;
        private double firstBuyOrder;
        public double FirstBuyOrder
        {
            get
            {
                double sec = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - firstBuyOrder;
                return sec;
            }
            set
            {
                firstBuyOrder = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }

        // Послежний BUY ордер
        private double lastBuyOrder;
        public double LastBuyOrder
        {
            get
            {
                double sec = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - lastBuyOrder;
                return sec;
            }
            set
            {
                // Указываемя время первого BUY ордера
                if (!this.IsFirstBuySet) { this.FirstBuyOrder = -1; this.IsFirstBuySet = true; }
                lastBuyOrder = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }

        // Первый SELL ордер
        public bool IsFirstSellSet { get; set; } = false;
        private double firstSellOrder;
        public double FirstSellOrder
        {
            get
            {
                double sec = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - firstSellOrder;
                return sec;
            }
            set
            {
                firstSellOrder = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }

        // Послежний SELL ордер
        private double lastSellOrder;
        public double LastSellOrder
        {
            get
            {
                double sec = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - lastSellOrder;
                return sec;
            }
            set
            {
                // Указываемя время первого SELL ордера
                if (!this.IsFirstSellSet) { this.FirstSellOrder = -1; this.IsFirstSellSet = true; }
                lastSellOrder = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }
    }
}
