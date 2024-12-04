namespace stratumbot.Interfaces
{
    /// <summary>
    /// Represent text (json) config
    /// Used for IConfig(object) <> IConfigFile(text-json) convertation
    /// </summary>
    public interface IConfigText
    {
        /// <summary>
        /// Strategy name
        /// </summary>
        string Strategy { get; set; }
    }
}
