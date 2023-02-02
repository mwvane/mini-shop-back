using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace mini_shop_api
{
    public static class DbHelpers
    {
       
        public static bool CRUD(string query, IConfiguration config)
        {
            try
            {
                SqlConnection connection = new SqlConnection(config.GetConnectionString("MyDbContext"));
                SqlCommand comand = new SqlCommand(query, connection);
                connection.Open();
                comand.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch { return false; }
        }
        public static List<int> Select(string query,int roxIndex, IConfiguration config)
        {
            List<int> list = new List<int>();
            try
            {
                SqlConnection connection = new SqlConnection(config.GetConnectionString("MyDbContext"));
                SqlCommand comand = new SqlCommand(query, connection);
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(comand);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine(table.Rows.Count);
                    list.Add(Convert.ToInt32(row[roxIndex]));
                }
                connection.Close();
                return list;
            }
            catch(Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
