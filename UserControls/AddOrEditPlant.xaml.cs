using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyGarden2.Models;
using MyGarden2.ViewModels;

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AddOrEditPlant.xaml
    /// </summary>
    public partial class AddOrEditPlant : UserControl
    {
        public Plant NewPlant;
        public event EventHandler Confirm;
        private PlantCardViewModel viewModel;
        public AddOrEditPlant()
        {
            InitializeComponent();
            viewModel = new PlantCardViewModel(NewPlant);
            DataContext = viewModel;
        }
        public AddOrEditPlant(Plant p)
        {
            NewPlant = p;
            InitializeComponent();
            viewModel = new PlantCardViewModel(NewPlant);
            PlantCheckBox.IsChecked = viewModel.IsGreenHouseRec;
            DataContext = viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.Name == null || viewModel.Name == "" || viewModel.Name == "+ новое растение" ||
                viewModel.ImagePath == null || viewModel.DateSowingRecBegin == null || viewModel.DateSowingRecEnd == null ||
                viewModel.DateHarvestRecBegin == null || viewModel.DateHarvestRecEnd == null ||
                viewModel.Description == null || viewModel.Description == "" ||
                viewModel.WateringConditions == null || viewModel.WateringConditions == "")
            {
                MessageBox.Show("Заполните все данные!", "Сохранение невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Regex regex = new Regex("[А-Яа-я]");
            if (!regex.IsMatch(viewModel.Name) || !regex.IsMatch(viewModel.Description) || !regex.IsMatch(viewModel.WateringConditions))
            {
                MessageBox.Show("Введите текст на русском!", "Сохранение невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (viewModel.DateSowingRecBegin > viewModel.DateSowingRecEnd)
            {
                MessageBox.Show("Даты посева заполнены некорректно!", "Сохранение невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (viewModel.DateHarvestRecBegin > viewModel.DateHarvestRecEnd)
            {
                MessageBox.Show("Даты сбора урожая заполнены некорректно!", "Сохранение невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (viewModel.DateHarvestRecBegin < viewModel.DateSowingRecBegin)
            {
                MessageBox.Show("Даты заполнены некорректно! Растение не может дать урожай раньше, чем вы его посадили.", "Сохранение невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            NewPlant = new Plant
            {
                Name = viewModel.Name,
                DateSowingRecBegin = viewModel.DateSowingRecBegin,
                DateSowingRecEnd = viewModel.DateSowingRecEnd,
                DateHarvestRecBegin = viewModel.DateHarvestRecBegin,
                DateHarvestRecEnd = viewModel.DateHarvestRecEnd,
                IsGreenHouseRec = viewModel.IsGreenHouseRec,
                WateringConditions = viewModel.WateringConditions,
                Length = viewModel.Length,
                Width = viewModel.Width,
                Description = viewModel.Description,
                ImagePath = viewModel.ImagePath
            };
            this.Visibility = Visibility.Collapsed;
            Confirm?.Invoke(this, EventArgs.Empty);
        }

        private void PlantCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.IsGreenHouseRec = true;
        }

        private void PlantCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            viewModel.IsGreenHouseRec = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
