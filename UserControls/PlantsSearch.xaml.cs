using MyGarden2.Models;
using MyGarden2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlantsSearch.xaml
    /// </summary>
    public partial class PlantsSearch : UserControl
    {
        PlantsSearchViewModel viewModel;
        public event Action<Plant> PlantSelected;
        public PlantsSearch()
        {
            viewModel = new PlantsSearchViewModel();
            InitializeComponent();
            DataContext = viewModel;
            plantsListView.SelectionChanged += PlantsListView_SelectionChanged;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                viewModel.SearchPlants(searchBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка поиска", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlantsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (plantsListView.SelectedItem is Plant selectedPlant)
            {
                PlantSelected?.Invoke(selectedPlant);
            }
        }

        internal void RefreshData()
        {
            viewModel.SearchPlants(searchBox.Text);
        }
    }
}
