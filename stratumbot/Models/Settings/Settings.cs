using Newtonsoft.Json;
using stratumbot.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    [Newtonsoft.Json.JsonObject(Newtonsoft.Json.MemberSerialization.OptIn)]
    public static class Settings
    {
        // Глобальные
        public static ushort SecretKey = 0; // Секретный ключ для второго этапа шифрования настроек из реестра

        // Общее
        public static bool ProMode = false; // Режим профессианальный

        public static int LogsLenth = 100; // Длинна логов
        public static List<string> Languages = new List<string>() { "English", "Russian" };
        public static Dictionary<string, string> LanguagesCode = new Dictionary<string, string>();
        public static string Lang; // Язык
        public static bool BabloVoice = true;
        public static bool Debug = true; // Режим отладки
        public static int ResponseLenth = 500; // Обрезать длинные ответы

        // Таймауты
        public static int CheckTimeout = 10000; // проверка рынка на возможность войти
        public static int CheckOrderTimeout = 5000; // проверка ордеров на исполнение
        public static int BetweenRequestTimeout = 500; // задержка между некоторыми запросами
        public static int FailedQueryTimeout = 1000; // частота запросов между неудачными попытками
        public static int FiltersTimeout = 3000; // частота проверок фильтров и индикаторов
        public static int RecheckBuyFiltersTimeout = 0; // повторная проверка фильтров на покуаку
        public static int RecheckSellFiltersTimeout = 0; // повторная проверка фильтров на продажу

        // Биржи
        public static List<APITokens> API = new List<APITokens>(); // список API ключей от всех бирж

        public static decimal BinanceRecvWindow = 5000;
        public static string ApproximationPercent = "0.2";

        // Данные
        public static string GoogleLogin = "";
        public static string GooglePassword = "";

        // Strategies

        // Scalping
        public static bool ParamInPercentScalpingAutofit = false; // Параметры в процентах (true) или в пунктах (false)
        public static string MinSpreadScalpingAutofit = "0.01";
        public static string OptSpreadScalpingAutofit = "0.06";
        public static string MinMarkupScalpingAutofit = "0.02";
        public static string OptMarkupScalpingAutofit = "0.06";
        public static string ZeroSellScalpingAutofit = "0.15";
        public static string InTimeoutScalpingAutofit = "0";

        // DCA
        public static bool IsDCAAutofit = true;
        public static string DCAProfitPercentAutofit = "0.05";
        public static string DCAStepCountAutofit = "0";
        public static ObservableCollection<string[]> DCAStepsAutofit = new ObservableCollection<string[]>();

        public static int StopLossTimeout = 0; // StopLoss Timeout

        // Scalping

        public static int BuyCanceledScalpingSituation = 3;
        public static int BuyLittleFilledPriceIncreasedScalpingSituation = 0;
        public static int BuyLittleFilledCanceledScalpingSituation = 0;
        public static int SellCanceledLittleReminderScalpingSituation = 0;
        public static int SellLittleReminderPriceDroppedScalpingSituation = 0;

        public static int XOrdersAheadLittleFilledPriceIncreasedScalpingSituation = 0;
        public static int SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation = 0;
        public static decimal DropPercentLittleFilledPriceIncreasedScalpingSituation = 0;
        public static decimal AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation = 0;

        public static int XOrdersAheadBuyFilledEnoughPriceIncreasedScalping = 0;
        public static int SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping = 0;
        public static decimal DropPercentBuyFilledEnoughPriceIncreasedScalping = 0;
        public static decimal AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping = 0;

        public static int XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation = 0;
        public static int SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation = 0;
        public static decimal DropPercentSellLittleReminderPriceDroppedScalpingSituation = 0;
        public static decimal AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation = 0;

        // Classic Long

        public static int BuyCanceledClassicLongSituation = 3;
		public static int BuyLittleFilledPriceIncreasedClassicLongSituation = 0;
		public static int BuyLittleFilledCanceledClassicLongSituation = 0;
		public static int SellCanceledLittleReminderClassicLongSituation = 0;
		public static int SellLittleReminderPriceDroppedClassicLongSituation = 0;

		public static int XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation = 0;
		public static int SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation = 0;
		public static decimal DropPercentLittleFilledPriceIncreasedClassicLongSituation = 0;
		public static decimal AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation = 0;

		public static int XOrdersAheadBuyFilledEnoughPriceIncreased = 0;
		public static int SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased = 0;
		public static decimal DropPercentBuyFilledEnoughPriceIncreased = 0;
		public static decimal AheadOrdersVolumeBuyFilledEnoughPriceIncreased = 0;

		public static int XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation = 0;
		public static int SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation = 0;
		public static decimal DropPercentSellLittleReminderPriceDroppedClassicLongSituation = 0;
		public static decimal AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation = 0;


		public static string StopAfterXStopLoss = "0";

        // Classic Short

        public static int SellCanceledClassicShortSituation = 3;
        public static int SellLittleFilledPriceDroppedClassicShortSituation = 0;
        public static int SellLittleFilledCanceledClassicShortSituation = 0;
        public static int BuyCanceledLittleReminderClassicShortSituation = 0;
        public static int BuyLittleReminderPriceIncreasedClassicShortSituation = 0;

        public static int XOrdersAheadLittleFilledPriceDroppedClassicShortSituation = 0;
        public static int SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation = 0;
        public static decimal DropPercentLittleFilledPriceDroppedClassicShortSituation = 0;
        public static decimal AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation = 0;

        public static int XOrdersAheadSellFilledEnoughPriceDropped = 0;
        public static int SecondsAfterLastUpdateSellFilledEnoughPriceDropped = 0;
        public static decimal DropPercentSellFilledEnoughPriceDropped = 0;
        public static decimal AheadOrdersVolumeSellFilledEnoughPriceDropped = 0;

        public static int XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation = 0;
        public static int SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation = 0;
        public static decimal DropPercentSellLittleReminderPriceIncreasedClassicShortSituation = 0;
        public static decimal AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation = 0;

        #region StratumBox

        public static bool MyOrdersBox = true;

        #endregion

        // Загрузка настроек в бота
        public static void Load()
        {
            var settingsJsonStr = Encryption.XOR(File.ReadAllText("settings"));
            var settingsJson = Newtonsoft.Json.Linq.JToken.Parse(settingsJsonStr);

            // Общее
            Lang = (string)settingsJson["Lang"] ?? "ru";
            ProMode = (bool?)settingsJson["ProMode"] ?? false;
            BabloVoice = (bool?)settingsJson["BabloVoice"] ?? true;
            Debug = (bool?)settingsJson["Debug"] ?? true;
            LogsLenth = (int?)settingsJson["LogsLenth"] ?? 100;
            //Таймауты
            CheckTimeout = (int?)settingsJson["CheckTimeout"] ?? 10000;
            CheckOrderTimeout = (int?)settingsJson["CheckOrderTimeout"] ?? 5000;
            BetweenRequestTimeout = (int?)settingsJson["BetweenRequestTimeout"] ?? 500;
            FiltersTimeout = (int?)settingsJson["FiltersTimeout"] ?? 3000;
            RecheckBuyFiltersTimeout = (int?)settingsJson["RecheckBuyFiltersTimeout"] ?? 0;
            RecheckSellFiltersTimeout = (int?)settingsJson["RecheckSellFiltersTimeout"] ?? 0;

            // Биржи
            if (settingsJson["API"] != null)
            {
                var APITokens = new APITokens();
                List<Tokens> Tokens = new List<Tokens>();

                foreach (var exchangesAPI in settingsJson["API"])
                {

                    APITokens = new APITokens();
                    Tokens = new List<Tokens>();

                    if ((int)exchangesAPI["Exchange"] == (int)Exchange.Binance)
                    {
                        APITokens.Exchange = Exchange.Binance;
                    }

                    if ((int)exchangesAPI["Exchange"] == (int)Exchange.BinanceFutures)
                    {
                        APITokens.Exchange = Exchange.BinanceFutures;
                    }

                    foreach (var tokens in exchangesAPI["Tokens"])
                    {
                        
                        Tokens.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<Tokens>(tokens.ToString()));
                    }

                    APITokens.Tokens = Tokens;
                    API.Add(APITokens);
                }
                
            }


            // Стратегии
            // Скальпинг
            ParamInPercentScalpingAutofit = (bool?)settingsJson["ParamInPercentScalpingAutofit"] ?? true;
            MinSpreadScalpingAutofit = (string)settingsJson["MinSpreadScalpingAutofit"] ?? "0.2";
            OptSpreadScalpingAutofit = (string)settingsJson["OptSpreadScalpingAutofit"] ?? "0.6";
            MinMarkupScalpingAutofit = (string)settingsJson["MinMarkupScalpingAutofit"] ?? "0.2";
            OptMarkupScalpingAutofit = (string)settingsJson["OptMarkupScalpingAutofit"] ?? "0.6";
            ZeroSellScalpingAutofit = (string)settingsJson["ZeroSellScalpingAutofit"] ?? "0.15";
            InTimeoutScalpingAutofit = (string)settingsJson["InTimeoutScalpingAutofit"] ?? "0";

            IsDCAAutofit = (bool?)settingsJson["IsDCAAutofit"] ?? false;
            if (IsDCAAutofit)
            {
                DCAProfitPercentAutofit = (string)settingsJson["DCAProfitPercentAutofit"] ?? "0.05";
                DCAStepCountAutofit = (string)settingsJson["DCAStepCountAutofit"] ?? "1";
                foreach (var dcaStep in settingsJson["DCAStepsAutofit"])
                {
                    DCAStepsAutofit.Add(new string[] { dcaStep[0].ToString(), dcaStep[1].ToString(), dcaStep[2].ToString() });
                }
            }
            ApproximationPercent = (string)settingsJson["ApproximationPercent"] ?? "0.2";
            StopLossTimeout = (int?)settingsJson["StopLossTimeout"] ?? 0;

            // Scalping
            BuyCanceledScalpingSituation = (int?)settingsJson["BuyCanceledScalpingSituation"] ?? 3;
            BuyLittleFilledPriceIncreasedScalpingSituation = (int?)settingsJson["BuyLittleFilledPriceIncreasedScalpingSituation"] ?? 0;
            BuyLittleFilledCanceledScalpingSituation = (int?)settingsJson["BuyLittleFilledCanceledScalpingSituation"] ?? 0;
            SellCanceledLittleReminderScalpingSituation = (int?)settingsJson["SellCanceledLittleReminderScalpingSituation"] ?? 0;
            SellLittleReminderPriceDroppedScalpingSituation = (int?)settingsJson["SellLittleReminderPriceDroppedScalpingSituation"] ?? 0;

            XOrdersAheadLittleFilledPriceIncreasedScalpingSituation = (int?)settingsJson["XOrdersAheadLittleFilledPriceIncreasedScalpingSituation"] ?? 0;
            SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation = (int?)settingsJson["SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation"] ?? 0;
            DropPercentLittleFilledPriceIncreasedScalpingSituation = (decimal?)settingsJson["DropPercentLittleFilledPriceIncreasedScalpingSituation"] ?? 0;
            AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation = (decimal?)settingsJson["AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation"] ?? 0;

            XOrdersAheadBuyFilledEnoughPriceIncreasedScalping = (int?)settingsJson["XOrdersAheadBuyFilledEnoughPriceIncreasedScalping"] ?? 0;
            SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping = (int?)settingsJson["SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping"] ?? 0;
            DropPercentBuyFilledEnoughPriceIncreasedScalping = (decimal?)settingsJson["DropPercentBuyFilledEnoughPriceIncreasedScalping"] ?? 0;
            AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping = (decimal?)settingsJson["AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping"] ?? 0;

            XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation = (int?)settingsJson["XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation"] ?? 0;
            SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation = (int?)settingsJson["SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation"] ?? 0;
            DropPercentSellLittleReminderPriceDroppedScalpingSituation = (decimal?)settingsJson["DropPercentSellLittleReminderPriceDroppedScalpingSituation"] ?? 0;
            AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation = (decimal?)settingsJson["AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation"] ?? 0;


            // Classic Long
            BuyCanceledClassicLongSituation = (int?)settingsJson["BuyCanceledClassicLongSituation"] ?? 3;
			BuyLittleFilledPriceIncreasedClassicLongSituation = (int?)settingsJson["BuyLittleFilledPriceIncreasedClassicLongSituation"] ?? 0;
			BuyLittleFilledCanceledClassicLongSituation = (int?)settingsJson["BuyLittleFilledCanceledClassicLongSituation"] ?? 0;
			SellCanceledLittleReminderClassicLongSituation = (int?)settingsJson["SellCanceledLittleReminderClassicLongSituation"] ?? 0;
			SellLittleReminderPriceDroppedClassicLongSituation = (int?)settingsJson["SellLittleReminderPriceDroppedClassicLongSituation"] ?? 0;

			XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation = (int?)settingsJson["XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation"] ?? 0;
			SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation = (int?)settingsJson["SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation"] ?? 0;
			DropPercentLittleFilledPriceIncreasedClassicLongSituation = (decimal?)settingsJson["DropPercentLittleFilledPriceIncreasedClassicLongSituation"] ?? 0;
			AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation = (decimal?)settingsJson["AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation"] ?? 0;

			XOrdersAheadBuyFilledEnoughPriceIncreased = (int?)settingsJson["XOrdersAheadBuyFilledEnoughPriceIncreased"] ?? 0;
			SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased = (int?)settingsJson["SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased"] ?? 0;
			DropPercentBuyFilledEnoughPriceIncreased = (decimal?)settingsJson["DropPercentBuyFilledEnoughPriceIncreased"] ?? 0;
			AheadOrdersVolumeBuyFilledEnoughPriceIncreased = (decimal?)settingsJson["AheadOrdersVolumeBuyFilledEnoughPriceIncreased"] ?? 0;

			XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation = (int?)settingsJson["XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation"] ?? 0;
			SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation = (int?)settingsJson["SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation"] ?? 0;
			DropPercentSellLittleReminderPriceDroppedClassicLongSituation = (decimal?)settingsJson["DropPercentSellLittleReminderPriceDroppedClassicLongSituation"] ?? 0;
			AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation = (decimal?)settingsJson["AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation"] ?? 0;


			StopAfterXStopLoss = (string)settingsJson["StopAfterXStopLoss"] ?? "0";

            // Classic Short
            SellCanceledClassicShortSituation = (int?)settingsJson["SellCanceledClassicShortSituation"] ?? 3;
            SellLittleFilledPriceDroppedClassicShortSituation = (int?)settingsJson["SellLittleFilledPriceDroppedClassicShortSituation"] ?? 0;
            SellLittleFilledCanceledClassicShortSituation = (int?)settingsJson["SellLittleFilledCanceledClassicShortSituation"] ?? 0;
            BuyCanceledLittleReminderClassicShortSituation = (int?)settingsJson["BuyCanceledLittleReminderClassicShortSituation"] ?? 0;
            BuyLittleReminderPriceIncreasedClassicShortSituation = (int?)settingsJson["BuyLittleReminderPriceIncreasedClassicShortSituation"] ?? 0;

            XOrdersAheadLittleFilledPriceDroppedClassicShortSituation = (int?)settingsJson["XOrdersAheadLittleFilledPriceDroppedClassicShortSituation"] ?? 0;
            SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation = (int?)settingsJson["SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation"] ?? 0;
            DropPercentLittleFilledPriceDroppedClassicShortSituation = (decimal?)settingsJson["DropPercentLittleFilledPriceDroppedClassicShortSituation"] ?? 0;
            AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation = (decimal?)settingsJson["AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation"] ?? 0;

            XOrdersAheadSellFilledEnoughPriceDropped = (int?)settingsJson["XOrdersAheadSellFilledEnoughPriceDropped"] ?? 0;
            SecondsAfterLastUpdateSellFilledEnoughPriceDropped = (int?)settingsJson["SecondsAfterLastUpdateSellFilledEnoughPriceDropped"] ?? 0;
            DropPercentSellFilledEnoughPriceDropped = (decimal?)settingsJson["DropPercentSellFilledEnoughPriceDropped"] ?? 0;
            AheadOrdersVolumeSellFilledEnoughPriceDropped = (decimal?)settingsJson["AheadOrdersVolumeSellFilledEnoughPriceDropped"] ?? 0;

            XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation = (int?)settingsJson["XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation"] ?? 0;
            SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation = (int?)settingsJson["SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation"] ?? 0;
            DropPercentSellLittleReminderPriceIncreasedClassicShortSituation = (decimal?)settingsJson["DropPercentSellLittleReminderPriceIncreasedClassicShortSituation"] ?? 0;
            AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation = (decimal?)settingsJson["AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation"] ?? 0;

            // Данные
            GoogleLogin = (string)settingsJson["GoogleLogin"] ?? "";
            GooglePassword = (string)settingsJson["GooglePassword"] ?? "";

            // Регистрируем коды языков
            LanguagesCode["English"] = "en"; LanguagesCode["en"] = "English";
            LanguagesCode["Russian"] = "ru"; LanguagesCode["ru"] = "Russian";

            // StratumBox

            MyOrdersBox = (bool?)settingsJson["MyOrdersBox"] ?? false;

        }

        

        // Сохранение настроек в файл
        public static void Save()
        {

            var settings = new SettingObject()
            {
                /* Общее */
                ProMode = ProMode,
                LogsLenth = LogsLenth,
                Lang = Lang,
                BabloVoice = BabloVoice,
                Debug = Debug,
                /* Таймауты */
                CheckTimeout = CheckTimeout,
                CheckOrderTimeout = CheckOrderTimeout,
                BetweenRequestTimeout = BetweenRequestTimeout,
                FiltersTimeout = FiltersTimeout,
                RecheckBuyFiltersTimeout = RecheckBuyFiltersTimeout,
                RecheckSellFiltersTimeout = RecheckSellFiltersTimeout,
                /* Биржи */
                API = API,
                /* Стратегии */
                ParamInPercentScalpingAutofit = ParamInPercentScalpingAutofit,
                MinSpreadScalpingAutofit = MinSpreadScalpingAutofit,
                OptSpreadScalpingAutofit = OptSpreadScalpingAutofit,
                MinMarkupScalpingAutofit = MinMarkupScalpingAutofit,
                OptMarkupScalpingAutofit = OptMarkupScalpingAutofit,
                ZeroSellScalpingAutofit = ZeroSellScalpingAutofit,
                InTimeoutScalpingAutofit = InTimeoutScalpingAutofit,
                IsDCAAutofit = IsDCAAutofit,
                DCAProfitPercentAutofit = DCAProfitPercentAutofit,
                DCAStepCountAutofit = DCAStepCountAutofit,
                DCAStepsAutofit = DCAStepsAutofit,
                ApproximationPercent = ApproximationPercent,
                StopLossTimeout = StopLossTimeout,

                /* Scalping */
                BuyCanceledScalpingSituation = BuyCanceledScalpingSituation,
                BuyLittleFilledCanceledScalpingSituation = BuyLittleFilledCanceledScalpingSituation,
                BuyLittleFilledPriceIncreasedScalpingSituation = BuyLittleFilledPriceIncreasedScalpingSituation,
                SellCanceledLittleReminderScalpingSituation = SellCanceledLittleReminderScalpingSituation,
                SellLittleReminderPriceDroppedScalpingSituation = SellLittleReminderPriceDroppedScalpingSituation,

                XOrdersAheadLittleFilledPriceIncreasedScalpingSituation = XOrdersAheadLittleFilledPriceIncreasedScalpingSituation,
                SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation = SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation,
                DropPercentLittleFilledPriceIncreasedScalpingSituation = DropPercentLittleFilledPriceIncreasedScalpingSituation,
                AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation = AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation,

                XOrdersAheadBuyFilledEnoughPriceIncreasedScalping = XOrdersAheadBuyFilledEnoughPriceIncreasedScalping,
                SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping = SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping,
                DropPercentBuyFilledEnoughPriceIncreasedScalping = DropPercentBuyFilledEnoughPriceIncreasedScalping,
                AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping = AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping,

                XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation = XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation,
                SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation = SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation,
                DropPercentSellLittleReminderPriceDroppedScalpingSituation = DropPercentSellLittleReminderPriceDroppedScalpingSituation,
                AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation = AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation,


                /* Classic Long */
                BuyCanceledClassicLongSituation = BuyCanceledClassicLongSituation,
				BuyLittleFilledCanceledClassicLongSituation = BuyLittleFilledCanceledClassicLongSituation,
				BuyLittleFilledPriceIncreasedClassicLongSituation = BuyLittleFilledPriceIncreasedClassicLongSituation,
				SellCanceledLittleReminderClassicLongSituation = SellCanceledLittleReminderClassicLongSituation,
				SellLittleReminderPriceDroppedClassicLongSituation = SellLittleReminderPriceDroppedClassicLongSituation,

				XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation = XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation,
				SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation = SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation,
				DropPercentLittleFilledPriceIncreasedClassicLongSituation = DropPercentLittleFilledPriceIncreasedClassicLongSituation,
				AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation = AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation,

				XOrdersAheadBuyFilledEnoughPriceIncreased = XOrdersAheadBuyFilledEnoughPriceIncreased,
				SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased = SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased,
				DropPercentBuyFilledEnoughPriceIncreased = DropPercentBuyFilledEnoughPriceIncreased,
				AheadOrdersVolumeBuyFilledEnoughPriceIncreased = AheadOrdersVolumeBuyFilledEnoughPriceIncreased,

				XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation = XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation,
				SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation = SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation,
				DropPercentSellLittleReminderPriceDroppedClassicLongSituation = DropPercentSellLittleReminderPriceDroppedClassicLongSituation,
				AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation = AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation,

				StopAfterXStopLoss = StopAfterXStopLoss,

                /* Classic Short */
                SellCanceledClassicShortSituation = SellCanceledClassicShortSituation,
                SellLittleFilledPriceDroppedClassicShortSituation = SellLittleFilledPriceDroppedClassicShortSituation,
                SellLittleFilledCanceledClassicShortSituation = SellLittleFilledCanceledClassicShortSituation,
                BuyCanceledLittleReminderClassicShortSituation = BuyCanceledLittleReminderClassicShortSituation,
                BuyLittleReminderPriceIncreasedClassicShortSituation = BuyLittleReminderPriceIncreasedClassicShortSituation,

                XOrdersAheadLittleFilledPriceDroppedClassicShortSituation = XOrdersAheadLittleFilledPriceDroppedClassicShortSituation,
                SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation = SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation,
                DropPercentLittleFilledPriceDroppedClassicShortSituation = DropPercentLittleFilledPriceDroppedClassicShortSituation,
                AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation = AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation,

                XOrdersAheadSellFilledEnoughPriceDropped = XOrdersAheadSellFilledEnoughPriceDropped,
                SecondsAfterLastUpdateSellFilledEnoughPriceDropped = SecondsAfterLastUpdateSellFilledEnoughPriceDropped,
                DropPercentSellFilledEnoughPriceDropped = DropPercentSellFilledEnoughPriceDropped,
                AheadOrdersVolumeSellFilledEnoughPriceDropped = AheadOrdersVolumeSellFilledEnoughPriceDropped,

                XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation = XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation,
                SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation = SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation,
                DropPercentSellLittleReminderPriceIncreasedClassicShortSituation = DropPercentSellLittleReminderPriceIncreasedClassicShortSituation,
                AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation = AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation,

                /* Данные */
                GoogleLogin = GoogleLogin,
                GooglePassword = GooglePassword,
                /* StratumBox */
                MyOrdersBox =   MyOrdersBox
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

            
            File.WriteAllText("settings", Encryption.XOR(json));
        }

        

    }

    public class SettingObject
    {
        [JsonProperty("ProMode")]
        public bool ProMode;
        [JsonProperty("LogsLenth")]
        public int LogsLenth;
        [JsonProperty("Lang")]
        public string Lang;
        [JsonProperty("BabloVoice")]
        public bool BabloVoice;
        [JsonProperty("Debug")]
        public bool Debug;

        [JsonProperty("CheckTimeout")]
        public int CheckTimeout;
        [JsonProperty("CheckOrderTimeout")]
        public int CheckOrderTimeout;
        [JsonProperty("BetweenRequestTimeout")]
        public int BetweenRequestTimeout;
        [JsonProperty("FiltersTimeout")]
        public int FiltersTimeout;
        [JsonProperty("RecheckBuyFiltersTimeout")]
        public int RecheckBuyFiltersTimeout;
        [JsonProperty("RecheckSellFiltersTimeout")]
        public int RecheckSellFiltersTimeout;

        [JsonProperty("API")]
        public List<APITokens> API;

        [JsonProperty("ParamInPercentScalpingAutofit")]
        public bool ParamInPercentScalpingAutofit;
        [JsonProperty("MinSpreadScalpingAutofit")]
        public string MinSpreadScalpingAutofit;
        [JsonProperty("OptSpreadScalpingAutofit")]
        public string OptSpreadScalpingAutofit ;
        [JsonProperty("MinMarkupScalpingAutofit")]
        public string MinMarkupScalpingAutofit;
        [JsonProperty("OptMarkupScalpingAutofit")]
        public string OptMarkupScalpingAutofit;
        [JsonProperty("ZeroSellScalpingAutofit")]
        public string ZeroSellScalpingAutofit;
        [JsonProperty("InTimeoutScalpingAutofit")]
        public string InTimeoutScalpingAutofit;

        // DCA
        [JsonProperty("IsDCAAutofit")]
        public bool IsDCAAutofit;
        [JsonProperty("DCAProfitPercentAutofit")]
        public string DCAProfitPercentAutofit;
        [JsonProperty("DCAStepCountAutofit")]
        public string DCAStepCountAutofit;
        [JsonProperty("DCAStepsAutofit")]
        public ObservableCollection<string[]> DCAStepsAutofit;

        [JsonProperty("ApproximationPercent")]
        public string ApproximationPercent;

        [JsonProperty("StopLossTimeout")]
        public int StopLossTimeout;

        [JsonProperty("StopAfterXStopLoss")]
        public string StopAfterXStopLoss;

        // Scalping
        [JsonProperty("BuyCanceledScalpingSituation")]
        public int BuyCanceledScalpingSituation;
        [JsonProperty("BuyLittleFilledPriceIncreasedScalpingSituation")]
        public int BuyLittleFilledPriceIncreasedScalpingSituation;
        [JsonProperty("BuyLittleFilledCanceledScalpingSituation")]
        public int BuyLittleFilledCanceledScalpingSituation;
        [JsonProperty("SellCanceledLittleReminderScalpingSituation")]
        public int SellCanceledLittleReminderScalpingSituation;
        [JsonProperty("SellLittleReminderPriceDroppedScalpingSituation")]
        public int SellLittleReminderPriceDroppedScalpingSituation;

        [JsonProperty("XOrdersAheadLittleFilledPriceIncreasedScalpingSituation")]
        public int XOrdersAheadLittleFilledPriceIncreasedScalpingSituation;
        [JsonProperty("SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation")]
        public int SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation;
        [JsonProperty("DropPercentLittleFilledPriceIncreasedScalpingSituation")]
        public decimal DropPercentLittleFilledPriceIncreasedScalpingSituation;
        [JsonProperty("AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation")]
        public decimal AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation;

        [JsonProperty("XOrdersAheadBuyFilledEnoughPriceIncreasedScalping")]
        public int XOrdersAheadBuyFilledEnoughPriceIncreasedScalping;
        [JsonProperty("SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping")]
        public int SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping;
        [JsonProperty("DropPercentBuyFilledEnoughPriceIncreasedScalping")]
        public decimal DropPercentBuyFilledEnoughPriceIncreasedScalping;
        [JsonProperty("AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping")]
        public decimal AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping;

        [JsonProperty("XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation")]
        public int XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation;
        [JsonProperty("SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation")]
        public int SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation;
        [JsonProperty("DropPercentSellLittleReminderPriceDroppedScalpingSituation")]
        public decimal DropPercentSellLittleReminderPriceDroppedScalpingSituation;
        [JsonProperty("AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation")]
        public decimal AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation;

        // Classic Long
        [JsonProperty("BuyCanceledClassicLongSituation")]
		public int BuyCanceledClassicLongSituation;
		[JsonProperty("BuyLittleFilledPriceIncreasedClassicLongSituation")]
		public int BuyLittleFilledPriceIncreasedClassicLongSituation;
		[JsonProperty("BuyLittleFilledCanceledClassicLongSituation")]
		public int BuyLittleFilledCanceledClassicLongSituation;
		[JsonProperty("SellCanceledLittleReminderClassicLongSituation")]
		public int SellCanceledLittleReminderClassicLongSituation;
		[JsonProperty("SellLittleReminderPriceDroppedClassicLongSituation")]
		public int SellLittleReminderPriceDroppedClassicLongSituation;

		[JsonProperty("XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation")]
		public int XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation;
		[JsonProperty("SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation")]
		public int SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation;
		[JsonProperty("DropPercentLittleFilledPriceIncreasedClassicLongSituation")]
		public decimal DropPercentLittleFilledPriceIncreasedClassicLongSituation;
		[JsonProperty("AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation")]
		public decimal AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation;

		[JsonProperty("XOrdersAheadBuyFilledEnoughPriceIncreased")]
		public int XOrdersAheadBuyFilledEnoughPriceIncreased;
		[JsonProperty("SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased")]
		public int SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased;
		[JsonProperty("DropPercentBuyFilledEnoughPriceIncreased")]
		public decimal DropPercentBuyFilledEnoughPriceIncreased;
		[JsonProperty("AheadOrdersVolumeBuyFilledEnoughPriceIncreased")]
		public decimal AheadOrdersVolumeBuyFilledEnoughPriceIncreased;

		[JsonProperty("XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation")]
		public int XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation;
		[JsonProperty("SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation")]
		public int SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation;
		[JsonProperty("DropPercentSellLittleReminderPriceDroppedClassicLongSituation")]
		public decimal DropPercentSellLittleReminderPriceDroppedClassicLongSituation;
		[JsonProperty("AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation")]
		public decimal AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation;

        // Classic Short
        [JsonProperty("SellCanceledClassicShortSituation")]
        public int SellCanceledClassicShortSituation;
        [JsonProperty("SellLittleFilledPriceDroppedClassicShortSituation")]
        public int SellLittleFilledPriceDroppedClassicShortSituation;
        [JsonProperty("SellLittleFilledCanceledClassicShortSituation")]
        public int SellLittleFilledCanceledClassicShortSituation;
        [JsonProperty("BuyCanceledLittleReminderClassicShortSituation")]
        public int BuyCanceledLittleReminderClassicShortSituation;
        [JsonProperty("BuyLittleReminderPriceIncreasedClassicShortSituation")]
        public int BuyLittleReminderPriceIncreasedClassicShortSituation;

        [JsonProperty("XOrdersAheadLittleFilledPriceDroppedClassicShortSituation")]
        public int XOrdersAheadLittleFilledPriceDroppedClassicShortSituation;
        [JsonProperty("SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation")]
        public int SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation;
        [JsonProperty("DropPercentLittleFilledPriceDroppedClassicShortSituation")]
        public decimal DropPercentLittleFilledPriceDroppedClassicShortSituation;
        [JsonProperty("AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation")]
        public decimal AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation;

        [JsonProperty("XOrdersAheadSellFilledEnoughPriceDropped")]
        public int XOrdersAheadSellFilledEnoughPriceDropped;
        [JsonProperty("SecondsAfterLastUpdateSellFilledEnoughPriceDropped")]
        public int SecondsAfterLastUpdateSellFilledEnoughPriceDropped;
        [JsonProperty("DropPercentSellFilledEnoughPriceDropped")]
        public decimal DropPercentSellFilledEnoughPriceDropped;
        [JsonProperty("AheadOrdersVolumeSellFilledEnoughPriceDropped")]
        public decimal AheadOrdersVolumeSellFilledEnoughPriceDropped;

        [JsonProperty("XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation")]
        public int XOrdersAheadSellLittleReminderPriceIncreasedClassicShortSituation;
        [JsonProperty("SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation")]
        public int SecondsAfterLastUpdateSellLittleReminderPriceIncreasedClassicShortSituation;
        [JsonProperty("DropPercentSellLittleReminderPriceIncreasedClassicShortSituation")]
        public decimal DropPercentSellLittleReminderPriceIncreasedClassicShortSituation;
        [JsonProperty("AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation")]
        public decimal AheadOrdersVolumeSellLittleReminderPriceIncreasedClassicShortSituation;

        // Данные
        [JsonProperty("GoogleLogin")]
        public string GoogleLogin;
        [JsonProperty("GooglePassword")]
        public string GooglePassword;

        #region StratumBox

        [JsonProperty("MyOrdersBox")]
        public bool MyOrdersBox;

        #endregion
    }
}
