using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using System;
using System.Collections.Generic;

namespace stratumbot.Models.Filters
{
    /// <summary>
    /// Filters manager
    /// </summary>
    public class Filters
    {
        /// <summary>
        /// The BUY list of filter objects
        /// </summary>
        public List<IFilter> BuyFilters { get; set; } = new List<IFilter>();

        public int TargetPointBuy { get; set; } = 0;

        /// <summary>
        /// The SELL list of filter objects
        /// </summary>
        public List<IFilter> SellFilters { get; set; } = new List<IFilter>();

        public int TargetPointSell { get; set; } = 0;

        /// <summary>
        /// The stoploss (BUY/SELL) list of filter objects
        /// </summary>
        public List<IFilter> StopLossFilters { get; set; } = new List<IFilter>();

        public int TargetPointStopLoss { get; set; } = 0;

        // TODO check does dmth wrong with this type?
        public Dictionary<int, Tools.DCAFilter> DCAFilters = new Dictionary<int, Tools.DCAFilter>();

        /// <summary>
        /// Creating IFilter object for each filter from BtnPlus ID
        /// </summary>
        /// <param name="list">A list of filter objects</param>
        /// <param name="filtersSettings">A list of filters settings from json</param>
        /// <param name="exchange">Enum exchange to set it for DataProvider</param>
        /// <param name="config">Thread/strategy config (used: Cur1, Cur2)</param>
        /// <param name="filterSide">BUY, SELL, STOPLOSS</param>
        private void FiltersInit(List<IFilter> list, List<JsonFilter> filtersSettings, IExchange exchange, IConfig config, string filterSide)
        {
            foreach (var filterSettings in filtersSettings)
            {
                // Create the filter object
                IFilter filter = FilterManager.GetFilterObjectByBtnPlusId(filterSettings, filterSide);

                // TODO check does specific filter can use for specific exchange or throw ThreadStopException
                
                // Set the axhange to DataProvider of the current filter
                filter.DataProvider.SetExchangeClient(exchange);

                // Set required data types and options for them to the DataProvider
                foreach (var dataType in filter.RequiredDataTypes)
                {
                    filter.DataProvider.AddRequiredDataType(dataType);

                    filter.DataProvider.SetRequiredDataOptions
                        (
                            dataType,
                            filter.GetOptions(dataType, config.Cur1, config.Cur2)
                        );
                }

                // Add IFilter object to the filter list
                list.Add(filter);
            }
        }

        /// <summary>
        /// Init filters from the BUY list
        /// </summary>
        /// <param name="filters">A list of filters settings from json</param>
        /// <param name="exchange">Enum exchange to set it for DataProvider</param>
        /// <param name="config">Thread/strategy config (used: Cur1, Cur2)</param>
        public void BuyFiltersInit(List<JsonFilter> filters, IExchange exchange, IConfig config)
        {
            this.FiltersInit
                (
                    this.BuyFilters,
                    filters,
                    exchange,
                    config,
                    "BUY"
                );
        }

        /// <summary>
        /// Init filters from the SELL list
        /// </summary>
        /// <param name="filters">A list of filters settings from json</param>
        /// <param name="exchange">Enum exchange to set it for DataProvider</param>
        /// <param name="config">Thread/strategy config (used: Cur1, Cur2)</param>
        public void SellFiltersInit(List<JsonFilter> filters, IExchange exchange, IConfig config)
        {
            this.FiltersInit
                (
                    this.SellFilters,
                    filters,
                    exchange,
                    config,
                    "SELL"
                );
        }

        /// <summary>
        /// Init filters from the stoploss SELL list
        /// </summary>
        /// <param name="filters">A list of filters settings from json</param>
        /// <param name="exchange">Enum exchange to set it for DataProvider</param>
        /// <param name="config">Thread/strategy config (used: Cur1, Cur2)</param>
        public void StopLossSellFiltersInit(List<JsonFilter> filters, IExchange exchange, IConfig config)
        {
            this.FiltersInit
                (
                    this.StopLossFilters,
                    filters,
                    exchange,
                    config,
                    "SELL"
                );
        }



        // public bool Allowed();

