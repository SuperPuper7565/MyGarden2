using MyGarden2.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MyGarden2.UserControls;
using System.IO.Packaging;
using MySqlX.XDevAPI.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Xml.Linq;
using MyGarden2.Windows;
using System.Data.Common;
using Mysqlx.Session;
using Mysqlx.Crud;

namespace MyGarden2.ViewModels
{
    public class PlantsTreeViewModel: INotifyPropertyChanged
    {
        string queryPlantingMaterials = "SELECT * FROM planting_materials";
        string queryPlantingCatalog = "SELECT * FROM planting_catalogs WHERE id_planting_material = @id";
        string queryPlantTypes = "SELECT * FROM plant_types WHERE id_planting_catalog = @id";
        string queryPlants = "SELECT * FROM plants WHERE id_plant_type = @id";
        public ObservableCollection<PlantingMaterial> PlantingMaterials { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        public void ShowAddOrEditItem() => CurrentView = new AddOrEditItem();
        public void ShowPlantCard(Plant p) => CurrentView = new PlantCard(p);
        public void ShowAddOrEditPlant() => CurrentView = new AddOrEditPlant();
        public void ShowAddOrEditPlant(Plant p) => CurrentView = new AddOrEditPlant(p);
        public PlantsTreeViewModel()
        {
            PlantingMaterials = new ObservableCollection<PlantingMaterial>();
            LoadPlantingMaterials();
        }
        private void LoadPlantingMaterials()
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    using (var command = new MySqlCommand(queryPlantingMaterials, _connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlantingMaterials.Add(new PlantingMaterial(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        for (int i = 0; i < PlantingMaterials.Count; i++)
                        {
                            int idPM = PlantingMaterials[i].Id;
                            PlantingMaterials[i].PlantingCatalog = LoadPlantingCatalog(idPM);
                        }
                        PlantingMaterials.Add(new PlantingMaterial());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки посадочного материала: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private ObservableCollection<PlantingCatalog> LoadPlantingCatalog(int id)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    ObservableCollection<PlantingCatalog> plantingCatalogs = new ObservableCollection<PlantingCatalog>();
                    using (var command = new MySqlCommand(queryPlantingCatalog, _connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                plantingCatalogs.Add(new PlantingCatalog(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        for (int i = 0; i < plantingCatalogs.Count; i++)
                        {
                            int idPC = plantingCatalogs[i].Id;
                            plantingCatalogs[i].PlantTypes = LoadPlantTypes(idPC);
                        }
                        plantingCatalogs.Add(new PlantingCatalog());
                        return plantingCatalogs;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки каталога: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        private ObservableCollection<PlantType> LoadPlantTypes(int id)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    ObservableCollection<PlantType> plantingTypes = new ObservableCollection<PlantType>();
                    using (var command = new MySqlCommand(queryPlantTypes, _connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                plantingTypes.Add(new PlantType(reader.GetInt32(0), reader.GetString(1)));
                            }
                        }
                        for (int i = 0; i < plantingTypes.Count; i++)
                        {
                            int idPT = plantingTypes[i].Id;
                            plantingTypes[i].Plants = LoadPlants(idPT);
                        }
                        plantingTypes.Add(new PlantType());
                        return plantingTypes;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов растений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        private ObservableCollection<Plant> LoadPlants(int id)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    ObservableCollection<Plant> plants = new ObservableCollection<Plant>();
                    using (var command = new MySqlCommand(queryPlants, _connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                plants.Add(new Plant
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    DateSowingRecBegin = reader.GetDateTime(2),
                                    DateSowingRecEnd = reader.GetDateTime(3),
                                    DateHarvestRecBegin = reader.GetDateTime(4),
                                    DateHarvestRecEnd = reader.GetDateTime(5),
                                    IsGreenHouseRec = reader.GetBoolean(6),
                                    Description = reader.GetString(7),
                                    WateringConditions = reader.GetString(8),
                                    Length = reader.GetInt32(9),
                                    Width = reader.GetInt32(10),
                                    ImagePath = reader.GetString(11)
                                });
                            }
                        }
                        plants.Add(new Plant());
                        return plants;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки растений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void DeleteItem(object selectedItem)
        {
            try
            {
                if (selectedItem is Plant plant)
                {
                    PlantType parent = FindParentInDataContext(plant);
                    DeleteFromDatabase("plants", plant.Id);
                    parent.Plants.Remove(plant);
                    MessageBox.Show($"Растение {plant.Name} удалено", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selectedItem is PlantType plantType)
                {
                    PlantingCatalog parent = FindParentInDataContext(plantType);
                    DeleteFromDatabase("plant_types", plantType.Id);
                    parent.PlantTypes.Remove(plantType);
                    MessageBox.Show($"Тип растения {plantType.Name} удалён", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selectedItem is PlantingCatalog catalog)
                {
                    PlantingMaterial parent = FindParentInDataContext(catalog);
                    DeleteFromDatabase("planting_catalogs", catalog.Id);
                    parent.PlantingCatalog.Remove(catalog);
                    MessageBox.Show($"Каталог {catalog.Name} удалён", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selectedItem is PlantingMaterial material)
                {
                    DeleteFromDatabase("planting_materials", material.Id);
                    PlantingMaterials.Remove(material);
                    MessageBox.Show($"Материал {material.Name} удалён", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                OnPropertyChanged(nameof(PlantingMaterials));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddItem(object data, object selected = null)
        {
            try
            {
                if (data is string && selected == null)
                {
                    int id = AddToDatabase((string)data);
                    PlantingMaterial pm = new PlantingMaterial(id, (string)data);
                    pm.PlantingCatalog.Add(new PlantingCatalog());
                    PlantingMaterials.Insert(PlantingMaterials.Count - 1, pm);
                    MessageBox.Show($"Материал {pm.Name} успешно добавлен.", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (data is string && selected is PlantingCatalog c)
                {
                    PlantingMaterial parent = FindParentInDataContext(c);
                    int id = AddToDatabase("planting_catalogs", (string)data, "planting_material", parent.Id);
                    PlantingCatalog pc = new PlantingCatalog(id, (string)data);
                    pc.PlantTypes.Add(new PlantType());
                    parent.PlantingCatalog.Insert(parent.PlantingCatalog.Count - 1, pc);
                    MessageBox.Show($"Каталог {pc.Name} успешно добавлен.", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (data is string && selected is PlantType t)
                {
                    PlantingCatalog parent = FindParentInDataContext(t);
                    int id = AddToDatabase("plant_types", (string)data, "planting_catalog", parent.Id);
                    PlantType pt = new PlantType(id, (string)data);
                    pt.Plants.Add(new Plant());
                    parent.PlantTypes.Insert(parent.PlantTypes.Count - 1, pt);
                    MessageBox.Show($"Тип {pt.Name} успешно добавлен.", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (data is Plant plant && selected is Plant p)
                {
                    PlantType parent = FindParentInDataContext(p);
                    plant.Id = AddToDatabase(plant, parent.Id);
                    parent.Plants.Insert(parent.Plants.Count - 1, plant);
                    MessageBox.Show($"Растение {plant.Name} успешно добавлен.", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    
        private void DeleteFromDatabase(string tableName, int id)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = $"DELETE FROM {tableName} WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления из {tableName}: {ex.Message}");
            }
        }

        private int AddToDatabase(string name)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = $"INSERT INTO planting_materials (name) VALUES (@name);" +
                                   $"SELECT LAST_INSERT_ID()";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        object result = cmd.ExecuteScalar();
                        return (result != null) ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось добавить материал: {ex.Message}");
            }
        }
        private int AddToDatabase(string tableName, string name, string parentTableName, int id)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = $"INSERT INTO {tableName} (name, id_{parentTableName}) VALUES (@name, @id);" +
                                   $"SELECT LAST_INSERT_ID(); ";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@id", id);
                        object result = cmd.ExecuteScalar();
                        return (result != null) ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось добавить {tableName}: {ex.Message}");
            }
        }
        private int AddToDatabase(Plant plant, int parentId)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = @"INSERT INTO plants (name, date_sowing_rec_begin, date_sowing_rec_end, date_harvest_rec_begin, date_harvest_rec_end, is_greenhouse_rec, description, watering_conditions, length, width, image_path, id_plant_type) VALUES (@name, @dsrb, @dsre, @dhrb, @dhre, @igr, @des, @wc, @length, @width, @ip, @idpt); SELECT LAST_INSERT_ID();";

                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@name", plant.Name);
                        cmd.Parameters.AddWithValue("@dsrb", plant.DateSowingRecBegin);
                        cmd.Parameters.AddWithValue("@dsre", plant.DateSowingRecEnd);
                        cmd.Parameters.AddWithValue("@dhrb", plant.DateHarvestRecBegin);
                        cmd.Parameters.AddWithValue("@dhre", plant.DateHarvestRecEnd);
                        cmd.Parameters.AddWithValue("@igr", plant.IsGreenHouseRec);
                        cmd.Parameters.AddWithValue("@wc", plant.WateringConditions);
                        cmd.Parameters.AddWithValue("@length", plant.Length);
                        cmd.Parameters.AddWithValue("@width", plant.Width);
                        cmd.Parameters.AddWithValue("@des", plant.Description);
                        cmd.Parameters.AddWithValue("@idpt", parentId);
                        cmd.Parameters.AddWithValue("@ip", plant.ImagePath);
                        object result = cmd.ExecuteScalar();
                        return (result != null) ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Не удалось добавить растение: {ex.Message}");
            }
        }
        public PlantingMaterial FindParentInDataContext(PlantingCatalog catalog)
        {
            // Найдем родительский элемент в данных. Мы предполагаем, что родительский элемент - это PlantingMaterial
            foreach (var pm in PlantingMaterials)
            {
                if (!pm.IsAddButton && pm.PlantingCatalog is not null && pm.PlantingCatalog.Contains(catalog))
                {
                    return pm;
                }
            }
            return null;
        }
        public PlantingCatalog FindParentInDataContext(PlantType type)
        {
            foreach (var pm in PlantingMaterials)
            {
                if (!pm.IsAddButton)
                {
                    foreach (var pc in pm.PlantingCatalog)
                    {
                        if (!pc.IsAddButton && pc.PlantTypes is not null && pc.PlantTypes.Contains(type))
                        {
                            return pc;
                        }
                    }
                }
            }
            return null;
        }
        public PlantType FindParentInDataContext(Plant plant)
        {
            foreach (var pm in PlantingMaterials)
            {
                if (!pm.IsAddButton)
                {
                    foreach (var pc in pm.PlantingCatalog)
                    {
                        if (!pc.IsAddButton)
                        {
                            foreach (var pt in pc.PlantTypes)
                            {
                                if (!pt.IsAddButton && pt.Plants is not null && pt.Plants.Contains(plant))
                                {
                                    return pt;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
