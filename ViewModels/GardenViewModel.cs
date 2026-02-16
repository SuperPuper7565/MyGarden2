using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MyGarden2.UserControls;
using MyGarden2.Models;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace MyGarden2.ViewModels
{
    class GardenViewModel : INotifyPropertyChanged
    {
        private object _currentView;

        private string head;
        public string Head
        {
            get => head;
            set
            {
                head = value;
                OnPropertyChanged(nameof(Head));
            }
        }
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public User User { get; set; }

        public GardenViewModel()
        {
            // Начальный контент
            CurrentView = null;
            Head = "Добро пожаловать, садовник!";
        }

        public void ShowCollection()
        {
            CurrentView = new Collection(User);
            Head = "Моя коллекция";
        }
        public void ShowGardenBeds()
        {
            CurrentView = new GardenBeds(User);
            Head = "Грядки";
        }
        public void ShowPlants()
        {
            CurrentView = new Plants(User);
            Head = "Растения";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void RegisterUser(string name, string login, string password)
        {
            try
            {
                // Генерация соли и хеша пароля
                string salt = Password.GenerateSalt();
                string passwordHash = Password.GetPasswordHash(password, salt);

                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO users (name, login, password_hash, salt) 
                            VALUES (@name, @login, @passwordHash, @salt)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@passwordHash", passwordHash);
                        command.Parameters.AddWithValue("@salt", salt);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Не удалось зарегистрировать пользователя");
                        }
                    }
                }
                User = GetUserByLogin(login);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка регистрации: {ex.Message}");
            }
        }

        public void LoginUser(string login, string password)
        {
            try
            {
                // Получаем пользователя из базы
                User = GetUserByLogin(login);

                if (User == null)
                {
                    throw new Exception("Пользователь не найден");
                }

                // Проверяем пароль
                bool isPasswordValid = VerifyPassword(password, User.PasswordHash, User.Salt);

                if (!isPasswordValid)
                {
                    throw new Exception("Неверный пароль");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка входа: {ex.Message}");
            }
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM users WHERE login = @login";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32("id"),
                                    Name = reader.GetString("name"),
                                    Login = reader.GetString("login"),
                                    PasswordHash = reader.GetString("password_hash"),
                                    Salt = reader.GetString("salt")
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при поиске пользователя: {ex.Message}");
            }
        }

        public static bool VerifyPassword(string enteredPassword, string savedPasswordHash, string savedSaltBase64)
        {
            string newHash = Password.GetPasswordHash(enteredPassword, savedSaltBase64);
            return newHash.StartsWith(savedPasswordHash);
        }

        internal void DeleteAccount()
        {
            try
            {
                if (User == null)
                {
                    throw new Exception("Пользователь не найден");
                }
                using (var connection = new MySqlConnection(AppConfig.ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM users WHERE id = @id";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", User.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Невозможно удалить аккаунт: {ex.Message}");
            }
        }
    }
}
