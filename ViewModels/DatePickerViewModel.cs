using MyGarden2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.ViewModels
{
    public class DatePickerViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime? _dateSowing;

        public DateTime? DateSowing
        {
            get => _dateSowing;
            set
            {
                if (_dateSowing != value)
                {
                    _dateSowing = value;
                    OnPropertyChanged();
                }
            }
        }

        public DatePickerViewModel(DateTime? dateSowing)
        {
            DateSowing = dateSowing;
        }
    }
}
