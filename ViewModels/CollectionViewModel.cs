using MyGarden2.Models;
using MyGarden2.UserControls;
using MySql.Data.MySqlClient;
using Mysqlx.Connection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace MyGarden2.ViewModels
{
    class CollectionViewModel
    {
        private User user;
        private string queryPlantsDefault = "SELECT plants.*, used_plants.id as used_id FROM plants JOIN used_plants ON used_plants.id_plant = plants.id WHERE used_plants.id_user = @id GROUP BY used_plants.id_plant;";
        public ObservableCollection<UsedGardenBed> UsedGardenBeds { get; set; }
        public ObservableCollection<UsedPlant> ChosenPlants { get; set; }
        public ObservableCollection<PlantInGarden> PlantsInGarden { get; set; }

        public CollectionViewModel(User user)
        {
            UsedGardenBeds = new ObservableCollection<UsedGardenBed>();
            ChosenPlants = new ObservableCollection<UsedPlant>();
            
            this.user = user;
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                LoadGardenBeds(connection);
                LoadPlants(connection, queryPlantsDefault);
            }
        }

        private void LoadGardenBeds(MySqlConnection connection)
        {
            UsedGardenBeds.Clear();

            string query = "SELECT " +
               "used_garden_beds.id AS used_garden_beds_id, " +
               "garden_beds.*, " +
               "used_garden_beds.id_user AS user_id " +
               "FROM used_garden_beds " +
               "JOIN garden_beds ON used_garden_beds.id_garden_bed = garden_beds.id " +
               "WHERE used_garden_beds.id_user = @id;";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", user.Id);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var gardenBed = new UsedGardenBed
                        {
                            UsedGBId = reader.GetInt32(0),
                            UserId = reader.GetInt32(6),
                            Id = reader.GetInt32(1),
                            IsGreenHouse = reader.GetBoolean(2),
                            LengthCells = reader.GetInt32(3),
                            WidthCells = reader.GetInt32(4),
                            CellSize = reader.GetInt32(5)
                        };

                        UsedGardenBeds.Add(gardenBed);
                    }
                }
            }
        }

        private void LoadPlants(MySqlConnection connection, string query, string text = "")
        {
            ChosenPlants.Clear();
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@text", text + "%");
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Plant plant = new Plant(reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetDateTime(2),
                            reader.GetDateTime(3),
                            reader.GetDateTime(4),
                            reader.GetDateTime(5),
                            reader.GetBoolean(6),
                            reader.GetString(7),
                            reader.GetString(8),
                            reader.GetInt32(9),
                            reader.GetInt32(10),
                            reader.GetString(11),
                            false);
                        var chosenPlant = new UsedPlant(reader.GetInt32(13), user, plant);
                        ChosenPlants.Add(chosenPlant);
                    }
                }
            }
        }

        public void DeletePlant(UsedPlant plant)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM used_plants WHERE id_user = @id_user AND id_plant = @id_plant";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("id_user", user.Id);
                        command.Parameters.AddWithValue("@id_plant", plant.Id);
                        command.ExecuteNonQuery();
                    }
                }
                ChosenPlants.Remove(plant);
                MessageBox.Show($"Растение {plant.Name} успешно удалено из вашей коллекции", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления из collection: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void DeleteGardenBed(UsedGardenBed gardenBed)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM used_garden_beds WHERE id = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", gardenBed.UsedGBId);
                        command.ExecuteNonQuery();
                    }
                }
                UsedGardenBeds.Remove(gardenBed);
                MessageBox.Show($"Грядка успешно удалена из вашей коллекции", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления из collection: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddToDatabase(int idGardenBed, int idPlant, int x, int y, DateTime? selectedDate = null)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO collection 
                          (id_used_garden_bed, id_used_plant, x_coordinate, y_coordinate, date_sowing) 
                          VALUES (@id_garden, @id_plant, @x, @y, @date)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_garden", idGardenBed);
                        command.Parameters.AddWithValue("@id_plant", idPlant);
                        command.Parameters.AddWithValue("@x", x);
                        command.Parameters.AddWithValue("@y", y);
                        command.Parameters.AddWithValue("@date", selectedDate ?? DateTime.Now);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении растения: {ex.Message}");
            }
        }

        public void SearchPlants(string text)
        {
            try
            {
                if (text != null && text != "")
                {
                    string query = "SELECT plants.*, used_plants.id as used_id FROM plants JOIN used_plants ON used_plants.id_plant = plants.id WHERE used_plants.id_user = @id AND name LIKE @text GROUP BY used_plants.id_plant;";
                    using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                    {
                        connection.Open();
                        LoadPlants(connection, query, text);
                    }
                }
                else
                {
                    using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                    {
                        connection.Open();
                        LoadPlants(connection, queryPlantsDefault);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int GetPlantsCountOnGardenBed(int gardenBedId)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM collection WHERE id_used_garden_bed = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", gardenBedId);
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении количества растений: {ex.Message}");
                return 0;
            }
        }
    }
}
