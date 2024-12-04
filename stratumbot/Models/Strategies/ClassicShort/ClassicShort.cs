using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using stratumbot.Models.Tools;
using stratumbot.Models.Tools.Reminder;
using System;
using System.Collections.Generic;
using System.Threading;

namespace stratumbot.Models.Strategies
{
	class ClassicShort : IStrategy, IDCAble
    {
        #region fields

        /// <summary>
        /// Token for stop the thread
        /// </summary>
        public CancellationTokenSource ts { get; set; }
        public CancellationToken cancellationToken { get; set; }

        /// <summary>
        /// Strategy configuration
        /// </summary>
        public ClassicShortConfig configuration { get; set; }

        /// <summary>
        /// Current exchange
        /// </summary>
        public IExchange Exchange { get; set; }

        /// <summary>
		///  Counters
		/// </summary>
        public int BuyCounter { get; set; } = 0;
        public int SellCounter { get; set; } = 0;

        /// <summary>
        /// Object of DCA class for trading with DCA
        /// </summary>
        public DCA DCA;

        /// <summary>
		/// List of remidnder orders with problems
		/// </summary>
		private List<ReminderItem> Reminder { get; set; }

        /// <summary>
		/// Budget (reminder) for the next iteration for bad situations
		/// </summary>
		private decimal? BudgetForNextIteration { get; set; }

        /// <summary>
        /// Buy price of current iteration
        /// </summary>
        public decimal BuyPrice { get; set; }

        /// <summary>
        /// Sell price of current iteration
        /// </summary>
        public decimal SellPrice { get; set; }

        /// <summary>
        /// Amount of current iteration
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
		/// Min ask price that was reached (for trailing profit)
		/// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
		/// Stoploss trigger price, if stoploss as market this value = StopLossPrice
		/// </summary>
		public decimal StopLossTriggerPrice { get; set; }

        /// <summary>
        /// Is stoploss was placed
        /// </summary>
        public bool IsStopLossTriggered { get; set; }

        /// <summary>
        /// Time when stoploss will be placed
        /// </summary>
        public decimal StopLossTime { get; set; }

        /// <summary>
		/// Priority buy price (used in profit trailing at least)
		/// </summary>
        public decimal? BuyPricePriority { get; set; }

        /// <summary>
        /// Pool of order of this iteration
        /// </summary>
        public Trades Trades { get; set; }

        /// <summary>
        /// Object for profit computing 
        /// </summary>
        public Profit Profit { get; set; }

        /// <summary>
		/// DCA step was changed event 
		/// </summary>
		public event DCAStepChangedDelegate DCAStepChangedEvent;

        #endregion

        /// <summary>
        /// [refactored] Classic Short Constructor
        /// </summary>
        public ClassicShort(ClassicShortConfig config, Exchange exchange)
		{
			// Set strategy config
			this.configuration = config;

			// Set current exchange
			this.Exchange = AvailableExchanges.CreateExchangeById(exchange);

			// DCA init
			this.NewDCAInit();

			// Init buy filter
			this.FiltersInit();

            // Reminder list init
            this.Reminder = new List<ReminderItem>();

            this.BudgetForNextIteration = null;
        }

        /// <summary>
		/// [refactored] Main trading algorithm. 
		/// 
		/// This is a one single deal: 
		/// preparing vars, checking, selling, preparing, buying, profit computing
		/// </summary>
		public void Trade()
		{
			this.SetNullAndDefaultValues();

			this.Selling();

			if (this.cancellationToken.IsCancellationRequested) { throw new ManuallyStopException(_.Log19); }

			this.AfterSoldProcessing();

            this.Buying();

            this.AfterBoughtProcessing();
		}

        /// <summary>
		/// [refactored] Stop the trading
		/// </summary>
        public void StopTrade()
		{
			if (this.ts != null) this.ts.Cancel();
			Logger.ToFile("Classic Short : Stop Trade ()"); // TODO del
		}

        /// <summary>
		/// Event DCAStepChangedEvent handler — "DCA step was changed event"
		/// </summary>
		/// <param name="stepNum"></param>
		public void DCAStepChangedHandler(int stepNum)
        {
            //this.DCAStep = _stepNum;
            DCAStepChangedEvent?.Invoke(stepNum);
        }

        // ***************************************************************************************************************
        // Init variables

        /// <summary>
		/// [refactored] Set the default values and nullify the vars before each iteration
		/// </summary>
        private void SetNullAndDefaultValues()
		{
            // TODO with this three fields - maybe stoploss move to sl class. 
            // OrderPoll beacuse of stat bug should be rewrited maybe
            this.StopLossTime = 0;
            this.IsStopLossTriggered = false;
            this.Trades = new Trades();
            
            // Set counters to zero
            this.BuyCounter = 0;
            this.SellCounter = 0;
            
            // DCA re init
            this.NewDCAInit();

            // Compute percentage budget
            this.PercentageBudgetCompute();

            this.MinPrice = 99999999;

            // TODO Times
            TThreadInfo.Times[Core.TID.CurrentID].StartIteration = -1;
        }

        /// <summary>
		/// [refactored] Get amount of budget if it set as percent
		/// </summary>
        private void PercentageBudgetCompute()
        {
            if (this.configuration.IsBudgetAsPercent)
            {
                decimal balance = this.Exchange.GetBalance(this.configuration.Cur1);
                this.configuration.Budget = Calc.AmountOfPercent(this.configuration.BudgetAsPercent, balance);
            }
        }

        /// <summary>
        /// [refactored] Create a new DCA object
        /// </summary>
        private void NewDCAInit()
        {
            if (this.configuration.IsDCA)
            {
                this.DCA = new DCA(this.configuration, this.Exchange);
                this.DCA.Filters = this.configuration.Filters;
                // Subscribe to event
                this.DCA.DCAStepChangedEvent += DCAStepChangedHandler;
                // TODO также надо отписаться где то
            }
        }

        /// <summary>
		/// [refactored] Filters initialization (buy, sell, stoploss)
		/// </summary>
        private void FiltersInit()
        {
            // TODO move init somewhere?
            if (this.configuration.IConfigText.FiltersBuy.Count > 0)
            {
                this.configuration.Filters.BuyFiltersInit(this.configuration.IConfigText.FiltersBuy, this.Exchange, this.configuration);
            }

            if (this.configuration.IConfigText.FiltersSell.Count > 0)
            {
                this.configuration.Filters.SellFiltersInit(this.configuration.IConfigText.FiltersSell, this.Exchange, this.configuration);
            }

            if (this.configuration.IConfigText.FiltersStopLoss.Count > 0)
            {
                this.configuration.Filters.StopLossSellFiltersInit(this.configuration.IConfigText.FiltersStopLoss, this.Exchange, this.configuration);
            }
        }


        // ***************************************************************************************************************
        // Selling

        /// <summary>
		/// [refactored] Selling process
		/// </summary>
        private void Selling()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                // Looking for filters allowance
                if (this.configuration.Filters.SellFilters.Count > 0)
                {
                    bool isFiltersAllowed = this.configuration.Filters.FiltersMonitoring(this.configuration.Filters.SellFilters, (this.configuration.Filters.TargetPointSell));
                    if (!isFiltersAllowed)
                    {
                        Thread.Sleep(Settings.FiltersTimeout);
                        continue;
                    }
                }

                Order sellOrder = new Order();

                if (this.configuration.IsMarketSell)
                {
                    // Check if first 10 orders enought to sell by market right now
                    if (!IsBidsAmountEnough())
                        continue; // TODO FUTURE это можно проверить перед фильтрами, чтобы если не первый ордер будем забирать проверить его цену по фильтрам

                    // The amount is equial to the budget, because the budget set in the first currency
                    this.Amount = this.BudgetForNextIteration ?? this.configuration.Budget;

                    // [loop] SELL
                    sellOrder = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

                    this.SellPrice = sellOrder.Price;
                }
                else // Limit order
                {
                    // SellPrice = last-1
                    var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);
                    decimal next = doms["asks"][0].Price - Exchange.TickSize;
                    this.SellPrice = doms["bids"][0].Price != next ? next : doms["asks"][0].Price;

