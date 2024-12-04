using stratumbot.Core;
using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace stratumbot.ViewModels
{
    class StratumBoxVM : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        public delegate void StratumBoxDelegate();
        public static event StratumBoxDelegate StratumBoxEvent;

        // Мои ордера
        private bool isMyOrderBox;
        public bool IsMyOrderBox
        {
            get { return isMyOrderBox; }
            set
            {
                isMyOrderBox = value;
                Settings.MyOrdersBox = value;
                StratumBoxEvent();
                OnPropertyChanged();
            }
        }

        public StratumBoxVM()
        {
            this.IsMyOrderBox = Settings.MyOrdersBox;
        }

        // Кнопка Сохранить
        public ICommand SaveClick
        {
            get
            {
                return new Command((obj) =>
                {
                    Settings.Save();
                });
            }
        }
    }
}
