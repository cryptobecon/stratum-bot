using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Core
{
    public static class Calc
    {

        /// <summary>
        /// Сколько будет percent процентов от amount. Результат в количестве amount.
        /// </summary>
        public static decimal AmountOfPercent(decimal percent, decimal amount)
        {
            return (amount / 100) * percent;
        }

        /// <summary>
        /// Сколько процентов составляет количество smallAmount от количества bigAmount. Вернет результат в процентах
        /// </summary>
        public static decimal PercentOfAmount(decimal smallAmount, decimal bigAmount)
        {
            return ((smallAmount / bigAmount) * 100);
        }

        /// <summary>
        /// Округлить наверх с указанным шагом (точностью)
        /// </summary>
        public static decimal RoundUp(decimal value, decimal step)
        {
            return Math.Ceiling(value / step) * step; 
            // if -value : Math.Floor(value / size) * size;
        }

        /// <summary>
        /// Округлить вниз с указанным шагом (точностью).
        /// </summary>
        public static decimal RoundDown(decimal value, decimal step)
        {
            return Math.Floor(value / step) * step;
            // if -value : Math.Ceiling(value / step) * step;
        }

        /// <summary>
        /// Метод рассчитывает сколько нужно купить монет на основе бюджета, цены покупки и шага объема stepSize. Округляет на один шаг вниз.
        /// </summary>
        public static decimal ComputeBuyAmountByBudget(decimal budget, decimal buyPrice, decimal stepSize)
        {
            decimal amount = budget / buyPrice;
            if (amount < stepSize)
			{
                stratumbot.Models.Logs.Logger.Error("Бюджет меньше допустимого!");
                throw new AutoStopException("Amount < stepSize.");
            }
            return RoundDown(amount, stepSize);
        }
    }
}
