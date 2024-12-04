namespace stratumbot.Models.Tools
{
    /// <summary>
    /// DTO represent a single DCA step configuration in IConfig objects
    /// </summary>
    public class DCAStepConfig
    {
        /// <summary>
        /// Step position
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Is the drop value of step set as percent
        /// </summary>
        public bool IsDropPercentage { get; set; }

        /// <summary>
        /// Drop value
        /// </summary>
        public decimal Drop { get; set; }

        /// <summary>
        /// Is the amount value of step set as percent
        /// </summary>
        public bool IsAmountPercentage { get; set; }

        /// <summary>
        /// Amount value
        /// </summary>
        public decimal Amount { get; set; }
    }
}
