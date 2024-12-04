using stratumbot.Core;
using stratumbot.Interfaces;
using stratumbot.Models.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    static class AutoFit
    {
        public static IConfig GetConfig(Strategy _id, Exchange _exchange, string _cur1, string _cur2, string _budget)
        {
            if(_id == Strategy.Scalping)
            {

                ScalpingConfigText config = new ScalpingConfigText();

                var exchange = AvailableExchanges.CreateExchangeById(_exchange);
                exchange.ClientInit(0);

                config.Cur1 = _cur1;
                config.Cur2 = _cur2;
                config.Budget = _budget;

                if (Settings.ParamInPercentScalpingAutofit)
                {
                    config.MinSpread = Settings.MinSpreadScalpingAutofit + "%";
                    config.OptSpread = Settings.OptSpreadScalpingAutofit + "%";
                    config.MinMarkup = Settings.MinMarkupScalpingAutofit + "%";
                    config.OptMarkup = Settings.OptMarkupScalpingAutofit + "%";
                    config.ZeroSell = Settings.ZeroSellScalpingAutofit + "%";
                } else
                {
                    var doms = exchange.GetDOM(_cur1, _cur2);
                    decimal lastPrice = doms["bids"][0].Price;

                    config.MinSpread = Conv.s8(Calc.AmountOfPercent(Conv.dec(Settings.MinSpreadScalpingAutofit), lastPrice));
                    config.OptSpread = Conv.s8(Calc.AmountOfPercent(Conv.dec(Settings.OptSpreadScalpingAutofit), lastPrice));
                    config.MinMarkup = Conv.s8(Calc.AmountOfPercent(Conv.dec(Settings.MinMarkupScalpingAutofit), lastPrice));
                    config.OptMarkup = Conv.s8(Calc.AmountOfPercent(Conv.dec(Settings.OptMarkupScalpingAutofit), lastPrice));
                    config.ZeroSell = Conv.s8(Calc.AmountOfPercent(Conv.dec(Settings.ZeroSellScalpingAutofit), lastPrice));
                }

                config.InTimeout = Settings.InTimeoutScalpingAutofit;
                config.StopLoss = "0";

                config.IsDCA = Settings.IsDCAAutofit;
                config.DCAProfitPercent = Settings.DCAProfitPercentAutofit;
                config.DCAStepCount = Settings.DCAStepCountAutofit;
                config.DCASteps = new System.Collections.ObjectModel.ObservableCollection<string[]>(Settings.DCAStepsAutofit);

                return new ScalpingConfig(config); ;
            }

            throw new Exception("AutoFit error"); // Просто чтоб не ругался компилятор
        }
    }
}
