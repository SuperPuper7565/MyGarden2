using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class UsedPlant: Plant
    {
        public int UsedId { get; set; }
        public User User { get; set; }
        public UsedPlant(int id, string name, DateTime? dateSowingRecBegin, DateTime? dateSowingRecEnd, DateTime? dateHarvestRecBegin, DateTime? dateHarvestRecEnd, bool isGreenHouseRec, string description, string wateringConditions, int length, int width, string imagePath, bool isAddButton, int usedId, User user): base (id, name, dateSowingRecBegin, dateHarvestRecEnd, dateHarvestRecBegin, dateHarvestRecEnd, isGreenHouseRec, description, wateringConditions, length, width, imagePath, isAddButton)
        {
            UsedId = usedId;
            User = user;
        }
        public UsedPlant(int usedId, User user, Plant plant): base(plant)
        {
            UsedId = usedId;
            User = user;
        }
        public UsedPlant(UsedPlant plant): base(plant)
        {
            UsedId = plant.UsedId;
            User = plant.User;
        }
        public string IsGreenHouseRecText
        {
            get
            {
                if (!IsGreenHouseRec)
                    return "Рекомендуется сажать на грядку";
                return "Рекомендуется сажать в теплицу";
            }
        }
    }
}
