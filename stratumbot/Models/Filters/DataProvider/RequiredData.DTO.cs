namespace stratumbot.Models.Filters.DataProvider
{
    /// <summary>
    /// DTO of item of list of required data types in DataProvider for specific filter
    /// </summary>
    public class RequiredData
    {
        /// <summary>
        /// Type of data
        /// </summary>
        public DataType DataType;

        /// <summary>
        /// Options to collect data of the type
        /// </summary>
        public DataOptions Options;
    }
}
