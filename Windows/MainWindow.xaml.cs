using MyGarden2.Windows;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyGarden2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void createNewAccountButton_Click(object sender, RoutedEventArgs e)
    {
        RegisterWindow registerWindow = new RegisterWindow();
        registerWindow.Show();
        this.Close();
    }

    private void adminButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void loginButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            GardenWindow gardenWindow = new GardenWindow(login.Text, password.Password);
            gardenWindow.Show();
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}