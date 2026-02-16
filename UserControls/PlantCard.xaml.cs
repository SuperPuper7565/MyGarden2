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
using MyGarden2.ViewModels;
using MyGarden2.Models;

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlantCard.xaml
    /// </summary>
    public partial class PlantCard : UserControl
    {
        private PlantCardViewModel viewModel;
        Plant plant { get; set; }
        public event EventHandler PlantConfirm;
        public event EventHandler ChangeConfirm;
        public event EventHandler DeleteConfirm;
        public event EventHandler CloseRequested;
        public PlantCard(Plant p)
        {
            plant = p;
            InitializeComponent();
            viewModel = new PlantCardViewModel(plant);
            DataContext = viewModel;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            DeleteConfirm?.Invoke(this, EventArgs.Empty);
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            ChangeConfirm?.Invoke(this, EventArgs.Empty);
        }

        private void PlantButton_Click(object sender, RoutedEventArgs e)
        {
            PlantConfirm?.Invoke(this, EventArgs.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
