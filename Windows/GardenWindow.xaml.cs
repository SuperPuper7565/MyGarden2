using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyGarden2.Models;
using MyGarden2.ViewModels;
using MySql.Data.MySqlClient;

namespace MyGarden2.Windows
{
    /// <summary>
    /// Логика взаимодействия для GardenWindow.xaml
    /// </summary>
    public partial class GardenWindow : Window
    {
        private GardenViewModel viewModel;
        private User user;

        public GardenWindow()
        {
            InitializeComponent();
            viewModel = new GardenViewModel();
            DataContext = viewModel;
        }

        public GardenWindow(string login, string password)
        {
            viewModel = new GardenViewModel();
            try
            {
                viewModel.LoginUser(login, password);
                InitializeComponent();
                DataContext = viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public GardenWindow(string name, string login, string password)
        {
            viewModel = new GardenViewModel();
            try
            {
                viewModel.RegisterUser(name, login, password);
                InitializeComponent();
                DataContext = viewModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void ShowCollection(object sender, MouseButtonEventArgs e) => viewModel.ShowCollection();
        private void ShowGardenBeds(object sender, MouseButtonEventArgs e) => viewModel.ShowGardenBeds();
        private void ShowPlants(object sender, MouseButtonEventArgs e) => viewModel.ShowPlants();

        private void deleteAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить аккаунт? Все данные о вашей коллекции также будут удалены.", "Удаление аккаунта", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.DeleteAccount();
                    MessageBox.Show("Аккаунт удалён", "Удалён", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void changeAccount_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите сменить аккаунт?", "Смена аккаунта", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }
    }
}
