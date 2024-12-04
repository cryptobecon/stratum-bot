using Newtonsoft.Json;
using Skender.Stock.Indicators;
using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.DataProvider;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models.Filters.Implementations
{
	class DonchianChannel : IFilter
	{
		/// <summary>
		/// ID
		/// </summary>
		public string ID { get; set; } = "28";

		/// <summary>
		/// Name of the filter
		/// </summary>
		public string Name { get; set; } = "Donchian Channel";

		/// <summary>
		/// Data provider object
		/// </summary>
		public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();

		/// <summary>
		/// A list of data types required
		/// </summary>
		public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

		#region Filter's configuration

		/// <summary>
		/// Filter mode
		/// ***
		/// 0 - price > upper
		/// 1 - price > lower
		/// 2 - price < upper
		/// 3 - price < lower
		/// ***
		/// </summary>
		public int Mode = 0;

		/// <summary>
		/// EMA Period
		/// </summary>
		public int Period { get; set; } = 0;

		/// <summary>
		/// TimeFrame
		/// </summary>
		public string TimeFrame { get; set; } = "5m";

		#endregion

		#region General filter's configuration

		/// <summary>
		/// My name of the filter that can be changed
		/// </summary>
		public string MyName { get; set; } = "Donchian";

		/// <summary>
		/// Indent bellow or above
		/// </summary>
		public decimal Indent { get; set; } = 0;

		/// <summary>
		/// Duration of allowance of the filter's true signal
		/// </summary>
		public int Duration { get; set; } = 0;

		/// <summary>
		/// Allowance time = Time of the last update + Duration
		/// </summary>
		public decimal AllowedTime = 0;

		/// <summary>
		/// DOM from which Current Price will be taken 
		/// </summary>
		public string DepthSide = "Bid";

		/// <summary>
		/// Group
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Point costs
		/// </summary>
		public int Weight { get; set; } = 0;

		/// <summary>
		/// Color for displaying
		/// </summary>
		public System.Windows.Media.Brush Color { get; set; }

		#endregion

		#region Filter's side, result, current weight (never changed)

		/// <summary>
		/// Filters type BUY / SELL
		/// </summary>
		public string FilterSide { get; set; }

		/// <summary>
		/// Filetrs result - true if passed, false if not
		/// </summary>
		bool result;
		public bool Result
		{
			get
			{
				this.Compute();
				return result;
			}
			set
			{
				result = value;
				this.CurrentWeight = (value) ? this.Weight : 0;
			}
		}

		/// <summary>
		/// Get (but first compute) numbers of points for the filter
		/// </summary>
		int currentWeight;
		public int CurrentWeight
		{
			get
			{
				this.Compute();
				return currentWeight;
			}
			set { currentWeight = value; }
		}

		#endregion

		#region JSON

		/// <summary>
		/// DTO of JSON object of the filter
		/// </summary>
		class JsonObject
		{
			[JsonProperty("ID")]
			public string ID { get; set; }
			[JsonProperty("Mode")]
			public int Mode { get; set; }
			[JsonProperty("Period")]
			public int Period { get; set; }
			[JsonProperty("TimeFrame")]
			public string TimeFrame { get; set; }
			[JsonProperty("Indent")]
			public decimal Indent { get; set; }
			[JsonProperty("Duration")]
			public int Duration { get; set; }
		}

		/// <summary>
		/// Get JSON of the filter
		/// </summary>
		public string Json
		{
			get
			{
				var array = new JsonObject()
				{
					ID = this.ID,
					Mode = this.Mode,
					Period = this.Period,
					TimeFrame = this.TimeFrame,
					Indent = this.Indent,
					Duration = this.Duration
				};

				string json = JsonConvert.SerializeObject(array);

				return json;
			}
		}

		#endregion

		// Конструктор
		public DonchianChannel(int moreOrLess, int period, string timeFrame, decimal indent, int duration)
		{
			this.RequiredDataInit();

			this.Mode = moreOrLess;
			this.Period = period;
			this.TimeFrame = timeFrame;
			this.Indent = indent;
			this.Duration = duration;
			this.Result = false;
		}

		/// <summary>
		/// Add required data type for the filter to the list
		/// </summary>
		public void RequiredDataInit()
		{
			this.RequiredDataTypes.Add(DataType.Quotes);
		}

		/// <summary>
		/// Get options for specefic data type for the filter
		/// </summary>
		/// <param name="dataType">Data type of which options are needly</param>
		/// <param name="cur1">First currency</param>
		/// <param name="cur2">Second (base) currency</param>
		/// <returns>(DataOptions) options for specific data type</returns>
		public DataOptions GetOptions(DataType dataType, string cur1 = null, string cur2 = null)
		{
			if (dataType == DataType.Quotes)
			{
				return new DataOptions
				{
					Cur1 = cur1,
					Cur2 = cur2,
					TimeFrame = this.TimeFrame,
					Period = this.Period + 1
				};
			}

			throw new Exception("GetOptions() DataType doesn't sent");
		}

		public void Compute()
		{
			// Duration
			if (this.Duration != 0 && Time.CurrentSeconds() < this.AllowedTime)
			{
				this.Result = true;
				return;
			}

			// Filter Side
			decimal currentPrice = (this.DepthSide == "Bid") ? this.DataProvider.CurrentBuyPrice : this.DataProvider.CurrentSellPrice;

			IEnumerable<DonchianResult> results =
				this.DataProvider.Quotes.GetDonchian(this.Period);

			// Current point of the filter
			decimal currentPoint = 0;

			if (this.Mode == 0) // price > upper
			{
				currentPoint = (decimal)results.Last().UpperBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().UpperBand);

				Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} > {currentPoint}");
				if (currentPrice > currentPoint)
				{
					this.Result = true;
					this.AllowedTime = Time.CurrentSeconds() + this.Duration;
				}
				else
					this.Result = false;
			}

			if (this.Mode == 1) // price > lower
			{
				currentPoint = (decimal)results.Last().LowerBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().LowerBand);

				Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} < {currentPoint}");
				if (currentPrice > currentPoint)
				{
					this.Result = true;
					this.AllowedTime = Time.CurrentSeconds() + this.Duration;
				}
				else
					this.Result = false;
			}

			if (this.Mode == 2) // price < upper
			{
				currentPoint = (decimal)results.Last().UpperBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().UpperBand);

				Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} > {currentPoint}");
				if (currentPrice < currentPoint)
				{
					this.Result = true;
					this.AllowedTime = Time.CurrentSeconds() + this.Duration;
				}
				else
					this.Result = false;
			}

			if (this.Mode == 3) // price < lower
			{
				currentPoint = (decimal)results.Last().LowerBand + Calc.AmountOfPercent(this.Indent, (decimal)results.Last().LowerBand);

				Logs.Logger.ToFile($" /{this.ID}/{this.Mode}/ ?: {currentPrice} < {currentPoint}");
				if (currentPrice < currentPoint)
				{
					this.Result = true;
					this.AllowedTime = Time.CurrentSeconds() + this.Duration;
				}
				else
					this.Result = false;
			}
		}
	}
}
