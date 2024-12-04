using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stratumbot.Interfaces;
using stratumbot.Models.Exchanges;
using stratumbot.Models.Strategies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using stratumbot.Core;
using System.Linq;
using System.IO;
using stratumbot.Models.Tools;

namespace stratumbot.Models
{
    /// <summary>
    /// Save config -> json (file) dto
    /// Read json (file) dto -> config
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// IConfigText to IConfig converter
        /// </summary>
        /// <param name="configText">IConfigText object</param>
        /// <returns>IConfig object</returns>
        public static IConfig ConfigTextToConfig(IConfigText configText)
        {
            if (configText.Strategy == Strategy.Scalping.ToString())
            {
                return new ScalpingConfig(configText);
            }

            if (configText.Strategy == Strategy.ClassicLong.ToString())
            {
                return new ClassicLongConfig(configText);
            }

            if (configText.Strategy == Strategy.ClassicShort.ToString())
            {
                return new ClassicShortConfig(configText);
            }


            throw new Exception($"Nothing for {configText.Strategy}");
        }

        /// <summary>
        /// IConfig to IConfigText converter
        /// </summary>
        /// <param name="config">IConfig object</param>
        /// <returns>IConfigText object</returns>
        public static IConfigText ConfigToConfigText(IConfig config)
        {
            if (config.Strategy == Strategy.Scalping)
            {
                return (config as ScalpingConfig).IConfigText as ScalpingConfigText; 
            }

            // TODO long/short

            throw new Exception($"Nothing for {config.Strategy.ToString()}");
        }

        /// <summary>
        /// Save IConfigText object to file as JSON
        /// </summary>
        public static void Save(string fileName, IConfigText config)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            if(json.IsValidJson())
            {
                StreamWriter file = new StreamWriter($"Strategies/{config.Strategy}/{fileName}.strat");
                // System.IO.StreamWriter file = new System.IO.StreamWriter($"Collections/{_config.Strategy}/{_fileName}.strat"); // TODO для сохр коллекций 
                file.WriteLine(json);
                file.Close();
            } else
            {
                throw new Exception("code 16");
            }

        }

        /// <summary>
        /// Read JSON string and convert it to IConfigText object
        /// </summary>
        /// <returns>IConfigText object</returns>
        public static IConfigText Read(string jsonText)
        {
            var configJson = JObject.Parse(jsonText);

            if (configJson["Strategy"].ToString() == Strategy.Scalping.ToString())
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ScalpingConfigText>(jsonText);
            }

            throw new Exception("code 18");
        }























        /// <summary>
        /// Метод сохраняет конфиг в файл для коллекции
        /// </summary>
        public static void SaveCollection(string _collectionName, string _fileName, IConfigText _config)
        {
            var json = JsonConvert.SerializeObject(_config, Formatting.Indented);

            if (json.IsValidJson())
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter($"Collections/{_collectionName}/{_fileName}.strat");
                file.WriteLine(json);
                file.Close();
            }
            else
            {
                throw new Exception("code 17");
            }
        }

        /// <summary>
        /// Метод подготавливает директорию для коллекции и возвращает её имя
        /// </summary>
        public static string CreateCollectionDirectory(string _directorynName)
        {
            // Заменяем запрещеные символы
            string collectionName = System.IO.Path.GetInvalidFileNameChars().Aggregate(_directorynName, (current, invalid_char) => current.Replace(invalid_char.ToString(), "_"));

            // Создаём директорию 
            if (Directory.Exists("Collections/" + collectionName))
                Directory.Delete("Collections/" + collectionName, true);
            Directory.CreateDirectory("Collections/" + collectionName);

            return collectionName;
        }


        /// <summary>
        /// Метож для загрузки коллекции. Получает имя коллекции. Возвращает список из CollectionItem(Config, Exchange).
        /// </summary>
        public static List<CollectionItem> LoadCollection(string _directorynName)
        {
            // Получаем все конфиги
            string[] configFiles = Directory.GetFiles(@"Collections/" + _directorynName + "/", "*.strat");

            List<CollectionItem> configs = new List<CollectionItem>(); // Список полученых конфигов

            foreach(var configFile in configFiles)
            {
                string configText = File.ReadAllText(configFile);

                if (!configText.IsValidJson())
                    throw new Exception("Invalid JSON collection file");

                // Считать конфиг
                var config = ConfigManager.Read(configText);
                Enum.TryParse(config.Strategy, out Strategy strategyId);

                var exchangeName = configFile.Split('/').Last().Split('_').First();
                Enum.TryParse(exchangeName, out Exchange exchange);

                if (config.Strategy == Strategy.Scalping.ToString())
                {
                    var scalpingConfig = new ScalpingConfigText();

                    scalpingConfig.Budget = (config as ScalpingConfigText).Budget;
                    scalpingConfig.Cur1 = (config as ScalpingConfigText).Cur1;
                    scalpingConfig.Cur2 = (config as ScalpingConfigText).Cur2;
                    scalpingConfig.MinSpread = (config as ScalpingConfigText).MinSpread;
                    scalpingConfig.OptSpread = (config as ScalpingConfigText).OptSpread;
                    scalpingConfig.MinMarkup = (config as ScalpingConfigText).MinMarkup;
                    scalpingConfig.OptMarkup = (config as ScalpingConfigText).OptMarkup;
                    scalpingConfig.ZeroSell = (config as ScalpingConfigText).ZeroSell;
                    scalpingConfig.InTimeout = (config as ScalpingConfigText).InTimeout;
                    scalpingConfig.IsDCA = (config as ScalpingConfigText).IsDCA;
                    scalpingConfig.DCAStepCount = (config as ScalpingConfigText).DCAStepCount;
                    scalpingConfig.DCAProfitPercent = (config as ScalpingConfigText).DCAProfitPercent;
                    scalpingConfig.DCASteps = (config as ScalpingConfigText).DCASteps;
                    scalpingConfig.FirsOredersAmountPercentIgnor = (config as ScalpingConfigText).FirsOredersAmountPercentIgnor;
                    scalpingConfig.FirsOredersCountIgnor = (config as ScalpingConfigText).FirsOredersCountIgnor;
                    scalpingConfig.StopLoss = (config as ScalpingConfigText).StopLoss;
                    scalpingConfig.FiltersBuy = (config as ScalpingConfigText).FiltersBuy; // TODO TEST
                    configs.Add(new CollectionItem { Config = scalpingConfig, Exchange = exchange });
                }

                // TODO Добавить Classic стратегии !!!!
                
            }

            return configs;
        }

    }

    /// <summary>
    /// Класс итем коллекции. Нужен чтобы помещать в него биржу, к которому был привязан поток
    /// </summary>
    public class CollectionItem
    {
        public IConfigText Config { get; set; }
        public Exchange Exchange { get; set; }
    }
}
