using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Exchanges;
using System;
using System.Collections.Generic;
using System.Linq;

namespace stratumbot.Models.Filters.DataProvider
{
    /// <summary>
    /// 
    /// Data provider for filters.
    /// 
    /// How to add a new data type:
    /// 1) Add a new field in the List of data types variables
    /// 2) Init the field in constructor if it need
    /// 3) Add a new method to get data of the new type
    /// 4) Add this method to GetData() method
    /// 
    /// </summary>
    public class DataProvider
    {
        #region List of data types variables

        /// <summary>
        /// A list of quotes
        /// Options: Cur1, Cur2, TimeFrame, Period
        /// </summary>
        public IEnumerable<Quote> Quotes;

        /// <summary>
        /// The best bid (buy) price in the orderbook
        /// </summary>
        public decimal CurrentBuyPrice;

        /// <summary>
        /// The best bid (buy) price in the orderbook
        /// </summary>
        public decimal CurrentSellPrice;

        /// <summary>
        /// 24h trading volume by base currency
        /// </summary>
        public decimal CurrentVolume;

        /// <summary>
        /// 24h price change percent
        /// </summary>
        public decimal PriceChangePercent;

        /// <summary>
        /// Orderbook
        /// </summary>
        public Dictionary<string, List<Depth>> DOM; // TODO Something wrong with this data structure

        #endregion

        #region Infrastructure fields

        /// <summary>
        /// Exchange client object from CryptoExchange.Net library. Used for getting the required data.
        /// </summary>
        private IExchange _exchange;

        /// <summary>
        /// List of required data for computing the filter of this DataProvider instance
        /// </summary>
        private List<DataType> _requiredDataTypes;

        /// <summary>
        /// List of  RequiredData objects, that containt Options for specific DataType
        /// </summary>
        private List<RequiredData> _requiredDataOptions;

        #endregion

        /// <summary>
        /// Constructor of DataProvider to varaibles init
        /// </summary>
        public DataProvider()
        {
            this._requiredDataTypes = new List<DataType>();
            this._requiredDataOptions = new List<RequiredData>();
        }

        #region Infrastructure methods

        /// <summary>
        /// Set exchange client for DataProvider
        /// </summary>
        /// <param name="exchange">Enum of Exchange</param>
        public void SetExchangeClient(IExchange exchange) {
            this._exchange = exchange;
            /*switch (exchange)
            {
                
                case Exchange.Binance : { this._exchange = new BinanceSpot(); ((BinanceSpot)_exchange).ClientInitTest(Core.TID.CurrentID); break; }

                default: throw new Exception($"SetExchangeClient(): exchange not found");
            }*/
        }

        /// <summary>
        /// Get exchange client of this DataProvider. Need for testing.
        /// </summary>
        /// <returns>IExchange object of exchange client</returns>
        public IExchange GetExchangeClient() { return _exchange; }

        /// <summary>
        /// Add data type that will need to compute the filter
        /// </summary>
        /// <param name="dataType">DataType enum of data type</param>
        public void AddRequiredDataType(DataType dataType)
        {
            this._requiredDataTypes.Add(dataType);
        }

        /// <summary>
        /// Get required data types of this DataProvider instance.
        /// </summary>
        /// <returns>List<DataType> list of required data types</returns>
        public List<DataType> GetRequiredDataTypes()
        {
            return this._requiredDataTypes;
        }

        /// <summary>
        /// Set options for specific data type
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="options"></param>
        public void SetRequiredDataOptions(DataType dataType, DataOptions options)
        {
            this._requiredDataOptions.Add(new RequiredData { DataType = dataType, Options = options });
        }

        /// <summary>
        /// Get options of specific data type
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public DataOptions GetRequiredDataOptions(DataType dataType)
        {
            return this._requiredDataOptions.Where(x => x.DataType == dataType).First().Options;
        }

        #endregion

        /// <summary>
        /// Get all required data from the market
        /// </summary>
        public void GetData() {

            if (this._requiredDataTypes.Contains(DataType.Quotes))
                this.Quotes = this.GetQuotes();
            if (this._requiredDataTypes.Contains(DataType.CurrentVolume))
                this.CurrentVolume = this.GetCurrentVolume();
            if (this._requiredDataTypes.Contains(DataType.PriceChangePercent))
                this.PriceChangePercent = this.GetPriceChangePercent();
            if (this._requiredDataTypes.Contains(DataType.DOM))
                this.DOM = this.GetDOM();
            if (this._requiredDataTypes.Contains(DataType.CurrentPrice))
            {
                this.CurrentBuyPrice = this.GetCurrentBuyPrice();
                this.CurrentSellPrice = this.GetCurrentSellPrice();
            }
        }

        #region Data getting methods for specific data types

        /// <summary>
        /// Get quotes
        /// </summary>
        /// <returns>IEnumerable<Quote> list of quotes</returns>
        private IEnumerable<Quote> GetQuotes() {

            DataOptions options = this.GetRequiredDataOptions(DataType.Quotes);

            return _exchange.GetQuotes
                (
                    options.Cur1, 
                    options.Cur2, 
                    options.TimeFrame, 
                    options.Period // The period must be specific for different filter (it's not just a indicator's period)
                ).Result;
        }

        /// <summary>
        /// Get the best bid (buy) price
        /// </summary>
        /// <returns>(decimal) best buy price</returns>
        private decimal GetCurrentBuyPrice() {
            DataOptions options = this.GetRequiredDataOptions(DataType.CurrentPrice);

            var orderBook = _exchange.GetDOM
                (
                    options.Cur1,
                    options.Cur2,
                    5
                );

            return orderBook["bids"][0].Price;
        }

        /// <summary>
        /// Get the best ask (sell) price
        /// </summary>
        /// <returns>(decimal) best ask price</returns>
        private decimal GetCurrentSellPrice() {

            DataOptions options = this.GetRequiredDataOptions(DataType.CurrentPrice);

            var orderBook = _exchange.GetDOM
                (
                    options.Cur1,
                    options.Cur2,
                    5
                );
            
            return orderBook["asks"][0].Price;
        }

        /// <summary>
        /// Get 24h volume of pair
        /// </summary>
        /// <returns>(decimal) 24h base volume</returns>
        private decimal GetCurrentVolume() {

            DataOptions options = this.GetRequiredDataOptions(DataType.CurrentVolume);

            return _exchange.Volume24h
                (
                    options.Cur1,
                    options.Cur2
                );
        }

        /// <summary>
        /// Get 24h price change percent
        /// </summary>
        /// <returns>(decimal) percent</returns>
        private decimal GetPriceChangePercent() {
            DataOptions options = this.GetRequiredDataOptions(DataType.PriceChangePercent);

            return _exchange.PriceChange
                (
                    options.Cur1,
                    options.Cur2
                );
        }

        /// <summary>
        /// Get orderbook
        /// </summary>
        /// <returns>(Dictionary<string, List<Depth>>) orderbook</returns>
        private Dictionary<string, List<Depth>> GetDOM() {

            DataOptions options = this.GetRequiredDataOptions(DataType.DOM);

            var orderBook = _exchange.GetDOM
                (
                    options.Cur1,
                    options.Cur2,
                    options.Period
                );

            return orderBook;
        }

        public Times GetTimes()
		{
            return TThreadInfo.Times[Core.TID.CurrentID];
        }

        #endregion
    }
}
