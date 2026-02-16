
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MyGarden2.ViewModels
{
    public class PlantInfoPopupViewModel : ObservableObject
    {
        private string _plantName;
        private ImageSource _plantImage;
        private string _plantingDate;

        public string PlantName
        {
            get => _plantName;
            set => Set(ref _plantName, value);
        }

        public ImageSource PlantImage
        {
            get => _plantImage;
            set => Set(ref _plantImage, value);
        }

        public string PlantingDate
        {
            get => _plantingDate;
            set => Set(ref _plantingDate, value);
        }

        private string _daysToHarvest;
        public string DaysToHarvest
        {
            get => _daysToHarvest;
            set => Set(ref _daysToHarvest, value);
        }

        public event EventHandler RemoveRequested;
        public event EventHandler TransplantRequested;

        public void RequestRemove()
        {
            RemoveRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RequestTransplant()
        {
            TransplantRequested?.Invoke(this, EventArgs.Empty);
        }


    }
}
