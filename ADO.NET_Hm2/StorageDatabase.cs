using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace ADO.NET_Hm2
{
    public class StorageDatabase
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public string GetAllData()
        {
            string query = "select p.ProductName, pt.TypeName as ProductType, s.SupplierName, " +
                           "p.Cost, p.Quantity, p.SupplyDate " +
                           "from Products p " +
                           "join ProductTypes pt on p.ProductType_Id = pt.Id " +
                           "join Suppliers s on p.Supplier_Id = s.Id";
            return ExecuteQuery(query, "ProductName", "ProductType", "SupplierName", "Cost", "Quantity", "SupplyDate");

        }

        public string GetAllTypes()
        {
            string query = "select distinct TypeName from ProductTypes";
            return $"Product Types:\n{ExecuteQuerySingleColumn(query)}";
        }

        public string GetAllSuppliers()
        {
            string query = "select distinct SupplierName from Suppliers";
            return $"Suppliers:\n{ExecuteQuerySingleColumn(query)}";
        }

        public string GetMaxQuantityItem()
        {
            string query = "select top 1 ProductName, Quantity from Products order by Quantity desc";
            return $"Item with maximum quantity: {ExecuteQuery(query, "ProductName", "Quantity")}";
        }

        public string GetMinQuantityItem()
        {
            string query = "select top 1 ProductName, Quantity from Products order by Quantity asc";
            return $"Item with minimum quantity: {ExecuteQuery(query, "ProductName", "Quantity")}";
        }

        public string GetMinCostItem()
        {
            string query = "select top 1 ProductName, Cost from Products order by Cost asc";
            return $"Item with minimum cost: {ExecuteQuery(query, "ProductName", "Cost")}";
        }

        public string GetMaxCostItem()
        {
            string query = "select top 1 ProductName, Cost from Products order by Cost desc";
            return $"Item with minimum cost: {ExecuteQuery(query, "ProductName", "Cost")}";
        }

        public string GetItemsByCategory(string category)
        {
            string query = $"select p.ProductName " +
                           $"from Products p " +
                           $"inner join ProductTypes pt on p.ProductType_Id = pt.Id " +
                           $"where pt.TypeName = '{category}'";
            return $"Items in category '{category}':\n{ExecuteQuerySingleColumn(query)}";
        }

        public string GetItemsBySupplier(string supplier)
        {
            string query = $"select p.ProductName " +
                           $"from Products p " +
                           $"inner join Suppliers s on p.Supplier_Id = s.Id " +
                           $"where s.SupplierName = '{supplier}'";
            return $"Items from supplier '{supplier}':\n{ExecuteQuerySingleColumn(query)}";
        }

        public string GetLongestInStorageItem()
        {
            string query = "select top 1 p.ProductName " +
                           "from Products p " +
                           "join ProductsStorage ps on p.Id = ps.Product_Id " +
                           "order by p.SupplyDate asc";
            return $"Longest storage item: {ExecuteQuery(query, "ProductName")}";
        }

        public string GetAverageQuantityByType()
        {
            string query = "select pt.TypeName, avg(p.Quantity) as AvgQuantity " +
                           "from Products p " +
                           "join ProductTypes pt on p.ProductType_Id = pt.Id " +
                           "group by pt.TypeName";
            return ExecuteQuery(query, "TypeName", "AvgQuantity");
        }

        public string AddProduct(string productName, string productType, string supplier, decimal cost, int quantity, DateTime supplyDate)
        {
            try
            {
                string insertProductQuery = $"insert into Products (ProductName, ProductType_Id, Supplier_Id, Cost, Quantity, SupplyDate) " +
                                            $"values ('{productName}', " +
                                            $"(select Id from ProductTypes where TypeName = '{productType}'), " +
                                            $"(select Id from Suppliers where SupplierName = '{supplier}'), " +
                                            $"{cost.ToString(CultureInfo.InvariantCulture)}, {quantity}, '{supplyDate:yyyy-MM-dd}')";

                return ExecuteOperation(insertProductQuery);
            }
            catch (Exception ex)
            {
                return $"Error adding product: {ex.Message}";
            }
        }

        public string UpdateProduct(string productName, string newProductName, string productType, string supplier, decimal cost, int quantity, DateTime supplyDate)
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

                return ExecuteOperation(updateProductQuery);
            }
            catch (Exception ex)
            {
                return $"Error updating product: {ex.Message}";
            }
        }

        public string DeleteProduct(string productName)
        {
            try
            {
                string deleteProductQuery = $"delete from Products where ProductName = '{productName}'";

                return ExecuteOperation(deleteProductQuery);
            }
            catch (Exception ex)
            {
                return $"Error deleting product: {ex.Message}";
            }
        }

        public string AddProductType(string productType)
        {
            try
            {
                string insertProductTypeQuery = $"insert into ProductTypes (TypeName) values ('{productType}')";

                return ExecuteOperation(insertProductTypeQuery);
            }
            catch (Exception ex)
            {
                return $"Error adding product type: {ex.Message}";
            }
        }

        public string UpdateProductType(string productType, string newProductType)
        {
            try
            {
                string updateProductTypeQuery = $"update ProductTypes set TypeName = '{newProductType}' where TypeName = '{productType}'";

                return ExecuteOperation(updateProductTypeQuery);
            }
            catch (Exception ex)
            {
                return $"Error updating product type: {ex.Message}";
            }
        }

        public string DeleteProductType(string productType)
        {
            try
            {
                string deleteProductTypeQuery = $"delete from ProductTypes where TypeName = '{productType}'";

                return ExecuteOperation(deleteProductTypeQuery);
            }
            catch (Exception ex)
            {
                return $"Error deleting product type: {ex.Message}";
            }
        }

        public string AddSupplier(string supplier)
        {
            try
            {
                string insertSupplierQuery = $"insert into Suppliers (SupplierName) values ('{supplier}')";

                return ExecuteOperation(insertSupplierQuery);
            }
            catch (Exception ex)
            {
                return $"Error adding supplier: {ex.Message}";
            }
        }

        public string UpdateSupplier(string supplier, string newSupplier)
        {
            try
            {
                string updateSupplierQuery = $"update Suppliers set SupplierName = '{newSupplier}' where SupplierName = '{supplier}'";

                return ExecuteOperation(updateSupplierQuery);
            }
            catch (Exception ex)
            {
                return $"Error updating supplier: {ex.Message}";
            }
        }

        public string DeleteSupplier(string supplier)
        {
            try
            {
                string deleteSupplierQuery = $"delete from Suppliers where SupplierName = '{supplier}'";

                return ExecuteOperation(deleteSupplierQuery);
            }
            catch (Exception ex)
            {
                return $"Error deleting supplier: {ex.Message}";
            }
        }

        public string ExecuteOperation(string sqlQuery)
        {
            try
            {
                int rowsAffected = ExecuteNonQuery(sqlQuery);

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

        private int ExecuteNonQuery(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected;
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

            return result;
        }

        private string ExecuteQuerySingleColumn(string query)
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
                            result += reader[0] + Environment.NewLine;
                        }
                    }
                    else
                    {
                        result = "Nothing was found.";
                    }
                }
                connection.Close();
            }

            return result;
        }
    }
}
