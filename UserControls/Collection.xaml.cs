using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MyGarden2.Models;
using MyGarden2.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для Collection.xaml
    /// </summary>
    public partial class Collection : UserControl
    {
        private CollectionViewModel viewModel;
        private UsedPlant draggedPlant;
        private Popup datePickerPopup;
        public Collection(User user)
        {
            viewModel = new CollectionViewModel(user);
            InitializeComponent();
            DataContext = viewModel;
        }

        private void DeletePlant_Click(object sender, RoutedEventArgs e)
        {
            if (plantsListView.SelectedItem is UsedPlant plant)
            {
                var result = MessageBox.Show($"Удалить растение {plant.Name} из коллекции? Все растения данного типа будут удалены с грядок.", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.DeletePlant(plant);
                    plantsListView.Items.Refresh();
                }
            }
        }

        private void DeleteGardenBed_Click(object sender, RoutedEventArgs e)
        {
            if (gardenBedsListView.SelectedIndex != -1)
            {
                int index = gardenBedsListView.SelectedIndex;
                var result = MessageBox.Show($"Удалить грядку из коллекции? Учтите, что все посаженные растения также будут удалены.", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.DeleteGardenBed(viewModel.UsedGardenBeds[index]);
                    gardenBedsListView.Items.Refresh();
                    plantsListView.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Ничего не выбрано!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlantListViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var listViewItem = sender as ListViewItem;
                if (listViewItem != null && listViewItem.Content is UsedPlant plant)
                {
                    draggedPlant = plant;
                    DragDrop.DoDragDrop(listViewItem, plant, DragDropEffects.Copy);
                }
            }
        }

        private void GardenBedListViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private async void GardenBedListViewItem_Drop(object sender, DragEventArgs e)
        {
            if (draggedPlant == null) return;

            var listViewItem = sender as ListViewItem;
            if (listViewItem?.Content is UsedGardenBed gardenBed)
            {
                var gardenBedControl = FindVisualChild<ListItemGardenBed>(listViewItem);
                if (gardenBedControl == null) return;

                var position = e.GetPosition(gardenBedControl);
                int cellSizePx = gardenBedControl.CellSize;

                int xCell = (int)(position.X / cellSizePx);
                int yCell = (int)(position.Y / cellSizePx);

                int plantWidthCells = (int)Math.Ceiling((double)draggedPlant.Width / gardenBed.CellSize);
                int plantHeightCells = (int)Math.Ceiling((double)draggedPlant.Length / gardenBed.CellSize);

                if (xCell < 0 || yCell < 0 ||
                    xCell + plantWidthCells > gardenBed.WidthCells ||
                    yCell + plantHeightCells > gardenBed.LengthCells)
                {
                    MessageBox.Show("Растение не помещается на грядку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (AppConfig.IsSpaceOccupied(gardenBed, xCell, yCell, plantWidthCells, plantHeightCells))
                {
                    MessageBox.Show("Место занято другим растением", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var (isValid, selectedDate) = await IsPlant(gardenBed, draggedPlant);
                if (isValid && selectedDate.HasValue)
                {
                    viewModel.AddToDatabase(gardenBed.UsedGBId, draggedPlant.UsedId, xCell, yCell, selectedDate.Value);
                    gardenBedControl.GenerateCanvas(gardenBed.LengthCells, gardenBed.WidthCells,
                                                  gardenBed.IsGreenHouse, cellSizePx);
                }
            }
            draggedPlant = null;
        }

        async Task<(bool IsValid, DateTime? SelectedDate)> IsPlant(UsedGardenBed gardenBed, UsedPlant plant)
        {
            // Проверяем рекомендации по типу грядки
            if (gardenBed.IsGreenHouse != plant.IsGreenHouseRec)
            {
                var message = gardenBed.IsGreenHouse
                    ? $"{plant.Name} рекомендуется садить на грядку. Вы уверены, что хотите посадить растение в теплицу?"
                    : $"{plant.Name} рекомендуется садить в теплицу. Вы уверены, что хотите посадить растение на грядку?";

                var result = MessageBox.Show(
                    message,
                    "Подтверждение посадки",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return (false, null);
            }

            // Проверяем дату посадки
            var (isValid, selectedDate) = await IsDateRight(plant);
            if (!isValid)
                return (false, null);

            // Все проверки пройдены
            return (true, selectedDate);
        }

        private async Task<(bool isValid, DateTime? date)> IsDateRight(UsedPlant plant)
        {
            while (true) // Бесконечный цикл, пока не получим валидную дату или отмену
            {
                var selectedDate = await SelectDateSowing();

                // Если пользователь закрыл окно выбора даты
                if (!selectedDate.HasValue)
                    return (false, null);

                // Если дата в рекомендуемом диапазоне
                if (selectedDate >= plant.DateSowingRecBegin &&
                    selectedDate <= plant.DateSowingRecEnd)
                    return (true, selectedDate);

                // Если дата вне диапазона - запрашиваем подтверждение
                var confirm = MessageBox.Show(
                    $"Дата {selectedDate.Value.ToShortDateString()} вне рекомендуемого периода ({plant.DateSowingRecBegin.Value.ToShortDateString()} - {plant.DateSowingRecEnd.Value.ToShortDateString()}).\n\nПродолжить с выбранной датой?",
                    "Подтверждение даты посадки",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                switch (confirm)
                {
                    case MessageBoxResult.Yes:
                        return (true, selectedDate);
                    case MessageBoxResult.No:
                        continue; // Возвращаемся к выбору даты
                    case MessageBoxResult.Cancel:
                    default:
                        return (false, null); // Полная отмена операции
                }
            }
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                viewModel.SearchPlants(searchBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка поиска", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаем диалог сохранения файла
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    Title = "Сохранить отчёт",
                    FileName = $"Отчёт_о_растениях_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Создаем новую книгу Excel
                    using (var workbook = new XLWorkbook())
                    {
                        // Добавляем лист с растениями
                        var plantsWorksheet = workbook.Worksheets.Add("Растения");
                        GeneratePlantsWorksheet(plantsWorksheet);

                        // Добавляем лист с грядками
                        var gardenBedsWorksheet = workbook.Worksheets.Add("Грядки");
                        GenerateGardenBedsWorksheet(gardenBedsWorksheet);

                        var collectionWorksheet = workbook.Worksheets.Add("Коллекция");
                        GenerateCollectionWorksheet(collectionWorksheet);

                        // Сохраняем файл
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("Отчёт успешно сохранён!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateCollectionWorksheet(IXLWorksheet worksheet)
        {
            // Заголовки столбцов
            worksheet.Cell(1, 1).Value = "Растение";
            worksheet.Cell(1, 2).Value = "Длина растения (см)";
            worksheet.Cell(1, 3).Value = "Ширина растения (см)";
            worksheet.Cell(1, 4).Value = "Дата посадки";
            worksheet.Cell(1, 5).Value = "Сбор урожая";
            worksheet.Cell(1, 6).Value = "Код грядки";
            worksheet.Cell(1, 7).Value = "Тип грядки";
            worksheet.Cell(1, 8).Value = "Координата Х на грядке";
            worksheet.Cell(1, 9).Value = "Координата Y на грядке";

            // Форматирование заголовков
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

            // Заполняем данными
            int row = 2;
            foreach (var gardenBed in viewModel.UsedGardenBeds)
            {
                var plants = AppConfig.LoadPlantsFromDatabase(gardenBed.UsedGBId);
                foreach (var plant in plants)
                {
                    worksheet.Cell(row, 1).Value = plant.Name;
                    worksheet.Cell(row, 2).Value = plant.Length;
                    worksheet.Cell(row, 3).Value = plant.Width;
                    worksheet.Cell(row, 4).Value = plant.DateSowing;
                    worksheet.Cell(row, 5).Value = AppConfig.CalculateDaysToHarvest(plant);
                    worksheet.Cell(row, 6).Value = gardenBed.UsedGBId;
                    worksheet.Cell(row, 7).Value = gardenBed.IsGreenHouse ? "Теплица" : "Грядка";
                    worksheet.Cell(row, 8).Value = plant.XCoordinate;
                    worksheet.Cell(row, 9).Value = plant.YCoordinate;
                    row++;
                }
            }

            // Автонастройка ширины столбцов
            worksheet.Columns().AdjustToContents();
        }

        private void GeneratePlantsWorksheet(IXLWorksheet worksheet)
        {
            // Заголовки столбцов
            worksheet.Cell(1, 1).Value = "Название";
            worksheet.Cell(1, 2).Value = "Длина (см)";
            worksheet.Cell(1, 3).Value = "Ширина (см)";
            worksheet.Cell(1, 4).Value = "Рекомендуется посадить";


            // Форматирование заголовков
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;

            // Заполняем данными
            int row = 2;
            foreach (var plant in viewModel.ChosenPlants)
            {
                worksheet.Cell(row, 1).Value = plant.Name;
                worksheet.Cell(row, 2).Value = plant.Length;
                worksheet.Cell(row, 3).Value = plant.Width;
                worksheet.Cell(row, 4).Value = plant.IsGreenHouseRec ? "Теплица" : "Грядка";

                row++;
            }

            // Автонастройка ширины столбцов
            worksheet.Columns().AdjustToContents();
        }

        private void GenerateGardenBedsWorksheet(IXLWorksheet worksheet)
        {
            // Заголовки столбцов
            worksheet.Cell(1, 1).Value = "Код";
            worksheet.Cell(1, 2).Value = "Тип";
            worksheet.Cell(1, 3).Value = "Длина (ячеек)";
            worksheet.Cell(1, 4).Value = "Ширина (ячеек)";
            worksheet.Cell(1, 5).Value = "Размер ячейки (см)";
            worksheet.Cell(1, 6).Value = "Количество растений";

            // Форматирование заголовков
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Заполняем данными
            int row = 2;
            foreach (var gardenBed in viewModel.UsedGardenBeds)
            {
                worksheet.Cell(row, 1).Value = gardenBed.UsedGBId;
                worksheet.Cell(row, 2).Value = gardenBed.IsGreenHouse ? "Теплица" : "Грядка";
                worksheet.Cell(row, 3).Value = gardenBed.LengthCells;
                worksheet.Cell(row, 4).Value = gardenBed.WidthCells;
                worksheet.Cell(row, 5).Value = gardenBed.CellSize;

                // Получаем количество растений на грядке
                var plantsCount = viewModel.GetPlantsCountOnGardenBed(gardenBed.UsedGBId);
                worksheet.Cell(row, 6).Value = plantsCount;

                row++;
            }

            // Автонастройка ширины столбцов
            worksheet.Columns().AdjustToContents();
        }

        private async Task<DateTime?> SelectDateSowing()
        {
            var tcs = new TaskCompletionSource<DateTime?>();

            var popupContent = new DatePickerPopup();
            var popup = new Popup
            {
                Child = popupContent,
                Placement = PlacementMode.MousePoint,
                HorizontalOffset = -20,
                VerticalOffset = -20,
                StaysOpen = false,
                IsOpen = true
            };

            popupContent.DateConfirmed += (s, date) =>
            {
                popup.IsOpen = false;
                tcs.SetResult(date);
            };

            popupContent.Canceled += (s, e) =>
            {
                popup.IsOpen = false;
                tcs.SetResult(null);
            };

            popup.Closed += (s, e) =>
            {
                if (!tcs.Task.IsCompleted)
                    tcs.SetResult(null);
            };

            return await tcs.Task;
        }
    }
}
