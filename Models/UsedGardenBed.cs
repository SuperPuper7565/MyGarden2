using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class UsedGardenBed: GardenBed
    {
        public int UserId { get; set; }
        public int UsedGBId { get; set; }

        public UsedGardenBed() { }

        public UsedGardenBed(int id, bool isGreenHouse, int length, int width, int cellSize, int userId, int usedGBId) : base(id, isGreenHouse, length, width, cellSize)
        {
            UserId = userId;
            UsedGBId = usedGBId;
        }

    }
}
