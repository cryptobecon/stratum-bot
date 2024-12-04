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
	internal class MAEnvelopes : IFilter
	{
		/// <summary>
		/// ID
		/// </summary>
		public string ID { get; set; } = "27";

		/// <summary>
		/// Name of the filter
		/// </summary>
		public string Name { get; set; } = "MA Envelopes";

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
		/// Percentage Offset
		/// </summary>
		public decimal Rate = 2;

		/// <summary>
		/// Period
		/// </summary>
		public int Period { get; set; } = 0;

		/// <summary>
		/// MA type
		/// </summary>
		public int Source { get; set; } = 0;

		/// <summary>
		/// TimeFrame
		/// </summary>
		public string TimeFrame { get; set; } = "5m";

		#endregion

		#region General filter's configuration

		/// <summary>
		/// My name of the filter that can be changed
		/// </summary>
		public string MyName { get; set; } = "MA Envelopes";

		/// <summary>
		/// Indent bellow or above
		/// </summary>
		//public decimal Indent { get; set; } = 0;

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
			[JsonProperty("Source")]
			public int Source { get; set; }
			[JsonProperty("Rate")]
			public decimal Rate { get; set; }
			[JsonProperty("TimeFrame")]
			public string TimeFrame { get; set; }
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
					Source = this.Source,
					Rate = this.Rate,
					TimeFrame = this.TimeFrame,
					Duration = this.Duration
				};

				string json = JsonConvert.SerializeObject(array);

				return json;
			}
		}

		#endregion

		int SourceMaType = 0;

		// Конструктор
		public MAEnvelopes(int moreOrLess, int period, int source, decimal rate, string timeFrame, int duration)
		{
			this.RequiredDataInit();

			this.Mode = moreOrLess;
			this.Period = period;
			this.Source = source;
			this.Rate = rate;
			this.TimeFrame = timeFrame;
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
				int N = this.Period;

				if (this.Source == 0) // ALMA
					N = this.Period;
				if (this.Source == 1) // DEMA
					N = Math.Max(this.Period * 3, (2 * this.Period) + 100);
				if (this.Source == 2) // EPMA
					N = this.Period;
				if (this.Source == 3) // EMA
					N = Math.Max(this.Period * 2, this.Period + 100);
				if (this.Source == 4) // HMA
					N = this.Period + ((int)Math.Sqrt(this.Period)) - 1;
				if (this.Source == 5) // SMA
					N = this.Period;
				if (this.Source == 6) // SMMA
					N = Math.Max(this.Period * 2, this.Period + 100);
				if (this.Source == 7) // TEMA
					N = Math.Max(this.Period * 4, (this.Period * 3) + 100);
				if (this.Source == 8) // WMA
					N = this.Period;

				return new DataOptions
				{
					Cur1 = cur1,
					Cur2 = cur2,
					TimeFrame = this.TimeFrame,
					Period = N
				};
			}

			throw new Exception("GetOptions() DataType doesn't sent");
		}

		public void Compute()
		{
			if (this.Source == 0) // ALMA
				this.SourceMaType = (int)MaType.ALMA;
			if (this.Source == 1) // DEMA
				this.SourceMaType = (int)MaType.DEMA;
			if (this.Source == 2) // EPMA
				this.SourceMaType = (int)MaType.EPMA;
			if (this.Source == 3) // EMA
				this.SourceMaType = (int)MaType.EMA;
			if (this.Source == 4) // HMA
				this.SourceMaType = (int)MaType.HMA;
			if (this.Source == 5) // SMA
				this.SourceMaType = (int)MaType.SMA;
			if (this.Source == 6) // SMMA
				this.SourceMaType = (int)MaType.SMMA;
			if (this.Source == 7) // TEMA
				this.SourceMaType = (int)MaType.TEMA;
			if (this.Source == 8) // WMA
				this.SourceMaType = (int)MaType.WMA;

			// Duration
			if (this.Duration != 0 && Time.CurrentSeconds() < this.AllowedTime)
			{
				this.Result = true;
				return;
			}

			// Filter Side
			decimal currentPrice = (this.DepthSide == "Bid") ? this.DataProvider.CurrentBuyPrice : this.DataProvider.CurrentSellPrice;

			IEnumerable<MaEnvelopeResult> results =
				this.DataProvider.Quotes.GetMaEnvelopes(this.Period, (double)this.Rate, (MaType)this.SourceMaType);

			// Current point of the filter
			decimal currentPoint = 0;

			if (this.Mode == 0) // price > upper
			{
				currentPoint = (decimal)results.Last().UpperEnvelope;

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
				currentPoint = (decimal)results.Last().LowerEnvelope;

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
				currentPoint = (decimal)results.Last().UpperEnvelope;

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
				currentPoint = (decimal)results.Last().LowerEnvelope;

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