                    // The amount is equial to the budget, because the budget set in the first currency
                    this.Amount = this.BudgetForNextIteration ?? this.configuration.Budget;

                    // [loop] SELL
                    sellOrder = this.Exchange.OpenLimitOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.SellPrice);
                }

                // Set the sell counter up
                this.SellCounter++;

                // TODO Timer Times.LastSellOrder
                TThreadInfo.Times[Core.TID.CurrentID].LastSellOrder = -1;

                Thread.Sleep(Settings.BetweenRequestTimeout);

                // [loop] Check the SELL order
                if (IsSELLFilled(sellOrder)) break;// SELL-order was filled
                else continue; // Goto to re-place with new markup/price
            }
        }

        /// <summary>
        /// [refactored] Check if ask volume of first 10 orders is enough to filled our market order
        /// </summary>
        /// <returns></returns>
        public bool IsBidsAmountEnough()
        {
            this.SellPrice = 0;
            decimal haveAmount = 0;
            var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 10);
            foreach (var bid in doms["bids"])
            {
                haveAmount += bid.Amount;

                if (haveAmount > (this.BudgetForNextIteration ?? this.configuration.Budget))
                {
                    this.SellPrice = bid.Price;
                    return true;
                }
            }

            Logger.Info(_.Log54); // Первых 10 ордеров не хватает чтобы заполнить ордер

            return false;
        }

        /// <summary>
        /// [refactored] Actions by setting if SELL order was canceled
        /// 
        /// true - filled amount > minimals - continue the iteratin - place the BUY order
		/// false - no filled amount - start a new iteration
		/// TinyOrderCanceledException - filled amount less than minimals - will buy it next iteratin - start a new iteration
		/// AutoStopException - stop the thread by setting (filled amount less than minimals)
        /// </summary>
        /// <param name="sellOrder"></param>
        /// <returns></returns>
        /// <exception cref="AutoStopException"></exception>
        private bool CanceledAction(Order sellOrder)
		{
            // Just start to looking for new trade-in this.Selling();
            if (sellOrder.Filled == 0) return false;

            if (sellOrder.Filled > this.Exchange.MinAmount && sellOrder.Filled * this.GetMinProfitBuyPrice() > this.Exchange.MinCost)
            {
                Logger.Info("SELL частично исполнен и отменён, ставим ордер на покупку"); // TODO norm text

                this.Trades.AddOrder(sellOrder);

                return true; // SELL filled : place BUY order
            }

            // else

            // Actions by setting
            this.SellOrderCanceledActionBySettings(sellOrder);

            throw new AutoStopException("Поток остановлен из-за отсутствия настроек"); // TODO norm text
        }

        /// <summary>
        /// [refactored] Actions by setting when [10] Sell Canceled Situation
        /// </summary>
        /// <param name="sellOrder"></param>
        /// <exception cref="AutoStopException"></exception>
        /// <exception cref="TinyOrderCanceledException"></exception>
        private void SellOrderCanceledActionBySettings(Order sellOrder)
		{
            // >>>	SELL order canceled
            // (1)	Stop
            // (2)	Start a new iteration with the reminder budget (filled part buy later) OR Stop
            // (3)	Start a new iteration with the full budget (filled part buy later) OR Stop
            // (4)	Start a new iteration with the reminder budget (filled part buy later) OR
            //		Start a new iteration with the full budget (filled part buy later) OR Stop

            // (1)
            if (Settings.SellCanceledClassicShortSituation == 0)
            {
                throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
            }

            this.Trades.AddOrder(sellOrder);

            // Get DOMs
            var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

            // Compute reminder budget
            decimal reminderBudget = this.configuration.Budget - sellOrder.Filled;

            // Get free balance 
            decimal balance = this.Exchange.GetBalance(this.configuration.Cur1);

            // (2)
            if (Settings.SellCanceledClassicShortSituation == 1)
			{
                if (reminderBudget > this.Exchange.MinAmount && Calc.RoundDown(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinCost)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("SELL order canceled - (2) action");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            // (3)
            if (Settings.SellCanceledClassicShortSituation == 2)
            {
                if (balance > this.configuration.Budget)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

                    throw new TinyOrderCanceledException("SELL order canceled - (3) action");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            // (4)
            if (Settings.SellCanceledClassicShortSituation == 3)
            {
                if (reminderBudget > this.Exchange.MinAmount && Calc.RoundDown(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinCost)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("SELL order canceled - (4) action [1]");
                }
                else if (balance > this.configuration.Budget)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

                    throw new TinyOrderCanceledException("SELL order canceled - (4) action [2]");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            throw new AutoStopException("Поток остановлен из-за отсутствия настроек"); // TODO norm text
        }

        /// <summary>
        /// [refactored] Check four conditions for buy
		/// true - if it allows to do actions by setting
		/// false - if actions are not allowed, just continue to check the order
        /// </summary>
        /// <param name="sellOrder"></param>
        /// <param name="xOrdersAhead"></param>
        /// <param name="secondsAfterLastUpdate"></param>
        /// <param name="dropPercent"></param>
        /// <param name="aheadOrdersVolume"></param>
        /// <returns></returns>
        // TODO merge from all class
        public bool IsXthFourSellConditions(Order sellOrder, int xOrdersAhead, int secondsAfterLastUpdate, decimal dropPercent, decimal aheadOrdersVolume)
		{
            if (xOrdersAhead == 0 && secondsAfterLastUpdate == 0 && dropPercent == 0 && aheadOrdersVolume == 0) return false;

            if (xOrdersAhead != 0)
            {
                // Get DOMs
                var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, xOrdersAhead);

                if (doms["asks"][xOrdersAhead - 1].Price >= sellOrder.Price) return false; // We ahead(or on) the x order
            }

            if (secondsAfterLastUpdate != 0)
            {
                decimal currentTime = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (Decimal.Parse(sellOrder.Time) + secondsAfterLastUpdate > currentTime) return false; // Time is not up
            }

            if (dropPercent != 0)
            {
                decimal triggerPrice = sellOrder.Price - (Calc.AmountOfPercent(dropPercent, sellOrder.Price));

                // Get DOMs
                var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 1);

                if (doms["asks"][0].Price > triggerPrice) return false; // Curreent price is more than sellPrice-x%
            }

            if (aheadOrdersVolume != 0)
            {
                // Get DOMs
                var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 50);

                decimal amount = 0;

                foreach (var ask in doms["asks"])
                {
                    if (ask.Price < sellOrder.Price) amount += ask.Amount; // Add volume of above orders only
                    else break;
                }

                if (amount < Calc.AmountOfPercent(aheadOrdersVolume, sellOrder.Amount)) return false; // Volume is less than x% of our
            }

            return true;
        }

        /// <summary>
        /// [refactored] [15] Four waiting conditions for [11] Sell Little Filled Price Dropped Situation
        /// </summary>
        /// <param name="sellOrder"></param>
        /// <returns></returns>
        private bool Is15thFourConditions(Order sellOrder)
		{
            return this.IsXthFourSellConditions
                (
                    sellOrder,
                    Settings.XOrdersAheadLittleFilledPriceDroppedClassicShortSituation,
                    Settings.SecondsAfterLastUpdateLittleFilledPriceDroppedClassicShortSituation,
                    Settings.DropPercentLittleFilledPriceDroppedClassicShortSituation,
                    Settings.AheadOrdersVolumeLittleFilledPriceDroppedClassicShortSituation
                );
        }

        /// <summary>
        /// [refactored] [16] Four waiting conditions for [17] Sell Filled Enough Price Dropped
        /// </summary>
        /// <param name="sellOrder"></param>
        /// <returns></returns>
        private bool Is16thFourConditions(Order sellOrder)
		{
            return this.IsXthFourSellConditions
                (
                    sellOrder,
                    Settings.XOrdersAheadSellFilledEnoughPriceDropped,
                    Settings.SecondsAfterLastUpdateSellFilledEnoughPriceDropped,
                    Settings.DropPercentSellFilledEnoughPriceDropped,
                    Settings.AheadOrdersVolumeSellFilledEnoughPriceDropped
                );
        }

        /// <summary>
		/// [refactored] Actions by settings for [11] Sell Little Filled Price Dropped Situation
		/// </summary>
		/// <param name="sellOrder"></param>
		private void SellLittleFilledPriceDroppedActionsBySettings(Order sellOrder)
		{
            // >>>	Sell order little filled and price gone down
            // (1)	Wait
            // (2)	Start a new iteration with the reminder budget (filled part buy later), cancel current one OR Wait
            // (3)	Start a new iteration with the full budget (filled part buy later), cancel current one OR Wait
            // (4)	Start a new iteration with the reminder budget (filled part buy later), cancel current one OR
            //		Start a new iteration with the full budget (filled part buy later), cancel current one OR Wait
            // (5)	Start a new iteration with the full budget (filled part buy later), cancel current one OR
            //		Start a new iteration with the reminder budget (filled part buy later), cancel current one OR Wait

            // Get DOMs
            var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

            // Compute reminder budget
            decimal reminderBudget = this.configuration.Budget - sellOrder.Filled;

            // Get free balance 
            decimal balance = this.Exchange.GetBalance(this.configuration.Cur1);

            bool isFreeBalanceGreaterFullBudget() { return balance > this.configuration.Budget; };
            bool isFreeBalanceGreaterReminderBudget() { return balance > reminderBudget; };
            bool isReminderBudgetGreaterMinimals() { return reminderBudget > this.Exchange.MinCost && Calc.RoundDown(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount; };
            void newIterationWithReminderBudget()
            {
                try
                {
                    sellOrder = this.Exchange.CancelOrder(sellOrder);

                    this.Trades.AddOrder(sellOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("SELL order canceled - (2) action");
                }
                catch (OrderFilledWhileWeCancelingException ex)
                {
                    Logger.Info(_.Log17); // Ордер исполнился пока мы его отменяли
                    // (1) Wait
                }
            }
            void newIterationWithFullBudget()
            {
                try
                {
                    sellOrder = this.Exchange.CancelOrder(sellOrder);

                    this.Trades.AddOrder(sellOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

                    throw new TinyOrderCanceledException("SELL order canceled - (3) action");
                }
                catch (OrderFilledWhileWeCancelingException ex)
                {
                    Logger.Info(_.Log17); // Ордер исполнился пока мы его отменяли
                    // (1) Wait
                }
            }

            // (1)
            // Just nothing to do, let method to finish

            // (2)
            if (Settings.SellLittleFilledPriceDroppedClassicShortSituation == 1)
            {
                if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
                // else (1) Wait
            }

            // (3)
            if (Settings.SellLittleFilledPriceDroppedClassicShortSituation == 2)
            {
                if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
                // else (1) Wait
            }

            // (4)
            if (Settings.SellLittleFilledPriceDroppedClassicShortSituation == 3)
            {
                if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
                else if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
                // else (1) Wait 
            }

            // (5)
            if (Settings.SellLittleFilledPriceDroppedClassicShortSituation == 4)
            {
                if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
                else if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
                // else (1) Wait 
            }

        }

        /// <summary>
        /// [refactored] Провера SELL-оредара на исполнение. 
        /// True, если ордер исполнен (Завершить итерацию).
        /// False, если нужно выставить новый ордер.
        /// </summary>
        private bool IsSELLFilled(Order _sellOrder)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                Logger.Info(_.Log20); // Проверяем SELL...

                // Get order info
                _sellOrder = Exchange.GetOrderInfo(_sellOrder);

                // TODO надо ли рассчитать AVG sell цену т.к. по разной цене исполнися

                if (_sellOrder.Status == OrderStatus.Filled)
                {
                    Logger.Info(_.Log51); // SELL исполнен! Ставим ордер на покупку...

                    this.Trades.AddOrder(_sellOrder);

                    return true; // SELL filled : place BUY order

                }
                if (_sellOrder.Status == OrderStatus.Canceled)
                {
                    return this.CanceledAction(_sellOrder);
                }
                if (_sellOrder.Status == OrderStatus.PartiallyFilled)
                {
                    Logger.Info(_.Log13); // Ордер частично исполнен

                    // Get DOMs
                    var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

                    // TODO TEST what if there is no filters
                    bool isFiltersAllowed = this.configuration.Filters.CheckFilters(this.configuration.Filters.SellFilters, this.configuration.Filters.TargetPointSell);
                    bool isReminderMoreMinimals = (_sellOrder.Remainder > this.Exchange.MinAmount && (_sellOrder.Remainder * (doms["asks"][0].Price - this.Exchange.TickSize)) > this.Exchange.MinCost);
                    bool isFilledAmountMoreMinimals = (_sellOrder.Filled > this.Exchange.MinAmount && (_sellOrder.Filled * (doms["bids"][0].Price + this.Exchange.TickSize)) > this.Exchange.MinCost);

                    // TODO check the orders volume as in Scalping
                    // (1) our order is first: price of first bid = our order price
                    bool isOrderFirst = doms["asks"][0].Price == _sellOrder.Price;
                    if (!isOrderFirst)
					{
                        Logger.Info($"{_.Log23} {_sellOrder.Price} > {doms["asks"][0].Price} [1]");

                        if (isFilledAmountMoreMinimals)
                        {
                            if (!this.Is16thFourConditions(_sellOrder)) continue;

                            try
                            {
                                _sellOrder = this.Exchange.CancelOrder(_sellOrder);

                                this.Trades.AddOrder(_sellOrder);

                                return true; // order was filled : finish the iteration
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.Info(_.Log17); // Ордер исполнился пока мы его отменяли
                                continue;
                            }
                        }
                        else
                        {
                            // TODO сделать чтоб можно было запускать без фильтров тоже
                            if (this.configuration.IConfigText.FiltersSell.Count == 0)
                            {
                                Logger.Error("code 38");
                                continue;
                            }

                            // We can place a new order, but for now it doesnt matter, filters are not allow to buy anyway
                            if (!isFiltersAllowed) continue;

                            // [6] Four waiting conditions for [2] Buy Little Filled Price Increased Situation
                            if (!this.Is15thFourConditions(_sellOrder)) continue;

                            // Action by setting
                            this.SellLittleFilledPriceDroppedActionsBySettings(_sellOrder);

                        }
                    }

                    // (2) our order too high above second one - inefficient price
                    bool isBackTrailingNeeded = isOrderFirst && doms["asks"][1].Price != (_sellOrder.Price + this.Exchange.TickSize);
                    if (isBackTrailingNeeded)
                    {
                        Logger.Info($"{_.Log23} ({_sellOrder.Price} + {this.Exchange.TickSize}) ≠ {doms["asks"][1].Price}");

                        // Re-place a new SELL order for the reminder amount

                        if (isReminderMoreMinimals /* ?? isFiltersAllowed */)
                        {
                            Logger.ToFile($"TP 1 2"); // TODO DELETE

                            try
                            {
                                _sellOrder = this.Exchange.CancelOrder(_sellOrder);

                                // TODO do we really need check again?
                                _sellOrder = this.Exchange.GetOrderInfo(_sellOrder);

                                // Remember the filled part
                                this.Trades.AddOrder(_sellOrder);

                                if (_sellOrder.Status == OrderStatus.Filled) return true; // Place BUY order

                                isReminderMoreMinimals = (_sellOrder.Remainder > this.Exchange.MinAmount && (_sellOrder.Remainder * (doms["asks"][0].Price - this.Exchange.TickSize)) > this.Exchange.MinCost);

                                if (isReminderMoreMinimals)
                                {
                                    // Reminder amount
                                    this.Amount = _sellOrder.Remainder;

                                    return false; // Current sell doesnt filled, we will place a new one for the reminder amount
                                }
                                else
                                {
                                    isFilledAmountMoreMinimals = (_sellOrder.Filled > this.Exchange.MinAmount && (_sellOrder.Filled * (doms["bids"][0].Price + this.Exchange.TickSize)) > this.Exchange.MinCost);

                                    if (isFilledAmountMoreMinimals) return true; // Place SELL order for filled amount
                                    else this.SellOrderCanceledActionBySettings(_sellOrder);
                                }
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[3]" + ex.Message);
                                Logger.Info(_.Log17);
                                continue;
							}
                        }
                    }

                }
                if (_sellOrder.Status == OrderStatus.New)
                {
                    // Get DOMs
                    var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

                    // TODO check the orders volume as in Scalping
                    // (1) our order is first: price of first bid = our order price
                    bool isOrderFirst = doms["asks"][0].Price == _sellOrder.Price;

                    // (2) our order too high above second one - inefficient price
                    bool isBackTrailingNeeded = isOrderFirst && doms["asks"][1].Price != (_sellOrder.Price + this.Exchange.TickSize);


                    if (!isOrderFirst || isBackTrailingNeeded)
                    {
                        if (!isOrderFirst) Logger.Info($"{_.Log23} {_sellOrder.Price} > {doms["asks"][0].Price} [1]");
                        else Logger.Info($"{_.Log23} ({_sellOrder.Price} - {this.Exchange.TickSize}) ≠ {doms["asks"][1].Price}");

                        // TODO Does it really helpful to check again?
                        _sellOrder = this.Exchange.GetOrderInfo(_sellOrder);

                        if (_sellOrder.Status == OrderStatus.Filled || _sellOrder.Status == OrderStatus.PartiallyFilled || _sellOrder.Status == OrderStatus.Canceled)
                        {
                            continue; // Continue to check buy
                        }

                        try
                        {
                            _sellOrder = this.Exchange.CancelOrder(_sellOrder);

                            this.Trades.AddOrder(_sellOrder);

                            return this.CanceledAction(_sellOrder);
                        }
                        catch (OrderFilledWhileWeCancelingException ex)
                        {
                            Logger.ToFile("[2]" + ex.Message);
                            Logger.Info(_.Log17);
                            continue;
                        }
                    }
                }

                Thread.Sleep(Settings.CheckOrderTimeout);
            }


            throw new ManuallyStopException(_.Log19); // Остановился!
        }

        /// <summary>
		/// [refactored] Check if StopLoss timer timedout
		/// </summary>
		/// <returns></returns>
        private bool IsStopLossTimerTimedOut()
        {
            if (this.StopLossTime == 0 && Settings.StopLossTimeout == 0) return true;

            decimal currentTime = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            if (this.StopLossTime == 0 && Settings.StopLossTimeout > 0)
            {
                Logger.Info(_.Log50); // Стоплосс таймер запущен
                this.StopLossTime = currentTime + Settings.StopLossTimeout;
            }

            return currentTime >= this.StopLossTime;
        }


        // ***************************************************************************************************************
        // After sold processing

        /// <summary>
		/// [refactored] Sell price and amount recomputing, computing stoploss and dca
		/// </summary>
        private void AfterSoldProcessing()
        {
            // Recomputing sell price and amount in case if there are multiple orders filled
            this.SellPrice = Trades.GetAvgSellPrice(this.Trades);
            this.Amount = Trades.GetFullSellAmount(this.Trades);

            // StopLoss computing
            if (this.configuration.IsStopLoss)
            {
                Logger.ToFile($"StopLoss computing...");

                // (1) Percent of price increasing [%]
                if (this.configuration.IsStopLossAsPercent)
                {
                    this.configuration.StopLossPrice = this.SellPrice + Calc.AmountOfPercent(this.configuration.StopLossPercent, this.SellPrice);
                    this.configuration.StopLossPrice = Calc.RoundDown(this.configuration.StopLossPrice, this.Exchange.TickSize);
                    Logger.ToFile($"StopLoss (1) {this.configuration.StopLossPrice} {this.configuration.StopLossPercent} {this.SellPrice}");
                }
                // (2) Minus pints of sell price [-]
                if (this.configuration.IsStopLossAsMinusPoints)
                {
                    this.configuration.StopLossPrice = this.SellPrice - this.configuration.StopLossMinus; // - наверно
                    this.configuration.StopLossPrice = Calc.RoundDown(this.configuration.StopLossPrice, this.Exchange.TickSize);
                    Logger.ToFile($"StopLoss (2) {this.configuration.StopLossPrice} {this.configuration.StopLossMinus} {this.SellPrice}");
                }
                // (3) Just inform: Specific price set [!% !-]
                if (!this.configuration.IsStopLossAsPercent && !this.configuration.IsStopLossAsMinusPoints)
                {
                    Logger.ToFile($"StopLoss (3) {this.configuration.StopLossPrice}");
                }

                // (4) Just inform: Filters
                if (this.configuration.Filters.StopLossFilters.Count > 0)
                {
                    Logger.ToFile($"StopLoss (4) {this.configuration.StopLossPrice} + filters");
                }


                // StopLoss as limit order
                if (this.configuration.StopLossApproximation != 0)
                {
                    // StopLoss trigger price computing...
                    if (this.configuration.IsStopLossApproximationAsPercent)
                    {
                        this.StopLossTriggerPrice = this.configuration.StopLossPrice - Calc.AmountOfPercent(this.configuration.StopLossApproximation, this.configuration.StopLossPrice);
                        Logger.ToFile($"StopLoss l(4)% {this.StopLossTriggerPrice} {this.configuration.StopLossPrice} {this.configuration.StopLossApproximation}");
                    }
                    else // as points
                    {
                        this.StopLossTriggerPrice = this.configuration.StopLossPrice - this.configuration.StopLossApproximation;
                        Logger.ToFile($"StopLoss l(4). {this.StopLossTriggerPrice} {this.configuration.StopLossPrice} {this.configuration.StopLossApproximation}");
                    }
                }
                // StopLoss as market order
                else
                {
                    this.StopLossTriggerPrice = this.configuration.StopLossPrice; // Trigger price just equial to StopLoss price itself
                    Logger.ToFile($"StopLoss m(4) {this.StopLossTriggerPrice} {this.configuration.StopLossPrice} {this.configuration.StopLossApproximation}");
                }

            }

            // DCA compute
            if (this.configuration.IsDCA)
            {
                this.DCA.Trades = this.Trades;
                this.DCA.ComputeShort();
            }
        }


        // ***************************************************************************************************************
        // Buying

        /// <summary>
        /// [refactored] Buying process
        /// </summary>
        private void Buying()
        {
            decimal GetAmount()
			{
                if (this.Reminder.Count > 0) return this.Amount + ReminderHelper.GetTotalReminderAmountForBuy(this.Reminder);
                else return this.Amount;
            }

            // Get actual sell price, depend on if there is a reminder
            decimal GetBuyPrice()
            {
                if (this.Reminder.Count > 0)
                {
                    decimal sellPriceWithAdditionalAmount = ReminderHelper.GetAvgPriceWithAdditionalAmountForBuy(this.Reminder, this.Amount, this.SellPrice);

                    // SetBuyPrice - for profit trailing; ?? first limit order by min.profit
                    this.BuyPrice = /*this.SetBuyPrice() ??*/ this.GetMinProfitBuyPrice(sellPriceWithAdditionalAmount);
                    this.BuyPrice = Calc.RoundDown(this.BuyPrice, this.Exchange.TickSize);

                    return this.BuyPrice;
                }
                else
                {
                    if (this.BuyPricePriority != null)
                    {
                        this.BuyPrice = (decimal)this.BuyPricePriority;
                        this.BuyPricePriority = null;
                        return this.BuyPrice;
                    }
                    // SetBuyPrice - for profit trailing; ?? first limit order by min.profit
                    this.BuyPrice = /*this.SetBuyPrice() ??*/ this.GetMinProfitBuyPrice();
                    this.BuyPrice = Calc.RoundDown(this.BuyPrice, this.Exchange.TickSize);

                    return this.BuyPrice;
                };
            }

            while (!this.cancellationToken.IsCancellationRequested)
            {
                Order buyOrder;

                // Buy by filters
                if (this.configuration.IConfigText.FiltersBuy.Count > 0)
                {
                    // Looking for filters allowance
                    bool isFiltersAllowed = this.configuration.Filters.FiltersMonitoring(this.configuration.Filters.BuyFilters, (this.configuration.Filters.TargetPointBuy));
                    if (!isFiltersAllowed)
                    {
                        Thread.Sleep(Settings.FiltersTimeout);
                        continue;
                    }

                    // Check if first 10 orders enought to get min profit if filled by market right now
                    if (!this.IsAsksAmountEnough()) // TODO FUTURE это можно проверить перед фильтрами, чтобы если не первый ордер будем забирать проверить его цену по фильтрам
                        continue;

                    // [loop] BUY
                    buyOrder = this.Exchange.OpenMarketOrder(
                        OrderSide.BUY,
                        this.configuration.Cur1,
                        this.configuration.Cur2,
                        GetAmount()
                    );
                }
                // Buy by profit trailing or just by min.profit
                else
                {
                    // SetBuyPrice - for profit trailing; ?? first limit order by min.profit
                    this.BuyPrice = /*this.SetBuyPrice() ??*/ this.GetMinProfitBuyPrice();
                    this.BuyPrice = Calc.RoundDown(this.BuyPrice, this.Exchange.TickSize);

                    // [loop] BUY
                    buyOrder = this.Exchange.OpenLimitOrder(
                        OrderSide.BUY, 
                        this.configuration.Cur1,
                        this.configuration.Cur2,
                        GetAmount(),
                        GetBuyPrice()
                    );
                }

                // Set the buy counter up
                this.BuyCounter++;

                // TODO Timer Times.LastBuyOrder = -1;
                TThreadInfo.Times[Core.TID.CurrentID].LastBuyOrder = -1;

                Thread.Sleep(Settings.BetweenRequestTimeout);

                // [loop] Check the BUY order (sometimes DCA BUY order)
                if (this.IsBUYFilled(buyOrder)) break; // BUY-ордер исполнен
                else continue; // Иначе идём выставлять BUY-ордер с новой ценой/количеством
            }
        }

        /// <summary>
		/// [refactored] Get prioritized price, if it was set somewhere (in StopLoss it will be)
		/// </summary>
		/// <returns></returns>
        public decimal? SetBuyPrice()
        {
            decimal? result = this.BuyPricePriority;
            this.BuyPricePriority = null;
            return result ?? null;
        }

        /// <summary>
        /// [refactored] Action by settings for [13] Buy Canceled Little Reminder Situation
        /// </summary>
        /// <param name="buyOrder"></param>
        private void BuyOrderCanceledActionBySettings(Order buyOrder)
		{
            // >>>	BUY order canceled
            // (1)	Stop
            // (2)	Start a new iteration with the reminder budget (reminder part buy later) OR Stop
            // (3)	Start a new iteration with the full budget (reminder part buy later) OR Stop
            // (4)	Start a new iteration with the reminder budget (reminder part buy later) OR
            //		Start a new iteration with the full budget (reminder part buy later) OR Stop
            // (5)	Start a new iteration with the full budget (reminder part buy later) OR
            //		Start a new iteration with the reminder budget (reminder part buy later) OR Stop
            // (6)	Start a new iteration with the full budget (forget about reminder part) OR Stop

            // TODO local methods for each actions

            // (1)
            if (Settings.BuyCanceledLittleReminderClassicShortSituation == 0)
            {
                throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
            }

            // Get DOMs
            var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

            // Compute reminder budget
            decimal reminderBudget = this.configuration.Budget - buyOrder.Remainder;

            // Get free balance 
            decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

            // (2)
            if (Settings.BuyCanceledLittleReminderClassicShortSituation == 1)
			{
                if (reminderBudget > this.Exchange.MinCost && Calc.RoundUp(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("BUY order canceled - (2) action");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            // (3)
            if (Settings.BuyCanceledLittleReminderClassicShortSituation == 2)
			{
                if (balance > this.configuration.Budget)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    throw new TinyOrderCanceledException("BUY order canceled - (3) action");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            // (4)
            if (Settings.BuyCanceledLittleReminderClassicShortSituation == 3)
			{
                if (reminderBudget > this.Exchange.MinCost && Calc.RoundDown(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("BUY order canceled - (4) action [1]");
                }
                else if (balance > this.configuration.Budget)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    throw new TinyOrderCanceledException("BUY order canceled - (4) action [2]");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            // (5)
            if (Settings.BuyCanceledLittleReminderClassicShortSituation == 4)
			{
                if (balance > this.configuration.Budget)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    throw new TinyOrderCanceledException("BUY order canceled - (5) action [1]");
                }
                else if (reminderBudget > this.Exchange.MinCost && Calc.RoundDown(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
                {
                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("BUY order canceled - (5) action [2]");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

            // (6)
            if (Settings.BuyCanceledLittleReminderClassicShortSituation == 5)
			{
                if (balance > this.configuration.Budget)
                {
                    throw new TinyOrderCanceledException("BUY order canceled - (6) action");
                }
                else
                {
                    throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
                }
            }

        }

        /// <summary>
        /// [refactored] Actions by settings for [14] Buy Little Reminder Price Increased Situation
        /// </summary>
        /// <param name="buyOrder"></param>
        private void BuyOrderLittleReminderPriceIncreasedActionsBySettings(Order buyOrder)
		{
            // >>>	BUY order with little reminder left, but price dropped
            // (1)	Wait
            // (2)	Start a new iteration with the reminder budget (reminder part buy later) OR Wait
            // (3)	Start a new iteration with the full budget (reminder part buy later) OR Wait
            // (4)	Start a new iteration with the reminder budget (reminder part buy later) OR
            //		Start a new iteration with the full budget (reminder part buy later) OR Wait
            // (5)	Start a new iteration with the full budget (reminder part buy later) OR
            //		Start a new iteration with the reminder budget (reminder part buy later) OR Wait
            // (6)	Start a new iteration with the full budget (forget about reminder part) OR Wait

            // TODO local methods for each actions

            // (1)
            // Just nothing to do

            // Get DOMs
            var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

            // Compute reminder budget
            decimal reminderBudget = this.configuration.Budget - (buyOrder.Remainder * buyOrder.Price);

            // Get free balance 
            decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

            // (2)
            if (Settings.BuyLittleReminderPriceIncreasedClassicShortSituation == 1)
			{
                if (reminderBudget > this.Exchange.MinCost && Calc.RoundDown(reminderBudget * doms["asks"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (2) action");
                }
                // else Wait
            }

            // (3)
            if (Settings.BuyLittleReminderPriceIncreasedClassicShortSituation == 2)
			{
                if (balance > this.configuration.Budget)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (3) action");
                }
                // else Wait
            }

            // (4)
            if (Settings.BuyLittleReminderPriceIncreasedClassicShortSituation == 3)
            {
                if (reminderBudget > this.Exchange.MinCost && Calc.RoundDown(reminderBudget * doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (4) action [1]");
                }
                else if (balance > this.configuration.Budget)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (4) action [2]");
                }
                // else Wait
            }

            // (5)
            if (Settings.BuyLittleReminderPriceIncreasedClassicShortSituation == 4)
            {
                if (balance > this.configuration.Budget)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (5) action [1]");
                }
                else if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    // Remember the reminder
                    this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

                    // Change budget for the next iteration
                    this.BudgetForNextIteration = reminderBudget;

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (5) action [2]");
                }
                // else Wait
            }

            // (6)
            if (Settings.BuyLittleReminderPriceIncreasedClassicShortSituation == 5)
            {
                if (balance > this.configuration.Budget)
                {
                    buyOrder = this.Exchange.CancelOrder(buyOrder);

                    this.Trades.AddOrder(buyOrder);

                    throw new TinyOrderCanceledException("Buy Little Reminder Price Increased - (6) action");
                }
                // else Wait
            }

        }

        /// <summary>
        /// [refactored] Провера BUY-оредара на исполнение. 
        /// True, если ордер исполнен (Перейти на продажу).
        /// False, если нужно выставить новый ордер.
        /// </summary>
        private bool IsBUYFilled(Order _buyOrder)
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                Logger.Info(_.Log10); // Проверяем BUY...

                // Get order info
                _buyOrder = this.Exchange.GetOrderInfo(_buyOrder);

                if (_buyOrder.Status == OrderStatus.Filled)
                {
                    // Sett null to Reminder
                    if (this.Reminder.Count > 0)
                    {
                        Logger.ToFile("Set null to reminder");
                        this.Reminder = new List<ReminderItem>();
                    }

                    Logger.Info(_.Loooooot); // Готово! Считаем бабло, начинаем заново

                    this.Trades.AddOrder(_buyOrder);

                    return true; // BUY filled : finish the iteration
                }
                if (_buyOrder.Status == OrderStatus.Canceled)
                {
                    if (_buyOrder.Remainder > this.Exchange.MinAmount && (_buyOrder.Remainder * this.GetMinProfitBuyPrice()) > this.Exchange.MinCost)
                    {
                        // TODO add filed part to Trades?
                        this.Trades.AddOrder(_buyOrder);

                        // If DCA on copmute again by reminder part
                        if (this.configuration.IsDCA) DCA.Compute();

                        // Set new amount
                        this.Amount = _buyOrder.Remainder;

                        return false; // Place buy order again with reminder amount
                    }

                    this.BuyOrderCanceledActionBySettings(_buyOrder);
                }

                // Get DOM
                var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

                // Min price reached update
                if (doms["asks"][0].Price < this.MinPrice) { this.MinPrice = doms["asks"][0].Price; } 

                if (_buyOrder.Status == OrderStatus.PartiallyFilled)
                {
                    Logger.Info(_.Log13); // Ордер частично исполнен

                    if (_buyOrder.Remainder < this.Exchange.MinAmount && _buyOrder.Remainder * doms["asks"][0].Price < this.Exchange.MinCost)
                    {
                        if(this.Is18thFourConditions(_buyOrder)) this.BuyOrderLittleReminderPriceIncreasedActionsBySettings(_buyOrder);
                    }

                    // TODO merge new and part methods into local or external method
                    // StopLoss
                    if (this.configuration.IsStopLoss)
                    {
                        // Get stoploss price depend on will it be as limit or as market order
                        decimal getStoplossPrice()
                        {
                            if (this.configuration.StopLossApproximation > 0) return this.configuration.StopLossPrice;
                            else return Tools.StopLoss.GetApproximatePriceShort(_buyOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
                        }

                        // StopLoss condition was triggered
                        bool stopLossNecessary = doms["bids"][0].Price >= this.StopLossTriggerPrice;

                        bool filtersAllowed = this.configuration.IConfigText.FiltersStopLoss.Count == 0 || this.configuration.Filters.CheckStopLossFilters(this.configuration.Filters.StopLossFilters, this.configuration.Filters.TargetPointStopLoss);

                        // Check for minimals is it possible to stoploss
                        bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_buyOrder, this.Exchange, getStoplossPrice());

                        // Yes, stoploss we need!
                        if (stopLossNecessary && IsStopLossTimerTimedOut() && filtersAllowed && reminderGreaterMinimals)
                        {
                            Logger.ToFile("[debug] 5656865659566"); // TODO delete

                            // Cancel current SELL order
                            try
                            {
                                Logger.Info(_.Log49); // Сработало условие на стоплосс

                                _buyOrder = this.Exchange.CancelOrder(_buyOrder);

                                this.Trades.AddOrder(_buyOrder);

                                // Just in case
                                if (_buyOrder.Status == OrderStatus.Filled) return true;

                                // Update amount so stoploss can placed for reminder amount
                                this.Amount = _buyOrder.Remainder;

                                Order stopLoss = new Order();

                                // Check reminder again because it can change while canceling
                                if (_buyOrder.Filled != 0)
                                {
                                    if (_buyOrder.Remainder > this.Exchange.MinAmount) continue;
                                    if (_buyOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;
                                }

                                // It possible to use the reminder here, compute the amount and price, and nullify then. But let's use the reminder in the next iteration

                                // SL as limit order
                                if (this.configuration.StopLossApproximation > 0)
                                {
                                    // TODO сделать в будущем чтобы ордер не забывался, а следил за ним пока не исполнится, и в случае чего отменял stoploss и возращался к обычному ордеру с профитом
                                    stopLoss = this.Exchange.OpenLimitOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.configuration.StopLossPrice);

                                    this.Trades.AddOrder(stopLoss);

                                    this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

                                    // TODO why true??? stat will be fucked up
                                    return true; // Ордер исполнится, выходим из итерации
                                }
                                // as market order
                                else
                                {
                                    stopLoss = this.Exchange.OpenMarketOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

                                    stopLoss = this.Exchange.GetOrderInfo(stopLoss);

                                    this.Trades.AddOrder(stopLoss);

                                    // For the stopping after SL if the feature turned on
                                    this.IsStopLossTriggered = true;

                                    // TODO add the counter of stoploss in TTHREAD

                                    return true; // StopLoss SELL order was filled
                                }
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[3]" + ex.Message); // Ордер исполнился пока мы его отменяли
                                Logger.Info(_.Log17);
                                continue;
                            }
                        }
                        // TODO consider about this part. Is it necessary?
                        /*else
                        {
                            // Set time to null, becauseprice increased and we didnt need stoploss
                            this.StopLossTime = 0;
                        }*/
                        if (!stopLossNecessary || !filtersAllowed || !reminderGreaterMinimals)
                        {
                            // Set time to null, becauseprice increased and we didnt need stoploss
                            this.StopLossTime = 0;
                        }
                    }

                    // TODO merge new and part methods into local or external method
                    // Trailing Profit
                    if (this.configuration.IsProfitTrailing)
                    {
                        // Next profit trailing step trigger price
                        decimal TrailStepTriggerPrice = this.BuyPrice + Calc.AmountOfPercent(this.configuration.ApproximationPercent, this.BuyPrice);
                        // Profit trailing stop-profit price (min + unapproximate)
                        decimal ProfitStopTriggerPrice = this.MinPrice + Calc.AmountOfPercent(this.configuration.UnApproximationPercent, this.MinPrice);
                        // Min target profit price
                        decimal TargetProfitSellPrice = this.SellPrice - Calc.AmountOfPercent(this.configuration.TargetProfitPercent, this.SellPrice); // buyPrice + TargetProfitpercent

                        Logger.ToFile($"check 2: {doms["asks"][0].Price} >  {ProfitStopTriggerPrice} && {doms["bids"][0].Price} < {TargetProfitSellPrice}");

                        // TODO future check exact price in doms depend on our amount
                        if (doms["asks"][0].Price > ProfitStopTriggerPrice && doms["asks"][0].Price < TargetProfitSellPrice)
                        {
                            Logger.Info(_.Log56); // Цена начала расти — BUY по рынку

                            try
                            {
                                _buyOrder = this.Exchange.CancelOrder(_buyOrder);

                                this.Trades.AddOrder(_buyOrder);

                                if (_buyOrder.Status == OrderStatus.Filled) continue;

                                if (_buyOrder.Remainder < this.Exchange.MinAmount && Tools.StopLoss.GetApproximatePriceShort(_buyOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2) < this.Exchange.MinCost) continue;

                                // BUY by market
                                _buyOrder = this.Exchange.OpenMarketOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

                                continue; // So, next check the order will have a FILLED status 
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[4]" + ex.Message); // Ордер исполнился пока мы его отменяли
                                Logger.Info(_.Log17);
                                continue;
                            }
                        }

                        Logger.ToFile($"check 1: {doms["bids"][0].Price} >= {TrailStepTriggerPrice}"); // TODO DELETE

                        if (doms["asks"][0].Price <= TrailStepTriggerPrice)
                        {
                            Logger.Info(_.Log55); // Cработал трейлинг профит, переставляем ордер ниже

                            try
                            {
                                _buyOrder = this.Exchange.CancelOrder(_buyOrder);

                                _buyOrder = this.Exchange.GetOrderInfo(_buyOrder);

                                if (_buyOrder.Status == OrderStatus.Filled) continue;

                                // Set buy price priority, so next buy order will be by price (this.BuyPrice + TrailStepPercent)
                                this.BuyPricePriority = this.BuyPrice - Calc.AmountOfPercent(this.configuration.TrailStepPercent, this.SellPrice);
                                this.BuyPricePriority = Calc.RoundDown((decimal)this.BuyPricePriority, this.Exchange.TickSize);

                                if (_buyOrder.Filled > 0) continue; // It will lead to CANCELED actions by setting or re place sell ordaer if amount > minimals

                                this.Trades.AddOrder(_buyOrder);

                                return false; // Re place order with new sell price
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[4]" + ex.Message); // Ордер исполнился пока мы его отменяли
                                Logger.Info(_.Log17);
                                continue;
                            }
                        }
                    }

                }
                if (_buyOrder.Status == OrderStatus.New)
                {
                    // TODO merge new and part methods into local or external method
                    // StopLoss
                    if (this.configuration.IsStopLoss)
                    {
                        // Get stoploss price depend on will it be as limit or as market order
                        decimal getStoplossPrice()
                        {
                            if (this.configuration.StopLossApproximation > 0) return this.configuration.StopLossPrice;
                            else return Tools.StopLoss.GetApproximatePriceShort(_buyOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
                        }

                        // StopLoss condition was triggered
                        bool stopLossNecessary = doms["bids"][0].Price >= this.StopLossTriggerPrice;

                        bool filtersAllowed = this.configuration.IConfigText.FiltersStopLoss.Count == 0 || this.configuration.Filters.CheckStopLossFilters(this.configuration.Filters.StopLossFilters, this.configuration.Filters.TargetPointStopLoss);

                        // Check for minimals is it possible to stoploss
                        bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_buyOrder, this.Exchange, getStoplossPrice());

                        // Yes, stoploss we need!
                        if (stopLossNecessary && IsStopLossTimerTimedOut() && filtersAllowed && reminderGreaterMinimals)
                        {
                            Logger.ToFile("[debug] 5656865659566"); // TODO delete

                            // Cancel current SELL order
                            try
                            {
                                Logger.Info(_.Log49); // Сработало условие на стоплосс

                                _buyOrder = this.Exchange.CancelOrder(_buyOrder);

                                this.Trades.AddOrder(_buyOrder);

                                // Just in case
                                if (_buyOrder.Status == OrderStatus.Filled) return true;

                                // Update amount so stoploss can placed for reminder amount
                                this.Amount = _buyOrder.Remainder;

                                Order stopLoss = new Order();

                                // Check reminder again because it can change while canceling
                                if (_buyOrder.Filled != 0)
                                {
                                    if (_buyOrder.Remainder > this.Exchange.MinAmount) continue;
                                    if (_buyOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;
                                }

                                // It possible to use the reminder here, compute the amount and price, and nullify then. But let's use the reminder in the next iteration

                                // SL as limit order
                                if (this.configuration.StopLossApproximation > 0)
                                {
                                    // TODO сделать в будущем чтобы ордер не забывался, а следил за ним пока не исполнится, и в случае чего отменял stoploss и возращался к обычному ордеру с профитом
                                    stopLoss = this.Exchange.OpenLimitOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.configuration.StopLossPrice);

                                    this.Trades.AddOrder(stopLoss);

                                    this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

                                    // TODO why true??? stat will be fucked up
                                    return true; // Ордер исполнится, выходим из итерации
                                }
                                // as market order
                                else
                                {
                                    stopLoss = this.Exchange.OpenMarketOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

                                    stopLoss = this.Exchange.GetOrderInfo(stopLoss); 
                                    this.Trades.AddOrder(stopLoss);

                                    // For the stopping after SL if the feature turned on
                                    this.IsStopLossTriggered = true;

                                    // TODO add the counter of stoploss in TTHREAD

                                    return true; // StopLoss SELL order was filled
                                }
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[3]" + ex.Message); // Ордер исполнился пока мы его отменяли
                                Logger.Info(_.Log17);
                                continue;
                            }
                        }
                        // TODO consider about this part. Is it necessary?
                        /*else
                        {
                            // Set time to null, becauseprice increased and we didnt need stoploss
                            this.StopLossTime = 0;
                        }*/
                        if (!stopLossNecessary || !filtersAllowed || !reminderGreaterMinimals)
                        {
                            // Set time to null, becauseprice increased and we didnt need stoploss
                            this.StopLossTime = 0;
                        }
                    }

                    // TODO merge new and part methods into local or external method
                    // Trailing Profit
                    if (this.configuration.IsProfitTrailing)
                    {
                        // Next profit trailing step trigger price
                        decimal TrailStepTriggerPrice = this.BuyPrice + Calc.AmountOfPercent(this.configuration.ApproximationPercent, this.BuyPrice);
                        // Profit trailing stop-profit price (min + unapproximate)
                        decimal ProfitStopTriggerPrice = this.MinPrice + Calc.AmountOfPercent(this.configuration.UnApproximationPercent, this.MinPrice);
                        // Min target profit price
                        decimal TargetProfitSellPrice = this.SellPrice - Calc.AmountOfPercent(this.configuration.TargetProfitPercent, this.SellPrice); // buyPrice + TargetProfitpercent

                        Logger.ToFile($"check 2: {doms["asks"][0].Price} >  {ProfitStopTriggerPrice} && {doms["bids"][0].Price} < {TargetProfitSellPrice}");

                        // TODO future check exact price in doms depend on our amount
                        if (doms["asks"][0].Price > ProfitStopTriggerPrice && doms["asks"][0].Price < TargetProfitSellPrice)
                        {
                            Logger.Info(_.Log56); // Цена начала расти — BUY по рынку

                            try
                            {
                                _buyOrder = this.Exchange.CancelOrder(_buyOrder);

                                this.Trades.AddOrder(_buyOrder);

                                if (_buyOrder.Status == OrderStatus.Filled) continue;

                                if (_buyOrder.Remainder < this.Exchange.MinAmount && Tools.StopLoss.GetApproximatePriceShort(_buyOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2) < this.Exchange.MinCost) continue;

                                // BUY by market
                                _buyOrder = this.Exchange.OpenMarketOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

                                continue; // So, next check the order will have a FILLED status 
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[4]" + ex.Message); // Ордер исполнился пока мы его отменяли
                                Logger.Info(_.Log17);
                                continue;
                            }
                        }

                        Logger.ToFile($"check 1: {doms["bids"][0].Price} >= {TrailStepTriggerPrice}"); // TODO DELETE

                        if (doms["asks"][0].Price <= TrailStepTriggerPrice)
                        {
                            Logger.Info(_.Log55); // Cработал трейлинг профит, переставляем ордер ниже

                            try
                            {
                                _buyOrder = this.Exchange.CancelOrder(_buyOrder);

                                _buyOrder = this.Exchange.GetOrderInfo(_buyOrder);

                                this.Trades.AddOrder(_buyOrder);

                                if (_buyOrder.Status == OrderStatus.Filled) continue;

                                // Set buy price priority, so next buy order will be by price (this.BuyPrice + TrailStepPercent)
                                this.BuyPricePriority = this.BuyPrice - Calc.AmountOfPercent(this.configuration.TrailStepPercent, this.SellPrice);
                                this.BuyPricePriority = Calc.RoundDown((decimal)this.BuyPricePriority, this.Exchange.TickSize);

                                if (_buyOrder.Filled > 0) continue; // It will lead to CANCELED actions by setting or re place sell ordaer if amount > minimals

                                return false; // Re place order with new sell price
                            }
                            catch (OrderFilledWhileWeCancelingException ex)
                            {
                                Logger.ToFile("[4]" + ex.Message); // Ордер исполнился пока мы его отменяли
                                Logger.Info(_.Log17);
                                continue;
                            }
                        }
                    }

                }

                if (this.configuration.IsDCA)
                {
                    DCA.CurrentPrice = doms["bids"][0].Price;
                    DCA.Trades = this.Trades;
                    DCA.BuyOrder = _buyOrder;

                    if (DCA.IsTriggered)
                    {
                        // TODO Timer Указываемя время последнего SELL ордера

                        if (DCA.IsSellFilled())
                        {
                            if (DCA.SellForOrderPool != null)
                            {
                                this.Trades.AddOrder(DCA.SellForOrderPool);
                                DCA.Trades = this.Trades;
                                DCA.SellForOrderPool = null;
                            }

                            _buyOrder = this.Exchange.CancelOrder(_buyOrder);// Здесь не будем проверять OrderFilledWhileWeCancelingException т.к. раз усреднились то рабочий sell стоит высоко далеко
                            _buyOrder = DCA.BigBuyOrder();
                            this.BuyCounter++;
                            //TODO Timer Указываемя время последнего BUY ордера
                            TThreadInfo.Times[Core.TID.CurrentID].LastBuyOrder = -1;
                        }
                        DCA.RollbackShort();
                    }
                    if (DCA.CheckNecessaryShort())
                    {
                        DCA.NextStepShort();
                        this.SellCounter++;
                        TThreadInfo.Times[Core.TID.CurrentID].LastSellOrder = -1;
                    }
                }

                Thread.Sleep(Settings.CheckOrderTimeout);
            }

            throw new ManuallyStopException(_.Log19); // Остановился!
        }

        /// <summary>
		/// [refactored] Get buy price that will give the minimal profit
		/// </summary>
		/// <returns></returns>
		public decimal GetMinProfitBuyPrice(decimal? sellPrice = null)
        {
            // Fee rate
            decimal buyFeeRate = this.configuration.IsMarketBuy ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
            decimal sellFeeRate = this.configuration.IsMarketSell ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
            return
            sellPrice ?? this.SellPrice
            /* Min profit markup */
            - Calc.AmountOfPercent(this.configuration.TargetProfitPercent, sellPrice ?? this.SellPrice)
            /* SELL fee */
            - Calc.AmountOfPercent(sellFeeRate, sellPrice ?? this.SellPrice)
            /* BUY approximate fee */
            - Calc.AmountOfPercent(buyFeeRate, sellPrice ?? this.SellPrice - Calc.AmountOfPercent(this.configuration.TargetProfitPercent, sellPrice ?? this.SellPrice));
        }

        /// <summary>
		/// [refactored] Check if ask volume of first 10 orders is enough to filled our market order
		/// </summary>
		/// <returns></returns>
        public bool IsAsksAmountEnough()
        {
            this.BuyPrice = 0;
            decimal haveCostAmount = 0;
            var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 10);
            foreach (var ask in doms["asks"])
            {
                haveCostAmount += ask.Cost;

                if (haveCostAmount > (this.BudgetForNextIteration ?? this.configuration.Budget))
                {
                    if (ask.Price <= this.GetMinProfitBuyPrice())
                    {

                        this.BuyPrice = ask.Price;
                        return true;
                    }
                    else
                    {
                        Logger.Info("Ожидаем мин.профит..."); // TODO TEXT
                        Thread.Sleep(Settings.FiltersTimeout);
                        return false;
                    }
                }
            }

            Logger.Info(_.Log54); // Первых 10 ордеров не хватает чтобы заполнить ордер

            return false;
        }

        /// <summary>
        /// [tested] [refactored] Check four conditions for buy
        /// true - if it allows to do actions by setting
        /// false - if actions are not allowed, just continue to check the order
        /// </summary>
        /// <param name="buyOrder"></param>
        /// <param name="xOrdersAhead"></param>
        /// <param name="secondsAfterLastUpdate"></param>
        /// <param name="dropPercent"></param>
        /// <param name="aheadOrdersVolume"></param>
        /// <returns></returns>
        // TODO merge from all class
        public bool IsXthFourBuyConditions(Order buyOrder, int xOrdersAhead, int secondsAfterLastUpdate, decimal dropPercent, decimal aheadOrdersVolume)
        {
            if (xOrdersAhead == 0 && secondsAfterLastUpdate == 0 && dropPercent == 0 && aheadOrdersVolume == 0) return false;

            if (xOrdersAhead != 0)
            {
                // Get DOMs
                var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, xOrdersAhead);

                if (doms["bids"][xOrdersAhead - 1].Price <= buyOrder.Price) return false; // We above(or on) the x order
            }

            if (secondsAfterLastUpdate != 0)
            {
                decimal currentTime = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (Decimal.Parse(buyOrder.Time) + secondsAfterLastUpdate > currentTime) return false; // Time is not up
            }

            if (dropPercent != 0)
            {
                decimal triggerPrice = buyOrder.Price + (Calc.AmountOfPercent(dropPercent, buyOrder.Price));

                // Get DOMs
                var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 1);

                if (doms["bids"][0].Price < triggerPrice) return false; // Curreent price is less than buyPrice+x%
            }

            if (aheadOrdersVolume != 0)
            {
                // Get DOMs
                var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 50);

                decimal amount = 0;

                foreach (var bid in doms["bids"])
                {
                    if (bid.Price > buyOrder.Price) amount += bid.Amount; // Add volume of above orders only
                    else break;
                }

                if (amount < Calc.AmountOfPercent(aheadOrdersVolume, buyOrder.Amount)) return false; // Volume is less than x% of our
            }

            return true;
        }

        private bool Is18thFourConditions(Order buyOrder)
        {
            return this.IsXthFourBuyConditions
                (
                    buyOrder,
                    Settings.XOrdersAheadSellFilledEnoughPriceDropped,
                    Settings.SecondsAfterLastUpdateSellFilledEnoughPriceDropped,
                    Settings.DropPercentSellFilledEnoughPriceDropped,
                    Settings.AheadOrdersVolumeSellFilledEnoughPriceDropped
                );
        }

        // ***************************************************************************************************************
        // After bought processing

        /// <summary>
		/// [refactored] After bought proceesing: profit computing, clear dca
		/// </summary>
        private void AfterBoughtProcessing()
        {

            // Profit computing

            Logger.ToFile("Profit computing...");

            this.Profit = new Profit(this.Trades, this.Reminder, this.Exchange, false);

            // Clear DCA

            if (this.configuration.IsDCA)
            {
                Logger.Debug("code 48");

                try
                {
                    this.DCA.Clear();
                }
                catch (Exception ex)
                {
                    Logger.ToFile("DCA Clear Exception:" + ex.Message);
                }
            }

            this.BudgetForNextIteration = null;
        }

    }
}
