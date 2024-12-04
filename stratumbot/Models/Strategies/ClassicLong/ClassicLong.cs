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
	public class ClassicLong : IStrategy, IDCAble
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
		public ClassicLongConfig configuration { get; set; }

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
		/// Current order's ID
		/// </summary>
		public string OrderId { get; set; }

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
		/// Max bid price that was reached (for trailing profit)
		/// </summary>
		public decimal MaxPrice { get; set; }

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
		/// Priority sell price (used in profit trailing at least)
		/// </summary>
		public decimal? SellPricePriority { get; set; }

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
		/// [refactored] Classic Long Constructor
		/// </summary>
		public ClassicLong(ClassicLongConfig config, Exchange exchange)
		{
			// Set strategy config
			this.configuration = config;

			// Set current exchange
			this.Exchange = AvailableExchanges.CreateExchangeById(exchange);

			// DCA init
			this.NewDCAInit();

			// Init sell filter
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

			if (this.cancellationToken.IsCancellationRequested) { throw new ManuallyStopException(_.Log19); }

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
			Logger.ToFile("Classic Long : Stop Trade ()");
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

			this.MaxPrice = 0;

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

				Order buyOrder = new Order();

				if (this.configuration.IsMarketBuy)
				{
					if (this.Exchange.Name == "Binance")
					{
						// [loop] BUY
						buyOrder = this.Exchange.OpenMarketOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, null, null, quote: this.BudgetForNextIteration ?? this.configuration.Budget);
					}
					// Budget AS IS for the Binance Futures
					if (this.Exchange.Name == "Binance Futures")
					{
						var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);
						decimal next = doms["bids"][0].Price + this.Exchange.TickSize;
						this.BuyPrice = doms["asks"][0].Price != next ? next : doms["bids"][0].Price;

						// Compute the amount from the budget
						this.Amount = Calc.ComputeBuyAmountByBudget(this.BudgetForNextIteration ?? this.configuration.Budget, this.BuyPrice, this.Exchange.StepSize);

						// [loop] BUY
						buyOrder = this.Exchange.OpenMarketOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount, null, null);
					}


					// Get the amount and the price afterwords, because order was quoteOrderQty type
					this.Amount = buyOrder.Amount;
					this.BuyPrice = buyOrder.Price;
				}
				else // Limit order
				{
					// BuyPrice = last+1 or last if there is no spread
					var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);
					decimal next = doms["bids"][0].Price + this.Exchange.TickSize;
					this.BuyPrice = doms["asks"][0].Price != next ? next : doms["bids"][0].Price;

					// Compute the amount from the budget
					this.Amount = Calc.ComputeBuyAmountByBudget(this.BudgetForNextIteration ?? this.configuration.Budget, this.BuyPrice, this.Exchange.StepSize);

					// Budget AS IS for the Binance Futures
					//if (this.Exchange.Name == "Binance Futures") this.Amount = this.configuration.Budget;

					// [loop] BUY
					buyOrder = this.Exchange.OpenLimitOrder(OrderSide.BUY, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.BuyPrice);
				}

				// Set the buy counter up
				this.BuyCounter++;

				// TODO Timer Times.LastBuyOrder = -1;
				TThreadInfo.Times[Core.TID.CurrentID].LastBuyOrder = -1;

				Thread.Sleep(Settings.BetweenRequestTimeout);

				// [loop] Check the BUY order
				if (this.IsBUYFilled(buyOrder)) break; // Order was filled
				else continue; // If order was canceled (for re-placing) - Go back to the filter monitoring
			}
		}

		/// <summary>
		/// [refactored] Actions by setting if BUY order was canceled
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

			if (buyOrder.Filled > this.Exchange.MinAmount && buyOrder.Filled * this.GetMinProfitSellPrice() > this.Exchange.MinCost)
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
		/// [tested] [refactored] Get sell price that will give the minimal profit
		/// </summary>
		/// <returns></returns>
		public decimal GetMinProfitSellPrice(decimal? buyPrice = null)
		{
			// Fee rate
			decimal buyFeeRate = this.configuration.IsMarketBuy ? this.Exchange.TakerFEE : this.Exchange.MakerFEE;
			decimal sellFeeRate = this.Exchange.TakerFEE; // TODO now just sell by merket...
			return
			buyPrice ?? this.BuyPrice
			/* Min profit markup */
			+ Calc.AmountOfPercent(this.configuration.TargetProfitPercent, buyPrice ?? this.BuyPrice)
			/* BUY fee */
			+ Calc.AmountOfPercent(buyFeeRate, buyPrice ?? this.BuyPrice)
			/* SELL approximate fee */
			+ Calc.AmountOfPercent(sellFeeRate, (buyPrice ?? this.BuyPrice + Calc.AmountOfPercent(this.configuration.TargetProfitPercent, buyPrice ?? this.BuyPrice)));
		}

		/// <summary>
		/// [refactored] Actions by setting when [1] Buy Canceled Situation
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
			if (Settings.BuyCanceledClassicLongSituation == 0)
			{
				throw new AutoStopException("Поток остановлен в соответствии с настройками"); // TODO norm text
			}

			this.Trades.AddOrder(buyOrder);

			// Get DOMs
			var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2);

			// Compute reminder budget
			decimal reminderBudget = this.configuration.Budget - (buyOrder.Filled * buyOrder.Price);

			// Get free balance 
			decimal balance = this.Exchange.GetBalance(this.configuration.Cur2);

			// (2)
			if (Settings.BuyCanceledClassicLongSituation == 1)
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
			if (Settings.BuyCanceledClassicLongSituation == 2)
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
			if (Settings.BuyCanceledClassicLongSituation == 3)
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
		/// [refactored] Actions by settings for [2] Buy Little Filled Price Increased Situation
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
			if (Settings.BuyLittleFilledPriceIncreasedClassicLongSituation == 1)
			{
				if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
				// else (1) Wait
			}

			// (3)
			if (Settings.BuyLittleFilledPriceIncreasedClassicLongSituation == 2)
			{
				if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
				// else (1) Wait
			}

			// (4)
			if (Settings.BuyLittleFilledPriceIncreasedClassicLongSituation == 3)
			{
				if (isReminderBudgetGreaterMinimals() && isFreeBalanceGreaterReminderBudget()) newIterationWithReminderBudget();
				else if (isFreeBalanceGreaterFullBudget()) newIterationWithFullBudget();
				// else (1) Wait 
			}

			// (5)
			if (Settings.BuyLittleFilledPriceIncreasedClassicLongSituation == 4)
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
		/// [tested] [refactored] [6] Four waiting conditions for [2] Buy Little Filled Price Increased Situation
		/// </summary>
		/// <returns></returns>
		private bool Is6thFourConditions(Order buyOrder)
		{
			return this.IsXthFourBuyConditions
				(
					buyOrder,
					Settings.XOrdersAheadLittleFilledPriceIncreasedClassicLongSituation,
					Settings.SecondsAfterLastUpdateLittleFilledPriceIncreasedClassicLongSituation,
					Settings.DropPercentLittleFilledPriceIncreasedClassicLongSituation,
					Settings.AheadOrdersVolumeLittleFilledPriceIncreasedClassicLongSituation
				);
		}

		/// <summary>
		/// [tested] [refactored] [7] Four waiting conditions for [8] Buy Filled Enough Price Increased
		/// </summary>
		/// <param name="buyOrder"></param>
		/// <returns></returns>
		private bool Is7thFourConditions(Order buyOrder)
		{
			return this.IsXthFourBuyConditions
				(
					buyOrder,
					Settings.XOrdersAheadBuyFilledEnoughPriceIncreased,
					Settings.SecondsAfterLastUpdateBuyFilledEnoughPriceIncreased,
					Settings.DropPercentBuyFilledEnoughPriceIncreased,
					Settings.AheadOrdersVolumeBuyFilledEnoughPriceIncreased
				);
		}

		/// <summary>
		/// [refactored] Checking the BUY order for filling.
		/// 
		/// true - order was filled - continue the iteratin - place the SELL order
		/// true - filled amount > minimals - continue the iteratin - place the SELL order
		/// false - filled amount less than minimals - remember it, will sell next iteratin - place new BUY order
		/// false - no filled amount - place new BUY order.
		/// </summary>
		private bool IsBUYFilled(Order _buyOrder)
		{
			while (!this.cancellationToken.IsCancellationRequested)
			{
				Logger.Info(_.Log10); // Проверяем BUY...

				// Get order info
				_buyOrder = Exchange.GetOrderInfo(_buyOrder);

				// TODO надо ли рассчитать AVG buy цену т.к. по разной цене исполнися

				if (_buyOrder.Status == OrderStatus.Filled)
				{
					Logger.Info(_.Log11); // BUY исполнен! Ставим ордер на продажу...

					this.Trades.AddOrder(_buyOrder);

					return true; // BUY filled : place SELL order
				}
				if (_buyOrder.Status == OrderStatus.Canceled)
				{
					return this.CanceledAction(_buyOrder);
				}
				if (_buyOrder.Status == OrderStatus.PartiallyFilled)
				{
					Logger.Info(_.Log13); // Ордер частично исполнен

					// Get DOMs
					var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

					// TODO TEST what if there is no filters
					bool isFiltersAllowed = this.configuration.Filters.CheckFilters(this.configuration.Filters.BuyFilters, this.configuration.Filters.TargetPointBuy);
					bool isReminderMoreMinimals = (_buyOrder.Remainder > this.Exchange.MinAmount && (_buyOrder.Remainder * (doms["bids"][0].Price + this.Exchange.TickSize)) > this.Exchange.MinCost);
					bool isFilledAmountMoreMinimals = (_buyOrder.Filled > this.Exchange.MinAmount && (_buyOrder.Filled * (doms["asks"][0].Price - this.Exchange.TickSize)) > this.Exchange.MinCost);

					// TODO check the orders volume as in Scalping
					// (1) our order is first: price of first bid = our order price
					bool isOrderFirst = doms["bids"][0].Price == _buyOrder.Price;
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

							// [6] Four waiting conditions for [2] Buy Little Filled Price Increased Situation
							if (!this.Is6thFourConditions(_buyOrder)) continue;

							// Action by setting
							this.BuyLittleFilledPriceIncreasedActionsBySettings(_buyOrder);

						}
					}

					// (2) our order too high above second one - inefficient price
					bool isBackTrailingNeeded = isOrderFirst && doms["bids"][1].Price != (_buyOrder.Price - this.Exchange.TickSize);
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
								/// Ордер исполнился пока мы его отменяли
								Logger.ToFile("[3]" + ex.Message);
								Logger.Info(_.Log17);
								continue;  /// продолжаем
							}
						}
					}
				}
				if (_buyOrder.Status == OrderStatus.New)
				{
					// Get DOMs
					var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

					// TODO check the orders volume as in Scalping
					// (1) our order is first: price of first bid = our order price
					bool isOrderFirst = doms["bids"][0].Price == _buyOrder.Price;

					// (2) our order too high above second one - inefficient price
					bool isBackTrailingNeeded = isOrderFirst && doms["bids"][1].Price != (_buyOrder.Price - this.Exchange.TickSize);

					if (!isOrderFirst || isBackTrailingNeeded)
					{
						if (!isOrderFirst) Logger.Info($"{_.Log18} {_buyOrder.Price} < {doms["bids"][0].Price} [1]");
						else Logger.Info($"{_.Log18} ({_buyOrder.Price} - {this.Exchange.TickSize}) ≠ {doms["bids"][1].Price}");

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

			// StopLoss computing
			if (this.configuration.IsStopLoss)
			{
				Logger.ToFile($"StopLoss computing...");

				// (1) Percent of price dropping [%]
				if (this.configuration.IsStopLossAsPercent)
				{
					this.configuration.StopLossPrice = this.BuyPrice - Calc.AmountOfPercent(this.configuration.StopLossPercent, this.BuyPrice);
					this.configuration.StopLossPrice = Calc.RoundUp(this.configuration.StopLossPrice, this.Exchange.TickSize);
					Logger.ToFile($"StopLoss (1) {this.configuration.StopLossPrice} {this.configuration.StopLossPercent} {this.BuyPrice}");
				}

				// (2) Minus pints of buy price [-]
				if (this.configuration.IsStopLossAsMinusPoints)
				{
					this.configuration.StopLossPrice = this.BuyPrice + this.configuration.StopLossMinus; // + потому что по сути отнимет т.к. StopLoss с минусом
					this.configuration.StopLossPrice = Calc.RoundUp(this.configuration.StopLossPrice, this.Exchange.TickSize);
					Logger.ToFile($"StopLoss (2) {this.configuration.StopLossPrice} {this.configuration.StopLossMinus} {this.BuyPrice}");
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
						this.StopLossTriggerPrice = this.configuration.StopLossPrice + Calc.AmountOfPercent(this.configuration.StopLossApproximation, this.configuration.StopLossPrice);
						Logger.ToFile($"StopLoss l(4)% {this.StopLossTriggerPrice} {this.configuration.StopLossPrice} {this.configuration.StopLossApproximation}");
					}
					else // as points
					{
						this.StopLossTriggerPrice = this.configuration.StopLossPrice + this.configuration.StopLossApproximation;
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
				this.DCA.Compute();
			}
		}


		// ***************************************************************************************************************
		// Selling

		/// <summary>
		/// [refactored] Selling process
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
				if (this.Reminder.Count > 0)
				{
					decimal buyPriceWithAdditionalAmount = ReminderHelper.GetAvgPriceWithAdditionalAmountForSell(this.Reminder, this.Amount, this.BuyPrice);

					// SetSellPrice - for profit trailing; ?? first limit order by min.profit
					this.SellPrice = /*this.SetSellPrice() ??*/ this.GetMinProfitSellPrice(buyPriceWithAdditionalAmount);
					this.SellPrice = Calc.RoundUp(this.SellPrice, this.Exchange.TickSize);

					return this.SellPrice;
				}
				else
				{
					if (this.SellPricePriority != null)
					{
						this.SellPrice = (decimal)this.SellPricePriority;
						this.SellPricePriority = null;
						return this.SellPrice;
					}
					// SetSellPrice - for profit trailing; ?? first limit order by min.profit
					this.SellPrice = /*this.SetSellPrice() ??*/ this.GetMinProfitSellPrice();
					this.SellPrice = Calc.RoundUp(this.SellPrice, this.Exchange.TickSize);

					return this.SellPrice;
				};
			}

			while (!this.cancellationToken.IsCancellationRequested)
			{
				Order sellOrder = new Order();

				// Sell by filters
				if (this.configuration.IConfigText.FiltersSell.Count > 0)
				{
					// Looking for filters allowance
					bool isFiltersAllowed = this.configuration.Filters.FiltersMonitoring(this.configuration.Filters.SellFilters, (this.configuration.Filters.TargetPointSell));
					if (!isFiltersAllowed)
					{
						// FILTERS + STOPLOSS EXPERIMENT
						var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 10);
						// TODO merge new and part methods into local or external method
						// StopLoss
						if (this.configuration.IsStopLoss)
						{
							// Get stoploss price depend on will it be as limit or as market order
							decimal getStoplossPrice()
							{
								if (this.configuration.StopLossApproximation > 0) return this.configuration.StopLossPrice;
								else return Tools.StopLoss.GetApproximatePrice(this.Amount, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
							}

							// StopLoss condition was triggered
							bool stopLossNecessary = doms["asks"][0].Price <= this.StopLossTriggerPrice;

							bool filtersAllowed = this.configuration.IConfigText.FiltersStopLoss.Count == 0 || this.configuration.Filters.CheckStopLossFilters(this.configuration.Filters.StopLossFilters, this.configuration.Filters.TargetPointStopLoss);

							// Check for minimals is it possible to stoploss
							//bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_sellOrder, this.Exchange, getStoplossPrice());

							// Yes, stoploss we need!
							if (stopLossNecessary && IsStopLossTimerTimedOut() && filtersAllowed/* && reminderGreaterMinimals*/)
							{
								// Cancel current SELL order
								try
								{
									Logger.Info(_.Log49); // Сработало условие на стоплосс

									//_sellOrder = this.Exchange.CancelOrder(_sellOrder);

									// Just in case
									//if (_sellOrder.Status == OrderStatus.Filled) return true;

									// Update amount so stoploss can placed for reminder amount
									//this.Amount = _sellOrder.Remainder;

									// Check reminder again because it can change while canceling
									//if (_sellOrder.Remainder > this.Exchange.MinAmount) continue; // Go to CANCELED
									//if (_sellOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;

									//this.Trades.AddOrder(_sellOrder);

									Order stopLoss = new Order();

									// It possible to use the reminder here, compute the amount and price, and nullify then. But let's use the reminder in the next iteration

									// TODO create local methods for both stoploss (new and part) for market and limit orders

									// SL as limit order
									if (this.configuration.StopLossApproximation > 0)
									{
										// TODO сделать в будущем чтобы ордер не забывался, а следил за ним пока не исполнится, и в случае чего отменял stoploss и возращался к обычному ордеру с профитом
										stopLoss = this.Exchange.OpenLimitOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.configuration.StopLossPrice);

										// TODO BUG 57162415 order will be added but filled amount = 0, so stat should be incorrect
										this.Trades.AddOrder(stopLoss);

										this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

										// TODO why true??? stat will be fucked up
										break; // Ордер исполнится, выходим из итерации
									}
									// as market order
									else
									{
										stopLoss = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

										stopLoss = this.Exchange.GetOrderInfo(stopLoss);

										this.Trades.AddOrder(stopLoss);

										// For the stopping after SL if the feature turned on
										this.IsStopLossTriggered = true;

										// TODO add the counter of stoploss in TTHREAD

										// Set the sell counter up
										this.SellCounter++;

										// TODO Timer Times.LastSellOrder
										TThreadInfo.Times[Core.TID.CurrentID].LastSellOrder = -1;

										break; // StopLoss SELL order was filled
									}
								}
								catch (OrderFilledWhileWeCancelingException ex)
								{
									Logger.ToFile("[4]" + ex.Message); // Ордер исполнился пока мы его отменяли
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
							if (!stopLossNecessary || !filtersAllowed/* || !reminderGreaterMinimals*/)
							{
								// Set time to null, becauseprice increased and we didnt need stoploss
								this.StopLossTime = 0;
							}
						}
						// FILTERS + STOPLOSS EXPERIMENT


						Thread.Sleep(Settings.FiltersTimeout);
						continue;
					}

					// Check if first 10 orders enought to get min profit if filled by market right now
					if (!this.IsAskAmountEnough()) // TODO FUTURE это можно проверить перед фильтрами, чтобы если не первый ордер будем забирать проверить его цену по фильтрам
						continue;

					// [loop] SELL
					sellOrder = this.Exchange.OpenMarketOrder(
						OrderSide.SELL, 
						this.configuration.Cur1, 
						this.configuration.Cur2,
						GetAmount()
					);
				}
				// Sell by profit trailing or just by min.profit
				else
				{
					// [loop] SELL
					sellOrder = this.Exchange.OpenLimitOrder(
						OrderSide.SELL,
						this.configuration.Cur1,
						this.configuration.Cur2,
						GetAmount(),
						GetSellPrice()
					);
				}

				// Set the sell counter up
				this.SellCounter++;

				// TODO Timer Times.LastSellOrder
				TThreadInfo.Times[Core.TID.CurrentID].LastSellOrder = -1;

				Thread.Sleep(Settings.BetweenRequestTimeout);

				// [loop] Check the SELL order (and DCA BUY)
				if (this.IsSELLFilled(sellOrder)) break; // SELL-order was filled
				else continue; // Goto to re-place with new markup/price
			}
		}

		/// <summary>
		/// [refactored] Check if ask volume of first 10 orders is enough to filled our market order
		/// </summary>
		/// <returns></returns>
		public bool IsAskAmountEnough()
		{
			this.SellPrice = 0;
			decimal haveAmount = 0;
			var doms = Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 10);

			foreach (var bid in doms["bids"])
			{
				haveAmount += bid.Amount;

				// TODO CHECK if amount is proper variable, because prev conf.Budget was here
				if (haveAmount > this.Amount)
				{
					if (bid.Price >= this.GetMinProfitSellPrice())
					{
						this.SellPrice = bid.Price;
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
				if (_sellOrder.Status == OrderStatus.Canceled)
				{
					if (_sellOrder.Remainder > this.Exchange.MinAmount && (_sellOrder.Remainder * this.GetMinProfitSellPrice()) > this.Exchange.MinCost)
					{
						// TODO add filed part to Trades?
						this.Trades.AddOrder(_sellOrder);

						// If DCA on copmute again by reminder part
						if (this.configuration.IsDCA) DCA.Compute();

						// Set new amount
						this.Amount = _sellOrder.Remainder;

						return false; // Place sell order again with reminder amount
					}

					this.SellOrderCanceledActionBySettings(_sellOrder);
				}

				// Get DOM
				var doms = this.Exchange.GetDOM(this.configuration.Cur1, this.configuration.Cur2, 2);

				// Max price reached update
				if (doms["bids"][0].Price > this.MaxPrice) this.MaxPrice = doms["bids"][0].Price;

				if (_sellOrder.Status == OrderStatus.PartiallyFilled)
				{
					Logger.Info(_.Log13); // Ордер частично исполнен

					if (_sellOrder.Remainder < this.Exchange.MinAmount && _sellOrder.Remainder * doms["bids"][0].Price < this.Exchange.MinCost)
					{
						if (this.Is9thFourConditions(_sellOrder)) this.SellOrderLittleReminderPriceDroppedActionsBySettings(_sellOrder);
					}

					// TODO merge new and part methods into local or external method
					// StopLoss
					if (this.configuration.IsStopLoss)
					{
						// Get stoploss price depend on will it be as limit or as market order
						decimal getStoplossPrice()
						{
							if (this.configuration.StopLossApproximation > 0) return this.configuration.StopLossPrice;
							else return Tools.StopLoss.GetApproximatePrice(_sellOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
						}

						// StopLoss condition was triggered
						bool stopLossNecessary = doms["asks"][0].Price <= this.StopLossTriggerPrice;

						bool filtersAllowed = this.configuration.IConfigText.FiltersStopLoss.Count == 0 || this.configuration.Filters.CheckStopLossFilters(this.configuration.Filters.StopLossFilters, this.configuration.Filters.TargetPointStopLoss);

						// Check for minimals is it possible to stoploss
						bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_sellOrder, this.Exchange, getStoplossPrice());

						// Yes, stoploss we need!
						if (stopLossNecessary && IsStopLossTimerTimedOut() && filtersAllowed && reminderGreaterMinimals)
						{
							// Cancel current SELL order
							try
							{
								Logger.Info(_.Log49); // Сработало условие на стоплосс

								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								// Just in case
								if (_sellOrder.Status == OrderStatus.Filled) return true;

								// Update amount so stoploss can placed for reminder amount
								this.Amount = _sellOrder.Remainder;

								// Check reminder again because it can change while canceling
								if (_sellOrder.Remainder > this.Exchange.MinAmount) continue; // Go to CANCELED
								if (_sellOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;

								this.Trades.AddOrder(_sellOrder);

								Order stopLoss = new Order();

								// It possible to use the reminder here, compute the amount and price, and nullify then. But let's use the reminder in the next iteration

								// TODO create local methods for both stoploss (new and part) for market and limit orders

								// SL as limit order
								if (this.configuration.StopLossApproximation > 0)
								{
									// TODO сделать в будущем чтобы ордер не забывался, а следил за ним пока не исполнится, и в случае чего отменял stoploss и возращался к обычному ордеру с профитом
									stopLoss = this.Exchange.OpenLimitOrder(OrderSide.SELL, 
										this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.configuration.StopLossPrice);

									// TODO BUG 57162415 order will be added but filled amount = 0, so stat should be incorrect
									this.Trades.AddOrder(stopLoss);

									this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

									// TODO why true??? stat will be fucked up
									return true; // Ордер исполнится, выходим из итерации
								}
								// as market order
								else
								{
									stopLoss = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, 
										this.configuration.Cur2, this.Amount);

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
								Logger.ToFile("[4]" + ex.Message); // Ордер исполнился пока мы его отменяли
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
						decimal TrailStepTriggerPrice = this.SellPrice - Calc.AmountOfPercent(this.configuration.ApproximationPercent, this.SellPrice);
						// Profit trailing stop-profit price (max - unapproximate)
						decimal ProfitStopTriggerPrice = this.MaxPrice - Calc.AmountOfPercent(this.configuration.UnApproximationPercent, this.MaxPrice);
						// Min target profit price
						decimal TargetProfitSellPrice = this.BuyPrice + Calc.AmountOfPercent(this.configuration.TargetProfitPercent, this.BuyPrice); // buyPrice + TargetProfitpercent

						Logger.ToFile($"check 2: {doms["bids"][0].Price} <  {ProfitStopTriggerPrice} && {doms["bids"][0].Price} > {TargetProfitSellPrice}");

						// TODO future check exact price in doms depend on our amount
						if (doms["bids"][0].Price < ProfitStopTriggerPrice && doms["bids"][0].Price > TargetProfitSellPrice)
						{
							Logger.Info(_.Log53); // Цена начала падать — SELL по рынку

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								if (_sellOrder.Status == OrderStatus.Filled) continue;

								if (_sellOrder.Remainder < this.Exchange.MinAmount && Tools.StopLoss.GetApproximatePrice(_sellOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2) < this.Exchange.MinCost) continue;

								this.Trades.AddOrder(_sellOrder);

								// SELL by market
								_sellOrder = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

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

						if (doms["bids"][0].Price >= TrailStepTriggerPrice)
						{
							Logger.Info(_.Log52); // Cработал трейлинг профит, переставляем ордер выше

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								_sellOrder = this.Exchange.GetOrderInfo(_sellOrder);

								if (_sellOrder.Status == OrderStatus.Filled) continue;

								// Set sell price priority, so next sell order will be by price (this.SellPrice + TrailStepPercent)
								this.SellPricePriority = this.SellPrice + Calc.AmountOfPercent(this.configuration.TrailStepPercent, this.SellPrice);
								this.SellPricePriority = Calc.RoundUp((decimal)this.SellPricePriority, this.Exchange.TickSize);

								if (_sellOrder.Filled > 0) continue; // It will lead to CANCELED actions by setting or re place sell ordaer if amount > minimals
								// TODO test never goes here because it is a partically filled oreder, there always will be filled part
								this.Trades.AddOrder(_sellOrder);
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
				if (_sellOrder.Status == OrderStatus.New)
				{
					// TODO merge new and part methods into local or external method
					// StopLoss
					if (this.configuration.IsStopLoss)
					{
						// Get stoploss price depend on will it be as limit or as market order
						decimal getStoplossPrice()
						{
							if (this.configuration.StopLossApproximation > 0) return this.configuration.StopLossPrice;
							else return Tools.StopLoss.GetApproximatePrice(_sellOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2);
						}

						// StopLoss condition was triggered
						bool stopLossNecessary = doms["asks"][0].Price <= this.StopLossTriggerPrice;

						bool filtersAllowed = this.configuration.IConfigText.FiltersStopLoss.Count == 0 || this.configuration.Filters.CheckStopLossFilters(this.configuration.Filters.StopLossFilters, this.configuration.Filters.TargetPointStopLoss);

						// Check for minimals is it possible to stoploss
						bool reminderGreaterMinimals = Tools.StopLoss.CheckMin(_sellOrder, this.Exchange, getStoplossPrice());

						// Yes, stoploss we need!
						if (stopLossNecessary && IsStopLossTimerTimedOut() && filtersAllowed && reminderGreaterMinimals)
						{
							Logger.ToFile("[debug] 5656865659566"); // TODO delete
							
							// Cancel current SELL order
							try
							{
								Logger.Info(_.Log49); // Сработало условие на стоплосс

								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								// Just in case
								if (_sellOrder.Status == OrderStatus.Filled) return true;

								// Update amount so stoploss can placed for reminder amount
								this.Amount = _sellOrder.Remainder;

								Order stopLoss = new Order();

								// Check reminder again because it can change while canceling
								if (_sellOrder.Filled != 0)
								{
									if (_sellOrder.Remainder > this.Exchange.MinAmount) continue;
									if (_sellOrder.Remainder * getStoplossPrice() > this.Exchange.MinCost) continue;
								}

								this.Trades.AddOrder(_sellOrder);

								// It possible to use the reminder here, compute the amount and price, and nullify then. But let's use the reminder in the next iteration

								// SL as limit order
								if (this.configuration.StopLossApproximation > 0)
								{
									// TODO сделать в будущем чтобы ордер не забывался, а следил за ним пока не исполнится, и в случае чего отменял stoploss и возращался к обычному ордеру с профитом
									stopLoss = this.Exchange.OpenLimitOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount, this.configuration.StopLossPrice);

									this.Trades.AddOrder(stopLoss);

									this.IsStopLossTriggered = true; // Для остановки потока после SL если нужно

									// TODO why true??? stat will be fucked up
									return true; // Ордер исполнится, выходим из итерации
								}
								// as market order
								else
								{
									stopLoss = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

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
						decimal TrailStepTriggerPrice = this.SellPrice - Calc.AmountOfPercent(this.configuration.ApproximationPercent, this.SellPrice);
						// Profit trailing stop-profit price (max - unapproximate)
						decimal ProfitStopTriggerPrice = this.MaxPrice - Calc.AmountOfPercent(this.configuration.UnApproximationPercent, this.MaxPrice);
						// Min target profit price
						decimal TargetProfitSellPrice = this.BuyPrice + Calc.AmountOfPercent(this.configuration.TargetProfitPercent, this.BuyPrice); // buyPrice + TargetProfitpercent

						Logger.ToFile($"check 2: {doms["bids"][0].Price} <  {ProfitStopTriggerPrice} && {doms["bids"][0].Price} > {TargetProfitSellPrice}");
						
						// TODO future check exact price in doms depend on our amount
						if (doms["bids"][0].Price < ProfitStopTriggerPrice && doms["bids"][0].Price > TargetProfitSellPrice)
						{
							Logger.Info(_.Log53); // Цена начала падать — SELL по рынку

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								if (_sellOrder.Status == OrderStatus.Filled) continue;

								if (_sellOrder.Remainder < this.Exchange.MinAmount && Tools.StopLoss.GetApproximatePrice(_sellOrder.Remainder, this.Exchange, this.configuration.Cur1, this.configuration.Cur2) < this.Exchange.MinCost) continue;

								this.Trades.AddOrder(_sellOrder);

								// SELL by market
								_sellOrder = this.Exchange.OpenMarketOrder(OrderSide.SELL, this.configuration.Cur1, this.configuration.Cur2, this.Amount);

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

						if (doms["bids"][0].Price >= TrailStepTriggerPrice)
						{
							Logger.Info(_.Log52); // Cработал трейлинг профит, переставляем ордер выше

							try
							{
								_sellOrder = this.Exchange.CancelOrder(_sellOrder);

								_sellOrder = this.Exchange.GetOrderInfo(_sellOrder);

								if (_sellOrder.Status == OrderStatus.Filled) continue;

								// Set sell price priority, so next sell order will be by price (this.SellPrice + TrailStepPercent)
								this.SellPricePriority = this.SellPrice + Calc.AmountOfPercent(this.configuration.TrailStepPercent, this.SellPrice);
								this.SellPricePriority = Calc.RoundUp((decimal)this.SellPricePriority, this.Exchange.TickSize);

								if (_sellOrder.Filled > 0) continue; // It will lead to CANCELED actions by setting or re place sell ordaer if amount > minimals

								this.Trades.AddOrder(_sellOrder);

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
					DCA.CurrentPrice = doms["asks"][0].Price;
					DCA.Trades = this.Trades;
					DCA.SellOrder = _sellOrder;

					if (DCA.IsTriggered)
					{
						// TODO Times

						if (DCA.IsBuyFilled())
						{
							if (DCA.BuyForOrderPool != null)
							{
								this.Trades.AddOrder(DCA.BuyForOrderPool);
								DCA.Trades = this.Trades;
								DCA.BuyForOrderPool = null;
							}

							_sellOrder = this.Exchange.CancelOrder(_sellOrder); // Здесь не будем проверять OrderFilledWhileWeCancelingException т.к. раз усреднились то рабочий sell стоит высоко далеко
							_sellOrder = DCA.BigSellOrder();
							this.SellCounter++;
							// TODO Times
							TThreadInfo.Times[Core.TID.CurrentID].LastSellOrder = -1;
						}
						DCA.Rollback();
					}
					if (DCA.CheckNecessary())
					{
						DCA.NextStep();
						this.BuyCounter++;
						TThreadInfo.Times[Core.TID.CurrentID].LastBuyOrder = -1;
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
					Settings.XOrdersAheadSellLittleReminderPriceDroppedClassicLongSituation,
					Settings.SecondsAfterLastUpdateSellLittleReminderPriceDroppedClassicLongSituation,
					Settings.DropPercentSellLittleReminderPriceDroppedClassicLongSituation,
					Settings.AheadOrdersVolumeSellLittleReminderPriceDroppedClassicLongSituation
				);
		}

		/// <summary>
		/// [refactored] Action by settings for [4] Sell Canceled Little Reminder Situation
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
			if (Settings.SellCanceledLittleReminderClassicLongSituation == 0)
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
			if (Settings.SellCanceledLittleReminderClassicLongSituation == 1)
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
			if (Settings.SellCanceledLittleReminderClassicLongSituation == 2)
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
			if (Settings.SellCanceledLittleReminderClassicLongSituation == 3)
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
			if (Settings.SellCanceledLittleReminderClassicLongSituation == 4)
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
			if (Settings.SellCanceledLittleReminderClassicLongSituation == 5)
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
		/// [refactored] Actions by settings for [5] Sell Little Reminder Price Dropped Situation
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
			if (Settings.SellLittleReminderPriceDroppedClassicLongSituation == 1)
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
			if (Settings.SellLittleReminderPriceDroppedClassicLongSituation == 2)
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
			if (Settings.SellLittleReminderPriceDroppedClassicLongSituation == 3)
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
			if (Settings.SellLittleReminderPriceDroppedClassicLongSituation == 4)
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
			if (Settings.SellLittleReminderPriceDroppedClassicLongSituation == 5)
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

		/// <summary>
		/// [refactored] Get prioritized price, if it was set somewhere (in StopLoss it will be)
		/// </summary>
		/// <returns></returns>
		// TODO check if i need it. I never use
		public decimal? SetSellPrice()
		{
			decimal? result = this.SellPricePriority;
			this.SellPricePriority = null;
			return result ?? null;
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

			decimal left = this.StopLossTime - currentTime;
			Logger.Debug(String.Format("До срабатывания стоплосса осталось {0} сек.", (left > 0) ? left : 0));

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

			Logger.ToFile("Profit computing...");

			this.Profit = new Profit(this.Trades, this.Reminder, this.Exchange);

			// Clear DCA

			if (this.configuration.IsDCA)
			{
				Logger.Debug("code 47");

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