        public bool FiltersMonitoring(List<IFilter> filters, int targetPoint)
        {
            bool isRecheckBuyFilters = false;
            bool isRecheckSellFilters = false;
            if (filters[0].FilterSide == "BUY")
                isRecheckBuyFilters = (Settings.RecheckBuyFiltersTimeout > 0) ? true : false; // Нужна ли повторная проверка фильтров
            else
                isRecheckSellFilters = (Settings.RecheckSellFiltersTimeout > 0) ? true : false; // Нужна ли повторная проверка фильтров

            bool isFirstCheck = true; // Первая удачная проверка

            //while (true)
            {
                filtercheck:

                Logger.Info(_.Log46); // Проверка по фильтрам...

                int currentWeigth = 0;
                // Получить все данные
                //this.DM.GetData(filters);

                foreach (var filter in filters)
                {
                    // Отдаём данные фильтру
                    //filter.DataMiner = this.DM;

                    try
                    {
                        filter.DataProvider.GetData();
                    }
                    catch (AutoStopException ex)
                    {
                        Logger.Debug(ex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Ошибка получения данных для фильтра");
                        Logger.ToFile($"[exception] Filters.CheckFilters 1 {ex.ToString()}");
                        return false;
                    }

                    currentWeigth += filter.CurrentWeight; // TODO rename CheckAndGetWeight()

                    if (currentWeigth >= targetPoint)
                        break;
                }

                // TODO check weight by group. BUG Сейчас может чекнуть по 1 фильтру в каждой группе и если таргет 2 то он продйт вроде
                if (currentWeigth >= targetPoint)
                {
                    if (isRecheckBuyFilters && isFirstCheck) // Если на buy есть таймаут и это первая удачная проверка
                    {
                        isFirstCheck = false;
                        System.Threading.Thread.Sleep(Settings.RecheckBuyFiltersTimeout);
                        //continue;
                        goto filtercheck;
                    }
                    if (isRecheckSellFilters && isFirstCheck) // Если на sell есть таймаут и это первая удачная проверка
                    {
                        isFirstCheck = false;
                        System.Threading.Thread.Sleep(Settings.RecheckSellFiltersTimeout);
                        //continue;
                        goto filtercheck;
                    }
                    //break; // Фильтры прошли проверку / Либо нет таймаутов либо со второй проверки
                    Logger.Info(_.Log57); // Проверка прошла!
                    return true;
                }
                else
                {
                    isFirstCheck = true; // Вторая попытка не разрешила торговлю, указываем что след удачная проверка будет первой
                    System.Threading.Thread.Sleep(Settings.FiltersTimeout);
                    //goto filtercheck;
                    return false;
                }

                //System.Threading.Thread.Sleep(Settings.FiltersTimeout);
            }

            
        }

        // TODO НАФИГА оно вообще нужно
        // Проверка по фильтрам разовая
        public bool CheckFilters(List<IFilter> filters, int targetPoint)
        {
            Logger.ToFile(_.Log46); // Проверка по фильтрам...

            var currentWeigth = 0;
            // Получить все данные
            //this.DM.GetData(filters);

            foreach (var filter in filters)
            {
                // Отдаём данные фильтру
                //filter.DataMiner = this.DM;

                try
                {
                    filter.DataProvider.GetData();
                }
                catch (AutoStopException ex)
                {
                    Logger.Debug(ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Error("Ошибка получения данных для фильтра");
                    Logger.ToFile($"[exception] Filters.CheckFilters 2 {ex.Message}");
                    return false;
                }

                currentWeigth += filter.CurrentWeight;

                if (currentWeigth >= targetPoint)
                    return true;
            }

            return false;
        }

        // Проверка фильтров для стоплосса
        public bool CheckStopLossFilters(List<IFilter> filters, int targetPoint)
        {
            Logger.ToFile("Проверка StopLoss по фильтрам..."); // TODO TEXT

            var currentWeigth = 0;
            // Получить все данные
            //this.DM.GetData(this.FiltersStopLoss);

            foreach (var filter in filters)
            {
                // Отдаём данные фильтру
                //filter.DataMiner = this.DM;

                try
                {
                    filter.DataProvider.GetData();
                }
                catch
                {
                    Logger.Error("Ошибка получения данных для фильтра");
                }

                currentWeigth += filter.CurrentWeight;

                if (currentWeigth >= targetPoint)
                    return true;
            }

            if (currentWeigth >= targetPoint)
                return true;

            return false;
        }


        public Dictionary<int, List<IFilter>> DCAStepFilters { get; set; } = new Dictionary<int, List<IFilter>>();// Настройки (реальные объекты фильтров) по шагамм, инициализированные фильтры по ID
        public void DcaFiltersInit(int StepCount, IExchange exchange, string cur1, string cur2)
        {
            if (this.DCAFilters.Count == 0)
                return;

            // Проходим по всем шагам
            for (int i = 1; i <= StepCount; i++)
            {
                // Проверяем, есть ли для текущега шага фильтры
                if (!this.DCAFilters.ContainsKey(i))
                    continue;

                // Инициализируем все фильтры данного шага
                // По всем ID получить настройки
                List<IFilter> stepFilters = new List<IFilter>();
                foreach (var filterSettings in this.DCAFilters[i].Filters)
                {

                    IFilter filter = FilterManager.GetFilterObjectByBtnPlusId(filterSettings, "BUY");

                        // Set the axhange to DataProvider of the current filter
                    filter.DataProvider.SetExchangeClient(exchange);

                    // Set required data types and options for them to the DataProvider
                    foreach (var dataType in filter.RequiredDataTypes)
                    {
                        filter.DataProvider.AddRequiredDataType(dataType);

                        filter.DataProvider.SetRequiredDataOptions
                            (
                                dataType,
                                filter.GetOptions(dataType, cur1, cur2)
                            );
                    }

                    stepFilters.Add(filter); // TODO check 5131 BUY, потому что DCA для Лонга = надо смотреть на ask
                }
                this.DCAStepFilters[i] = new List<IFilter>(stepFilters);

                // Получить все нужные типы данных
                //DM.GetRequiredData(this.DCAStepFilters[i]);

                // TODO в будущем лучше наверно сделать DM для каждого шага, чтобы запрашивать меньше данных при шагах
            }

            //DM.SetExchange(this.Exchange); // Передаем биржу
            //DM.Config = this.Config;
        }


    }
}
