using stratumbot.Core;
using stratumbot.Models.Filters;
using stratumbot.Models.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace stratumbot.ViewModels
{
    // Делегат для настройки DCA из менеджера
    public delegate void AddDCABox(string stepCount, ObservableCollection<string[]> dcaSteps, Dictionary<int, DCAFilter> dcaFilters);

    public class DCAManagerVM : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        public event AddDCABox AddDCABoxEvent;

        public string FiltersSide = "BUY"; // Режим, который будет установлен на фильтры. Какую цену будет брать ask/bid для long/short/иногда разное над.

        public string dcaProfitPercent; // DCA % профита
        public string DCAProfitPercent
        {
            get { return dcaProfitPercent; }
            set
            {
                dcaProfitPercent = value;
                OnPropertyChanged();
            }
        }

        private int dcaStepCount; // Количество DCA шагов
        public string DCAStepCount
        {
            get { return dcaStepCount.ToString(); }
            set
            {
                dcaStepCount = Conv.cleanInt(value);
                DCAStepsGenerate(); // Генерируем шаги, т.к. их кол-во изменилось
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string[]> DCASteps { get; set; } = new ObservableCollection<string[]>();// Хранит в себе настройки DCA шагов [#1, 1%, 100%]

        // Список списков фильтров по шагам DCA - DCAFilters[номерШага] = список фильтров
        private Dictionary<int, DCAFilter> dcaFilters;
        public Dictionary<int, DCAFilter> DCAFilters // Хранит в себе настройки Фильтров по DCA шагам
        {
            get { return dcaFilters; }
            set
            {
                dcaFilters = value;
                OnPropertyChanged();
            }
        }
        // dcaf

        // Конструктор
        public DCAManagerVM()
        {

        }

        // Метод генерации параметров шагов по умолчанию
        private void DCAStepsGenerate()
        {
            DCASteps.Clear();
            //dcaf
            DCAFilters = new Dictionary<int, DCAFilter>();
            var test = new Dictionary<int, DCAFilter>();
            for (int i = 0; i < this.dcaStepCount; i++)
            {
                int val = i + 1;
                DCASteps.Add(new string[] { $"#{val}", $"{val}%", $"100%" });
                var dcaFilter = new DCAFilter { Filters = new List<JsonFilter>(), FiltersSettings = new List<Interfaces.IFilter>(), CurrentWeigth = 0, TargetPoint = 0 }; //dcaf
                test[val] = dcaFilter;
            }
            DCAFilters = test;

        }

        public ICommand OkClick
        {
            get
            {
                return new Command((obj) =>
                {
                    // Передать DCABox
                    AddDCABoxEvent?.Invoke(this.DCAStepCount, this.DCASteps, this.DCAFilters);

                    // TODO execute evenet

                });
            }
        }

        // Добавление списка индикаторов (ID, без настроек)
        public void AddFilters(List<JsonFilter> filters, int targetPoint, string filtersSide, bool isStopLoss = false, int step = 0)
        {
            if (step == 0)
                return;

            DCAFilters[step].Filters.Clear();

            foreach (var filter in filters)
            {
                DCAFilters[step].Filters.Add(filter);
            }

            this.DCAFilters[step].TargetPoint = targetPoint;

            // dcaf Костыль для того, чтобы DCA фильтры по вью обновились
            this.DCAFilters = new Dictionary<int, DCAFilter>(this.DCAFilters);
        }


        // Кнопка: Фильтры&Индикаторы Окно
        public ICommand FiltersAndIndicatorsStepManagerClick
        {
            get
            {
                return new Command((obj) =>
                {

                    //MessageBox.Show();

                    int step = int.Parse(obj.ToString());

                    List<JsonFilter> Filters = DCAFilters[step].Filters;
                    Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<JsonFilter>(Filters), DCAFilters[step].TargetPoint, this.FiltersSide, false, step);
                    faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                    //faim.VM.AddFiltersSettingsEvent += AddFiltersSettings; // Фильтры с настройками для отображения в окне
                    faim.Show();

                    /*
                    // Фильтры для покупки
                    if (obj.ToString() == "BUY")
                    {
                        Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<Filter>(FiltersBuy), this.TargetPointBuy, "BUY");
                        faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                        //faim.VM.AddFiltersSettingsEvent += AddFiltersSettings; // Фильтры с настройками для отображения в окне
                        faim.Show();
                    }
                    // Фильтры для продажи
                    if (obj.ToString() == "SELL")
                    {
                        Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<Filter>(FiltersSell), this.TargetPointSell, "SELL");
                        faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                        faim.Show();
                    }
                    // Фильтры для StopLoss
                    if (obj.ToString() == "STOPLOSS")
                    {
                        if (SelectedStrategy.Id == Strategy.ClassicLong) // Нужно отдельно по стратегиям т.к. стоплосс для лонга будет по SELL смотреть а для шорта по BUY
                        {
                            Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<Filter>(FiltersStopLoss), this.TargetPointStopLoss, "SELL", true);
                            faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                            faim.Show();
                        }

                        if (SelectedStrategy.Id == Strategy.ClassicShort) // Нужно отдельно по стратегиям т.к. стоплосс для лонга будет по SELL смотреть а для шорта по BUY
                        {
                            Views.FiltersAndIndicatorsManagerWindow faim = new Views.FiltersAndIndicatorsManagerWindow(new List<Filter>(FiltersStopLoss), this.TargetPointStopLoss, "BUY", true);
                            faim.VM.AddFiltersEvent += AddFilters; // Филтры (ID, без настроек) для конфига (+отображения в голубой панели)
                            faim.Show();
                        }
                    }*/
                });
            }
        }
    }
}
