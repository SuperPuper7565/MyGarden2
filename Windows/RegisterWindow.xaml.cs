
using MyGarden2.Models;
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
using System.Windows.Shapes;

namespace MyGarden2.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        void CheckPassword(string password)
        {

            if (password.Length < 8)
            {
                throw new Exception("Пароль слишком короткий! Длина пароля должна быть не менее 8 символов");
            }
            if (!(Regex.IsMatch(password, @"\d")))
            {
                throw new Exception("Пароль должен содержать хотя бы одну цифру");
            }
            if (!(Regex.IsMatch(password, "[a-z]")))
            {
                throw new Exception("Пароль должен содержать хотя бы одну букву нижнего регистра");
            }
            if (!(Regex.IsMatch(password, "[A-Z]")))
            {
                throw new Exception("Пароль должен содержать хотя бы одну букву заглавного регистра");
            }
            return;
        }
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void createAccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (password1.Password != password2.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                CheckPassword(password1.Password);
                GardenWindow gardenWindow = new GardenWindow(name.Text, login.Text, password1.Password);
                gardenWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void adminButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
