using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace SkyTrack
{
    internal class SqlQuery
    {

        private string? connectionString;
        private MySqlConnection? connection;

        public string ConnectionString 
        {
            get { return connectionString!; }
            set { connectionString = value; }
        }
        
        public SqlQuery()
        {
        }

       /// <summary>
       /// Automatically connects to DB
       /// </summary>
       /// <param name="connectionString"></param>
        public SqlQuery(string connectionString)
        {
            this.connectionString = connectionString;

        }

        public string GetUser(string username)
        {
            return $"SELECT * FROM Users WHERE username = '{username}'";
        }
        public string GetAllUsers()
        {
            return "SELECT * FROM Users";
        }
        public string AddUser(string username, string password)
        {
            return $"INSERT INTO Users (username, password) VALUES ('{username}', '{password}')";
        }
        public string UpdateUser(string username, string password)
        {
            return $"UPDATE Users SET password = '{password}' WHERE username = '{username}'";
        }
        public string DeleteUser(string username)
        {
            return $"DELETE FROM Users WHERE username = '{username}'";
        }
    }
}
