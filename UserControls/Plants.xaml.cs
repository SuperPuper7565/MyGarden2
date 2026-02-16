using MyGarden2.ViewModels;
using MyGarden2.Models;
using MySql.Data.MySqlClient;
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
using MaterialDesignThemes.Wpf;
using System.Windows.Media.Media3D;

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Plants.xaml
    /// </summary>

    public partial class Plants : UserControl
    {
        private PlantsViewModel plantsViewModel;
        public Plants(User user)
        {
            plantsViewModel = new PlantsViewModel(user);
            InitializeComponent();
            DataContext = plantsViewModel;
            plantsViewModel.ShowPlantsTreeView();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchControl = new PlantsSearch();
            searchControl.PlantSelected += SearchControl_PlantSelected;
            plantsViewModel.ListCurrentView = searchControl;
        }

        private void SearchControl_PlantSelected(Plant selectedPlant)
        {
            plantsViewModel.ShowPlantCard(selectedPlant);
        }

        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            plantsViewModel.ShowPlantsTreeView();
        }
    }
}
