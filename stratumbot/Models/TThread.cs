using Newtonsoft.Json;
using stratumbot.Core;
using stratumbot.DTO;
using stratumbot.Interfaces;
using stratumbot.Models.Logs;
using stratumbot.Models.Strategies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows;

namespace stratumbot.Models
{
    /// <summary>
    /// Объект потока
    /// </summary>
    public class TThread : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        // Количество добавленных потоков (для TID)
        private static int counter = 0;

        //private bool isTrading = false;
        private CancellationTokenSource ts;
        private CancellationToken cancellationToken;

        // Задача выполняющая работу потока (торговлю)
        Task trading;

        // Работа стратегии (одна сделка)
        public IStrategy deal { get; set; }

        // Настройки для данного потокоа (Нужны чтоб получать их из VM)
        public IConfig config { get; set; }

        public bool Recovering { get; set; } // Для восстановления потока. True если сейчас идёт восстановление. False обычный запуск потока.

        // ТУТ МОЖНО ПОЛЯ КОТОРЫЕ *НЕ БУДУТ* МЕНЯТЬСЯ ВНУТРИ СТРАТЕГИИ

        string sid; // Session ID BtnPlus
        public string Sid
        {
            get { return sid; }
            set
            {
                sid = value;
                OnPropertyChanged();
            }
        }

        string tid;
        public string TID
        {
            get { return tid; }
            set
            {
                tid = value;
                OnPropertyChanged();
            }
        }

        string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        // Кол-во итерайций по потоку
        int iteration;
        public int Iteration
        {
            get { return iteration; }
            set
            {
                iteration = value;
                OnPropertyChanged();
            }
        }

        // Кол-во StopLoss по потоку
        int stopLossCount;
        public int StopLossCount
        {
            get { return stopLossCount; }
            set
            {
                stopLossCount = value;
                OnPropertyChanged();
            }
        }

        public delegate void IterationCountDelegate(decimal profitUSD, decimal profitPercent);
        public event IterationCountDelegate NewIterationEvent; // Событие "Новая итерация"

