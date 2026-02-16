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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyGarden2.Models;

namespace MyGarden2.UserControls
{
    /// <summary>
    /// Логика взаимодействия для GardenBedCard.xaml
    /// </summary>
    public partial class GardenBedCard : UserControl
    {
        public event EventHandler AddConfirm;
        public event EventHandler ChangeConfirm;
        public event EventHandler DeleteConfirm;

        public GardenBedCard()
        {
            InitializeComponent();
            DataContextChanged += GardenBedCard_DataContextChanged;
        }

        private void GardenBedCard_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is GardenBed gardenBed)
            {
                GenerateGrid(gardenBed.LengthCells, gardenBed.WidthCells, gardenBed.IsGreenHouse, gardenBed.CellSize);
            }
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

            for (int i = 0; i < rows; i++)
            {
                GardenGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });
            }

            for (int j = 0; j < columns; j++)
            {
                GardenGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
            }

            SolidColorBrush borderBrush;
            if (isGreenHouse)
                borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#607D8B"));
            else
                borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B4513"));
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
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddConfirm?.Invoke(this, EventArgs.Empty);
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            ChangeConfirm?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            DeleteConfirm?.Invoke(this, EventArgs.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
