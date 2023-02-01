using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

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
                return true;
            }
            catch { return false; }
        }
    }
}
