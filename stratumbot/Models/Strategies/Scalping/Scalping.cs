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
    /// <summary>
    /// Scalping strategy algorythm
    /// </summary>
    class Scalping : IStrategy, IDCAble
    {
        #region fields

        /// <summary>
        /// Token for the thread stopping
        /// </summary>
        public CancellationTokenSource ts { get; set; }
        public CancellationToken cancellationToken { get; set; }

        /// <summary>
        /// Strategy configuration
        /// </summary>
        public ScalpingConfig configuration { get; set; }

        /// <summary>
        /// Current exchange
        /// </summary>
        public IExchange Exchange { get; set; }

        /// <summary>
        /// Currently trade amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Buy price
        /// </summary>
        public decimal BuyPrice { get; set; }

        public decimal? Markup { get; set; } // comment
        public decimal SellPrice { get; set; } // comment

        private decimal? sellPricePriority;
        public decimal? SellPricePriority {
            get
            {
                // I want to get priorty sell price once
                decimal? x = this.sellPricePriority;
                this.sellPricePriority = null;
                return x;
            }
            set
            {
                this.sellPricePriority = value;
            }
        } // comment

        /// <summary>
        /// Currently order's ID
        /// </summary>
        public string OrderId { get; set; } 

        public Trades Trades { get; set; } // comment
        public Profit Profit { get; set; } // comment

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
		///  Counters
		/// </summary>
		public int BuyCounter { get; set; } = 0;
        public int SellCounter { get; set; } = 0;

        /// <summary>
        /// Timeout before place stoploss order
        /// </summary>
        public decimal StopLossTime { get; set; } = 0; // TODO move to StopLoss object ?

        /// <summary>
        /// Was stoploss triggered (stoploss order placed) (for stop the thread after stoploss)
        /// </summary>
        public bool IsStopLossTriggered { get; set; } = false; // TODO move to StopLoss object ?

		/// <summary>
		/// DCA step was changed event 
		/// </summary>
		public event DCAStepChangedDelegate DCAStepChangedEvent;

		#endregion

		/// <summary>
		/// 
		/// [refactored] Constructor
		/// 
		/// </summary>
		/// <param name="config">IConfig configuration of thread's strategy</param>
		/// <param name="exchange">Enum.Exchange of exchnage for thread's strategy</param>
		public Scalping(ScalpingConfig config, Exchange exchange)
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
        /// preparing vars, checking, buying, preparing, selling, profit computing
        /// </summary>
        public void Trade()
        {
            this.SetNullAndDefaultValues();

            this.Buying();

            if(this.cancellationToken.IsCancellationRequested) { throw new ManuallyStopException(_.Log19); }

            this.AfterBoughtProcessing();
            
            this.Selling();
            
            this.AfterSoldProcessing();
        }

		/// <summary>
		/// [refactored] Stop the trading
		/// </summary>
		public void StopTrade()
		{
			if (this.ts != null) this.ts.Cancel();
			Logger.ToFile("Scalping() : Stop Trade ()");
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
			this.IsStopLossTriggered = false; // По умолчанию положение стоплосса = не сработал
			this.Trades = new Trades();

			// Set counters to zero
			this.BuyCounter = 0;
			this.SellCounter = 0;

			// DCA re init
			this.NewDCAInit();

			// Compute percentage budget
			this.PercentageBudgetCompute();

			// Compute percentage parameters
			this.PercentageParametersCompute();

			// Check parameters for minimum possible amount
			this.CheckParametersForMinimals();

			// TODO delete and make config.ToString() instead
			Logger.ToFile($"MinSpread {this.configuration.MinSpread} OptSpread {this.configuration.OptSpread} MinMarkup {this.configuration.MinMarkup} OptMarkup {this.configuration.OptMarkup} ZeroSell {this.configuration.ZeroSell}");

			// TODO Timer here: Times.StartIteration
			TThreadInfo.Times[Core.TID.CurrentID].StartIteration = -1;
		}

		/// <summary>
		/// [refactored] Get amount of budget if it set as percent
		/// </summary>
		private void PercentageBudgetCompute()
		{
			if (this.configuration.IsBudgetAsPercent)
			{
				decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);
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
				this.DCA.CurrentStep = 0;
				// Subscribe to event
				this.DCA.DCAStepChangedEvent += DCAStepChangedHandler;
				// TODO также надо отписаться где то
			}
		}

		/// <summary>
		/// [refactored] Compute actual values if parameters set as percent
		/// </summary>
		private void PercentageParametersCompute()
        {
            if (
                this.configuration.IsMinSpreadAsPercent ||
                this.configuration.IsOptSpreadAsPercent ||
                this.configuration.IsMinMarkupAsPercent ||
                this.configuration.IsOptMarkupAsPercent ||
                this.configuration.IsZeroSellAsPercent)
            {
                Logger.Debug(_.Log6); // Высчитываю параметры из процентов

                var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

                // Min spread
                if (this.configuration.IsMinSpreadAsPercent)
                {
                    this.configuration.MinSpread = Calc.RoundUp(Calc.AmountOfPercent(this.configuration.MinSpreadAsPercent, doms["bids"][0].Price), this.Exchange.TickSize);
                }
                // Opt spread
                if (this.configuration.IsOptSpreadAsPercent)
                {
                    this.configuration.OptSpread = Calc.RoundUp(Calc.AmountOfPercent(this.configuration.OptSpreadAsPercent, doms["bids"][0].Price), this.Exchange.TickSize);
                }
                // Min markup
                if (this.configuration.IsMinMarkupAsPercent)
                {
                    this.configuration.MinMarkup = Calc.RoundUp(Calc.AmountOfPercent(this.configuration.MinMarkupAsPercent, doms["bids"][0].Price), this.Exchange.TickSize);
                }
                // Opt markup
                if (this.configuration.IsOptMarkupAsPercent)
                {
                    this.configuration.OptMarkup = Calc.RoundUp(Calc.AmountOfPercent(this.configuration.OptMarkupAsPercent, doms["bids"][0].Price), this.Exchange.TickSize);
                }
                // Zero sell markup
                if (this.configuration.IsZeroSellAsPercent)
                {
                    this.configuration.ZeroSell = Calc.RoundUp(Calc.AmountOfPercent(this.configuration.ZeroSellAsPercent + (decimal)0.01, doms["bids"][0].Price), this.Exchange.TickSize);
                }
            }
        }

        /// <summary>
        /// [refactored] Check paramenetrs for minimum possible amount (0.00000001)
        /// </summary>
        private void CheckParametersForMinimals()
        {
            decimal min = (decimal)0.00000001;

            if (
                this.configuration.MinSpread < min || 
                this.configuration.OptSpread < min || 
                this.configuration.MinMarkup < min || 
                this.configuration.OptMarkup < min || 
                this.configuration.ZeroSell < min
                )
            {
                throw new Exception("code 13"); // EX5 При расчёте из процентов значения получились меньше 1 сатоша. Скажите это админу
            }
        }

        /// <summary>
        /// [refactored] BUY filters initialization
        /// </summary>
        private void FiltersInit()
        {
            // TODO move init somewhere?
            if (this.configuration.IConfigText.FiltersBuy.Count > 0)
            {
                this.configuration.Filters.BuyFiltersInit(this.configuration.IConfigText.FiltersBuy, this.Exchange, this.configuration);
            }
        }

       
		// ***************************************************************************************************************
		// Buying

		/// <summary>
		/// [refactored] Buying process
		/// </summary>
		private void Buying()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {

                // Looking for filters allowance
                if (this.configuration.Filters.BuyFilters.Count > 0)
                {
                    bool isFiltersAllowed = this.configuration.Filters.FiltersMonitoring(this.configuration.Filters.BuyFilters, (this.configuration.Filters.TargetPointBuy));
                    if (!isFiltersAllowed)
                    {
                        Thread.Sleep(Settings.FiltersTimeout);
                        continue;
                    }
                }
                    
                // Looking for spread
                if (!this.CheckForSpread())
                    continue;

                // Compute the amount from the budget
                this.Amount = Calc.ComputeBuyAmountByBudget(this.BudgetForNextIteration ?? this.configuration.Budget, this.BuyPrice, this.Exchange.StepSize);
                
                // TODO remove loop
                // [loop] BUY
                Order buyOrder = this.Exchange.OpenLimitOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.BuyPrice);

                // Set the buy counter up
                this.BuyCounter++;

				// TODO Timer Times.LastBuyOrder = -1;
				TThreadInfo.Times[Core.TID.CurrentID].LastBuyOrder = -1;

				Thread.Sleep(Settings.BetweenRequestTimeout);

                // [loop] Check the BUY order
                if (this.IsBUYFilled(buyOrder)) break; // Order was filled
				else continue; // If order was canceled (for re-placing) // go back to spread monitoring
			}
        }

		/// <summary>
		/// Actions by setting if BUY order was canceled
		/// 
		/// true - filled amount > minimals - continue the iteratin - place the SELL order
		/// false - no filled amount - start a new iteration
		/// TinyOrderCanceledException - filled amount less than minimals - will sell it next iteratin - start a new iteration
		/// AutoStopException - stop the thread by setting (filled amount less than minimals)
		/// </summary>
		/// <param name="buyOrder"></param>
		/// <returns></returns>
		private bool CanceledAction(Order buyOrder)
		{
			// Just start to looking for new trade-in this.Buying();
			if (buyOrder.Filled == 0) return false;

			if (buyOrder.Filled > this.Exchange.MinAmount && buyOrder.Filled * this.GetSellPrice() > this.Exchange.MinCost)
			{
				Logger.Info("BUY частично исполнен и отменён, ставим ордер на продажу"); // TODO norm text

				this.Trades.AddOrder(buyOrder);

				return true; // BUY filled : place SELL order
			}

			// else

			// Actions by setting
			this.BuyOrderCanceledActionBySettings(buyOrder);

			throw new AutoStopException("Поток остановлен из-за отсутствия настроек"); // TODO norm text
		}

		/// <summary>
		/// Get sell price depending on spread
		/// </summary>
		/// <returns></returns>
		private decimal GetSellPrice()
		{
			if (this.Reminder.Count > 0)
			{
				decimal buyPriceWithAdditionalAmount = ReminderHelper.GetAvgPriceWithAdditionalAmountForSell(this.Reminder, this.Amount, this.BuyPrice);
				return Calc.RoundUp(buyPriceWithAdditionalAmount + this.GetMarkup(buyPriceWithAdditionalAmount), this.Exchange.TickSize);
			}
			else
			{
				return Calc.RoundUp(this.BuyPrice + this.GetMarkup(this.BuyPrice), this.Exchange.TickSize);
				//return this.BuyPrice + this.GetMarkup(this.BuyPrice);
			}
		}

		/// <summary>
		/// Actions by setting when [19] Buy Canceled Situation
		/// </summary>
		/// <param name="buyOrder"></param>
		private void BuyOrderCanceledActionBySettings(Order buyOrder)
		{
			// >>>	BUY order canceled
			// (1)	Stop
			// (2)	Start a new iteration with the reminder budget (filled part sell later) OR Stop
			// (3)	Start a new iteration with the full budget (filled part sell later) OR Stop
			// (4)	Start a new iteration with the reminder budget (filled part sell later) OR
			//		Start a new iteration with the full budget (filled part sell later) OR Stop

			// (1)
			if (Settings.BuyCanceledScalpingSituation == 0)
			{
				throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
			}

			this.Trades.AddOrder(buyOrder);

			// Get DOMs
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Compute reminder budget
			decimal reminderBudget = this.configuration.Budget - (buyOrder.Filled * buyOrder.Price);

			// Get free balance 
			decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

			// (2)
			if (Settings.BuyCanceledScalpingSituation == 1)
			{
				if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
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
			if (Settings.BuyCanceledScalpingSituation == 2)
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
			if (Settings.BuyCanceledScalpingSituation == 3)
			{
				if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
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

			throw new AutoStopException("Поток остановлен из-за отсутствия настроек"); // TODO norm text
		}

		/// <summary>
		/// Actions by settings for [20] Buy Little Filled Price Increased Situation
		/// </summary>
		/// <param name="buyOrder"></param>
		private void BuyLittleFilledPriceIncreasedActionsBySettings(Order buyOrder)
		{
			// >>>	BUY order little filled and price gone up
			// (1)	Wait
			// (2)	Start a new iteration with the reminder budget (filled part sell later), cancel current one OR Wait
			// (3)	Start a new iteration with the full budget (filled part sell later), cancel current one OR Wait
			// (4)	Start a new iteration with the reminder budget (filled part sell later), cancel current one OR
			//		Start a new iteration with the full budget (filled part sell later), cancel current one OR Wait
			// (5)	Start a new iteration with the full budget (filled part sell later), cancel current one OR
			//		Start a new iteration with the reminder budget (filled part sell later), cancel current one OR Wait

			// Get DOMs
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Compute reminder budget
			decimal reminderBudget = this.configuration.Budget - (buyOrder.Filled * buyOrder.Price);

			// Get free balance 
			decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

			bool isFreeBalanceGreaterFullBudget() { return balance > this.configuration.Budget; };
			bool isFreeBalanceGreaterReminderBudget() { return balance > reminderBudget; };
			bool isReminderBudgetGreaterMinimals() { return reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount; };
			void newIterationWithReminderBudget()
			{
				try
				{
					buyOrder = this.Exchange.CancelOrder(buyOrder);

					this.Trades.AddOrder(buyOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

					// Change budget for the next iteration
					this.BudgetForNextIteration = reminderBudget;

					throw new TinyOrderCanceledException("BUY order canceled - (2) action");
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
					buyOrder = this.Exchange.CancelOrder(buyOrder);

					this.Trades.AddOrder(buyOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(buyOrder, this.Trades));

					throw new TinyOrderCanceledException("BUY order canceled - (3) action");
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
			if (Settings.BuyLittleFilledPriceIncreasedScalpingSituation == 1)
			{
				if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
				// else (1) Wait
			}

			// (3)
			if (Settings.BuyLittleFilledPriceIncreasedScalpingSituation == 2)
			{
				if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
				// else (1) Wait
			}

			// (4)
			if (Settings.BuyLittleFilledPriceIncreasedScalpingSituation == 3)
			{
				if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
				else if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
				// else (1) Wait 
			}

			// (5)
			if (Settings.BuyLittleFilledPriceIncreasedScalpingSituation == 4)
			{
				if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
				else if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
				// else (1) Wait 
			}
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

		/// <summary>
		/// [tested] [refactored] [24] Four waiting conditions for [20] Buy Little Filled Price Increased Situation
		/// </summary>
		/// <returns></returns>
		private bool Is6thFourConditions(Order buyOrder)
		{
			return this.IsXthFourBuyConditions
				(
					buyOrder,
					Settings.XOrdersAheadLittleFilledPriceIncreasedScalpingSituation,
					Settings.SecondsAfterLastUpdateLittleFilledPriceIncreasedScalpingSituation,
					Settings.DropPercentLittleFilledPriceIncreasedScalpingSituation,
					Settings.AheadOrdersVolumeLittleFilledPriceIncreasedScalpingSituation
				);
		}

		/// <summary>
		/// [tested] [refactored] [25] Four waiting conditions for [26] Buy Filled Enough Price Increased
		/// </summary>
		/// <param name="buyOrder"></param>
		/// <returns></returns>
		private bool Is7thFourConditions(Order buyOrder)
		{
			return this.IsXthFourBuyConditions
				(
					buyOrder,
					Settings.XOrdersAheadBuyFilledEnoughPriceIncreasedScalping,
					Settings.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreasedScalping,
					Settings.DropPercentBuyFilledEnoughPriceIncreasedScalping,
					Settings.AheadOrdersVolumeBuyFilledEnoughPriceIncreasedScalping
				);
		}

		/// <summary>
		/// [refactored] Check market for spread
		/// </summary>
		/// <returns>(bool) Is there is spread</returns>
		private bool CheckForSpread()
		{
			// while (!this.cancellationToken.IsCancellationRequested)  {}

			// FIRST CHECK

			// Get the orderbook
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Spread compute
			var spread = doms["asks"][0].Price - doms["bids"][0].Price;

			// Is spread good enough
			if (spread < this.configuration.MinSpread)
			{
				// No. So timeout for checking [and continue]
				Logger.Info($"{_.Log7} {spread} < {this.configuration.MinSpread}"); // Не входим. Маленький спред
				Thread.Sleep(Settings.CheckTimeout);
				return false;
			}

			// Waiting for {0} sec.
			Logger.Info(String.Format(_.Log8, this.configuration.InTimeout));
			Thread.Sleep(this.configuration.InTimeout * 1000);

			// SECOND CHECK

			// Get the orderbook again
			doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Spread compute again
			spread = doms["asks"][0].Price - doms["bids"][0].Price;

			// Is spread still good enough
			if (spread < this.configuration.MinSpread)
			{
				// No. Spread was changed [continue and check again]
				Logger.Info(_.Log9); // Спред изменился
				return false;
			}

			// Spread is good enough

			// Set the buy price
			this.BuyPrice = this.GetBuyPrice(spread, doms["bids"][0].Price);

			return true;

			//throw new StopException("code 23"); // Мониторинг прекращён
		}

		/// <summary>
		/// [refactored] Get the buy price depends on spread and best bid price
		/// </summary>
		/// <param name="spread">Current spread</param>
		/// <param name="bestBidPrice">Best bid price in the orderbook</param>
		/// <returns>The buy price</returns>
		private decimal GetBuyPrice(decimal spread, decimal bestBidPrice)
		{
			// Spread >= MinSpread & Spread < OptSpread : glue to first order
			if (spread >= this.configuration.MinSpread && spread < this.configuration.OptSpread) return bestBidPrice;

			// Spread >= OptSpread : place the order above other
			if (spread >= this.configuration.OptSpread) return bestBidPrice + this.Exchange.TickSize;

			throw new Exception($"BuyPriceCompute(): {spread} <=> {bestBidPrice}");
		}

		private bool IsXOrdersAbove(Order buyOrder, Dictionary<string, List<Depth>> doms)
		{
			int orderNum = 0;
			foreach (var bid in doms["bids"])
			{
				orderNum++;
				if (bid.Price >= buyOrder.Price &&
					orderNum >= this.configuration.FirsOredersCountIgnor
				)
				{
					// There is X orders above us
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Провера BUY-оредара на исполнение. 
		/// True, если ордер исполнен (Перейти на продажу).
		/// False, если нужно выставить новый ордер.
		/// </summary>
		private bool IsBUYFilled(Order _buyOrder)
		{
			while (!this.cancellationToken.IsCancellationRequested)
			{
				// Check the BUY order...
				Logger.Info(_.Log10);

				// Get the order info
				_buyOrder = Exchange.GetOrderInfo(_buyOrder);

				if (_buyOrder.Status == OrderStatus.Filled)
				{
					Logger.Info(_.Log11); // BUY исполнен! Ставим ордер на продажу...

					this.Trades.AddOrder(_buyOrder);

					return true; // BUY is filled : place the SELL
				}
				if (_buyOrder.Status == OrderStatus.Canceled)
				{

					return this.CanceledAction(_buyOrder);

					/*
					// // остановка бота
					// 10.10.21 - Сейчас он не востановится 
					if (_buyOrder.Filled > 0)
					{

						bool isGreaterThanMins = true;

						// Если исполненная часть < Мин. допустимого объема
						if (_buyOrder.Filled < this.Exchange.MinAmount)
						{
							//Logger.Info(_.Log14);
							//Thread.Sleep(Settings.CheckOrderTimeout);
							isGreaterThanMins = false;
						}

						// Если сумма исполненной части < Мин.допустимой стоимоссти
						if ((_buyOrder.Filled * (this.BuyPrice + this.configuration.ZeroSell)) < this.Exchange.MinCost)
						{
							//Logger.Info(_.Log15);
							//Thread.Sleep(Settings.CheckOrderTimeout);
							isGreaterThanMins = false;
						}

						if (isGreaterThanMins)
						{
							return true; // BUY is filled : place the SELL
						}

						// Remember the reminder
						this.Reminder.Add(ReminderHelper.OrderToReminderItem(_buyOrder, this.Trades));
					}

					// Will start new iteration
					// TODO change log info
					throw new OrderCanceledException("Ордер был отменён — попробую решить (3)"); // Ордер был отменён я останавливаюсь -> TThread // EX3 
					*/
				}
				if (_buyOrder.Status == OrderStatus.PartiallyFilled)
				{
					Logger.Info(_.Log13); // Ордер частично исполнен

					//_//_//_//_//_//_//_//_//
					//    ЧАСТИЧНЫЙ BUY     //
					//_//_//_//_//_//_//_//_//

					// (1) если уже нельзя выставить селл с минимальной наценкой
					// (2) или если перед нашим баем уже 4 ордера по лучшей цене
					//     -> то отменяем бай и выставляем селл - конец итерации
					//  - иначе ждём пока дольют или ситуация не изменится

					// Get DOMs
					var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, this.configuration.FirsOredersCountIgnor);

					// TODO check the orders volume as in Scalping
					// (1) our order is first: price of first bid = our order price
					bool isOrderFirst = doms["bids"][0].Price == _buyOrder.Price;
					bool isReminderMoreMinimals = (_buyOrder.Remainder > this.Exchange.MinAmount && (_buyOrder.Remainder * (doms["bids"][0].Price + this.Exchange.TickSize)) > this.Exchange.MinCost);
					// TOSO check should we really check here by zerosell price or by asks like in Classic Long?
					bool isFilledAmountMoreMinimals = (_buyOrder.Filled > this.Exchange.MinAmount && (_buyOrder.Filled * (this.BuyPrice + this.configuration.ZeroSell)) > this.Exchange.MinCost);
					// (2) our order too high above second one - inefficient price
					bool isBackTrailingNeeded = isOrderFirst && doms["bids"][1].Price != (_buyOrder.Price - this.Exchange.TickSize);
					// TODO TEST what if there is no filters
					bool isFiltersAllowed = this.configuration.Filters.CheckFilters(this.configuration.Filters.BuyFilters, this.configuration.Filters.TargetPointBuy);

					if (!isOrderFirst)
					{
						Logger.Info($"{_.Log18} {_buyOrder.Price} < {doms["bids"][0].Price} [1]");

						if (isFilledAmountMoreMinimals)
						{
							if (!this.Is7thFourConditions(_buyOrder)) continue;

							try
							{
								_buyOrder = this.Exchange.CancelOrder(_buyOrder);

								this.Trades.AddOrder(_buyOrder);

								return true; // order was filled : place SELL order
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
							if (this.configuration.IConfigText.FiltersBuy.Count == 0)
							{
								Logger.Error("code 38");
								continue;
							}

							// We can place a new order, but for now it doesnt matter, filters are not allow to buy anyway
							if (!isFiltersAllowed) continue;

							// [24] Four waiting conditions for [20] Buy Little Filled Price Increased Situation
							if (!this.Is6thFourConditions(_buyOrder)) continue;

							// Action by setting
							this.BuyLittleFilledPriceIncreasedActionsBySettings(_buyOrder);

						}
					}

					if (isBackTrailingNeeded)
					{
						Logger.Info($"{_.Log18} ({_buyOrder.Price} - {this.Exchange.TickSize}) ≠ {doms["bids"][1].Price}");

						// Re-place a new BUY order for the reminder amount

						if (isReminderMoreMinimals /* ?? isFiltersAllowed */)
						{
							Logger.ToFile($"TP 1 2"); // TODO DELETE

							try
							{
								_buyOrder = this.Exchange.CancelOrder(_buyOrder);

								// TODO do we really need check again?
								_buyOrder = this.Exchange.GetOrderInfo(_buyOrder);

								if (_buyOrder.Status == OrderStatus.Filled) return true; // Place SELL order

								// Remember the filled part
								this.Trades.AddOrder(_buyOrder);

								isReminderMoreMinimals = (_buyOrder.Remainder > this.Exchange.MinAmount && (_buyOrder.Remainder * (doms["bids"][0].Price + this.Exchange.TickSize)) > this.Exchange.MinCost);

								if (isReminderMoreMinimals)
								{
									// Reminder amount
									this.Amount = _buyOrder.Remainder;

									return false; // Current buy doesnt filled, we will place a new one for the reminder amount
								}
								else
								{
									isFilledAmountMoreMinimals = (_buyOrder.Filled > this.Exchange.MinAmount && (_buyOrder.Filled * (doms["asks"][0].Price - this.Exchange.TickSize)) > this.Exchange.MinCost);

									if (isFilledAmountMoreMinimals) return true; // Place SELL order for filled amount
									// TODO can i here replace this with CanceledAction? this method called anyway
									else this.BuyOrderCanceledActionBySettings(_buyOrder);
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

					/*
					// Spread compute
					var spread = Math.Round(doms["asks"][0].Price - doms["bids"][0].Price, 8);
					bool isSpreadLessThanMinMarkup = spread < this.configuration.MinMarkup;

					//bool isMinMarkupImpossible = doms["asks"][0].Price <;

					if (isSpreadLessThanMinMarkup || this.IsXOrdersAbove(_buyOrder, doms))
					{
						Logger.Info(_.Log16); // Отменяем частинчый BUY для продажи...

						try
						{
							_buyOrder = this.Exchange.CancelOrder(_buyOrder);


							this.Amount = _buyOrder.Filled; // Обновляем количество на которое он исполнился
							this.Trades.AddOrder(_buyOrder);

							if (!isFilledAmountMoreMinimals)
							{
								// Remember the reminder
								this.Reminder.Add(ReminderHelper.OrderToReminderItem(_buyOrder, this.Trades));

								// Will start new iteration
								// TODO change log info
								throw new OrderCanceledException("Ордер был отменён — попробую решить (2)"); // EX1

							}

							return true; // BUY partially filled : SELL давай

						}
						catch (OrderFilledWhileWeCancelingException ex)
						{
							/// Ордер исполнился пока мы его отменяли
							Logger.ToFile("[2]" + ex.Message);
							Logger.Info(_.Log17);
							continue;  /// продолжаем
						}
					}
					*/
				}
				if (_buyOrder.Status == OrderStatus.New)
				{
					//_//_//_//_//_//_//_//_//
					//    ПЕРВЫЕ ЛИ МЫ?     //
					//_//_//_//_//_//_//_//_//

					// проверить цену первого ордера, если лучше нашей 
					// -> (1) проверить объем, если мелкий то ничего
					// -> (2) если не мелкий то отменяем и начинаем цикл заново
					// -> (3) Но если мы уже не 2 а 3, то отменяем не обращая внимания на объем
					// -> (4) если мы первые но цена не равна next+1 (выше рынка пытаемся купить)

					// Get DOMs
					var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

					// (1) our order is first: price of first bid = our order price
					bool isOrderFirst = doms["bids"][0].Price == _buyOrder.Price;

					// (2)
					// TODO check not just firts but all above orders
					bool isAmountTooBig = doms["bids"][0].Amount > Calc.AmountOfPercent(this.configuration.FirsOredersAmountPercentIgnor, _buyOrder.Amount);

					// (3)
					bool isMoreThanOneOrderAbove = doms["bids"][1].Price > _buyOrder.Price;

					// (4) our order too high above second one - inefficient price
					bool isBackTrailingNeeded = isOrderFirst && doms["bids"][1].Price != (_buyOrder.Price - this.Exchange.TickSize);

					// TODO FUTURE Достаточный объем для перестановки смотреть в отедльной настройке можно а не FirsOredersAmountPercentIgnor
					// WTF is this? it was in condition with isBackTrailingNeeded
					//bool isSMtHAMMDM = _buyOrder.Amount < (_buyOrder.Amount + Calc.AmountOfPercent(this.configuration.FirsOredersAmountPercentIgnor, _buyOrder.Amount));

					// (1) (2) (3)
					if ( (!isOrderFirst && isAmountTooBig) || isMoreThanOneOrderAbove || isBackTrailingNeeded)
					{
						if (!isOrderFirst && isAmountTooBig) Logger.Info($"{_.Log18} {_buyOrder.Price} < {doms["bids"][0].Price} [1]");
						if (isMoreThanOneOrderAbove) Logger.Info($"{_.Log18} {_buyOrder.Price} < {doms["bids"][0].Price} [2]");
						if (isBackTrailingNeeded) Logger.Info($"{_.Log18} ({_buyOrder.Price} - {this.Exchange.TickSize}) ≠ {doms["bids"][1].Price}");

						// TODO Does it really helpful to check again?
						_buyOrder = this.Exchange.GetOrderInfo(_buyOrder);

						if (_buyOrder.Status == OrderStatus.Filled || _buyOrder.Status == OrderStatus.PartiallyFilled || _buyOrder.Status == OrderStatus.Canceled)
						{
							continue; // Continue to check buy
						}

						try
						{
							_buyOrder = this.Exchange.CancelOrder(_buyOrder);

							return this.CanceledAction(_buyOrder);
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


		// ***************************************************************************************************************
		// After bought processing

		/// <summary>
		/// [refactored] Buy price and amount recomputing, computing stoploss and dca
		/// </summary>
		private void AfterBoughtProcessing()
        {
			// Recomputing buy price and amount in case if there are multiple orders filled
			this.BuyPrice = Trades.GetAvgBuyPrice(this.Trades);
            this.Amount = Trades.GetFullBuyAmount(this.Trades);

			// TODO upgrate stoploss line in classic algorythms
            // StopLoss compute
            if (this.configuration.IsStopLoss)
            {
                if (this.configuration.StopLossPercent != 0)
                {
                    this.configuration.StopLossPrice = this.BuyPrice - (Calc.AmountOfPercent(this.configuration.StopLossPercent, this.BuyPrice));
					// добавил от 0.3.22 5246446144264
					this.configuration.StopLossPrice = Calc.RoundUp(this.configuration.StopLossPrice, this.Exchange.TickSize);
					
				}
            }

            // DCA compute
            if (this.configuration.IsDCA)
            {
                this.DCA.Trades = this.Trades;
                this.DCA.Compute();
            }

            Thread.Sleep(Settings.BetweenRequestTimeout);
        }


		// ***************************************************************************************************************
		// Selling

		/// <summary>
		/// Selling process
		/// </summary>
		private void Selling()
        {
			// Get actual amount, depend on if there is a reminder
			decimal GetAmount()
			{
				if (this.Reminder.Count > 0) return this.Amount + ReminderHelper.GetTotalReminderAmountForSell(this.Reminder);
				else return this.Amount;
			}

			// Get actual sell price, depend on if there is a reminder
			decimal GetSellPrice()
			{
				return this.SellPrice = this.GetSellPrice();
			}

            while (!this.cancellationToken.IsCancellationRequested)
            {
				// [loop] SELL
				Order sellOrder = this.Exchange.OpenLimitOrder(
					OrderSide.SELL,
					this.configuration.Cur1,
					this.configuration.Cur2,
					GetAmount(),
					GetSellPrice()
				);

				// Set the sell counter up
				this.SellCounter++;

				// TODO Timer Times.LastSellOrder
				TThreadInfo.Times[Core.TID.CurrentID].LastSellOrder = -1;

				Thread.Sleep(Settings.BetweenRequestTimeout);

                // [loop] Check the SELL order (and DCA BUY)
                if (IsSELLFilled(sellOrder)) break; // SELL-order was filled
                else continue; // Goto to re-place with new markup/price
            }
        }

		/// <summary>
		/// [refactored] Get default markup (on first placing a sell order)
		/// </summary>
		/// <param name="_buyPrice"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public decimal GetMarkup(decimal _buyPrice)
		{
			// Получить стаканы
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// >= opt - opt 
			if (doms["asks"][0].Price >= (_buyPrice + this.configuration.OptMarkup))
			{
				return this.configuration.OptMarkup; // не наглеем
			}
			// > min , < opt - one-1 
			if (doms["asks"][0].Price > (_buyPrice + this.configuration.MinMarkup) && doms["asks"][0].Price < (_buyPrice + this.configuration.OptMarkup))
			{
				return (doms["asks"][0].Price - this.Exchange.TickSize) - _buyPrice;
			}
			// <= min - min (а дальше он sell сам переставит) 
			if (doms["asks"][0].Price <= (_buyPrice + this.configuration.MinMarkup))
			{
				return this.configuration.MinMarkup;
			}

			throw new Exception("code 12"); // EX2 Не нашли какую сделать минимадку
		}

		/// <summary>
		/// [refactored] Get markup, if we already computed it while canceling (used in ???)
		/// </summary>
		/// <returns></returns>
		/*public decimal? SetMarkup()
		{
			var result = this.Markup;
			this.Markup = null;
			return result ?? null;
		}*/

		/// <summary>
		/// Провера SELL-оредара на исполнение. 
		/// True, если ордер исполнен (Завершить итерацию).
		/// False, если нужно выставить новый ордер.
		/// </summary>
		private bool IsSELLFilled(Order _sellOrder)
		{
			while (!this.cancellationToken.IsCancellationRequested)
			{
				Logger.Info(_.Log20); // Проверяем SELL...

				// Get order info
				_sellOrder = this.Exchange.GetOrderInfo(_sellOrder);

				if (_sellOrder.Status == OrderStatus.Filled)
				{
					// Sett null to Reminder
					if (this.Reminder.Count > 0)
					{
						Logger.ToFile("Set null to reminder");
						this.Reminder = new List<ReminderItem>();
					}

					Logger.Info(_.Loooooot); // Готово! Считаем бабло, начинаем заново
					
					this.Trades.AddOrder(_sellOrder);

					return true; // SELL исполнен : заканчиваем итерацию
				}
				// Получаем стаканы, дальше пригодятся
				var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);
				if (_sellOrder.Status == OrderStatus.Canceled)
				{

					// Если исполненная часть < Мин. допустимого объема
					if (_sellOrder.Remainder > this.Exchange.MinAmount && (_sellOrder.Remainder * (this.SellPrice + this.configuration.ZeroSell)) > this.Exchange.MinCost)
					{
						// TODO add filed part to Trades?
						this.Trades.AddOrder(_sellOrder);

						// If DCA on copmute again by reminder part
						if (this.configuration.IsDCA) DCA.Compute();

						//this.Markup = doms["asks"][0].Price - this.Exchange.TickSize - this.BuyPrice;
						this.Amount = _sellOrder.Remainder;

						return false; // re place SELL order
					}

					// // остановка бота
					// TODO change log info
					//throw new OrderCanceledException("Ордер был отменён — попробую решить (1)"); // Ордер был отменён — я останавливаюсь -> TThread // EX3
					this.SellOrderCanceledActionBySettings(_sellOrder);
				}

				if (_sellOrder.Status == OrderStatus.PartiallyFilled)
				{
					Logger.Info(_.Log13); // Ордер частично исполнен

					if (_sellOrder.Remainder < this.Exchange.MinAmount && _sellOrder.Remainder * (this.BuyPrice + this.configuration.ZeroSell) < this.Exchange.MinCost)
					{
						if (this.Is9thFourConditions(_sellOrder)) this.SellOrderLittleReminderPriceDroppedActionsBySettings(_sellOrder);
					}

					// StopLoss
					if (this.configuration.IsStopLoss)
					{
						// Get stoploss price depend on will it be as limit or as market order
						decimal getStoplossPrice()
						{
							return Tools.StopLoss.GetApproximatePrice(_sellOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
						}

						// StopLoss condition was triggered
						bool stopLossNecessary = doms["bids"][0].Price <= this.configuration.StopLossPrice;

						// Check for minimals is it possible to stoploss
						bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_sellOrder, this.Exchange, this.configuration.StopLossPrice);

						// Yes, stoploss we need!
						// TODO filters stoploss as 
						if (stopLossNecessary && reminderGreaterMinimals && this.IsStopLossTimerTimedOut())
						{

							Logger.Info(_.Log49); // Сработало условие на стоплосс

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								// Just in case
								if (_sellOrder.Status == OrderStatus.Filled) return true;

								// Update amount so stoploss can placed for reminder amount
								this.Amount = _sellOrder.Remainder;

								// Check reminder again because it can change while canceling
								if (_sellOrder.Filled != 0)
								{
									if (_sellOrder.Remainder > this.Exchange.MinAmount) continue;
									if (_sellOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;
								}

								//Order stopLoss = StopLoss.SellByMarket(_sellOrder, this.Exchange, this.configuration.StopLossPrice);
								Order stopLoss = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

								this.Trades.AddOrder(stopLoss); // TODO возможно тут баг статы, потому что у такого ордера наверно цена 0

								this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

								// TODO add the counter of stoploss in TTHREAD

								return true; // StopLoss SELL order was filled
							}
							catch (OrderFilledWhileWeCancelingException ex)
							{
								/// Ордер исполнился пока мы его отменяли
								Logger.ToFile("[3]" + ex.Message);
								Logger.Info(_.Log17);
								continue;  /// продолжаем
							}

						}
						// TODO consider about this part. Is it necessary?
						else
						{
							// Set time to null, becauseprice increased and we didnt need stoploss
							this.StopLossTime = 0;
						}
					}

					bool isOrderFirst = doms["asks"][0].Price == _sellOrder.Price;
					bool isAheadOrderBigEnough = doms["asks"][0].Amount > Calc.AmountOfPercent(this.configuration.FirsOredersAmountPercentIgnor, _sellOrder.Amount);
					bool isNextPriceGreaterZeroSell = (doms["asks"][0].Price - this.Exchange.TickSize) >= (this.BuyPrice + this.configuration.ZeroSell);

					if (!isOrderFirst && isAheadOrderBigEnough && isNextPriceGreaterZeroSell)
					{
						try
						{
							_sellOrder = this.Exchange.CancelOrder(_sellOrder);

							// Just continue
							// if order was filled - ok
							// if order was canceled - place new order with reminder or actions
							continue;
						}
						catch (OrderFilledWhileWeCancelingException ex)
						{
							Logger.ToFile("[2]" + ex.Message);
							Logger.Info(_.Log17);
							continue;
						}
					}
				}
				if (_sellOrder.Status == OrderStatus.New)
				{
					// StopLoss
					if (this.configuration.IsStopLoss)
					{
						// Get stoploss price depend on will it be as limit or as market order
						decimal getStoplossPrice()
						{
							return Tools.StopLoss.GetApproximatePrice(_sellOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
						}

						// StopLoss condition was triggered
						bool stopLossNecessary = doms["bids"][0].Price <= this.configuration.StopLossPrice;

						// Check for minimals is it possible to stoploss
						bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_sellOrder, this.Exchange, this.configuration.StopLossPrice);

						// Yes, stoploss we need!
						// TODO filters stoploss as 
						if (stopLossNecessary && reminderGreaterMinimals && this.IsStopLossTimerTimedOut())
						{

							Logger.Info(_.Log49); // Сработало условие на стоплосс

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								// Just in case
								if (_sellOrder.Status == OrderStatus.Filled) return true;

								// Update amount so stoploss can placed for reminder amount
								this.Amount = _sellOrder.Remainder;

								// Check reminder again because it can change while canceling
								if (_sellOrder.Filled != 0)
								{
									if (_sellOrder.Remainder > this.Exchange.MinAmount) continue;
									if (_sellOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;
								}

								//Order stopLoss = StopLoss.SellByMarket(_sellOrder, this.Exchange, this.configuration.StopLossPrice);
								Order stopLoss = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);
								
								this.Trades.AddOrder(stopLoss); // TODO возможно тут баг статы, потому что у такого ордера наверно цена 0
								
								this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

								// TODO add the counter of stoploss in TTHREAD

								return true; // StopLoss SELL order was filled
							}
							catch (OrderFilledWhileWeCancelingException ex)
							{
								/// Ордер исполнился пока мы его отменяли
								Logger.ToFile("[3]" + ex.Message);
								Logger.Info(_.Log17);
								continue;  /// продолжаем
							}
							
						}
						// TODO consider about this part. Is it necessary?
						else
						{
							// Set time to null, becauseprice increased and we didnt need stoploss
							this.StopLossTime = 0;
						}
					}

					bool isOrderFirst = doms["asks"][0].Price == _sellOrder.Price;
					bool isAheadOrderBigEnough = doms["asks"][0].Amount > Calc.AmountOfPercent(this.configuration.FirsOredersAmountPercentIgnor, _sellOrder.Amount);

					bool isBackTrailingNeeded = isOrderFirst && doms["asks"][1].Price != (_sellOrder.Price + this.Exchange.TickSize);
					
					if (!isOrderFirst && isAheadOrderBigEnough)
					{
						bool isNextPriceGreaterZeroSell = (doms["asks"][0].Price - this.Exchange.TickSize) >= (this.BuyPrice + this.configuration.ZeroSell);

						if (isNextPriceGreaterZeroSell)
						{
							Logger.Info($"{_.Log23} {_sellOrder.Price} > {doms["asks"][0].Price}");

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								if (_sellOrder.Filled > 0) continue; // It will lead to CANCELED actions by setting or re place sell ordaer if amount > minimals

								//this.Markup = doms["asks"][0].Price - this.Exchange.TickSize - this.BuyPrice;

								return false; // Order was canceled, place again
							}
							catch (OrderFilledWhileWeCancelingException ex)
							{
								Logger.ToFile("[4]" + ex.Message);
								Logger.Info(_.Log17);
								continue;
							}
						}
					}
					
					if (isBackTrailingNeeded) // TODO FUTURE также как в buy, можно сделать отдельнеое поле для этой настройки
					{
						bool isOptMarkupAllowBackTrailing = (doms["asks"][1].Price - this.Exchange.TickSize) <= (this.BuyPrice + this.configuration.OptMarkup);
						bool isBackTrailingPriceGreaterZeroSell = (doms["asks"][1].Price - this.Exchange.TickSize) >= (this.BuyPrice + this.configuration.ZeroSell);

						if (isOptMarkupAllowBackTrailing && isBackTrailingPriceGreaterZeroSell)
						{
							_sellOrder = this.Exchange.GetOrderInfo(_sellOrder);
							
							// Just in case
							if (_sellOrder.Status != OrderStatus.New) continue;

							Logger.Info($"{_.Log23} {this.SellPrice} + {this.Exchange.TickSize} ≠ {doms["asks"][1].Price}");

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								if (_sellOrder.Filled != 0) continue; // lead to Caceled actions by settings
								
								// TODO TEST doest it really need?
								this.Markup = doms["asks"][1].Price - this.Exchange.TickSize - this.BuyPrice;
								
								return false; // re-place sell again
							}
							catch (OrderFilledWhileWeCancelingException ex)
							{
								Logger.ToFile("[5]" + ex.Message);
								Logger.Info(_.Log17);
								continue;
							}
						}
					}
				}

				if (this.configuration.IsDCA)
				{
					DCA.CurrentPrice = doms["asks"][0].Price;
					DCA.Trades = this.Trades;
					DCA.SellOrder = _sellOrder;

					if (DCA.IsTriggered)
					{
						if (DCA.IsBuyFilled())
						{
							if (DCA.BuyForOrderPool != null)
							{
								this.Trades.AddOrder(DCA.BuyForOrderPool);
								DCA.Trades = this.Trades;
								DCA.BuyForOrderPool = null;
							}

							_sellOrder = this.Exchange.CancelOrder(_sellOrder);// Здесь не будем проверять OrderFilledWhileWeCancelingException т.к. раз усреднились то рабочий sell стоит высоко далеко
							_sellOrder = DCA.BigSellOrder();
							this.SellCounter++;
						}
						DCA.Rollback();
					}
					if (DCA.CheckNecessary())
					{
						DCA.NextStep();
						this.BuyCounter++;
					}
				}

				Thread.Sleep(Settings.CheckOrderTimeout);
			}

			throw new ManuallyStopException(_.Log19); // Остановился!
		}

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

		private bool Is9thFourConditions(Order sellOrder)
		{
			return this.IsXthFourSellConditions
				(
					sellOrder,
					Settings.XOrdersAheadSellLittleReminderPriceDroppedScalpingSituation,
					Settings.SecondsAfterLastUpdateSellLittleReminderPriceDroppedScalpingSituation,
					Settings.DropPercentSellLittleReminderPriceDroppedScalpingSituation,
					Settings.AheadOrdersVolumeSellLittleReminderPriceDroppedScalpingSituation
				);
		}

		/// <summary>
		/// Action by settings for [22] Sell Canceled Little Reminder Situation
		/// </summary>
		/// <param name="sellOrder"></param>
		private void SellOrderCanceledActionBySettings(Order sellOrder)
		{
			// >>>	SELL order canceled
			// (1)	Stop
			// (2)	Start a new iteration with the reminder budget (reminder part sell later) OR Stop
			// (3)	Start a new iteration with the full budget (reminder part sell later) OR Stop
			// (4)	Start a new iteration with the reminder budget (reminder part sell later) OR
			//		Start a new iteration with the full budget (reminder part sell later) OR Stop
			// (5)	Start a new iteration with the full budget (reminder part sell later) OR
			//		Start a new iteration with the reminder budget (reminder part sell later) OR Stop
			// (6)	Start a new iteration with the full budget (forget about reminder part) OR Stop

			// TODO local methods for each actions

			// (1)
			if (Settings.SellCanceledLittleReminderScalpingSituation == 0)
			{
				throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
			}

			// Get DOMs
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Compute reminder budget
			decimal reminderBudget = this.configuration.Budget - (sellOrder.Remainder * sellOrder.Price);

			// Get free balance 
			decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

			// (2)
			if (Settings.SellCanceledLittleReminderScalpingSituation == 1)
			{
				if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
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
			if (Settings.SellCanceledLittleReminderScalpingSituation == 2)
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
			if (Settings.SellCanceledLittleReminderScalpingSituation == 3)
			{
				if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
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

			// (5)
			if (Settings.SellCanceledLittleReminderScalpingSituation == 4)
			{
				if (balance > this.configuration.Budget)
				{
					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					throw new TinyOrderCanceledException("SELL order canceled - (5) action [1]");
				}
				else if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
				{
					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					// Change budget for the next iteration
					this.BudgetForNextIteration = reminderBudget;

					throw new TinyOrderCanceledException("SELL order canceled - (5) action [2]");
				}
				else
				{
					throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
				}
			}

			// (6)
			if (Settings.SellCanceledLittleReminderScalpingSituation == 5)
			{
				if (balance > this.configuration.Budget)
				{
					throw new TinyOrderCanceledException("SELL order canceled - (6) action");
				}
				else
				{
					throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
				}
			}
		}

		/// <summary>
		/// [refactored] Actions by settings for [23] Sell Little Reminder Price Dropped Situation
		/// </summary>
		/// <param name="sellOrder"></param>
		private void SellOrderLittleReminderPriceDroppedActionsBySettings(Order sellOrder)
		{
			// >>>	SELL order with little reminder left, but price increased
			// (1)	Wait
			// (2)	Start a new iteration with the reminder budget (reminder part sell later) OR Wait
			// (3)	Start a new iteration with the full budget (reminder part sell later) OR Wait
			// (4)	Start a new iteration with the reminder budget (reminder part sell later) OR
			//		Start a new iteration with the full budget (reminder part sell later) OR Wait
			// (5)	Start a new iteration with the full budget (reminder part sell later) OR
			//		Start a new iteration with the reminder budget (reminder part sell later) OR Wait
			// (6)	Start a new iteration with the full budget (forget about reminder part) OR Wait

			// TODO local methods for each actions

			// (1)
			// Just nothing to do

			// Get DOMs
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Compute reminder budget
			decimal reminderBudget = this.configuration.Budget - (sellOrder.Remainder * sellOrder.Price);

			// Get free balance 
			decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

			// (2)
			if (Settings.SellLittleReminderPriceDroppedScalpingSituation == 1)
			{
				if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					// Change budget for the next iteration
					this.BudgetForNextIteration = reminderBudget;

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (2) action");
				}
				// else Wait
			}

			// (3)
			if (Settings.SellLittleReminderPriceDroppedScalpingSituation == 2)
			{
				if (balance > this.configuration.Budget)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (3) action");
				}
				// else Wait
			}

			// (4)
			if (Settings.SellLittleReminderPriceDroppedScalpingSituation == 3)
			{
				if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					// Change budget for the next iteration
					this.BudgetForNextIteration = reminderBudget;

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (4) action [1]");
				}
				else if (balance > this.configuration.Budget)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (4) action [2]");
				}
				// else Wait
			}

			// (5)
			if (Settings.SellLittleReminderPriceDroppedScalpingSituation == 4)
			{
				if (balance > this.configuration.Budget)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (5) action [1]");
				}
				else if (reminderBudget > this.Exchange.MinCost && Calc.ComputeBuyAmountByBudget(reminderBudget, doms["bids"][0].Price, this.Exchange.StepSize) > this.Exchange.MinAmount)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					// Remember the reminder
					this.Reminder.Add(ReminderHelper.OrderToReminderItem(sellOrder, this.Trades));

					// Change budget for the next iteration
					this.BudgetForNextIteration = reminderBudget;

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (5) action [2]");
				}
				// else Wait
			}

			// (6)
			if (Settings.SellLittleReminderPriceDroppedScalpingSituation == 5)
			{
				if (balance > this.configuration.Budget)
				{
					sellOrder = this.Exchange.CancelOrder(sellOrder);

					this.Trades.AddOrder(sellOrder);

					throw new TinyOrderCanceledException("Sell Little Reminder Price Dropped - (6) action");
				}
				// else Wait
			}
		}

		// Получение приоритетной цены, если в алгоритме она была указана (в Stop Loss будет)
		/*public decimal? SetSellPrice()
        {
            var result = this.SellPricePriority;
            this.SellPricePriority = null;
            return result ?? null;
        }*/

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
		/// [refactored] After sold proceesing: profit computing, clear dca
		/// </summary>
		private void AfterSoldProcessing()
        {
			// Profit computing

			Logger.ToFile("Profit compute...");

            this.Profit = new Profit(this.Trades, this.Reminder, this.Exchange);


			// Clear DCA

			if (this.configuration.IsDCA)
			{
				Logger.Debug("code 46");

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
