using MyGarden2.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Controls.Primitives;
using MyGarden2.ViewModels;
using System.Collections.ObjectModel;

namespace MyGarden2.UserControls
{
    public partial class ListItemGardenBed : UserControl
    {
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(int), typeof(ListItemGardenBed),
                new PropertyMetadata(0, OnGridPropertyChanged));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(ListItemGardenBed),
                new PropertyMetadata(0, OnGridPropertyChanged));

        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register(nameof(CellSize), typeof(int), typeof(ListItemGardenBed),
                new PropertyMetadata(0, OnGridPropertyChanged));

        public static readonly DependencyProperty IsGreenHouseProperty =
            DependencyProperty.Register(nameof(IsGreenHouse), typeof(bool), typeof(ListItemGardenBed),
                new PropertyMetadata(false, OnGridPropertyChanged));

        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public bool IsGreenHouse
        {
            get => (bool)GetValue(IsGreenHouseProperty);
            set => SetValue(IsGreenHouseProperty, value);
        }

        public int CellSize
        {
            get => (int)GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, value);
        }

        private Popup _plantPopup;
        private ObservableCollection<PlantInGarden> plants;

        public ListItemGardenBed()
        {
            InitializeComponent();
            Loaded += (s, e) => GenerateCanvas(Rows, Columns, IsGreenHouse, CellSize);
        }

        private static void OnGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ListItemGardenBed)d;
            control.GenerateCanvas(control.Rows, control.Columns, control.IsGreenHouse, control.CellSize);
        }

        public void GenerateCanvas(int rows, int columns, bool isGreenHouse, int cellSize)
        {
            GardenCanvas.Children.Clear();

            // Устанавливаем размеры Canvas
            GardenCanvas.Width = columns * cellSize;
            GardenCanvas.Height = rows * cellSize;

            // Устанавливаем размеры всего контрола
            this.Width = columns * cellSize + 10; // +10 для padding
            this.Height = rows * cellSize + 10;

            // Устанавливаем фон для всего контрола
            if (isGreenHouse)
                MainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
            else
                MainBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2B48C"));

            SolidColorBrush borderBrush;
            if (isGreenHouse)
                borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#607D8B"));
            else
                borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B4513"));

            // Рисуем линии сетки
            for (int x = 0; x <= columns; x++)
            {
                var line = new Line
                {
                    X1 = x * cellSize,
                    Y1 = 0,
                    X2 = x * cellSize,
                    Y2 = rows * cellSize,
                    Stroke = borderBrush,
                    StrokeThickness = 1
                };
                GardenCanvas.Children.Add(line);
            }

            for (int y = 0; y <= rows; y++)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 = y * cellSize,
                    X2 = columns * cellSize,
                    Y2 = y * cellSize,
                    Stroke = borderBrush,
                    StrokeThickness = 1
                };
                GardenCanvas.Children.Add(line);
            }

            LoadPlantsOnGardenBed();
        }

        private void LoadPlantsOnGardenBed()
        {
            try
            {
                if (DataContext is UsedGardenBed gardenBed)
                {
                    plants = AppConfig.LoadPlantsFromDatabase(gardenBed.UsedGBId);
                    foreach (var plant in plants)
                    {
                        // Переводим координаты из клеток в пиксели
                        int xPosPx = plant.XCoordinate * CellSize;
                        int yPosPx = plant.YCoordinate * CellSize;

                        // Переводим размеры растения из см в клетки, затем в пиксели
                        int widthCells = (int)Math.Ceiling((double)plant.Width / gardenBed.CellSize);
                        int heightCells = (int)Math.Ceiling((double)plant.Length / gardenBed.CellSize);

                        int widthPx = widthCells * CellSize;
                        int heightPx = heightCells * CellSize;

                        Border plantContainer = new Border
                        {
                            BorderBrush = Brushes.DarkGreen,
                            BorderThickness = new Thickness(1),
                            Background = Brushes.Transparent,
                            CornerRadius = new CornerRadius(4),
                            Cursor = Cursors.Hand,
                            Focusable = true,
                            Width = widthPx,
                            Height = heightPx
                        };

                        plantContainer.MouseDown += PlantContainer_MouseDown;
                        plantContainer.Tag = plant;

                        Image plantImage = new Image
                        {
                            Source = LoadImage(plant.ImagePath),
                            Stretch = Stretch.UniformToFill
                        };

                        plantContainer.Child = plantImage;

                        // Позиционирование
                        Canvas.SetLeft(plantContainer, xPosPx);
                        Canvas.SetTop(plantContainer, yPosPx);

                        GardenCanvas.Children.Add(plantContainer);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки растений: {ex.Message}", "Ошибка",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private BitmapImage LoadImage(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(path);
                    image.EndInit();
                    return image;
                }
                return new BitmapImage(new Uri("pack://application:,,,/Resources/default_plant.png"));
            }
            catch
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/default_plant.png"));
            }
        }

        private void ShowPlantInfoPopup(PlantInGarden plant)
        {
            if (_plantPopup == null)
            {
                _plantPopup = new Popup
                {
                    Placement = PlacementMode.MousePoint,
                    HorizontalOffset = -20,
                    VerticalOffset = -20,
                    StaysOpen = false,
                    AllowsTransparency = true,
                    PopupAnimation = PopupAnimation.Fade
                };
            }
            
            var popupContent = new PlantInfoPopup
            {
                ViewModel = new PlantInfoPopupViewModel
                {
                    PlantName = plant.Name,
                    PlantImage = LoadImage(plant.ImagePath),
                    PlantingDate = $"Дата посадки: {plant.DateSowing.ToShortDateString()}",
                    DaysToHarvest = AppConfig.CalculateDaysToHarvest(plant),
                }
            };
            popupContent.ViewModel.RemoveRequested += (s, e) => RemovePlant(plant);
            popupContent.ViewModel.TransplantRequested += (s, e) => StartTransplanting(plant);

            _plantPopup.Child = popupContent;
            _plantPopup.IsOpen = true;
        }

        private bool _isTransplanting = false;
        private PlantInGarden _plantToTransplant;

        private void StartTransplanting(PlantInGarden plant)
        {
            _isTransplanting = true;
            _plantToTransplant = plant;
            _plantPopup.IsOpen = false;

            // Можно добавить визуальную индикацию режима пересадки
            MainBorder.BorderBrush = Brushes.Red;
            MainBorder.BorderThickness = new Thickness(2);
            this.Cursor = Cursors.Hand;
        }

        // Добавляем обработчик клика по грядке
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (_isTransplanting && DataContext is UsedGardenBed gardenBed)
            {
                var position = e.GetPosition(GardenCanvas);
                int xCell = (int)(position.X / CellSize);
                int yCell = (int)(position.Y / CellSize);

                // Проверка границ и занятости места
                if (CanTransplantHere(gardenBed, xCell, yCell))
                {
                    TransplantPlant(gardenBed, xCell, yCell);
                }
            }

            base.OnMouseDown(e);
        }

        private bool CanTransplantHere(UsedGardenBed gardenBed, int xCell, int yCell)
        {
            // Проверка границ грядки
            int plantWidthCells = (int)Math.Ceiling((double)_plantToTransplant.Width / gardenBed.CellSize);
            int plantHeightCells = (int)Math.Ceiling((double)_plantToTransplant.Length / gardenBed.CellSize);

            // Проверка занятости места
            if (AppConfig.IsSpaceOccupied(gardenBed, xCell, yCell, plantWidthCells, plantHeightCells))
            {
                MessageBox.Show("Место занято другим растением");
                return false;
            }

            return true;
        }

        private void TransplantPlant(UsedGardenBed targetGardenBed, int xCell, int yCell)
        {
            try
            {
                // Удалить из старой грядки
                DeletePlantFromDatabase(_plantToTransplant);

                // Добавить в новую грядку
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO collection 
                        (id_used_garden_bed, id_used_plant, x_coordinate, y_coordinate, date_sowing) 
                        VALUES (@id_garden, @id_plant, @x, @y, @date)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_garden", targetGardenBed.UsedGBId);
                        command.Parameters.AddWithValue("@id_plant", _plantToTransplant.UsedId);
                        command.Parameters.AddWithValue("@x", xCell);
                        command.Parameters.AddWithValue("@y", yCell);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }

                // Обновить UI
                plants.Remove(_plantToTransplant);
                GenerateCanvas(Rows, Columns, IsGreenHouse, CellSize);

                MessageBox.Show("Растение успешно пересажено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при пересадке растения: {ex.Message}");
            }
            finally
            {
                ResetTransplantingState();
            }
        }

        private void ResetTransplantingState()
        {
            _isTransplanting = false;
            _plantToTransplant = null;
            MainBorder.BorderBrush = Brushes.Transparent;
            MainBorder.BorderThickness = new Thickness(0);
            this.Cursor = Cursors.Arrow;
        }

        private void RemovePlant(PlantInGarden plant)
        {
            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите выкопать растение?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Удаление из базы данных
                    DeletePlantFromDatabase(plant);

                    // Удаление из коллекции
                    plants.Remove(plant);

                    // Обновление UI
                    GenerateCanvas(Rows, Columns, IsGreenHouse, CellSize);

                    MessageBox.Show("Растение успешно выкопано");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении растения: {ex.Message}");
            }
        }

        private void DeletePlantFromDatabase(PlantInGarden plant)
        {
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM collection WHERE id_used_garden_bed = @gardenId AND x_coordinate = @x AND y_coordinate = @y";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gardenId", ((UsedGardenBed)DataContext).UsedGBId);
                    command.Parameters.AddWithValue("@x", plant.XCoordinate);
                    command.Parameters.AddWithValue("@y", plant.YCoordinate);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void PlantContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                if (sender is Border plantContainer && plantContainer.Tag is PlantInGarden plant)
                {
                    ShowPlantInfoPopup(plant);
                    e.Handled = true;
                }
            }
        }
    }
}
