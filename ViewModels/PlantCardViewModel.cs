using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using MyGarden2.Models;
using GalaSoft.MvvmLight.Command;

namespace MyGarden2.ViewModels
{
    public class PlantCardViewModel: INotifyPropertyChanged
    {
        Plant plant;
        public PlantCardViewModel(Plant selectedPlant)
        {
            if (selectedPlant == null)
            {
                plant = new Plant();
            }
            else
            {
                plant = selectedPlant;
            }
            SelectImageCommand = new RelayCommand(SelectImage);
        }
        // Свойства с поддержкой обновления
        public string Name
        {
            get => plant.Name;
            set
            {
                if (plant.Name != value)
                {
                    plant.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ImagePath
        {
            get => plant.ImagePath;
            set
            {
                plant.ImagePath = value;
                OnPropertyChanged();
            }
        }
        public DateTime? DateSowingRecBegin
        {
            get => plant.DateSowingRecBegin;
            set
            {
                if (plant.DateSowingRecBegin != value)
                {
                    plant.DateSowingRecBegin = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? DateSowingRecEnd
        {
            get => plant.DateSowingRecEnd;
            set
            {
                if (plant.DateSowingRecEnd != value)
                {
                    plant.DateSowingRecEnd = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? DateHarvestRecBegin
        {
            get => plant.DateHarvestRecBegin;
            set
            {
                if (plant.DateHarvestRecBegin != value)
                {
                    plant.DateHarvestRecBegin = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? DateHarvestRecEnd
        {
            get => plant.DateHarvestRecEnd;
            set
            {
                if (plant.DateHarvestRecEnd != value)
                {
                    plant.DateHarvestRecEnd = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsGreenHouseRec
        {
            get => plant.IsGreenHouseRec;
            set
            {
                if (plant.IsGreenHouseRec != value)
                {
                    plant.IsGreenHouseRec = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => plant.Description;
            set
            {
                if (plant.Description != value)
                {
                    plant.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WateringConditions
        {
            get => plant.WateringConditions;
            set
            {
                if (plant.WateringConditions != value)
                {
                    plant.WateringConditions = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Length
        {
            get => plant.Length;
            set
            {
                if (value >= 0 && plant.Length != value)
                {
                    plant.Length = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Width
        {
            get => plant.Width;
            set
            {
                if (value >= 0 && plant.Width != value)
                {
                    plant.Width = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IsGreenHouseRecText
        {
            get
            {
                if (!IsGreenHouseRec)
                    return "Рекомендуется сажать на грядку";
                return "Рекомендуется сажать в теплицу";
            }
        }

        public ICommand SelectImageCommand { get; }


        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Выберите изображение растения"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
