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
    /// Логика взаимодействия для PlantsTreeView.xaml
    /// </summary>
    public partial class PlantsTreeView : UserControl
    {
        private PlantsTreeViewModel viewModel;
        private readonly PlantsViewModel _plantsViewModel;
        public PlantsTreeView(PlantsViewModel plantsViewModel)
        {
            _plantsViewModel = plantsViewModel;
            viewModel = new PlantsTreeViewModel();
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Получаем элемент, который находится под курсором
            var treeViewItem = FindParent<TreeViewItem>((DependencyObject)e.OriginalSource);

            if (treeViewItem != null)
            {
                // Выбираем этот элемент
                treeViewItem.IsSelected = true;
                treeViewItem.Focus();
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T parentItem)
                    return parentItem;

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        private void plantsTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var treeViewItem = FindParent<TreeViewItem>((DependencyObject)e.OriginalSource);

            if (treeViewItem != null)
            {
                var data = treeViewItem.DataContext;
                bool b0 = data is PlantingMaterial pm && pm.IsAddButton;
                bool b1 = data is PlantingCatalog pc && pc.IsAddButton;
                bool b2 = data is PlantType pt && pt.IsAddButton;
                if (b0 || b1 || b2)
                {
                    e.Handled = true;
                }
            }
        }

        private void plantsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedItem = plantsTreeView.SelectedItem;
            if (selectedItem is PlantingMaterial pm)
            {
                if (pm.Name == "+ новый материал")
                {
                    _plantsViewModel.ShowAddOrEditItem();
                    var addUserControl = _plantsViewModel.PlantCurrentView as AddOrEditItem;
                    addUserControl.HeaderText.Text = "Введите название";
                    addUserControl.OKButton.Content = "Сохранить";
                    addUserControl.Confirm += (s, e) =>
                    {
                        viewModel.AddItem(addUserControl.ChangedName);
                        plantsTreeView.Items.Refresh();
                    };
                }
            }
            else if (selectedItem is PlantingCatalog pc)
            {
                if (pc.Name == "+ новый каталог")
                {
                    _plantsViewModel.ShowAddOrEditItem();
                    var addUserControl = _plantsViewModel.PlantCurrentView as AddOrEditItem;
                    addUserControl.HeaderText.Text = "Введите название";
                    addUserControl.OKButton.Content = "Сохранить";
                    addUserControl.Confirm += (s, e) =>
                    {
                        viewModel.AddItem(addUserControl.ChangedName, pc);
                        plantsTreeView.Items.Refresh();
                    };
                }
            }
            else if (selectedItem is PlantType pt)
            {
                if (pt.Name == "+ новый тип")
                {
                    _plantsViewModel.ShowAddOrEditItem();
                    var addUserControl = _plantsViewModel.PlantCurrentView as AddOrEditItem;
                    addUserControl.HeaderText.Text = "Введите название";
                    addUserControl.OKButton.Content = "Сохранить";
                    addUserControl.Confirm += (s, e) =>
                    {
                        viewModel.AddItem(addUserControl.ChangedName, pt);
                        plantsTreeView.Items.Refresh();
                    };
                }
            }
            else if (selectedItem is Plant p)
            {
                if (p.Name == "+ новое растение")
                {
                    _plantsViewModel.ShowAddOrEditPlant();
                    var uc = _plantsViewModel.PlantCurrentView as AddOrEditPlant;
                    uc.Confirm += (s, e) =>
                    {
                        viewModel.AddItem(uc.NewPlant, p);
                        plantsTreeView.Items.Refresh();
                    };
                }
                else
                {
                    _plantsViewModel.ShowPlantCard(p);
                }
            }

        }

        private void ChangeName_Click(object sender, RoutedEventArgs e)
        {
            if (plantsTreeView.SelectedItem is Plant plant)
            {
                _plantsViewModel.ShowAddOrEditPlant(plant);
                var editWindow = _plantsViewModel.PlantCurrentView as AddOrEditPlant;
                editWindow.Confirm += (s, e) =>
                {
                    _plantsViewModel.UpdateItem(plant, editWindow.NewPlant);
                    plantsTreeView.Items.Refresh();
                };
            }
            else
            {
                _plantsViewModel.ShowAddOrEditItem();
                var editWindow = _plantsViewModel.PlantCurrentView as AddOrEditItem;
                editWindow.HeaderText.Text = "Измените название";
                editWindow.OKButton.Content = "Применить изменения";
                if (plantsTreeView.SelectedItem is PlantType plantType)
                {
                    editWindow.NameTextBox.Text = plantType.Name;
                    editWindow.Confirm += (s, e) =>
                    {
                        _plantsViewModel.UpdateItem(plantType, editWindow.ChangedName);
                        plantsTreeView.Items.Refresh();
                    };
                }
                else if (plantsTreeView.SelectedItem is PlantingCatalog catalog)
                {
                    editWindow.NameTextBox.Text = catalog.Name;
                    editWindow.Confirm += (s, e) =>
                    {
                        _plantsViewModel.UpdateItem(catalog, editWindow.ChangedName);
                        plantsTreeView.Items.Refresh();
                    };
                }
                else if (plantsTreeView.SelectedItem is PlantingMaterial material)
                {
                    editWindow.NameTextBox.Text = material.Name;
                    editWindow.Confirm += (s, e) =>
                    {
                        _plantsViewModel.UpdateItem(material, editWindow.ChangedName);
                        plantsTreeView.Items.Refresh();
                    };
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (plantsTreeView.SelectedItem is null)
            {
                MessageBox.Show("Выберите элемент для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этот элемент? Все вложенные элементы также будут удалены.", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                viewModel.DeleteItem(plantsTreeView.SelectedItem);
                plantsTreeView.Items.Refresh();
            }
        }

        public void RefreshData()
        {
            plantsTreeView.Items.Refresh();
        }

        public void DeleteItem(object selectedItem) 
        {
            viewModel.DeleteItem(selectedItem);
        }
    }
}