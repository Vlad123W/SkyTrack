using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SkyTrack
{
    internal class SqlQuery
    {
        private string? dbName;
        private MySqlConnection? connection;
        private string connectionString;

        public string ConnectionString
        {
            get => connectionString;
            set => connectionString = value;
        }


        public SqlQuery(string dbName)
        {
            this.dbName = dbName;

            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
            
            connectionStringBuilder.Server = "localhost";
            connectionStringBuilder.UserID = "root";
            connectionStringBuilder.Password = "LfymjrKj[930";
            connectionStringBuilder.Database = dbName;
            connectionStringBuilder.Port = 3306;

            ConnectionString = connectionStringBuilder.ToString();

            EnsureDatabaseExists();
            EnsureTablesExist();
            connection = new MySqlConnection(connectionString);
        }

        private int GenerateUniqueUserId()
        {
            int id;
            Random rand = new Random();
            do
            {
                id = rand.Next(100000, 999999);
                var check = ExecuteQuery("SELECT 1 FROM Users WHERE id = @id",
                    new MySqlParameter("@id", id));
                if (check.Rows.Count == 0)
                    break;
            } while (true);
            return id;
        }

        private void EnsureDatabaseExists()
        {
            using var serverConnection = new MySqlConnection(connectionString);
            serverConnection.Open();
            string sql = $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;";
            using var cmd = new MySqlCommand(sql, serverConnection);
            cmd.ExecuteNonQuery();
        }

        private void EnsureTablesExist()
        {
            using var dbConnection = new MySqlConnection($"{connectionString};Database={dbName}");
            dbConnection.Open();

            string createUsers = @"CREATE TABLE IF NOT EXISTS Users (
                id INT NOT NULL PRIMARY KEY,
                login VARCHAR(100) NOT NULL UNIQUE,
                password VARCHAR(100) NOT NULL,
                is_admin BOOLEAN NOT NULL DEFAULT FALSE
            );";

            string createFlights = @"
            CREATE TABLE IF NOT EXISTS Flights (
                flight_id INT AUTO_INCREMENT PRIMARY KEY,
                origin VARCHAR(100) NOT NULL,
                destination VARCHAR(100) NOT NULL,
                departure_time DATETIME NOT NULL,
                arrival_time DATETIME NOT NULL,
                price DECIMAL(10,2) NOT NULL,
                available_seats INT NOT NULL
            );";

            using var cmdUsers = new MySqlCommand(createUsers, dbConnection);
            using var cmdFlights = new MySqlCommand(createFlights, dbConnection);
            cmdUsers.ExecuteNonQuery();
            cmdFlights.ExecuteNonQuery();
        }

        private void Open()
        {
            if (connection == null)
                connection = new MySqlConnection($"{connectionString};Database={dbName}");
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        private void Close()
        {
            if (connection?.State == ConnectionState.Open)
                connection.Close();
        }

        private DataTable ExecuteQuery(string sql, params MySqlParameter[] parameters)
        {
            var table = new DataTable();
            using var cmd = new MySqlCommand(sql, connection);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            using var adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(table);
            return table;
        }

        private int ExecuteNonQuery(string sql, params MySqlParameter[] parameters)
        {
            using var cmd = new MySqlCommand(sql, connection);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            return cmd.ExecuteNonQuery();
        }

        // Users table queries
        public User? GetUser(string login)
        {
            Open();
            var table = ExecuteQuery("SELECT * FROM Users WHERE login = @login",
                new MySqlParameter("@login", login));
            Close();

            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new User
            {
                Id = Convert.ToInt32(row["id"]),
                Login = row["login"].ToString(),
                Password = row["password"].ToString(),
                IsAdmin = Convert.ToBoolean(row["is_admin"])
            };
        }

        public List<User> GetAllUsers()
        {
            Open();
            var table = ExecuteQuery("SELECT * FROM Users");
            Close();

            var users = new List<User>();
            foreach (DataRow row in table.Rows)
            {
                users.Add(new User
                {
                    Id = Convert.ToInt32(row["id"]),
                    Login = row["login"].ToString(),
                    Password = row["password"].ToString(),
                    IsAdmin = Convert.ToBoolean(row["is_admin"])
                });
            }
            return users;
        }

        public bool AddUser(User user)
        {
            Open();
            var existing = ExecuteQuery("SELECT 1 FROM Users WHERE login = @login",
                new MySqlParameter("@login", user.Login));

            if (existing.Rows.Count > 0)
            {
                Close();
                return false; // пользователь уже существует
            }

            user.Id = GenerateUniqueUserId();

            var result = ExecuteNonQuery("INSERT INTO Users (id, login, password, is_admin) VALUES (@id, @login, @password, @is_admin)",
                new MySqlParameter("@id", user.Id),
                new MySqlParameter("@login", user.Login),
                new MySqlParameter("@password", user.Password!),
                new MySqlParameter("@is_admin", user.IsAdmin));
            Close();
            return result > 0;
        }

        public bool UpdateUser(User user)
        {
            Open();
            var result = ExecuteNonQuery("UPDATE Users SET password = @password, is_admin = @is_admin WHERE login = @login",
                new MySqlParameter("@login", user.Login),
                new MySqlParameter("@password", user.Password),
                new MySqlParameter("@is_admin", user.IsAdmin));
            Close();
            return result > 0;
        }

        public bool DeleteUser(string login)
        {
            Open();
            var result = ExecuteNonQuery("DELETE FROM Users WHERE login = @login",
                new MySqlParameter("@login", login));
            Close();
            return result > 0;
        }

        // Flights table queries
        public List<Flight> GetAllFlights()
        {
            Open();
            var table = ExecuteQuery("SELECT * FROM Flights");
            Close();

            var flights = new List<Flight>();
            foreach (DataRow row in table.Rows)
            {
                flights.Add(new Flight
                {
                    FlightId = Convert.ToInt32(row["flight_id"]),
                    Origin = row["origin"].ToString(),
                    Destination = row["destination"].ToString(),
                    DepartureTime = Convert.ToDateTime(row["departure_time"]),
                    ArrivalTime = Convert.ToDateTime(row["arrival_time"]),
                    Price = Convert.ToDecimal(row["price"])
                });
            }
            return flights;
        }

        public Flight? GetFlightById(int flightId)
        {
            Open();
            var table = ExecuteQuery("SELECT * FROM Flights WHERE flight_id = @flight_id",
                new MySqlParameter("@flight_id", flightId));
            Close();

            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new Flight
            {
                FlightId = Convert.ToInt32(row["flight_id"]),
                Origin = row["origin"].ToString(),
                Destination = row["destination"].ToString(),
                DepartureTime = Convert.ToDateTime(row["departure_time"]),
                ArrivalTime = Convert.ToDateTime(row["arrival_time"]),
                Price = Convert.ToDecimal(row["price"])
            };
        }

        public bool AddFlight(Flight flight)
        {
            Open();
            var result = ExecuteNonQuery(@"INSERT INTO Flights (origin, destination, departure_time, arrival_time, price) 
                                               VALUES (@origin, @destination, @departure_time, @arrival_time, @price)",
                new MySqlParameter("@origin", flight.Origin),
                new MySqlParameter("@destination", flight.Destination),
                new MySqlParameter("@departure_time", flight.DepartureTime),
                new MySqlParameter("@arrival_time", flight.ArrivalTime),
                new MySqlParameter("@price", flight.Price));
            Close();
            return result > 0;
        }

        public bool DeleteFlight(int flightId)
        {
            Open();
            var result = ExecuteNonQuery("DELETE FROM Flights WHERE flight_id = @flight_id",
                new MySqlParameter("@flight_id", flightId));
            Close();
            return result > 0;
        }
    }
}