using Newtonsoft.Json;
using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Filters;
using stratumbot.Models.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace stratumbot.Models.Tools
{
    public delegate void DCAStepChangedDelegate(int stepNum);

    public class DCAStep
    {
        public decimal PriceDropPercent { get; set; } // % на который должна упасть цена, чтобы шаг активировался (выставился BUY-ордер)
        public decimal AddAmountPercent { get; set; } // % от уже купленного объема (в т.ч. из рабочего алгоритма и предыдущих шагов) на который будет выставлен BUY-ордер
        public Order Order { get; set; } // BUY-ордер данного шага
        public decimal TriggerPrice { get; set; } // Цена, при котороый выставляется BUY данного шага
        public bool BuyExposed { get; set; } // Активирован (выставлен BUY) ли данный шаг
        public decimal BuyPrice { get; set; } // Цена по которой будет выставлен BUY данного шага
        public decimal Amount { get; set; } // Объем, которые следует докупить на данном шаге
        public decimal Filled { get; set; } // Исполненый BUY объём данного шага
        public bool SellExposed { get; set; } // Выставлен ли усреднёный SELL по данному шагу
        public decimal SellPrice { get; set; } // Цена по которой будет выставлен SELL данного шага
    }

    //dcaf
    public class DCAFilter
    {
        [JsonProperty("Filters")]
        public List<JsonFilter> Filters { get; set; } // Спиок фильтров без настроек (Id, цвета)
        [JsonProperty("FiltersSettings")]
        public List<IFilter> FiltersSettings { get; set; } // Список фильтров (типа как FiltersBuy в ClassicLong.cs)
        [JsonProperty("TargetPoint")]
        public int TargetPoint { get; set; } // TargetPoint для текущих фильтров
        [JsonProperty("CurrentWeigth")]
        public int CurrentWeigth { get; set; } // Текущий вес для текущего списка фильтров
    }

    /// <summary>
    /// Dollar Cost Average algorithm
    /// </summary>
    public class DCA
    {
        public List<DCAStep> Step { get; set; }

        private IConfig Config; // Настройки потока, который будет использовать DCA

        private IExchange Exchange; // Биржа на которой будут проверяться/выставлять ордера (передаем в конструкторе)
        
        public Trades Trades; // Пул ордеров от рабочего алгоритма

        public decimal ProfitPercent { get; set; } // Процент профита при срабатывании DCA
        public int StepCount { get; set; } // Количество шагов DCA
        public decimal ApproximationPercent { get; set; } // При приближении к PriceDropPercent на этот процент ставим BUY-ордер
        public bool IsTriggered { get; set; } // Активирован ли режим DCA (выставлен хотя бы один buy)
        int currentStep; // Текущий сработанный (выставленный) DCA шаг (0 если никакой)
        public int CurrentStep {
            get { return currentStep; }
            set
            {
                currentStep = value;
                DCAStepChangedEvent?.Invoke(value);
            }
        } 
        public decimal CurrentPrice { get; set; } // Текущая цена в стакане
        public Order SellOrder { get; set; } // Текущий SELL ордер (как от рабочего алгоритма так и усреднёный может быть)
        public Order BuyForOrderPool { get; set; } // Частично сработанный BUY-ордер, чтобы добавить его в пул ордеров рабочего алгоритма а затем опять присвоить null
        public Order BuyOrder { get; set; } // Текущий BUY ордер (как от рабочего алгоритма так и усреднёный может быть)
        public Order SellForOrderPool { get; set; } // Частично сработанный SELL-ордер, чтобы добавить его в пул ордеров рабочего алгоритма а затем опять присвоить null

        public Filters.Filters Filters { get; set; } = new Filters.Filters();
        public Dictionary<int, DCAFilter> DCAFilters { get; set; } // Фильтры по шагам (Id, без настроек)
        public Dictionary<int, List<IFilter>> DCAStepFilters { get; set; } // Настройки (реальные объекты фильтров) по шагамм, инициализированные фильтры по ID
        //public DataMiner DM { get; set; } // Данные
        public int CurrentWeigth { get; set; } // Вес

        public event DCAStepChangedDelegate DCAStepChangedEvent; // Событие "DCA step изменился"

        public DCA(IConfig _config, IExchange _exchange)
        {
            // Инициализаци всех полей класса
            this.Step = new List<DCAStep>();
            this.Config = _config;
            this.Exchange = _exchange; // Если сделать AvailableExchanges.CreateExchangeById(_config.Exchange); то это новый экземпляр, который не имеет ни минималок ничего общ
            this.Trades = new Trades();
            this.ProfitPercent = _config.DCAProfitPercent;
            this.StepCount = _config.DCAStepCount;
            for (int i = 0; i <= this.StepCount; i++)
            {
                // Переменные для вычисления какого типа указаны значения объема докупки (в процентах или пунктах)
                // TODO FUTURE Сделать также и падение цены - decimal priceDropPercent = 0; decimal triggerPrice = 0;
                decimal addAmountPercent = 0;
                decimal amount = 0;

                // Если объем докупки содержит %. Если нет то значит это в количестве (пунктах)
                if(i > 0)
                {
                    //if (_config.DCAStepsStr[i - 1][2].Contains("%"))
                    if (_config.DCASteps.Where(x => x.Number == i).Last().IsAmountPercentage)
                        addAmountPercent = _config.DCASteps.Where(x => x.Number == i).Last().Amount;
                        //addAmountPercent = Conv.dec(_config.DCAStepsStr[i - 1][2], true);
                    else
                        amount = _config.DCASteps.Where(x => x.Number == i).Last().Amount;
                        //amount = Conv.dec(_config.DCAStepsStr[i - 1][2], true);
                }

                this.Step.Add(
                new DCAStep() {
                    //PriceDropPercent = (i == 0) ? 0 : Conv.dec(_config.DCAStepsStr[i - 1][1], true),
                    PriceDropPercent = (i == 0) ? 0 : _config.DCASteps.Where(x => x.Number == i).Last().Drop,
                    AddAmountPercent = (i == 0) ? 0 : addAmountPercent,
                    Order = new Order(),
                    TriggerPrice = 0,
                    BuyExposed = false,
                    BuyPrice = 0,
                    Amount = amount,
                    Filled = 0,
                    SellExposed = false,
                    SellPrice = 0
                }); // Инициализация переменных шаг(а/ов)
            }
            this.ApproximationPercent = Conv.dec(Settings.ApproximationPercent, true);
            this.IsTriggered = false;
            this.CurrentStep = 0;
            this.CurrentPrice = 0;
            this.SellOrder = null;
            this.BuyForOrderPool = null;

            if (this.Filters.DCAFilters != null)
                this.DCAFilters = new Dictionary<int, DCAFilter>(this.Filters.DCAFilters); // dcaf
            else
                this.DCAFilters = new Dictionary<int, DCAFilter>(); // dcaf

            this.DCAStepFilters = new Dictionary<int, List<IFilter>>();
            //this.DM = new DataMiner();
            this.CurrentWeigth = 0; // Собранное количество баллов
        }

        /// <summary>
        /// Метод проверяет возможно и нужно ли ставить следующий шаг. Условие для входа в NextStep()
        /// </summary>
        /// <returns></returns>
        public bool CheckNecessary()
        {
            // Если текущая цена больше, чем цена при которой должен быть выставлен BUY - пока ничего не делаем. 
            if (this.CurrentStep == this.StepCount || this.CurrentPrice > Step[(CurrentStep + 1)].TriggerPrice)
            {
                // TODO DELETE
                try
                {
                    Logger.ToFile($"Checking DCA necessary... this.CurrentStep {this.CurrentStep} this.StepCount {this.StepCount} || this.CurrentPrice {this.CurrentPrice} > TriggerPrice {Step[(CurrentStep + 1)].TriggerPrice}");
                }
                catch
                {
                    Logger.ToFile($"Checking DCA necessary .2... this.CurrentStep {this.CurrentStep} == this.StepCount {this.StepCount}");
                }
                return false;
            }
                

            // Если SELL-ордер от рабочего алгоритма (скальпинга) частично исполнен и режим DCA ещё не активирован
            if (this.SellOrder.Status == OrderStatus.PartiallyFilled && this.IsTriggered == false)
            {
                // Проверяем минималки
                // Объём и стоимость на основе первого шага DCA, т.к. он будет докупаться
                if(this.Step[1].AddAmountPercent != 0)
                {
                    // Если значения объема докупки указаны в процентах
                    if (Calc.AmountOfPercent(this.Step[1].AddAmountPercent, this.SellOrder.Remainder) <= this.Exchange.MinAmount)
                        return false;
                    if ((Calc.AmountOfPercent(this.Step[1].AddAmountPercent, this.SellOrder.Remainder) * CurrentPrice) <= this.Exchange.MinCost)
                        return false;
                } else
                {
                    // Если значения объема докупки указаны в пунктах
                    if (this.Step[1].Amount <= this.Exchange.MinAmount)
                        return false;
                    if ((this.Step[1].Amount * CurrentPrice) <= this.Exchange.MinCost)
                        return false;
                }
                

                // Делаем перерасчёт, т.к. SELL рабочего алгоритма частично исполнен, а значит часть денег мы вернули.
                this.ComputeSteps();
            }

            // Не кончились ли шаги в настройках DCA
            if ((CurrentStep + 1) >= Step.Count)
                return false; // След ордер ставить не надо, т.к. его нет в настройках

            // Проверяем шаг на фильтры
            // Если для след. шага есть настройки по фильтрам && фильтров больше 0 (они есть)
            if (this.DCAFilters.ContainsKey((CurrentStep + 1)))
            {
                if (this.DCAFilters[(CurrentStep + 1)].Filters.Count > 0)
                {
                    if (!this.Filters.CheckFilters(this.DCAStepFilters[(CurrentStep + 1)], this.DCAFilters[(CurrentStep + 1)].TargetPoint))
                    {
                        Logger.ToFile("DCA filter не прошли");
                        return false;
                    }
                    Logger.Info("DCA фильтры прошли проверку!"); // TODO norm text
                }
            }


            return true; // Прошли все проверки, активируем (выставляем BUY) следующий шаг
        }
        public bool CheckNecessaryShort()
        {
            // Если текущая цена меньше, чем цена при которой должен быть выставлен SELL - пока ничего не делаем. 
            if (this.CurrentStep == this.StepCount || this.CurrentPrice < Step[(CurrentStep + 1)].TriggerPrice)
            {
                // TODO DELETE
                try
                {
                    Logger.ToFile($"Checking DCA necessary... this.CurrentStep {this.CurrentStep} this.StepCount {this.StepCount} || this.CurrentPrice {this.CurrentPrice} < TriggerPrice {Step[(CurrentStep + 1)].TriggerPrice}");
                }
                catch
                {
                    Logger.ToFile($"Checking DCA necessary .2... this.CurrentStep {this.CurrentStep} == this.StepCount {this.StepCount}");
                }
                return false;
            }


            // Если BUY-ордер от рабочего алгоритма (скальпинга) частично исполнен и режим DCA ещё не активирован
            if (this.BuyOrder.Status == OrderStatus.PartiallyFilled && this.IsTriggered == false)
            {
                // Проверяем минималки
                // Объём и стоимость на основе первого шага DCA, т.к. он будет докупаться
                if (this.Step[1].AddAmountPercent != 0)
                {
                    // Если значения объема допродажи указаны в процентах
                    if (Calc.AmountOfPercent(this.Step[1].AddAmountPercent, this.BuyOrder.Remainder) <= this.Exchange.MinAmount)
                        return false;
                    if ((Calc.AmountOfPercent(this.Step[1].AddAmountPercent, this.BuyOrder.Remainder) * CurrentPrice) <= this.Exchange.MinCost)
                        return false;
                }
                else
                {
                    // Если значения объема докупки указаны в пунктах
                    if (this.Step[1].Amount <= this.Exchange.MinAmount)
                        return false;
                    if ((this.Step[1].Amount * CurrentPrice) <= this.Exchange.MinCost)
                        return false;
                }


                // Делаем перерасчёт, т.к. BUY рабочего алгоритма частично исполнен, а значит часть денег мы вернули.
                this.ComputeStepsShort();
            }

            // Не кончились ли шаги в настройках DCA
            if ((CurrentStep + 1) >= Step.Count)
                return false; // След ордер ставить не надо, т.к. его нет в настройках

            // Проверяем шаг на фильтры
            // Если для след. шага есть настройки по фильтрам && фильтров больше 0 (они есть)
            if (this.DCAFilters.ContainsKey((CurrentStep + 1)))
            {
                if (this.DCAFilters[(CurrentStep + 1)].Filters.Count > 0)
                {
                    if (!this.Filters.CheckFilters(this.DCAStepFilters[(CurrentStep + 1)], this.DCAFilters[(CurrentStep + 1)].TargetPoint))
                    {
                        Logger.ToFile("DCA filter не прошли");
                        return false;
                    }
                    Logger.Info("DCA фильтры прошли проверку!"); // TODO norm text
                }
            }


            return true; // Прошли все проверки, активируем (выставляем SELL) следующий шаг
        }

        /// <summary>
        /// Метод выставляет следующий шаг, если CurrentPrice меньше либо равна TriggerPrice текущего шага
        /// </summary>
        public void NextStep()
        {
            // Если текущая цена <= цене, при которой должен быть сработан(выставлен BUY) следующий шаг:

            this.CurrentStep++; // Сработал (выставлен) новый шаг
            Logger.Info(String.Format(_.Log34, this.CurrentStep)); // Активирован {CurrentStep} DCA-шаг

            if (this.CurrentStep == 1)
                IsTriggered = true; // Режим DCA активирован (т.к. сработал хотя бы один шаг)
            Step[CurrentStep].BuyExposed = true; // Шаг активирован

            // Расчитываем объем текущего шага на случай, если он был активирован ранее и частично исполнен
            decimal amount = this.Step[CurrentStep].Amount - Step[CurrentStep].Filled;
            // ↓ Если BUY частично исполнен на 99% и он отменяется, то в следующий раз при попытке активировать данный шаг будет ошибка MinAmount
            if (amount < this.Exchange.MinAmount) // Проверка на минималку
            {
                amount = this.Exchange.MinAmount; // Возьмем минималку, чтобы хватило.
                // TODO FUTURE учитывать лишнее закупленное при следующм шаге или оно само учитывается?
            }
            // Выставляем BUY ордер 
            Order buyOrder = this.Exchange.OpenLimitOrder(OrderSide.BUY, Config.Cur1, Config.Cur2, amount, Step[CurrentStep].BuyPrice);
            Step[CurrentStep].Order = buyOrder; // Запоминаем ордер для дальнейших обращений
        }
        public void NextStepShort()
        {
            // Если текущая цена >= цене, при которой должен быть сработан(выставлен SELL) следующий шаг:

            this.CurrentStep++; // Сработал (выставлен) новый шаг
            Logger.Info(String.Format(_.Log34, this.CurrentStep)); // Активирован {CurrentStep} DCA-шаг

            if (this.CurrentStep == 1)
                IsTriggered = true; // Режим DCA активирован (т.к. сработал хотя бы один шаг)
            Step[CurrentStep].SellExposed = true; // Шаг активирован

            // Расчитываем объем текущего шага на случай, если он был активирован ранее и частично исполнен
            decimal amount = this.Step[CurrentStep].Amount - Step[CurrentStep].Filled;
            // ↓ Если SELL частично исполнен на 99% и он отменяется, то в следующий раз при попытке активировать данный шаг будет ошибка MinAmount
            if (amount < this.Exchange.MinAmount) // Проверка на минималку
            {
                amount = this.Exchange.MinAmount; // Возьмем минималку, чтобы хватило.
                // TODO FUTURE учитывать лишнее закупленное при следующм шаге или оно само учитывается?
            }
            // Выставляем SELL ордер 
            Order sellOrder = this.Exchange.OpenLimitOrder(OrderSide.SELL, Config.Cur1, Config.Cur2, amount, Step[CurrentStep].SellPrice);
            Step[CurrentStep].Order = sellOrder; // Запоминаем ордер для дальнейших обращений
        }

        /// <summary>
        /// Метод отката шагов DCA. Отменяет BUY-ордера, откатывает CurrentStep и выключает режим DCA при надобности.
        /// </summary>
        public void Rollback()
        {
            // Если текущая цена больше сигнала на срабатывание шага (выставление buy) И BUY-ордер выставлен И не исполнен никак И SELL-ордер ещё не выставлен
            if(this.CurrentPrice > this.Step[CurrentStep].TriggerPrice && this.Step[CurrentStep].BuyExposed == true && this.Step[CurrentStep].Order.Status == OrderStatus.New && this.Step[CurrentStep].SellExposed == false)
            {
                this.Step[CurrentStep].BuyExposed = false; // Отменяем активацию шага и его ордер
                Order buyOrder = this.Exchange.CancelOrder(this.Step[CurrentStep].Order);
                this.CurrentStep--; // Откатываем текущий шаг, т.к. отменили
                // Если не выставлен ни один BUY-ордер отключаем режим DCA
                if (this.CurrentStep == 0)
                {
                    this.IsTriggered = false;
                    Logger.Info(_.Log35); // Режим DCA деактивирован!
                }
            }
        }
        public void RollbackShort()
        {
            // Если текущая цена меньше сигнала на срабатывание шага (выставление buy) И BUY-ордер выставлен И не исполнен никак И SELL-ордер ещё не выставлен
            if (this.CurrentPrice < this.Step[CurrentStep].TriggerPrice && this.Step[CurrentStep].SellExposed == true && this.Step[CurrentStep].Order.Status == OrderStatus.New && this.Step[CurrentStep].BuyExposed == false)
            {
                this.Step[CurrentStep].SellExposed = false; // Отменяем активацию шага и его ордер
                Order sellOrder = this.Exchange.CancelOrder(this.Step[CurrentStep].Order);
                this.CurrentStep--; // Откатываем текущий шаг, т.к. отменили
                // Если не выставлен ни один BUY-ордер отключаем режим DCA
                if (this.CurrentStep == 0)
                {
                    this.IsTriggered = false;
                    Logger.Info(_.Log35); // Режим DCA деактивирован!
                }
            }
        }

        /// <summary>
        /// Проверка статуса BUY-ордера для разрешения на выставление SELL. Если BUY частично исполнен, и
        /// цена уже подрастает, то сделать пересчёт и отменить его.
        /// </summary>
        /// <returns></returns>
        public bool IsBuyFilled()
        {
            // Если усреднёный SELL уже выставлен не надо проверять BUY-ордер уже
            if (this.Step[CurrentStep].SellExposed == true)
                return false;

            Order buyOrder = this.Exchange.GetOrderInfo(this.Step[CurrentStep].Order);
            if (buyOrder.Status == OrderStatus.Filled)
            {
                Logger.Info(_.Log36); // DCA BUY исполнен
                Step[CurrentStep].Filled = buyOrder.Filled;
                // Передаём BUY в пул ордеров на уровень выше
                this.BuyForOrderPool = buyOrder;
                return true; // : Выставляем усреднёный SELL
            }
            if (buyOrder.Status == OrderStatus.PartiallyFilled)
            {
                // Если цена пока ещё близка (на уровень ApproximationPercent%) к TriggerPrice то пока ждём пока дольют
                if (this.CurrentPrice <= this.Step[CurrentStep].TriggerPrice)
                    return false;
                // Иначе отменяем BUY, пересчитываем и ставим уже усреднёный SELL
                buyOrder = this.Exchange.CancelOrder(buyOrder);

                Thread.Sleep(Settings.BetweenRequestTimeout);
                buyOrder = this.Exchange.GetOrderInfo(buyOrder);

                if (buyOrder.Status != OrderStatus.Canceled || buyOrder.Amount == buyOrder.Filled)
                    return false;

                Step[CurrentStep].Filled = buyOrder.Filled;

                this.BuyForOrderPool = buyOrder; // Передаём BUY в пул ордеров на уровень выше

                this.ComputeSteps();

                this.Step[CurrentStep].SellExposed = true;
                CurrentStep--; // Откатываем текущий шаг (т.к. он исполнился не полностью. В NextStep будем его доливать)
                return true; // : Выставляем усреднёный SELL
            }

            return false; // По умолчанию говорим, что BUY-ордер пока не исполнился : SELL пока не ставим
        }
        public bool IsSellFilled() // Для Short
        {
            // Если усреднёный BUY уже выставлен не надо проверять SELL-ордер уже
            if (this.Step[CurrentStep].BuyExposed == true)
                return false;

            Order sellOrder = this.Exchange.GetOrderInfo(this.Step[CurrentStep].Order);
            if (sellOrder.Status == OrderStatus.Filled)
            {
                Logger.Info("DCA SELL исполнен"); // TODO norm text
                Step[CurrentStep].Filled = sellOrder.Filled;
                // Передаём BUY в пул ордеров на уровень выше
                this.SellForOrderPool = sellOrder;
                return true; // : Выставляем усреднёный BUY
            }
            if (sellOrder.Status == OrderStatus.PartiallyFilled)
            {
                // Если цена пока ещё близка (на уровень ApproximationPercent%) к TriggerPrice то пока ждём пока дольют
                if (this.CurrentPrice >= this.Step[CurrentStep].TriggerPrice)
                    return false;
                // Иначе отменяем SELL, пересчитываем и ставим уже усреднёный BUY
                sellOrder = this.Exchange.CancelOrder(sellOrder);

                Thread.Sleep(Settings.BetweenRequestTimeout);
                sellOrder = this.Exchange.GetOrderInfo(sellOrder);

                if (sellOrder.Status != OrderStatus.Canceled || sellOrder.Amount == sellOrder.Filled)
                    return false;

                Step[CurrentStep].Filled = sellOrder.Filled;

                this.SellForOrderPool = sellOrder; // Передаём SELL в пул ордеров на уровень выше

                this.ComputeStepsShort();

                this.Step[CurrentStep].BuyExposed = true;
                CurrentStep--; // Откатываем текущий шаг (т.к. он исполнился не полностью. В NextStep будем его доливать)
                return true; // : Выставляем усреднёный BUY
            }

            return false; // По умолчанию говорим, что SELL-ордер пока не исполнился : BUY пока не ставим
        }

        /// <summary>
        /// Выставление усреднённого SELL-ордера
        /// </summary>
        /// <returns></returns>
        public Order BigSellOrder()
        {
            decimal amount = Trades.GetFullBuyAmount(this.Trades);
            Order bigSellOrder = this.Exchange.OpenLimitOrder(OrderSide.SELL, Config.Cur1, Config.Cur2, amount, this.Step[CurrentStep].SellPrice);
            this.Step[CurrentStep].SellExposed = true; // Отмечаем что SELL по данному шагу выставлен
            return bigSellOrder;
        }
        public Order BigBuyOrder() // Для Short
        {
            decimal amount = Trades.GetFullSellAmount(this.Trades);
            Order bigBuyOrder = this.Exchange.OpenLimitOrder(OrderSide.BUY, Config.Cur1, Config.Cur2, amount, this.Step[CurrentStep].BuyPrice);
            this.Step[CurrentStep].BuyExposed = true; // Отмечаем что BUY по данному шагу выставлен
            return bigBuyOrder;
        }

        /// <summary>
        /// Подсчёт нулевого шага (основы) для DCA на основе всего купленного в рабочем алгоритме.
        /// Этот метот вызывается только до того, как выставлен усреднёный SELL или в пул ордеров уже добавлен частичный DCA BUY-ордер.
        /// !!! Но сейчас ещё и в ситуации резкого выкупа, когда рабочий SELL частично исполнен и частичный DCA BUY был.
        /// </summary>
        public void ComputeZeroStep()
        {
            decimal fullBuyAmount = 0;
            decimal fullBuyCost = 0;
            decimal avgBuyPrice = 0;
            decimal fullSellAmount = 0;
            decimal fullSellCost = 0;
            decimal remainderAmount = 0;

            // Расчитать на основе пула ордеров
            foreach (var buyOrder in this.Trades.BUY)
            {
                fullBuyAmount += buyOrder.Amount;
                fullBuyCost += (buyOrder.Amount * buyOrder.Price);
            }
            avgBuyPrice = fullBuyCost / fullBuyAmount;
            foreach (var sellOrder in this.Trades.SELL)
            {
                fullSellAmount += sellOrder.Amount;
                fullSellCost += (sellOrder.Amount * sellOrder.Price);
            }
            remainderAmount = fullBuyAmount - fullSellAmount;
            
            this.Step[0].BuyPrice = avgBuyPrice;
            this.Step[0].Amount = remainderAmount;
        }
        public void ComputeZeroStepShort()
        {
            decimal fullSellAmount = 0;
            decimal fullSellCost = 0;
            decimal avgSellPrice = 0;
            decimal fullBuyAmount = 0;
            decimal fullBuyCost = 0;
            decimal remainderAmount = 0;

            // Расчитать на основе пула ордеров
            foreach (var sellOrder in this.Trades.SELL)
            {
                fullSellAmount += sellOrder.Amount;
                fullSellCost += (sellOrder.Amount * sellOrder.Price);
            }
            avgSellPrice = fullSellCost / fullSellAmount;
            foreach (var buyOrder in this.Trades.BUY)
            {
                fullBuyAmount += buyOrder.Amount;
                fullBuyCost += (buyOrder.Amount * buyOrder.Price);
            }
            remainderAmount = fullSellAmount - fullBuyAmount;

            this.Step[0].SellPrice = avgSellPrice;
            this.Step[0].Amount = remainderAmount;
        }

        /// <summary>
        /// Метод делает вычисления по шагам DCA и записывает их в this.Step
        /// </summary>
        public void ComputeSteps()
        {
            for (int i = 1; i <= StepCount; i++)
            {
                // Весь "купленный" объем до i шага
                decimal fullBoughtAmount = 0; 
                for (int j = 0; j < i; j++)
                {
                    fullBoughtAmount += this.Step[j].Amount;
                }

                // (BuyPrice) Цена по которой будет выставлен BUY-ордер = this.Step[0].BuyPrice - PriceDropPercent процентов
                this.Step[i].BuyPrice = this.Step[0].BuyPrice - Calc.AmountOfPercent(this.Step[i].PriceDropPercent, this.Step[0].BuyPrice);
                this.Step[i].BuyPrice = Calc.RoundUp(this.Step[i].BuyPrice, this.Exchange.TickSize);

                // (Amount) Объём который следует докупить на данном шаге = AddAmountPercent процентов от fullBoughtAmount
                if (this.Step[i].AddAmountPercent != 0) // Если объем докупки указаны были в процентах. Если нет, то объем уже сразу записан в Amount
                {
                    this.Step[i].Amount = Calc.AmountOfPercent(this.Step[i].AddAmountPercent, this.Step[0].Amount); // TODO TEST тут процент теперь считается от базовогор ордера this.Step[0].Amount
                } else
                {
                    this.Step[i].Amount = Calc.ComputeBuyAmountByBudget(this.Step[i].Amount, this.Step[i].BuyPrice, this.Exchange.StepSize);
                }
                this.Step[i].Amount = Calc.RoundUp(this.Step[i].Amount, this.Exchange.StepSize);
                Logger.ToFile($"[bug1] i{i} Amount {this.Step[i].Amount} Filled {this.Step[i].Filled}"); // TODO DELETE

                // (SellPrice)
                // Расчитываем среднюю цену всего купленного для вычисления SellPrice
                decimal fullBoughtAvgPrice = 0; // Средняя стоимость одной еденицы до текущего шага (включительно)
                decimal fullBoughtCost = 0; // Общая стоимость всего до текущего шага (включительно)
                fullBoughtAmount = 0; // Общий объем всего купленного до текущего шага (включительно)
                for (int j = 0; j <= i; j++)
                {
                    decimal amountForCompute = (this.Step[j].Filled != 0) ? this.Step[j].Filled : this.Step[j].Amount;
                    fullBoughtAmount += amountForCompute;
                    fullBoughtCost += amountForCompute * this.Step[j].BuyPrice;
                    Logger.ToFile($"[bug1] j{j} i{i} amountForCompute {amountForCompute} this.Step[j].Amount {this.Step[j].Amount}, this.Step[j].BuyPrice {this.Step[j].BuyPrice} fullBoughtAmount {fullBoughtAmount}, fullBoughtCost {fullBoughtCost}"); // TODO DELETE
                }
                fullBoughtAvgPrice = fullBoughtCost / fullBoughtAmount;
                Logger.ToFile($"[bug1] i{i} fullBoughtAvgPrice {fullBoughtAvgPrice}"); // TODO DELETE

                // Цена продажи = сред.цена + комиссия в обе стороны + процент профита
                decimal buyFeeRate = 0;
                decimal sellFeeRate = 0;
                if (this.Config.Strategy == Strategy.Scalping)
                {
                    buyFeeRate = this.Exchange.TakerFEE;
                    sellFeeRate = this.Exchange.TakerFEE;
                }
                if (this.Config.Strategy == Strategy.ClassicLong)
                {
                    buyFeeRate = ((Strategies.ClassicLongConfig)this.Config).IsMarketBuy ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
                    sellFeeRate = this.Exchange.TakerFEE; // TODO now sell just by market
                }
                if (this.Config.Strategy == Strategy.ClassicShort)
                {
                    buyFeeRate = ((Strategies.ClassicShortConfig)this.Config).IsMarketBuy ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
                    sellFeeRate = ((Strategies.ClassicShortConfig)this.Config).IsMarketSell ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
                }
                decimal markup = buyFeeRate + sellFeeRate + this.ProfitPercent;
                this.Step[i].SellPrice = fullBoughtAvgPrice + Calc.AmountOfPercent(markup, fullBoughtAvgPrice);
                Logger.ToFile($"[bug1] i{i} SellPrice {this.Step[i].SellPrice}"); // TODO DELETE
                this.Step[i].SellPrice = Calc.RoundUp(this.Step[i].SellPrice, this.Exchange.TickSize);
                Logger.ToFile($"[bug1] i{i} SellPrice {this.Step[i].SellPrice}"); // TODO DELETE

                // (TriggerPrice) Цена, при котороый выставляется BUY данного шага
                decimal approximationInPoint = Calc.AmountOfPercent(this.ApproximationPercent, this.Step[i].BuyPrice);
                Logger.ToFile($"[bug1] this.ApproximationPercent {this.ApproximationPercent} approximationInPoint {approximationInPoint}"); // TODO DELETE
                decimal priceDropInPoint = Calc.AmountOfPercent(this.Step[i].PriceDropPercent, this.Step[0].BuyPrice); // PriceDropPercent рассчитывается от цены покупки в рабочем алгоритме (шаг 0)
                this.Step[i].TriggerPrice = Calc.RoundUp((this.Step[i].BuyPrice /*- priceDropInPoint*/ + approximationInPoint), this.Exchange.TickSize); // Roud сделать вниз
                Logger.ToFile($"[bug1] i{i} this.Step[i].BuyPrice {this.Step[i].BuyPrice} this.Step[i].TriggerPrice {this.Step[i].TriggerPrice} || priceDropInPoint {priceDropInPoint} this.Step[i].PriceDropPercent {this.Step[i].PriceDropPercent} "); // TODO DELETE
            }
        }
        public void ComputeStepsShort()
        {
            for (int i = 1; i <= StepCount; i++)
            {
                // Весь "проданный" объем до i шага
                decimal fullSoldAmount = 0;
                for (int j = 0; j < i; j++)
                {
                    fullSoldAmount += this.Step[j].Amount;
                }
                
                // (SellPrice) Цена по которой будет выставлен SELL-ордер = this.Step[0].SellPrice + PriceDropPercent процентов
                this.Step[i].SellPrice = this.Step[0].SellPrice + Calc.AmountOfPercent(this.Step[i].PriceDropPercent, this.Step[0].SellPrice);
                this.Step[i].SellPrice = Calc.RoundDown(this.Step[i].SellPrice, this.Exchange.TickSize);

                // (Amount) Объём который следует докупить на данном шаге = AddAmountPercent процентов от fullSoldAmount
                if (this.Step[i].AddAmountPercent != 0) // Если объем допродажи указаны были в процентах. Если нет, то объем уже сразу записан в Amount
                {
                    this.Step[i].Amount = Calc.AmountOfPercent(this.Step[i].AddAmountPercent, this.Step[0].Amount); // TODO TEST тут процент теперь считается от базовогор ордера this.Step[0].Amount
                }
                else
                {
                    this.Step[i].Amount = Calc.RoundUp(this.Step[i].Amount, this.Exchange.StepSize);
                }
                this.Step[i].Amount = Calc.RoundDown(this.Step[i].Amount, this.Exchange.StepSize);
                Logger.ToFile($"[bug1] i{i} Amount {this.Step[i].Amount} Filled {this.Step[i].Filled}"); // TODO DELETE

                // (BuyPrice)
                // Расчитываем среднюю цену всего проданного для вычисления BuyPrice
                decimal fullSoldAvgPrice = 0; // Средняя стоимость одной еденицы до текущего шага (включительно)
                decimal fullSoldCost = 0; // Общая стоимость всего до текущего шага (включительно)
                fullSoldAmount = 0; // Общий объем всего проданного до текущего шага (включительно)
                for (int j = 0; j <= i; j++)
                {
                    decimal amountForCompute = (this.Step[j].Filled != 0) ? this.Step[j].Filled : this.Step[j].Amount;
                    fullSoldAmount += amountForCompute;
                    fullSoldCost += amountForCompute * this.Step[j].SellPrice;
                    Logger.ToFile($"[bug1] j{j} i{i} amountForCompute {amountForCompute} this.Step[j].Amount {this.Step[j].Amount}, this.Step[j].SellPrice {this.Step[j].SellPrice} fullSoldAmount {fullSoldAmount}, fullSoldCost {fullSoldCost}"); // TODO DELETE
                }
                fullSoldAvgPrice = fullSoldCost / fullSoldAmount;
                Logger.ToFile($"[bug1] i{i} fullSoldAvgPrice {fullSoldAvgPrice}"); // TODO DELETE

                // Цена покупки = сред.цена + комиссия в обе стороны + процент профита
                decimal buyFeeRate = 0;
                decimal sellFeeRate = 0;
                if (this.Config.Strategy == Strategy.Scalping)
				{
                    buyFeeRate = this.Exchange.TakerFEE;
                    sellFeeRate = this.Exchange.TakerFEE;
                }
                if (this.Config.Strategy == Strategy.ClassicLong)
                {
                    buyFeeRate = ((Strategies.ClassicLongConfig)this.Config).IsMarketBuy ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
                    sellFeeRate = this.Exchange.TakerFEE; // TODO now sell just by market
                }
                if (this.Config.Strategy == Strategy.ClassicShort)
                {
                    buyFeeRate = ((Strategies.ClassicShortConfig)this.Config).IsMarketBuy ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
                    sellFeeRate = ((Strategies.ClassicShortConfig)this.Config).IsMarketSell ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
                }
                decimal markup = buyFeeRate + sellFeeRate + this.ProfitPercent;
                this.Step[i].BuyPrice = fullSoldAvgPrice - Calc.AmountOfPercent(markup, fullSoldAvgPrice);
                Logger.ToFile($"[bug1] i{i} BuyPrice {this.Step[i].BuyPrice}"); // TODO DELETE
                this.Step[i].BuyPrice = Calc.RoundDown(this.Step[i].BuyPrice, this.Exchange.TickSize);
                Logger.ToFile($"[bug1] i{i} BuyPrice {this.Step[i].BuyPrice}"); // TODO DELETE

                // (TriggerPrice) Цена, при котороый выставляется SELL данного шага
                decimal approximationInPoint = Calc.AmountOfPercent(this.ApproximationPercent, this.Step[i].SellPrice);
                decimal priceDropInPoint = Calc.AmountOfPercent(this.Step[i].PriceDropPercent, this.Step[0].SellPrice); // PriceDropPercent рассчитывается от цены продажи в рабочем алгоритме (шаг 0)
                this.Step[i].TriggerPrice = Calc.RoundDown((this.Step[i].SellPrice /*+ priceDropInPoint*/ - approximationInPoint), this.Exchange.TickSize); // Roud сделать вниз
                Logger.ToFile($"[bug1] i{i} this.Step[i].SellPrice {this.Step[i].SellPrice} this.Step[i].TriggerPrice {this.Step[i].TriggerPrice}"); // TODO DELETE
            }
        }

        /// <summary>
        /// Инициализирует фильтры для шагов
        /// </summary>
        /*public void FiltersInit()
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
                    stepFilters.Add(FilterManager.GetFilterObjectByBtnPlusId(filterSettings, "BUY")); // TODO check 5131 BUY, потому что DCA для Лонга = надо смотреть на ask
                }
                this.DCAStepFilters[i] = new List<IFilter>(stepFilters);

                // Получить все нужные типы данных
                DM.GetRequiredData(this.DCAStepFilters[i]);
                // TODO в будущем лучше наверно сделать DM для каждого шага, чтобы запрашивать меньше данных при шагах
            }
            
            DM.SetExchange(this.Exchange); // Передаем биржу
            DM.Config = this.Config;
        }*/

        /// <summary>
        /// Проверка по фильтрам разовая
        /// </summary>
        // TODO move filters from DCA
        /*private bool CheckFilters(List<IFilter> filters, int targetPoint)
        {
            Logger.ToFile(_.Log46); // Проверка по фильтрам...

            this.CurrentWeigth = 0;
            // Получить все данные
            this.DM.GetData(filters);

            foreach (var filter in filters)
            {
                // Отдаём данные фильтру
                filter.DataMiner = this.DM;

                this.CurrentWeigth += filter.CurrentWeight;

                if (this.CurrentWeigth >= targetPoint)
                    return true;
            }

            return false;
        }*/

        /// <summary>
        /// Объеденяет ComputeZeroStep() и ComputeSteps() чтобы не писать 2 сточки
        /// </summary>
        public void Compute()
        {
            Logger.ToFile("Computing... ");

            if (this.DCAFilters.Count > 0) // Инициализация фильтров, если они есть
                this.Filters.DcaFiltersInit(this.StepCount, this.Exchange, this.Config.Cur1, this.Config.Cur2); 

            Logger.ToFile("Computing DCA steps...");
            this.ComputeZeroStep();
            this.ComputeSteps();
        }
        public void ComputeShort()
        {
            Logger.ToFile("Computing short DCA... ");

            if (this.DCAFilters.Count > 0) // Инициализация фильтров, если они есть
                this.Filters.DcaFiltersInit(this.StepCount, this.Exchange, this.Config.Cur1, this.Config.Cur2);

            Logger.ToFile("Computing DCA steps...");
            this.ComputeZeroStepShort();
            this.ComputeStepsShort();
        }

		public void Clear()
		{
            // TODO TESTING... в конце итерацию проверяем обнуляется ли шаг

            // Check and cancel all DCA orders
            if (this.IsTriggered)
            {
                Logger.ToFile("DCA was triggered");

                for (int i = 0; i < this.CurrentStep; i++)
                {
                    if (this.Step[i].Order.Id == null)
					{
                        Logger.ToFile("Order id was null - continue");
                        continue;
                    }

                    Order order = this.Exchange.GetOrderInfo(this.Step[i].Order);

                    if (order.Status == OrderStatus.New || order.Status == OrderStatus.PartiallyFilled)
					{
                        Logger.Info(_.Log48); // Отменяем DCA-ордер, т.к. основной ордер исполнен

                        Order cancel = this.Exchange.CancelOrder(this.Step[i].Order);
                    }
				}
			}

            this.CurrentStep = 0; 
			this.IsTriggered = false;

            Logger.ToFile("DCA creared");
        }

    }
}
