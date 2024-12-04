namespace stratumbot.Models.Filters.DataProvider
{
    /// <summary>
    /// Data type collection for DataProvider.
    /// Used for setting a list of requred data for compute filters or indicators.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Represent quotes data type as 
        /// IEnumerable<Quote>
        /// </summary>
        Quotes = 1,

        /// <summary>
        /// Represent current price data type as 
        /// CurrentBuyPrice
        /// CurrentSellPrice
        /// </summary>
        CurrentPrice,

        /// <summary>
        /// Represent 24h trading volume data type
        /// </summary>
        CurrentVolume,

        /// <summary>
        /// Represent 24h change percent data type
        /// </summary>
        PriceChangePercent,

        /// <summary>
        /// Represent DOM data type
        /// </summary>
        DOM,

        /// <summary>
        /// Represent Times info about the TThread
        /// </summary>
        Times
    }
}