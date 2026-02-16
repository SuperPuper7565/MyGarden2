using MyGarden2.Models;
using MyGarden2.UserControls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyGarden2.ViewModels
{
    public class PlantsSearchViewModel
    {
        public ObservableCollection<Plant> Plants { get; set; }

        public PlantsSearchViewModel()
        {
            Plants = new ObservableCollection<Plant>();
        }

        public void SearchPlants(string textFromTextBox)
        {
            try
            {
                if (textFromTextBox != null && textFromTextBox != "")
                {
                    string query = $"SELECT * FROM plants WHERE name LIKE @text";
                    using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                    {
                        connection.Open();
                        var cmd = new MySqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@text", textFromTextBox + "%");
                        LoadPlants(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void LoadPlants(MySqlCommand command)
        {
            try
            {
                Plants.Clear();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Plants.Add(new Plant
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки растений: {ex.Message}");
            }
        }
    }
}
