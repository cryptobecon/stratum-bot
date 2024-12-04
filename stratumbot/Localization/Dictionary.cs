using stratumbot.Models;

namespace stratumbot
{
    public static class _
    {
        #region Links
        
        public static string BtnPlusLoginUrl // Личный кабинет
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://account.btn.plus/login?lang=ru&amp;utm_source=soft&amp;utm_medium=stratumbot&amp;utm_campaign=btnplus"; }
                return "https://account.btn.plus/login?lang=en&amp;utm_source=soft&amp;utm_medium=stratumbot&amp;utm_campaign=btnplus";
            }
        }
        public static string BtnPlusWikiUrl // Wiki
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/?utm_source=soft&utm_medium=stratumbot&utm_campaign=btnplus"; }
                return "https://docs.btn.plus/en/stratum-bot/?utm_source=soft&utm_medium=stratumbot&utm_campaign=btnplus";
            }
        }
        public static string TelegramChannelUrl // Telegram канал
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://t.me/btnplus"; }
                return "https://t.me/btn_plus";
            }
        }
        public static string ChangeLogUrl // Список изменений
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://btn.plus/public/changelog/stratum_ru.txt"; }
                return "https://btn.plus/public/changelog/stratum_en.txt";
            }
        }

        #endregion

        #region Интерфейс

        public static string SettingsButtonText // Настройки
        {
            get
            {
                if (Settings.Lang == "ru") { return "Настройки"; }
                return "Preferences";
            }
        }
        public static string HelpButton // Помощь
        {
            get
            {
                if (Settings.Lang == "ru") { return "Помощь"; }
                return "Help";
            }
        }
        public static string CabinetButton // Кабинет
        {
            get
            {
                if (Settings.Lang == "ru") { return "Кабинет"; }
                return "Account";
            }
        }

        public static string BabloStartButton // ЗАПУСТИТЬ
        {
            get
            {
                if (Settings.Lang == "ru") { return "ЗАПУСТИТЬ"; }
                return "RUN";
            }
        }
        public static string BabloStopButton // ОСТАНОВИТЬ
        {
            get
            {
                if (Settings.Lang == "ru") { return "ОСТАНОВИТЬ"; }
                return "STOP";
            }
        }
        public static string BabloSaveButton // СОХРАНИТЬ
        {
            get
            {
                if (Settings.Lang == "ru") { return "СОХРАНИТЬ"; }
                return "SAVE";
            }
        }
        public static string BabloAddButton // ДОБАВИТЬ
        {
            get
            {
                if (Settings.Lang == "ru") { return "ДОБАВИТЬ"; }
                return "ADD";
            }
        }
        public static string BabloButtonToolTip // Кнопка Бабло
        {
            get
            {
                if (Settings.Lang == "ru") { return "Кнопка «Бабло»"; }
                return "«Bablo» button";
            }
        }

        /// Рабочие потоки
        public static string StatusColumn // Статус
        {
            get
            {
                if (Settings.Lang == "ru") { return "Статус"; }
                return "Status";
            }
        }
        public static string Date // Дата
        {
            get
            {
                if (Settings.Lang == "ru") { return "Дата"; }
                return "Date";
            }
        }
        public static string Pair // Пара
        {
            get
            {
                if (Settings.Lang == "ru") { return "Пара"; }
                return "Pair";
            }
        }
        public static string Budget // Бюджет
        {
            get
            {
                if (Settings.Lang == "ru") { return "Бюджет"; }
                return "Budget";
            }
        }
        public static string IterationsColumn // Итерации
        {
            get
            {
                if (Settings.Lang == "ru") { return "Итерации"; }
                return "Iterations";
            }
        }
        public static string Profit // Профит
        {
            get
            {
                if (Settings.Lang == "ru") { return "Профит"; }
                return "Profit";
            }
        }
        public static string ProfitPercentColumn // Профит (%)
        {
            get
            {
                if (Settings.Lang == "ru") { return "Профит (%)"; }
                return "Profit (%)";
            }
        }
        public static string StopAfterSellThreadMenu
        {
            get
            {
                if (Settings.Lang == "ru") { return "Остановить по завершению"; }
                return "Stop after finish";
            }
        }
        public static string EditModeThreadMenu
        {
            get
            {
                if (Settings.Lang == "ru") { return "Изменить параметры"; }
                return "Edit";
            }
        }
        public static string SaveCollectionThreadMenu
        {
            get
            {
                if (Settings.Lang == "ru") { return "Сохранить коллекцию"; }
                return "Save collection";
            }
        }
        public static string LoadCollectionThreadMenu
        {
            get
            {
                if (Settings.Lang == "ru") { return "Загрузить коллекцию >"; }
                return "Load collection >";
            }
        }
        public static string TimeColumn // Время в логах
        {
            get
            {
                if (Settings.Lang == "ru") { return "Время"; }
                return "Time";
            }
        }
        public static string ActionColumn // Действие в логах
        {
            get
            {
                if (Settings.Lang == "ru") { return "Действие"; }
                return "Action";
            }
        }

        public static string Threads // Потоки
        {
            get
            {
                if (Settings.Lang == "ru") { return "Потоки"; }
                return "Threads";
            }
        }
        public static string Logs // Логи
        {
            get
            {
                if (Settings.Lang == "ru") { return "Логи"; }
                return "Logs";
            }
        }
        public static string MyOrders // Мои ордера
        {
            get
            {
                if (Settings.Lang == "ru") { return "Мои ордера"; }
                return "My orders";
            }
        }

        public static string Save
        {
            get
            {
                if (Settings.Lang == "ru") { return "Сохранить"; }
                return "Save";
            }
        }
        public static string Cancel
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отмена"; }
                return "Cancel";
            }
        }

        /// Настройки
        public static string SETTINGS // НАСТРОЙКИ
        {
            get
            {
                if (Settings.Lang == "ru") { return "НАСТРОЙКИ"; }
                return "PREFERENCES";
            }
        }
        public static string Generals // Общее
        {
            get
            {
                if (Settings.Lang == "ru") { return "Общее"; }
                return "General";
            }
        }
        public static string Timeouts // Таймауты
        {
            get
            {
                if (Settings.Lang == "ru") { return "Таймауты"; }
                return "Timeouts";
            }
        }
        public static string Exchanges // Биржи
        {
            get
            {
                if (Settings.Lang == "ru") { return "Биржи"; }
                return "Exchanges";
            }
        }
        public static string Algorithms // Алгоритмы
		{
            get
            {
                if (Settings.Lang == "ru") { return "Алогритмы"; }
                return "Algorithms";
            }
        }
        public static string License // Лицензия
        {
            get
            {
                if (Settings.Lang == "ru") { return "Лицензия"; }
                return "License";
            }
        }
        public static string AboutSoftware // О программе
        {
            get
            {
                if (Settings.Lang == "ru") { return "О программе"; }
                return "About";
            }
        }

        public static string ProMode //Профессиональный режим
        {
            get
            {
                if (Settings.Lang == "ru") { return "Профессиональный режим"; }
                return "Pro Mode";
            }
        }
        public static string LogsLenth //Количество записей в логах:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество записей в логах:"; }
                return "Logs length";
            }
        }
        public static string Language // Язык
        {
            get
            {
                if (Settings.Lang == "ru") { return "Язык:"; }
                return "Language:";
            }
        }
        public static string BabloVoice // Радостно подкидывать бабло
        {
            get
            {
                if (Settings.Lang == "ru") { return "Радостно подкидывать бабло"; }
                return "Voice";
            }
        }
        public static string DebugMode // Режим отладки
        {
            get
            {
                if (Settings.Lang == "ru") { return "Режим отладки"; }
                return "Debug Mode";
            }
        }

        public static string CheckTimeout // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "проверка рынка на возможность войти"; }
                return "market check timeout for trade opportunity";
            }
        }
        public static string CheckOrderTimeout // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "проверка ордеров на исполнение"; }
                return "order check timeout";
            }
        }
        public static string BetweenRequestTimeout // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "задержка перед некоторыми запросами"; }
                return "delay before some requests";
            }
        }

        public static string FiltersTimeout // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "проверка фильтров и индикаторов"; }
                return "check timeout for filters and indicators";
            }
        }

        public static string RecheckBuyFiltersTimeout // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "повторная проверка фильтров на покупку"; }
                return "re-check timeout for BUY filters";
            }
        }

        public static string RecheckSellFiltersTimeout // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "повторная проверка фильтров на продажу"; }
                return "re-check timeout for SELL filters";
            }
        }

        public static string Add // Добавить
        {
            get
            {
                if (Settings.Lang == "ru") { return "Добавить"; }
                return "Add";
            }
        }
        public static string Delete // Удалить
        {
            get
            {
                if (Settings.Lang == "ru") { return "Удалить"; }
                return "Delete";
            }
        }

        public static string Scalping // Скальпинг
        {
            get
            {
                if (Settings.Lang == "ru") { return "Scalping"; }
                return "Scalping";
            }
        }

        public static string ClassicLong // Classic Long
        {
            get
            {
                if (Settings.Lang == "ru") { return "Classic Long "; }
                return "Classic Long ";
            }
        }

        public static string Autofit // Автоподбор параметров
        {
            get
            {
                if (Settings.Lang == "ru") { return "Автоподбор параметров"; }
                return "Autofit";
            }
        }

        public static string PercentZeroSell // % продажи в ноль (комиссия биржи x2)
        {
            get
            {
                if (Settings.Lang == "ru") { return "% продажи в ноль (комиссия биржи x2)"; }
                return "% zero sell (exchange's fee x2)";
            }
        }
        public static string PercentMinSpread // % Мин. спред
        {
            get
            {
                if (Settings.Lang == "ru") { return "% Мин. спред"; }
                return "% Min. spread";
            }
        }
        public static string PercentOptSpread // % Опт. спред
        {
            get
            {
                if (Settings.Lang == "ru") { return "% Опт. спред"; }
                return "% Opt. spread";
            }
        }
        public static string PercentMinMarkup // % Мин. наценка
        {
            get
            {
                if (Settings.Lang == "ru") { return "% Мин. наценка"; }
                return "% Min. markup";
            }
        }
        public static string PercentOptMarkup // % Опт. наценка
        {
            get
            {
                if (Settings.Lang == "ru") { return "% Опт. наценка"; }
                return "% Opt. markup";
            }
        }
        public static string InPercent // В процентах
        {
            get
            {
                if (Settings.Lang == "ru") { return "В процентах"; }
                return "In percent";
            }
        }
        public static string InPoint // В пунктах
        {
            get
            {
                if (Settings.Lang == "ru") { return "В пунктах"; }
                return "In point";
            }
        }
        public static string InTimeout // Время ожидания
        {
            get
            {
                if (Settings.Lang == "ru") { return "Время ожидания"; }
                return "Timeout";
            }
        }
        public static string DCAPercentProfit // % профита
        {
            get
            {
                if (Settings.Lang == "ru") { return "% профита"; }
                return "% profit";
            }
        }
        public static string DCAStepss //  шагов
        {
            get
            {
                if (Settings.Lang == "ru") { return " шагов"; }
                return " steps";
            }
        }
        public static string DCADropPercent // Процента падения:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процента падения:"; }
                return "Percentage drop:";
            }
        }
        public static string DCAAdditBuy // Процента докупки:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процента докупки:"; }
                return "Percentage buy:";
            }
        }
        public static string DCAPercentApproximation // % Приближения
        {
            get
            {
                if (Settings.Lang == "ru") { return "% Приближения"; }
                return "% Approximation";
            }
        }
        public static string StopAfterXStopLoss // Остановка после X стоплоссов
        {
            get
            {
                if (Settings.Lang == "ru") { return "Остановка после X стоплоссов"; }
                return "Stop after X stoplosses";
            }
        }
        public static string StopLossTimeout // StopLoss Timeout
        {
            get
            {
                if (Settings.Lang == "ru") { return "StopLoss Timeout"; }
                return "StopLoss Timeout";
            }
        }

        public static string CustomEventsHeader
        {
            get
            {
                if (Settings.Lang == "ru") { return "Настраеваемые события"; }
                return "Custom events";
            }
        }

        public static string BuyFilledEnoughPriceIncreasedHeader
        {
            get
            {
                if (Settings.Lang == "ru") { return "Buy ордер исполнен достаточно, цена ушла вверх"; }
                return "BUY order is filled enough, the price increased";
            }
        }
        public static string SellFilledEnoughPriceDecreasedHeader
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL-ордер исполнен достаточно, цена ушла вниз"; }
                return "SELL order is filled enough, the price decreased";
            }
        }

        public static string DontShareYourDevice // (никому не сообщайте ваш ключ)
        {
            get
            {
                if (Settings.Lang == "ru") { return "(никому не сообщайте ваш ключ)"; }
                return "(don't show your key to anyone)";
            }
        }
        public static string GoToMyCabonet // Перейти в мой кабинет
        {
            get
            {
                if (Settings.Lang == "ru") { return "Перейти в мой кабинет"; }
                return "Go to my account";
            }
        }
        public static string LicenseStatus // Статус лицензии:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Статус лицензии:"; }
                return "License status:";
            }
        }
        public static string LicenseKey // Лицензия (ключ)
        {
            get
            {
                if (Settings.Lang == "ru") { return "Лицензия (ключ)"; }
                return "License (key)";
            }
        }
        public static string ActivtionButton
        {
            get
            {
                if (Settings.Lang == "ru") { return "Активировать"; }
                return "Activate";
            }
        }
        public static string RenewButton
        {
            get
            {
                if (Settings.Lang == "ru") { return "Продлить"; }
                return "Renew";
            }
        }
        public static string Copyright
        {
            get
            {
                if (Settings.Lang == "ru") { return "Разработчик: www.btn.plus"; }
                return "Dev by www.btn.plus";
            }
        }

		public static string Data // Данные
		{
			get
			{
				if (Settings.Lang == "ru") { return "Данные"; }
				return "Data";
			}
		}

		public static string EmailDataCaption // Данные для использования фильтра Email Notify
		{
			get
			{
				if (Settings.Lang == "ru") { return "Данные для использования фильтра Email Notify"; }
				return "Data for the Email Notify filter";
			}
		}

		public static string RECOVERING // ВОССТАНОВЛЕНИЕ
        {
            get
            {
                if (Settings.Lang == "ru") { return "ВОССТАНОВЛЕНИЕ"; }
                return "RECOVERING";
            }
        }
        public static string ChoiceThread // Выберите потоки для восстановления:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Выберите потоки для восстановления:"; }
                return "Select the thread for recovery";
            }
        }
        public static string Step // Шаг
        {
            get
            {
                if (Settings.Lang == "ru") { return "Шаг"; }
                return "Step";
            }
        }
        public static string DateCreated // Дата создания
        {
            get
            {
                if (Settings.Lang == "ru") { return "Дата создания"; }
                return "Date of creation";
            }
        }

        public static string Recovery // Восстановить
        {
            get
            {
                if (Settings.Lang == "ru") { return "Восстановить"; }
                return "Recovery";
            }
        }
        public static string UPDATE // ОБНОВЛЕНИЕ
        {
            get
            {
                if (Settings.Lang == "ru") { return "ОБНОВЛЕНИЕ"; }
                return "UPDATE";
            }
        }
        public static string Update // Обновить
        {
            get
            {
                if (Settings.Lang == "ru") { return "Обновить"; }
                return "Update";
            }
        }
        public static string Changelog // Список изменений:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Список изменений:"; }
                return "Changelog:";
            }
        }

        public static string Strategy // Стратегия
        {
            get
            {
                if (Settings.Lang == "ru") { return "Стратегия"; }
                return "Strategy";
            }
        }

        public static string Side // Тип
        {
            get
            {
                if (Settings.Lang == "ru") { return "Тип"; }
                return "Side";
            }
        }
        public static string Price // Цена
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цена"; }
                return "Price";
            }
        }
        public static string Amount // Количество
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество"; }
                return "Amount";
            }
        }
        public static string Filled // Исполнено
        {
            get
            {
                if (Settings.Lang == "ru") { return "Исполнено"; }
                return "Filled";
            }
        }

        /// Скальпинг Вью
        public static string CurrencyPair // Валютная пара
        {
            get
            {
                if (Settings.Lang == "ru") { return "Валютная пара"; }
                return "Pair";
            }
        }
        public static string CurrencyPairToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Валютная пара для торговли"; }
                return "Pair for trading";
            }
        }

        public static string AutofitToolTip // Подобрать параметры автоматически
        {
            get
            {
                if (Settings.Lang == "ru") { return "Подобрать параметры автоматически"; }
                return "Get params automatically";
            }
        }

        public static string BudgetToolTip // Количество монет валюты #2, на которое будут приобретены монеты валюты #1
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество монет валюты #2, на которое будут приобретены монеты валюты #1"; }
                return "The number of currency coins # 2, which will be used to purchase currency coins # 1";
            }
        }
        public static string BudgetShortToolTip // Количество монет валюты #1, которое будут проданы за монеты валюты #2
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество монет валюты #1, которое будут проданы за монеты валюты #2"; }
                return "The number of currency coins #1, which will be sold for currency coins #2";
            }
        }
        public static string Buying // Покупка
        {
            get
            {
                if (Settings.Lang == "ru") { return "Покупка"; }
                return "BUY";
            }
        }
        public static string MinSpread // Мин. спред
        {
            get
            {
                if (Settings.Lang == "ru") { return "Мин. спред"; }
                return "Min. spread";
            }
        }
        public static string OptSpread // Опт. спред
        {
            get
            {
                if (Settings.Lang == "ru") { return "Опт. спред"; }
                return "Opt. spread";
            }
        }
        public static string MinSpreadToolTip 
        {
            get
            {
                if (Settings.Lang == "ru") { return "Минимальная разность между стаканами, при которой бот войдет в торговлю. В этом случае бот выставит свой ордер по цене первого ордера в стакане"; }
                return "The minimum difference between the market depth at which the bot will start to trade. In this case, the bot will place order at the price of the first order in the market depth";
            }
        }
        public static string OptSpreadToolTip 
        {
            get
            {
                if (Settings.Lang == "ru") { return "Оптимальная разница между крайними ордерами в стаканах, при которой бот войдёт в торговлю. В этом случае бот выставит свой ордер по цене на 0,00000001 выгоднее крайнего ордера в стакане"; }
                return "The optimal difference between the market depth, at which the bot will start to trade. In this case, the bot will place order at a price 0.00000001 more than the last order in the market depth";
            }
        }
        public static string InTimeoutToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество секунд, которые бот ожидает после того, как обнаружил, что разница между крайними ордерами в стаканах позволяет скальпить"; }
                return "The number of seconds that the bot expects after it discovered that the difference between the extreme orders in the market depth allows  to scalp";
            }
        }
        public static string Selling // Продажа
        {
            get
            {
                if (Settings.Lang == "ru") { return "Продажа"; }
                return "SELL";
            }
        }
        public static string MinMarkup // Мин. наценка
        {
            get
            {
                if (Settings.Lang == "ru") { return "Мин. наценка"; }
                return "Min. markup";
            }
        }
        public static string OptMarkup // Опт. наценка
        {
            get
            {
                if (Settings.Lang == "ru") { return "Опт. наценка"; }
                return "Opt. markup";
            }
        }
        public static string ZeroSell // Продажа в ноль
        {
            get
            {
                if (Settings.Lang == "ru") { return "Продажа в ноль"; }
                return "ZeroSell";
            }
        }
        public static string MinMarkupToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Минимальная наценка, которую бот будет выставлять если выше нашего ордера есть не больше X ордеров"; }
                return "The minimum markup that the bot will set if there are no more than X orders above our order";
            }
        }
        public static string OptMarkupToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Наценка, на случай если мы можем встать первыми"; }
                return "Markup, in case we can get up first";
            }
        }
        public static string ZeroSellToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Наценка меньше которой не продавать. Обычно тут учитывается комиссия биржи, чтобы продавать в ноль. Значение может быть отрицательным"; }
                return "Minimal markup. Usually, the exchange commission is taken into account to sell to zero. Value may be negative";
            }
        }
        public static string DCAStepsCount // Количество шагов
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество шагов"; }
                return "Number of steps";
            }
        }
        public static string DCAProfitToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процент профита при срабатывании DCA"; }
                return "DCA Profit Percentage";
            }
        }
        public static string DCAStepsCountToolTip 
        {
            get
            {
                if (Settings.Lang == "ru") { return "Количество возможных страховочных BUY ордеров"; }
                return "The number of possible BUY safety orders";
            }
        }
        public static string DCAProfitPercentToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процент профита"; }
                return "Profit percent";
            }
        }
        public static string DCAGridSizeToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Размер сетки"; }
                return "Grid size";
            }
        }
        public static string PriceDrop // Падение курса
        {
            get
            {
                if (Settings.Lang == "ru") { return "Падение курса"; }
                return "Price drop";
            }
        }
        public static string AdditionalBuyAmount // Объём покупки
        {
            get
            {
                if (Settings.Lang == "ru") { return "Объём покупки"; }
                return "Amount";
            }
        }
        public static string PriceDropToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Если курс упадёт на указанное количество процентов будет совершена докупка на указанный объём"; }
                return "If the price drops by the specified percentage, a purchase will be made for the specified amount";
            }
        }
        public static string AdditionalBuyAmountToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Объём на который нужно докупать на определенном шаге"; }
                return "The amount for which you need to buy at a certain step";
            }
        }

        public static string StopLoss // Стоплосс (ex Быстрая продажа)
        {
            get
            {
                if (Settings.Lang == "ru") { return "Стоплосс"; }
                return "StopLoss";
            }
        }
        public static string StopLossToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Стоплосс"; }
                return "StopLoss";
            }
        }
        public static string StopLossTextBoxToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цена на монету или % от цены на покупку (обязательно со знаком процента)"; }
                return "Coin price or % of the purchase price (required with a percent sign)";
            }
        }
        public static string IgnoreOrdersLess // Игнорировать ордера меньше
        {
            get
            {
                if (Settings.Lang == "ru") { return "Игнорировать ордера меньше"; }
                return "Ignore orders num less";
            }
        }
        public static string IgnoreOrdersPercentToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процентов"; }
                return "Percent";
            }
        }
        public static string IgnoreOrdersCountToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Штук"; }
                return "Number of ignore orders (pieces)";
            }
        }
        public static string SellStart // Начать с продажи
        {
            get
            {
                if (Settings.Lang == "ru") { return "Начать с продажи"; }
                return "Start for sell";
            }
        }
        public static string SellStartToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Начать первую итерацию с выставления SELL ордера"; }
                return "Start the first iteration by placing a SELL order";
            }
        }
        public static string SellStartPriceToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цена по которой был исполнен BUY ордер"; }
                return "The price at which the BUY order was executed";
            }
        }
        public static string SellStartAmountToolTip
        {
            get
            {
                if (Settings.Lang == "ru") { return "Объем монет на который был исполнен BUY ордер"; }
                return "The volume of coins for which the BUY order was executed";
            }
        }

        public static string Iterations // Итераций:_
        {
            get
            {
                if (Settings.Lang == "ru") { return "Итераций: "; }
                return "Iterations: ";
            }
        }

        public static string Profits // Заработали:_
        {
            get
            {
                if (Settings.Lang == "ru") { return "Заработали: "; }
                return "Profit: ";
            }
        }


        public static string Attention // Внимание!
        {
            get
            {
                if (Settings.Lang == "ru") { return "Внимание!"; }
                return "Attention!";
            }
        }
        public static string Error // Ошибка!
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ошибка!"; }
                return "Error!";
            }
        }
        public static string Accepted // Принято!
        {
            get
            {
                if (Settings.Lang == "ru") { return "Принято!"; }
                return "Accepted!";
            }
        }

        public static string Empty // (пусто)
        {
            get
            {
                if (Settings.Lang == "ru") { return "(пусто)"; }
                return "(empty)";
            }
        }

        // Classic Long/Short

        public static string MinProfit // Мин.профит
        {
            get
            {
                if (Settings.Lang == "ru") { return "Мин.профит"; }
                return "Min.profit";
            }
        }

        public static string TrailingProfit // Трейлинг профит
        {
            get
            {
                if (Settings.Lang == "ru") { return "Трейлинг профит"; }
                return "Trailing profit";
            }
        }

        public static string TrailPercent // Процент трейла
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процент трейла"; }
                return "Trail percent";
            }
        }

        public static string Approximation // Приближение
        {
            get
            {
                if (Settings.Lang == "ru") { return "Приближение"; }
                return "Approximation";
            }
        }

        public static string UnApproximation // Отдаление
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отдаление"; }
                return "Estrangement";
            }
        }

        public static string BuyByMarket // Покупка по рынку
        {
            get
            {
                if (Settings.Lang == "ru") { return "Покупка по рынку"; }
                return "Buy by market";
            }
        }

        public static string SellByMarket // Продажа по рынку
        {
            get
            {
                if (Settings.Lang == "ru") { return "Продажа по рынку"; }
                return "Sell by market";
            }
        }

        public static string Filters // Фильтры
        {
            get
            {
                if (Settings.Lang == "ru") { return "Фильтры"; }
                return "Filters";
            }
        }


        // ФИЛЬТРЫ И ИНДИКАТОРЫ

        public static string FiltersAndIndicatprs // ФИЛЬТРЫ & ИНДИКАТОРЫ
        {
            get
            {
                if (Settings.Lang == "ru") { return "ФИЛЬТРЫ & ИНДИКАТОРЫ"; }
                return "FILTERS & INDOCATORS";
            }
        }

        public static string Limits // Limits
        {
            get
            {
                if (Settings.Lang == "ru") { return "Лимиты"; }
                return "Limits";
            }
        }

        public static string Oscillators // Осцилляторы
        {
            get
            {
                if (Settings.Lang == "ru") { return "Осцилляторы"; }
                return "Oscillators";
            }
        }

        public static string Volume // Объём
        {
            get
            {
                if (Settings.Lang == "ru") { return "Объём"; }
                return "Volume";
            }
        }

        public static string General // Общее
        {
            get
            {
                if (Settings.Lang == "ru") { return "Общее"; }
                return "General";
            }
        }


        public static string External // Внешнее
        {
            get
            {
                if (Settings.Lang == "ru") { return "Внешнее"; }
                return "External";
            }
        }

        public static string Mode // Режим:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Режим:"; }
                return "Mode:";
            }
        }

        public static string Coefficient // Коэффициент:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Коэффициент:"; }
                return "Coefficient:";
            }
        }

        public static string Period // Period:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период:"; }
                return "Period:";
            }
        }
        
        public static string DCAManager
        {
            get
            {
                if (Settings.Lang == "ru") { return "DCA МЕНЕДЖЕР"; }
                return "DCA MANAGER";
            }
        }

        public static string StopLossValueLabel
        {
            get
            {
                if (Settings.Lang == "ru") { return "Значение"; }
                return "Value";
            }
        }

        public static string StopLossApproximationLabel
        {
            get
            {
                if (Settings.Lang == "ru") { return "Приближение"; }
                return "Approximation";
            }
        }

        public static string StopLossFiltersLabel
        {
            get
            {
                if (Settings.Lang == "ru") { return "Фильтры"; }
                return "Filters";
            }
        }

        public static string Value
        {
            get
            {
                if (Settings.Lang == "ru") { return "Значение:"; }
                return "Value:";
            }
        }

        public static string ValuePercent
        {
            get
            {
                if (Settings.Lang == "ru") { return "Значение (%):"; }
                return "Value (%):";
            }
        }

        public static string FilterSide
        {
            get
            {
                if (Settings.Lang == "ru") { return "Сторона:"; }
                return "Side:";
            }
        }

        public static string ModeMore
        {
            get
            {
                if (Settings.Lang == "ru") { return "Больше"; }
                return "More";
            }
        }

        public static string ModeLess
        {
            get
            {
                if (Settings.Lang == "ru") { return "Меньше"; }
                return "Less";
            }
        }

        public static string FilterPair
        {
            get
            {
                if (Settings.Lang == "ru") { return "Пара:"; }
                return "Pair:";
            }
        }

        public static string Period1
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период 1:"; }
                return "Period 1:";
            }
        }

        public static string Period2
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период 2:"; }
                return "Period 2:";
            }
        }

        public static string PeriodATR
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период ATR:"; }
                return "Period ATR:";
            }
        }

        public static string Percent
        {
            get
            {
                if (Settings.Lang == "ru") { return "Процент:"; }
                return "Percent:";
            }
        }

        public static string Source
        {
            get
            {
                if (Settings.Lang == "ru") { return "Источник:"; }
                return "Source:";
            }
        }
        public static string PriceType
        {
            get
            {
                if (Settings.Lang == "ru") { return "Тип цены:"; }
                return "Price type:";
            }
        }
        public static string Spread
        {
            get
            {
                if (Settings.Lang == "ru") { return "Спред:"; }
                return "Spread:";
            }
        }

        public static string PeriodEMA
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период EMA:"; }
                return "Period EMA:";
            }
        }
        public static string PeriodK
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период %K:"; }
                return "Period %K:";
            }
        }
        public static string PeriodD
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период %D:"; }
                return "Period %D:";
            }
        }
        public static string PeriodSmooth
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период сглаживания:"; }
                return "Period Smooth:";
            }
        }
        public static string TimerOpt1
        {
            get
            {
                if (Settings.Lang == "ru") { return "Начало итерации"; }
                return "Iteration start";
            }
        }
        public static string TimerOpt2
        {
            get
            {
                if (Settings.Lang == "ru") { return "Первый BUY ордер"; }
                return "First BUY order";
            }
        }
        public static string TimerOpt3
        {
            get
            {
                if (Settings.Lang == "ru") { return "Последний BUY ордер"; }
                return "Last BUY order";
            }
        }
        public static string TimerOpt4
        {
            get
            {
                if (Settings.Lang == "ru") { return "Первый SELL ордер"; }
                return "First SELL order";
            }
        }
        public static string TimerOpt5
        {
            get
            {
                if (Settings.Lang == "ru") { return "Последний SELL ордер"; }
                return "Last SELL order";
            }
        }

        public static string PeriodSMA
        {
            get
            {
                if (Settings.Lang == "ru") { return "Период SMA:"; }
                return "Period SMA:";
            }
        }
        public static string ValidatySec
        {
            get
            {
                if (Settings.Lang == "ru") { return "Действительность (сек):"; }
                return "Validaty (sec):";
            }
        }

        public static string Indentation // Отступ:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отступ:"; }
                return "Indentation:";
            }
        }

        public static string PriceSource // Источник цены:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Источник цены:"; }
                return "Price source:";
            }
        }

        public static string Duration // Длительность:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Длительность:"; }
                return "Duration:";
            }
        }

        public static string Group // Группа:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Группа:"; }
                return "Group:";
            }
        }

        public static string Title // Название:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Название:"; }
                return "Title:";
            }
        }
        public static string Color // Цвет:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цвет:"; }
                return "Color:";
            }
        }

        public static string Weight // Вес:
        {
            get
            {
                if (Settings.Lang == "ru") { return "Вес:"; }
                return "Weight:";
            }
        }



		public static string Action // Действие
		{
			get
			{
				if (Settings.Lang == "ru") { return "Действие"; }
				return "Action";
			}
		}

		public static string Description // Описание
		{
			get
			{
				if (Settings.Lang == "ru") { return "Описание"; }
				return "Description";
			}
		}

        public static string BuyOrderWasCanceled
        {
            get
            {
                if (Settings.Lang == "ru") { return "BUY ордер - был отменён"; }
                return "BUY order - was canceled";
            }
        }
        public static string BuyOrderSmallIncreased
        {
            get
            {
                if (Settings.Lang == "ru") { return "BUY ордер - исполнился на маленький объём - цена ушла вверх"; }
                return "BUY order - executed on a small volume - the price increased";
            }
        }
        public static string BuyOrderSmallWasCanceled
        {
            get
            {
                if (Settings.Lang == "ru") { return "BUY ордер - исполнился на маленький объём - был отменён"; }
                return "BUY order - executed on a small volume - was canceled";
            }
        }
        
        public static string SellOrderWasCanceledSmallLeft
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL ордер - был отменён - остался маленький объём"; }
                return "SELL order - was canceled - a small volume left";
            }
        }
        public static string SellPriceDecreasedSmallLeft
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL ордер - цена ушла вниз - остался маленький объём"; }
                return "SELL order - the price decreased - a small volume left";
            }
        }
        public static string SellWasCanceled
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL ордер - был отменён"; }
                return "SELL order - was canceled";
            }
        }
        public static string SellSmallFilledDecreased
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL ордер - исполнился на маленький объём - цена ушла вниз"; }
                return "SELL order - executed on a small volume - the price decreased";
            }
        }
        public static string SellSmallFilledWasCanceled
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL ордер - исполнился на маленький объём - был отменён"; }
                return "SELL order - executed on a small volume - was canceled";
            }
        }
        public static string BuyWasCanceledSmallLeft
        {
            get
            {
                if (Settings.Lang == "ru") { return "BUY ордер - был отменён - остался маленький объём"; }
                return "BUY order - was canceled - a small volume left";
            }
        }
        public static string BuyPriceDecreasedSmallLeft
        {
            get
            {
                if (Settings.Lang == "ru") { return "BUY ордер - цена ушла вверх - остался маленький объём"; }
                return "BUY order - the price increased - a small volume left";
            }
        }


        public static string StopActionSituation // Остановиться
		{
			get
			{
				if (Settings.Lang == "ru") { return "1. Остановиться"; }
				return "1. Stop";
			}
		}

		public static string ReminderBudgetOrStopActionSituation // 2
		{
			get
			{
				if (Settings.Lang == "ru") { return "2. Начать новую итерацию с оставшимся бюджетом (исполненную часть продать позже) ИЛИ Остановиться"; }
				return "2. Start a new iteration with the reminder budget (filled part sell later) OR Stop";
			}
		}

		public static string FullBudgetOrStopActionSituation // 3
		{
			get
			{
				if (Settings.Lang == "ru") { return "3. Начать новую итерацию с полным бюджетом (исполненную часть продать позже) ИЛИ Остановиться"; }
				return "3. Start a new iteration with the full budget (filled part sell later) OR Stop";
			}
		}

		public static string ReminderBudgetOrFullBudgetOrStopActionSituation // 4
		{
			get
			{
				if (Settings.Lang == "ru") { return "4. Начать новую итерацию с оставшимся бюджетом (исполненную часть продать позже) ИЛИ Начать новую итерацию с полным бюджетом (исполненную часть продать позже) ИЛИ Остановиться"; }
				return "4. Start a new iteration with the reminder budget (filled part sell later) OR Start a new iteration with the full budget (filled part sell later) OR Stop";
			}
		}

		public static string FullBudgetOrReminderBudgetOrStopActionSituation // 4
		{
			get
			{
				if (Settings.Lang == "ru") { return "5.  Начать новую итерацию с полным бюджетом (исполненную часть продать позже) ИЛИ Начать новую итерацию с оставшимся бюджетом (исполненную часть продать позже) ИЛИ Остановиться"; }
					return "5. Start a new iteration with the full budget (reminder part sell later) OR Start a new iteration with the reminder budget (reminder part sell later) OR Stop";
			}
		}

		public static string FullBudgetAndForgetOrStopActionSituation // 4
		{
			get
			{
				if (Settings.Lang == "ru") { return "6.  Начать новую итерацию с полным бюджетом (исполненную часть забыть) ИЛИ Остановиться"; }
				return "6. Start a new iteration with the full budget (forget about reminder part) OR Stop";
			}
		}


		public static string WaitActionSituation // Ждать
		{
			get
			{
				if (Settings.Lang == "ru") { return "1. Ждать"; }
				return "1. Wait";
			}
		}

		public static string ReminderBudgetOrWaitActionSituation // 2
		{
			get
			{
				if (Settings.Lang == "ru") { return "2. Начать новую итерацию с оставшимся бюджетом (исполненную часть продать позже), отменить текущий ордер ИЛИ Ждать"; }
				return "2. Start a new iteration with the reminder budget (filled part sell later), cancel current one OR Wait";
			}
		}

		public static string FullBudgetOrWaitActionSituation // 3
		{
			get
			{
				if (Settings.Lang == "ru") { return "3. Начать новую итерацию с полным бюджетом (исполненную продать позже), отменить текущий ордер ИЛИ Ждать"; }
				return "3. Start a new iteration with the full budget (filled part sell later), cancel current one OR Wait";
			}
		}

		public static string ReminderBudgetOrFullBudgetOrWaitActionSituation // 4
		{
			get
			{
				if (Settings.Lang == "ru") { return "4. Начать новую итерацию с оставшимся бюджетом (исполненную часть продать позже), отменить текущий ордер ИЛИ Начать новую итерацию с полным бюджетом (исполненную часть продать позже), отменить текущий ордер ИЛИ Ждать"; }
				return "4. Start a new iteration with the reminder budget (filled part sell later), cancel current one OR Start a new iteration with the full budget (filled part sell later), cancel current one OR Wait";
			}
		}

		public static string FullBudgetOrReminderBudgetOrWaitActionSituation // 4
		{
			get
			{
				if (Settings.Lang == "ru") { return "5.  Начать новую итерацию с полным бюджетом (исполненную продать позже), отменить текущий ордер ИЛИ Начать новую итерацию с оставшимся бюджетом (исполненную часть продать позже), отменить текущий ордер ИЛИ Ждать"; }
					return "5. Start a new iteration with the full budget (filled part sell later), cancel current one OR Start a new iteration with the reminder budget (filled part sell later), cancel current one OR Wait";
			}
		}

		public static string FullBudgetAndForgetOrWaitActionSituation // 4
		{
			get
			{
				if (Settings.Lang == "ru") { return "6. Начать новую итерацию с полным бюджетом (исполненную часть забыть), отменить текущий ордер ИЛИ Ждать"; }
				return "6. Start a new iteration with the full budget (forget about reminder part) OR Wait";
			}
		}

		public static string BuyLittleFilledPriceIncreasedClassicLongSituation // BUY ордер - исполнился на маленький объём - цена ушла вверх
		{
            get
            {
                if (Settings.Lang == "ru") { return "BUY ордер - исполнился на маленький объём - цена ушла вверх"; }
                return "BUY order — executed on a small volume — the price increased";
            }
        }

		public static string DropBuyLittleFilledPriceIncreasedClassicLongSituation // Бросить ожидание: BUY ордер - исполнился на маленький объём - цена ушла вверх
		{
			get
			{
				if (Settings.Lang == "ru") { return "Действия #2, #3 и #4 могут быть выполнены если"; }
				return "Additional conditions can be set for actions #2, #3 and #4:";
			}
		}

		public static string XOrdersAheadDropSituation_1 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "В стакане"; }
				return "There are";
			}
		}

		public static string XOrdersAheadDropSituation_2 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "ордеров перед моим ордером"; }
				return "orders before mine";
			}
		}

		public static string SecondsAfterLastUpdateDropSituation_1 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "Прошло"; }
				return "";
			}
		}

		public static string SecondsAfterLastUpdateDropSituation_2 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "секунд после изменения ордера"; }
				return "seconds have passed since the order was changed";
			}
		}

        public static string DropPercentIncreasedSituation_1 //  увелчилась
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цена увеличилась на"; }
                return "Price increased by";
            }
        }

        public static string DropPercentDropSituation_1 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "Цена упала на"; }
				return "Price decreased by";
			}
		}

		public static string DropPercentDropSituation_2 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "% от цены покупки"; }
				return "% of the buy price";
			}
		}

		public static string AheadOrdersVolumeDropSituation_1 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "Перед моим ордером стоят другие объемом в"; }
				return "Before my order, there are others with a volume of";
			}
		}

		public static string AheadOrdersVolumeDropSituation_2 // 
		{
			get
			{
				if (Settings.Lang == "ru") { return "% от моего"; }
				return "% of mine";
			}
		}


		public static string SellLittleReminderPriceDroppedClassicLongSituationAction1 // Остановиться
		{
			get
			{
				if (Settings.Lang == "ru") { return "1. Остановиться"; }
				return "1. Stop";
			}
		}

		public static string SellLittleReminderPriceDroppedClassicLongSituation
		{
			get
			{
				if (Settings.Lang == "ru") { return "Действия #2, #3 #4, #5 и #6 могут быть выполнены если"; }
				return "Additional conditions can be set for actions #2, #3, #4, #5 and #6:";
			}
		}

        // Short Situations and etc

        public static string ReminderBudgetOrStopActionShortSituation // 2
        {
            get
            {
                if (Settings.Lang == "ru") { return "2. Начать новую итерацию с оставшимся бюджетом (исполненную часть купить позже) ИЛИ Остановиться"; }
                return "2. Start a new iteration with the reminder budget (filled part buy later) OR Stop";
            }
        }

        public static string FullBudgetOrStopActionShortSituation // 3
        {
            get
            {
                if (Settings.Lang == "ru") { return "3. Начать новую итерацию с полным бюджетом (исполненную часть купить позже) ИЛИ Остановиться"; }
                return "3. Start a new iteration with the full budget (filled part buy later) OR Stop";
            }
        }

        public static string ReminderBudgetOrFullBudgetOrStopActionShortSituation // 4
        {
            get
            {
                if (Settings.Lang == "ru") { return "4. Начать новую итерацию с оставшимся бюджетом (исполненную часть купить позже) ИЛИ Начать новую итерацию с полным бюджетом (исполненную часть купить позже) ИЛИ Остановиться"; }
                return "4. Start a new iteration with the reminder budget (filled part buy later) OR Start a new iteration with the full budget (filled part buy later) OR Stop";
            }
        }

        public static string ReminderBudgetOrWaitActionShortSituation // 2
        {
            get
            {
                if (Settings.Lang == "ru") { return "2. Начать новую итерацию с оставшимся бюджетом (исполненную часть купить позже), отменить текущий ордер ИЛИ Ждать"; }
                return "2. Start a new iteration with the reminder budget (filled part buy later), cancel current one OR Wait";
            }
        }

        public static string FullBudgetOrWaitActionShortSituation // 3
        {
            get
            {
                if (Settings.Lang == "ru") { return "3. Начать новую итерацию с полным бюджетом (исполненную купить позже), отменить текущий ордер ИЛИ Ждать"; }
                return "3. Start a new iteration with the full budget (filled part buy later), cancel current one OR Wait";
            }
        }

        public static string ReminderBudgetOrFullBudgetOrWaitActionShortSituation // 4
        {
            get
            {
                if (Settings.Lang == "ru") { return "4. Начать новую итерацию с оставшимся бюджетом (исполненную часть купить позже), отменить текущий ордер ИЛИ Начать новую итерацию с полным бюджетом (исполненную часть купить позже), отменить текущий ордер ИЛИ Ждать"; }
                return "4. Start a new iteration with the reminder budget (filled part buy later), cancel current one OR Start a new iteration with the full budget (filled part buy later), cancel current one OR Wait";
            }
        }

        public static string FullBudgetOrReminderBudgetOrWaitActionShortSituation // 4
        {
            get
            {
                if (Settings.Lang == "ru") { return "5.  Начать новую итерацию с полным бюджетом (исполненную купить позже), отменить текущий ордер ИЛИ Начать новую итерацию с оставшимся бюджетом (исполненную часть купить позже), отменить текущий ордер ИЛИ Ждать"; }
                return "5. Start a new iteration with the full budget (filled part buy later), cancel current one OR Start a new iteration with the reminder budget (filled part buy later), cancel current one OR Wait";
            }
        }

        public static string DropBuyLittleFilledPriceIncreasedClassicShortSituation // 
        {
            get
            {
                if (Settings.Lang == "ru") { return "Действия #2, #3 и #4 могут быть выполнены если"; }
                return "Additional conditions can be set for actions #2, #3 and #4:";
            }
        }

        public static string FullBudgetOrReminderBudgetOrStopActionShortSituation // 4
        {
            get
            {
                if (Settings.Lang == "ru") { return "5.  Начать новую итерацию с полным бюджетом (исполненную часть купить позже) ИЛИ Начать новую итерацию с оставшимся бюджетом (исполненную часть купить позже) ИЛИ Остановиться"; }
                return "5. Start a new iteration with the full budget (reminder part buy later) OR Start a new iteration with the reminder budget (reminder part buy later) OR Stop";
            }
        }

        public static string FullBudgetAndForgetOrStopActionShortSituation // 4
        {
            get
            {
                if (Settings.Lang == "ru") { return "6.  Начать новую итерацию с полным бюджетом (исполненную часть забыть) ИЛИ Остановиться"; }
                return "6. Start a new iteration with the full budget (forget about reminder part) OR Stop";
            }
        }

        public static string SellLittleReminderPriceDroppedClassicShortSituation
        {
            get
            {
                if (Settings.Lang == "ru") { return "Действия #2, #3 #4, #5 и #6 могут быть выполнены если"; }
                return "Additional conditions can be set for actions #2, #3, #4, #5 and #6:";
            }
        }

        public static string FullBudgetAndForgetOrWaitActionShortSituation // 4
        {
            get
            {
                if (Settings.Lang == "ru") { return "6. Начать новую итерацию с полным бюджетом (исполненную часть забыть), отменить текущий ордер ИЛИ Ждать"; }
                return "6. Start a new iteration with the full budget (forget about reminder part) OR Wait";
            }
        }

        #endregion



        public static string Loooooot
        {
            get
            {
                if (Settings.Lang == "ru") { return "Готово! Считаем бабло, начинаем заново"; }
                return "Done! Starting a new iteration...";
            }
        }

        public static string Log1 // Старт!
        {
            get
            {
                if (Settings.Lang == "ru") { return "Старт!"; }
                return "Start!";
            }
        }
        public static string Log2 // Стоп!
        {
            get
            {
                if (Settings.Lang == "ru") { return "Стоп!"; }
                return "Stop!";
            }
        }
        public static string Log3 
        {
            get
            {
                if (Settings.Lang == "ru") { return "Параметры конфига изменены!"; }
                return "The config was changed!";
            }
        }
        public static string Log4
        {
            get
            {
                if (Settings.Lang == "ru") { return "Нет свободных API-ключей"; }
                return "Not enough API-keys";
            }
        }
        public static string Log5
        {
            get
            {
                if (Settings.Lang == "ru") { return "Запланированная остановка!"; }
                return "Scheduled stop!";
            }
        }
        public static string Log6
        {
            get
            {
                if (Settings.Lang == "ru") { return "Высчитываю параметры из процентов"; }
                return "Calculation of parameters from percent";
            }
        }
        public static string Log7
        {
            get
            {
                if (Settings.Lang == "ru") { return "Не входим. Маленький спред:"; }
                return "Do not start. Spread is small:";
            }
        }
        public static string Log8
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ждём {0} сек..."; }
                return "Waiting for {0} sec...";
            }
        }
        public static string Log9
        {
            get
            {
                if (Settings.Lang == "ru") { return "Спред изменился"; }
                return "Spread was changed";
            }
        }
        public static string Log10
        {
            get
            {
                if (Settings.Lang == "ru") { return "Проверяем BUY..."; }
                return "Check the BUY order...";
            }
        }
        public static string Log11
        {
            get
            {
                if (Settings.Lang == "ru") { return "BUY исполнен! Ставим ордер на продажу..."; }
                return "The BUY order filled! Open the SELL order...";
            }
        }
        public static string Log12
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ордер был отменён — я останавливаюсь"; }
                return "The order was canceled — I stop";
            }
        }
        public static string Log13
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ордер частично исполнен"; }
                return "The order is partially filled";
            }
        }
        public static string Log14
        {
            get
            {
                if (Settings.Lang == "ru") { return "Условие на SELL но минималка (Q) не позволяет:"; }
                return "The condition for SELL but minimal (Q) does not allow:";
            }
        }
        public static string Log15
        {
            get
            {
                if (Settings.Lang == "ru") { return "Условие на SELL но минималка (P) не позволяет:"; }
                return "The condition for SELL but minimal (P) does not allow:";
            }
        }
        public static string Log16
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отменяем частинчый BUY для продажи..."; }
                return "Canceling partially BUY order for sale...";
            }
        }
        public static string Log17
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ордер исполнился пока отменялся"; }
                return "The order was filled while canceled";
            }
        }
        public static string Log18
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отменил BUY:"; }
                return "Canceled BUY:";
            }
        }
        public static string Log19
        {
            get
            {
                if (Settings.Lang == "ru") { return "Остановился!"; }
                return "Stopped!";
            }
        }
        public static string Log20
        {
            get
            {
                if (Settings.Lang == "ru") { return "Проверяем SELL..."; }
                return "Check the SELL order...";
            }
        }
        public static string Log21
        {
            get
            {
                if (Settings.Lang == "ru") { return "Условие на перестановку SELL но минималка (Q) не позволяет: "; }
                return "The condition for change SELL but minimal (Q) does not allow:";
            }
        }
        public static string Log22
        {
            get
            {
                if (Settings.Lang == "ru") { return "Условие на перестановку SELL но минималка (P) не позволяет: "; }
                return "The condition for change SELL but minimal (P) does not allow:";
            }
        }
        public static string Log23
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отменил SELL:"; }
                return "Canceled SELL:";
            }
        }
        public static string Log24
        {
            get
            {
                if (Settings.Lang == "ru") { return "Условие на PANIC SELL но минималка (Q) не позволяет: {0}"; }
                return "The condition for PANIC SELL but minimal (Q) does not allow: {0}";
            }
        }
        public static string Log25
        {
            get
            {
                if (Settings.Lang == "ru") { return "Условие на PANIC SELL но минималка (P) не позволяет: {0}"; }
                return "The condition for PANIC SELL but minimal (P) does not allow: {0}";
            }
        }
        public static string Log26
        {
            get
            {
                if (Settings.Lang == "ru") { return "{0} ордер по цене {1}"; }
                return "{0} order for {1}";
            }
        }
        public static string Log27
        {
            get
            {
                if (Settings.Lang == "ru") { return "Попытка выставить ордер..."; }
                return "Trying to open an order ...";
            }
        }

        public static string Log28
        {
            get
            {
                if (Settings.Lang == "ru") { return "Не удалось проверить ордер"; }
                return "Failed to check order";
            }
        }

        public static string Log29
        {
            get
            {
                if (Settings.Lang == "ru") { return "Не удалось отменить ордер"; }
                return "Failed to cancel order";
            }
        }

        public static string Log30
        {
            get
            {
                if (Settings.Lang == "ru") { return "Не удалось получить минималки"; }
                return "Failed to get minimal";
            }
        }

        public static string Log31
        {
            get
            {
                if (Settings.Lang == "ru") { return "Не удалось получить баланс"; }
                return "Failed to get balance";
            }
        }

        public static string Log32
        {
            get
            {
                if (Settings.Lang == "ru") { return "На аккаунте недостаточно средаств для выставления ордера!"; }
                return "There is not enough funds on the account to open this order!";
            }
        }

        public static string Log33
        {
            get
            {
                if (Settings.Lang == "ru") { return "Указанной пары нет на бирже!"; }
                return "This pair does not exist on the exchange!";
            }
        }

        public static string Log34
        {
            get
            {
                if (Settings.Lang == "ru") { return "Активирован {0} DCA-шаг"; }
                return "{0} DCA step activated";
            }
        }
        public static string Log35
        {
            get
            {
                if (Settings.Lang == "ru") { return "Режим DCA деактивирован!"; }
                return "DCA mode deactivated!";
            }
        }
        public static string Log36
        {
            get
            {
                if (Settings.Lang == "ru") { return "DCA BUY исполнен"; }
                return "The DCA BUY order was filled";
            }
        }
        public static string Log37
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ордер вероятно исполнился пока отменялся"; }
                return "The order is probably filled while canceled";
            }
        }
        public static string Log38
        {
            get
            {
                if (Settings.Lang == "ru") { return "Неизвестная ошибка! Покажите логи разработчику!"; }
                return "Unknown error! Show logs to the developer!";
            }
        }
        public static string Log39
        {
            get
            {
                if (Settings.Lang == "ru") { return "Биржа вернула неправильный ответ"; }
                return "The exchange returned a wrong response";
            }
        }
        public static string Log40
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ошибка при отправке запроса. Новая попытка..."; }
                return "Error sending request. New attempt...";
            }
        }
        public static string Log41
        {
            get
            {
                if (Settings.Lang == "ru") { return "Попытка получить стаканы"; }
                return "Trying to get a depths";
            }
        }
        public static string Log42
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отменяем ордер..."; }
                return "Canceling the order...";
            }
        }

        public static string Log43
        {
            get
            {
                if (Settings.Lang == "ru") { return "Похоже новая базовая валюта. Базовой пары не обнаружино в списке. Сообщите разработчику."; }
                return "Looks like a new base currency. The base pair is not found in the list. Notify the developer.";
            }
        }

        public static string Log44
        {
            get
            {
                if (Settings.Lang == "ru") { return "Остановился!"; }
                return "Stopped!";
            }
        }

        public static string Log45
        {
            get
            {
                if (Settings.Lang == "ru") { return "Во время отмены ордер сработал на небольшое количество, которое теперь нельзя продать из-за минималки. Поток остановлен!"; }
                return "During the canceling, the order filled for a small amount, which can not be sold due to the minimum amount. The thread is stopped!";
            }
        }

        public static string Log46
        {
            get
            {
                if (Settings.Lang == "ru") { return "Проверка по фильтрам..."; }
                return "Filter check...";
            }
        }
        public static string Log47
        {
            get
            {
                if (Settings.Lang == "ru") { return "На аккаунте недостаточно BNB для оплаты комиссии"; }
                return "Not enough BNB on account to pay fee";
            }
        }
        public static string Log48
        {
            get
            {
                if (Settings.Lang == "ru") { return "Отменяем DCA-ордер, т.к. основной ордер исполнен"; }
                return "Canceling the DCA order, because main order was filled";
            }
        }

        public static string Log49
        {
            get
            {
                if (Settings.Lang == "ru") { return "Сработало условие на стоплосс"; }
                return "The condition for StopLoss";
            }
        }

        public static string Log50
        {
            get
            {
                if (Settings.Lang == "ru") { return "Стоплосс таймер запущен"; }
                return "Start the StopLoss timer";
            }
        }

        public static string Log51
        {
            get
            {
                if (Settings.Lang == "ru") { return "SELL исполнен! Ставим ордер на покупку..."; }
                return "The SELL order filled! Open the BUY order...";
            }
        }

        public static string Log52
        {
            get
            {
                if (Settings.Lang == "ru") { return "Cработал трейлинг профит, переставляем ордер выше"; }
                return "The condition for Trailing Profit, rearrange the order higher...";
            }
        }

        public static string Log53
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цена начала падать — SELL по рынку"; }
                return "The price started to drop — SELL by market";
            }
        }

        public static string Log54
        {
            get
            {
                if (Settings.Lang == "ru") { return "Первых 10 ордеров не хватает чтобы заполнить ордер"; }
                return "The first 10 orders are not enough to fill an order";
            }
        }

        public static string Log55
        {
            get
            {
                if (Settings.Lang == "ru") { return "Cработал трейлинг профит, переставляем ордер"; }
                return "The condition for Trailing Profit, rearrange the order...";
            }
        }

        public static string Log56
        {
            get
            {
                if (Settings.Lang == "ru") { return "Цена начала расти — BUY по рынку"; }
                return "The price started to go up — BUY by market";
            }
        }

        public static string Log57
        {
            get
            {
                if (Settings.Lang == "ru") { return "Проверка прошла!"; }
                return "Filter check passed!";
            }
        }

        public static string Log58
        {
            get
            {
                if (Settings.Lang == "ru") { return "Получение правил торговли по валютной паре..."; }
                return "Getting trade reules for current pair...";
            }
        }

        public static string Log59
        {
            get
            {
                if (Settings.Lang == "ru") { return "Правила торговли получены!"; }
                return "Trade rules received ";
            }
        }

        public static string Log60
        {
            get
            {
                if (Settings.Lang == "ru") { return "Возможно нет BNB для оплаты комиссии"; }
                return "Perhaps there is no BNB to pay the fee";
            }
        }

        public static string Log61
        {
            get
            {
                if (Settings.Lang == "ru") { return "Следующая попытка через 60 секунд"; }
                return "Next try in 60 seconds";
            }
        }

        public static string Log62
        {
            get
            {
                if (Settings.Lang == "ru") { return "Недостаточно средств"; }
                return "Margin is insufficient";
            }
        }

        public static string Msg1
        {
            get
            {
                if (Settings.Lang == "ru") { return "Рекомендуется проверить все параметры перед запуском!"; }
                return "It is recommended to check all parameters before starting!";
            }
        }
        public static string Msg2
        {
            get
            {
                if (Settings.Lang == "ru") { return "Конфиг сохранён!"; }
                return "The config was saved!";
            }
        }
        public static string Msg3
        {
            get
            {
                if (Settings.Lang == "ru") { return "Конфиг загружён!"; }
                return "The config was loaded!";
            }
        }
        public static string Msg4
        {
            get
            {
                if (Settings.Lang == "ru") { return "Для данной биржи эта стратегия недоступна!"; }
                return "This strategy is not available for this exchange!";
            }
        }
        public static string Msg5
        {
            get
            {
                if (Settings.Lang == "ru") { return "С этим файлом что-то не так..."; }
                return "There is something wrong with this file ...";
            }
        }
        public static string Msg6
        {
            get
            {
                if (Settings.Lang == "ru") { return "Конфиг обновлён!"; }
                return "The config is updated!";
            }
        }
        public static string Msg7
        {
            get
            {
                if (Settings.Lang == "ru") { return "Коллекция сохранена!"; }
                return "The collection was saved!";
            }
        }
        public static string Msg8
        {
            get
            {
                if (Settings.Lang == "ru") { return "Не выбран ни один ордер"; }
                return "No order selected";
            }
        }
        public static string Msg9
        {
            get
            {
                if (Settings.Lang == "ru") { return "При загрузке бекапа произошла ошибка!"; }
                return "An error occurred while loading backup!";
            }
        }

        public static string Msg10
        {
            get
            {
                if (Settings.Lang == "ru") { return "Продлите лицензию!"; }
                return "Renew the license!";
            }
        }

        public static string Msg11
        {
            get
            {
                if (Settings.Lang == "ru") { return "Лицензии не существует или уже активирована!"; }
                return "License does not exist or is already activated!";
            }
        }

        public static string Msg12
        {
            get
            {
                if (Settings.Lang == "ru") { return "Перед запросом лицензии необходимо добавить данный девайс в личном кабинете btn.plus на странице Девайсы"; }
                return "Before requesting a license, you must add this device in your btn.plus personal cabinet on the Devices page";
            }
        }
        public static string Msg13
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ключ вашей лицензии вставлен в нужное поле. Активируйте лицензию!"; }
                return "Your license key is inserted in the required field. Activate the license!";
            }
        }
        public static string Msg14
        {
            get
            {
                if (Settings.Lang == "ru") { return "Поздравляем! Ваша лицензия успешно активирована! Бот будет закрыт, ЗАПУСТИТЕ ЕГО ЗАНОВО, чтобы продолжить работу!"; }
                return "Congratulations! Your license has been successfully activated! The bot will be closed, START IT AGAIN to continue!";
            }
        }

        public static string Msg15
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ошибка лицензии!"; }
                return "License error!";
            }
        }

        public static string Msg16
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ошибка лицензии! Убедитесь, что данный девайс добавлен в ваш личный кабинет btn.plus!"; }
                return "License Error! Make sure that this device is added to your personal cabinet on btn.plus!";
            }
        }

        public static string Msg17
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ошибка лицензии! Лицензия не найдена!"; }
                return "License Error! No license found!";
            }
        }
        public static string Msg18
        {
            get
            {
                if (Settings.Lang == "ru") { return "Нужно добавить API-ключи!"; }
                return "You need to add API-keys!";
            }
        }
        public static string Msg19
        {
            get
            {
                if (Settings.Lang == "ru") { return "Ошибка при загрузке файла. Скачайте архив вручную в кабинете account.btn.plus/license."; }
                return "Loading file error. Download the archive manually in your personal cibanet account.btn.plus/license.";
            }
        }

        // Ограничения
        public static string Msg20 // (!) тут соддержим ограничение
        {
            get
            {
                if (Build.Level >= (int)Level.SellLimit) { return "code error 380034003200"; } // 842 - INFINITI
                if (Build.Level >= (int)Level.SellMarket) { return "code error 380034003200"; } // 842 - PRO
                if (Build.Level >= (int)Level.BuyLimit) { return "code error 32003000"; } // 20 - LITE
                if (Build.Level >= (int)Level.BuyMarket) { return "code error 3300"; } // 3 - FREE
                return "Default error";
            }
        }

        public static string Msg21
        {
            get
            {
                if (Settings.Lang == "ru") { return "Временно отключено из-за добавления новых функций."; }
                return "Temporarily disabled due to the addition of new features.";
            }
        }

        public static string Msg22
        {
            get
            {
                if (Settings.Lang == "ru") { return "Скорее всего произошёл сбой статистики! Пожалуйста отправьте логи разработчику прямо сейчас, чтобы помочь улучшить продукт!"; }
                return "Error. Send all logs to the developer!";
            }
        }


        // Количество разрешенных запусков приложения  
        public static string c
        {
            get
            {
                if (Settings.Lang == "ru") { return "3300310030003000"; }
                return "3300310030003000";
            }
        }

        // HelpLink фильтра id=1 H/L SMA
        public static string FilterHelpLink1
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/hlsma"; }
                return "https://docs.btn.plus/en/stratum-bot/hlsma";
            }
        }
        // HelpLink фильтра id=2 H/L EMA
        public static string FilterHelpLink2
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/hlema"; }
                return "https://docs.btn.plus/en/stratum-bot/hlema";
            }
        }
        // HelpLink фильтра id=3 PriceChange
        public static string FilterHelpLink3
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/price-change"; }
                return "https://docs.btn.plus/en/stratum-bot/price-change";
            }
        }
        // HelpLink фильтра id=4 PriceLimit
        public static string FilterHelpLink4
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/price-limit"; }
                return "https://docs.btn.plus/en/stratum-bot/price-limit";
            }
        }

        // HelpLink фильтра id=5 DOM Volume Diff
        public static string FilterHelpLink5
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/dom-volume-diff"; }
                return "https://docs.btn.plus/en/stratum-bot/dom-volume-diff";
            }
        }

        // HelpLink фильтра id=6 H/L SMMA
        public static string FilterHelpLink6
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/hlsmma"; }
                return "https://docs.btn.plus/en/stratum-bot/hlsmma";
            }
        }
        // HelpLink фильтра id=7 NGA
        public static string FilterHelpLink7
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/nga"; }
                return "https://docs.btn.plus/en/stratum-bot/nga";
            }
        }
        // HelpLink фильтра id=8 OHLC+ Limit
        public static string FilterHelpLink8
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/ohlc-limit"; }
                return "https://docs.btn.plus/en/stratum-bot/ohlc-limit";
            }
        }
        // HelpLink фильтра id=9 Cross
        public static string FilterHelpLink9
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/cross"; }
                return "https://docs.btn.plus/en/stratum-bot/cross";
            }
        }
        // HelpLink фильтра id=10 Bollinger Bands
        public static string FilterHelpLink10
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/bollinger-bands"; }
                return "https://docs.btn.plus/en/stratum-bot/bollinger-bands";
            }
        }
        // HelpLink фильтра id=11 RSI
        public static string FilterHelpLink11
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/rsi"; }
                return "https://docs.btn.plus/en/stratum-bot/rsi";
            }
        }
        // HelpLink фильтра id=12 Stoch
        public static string FilterHelpLink12
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/stoch"; }
                return "https://docs.btn.plus/en/stratum-bot/stoch";
            }
        }
        // HelpLink фильтра id=13 Stoch RSI
        public static string FilterHelpLink13
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/stoch-rsi"; }
                return "https://docs.btn.plus/en/stratum-bot/stoch-rsi";
            }
        }
        // HelpLink фильтра id=14 Email_Notify
        public static string FilterHelpLink14
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/email-notify"; }
                return "https://docs.btn.plus/en/stratum-bot/email-notify";
            }
        }
        // HelpLink фильтра id=15 URL
        public static string FilterHelpLink15
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/url"; }
                return "https://docs.btn.plus/en/stratum-bot/url";
            }
        }
        // HelpLink фильтра id=16 Spread
        public static string FilterHelpLink16
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/spread"; }
                return "https://docs.btn.plus/en/stratum-bot/spread";
            }
        }
        // HelpLink фильтра id=17 MA Spread
        public static string FilterHelpLink17
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/ma-spread"; }
                return "https://docs.btn.plus/en/stratum-bot/ma-spread";
            }
        }
        // HelpLink фильтра id=18 Candle Price Change 
        public static string FilterHelpLink18
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/candle-price-change"; }
                return "https://docs.btn.plus/en/stratum-bot/candle-price-change";
            }
        }
        // HelpLink фильтра id=19 MFI
        public static string FilterHelpLink19
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/mfi"; }
                return "https://docs.btn.plus/en/stratum-bot/mfi";
            }
        }
        // HelpLink фильтра id=20 CCI
        public static string FilterHelpLink20
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/cci"; }
                return "https://docs.btn.plus/en/stratum-bot/cci";
            }
        }
        // HelpLink фильтра id=21 Timer
        public static string FilterHelpLink21
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/timer"; }
                return "https://docs.btn.plus/en/stratum-bot/timer";
            }
        }
        // HelpLink фильтра id=22 Candle Color
        public static string FilterHelpLink22
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/candle-color"; }
                return "https://docs.btn.plus/en/stratum-bot/candle-color";
            }
        }

        // HelpLink фильтра id=23 Volume Limit
        public static string FilterHelpLink23
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/volume-limit"; }
                return "https://docs.btn.plus/en/stratum-bot/volume-limit";
            }
        }

        // HelpLink фильтра id=24 Bollinger Bands Width
        public static string FilterHelpLink24
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/bollinger-bands"; }
                return "https://docs.btn.plus/en/stratum-bot/bollinger-bands";
            }
        }

        // HelpLink фильтра id=25 Keltner Channels
        public static string FilterHelpLink25
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/keltner-channels"; }
                return "https://docs.btn.plus/en/stratum-bot/keltner-channels";
            }
        }

        // HelpLink фильтра id=26 STARC Bands
        public static string FilterHelpLink26
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/starc-bands"; }
                return "https://docs.btn.plus/en/stratum-bot/starc-bands";
            }
        }

        // HelpLink фильтра id=27 MA Envelopes
        public static string FilterHelpLink27
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/ma-envelopes"; }
                return "https://docs.btn.plus/en/stratum-bot/ma-envelopes";
            }
        }

		// HelpLink фильтра id=28 Donchian Channel
		public static string FilterHelpLink28
		{
			get
			{
				if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/donchian-channel"; }
				return "https://docs.btn.plus/en/stratum-bot/donchian-channel";
			}
		}

        // HelpLink фильтра id=29 supertrend
        public static string FilterHelpLink29
        {
            get
            {
                if (Settings.Lang == "ru") { return "https://docs.btn.plus/ru/stratum-bot/supertrend"; }
                return "https://docs.btn.plus/en/stratum-bot/supertrend";
            }
        }
    }
}
