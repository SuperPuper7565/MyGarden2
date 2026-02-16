using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class PlantType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAddButton { get; set; } = false;
        public ObservableCollection<Plant> Plants { get; set; }
        public PlantType()
        {
            Name = "+ новый тип";
            IsAddButton = true;
        }
        public PlantType(int id, string name)
        {
            Id = id;
            Name = name;
            Plants = new ObservableCollection<Plant>();
        }
    }
}