        // Время добавления потока
        string time;
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged();
            }
        }

        // Профит по потоку
        decimal profitPercent;
        public decimal ProfitPercent
        {
            get { return profitPercent; }
            set
            {
                profitPercent = value;
                OnPropertyChanged();
            }
        }

        decimal profit;
        public decimal Profit
        {
            get { return profit; }
            set
            {
                profit = value;
                OnPropertyChanged();
            }
        }

        decimal profitUSD;
        public decimal ProfitUSD
        {
            get { return profitUSD; }
            set
            {
                profitUSD = value;
                OnPropertyChanged();
            }
        }


        string pair;
        public string Pair
        {
            get { return pair; }
            set
            {
                pair = value;
                OnPropertyChanged();
            }
        }

        string budget;
        public string Budget
        {
            get { return budget; }
            set
            {
                budget = value;
                OnPropertyChanged();
            }
        }

        int dcaStep;
        public int DCAStep
        {
            get { return dcaStep; }
            set
            {
                dcaStep = value;
                OnPropertyChanged();
            }
        }

        // Длительность работы потока
        decimal threadStartTimestamp;
        public decimal ThreadStartTimestamp
        {
            get { return threadStartTimestamp; }
            set
            {
                threadStartTimestamp = value;
                OnPropertyChanged();
            }
        }

        // Длительность итерации
        decimal iterationStartTimestamp;
        public decimal IterationStartTimestamp
        {
            get { return iterationStartTimestamp; }
            set
            {
                iterationStartTimestamp = value;
                OnPropertyChanged();
            }
        }

        // Остановка после продажи
        bool isStopAfterSell;
        public bool IsStopAfterSell
        {
            get { return isStopAfterSell; }
            set
            {
                isStopAfterSell = value;
                OnPropertyChanged();
            }
        }

        // Конструктор потока
        public TThread(IConfig _config, Exchange exchange)
        {
            //this.cancellationToken = ts.Token;

            this.config = _config;
            counter++; // Увеличиванием кол-во созданных потоков (для TID)
            this.TID = counter.ToString();
            this.Status = "draft";
            this.Time = DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy");
            this.Iteration = 0;
            this.ProfitPercent = (decimal)0.00;
            this.Profit = (decimal)0.00000000;
            this.Pair = this.config.Cur1 + this.config.Cur2;
            this.Budget = this.config.Budget.ToString();
            this.DCAStep = 0;
            this.deal = AvailableStrategies.CreateStrategyByConfig(_config, exchange);
            
        }

        // Конструктор потока - используется при изменении потока и его сохранении
        public void TThreadAgain(IConfig _config, Exchange exchange)
        {
            this.cancellationToken = ts.Token;

            this.config = _config;
            /*this.Iteration = 0;
            this.ProfitPercent = (decimal)0.00;
            this.Profit = (decimal)0.00000000;*/
            this.Pair = this.config.Cur1 + this.config.Cur2;
            this.Budget = this.config.Budget.ToString();
            this.DCAStep = 0;
            this.deal = AvailableStrategies.CreateStrategyByConfig(_config, exchange);
        }

        /// <summary>
        /// Метод для создания объекта и настройка бекапа потока
        /// </summary>
        /*private void ThreadBackupInit() // Чтото тут все бсивается при восстановлении ж. от статуса до флага восстановления
        {
            this.deal.Backup = new ThreadBackup(); // Инициализируем объект бекапа (внутри объекта стратегии, чтобы могли работать и тут и там)

            this.deal.Recovering = false;
            this.deal.Backup.Config = this.config; 

            this.deal.Backup.Action = "CHECK";
            this.deal.Backup.Time = this.Time;
            this.deal.Backup.Iteration = this.Iteration;
            this.deal.Backup.ProfitPercent = this.ProfitPercent;
            this.deal.Backup.Profit = this.Profit;
            this.deal.Backup.ProfitUSD = this.ProfitUSD;
            this.deal.Backup.Pair = this.Pair;
            this.deal.Backup.Budget = this.Budget;
            this.deal.Backup.DCAStep = this.DCAStep;
            this.deal.Backup.Order = null;

            //this.deal.Backup.Amount = 0;
            this.deal.Backup.Session = this.Sid;
            this.deal.Backup.BuyCounter = 0; 
            this.deal.Backup.SellCounter = 0;

            // TODO тут наверно проиниц 3 новых поля бэкапа currentStep trigeer ect
            /*this.deal.Backup.IsTriggered = false;
            this.deal.Backup.CurrentStep = 0;
            this.deal.Backup.Step = null;*/ // TODO TEST
            /*
            ThreadBackup.Save(this.deal.Backup, int.Parse(this.TID));
        }*/

        /// <summary>
        /// Конструктор объекта потока при восстановлении
        /// </summary>
        /*public TThread(ThreadBackup _backup)
        {
            // Восстановление в отдельный метод
            if (_backup != null)
            {

                this.config = _backup.Config;
                counter++; // Увеличиванием кол-во созданных потоков (для TID)
                this.TID = counter.ToString();
                this.Status = "work";
                this.Time = _backup.Time;
                this.Iteration = _backup.Iteration;
                this.ProfitPercent = Calc.RoundUp(_backup.ProfitPercent, (decimal)0.001);
                this.Profit = Calc.RoundUp(_backup.Profit, (decimal)0.00000001);
                this.Pair = _backup.Pair;
                this.Budget = _backup.Budget;
                this.DCAStep = _backup.DCAStep;

                this.Recovering = true;

                this.deal = AvailableStrategies.CreateStrategyByConfig(_backup.Config);
                this.deal.Recovering = true;

                this.deal.Backup = new ThreadBackup(); // Инициализируем объект бекапа (внутри объекта стратегии, чтобы могли работать и тут и там)
                this.deal.Backup.Config = _backup.Config;
                this.deal.Backup.Action = _backup.Action;
                this.deal.Backup.Time = _backup.Time;
                this.deal.Backup.Iteration = _backup.Iteration;
                this.deal.Backup.ProfitPercent = _backup.ProfitPercent;
                this.deal.Backup.Profit = _backup.Profit;
                this.deal.Backup.ProfitUSD = _backup.ProfitUSD;
                this.deal.Backup.Pair = _backup.Pair;
                this.deal.Backup.Budget = _backup.Budget;
                this.deal.Backup.DCAStep = _backup.DCAStep;
                this.deal.Backup.Order = _backup.Order;
                //this.deal.Backup.Amount = _backup.Amount;
                this.deal.Backup.Session = _backup.Session;
                this.deal.Backup.BuyCounter = _backup.BuyCounter; 
                this.deal.Backup.SellCounter = _backup.SellCounter;
                this.deal.Backup.OrderPool = _backup.OrderPool;

                /*this.deal.Backup.IsTriggered = _backup.IsTriggered;
                this.deal.Backup.CurrentStep = _backup.CurrentStep;
                this.deal.Backup.Step = _backup.Step;*/

                /*ThreadBackup.Save(this.deal.Backup, int.Parse(this.TID) );

                return;
            }

        }*/

        // Торговля - суть задачи
        public void Trading()
        {
            //isTrading = true;
            //this.deal.Exchange.IsTrading = true;

            Core.TID.CurrentID = int.Parse(this.TID); // TODO REFACTORING а зачем this.TID в стринг формате вообще?
            // Бронируем свободные API токены по бирже для потока
            //Tokens tokens = TResource.GetReservedAPI(this.config.Exchange, Core.TID.CurrentID);
            Tokens tokens = TResource.GetReservedAPI(this.deal.Exchange.Id, Core.TID.CurrentID);
            if (tokens == null)
            {
                Logger.Error(_.Log4); // Нет свободных API-ключей
                this.Stop();
                MessageBox.Show(_.Msg18); // Нужно добавить API-ключи
                this.Status = "stop";
                return;
            }
            // Указываем стратегию и биржу по потоку (для Моих ордеров пока только)
            TResource.Exchange[Core.TID.CurrentID] = this.deal.Exchange.Id;
            TResource.Strategy[Core.TID.CurrentID] = this.config.Strategy;

            TThreadInfo.Times[Core.TID.CurrentID] = new Times();

            // Указываем токен в экземпляре IExchange данного потока
            this.deal.Exchange.Tokens = tokens; // TODO REFACTORING вообще как-то не круто отсюда лезть в биржу. Но пришлось бы делать TResource.GetAPI(); каждый запрос чтоб получить. Сделать это как то по уму из самой бирже и один раз.
            this.deal.Exchange.ClientInit(Core.TID.CurrentID);

            // Получении session_id
            this.Sid = BtnPlus.GetSessionID(this.config);

            /*if(this.deal is Scalping)
            {
                if (!this.Recovering)
                {
                    ThreadBackupInit(); // Создание бекапа потока
                }
            }*/

            // Подготавливаем минималку
            try
            {
                Logger.Debug(_.Log58);
                deal.Exchange.GetMinimals(this.config.Cur1, this.config.Cur2);
                Logger.Debug(_.Log59);
            } catch (ManuallyStopException ex)
            {
                TResource.UnreservedAPI(int.Parse(this.TID));
                Logger.ToFile(ex.Message, "[exception:ManuallyStopException]"); // code 9
                this.Status = "stop";
                /*if (this.deal is Scalping)
                    (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // TODO FUTURE Пока только скальпинг
                if (this.deal is IDCAble)
                    (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                Logger.Info(new string('-', 20));
                return;
            }

            try
            {
                Logger.ToFile($"[config] {JsonConvert.SerializeObject(this.config)}");
            } catch
            {
                Logger.ToFile($"[exception] TThread.Trading - can't save config");
            }
            

            while (true) // Продолжаем совершать итерации
            {
                IterationStartTimestamp = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; // Время запуска итерации в Unix Timestamp
               
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        TResource.UnreservedAPI(int.Parse(this.TID));
                        this.Status = "stop";
                        /*if (this.deal is Scalping)
                            (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
                        if (this.deal is IDCAble)
                            (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                        break;
                    }

                    this.deal.Trade();
                    // Конец итерации: 
                    this.Iteration++;
                    this.Profit += Calc.RoundUp(this.deal.Profit.profitBase, (decimal)0.00000001); // Профит в базовой валюте по итерации
                    this.ProfitPercent += Calc.RoundUp(this.deal.Profit.profitPercent, (decimal)0.001); // Профит в процентах по итерации
                    this.ProfitUSD += Calc.RoundUp(this.deal.Profit.profitUSD, (decimal)0.00000001); // Профит в долларах по итерации

                    // Количество выставленных BUY ордеров за итерацию
                    // Количество выставленных SELL ордеров за итерацию
                    /*if (this.deal is Scalping)
                    {
                        this.deal.Backup.BuyCounter = this.deal.BuyCounter;
                        this.deal.Backup.SellCounter = this.deal.SellCounter;
                        this.deal.Backup.Profit = this.Profit;
                        this.deal.Backup.ProfitUSD = this.ProfitUSD;
                        this.deal.Backup.ProfitPercent = this.ProfitPercent;
                        this.deal.Backup.Iteration = this.Iteration;
                        ThreadBackup.Save(this.deal.Backup);
                    }*/

                    // Длительность итерации TODO Отрефакторить в одну
                    decimal iterationDurationSec = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - IterationStartTimestamp;
                    TimeSpan duration = TimeSpan.FromSeconds((double)iterationDurationSec);
                    Logger.ToFile($"Iteration duration: {duration.ToString(@"dd\.hh\:mm\:ss")}");

                    // Регистрируем новую итерацию
                    BtnPlus.NewIteration(this.deal, iterationDurationSec, this.Sid);

                    NewIterationEvent(
                        Calc.RoundUp(this.deal.Profit.profitUSD, (decimal)0.00000001),
                        Calc.RoundUp(this.deal.Profit.profitPercent, (decimal)0.001)
                    );

                    Ring.BabloVoice();
                    
                    // Отсановка после продажи
                    if(IsStopAfterSell)
                    {
                        //ThreadBackup.Delete(int.Parse(this.TID));
                        TResource.UnreservedAPI(int.Parse(this.TID));
                        Logger.Info(_.Log5); // Остановка после продажи..
                        this.Stop();
                        this.Status = "stop";
                        /*if (this.deal is Scalping)
                            (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
                        if (this.deal is IDCAble)
                            (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                        break;
                    }
                    // Остановка после StopLoss 
                    if (this.deal is ClassicLong)
                    {
                        if ((this.deal as ClassicLong).IsStopLossTriggered)
                        {
                            StopLossCount++;
                            if (int.Parse(Settings.StopAfterXStopLoss) != 0) // Есть ли запланированные остановки после Х стоплоссов
                            {
                                if (StopLossCount >= int.Parse(Settings.StopAfterXStopLoss)) // Текущее количество >= запланированных на остановку
                                {
                                    Logger.Info($"Остановка после {Settings.StopAfterXStopLoss} стоплосс"); // TODO TEXT
                                    this.Stop();
                                    this.Status = "stop";
                                    break;
                                }
                            }
                            
                        }
                    }

                    if (this.deal is ClassicShort)
                    {
                        if ((this.deal as ClassicShort).IsStopLossTriggered)
                        {
                            StopLossCount++;
                            if (int.Parse(Settings.StopAfterXStopLoss) != 0) // Есть ли запланированные остановки после Х стоплоссов
                            {
                                if (StopLossCount >= int.Parse(Settings.StopAfterXStopLoss)) // Текущее количество >= запланированных на остановку
                                {
                                    Logger.Info($"Остановка после {Settings.StopAfterXStopLoss} стоплосс"); // TODO TEXT
                                    this.Stop();
                                    this.Status = "stop";
                                    break;
                                }
                            }

                        }
                    }

                    if (this.deal is Scalping)
                    {
                        if ((this.deal as Scalping).IsStopLossTriggered) // TODO move to stoploss obj ?
                        {
                            StopLossCount++;
                            if (int.Parse(Settings.StopAfterXStopLoss) != 0) // Есть ли запланированные остановки после Х стоплоссов
                            {
                                if (StopLossCount >= int.Parse(Settings.StopAfterXStopLoss)) // Текущее количество >= запланированных на остановку
                                {
                                    Logger.Info($"Остановка после {Settings.StopAfterXStopLoss} стоплосс"); // TODO TEXT
                                    this.Stop();
                                    break;
                                }
                            }

                        }
                    }
                }
                catch (InvalidParamException ex)
                {
                    //ThreadBackup.Delete(int.Parse(this.TID));
                    TResource.UnreservedAPI(int.Parse(this.TID));
                    Logger.ToFile($"[exception:InvalidParamException] {ex.ToString()}"); // поток был остановлен 
                    Logger.Error(ex.Message);
                    this.Status = "stop";
                    /*if (this.deal is Scalping)
                        (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
                    if (this.deal is IDCAble)
                        (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                    break;
                }
                catch (ManuallyStopException ex)
                {
                    //ThreadBackup.Delete(int.Parse(this.TID));
                    TResource.UnreservedAPI(int.Parse(this.TID));
                    Logger.ToFile($"[exception:ManuallyStopException] {ex.Message}"); // поток был остановлен 
                    Logger.Info(_.Log44);
                    this.Status = "stop";
                    /*if (this.deal is Scalping)
                        (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
                    if (this.deal is IDCAble)
                        (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                    break;
                }
				catch (AutoStopException ex)
				{
					//ThreadBackup.Delete(int.Parse(this.TID));
					TResource.UnreservedAPI(int.Parse(this.TID));
					Logger.ToFile($"[exception:AutoStopException] {ex.Message}"); // поток был остановлен 
					Logger.Info(_.Log44);
					this.Status = "stop";
					/*if (this.deal is Scalping)
						(this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
                    if (this.deal is IDCAble)
                        (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                    break;
				}
				catch (TinyOrderCanceledException ex)
				{
					Logger.Debug(ex.Message);
				}
				catch (OrderCanceledException ex)
                {
                    //ThreadBackup.Delete(int.Parse(this.TID));
                    //TResource.UnreservedAPI(int.Parse(this.TID));
                    // EX3
                    //Logger.Info(ex.Message); // ордер был отменен вручную на бирже
                    //this.Status = "stop";
                    //if (this.deal is Scalping)
                    //  (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler; // TODO FUTURE Пока только скальпинг
                    //break;
                    Logger.Debug($"OrderCanceledException, но продолжаем, потом решим");
                }
                catch (Exception ex)
                {
                    TResource.UnreservedAPI(int.Parse(this.TID));
                    // TODO FUTURE резервное восстановление можно прям тут попробовать активировать не перезапуская бота)
                    // EX1, EX2, EX4, EX5, EX6, EX7
                    Logger.ToFile($"[exception] TThread.Trading {ex.ToString()}");
                    this.Status = "stop";
                    /*if (this.deal is Scalping)
                        (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
                    if (this.deal is IDCAble)
                        (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
                    break;
                }
                finally
                {
                    // nullexception if there wa no iteration
                    decimal iterationProfitBase = 0;// Calc.RoundUp(this.deal.Profit.profitBase, (decimal)0.00000001);
                    decimal iterationProfitPercent = 0;// Calc.RoundUp(this.deal.Profit.profitPercent, (decimal)0.001);
                    decimal iterationProfitUSD = 0;// Calc.RoundUp(this.deal.Profit.profitUSD, (decimal)0.00000001);
                    // Продолжительность работы потока TODO Отрефакторить в одну
                    decimal threadDurationSec = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds - ThreadStartTimestamp;
                    TimeSpan duration = TimeSpan.FromSeconds((double)threadDurationSec);
                    Logger.ToFile($"#{this.Iteration} : {this.Profit} + {iterationProfitBase} / {this.ProfitPercent}% + {iterationProfitPercent}  / {this.ProfitUSD}$ + {iterationProfitUSD} / Thread duration: {duration.ToString(@"dd\.hh\:mm\:ss")}");

                    Logger.Info(new string('-', 20));
                }
            }
        }

        // Start < Начать торговлю
        public void Start()
        {
            ThreadStartTimestamp = (decimal)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; // Время запуска потока в Unix Timestamp
            /*if(this.Recovering)
            {   // Если сейчас идёт восстановление, то указываем это в самой стратегии.
                this.deal.Recovering = true;
            }*/

            //if(this.deal is Scalping)
            //    (this.deal as Scalping).DCAStepChangedEvent += DCAStepChangedHandler; // T-ODO FUTURE Пока только скальпинг

            if (this.deal is IDCAble)
                (this.deal as IDCAble).DCAStepChangedEvent += DCAStepChangedHandler;

            this.ts = new CancellationTokenSource();
            this.cancellationToken = ts.Token; // TODO del, we can use ts.Token always

            this.deal.ts = this.ts;
            this.deal.cancellationToken = this.cancellationToken;

            this.deal.Exchange.ts = this.ts;
            this.deal.Exchange.cancellationToken = this.cancellationToken;

            trading = new Task(Trading, this.cancellationToken);
            trading.Start();
        }

        // Stop < Безопасная остановка
        public void Stop()
        {
            //isTrading = false;
            if (this.ts != null)
                this.ts.Cancel();
            //this.Status = "stop";
            deal.StopTrade();
            TResource.UnreservedAPI(int.Parse(this.TID));
            /*if (this.deal is Scalping)
                (this.deal as Scalping).DCAStepChangedEvent -= DCAStepChangedHandler;*/ // T-ODO FUTURE Пока только скальпинг
            if (this.deal is IDCAble)
                (this.deal as IDCAble).DCAStepChangedEvent -= DCAStepChangedHandler;
            Logger.ToFile("! Stop() #2"); // TODO del/ тут удалить поток из id? СТОП
        }

        // Обработчик события DCAStepChangedEvent — "DCA step изменился"
        public void DCAStepChangedHandler(int _stepNum)
        {
            this.DCAStep = _stepNum;
        }
    }
}
