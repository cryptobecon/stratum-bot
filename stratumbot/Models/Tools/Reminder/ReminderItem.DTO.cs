using stratumbot.DTO;

namespace stratumbot.Models.Tools.Reminder
{
    /// <summary>
    /// 
    /// Reminder item DTO.
    /// 
    /// Used for sell/buy reminder orders with problem.
    /// 
    /// </summary>
    public class ReminderItem
    {
        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Original buy price
        /// </summary>
        public decimal OriginalBuyPrice { get; set; }

        /// <summary>
        /// Original sell price
        /// </summary>
        public decimal OriginalSellPrice { get; set; }

    }
}
