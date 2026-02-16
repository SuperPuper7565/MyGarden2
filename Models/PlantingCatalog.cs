using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGarden2.ViewModels;

namespace MyGarden2.Models
{
    public class PlantingCatalog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAddButton { get; set; } = false;
        public ObservableCollection<PlantType> PlantTypes { get; set; }
        public PlantingCatalog()
        {
            Name = "+ новый каталог";
            IsAddButton = true;
        }
        public PlantingCatalog(int id, string name)
        {
            Id = id;
            Name = name;
            PlantTypes = new ObservableCollection<PlantType>();
        }
    }
}
