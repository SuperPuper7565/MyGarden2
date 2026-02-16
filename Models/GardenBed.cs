using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class GardenBed
    {
        public int Id { get; set; }
        public bool IsGreenHouse { get; set; }
        public int LengthCells { get; set; } // Количество клеток в длину
        public int WidthCells { get; set; } // Количество клеток в ширину
        public int CellSize { get; set; }
        public int Length => LengthCells * CellSize; // Длина в см
        public int Width => WidthCells * CellSize;   // Ширина в см
        public string Text
        {
            get
            {
                if (IsGreenHouse)
                    return $"Теплица: {Length}x{Width} (см)\nСетка: {CellSize} см";
                return $"Грядка: {Length}x{Width} (см)\nСетка: {CellSize} см";
            }
        }

        public GardenBed() { }

        public GardenBed(int id, bool isGreenHouse, int length, int width, int cellSize)
        {
            Id = id;
            IsGreenHouse = isGreenHouse;
            LengthCells = length;
            WidthCells = width;
            CellSize = cellSize;
        }
    }
}
