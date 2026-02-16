using MaterialDesignThemes.Wpf;
using MyGarden2.Models;
using MyGarden2.UserControls;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyGarden2.ViewModels
{
    class GardenBedsViewModel: INotifyPropertyChanged
    {

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
        private User user;

        private ListView listView;

        private GardenBed _selectedGardenBed;
        public GardenBed SelectedGardenBed
        {
            get => _selectedGardenBed;
            set
            {
                _selectedGardenBed = value;
                OnPropertyChanged(nameof(SelectedGardenBed));

                ShowGardenBedCard();
            }
        }

        private void ShowGardenBedCard()
        {
            // Обновляем CurrentView для отображения выбранной грядки
            if (_selectedGardenBed != null)
            {
                GardenBedCard cv = new GardenBedCard { DataContext = _selectedGardenBed };
                CurrentView = cv;
                cv.DeleteConfirm += (s, e) => DeleteGardenBed(cv);
                cv.ChangeConfirm += (s, e) => UpdateGardenBed(cv);
                cv.AddConfirm += (s, e) => AddGardenBedToCollection(user.Id, _selectedGardenBed.Id);

            }
        }

        private void UpdateGardenBed(GardenBedCard cv)
        {
            try
            {
                var gb = cv.DataContext as GardenBed;
                var edit = new AddOrEditGardenBed(gb.LengthCells, gb.WidthCells, gb.IsGreenHouse, gb.CellSize);
                CurrentView = edit;
                edit.Confirm += (s, e) =>
                {
                    var gbNew = edit.GardenBed;
                    UpdateInDatabase(gb.Id, gbNew);
                    _selectedGardenBed = gbNew;
                    cv.DataContext = _selectedGardenBed;
                    cv.Visibility = Visibility.Visible;
                    CurrentView = cv;
                    for (int i = 0; i < GardenBeds.Count; i++)
                    {
                        if (GardenBeds[i] == _selectedGardenBed)
                            GardenBeds[i] = gbNew;
                    }
                    MessageBox.Show("Грядка успешно обновлена", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteGardenBed(GardenBedCard cv)
        {
            try
            {
                ShowGardenBedCard();
                var result = MessageBox.Show("Вы уверены, что хотите удалить грядку? Грядки такого типа также будут удалены из коллекции.", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var gb = cv.DataContext as GardenBed;
                    DeleteFromDatabase(gb.Id);
                    GardenBeds.Remove(gb);
                    CurrentView = null;
                    MessageBox.Show("Грядка успешно удалена", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddGardenBedToCollection(int userId, int gardenBedId)
        {
            try
            {
                using (var _connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    _connection.Open();
                    string query = $"INSERT INTO used_garden_beds (id_user, id_garden_bed) VALUES (@user, @gb)";
                    using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("@user", userId);
                        cmd.Parameters.AddWithValue("@gb", gardenBedId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show($"Грядка успешно добавлено в коллекцию. Перейдите в вашу коллекцию для того, чтобы посадить на неё растения.", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось добавить грядку в коллекцию: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<GardenBed> GardenBeds { get; set; }
        public GardenBedsViewModel(ListView listView, User user)
        {
            GardenBeds = new ObservableCollection<GardenBed>();
            using (var connection = new MySqlConnection(AppConfig.ConnectionString))
            {
                connection.Open();
                var cmd = new MySqlCommand("SELECT * FROM garden_beds", connection);
                LoadGardenBeds(cmd);
            }
            this.listView = listView;
            this.user = user;
        }

        private void LoadGardenBeds(MySqlCommand command)
        {
            try
            {
                GardenBeds.Clear();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GardenBeds.Add(new GardenBed
                        (
                            reader.GetInt32(0),
                            reader.GetBoolean(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4)
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование ошибки, чтобы понять, что именно пошло не так
                MessageBox.Show($"Ошибка загрузки грядок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void AddGardenBed()
        {
            try
            {
                var uc = new AddOrEditGardenBed();
                CurrentView = uc;
                uc.Confirm += (s, e) =>
                {
                    var gb = uc.GardenBed;
                    gb.Id = AddToDatabase(gb);
                    GardenBeds.Add(gb);
                    MessageBox.Show("Новая грядка успешно добавлена", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private int AddToDatabase(GardenBed gb)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO garden_beds (is_greenhouse, length, width, cell_size) VALUES (@ig, @length, @width, @cs);" +
                                   $"SELECT LAST_INSERT_ID()";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ig", gb.IsGreenHouse);
                        cmd.Parameters.AddWithValue("@length", gb.LengthCells);
                        cmd.Parameters.AddWithValue("@width", gb.WidthCells);
                        cmd.Parameters.AddWithValue("@cs", gb.CellSize);
                        object result = cmd.ExecuteScalar();
                        return (result != null) ? Convert.ToInt32(result) : -1;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка добавления грядки: {ex.Message}");
            }
            return -1;
        }
        private void UpdateInDatabase(int id, GardenBed gb)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE garden_beds SET is_greenhouse = @ig, length = @length, width = @width, cell_size = @cs WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@ig", gb.IsGreenHouse);
                        cmd.Parameters.AddWithValue("@length", gb.LengthCells);
                        cmd.Parameters.AddWithValue("@width", gb.WidthCells);
                        cmd.Parameters.AddWithValue("@cs", gb.CellSize);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка редактирования грядки: {ex.Message}");
            }
        }
        private void DeleteFromDatabase(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = $"DELETE FROM garden_beds WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception ($"Ошибка удаления грядки: {ex.Message}");
            }
        }
    }
}
