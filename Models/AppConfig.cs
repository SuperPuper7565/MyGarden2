using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyGarden2.Models
{
    public class AppConfig
    {
        public static string ConnectionString { get; } = "Server=localhost;Database=garden;User ID=root;Password=;";

        public static string CalculateDaysToHarvest(PlantInGarden plant)
        {
            var daysLeft = (plant.DateHarvestRecBegin - DateTime.Now)?.Days;
            return daysLeft > 0
            ? $"Осталось дней до сбора урожая: {daysLeft}"
            : $"Готово к сбору!\nУспейте собрать до {plant.DateHarvestRecEnd.Value.ToShortDateString()}";
        }

        public static ObservableCollection<PlantInGarden> LoadPlantsFromDatabase(int gardenBedId)
        {
            try
            {
                var plants = new ObservableCollection<PlantInGarden>();
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT plants.*, used_plants.id as used_id, users.id, users.name, users.login, 
                                  collection.date_sowing, 
                                  collection.x_coordinate, collection.y_coordinate
                                  FROM collection
                                  JOIN used_plants ON collection.id_used_plant = used_plants.id
                                  JOIN plants ON used_plants.id_plant = plants.id
                                  JOIN users ON used_plants.id_user = users.id
                                  WHERE collection.id_used_garden_bed = @id_used_garden_bed";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_used_garden_bed", gardenBedId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var plant = new PlantInGarden(
                                    reader.GetInt32(0),
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
                                    false,
                                    reader.GetInt32(13),
                                    new User
                                    {
                                        Id = reader.GetInt32(14),
                                        Name = reader.GetString(15),
                                        Login = reader.GetString(16)
                                    },
                                    reader.GetDateTime(17),
                                    reader.GetInt32(18),
                                    reader.GetInt32(19)
                                );
                                plants.Add(plant);
                            }
                        }
                    }
                }
                return plants;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки растений: {ex.Message}");
            }
        }

        public static bool IsSpaceOccupied(UsedGardenBed gardenBed, int xCell, int yCell, int plantWidthCells, int plantHeightCells)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();

                    // Получаем все растения на этой грядке (координаты в клетках)
                    string query = @"SELECT x_coordinate, y_coordinate, 
                            CEILING(plants.width / @cellSizeCm) as plant_width_cells, 
                            CEILING(plants.length / @cellSizeCm) as plant_height_cells
                            FROM collection
                            JOIN used_plants ON collection.id_used_plant = used_plants.id
                            JOIN plants ON used_plants.id_plant = plants.id
                            WHERE collection.id_used_garden_bed = @id_garden";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id_garden", gardenBed.UsedGBId);
                        command.Parameters.AddWithValue("@cellSizeCm", gardenBed.CellSize); // Используем размер клетки в см

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int existingX = reader.GetInt32(0); // Координата X в клетках
                                int existingY = reader.GetInt32(1); // Координата Y в клетках
                                int existingWidthCells = reader.GetInt32(2); // Ширина в клетках
                                int existingHeightCells = reader.GetInt32(3); // Высота в клетках

                                // Проверяем пересечение прямоугольников (все в клетках)
                                bool xOverlap = xCell < existingX + existingWidthCells &&
                                                xCell + plantWidthCells > existingX;

                                bool yOverlap = yCell < existingY + existingHeightCells &&
                                               yCell + plantHeightCells > existingY;

                                if (xOverlap && yOverlap)
                                {
                                    return true; // Место занято
                                }
                            }
                        }
                    }
                }
                return false; // Место свободно
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки занятости места: {ex.Message}");
                return true; // В случае ошибки считаем место занятым
            }
        }
    }
}
