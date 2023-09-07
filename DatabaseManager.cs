using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ADO.NET_Hm1
{
    public abstract class DatabaseManager
    {
        protected static string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            return connectionString;
        }

        protected void CreateDatabase(string databaseName)
        {
            string masterConnectionString = "Data Source=DESKTOP-VI7HLAA\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;Connect Timeout=30";
            using (SqlConnection connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                string createDatabaseQuery = $"create database {databaseName}";
                using (SqlCommand command = new SqlCommand(createDatabaseQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void CreateTable(string tableName, params TableColumn[] columns)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = $"create table {tableName} (";

                for (int i = 0; i < columns.Length; i++)
                {
                    createTableQuery += columns[i].ToString();
                    if (i < columns.Length - 1)
                    {
                        createTableQuery += ", ";
                    }
                }

                createTableQuery += ")";

                using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class TableColumn
    {
        public string Name { get; }
        public string Type { get; }

        public TableColumn(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Name} {Type}";
        }
    }

}
