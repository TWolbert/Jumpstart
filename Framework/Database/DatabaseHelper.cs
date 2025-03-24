using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MySqlConnector;

namespace PapenChat.Framework.Database
{

    public static class DatabaseHelper
    {
        private static string _connectionString;

        static DatabaseHelper()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = config.GetConnectionString("MariaDb");
        }

        public static IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }

}