using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateSowingRecBegin { get; set; }
        public DateTime? DateSowingRecEnd { get; set; }
        public DateTime? DateHarvestRecBegin { get; set; }
        public DateTime? DateHarvestRecEnd { get; set; }
        public bool IsGreenHouseRec { get; set; }
        public string Description { get; set; }
        public string WateringConditions { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public string ImagePath { get; set; }
        public bool IsAddButton { get; set; } = false;

        public Plant()
        {
            Name = "+ новое растение";
            IsAddButton = true;
        }
        public Plant(int id, string name, DateTime? dateSowingRecBegin, DateTime? dateSowingRecEnd, DateTime? dateHarvestRecBegin, DateTime? dateHarvestRecEnd, bool isGreenHouseRec, string description, string wateringConditions, int length, int width, string imagePath, bool isAddButton)
        {
            Id = id;
            Name = name;
            DateSowingRecBegin = dateSowingRecBegin;
            DateSowingRecEnd = dateSowingRecEnd;
            DateHarvestRecBegin = dateHarvestRecBegin;
            DateHarvestRecEnd = dateHarvestRecEnd;
            IsGreenHouseRec = isGreenHouseRec;
            Description = description;
            WateringConditions = wateringConditions;
            Length = length;
            Width = width;
            ImagePath = imagePath;
            IsAddButton = isAddButton;
        }
        public Plant(Plant plant)
        {
            Id = plant.Id;
            Name = plant.Name;
            DateSowingRecBegin = plant.DateSowingRecBegin;
            DateSowingRecEnd = plant.DateSowingRecEnd;
            IsGreenHouseRec = plant.IsGreenHouseRec;
            Description = plant.Description;
            WateringConditions = plant.WateringConditions;
            Length = plant.Length;
            Width = plant.Width;
            ImagePath = plant.ImagePath;
            IsAddButton = plant.IsAddButton;
        }
    }
}
