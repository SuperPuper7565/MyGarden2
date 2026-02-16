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
    /// Логика взаимодействия для DatePickerPopup.xaml
    /// </summary>
    public partial class DatePickerPopup : UserControl
    {
        public event EventHandler<DateTime?> DateConfirmed;
        public event EventHandler Canceled;

        private DatePickerViewModel viewModel;

        public DateTime? SelectedDate => viewModel.DateSowing;

        public DatePickerPopup(DateTime? initialDate = null)
        {
            viewModel = new DatePickerViewModel(initialDate ?? DateTime.Today);
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DateConfirmed?.Invoke(this, SelectedDate);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }
    }
}
