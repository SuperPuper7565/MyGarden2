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

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для EditItem.xaml
    /// </summary>
    public partial class AddOrEditItem : UserControl
    {
        public string ChangedName;
        public event EventHandler Confirm;
        public AddOrEditItem()
        {
            InitializeComponent();
        }

        private void EditOKButton_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("[А-Яа-я]");
            if (regex.IsMatch(NameTextBox.Text))
            {
                ChangedName = NameTextBox.Text;
                this.Visibility = Visibility.Collapsed;
                Confirm?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show("Введите название на русском!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
