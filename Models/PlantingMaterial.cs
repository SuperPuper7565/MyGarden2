using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGarden2.Models
{
    public class PlantingMaterial
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAddButton { get; set; } = false;
        public ObservableCollection<PlantingCatalog> PlantingCatalog { get; set; }
        public PlantingMaterial() // Плюсик
        {
            Name = "+ новый материал";
            IsAddButton = true;
        }
        public PlantingMaterial(int id, string name)
        {
            Id = id;
            Name = name;
            PlantingCatalog = new ObservableCollection<PlantingCatalog>();
        }
    }
}
