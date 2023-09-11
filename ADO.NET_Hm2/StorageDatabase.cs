using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace ADO.NET_Hm2
{
    public class StorageDatabase
    {
        private static DbProviderFactory factory = null;
        private string connectionString = ConfigurationManager.ConnectionStrings["Default"].ToString();

        public StorageDatabase()
        {
            DbProviderFactories.RegisterFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName,
                SqlClientFactory.Instance);

            factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["Default"].ProviderName);
        }

        public async Task<string> GetAllDataAsync()
        {
            string query = "select p.ProductName, pt.TypeName as ProductType, s.SupplierName, " +
                           "p.Cost, p.Quantity, p.SupplyDate " +
                           "from Products p " +
                           "join ProductTypes pt on p.ProductType_Id = pt.Id " +
                           "join Suppliers s on p.Supplier_Id = s.Id";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "ProductName", 
                "ProductType", "SupplierName", "Cost", "Quantity", "SupplyDate");

            stopwatch.Stop();

            return $"{result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetAllTypesAsync()
        {
            string query = "select distinct TypeName from ProductTypes";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQuerySingleColumnAsunc(query);
            stopwatch.Stop();

            return $"Product Types:\n{result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetAllSuppliersAsync()
        {
            string query = "select distinct SupplierName from Suppliers";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQuerySingleColumnAsunc(query);
            stopwatch.Stop();

            return $"Suppliers:\n{result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetMaxQuantityItemAsync()
        {
            string query = "select top 1 ProductName, Quantity from Products order by Quantity desc";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "ProductName", "Quantity");
            stopwatch.Stop();

            return $"Item with maximum quantity: {result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetMinQuantityItemAsync()
        {
            string query = "select top 1 ProductName, Quantity from Products order by Quantity asc";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "ProductName", "Quantity");
            stopwatch.Stop();

            return $"Item with minimum quantity: {result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetMinCostItemAsync()
        {
            string query = "select top 1 ProductName, Cost from Products order by Cost asc";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "ProductName", "Cost");
            stopwatch.Stop();

            return $"Item with minimum cost: {result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetMaxCostItemAsync()
        {
            string query = "select top 1 ProductName, Cost from Products order by Cost desc";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "ProductName", "Cost");
            stopwatch.Stop();

            return $"Item with maximum cost: {result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetItemsByCategoryAsync(string category)
        {
            string query = $"select p.ProductName " +
                           $"from Products p " +
                           $"inner join ProductTypes pt on p.ProductType_Id = pt.Id " +
                           $"where pt.TypeName = '{category}'";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQuerySingleColumnAsunc(query);
            stopwatch.Stop();

            return $"Items in category '{category}':\n{result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetItemsBySupplierAsync(string supplier)
        {
            string query = $"select p.ProductName " +
                           $"from Products p " +
                           $"inner join Suppliers s on p.Supplier_Id = s.Id " +
                           $"where s.SupplierName = '{supplier}'";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQuerySingleColumnAsunc(query);
            stopwatch.Stop();

            return $"Items from supplier '{supplier}':\n{result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetLongestInStorageItemAsync()
        {
            string query = "select top 1 p.ProductName " +
                           "from Products p " + 
                           "order by p.SupplyDate asc";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "ProductName");
            stopwatch.Stop();

            return $"Longest storage item: {result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> GetAverageQuantityByTypeAsync()
        {
            string query = "select pt.TypeName, avg(p.Quantity) as AvgQuantity " +
                           "from Products p " +
                           "join ProductTypes pt on p.ProductType_Id = pt.Id " +
                           "group by pt.TypeName";

            Stopwatch stopwatch = Stopwatch.StartNew();
            string result = await ExecuteQueryAsync(query, "TypeName", "AvgQuantity");
            stopwatch.Stop();

            return $"{result}Time elapsed: {stopwatch.ElapsedMilliseconds} ms";
        }

        public async Task<string> AddProductAsync(string productName, string productType, string supplier, decimal cost, int quantity, DateTime supplyDate)
        {
            try
            {
                string insertProductQuery = $"insert into Products (ProductName, ProductType_Id, Supplier_Id, Cost, Quantity, SupplyDate) " +
                                            $"values ('{productName}', " +
                                            $"(select Id from ProductTypes where TypeName = '{productType}'), " +
                                            $"(select Id from Suppliers where SupplierName = '{supplier}'), " +
                                            $"{cost.ToString(CultureInfo.InvariantCulture)}, {quantity}, '{supplyDate:yyyy-MM-dd}')";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(insertProductQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error adding product: {ex.Message}";
            }
        }

        public async Task<string> UpdateProductAsync(string productName, string newProductName, string productType, string supplier, decimal cost, int quantity, DateTime supplyDate)
        {
            try
            {
                string updateProductQuery = $"update Products " +
                                            $"set ProductName = '{newProductName}', " +
                                            $"ProductType_Id = (select Id from ProductTypes where TypeName = '{productType}'), " +
                                            $"Supplier_Id = (select Id from Suppliers where SupplierName = '{supplier}'), " +
                                            $"Cost = {cost.ToString(CultureInfo.InvariantCulture)}, " +
                                            $"Quantity = {quantity}, " +
                                            $"SupplyDate = '{supplyDate:yyyy-MM-dd}' " +
                                            $"where ProductName = '{productName}'";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(updateProductQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error updating product: {ex.Message}";
            }
        }

        public async Task<string> DeleteProductAsync(string productName)
        {
            try
            {
                string deleteProductQuery = $"delete from Products where ProductName = '{productName}'";


                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(deleteProductQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error deleting product: {ex.Message}";
            }
        }

        public async Task<string> AddProductTypeAsync(string productType)
        {
            try
            {
                string insertProductTypeQuery = $"insert into ProductTypes (TypeName) values ('{productType}')";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(insertProductTypeQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error adding product type: {ex.Message}";
            }
        }

        public async Task<string> UpdateProductTypeAsync(string productType, string newProductType)
        {
            try
            {
                string updateProductTypeQuery = $"update ProductTypes set TypeName = '{newProductType}' where TypeName = '{productType}'";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(updateProductTypeQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error updating product type: {ex.Message}";
            }
        }

        public async Task<string> DeleteProductTypeAsync(string productType)
        {
            try
            {
                string deleteProductTypeQuery = $"delete from ProductTypes where TypeName = '{productType}'";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(deleteProductTypeQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error deleting product type: {ex.Message}";
            }
        }

        public async Task<string> AddSupplierAsync(string supplier)
        {
            try
            {
                string insertSupplierQuery = $"insert into Suppliers (SupplierName) values ('{supplier}')";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(insertSupplierQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error adding supplier: {ex.Message}";
            }
        }

        public async Task<string> UpdateSupplierAsync(string supplier, string newSupplier)
        {
            try
            {
                string updateSupplierQuery = $"update Suppliers set SupplierName = '{newSupplier}' where SupplierName = '{supplier}'";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(updateSupplierQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error updating supplier: {ex.Message}";
            }
        }

        public async Task<string> DeleteSupplierAsync(string supplier)
        {
            try
            {
                string deleteSupplierQuery = $"delete from Suppliers where SupplierName = '{supplier}'";

                Stopwatch stopwatch = Stopwatch.StartNew();
                string result = await ExecuteOperationAsync(deleteSupplierQuery);
                stopwatch.Stop();

                return result;
            }
            catch (Exception ex)
            {
                return $"Error deleting supplier: {ex.Message}";
            }
        }

        public async Task<string> ExecuteOperationAsync(string sqlQuery)
        {
            try
            {
                int rowsAffected = await ExecuteNonQueryAsync(sqlQuery);

                if (rowsAffected > 0)
                {
                    return "Operation completed successfully.";
                }
                else
                {
                    return "Operation failed.";
                }
            }
            catch (Exception ex)
            {
                return $"Error executing operation: {ex.Message}";
            }
        }

        private async Task<int> ExecuteNonQueryAsync(string query)
        {
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected;
                }
            }
        }

        private async Task<string> ExecuteQueryAsync(string query, params string[] columnNames)
        {
            string result = "";

            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;

                    command.CommandText = query;

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
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

                }
            }

            return result;
        }

        private async Task<string> ExecuteQuerySingleColumnAsunc(string query)
        {
            string result = "";

            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;

                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;

                    command.CommandText = query;

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                result += reader[0] + Environment.NewLine;
                            }
                        }
                        else
                        {
                            result = "Nothing was found.";
                        }
                    }
                }
            }

            return result;
        }
    }
}
