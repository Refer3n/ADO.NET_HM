using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace ADO.NET_Hm1
{
    public class FruitVegetableDatabase : DatabaseManager
    {
        private string connectionString;

        public FruitVegetableDatabase()
        {
            connectionString = GetConnectionString();
        }

        public string GetAllData()
        {
            string query = "select * from VegetablesAndFruitsTable";
            return ExecuteQuery(query, "Name", "Type", "Color", "Calories");
        }

        public string GetAllNames()
        {
            string query = "select Name from VegetablesAndFruitsTable";
            return $"Names: \n{ExecuteQuerySingleColumn(query, "Name")}";
        }

        public string GetAllColors()
        {
            string query = "select distinct Color from VegetablesAndFruitsTable";
            return $"Colors: \n{ExecuteQuerySingleColumn(query, "Color")}";
        }

        public string GetMaxCalories()
        {
            string query = "select max(Calories) as MaxCalories from VegetablesAndFruitsTable";
            return $"Maximum calories: {ExecuteQuerySingleColumn(query, "MaxCalories")}";
        }

        public string GetMinCalories()
        {
            string query = "select min(Calories) as MinCalories from VegetablesAndFruitsTable";
            return $"Minimum calories: {ExecuteQuerySingleColumn(query, "MinCalories")}";
        }

        public string GetAvgCalories()
        {
            string query = "select avg(Calories) as AvgCalories from VegetablesAndFruitsTable";
            return $"Average calories: {ExecuteQuerySingleColumn(query, "AvgCalories")}";
        }

        public string GetVegetableCount()
        {
            string query = "select count(*) as VegetableCount from VegetablesAndFruitsTable where Type = 0";
            return $"Number of vegatables: {ExecuteQuerySingleColumn(query, "VegetableCount")}";
        }

        public string GetFruitCount()
        {
            string query = "select count(*) as FruitCount from VegetablesAndFruitsTable where Type = 1";
            return $"Number of fruits: {ExecuteQuerySingleColumn(query, "FruitCount")}";
        }

        public string GetItemCountBySelectetColor(string color)
        {
            string query = $"select count(*) as ItemCount from VegetablesAndFruitsTable where Color = '{color}'";
            return $"Number of items by color: {color}: {ExecuteQuerySingleColumn(query, "ItemCount")}";
        }

        public string GetItemCountByColor()
        {
            string query = "select Color, count(*) as ItemCount from VegetablesAndFruitsTable group by Color";
            return ExecuteQuery(query, "Color", "ItemCount");
        }

        public string GetItemsByLowerCalories(decimal maxCalories)
        {
            string query = $"select * from VegetablesAndFruitsTable where Calories <= {maxCalories}";
            return $"Items with calories lower than {maxCalories}: \n{ExecuteQuery(query, "Name", "Type", "Color", "Calories")}";
        }

        public string GetItemsByHigherCalories(decimal minCalories)
        {
            string query = $"select * from VegetablesAndFruitsTable where Calories >= {minCalories}";
            return $"Items with calories higher than {minCalories}: \n{ExecuteQuery(query, "Name", "Type", "Color", "Calories")}";
        }

        public string GetItemsByCalories(decimal minCalories, decimal maxCalories)
        {
            string query = $"select * from VegetablesAndFruitsTable where Calories >= {minCalories} and Calories <= {maxCalories}";
            return $"Items with calories in range between {minCalories} and {maxCalories}:" +
                $" \n{ExecuteQuery(query, "Name", "Type", "Color", "Calories")}";
        }

        public string GetItemsByColor(string color1, string color2)
        {
            string query = $"select * from VegetablesAndFruitsTable where Color = '{color1}' or Color = '{color2}'";
            return $"All {color1} or {color2} items: \n{ExecuteQuery(query, "Name", "Type", "Color", "Calories")}";
        }

        public bool CreateDatabase()
        {
            string databaseName = "VegetablesAndFruits";

            try
            {
                CreateDatabase(databaseName);
                connectionString = GetConnectionString();

                CultureInfo originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                CreateTable("VegetablesAndFruitsTable",
                           new TableColumn("Id", "int primary key identity"),
                           new TableColumn("Name", "nvarchar(255)"),
                           new TableColumn("Type", "bit"),
                           new TableColumn("Color", "nvarchar(50)"),
                           new TableColumn("Calories", "decimal(18, 2)"));
            }
            catch
            {

            }

            return true;
        }

        public void InsertTestData()
        {
            string[] names = { "Apple", "Banana", "Carrot", "Grapes", "Lettuce", "Orange", "Tomato", "Watermelon" };
            bool[] types = { true, true, false, true, false, true, false, true };
            string[] colors = { "Red", "Yellow", "Orange", "Purple", "Green", "Orange", "Red", "Green" };
            decimal[] calories = { 52.0m, 89.0m, 41.0m, 69.0m, 5.0m, 43.0m, 18.0m, 30.0m };

            CultureInfo originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                for (int i = 0; i < names.Length; i++)
                {
                    string query = $"insert into VegetablesAndFruitsTable (Name, Type, Color, Calories) " +
                                   $"values ('{names[i]}', {(types[i] ? 1 : 0)}, '{colors[i]}', {calories[i]})";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private string ExecuteQuery(string query, params string[] columnNames)
        {
            string result = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            foreach (string columnName in columnNames)
                            {
                                result += $"{columnName}: {reader[columnName]}, ";
                            }
                            result += Environment.NewLine;
                        }
                    }
                    else
                    {
                        result = "Nothing was found.";
                    }
                }

                connection.Close();
            }

            return ReplaceNumbersWithTypes(result);
        }

        private string ExecuteQuerySingleColumn(string query, string columnName)
        {
            string result = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result += reader[columnName] + Environment.NewLine;
                        }
                    }
                    else
                    {
                        result = "Nothing was found.";
                    }
                }
                connection.Close();
            }

            return ReplaceNumbersWithTypes(result);
        }

        private string ReplaceNumbersWithTypes(string text)
        {
            text = text.Replace("True", "Fruit").Replace("False", "Vegatable");
            return text;
        }

    }
}
