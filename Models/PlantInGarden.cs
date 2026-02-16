using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class PlantInGarden: UsedPlant
    {
        public DateTime DateSowing { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public PlantInGarden(int id, string name, DateTime? dateSowingRecBegin, DateTime? dateSowingRecEnd, DateTime? dateHarvestRecBegin, DateTime? dateHarvestRecEnd, bool isGreenHouseRec, string description, string wateringConditions, int length, int width, string imagePath, bool isAddButton, int usedId, User user, DateTime dateSowing, int x, int y) : base(id, name, dateSowingRecBegin, dateHarvestRecEnd, dateHarvestRecBegin, dateHarvestRecEnd, isGreenHouseRec, description, wateringConditions, length, width, imagePath, isAddButton, usedId, user)
        {
            DateSowing = dateSowing;
            XCoordinate = x;
            YCoordinate = y;
        }

        public PlantInGarden(UsedPlant plant, DateTime dateSowing, int x, int y) : base(plant)
        {
            DateSowing = dateSowing;
            XCoordinate = x;
            YCoordinate = y;
        }
    }
}
