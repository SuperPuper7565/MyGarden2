using MyGarden2.Models;
using MyGarden2.UserControls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyGarden2.ViewModels
{
    public class PlantsViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private object _listCurrentView;
        private object _plantCurrentView;

        public object ListCurrentView
        {
            get => _listCurrentView;
            set
            {
                _listCurrentView = value;
                OnPropertyChanged(nameof(ListCurrentView));
            }
        }

        public object PlantCurrentView
        {
            get => _plantCurrentView;
            set
            {
                _plantCurrentView = value;
                OnPropertyChanged(nameof(PlantCurrentView));
            }
        }
        private User _user;
        public PlantsViewModel(User user)
        {
            _user = user;
        }

        public void ShowPlantSearch() => ListCurrentView = new PlantsSearch();
        public void ShowPlantsTreeView() => ListCurrentView = new PlantsTreeView(this);
        public void ShowAddOrEditItem() => PlantCurrentView = new AddOrEditItem();
        public void ShowPlantCard(Plant p)
        {
            var plantCard = new PlantCard(p);
            PlantCurrentView = plantCard;
            plantCard.PlantConfirm += (s, e) => AddPlantToCollection(p.Id);
            plantCard.ChangeConfirm += (s, e) =>
            {
                var editCard = new AddOrEditPlant(p);
                PlantCurrentView = editCard;
                editCard.Confirm += (s, e) => UpdateItem(p, editCard.NewPlant);
            };
            plantCard.DeleteConfirm += (s, e) => ConfirmDelete(p);
            plantCard.CloseRequested += (s, e) => CloseUC();
        }

        private void ConfirmDelete(Plant p)
        {
            ShowPlantCard(p);
            var result = MessageBox.Show($"Вы уверены, что хотите удалить {p.Name} из коллекции?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DeleteItem(p);
                CloseUC();
            }
        }

        private void CloseUC() => PlantCurrentView = null;

        public void ShowAddOrEditPlant() => PlantCurrentView = new AddOrEditPlant();
        public void ShowAddOrEditPlant(Plant p) => PlantCurrentView = new AddOrEditPlant(p);

        public void AddPlantToCollection(int plantId)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = "SELECT COUNT(*) FROM used_plants WHERE id_user = @user AND id_plant = @plant";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@user", _user.Id);
                        cmd.Parameters.AddWithValue("@plant", plantId);
                        long count = (long)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Это растение уже есть в вашей коллекции.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                    query = $"INSERT INTO used_plants (id_user, id_plant) VALUES (@user, @plant)";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@user", _user.Id);
                        cmd.Parameters.AddWithValue("@plant", plantId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Растение успешно добавлено в коллекцию. Перейдите в вашу коллекцию для того, чтобы посадить его.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить растение в коллекцию: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateItem(object selectedItem, object data)
        {
            try
            {
                if (selectedItem is PlantingMaterial pm && data is string)
                {
                    UpdateInDatabase("planting_materials", pm.Id, (string)data);
                    pm.Name = (string)data;
                    MessageBox.Show($"Материал успешно обновлён. Новое название: {pm.Name}.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selectedItem is PlantingCatalog pc && data is string)
                {
                    UpdateInDatabase("planting_catalogs", pc.Id, (string)data);
                    pc.Name = (string)data;
                    MessageBox.Show($"Каталог успешно обновлён. Новое название: {pc.Name}.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selectedItem is PlantType pt && data is string)
                {
                    UpdateInDatabase("plant_types", pt.Id, (string)data);
                    pt.Name = (string)data;
                    MessageBox.Show($"Тип растения успешно обновлён. Новое название: {pt.Name}.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (selectedItem is Plant toUpdate && data is Plant newPlant)
                {
                    UpdateInDatabase(toUpdate.Id, newPlant);
                    toUpdate = newPlant;
                    ShowPlantCard(newPlant);
                    MessageBox.Show($"Данные о растении {toUpdate.Name} успешно обновлены.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                RefreshPlantsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateInDatabase(string tableName, int id, string name)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = $"UPDATE {tableName} SET name = @name WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка редактирования {tableName}: {ex.Message}");
            }
        }
        private void UpdateInDatabase(int id, Plant plant)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = "UPDATE plants SET name = @name, date_sowing_rec_begin = @dsrb, date_sowing_rec_end = @dsre, date_harvest_rec_begin = @dhrb, date_harvest_rec_end = @dhre, is_greenhouse_rec = @igr, description = @des, watering_conditions = @wc, length = @length, width = @width, image_path = @ip WHERE id = @id";
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
                        cmd.Parameters.AddWithValue("@ip", plant.ImagePath);
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка редактирования plants: {ex.Message}");
            }
        }

        public void DeleteItem(Plant plant)
        {
            if (ListCurrentView is PlantsTreeView treeView)
            {
                treeView.DeleteItem(plant);
            }
            else
            {
                DeleteFromDatabase(plant.Id);
            }
            RefreshPlantsList();
        }
        private void DeleteFromDatabase(int id)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = $"DELETE FROM plants WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления растения: {ex.Message}");
            }
        }

        public void RefreshPlantsList()
        {
            if (ListCurrentView is PlantsTreeView treeView)
            {
                treeView.RefreshData();
            }
            else if (ListCurrentView is PlantsSearch searchView)
            {
                searchView.RefreshData();
            }
        }
    }
}
