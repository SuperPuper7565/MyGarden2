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
using static MaterialDesignThemes.Wpf.Theme.ComboBox;

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PlantInfoPopup.xaml
    /// </summary>
    public partial class PlantInfoPopup : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                "ViewModel",
                typeof(PlantInfoPopupViewModel),
                typeof(PlantInfoPopup),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnViewModelChanged)));

        public PlantInfoPopupViewModel ViewModel
        {
            get => (PlantInfoPopupViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public PlantInfoPopup()
        {
            InitializeComponent();
            Loaded += OnLoaded; // Подписываемся на событие загрузки
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем DataContext при загрузке контрола
            DataContext = ViewModel;
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlantInfoPopup popup && popup.IsLoaded)
            {
                // Обновляем DataContext при изменении ViewModel
                popup.DataContext = e.NewValue;
            }
        }

        private void DeletePlantButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RequestRemove();
        }

        private void ReplacePlantButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RequestTransplant();
        }
    }
}
