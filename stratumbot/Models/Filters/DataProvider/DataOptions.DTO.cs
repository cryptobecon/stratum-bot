namespace stratumbot.Models.Filters.DataProvider
{
    /// <summary>
    /// Data options to use in DataProvider for getting data of specific data type
    /// </summary>
    public class DataOptions
    {
        /// <summary>
        /// First currency
        /// </summary>
        public string Cur1;

        /// <summary>
        /// Second (base) currency
        /// </summary>
        public string Cur2;

        /// <summary>
        /// TimeFrame
        /// </summary>
        public string TimeFrame; // TODO make a enum type of timeframes, because i will need converter for each exchanges

        /// <summary>
        /// Period (numbers of data items)
        /// </summary>
        public int Period;
    }
}
