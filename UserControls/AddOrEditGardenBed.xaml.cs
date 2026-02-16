using MyGarden2.Models;
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
    /// Логика взаимодействия для AddOrEditGardenBed.xaml
    /// </summary>
    public partial class AddOrEditGardenBed : UserControl
    {
        private const int MaxRows = 50;     // Максимум рядов
        private const int MaxColumns = 50;  // Максимум колонок

        private int _rows = 3;    // Начальное количество рядов
        private int _columns = 5; // Начальное количество колонок
        private bool _isGreenHouse = false; // По умолчанию не теплица
        private int _cellSize = 10; // Размер сетки по умолчанию

        public GardenBed GardenBed { get; set; }
        public event EventHandler Confirm;

        public AddOrEditGardenBed()
        {
            InitializeComponent();
            GreenhouseCheckBox.IsChecked = _isGreenHouse;
            GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
        }
        public AddOrEditGardenBed(int rows, int columns, bool isGreenHouse, int cellSize)
        {
            InitializeComponent();
            _rows = rows;
            _columns = columns;
            _isGreenHouse = isGreenHouse;
            GreenhouseCheckBox.IsChecked = _isGreenHouse;
            _cellSize = cellSize;
            GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
        }
        private void GenerateGrid(int rows, int columns, bool isGreenHouse, int cellSize)
        {
            GardenGrid.Children.Clear();
            GardenGrid.RowDefinitions.Clear();
            GardenGrid.ColumnDefinitions.Clear();
            if (isGreenHouse)
                GardenGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
            else
                GardenGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D2B48C"));
            GardenGrid.Width = columns * cellSize;
            GardenGrid.Height = rows * cellSize;

            // Создаем строки
            for (int i = 0; i < rows; i++)
            {
                GardenGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });
            }

            // Создаем столбцы
            for (int j = 0; j < columns; j++)
            {
                GardenGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
            }

            SolidColorBrush borderBrush;
            if (isGreenHouse)
                borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#607D8B"));
            else
                borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B4513"));
                // Заполняем ячейки
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        Border cell = new Border
                        {
                            BorderBrush = borderBrush,
                            BorderThickness = new Thickness(1),
                            Width = cellSize,
                            Height = cellSize
                        };
                        Grid.SetRow(cell, row);
                        Grid.SetColumn(cell, col);
                        GardenGrid.Children.Add(cell);
                    }
                }
        }

        // Добавить ряд
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            if (_rows < MaxRows)
            {
                _rows++;
                GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
            }
        }

        // Удалить ряд
        private void RemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if (_rows > 1)
            {
                _rows--;
                GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
            }
        }

        // Добавить столбец
        private void AddColumn_Click(object sender, RoutedEventArgs e)
        {
            if (_columns < MaxColumns)
            {
                _columns++;
                GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
            }
        }

        // Удалить столбец
        private void RemoveColumn_Click(object sender, RoutedEventArgs e)
        {
            if (_columns > 1)
            {
                _columns--;
                GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            GardenBed = new GardenBed { LengthCells = _rows, WidthCells = _columns, IsGreenHouse = _isGreenHouse, CellSize = _cellSize };
            this.Visibility = Visibility.Collapsed;
            Confirm?.Invoke(this, EventArgs.Empty);
        }

        private void GreenhouseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _isGreenHouse = true;
            GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
        }

        private void GreenhouseCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _isGreenHouse = false;
            GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void ConfirmCellCize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cellSize = int.Parse(CellSizeTextBox.Text);
                if (_cellSize < 10)
                {
                    MessageBox.Show("Не устанавливайте слишком маленький размер.\nРазмер клетки должен быть не менее 10 см.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (_cellSize > 100)
                {
                    MessageBox.Show("Не устанавливайте слишком большой размер.\nРазмер клетки должен быть не более 100 см.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                GenerateGrid(_rows, _columns, _isGreenHouse, _cellSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Размер сетки должен быть целым числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
