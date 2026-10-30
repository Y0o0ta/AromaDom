using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AromaDom
{
    public static class Service
    {
        private static string ConnectionString =>ConfigurationManager.ConnectionStrings["ConnectionDataBase"].ConnectionString;

        public static void InsertMaterial(int type_id,string name,decimal price,string unit,decimal pack_qty,decimal stock_qty,decimal min_qty)
        {
            string query = @"
                INSERT INTO Materials
                (type_id, name, price, unit, package_qty, stock_qty, min_qty)
                VALUES
                (@typeId, @name, @price, @unit, @packageQty, @stockQty, @minQty)";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", type_id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@unit", unit);
                command.Parameters.AddWithValue("@pack_qty", pack_qty);
                command.Parameters.AddWithValue("@stock_qty", stock_qty);
                command.Parameters.AddWithValue("@min_qty", min_qty);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }



        public static void UpdateMaterial(int id,int type_id, string name, decimal price, string unit, decimal pack_qty, decimal stock_qty, decimal min_qty)
        {
            string query = @"
                UPDATE Materials
                SET type_id = @typeId,
                    name = @name,
                    price = @price,
                    unit = @unit,
                    package_qty = @pack_qty,
                    stock_qty = @stock_qty,
                    min_qty = @min_qty
                WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@type_id", type_id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@unit", unit);
                command.Parameters.AddWithValue("@pack_qty", pack_qty);
                command.Parameters.AddWithValue("@stock_qty", stock_qty);
                command.Parameters.AddWithValue("@min_qty", min_qty);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }




        public static DataTable LoadMaterials()
        {
            DataTable table = new DataTable();
            string query = "SELECT * FROM Materials";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(table);
            }
            return table;
        }

        public static decimal CalculateMaterial(int materialId)
        {
            decimal requiredAmount = 0m;
            string query = @"
                SELECT ISNULL(SUM(material_unit_qty), 0)
                FROM Material_Products
                WHERE id = @material_Id";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", materialId);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    requiredAmount = Convert.ToDecimal(result);
                }
            }

            return requiredAmount;
        }



        internal static void InsertMaterial(object typeId, object name, decimal price, object unit, object packageQty, object stockQty, object minQty)
        {
            throw new NotImplementedException();
        }


        internal static void UpdateMaterial(object value, object typeId, object name, decimal price, object unit, object packageQty, object stockQty, object minQty)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateMaterial(int value, string name, decimal price, string unit, decimal pack_qty, decimal stock_qty, decimal min_qty)
        {
            throw new NotImplementedException();
        }



        internal static void InsertMaterial(string name, decimal price, string unit, decimal pack_qty, decimal stock_qty, decimal min_qty)
        {
            throw new NotImplementedException();
        }


        public static DataTable LoadProductsByMaterial(int materialId)
        {
            DataTable table = new DataTable();
            string query = @"
            SELECT p.name AS ProductName, mp.material_unit_qty AS MaterialQty
            FROM Material_Products mp
            JOIN Products p ON p.id = mp.product_id
            WHERE mp.id = @materialId";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@materialId", materialId);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(table);
            }

            return table;
        }
        public static int CalculateProductQuantity(int productTypeId,int materialTypeId, int rawQuantity, double param1, double param2)
        {
            if (rawQuantity <= 0 || param1 <= 0 || param2 <= 0)
            {
                return -1;
            }
            double productCoefficient;
            double materialLossPercent;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string productQuery = "SELECT coefficient FROM ProductTypes WHERE id = @id";
                using (SqlCommand command = new SqlCommand(productQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", productTypeId);
                    object result = command.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        return -1; 
                    }

                    productCoefficient = Convert.ToDouble(result);
                }


                string materialQuery = "SELECT loss_percent FROM MaterialTypes WHERE id = @id";
                using (SqlCommand command = new SqlCommand(materialQuery, connection))  //потеря сырья
                {
                    command.Parameters.AddWithValue("@id", materialTypeId);
                    object result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        return -1; 
                    }

                    materialLossPercent = Convert.ToDouble(result);
                }
            }


            double consumptionPerUnit = param1 * param2 * productCoefficient;
            if (consumptionPerUnit <= 0)  //расход на единицу
            {
                return -1; 
            }

            double effectiveRaw = rawQuantity * (1.0 - materialLossPercent / 100.0);
            if (effectiveRaw < 0)
            {
                effectiveRaw = 0;
            } //подсчет
            int productQuantity = (int)Math.Floor(effectiveRaw / consumptionPerUnit);
            return productQuantity;
        }

        internal static DataRow GetMaterialById(int materialId)
        {
            throw new NotImplementedException();
        }
    }
}