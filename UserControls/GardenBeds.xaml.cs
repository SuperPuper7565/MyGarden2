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
using MyGarden2.Models;
using MyGarden2.ViewModels;
using MySql.Data.MySqlClient;

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для GardenBeds.xaml
    /// </summary>
    public partial class GardenBeds : UserControl
    {
        GardenBedsViewModel viewModel;
        public GardenBeds(User user)
        {
            InitializeComponent();
            viewModel = new GardenBedsViewModel(GardenBedsListView, user);
            DataContext = viewModel;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddGardenBed();
        }
    }
}
