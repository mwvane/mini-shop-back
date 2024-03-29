﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using mini_shop_api.Models;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace mini_shop_api
{
    public static class DbHelpers
    {

        public static int? CRUD(string query, List<object> values, IConfiguration config)
        {
            try
            {
                SqlConnection connection = new SqlConnection(config.GetConnectionString("MyDbContext"));
                SqlCommand command = new SqlCommand(query, connection);
                int? productId = null;
                for (int i = 0; i < values.Count; i++)
                {
                    command.Parameters.AddWithValue($"@{i}", values[i]);
                }
                connection.Open();
                object returnObj = command.ExecuteScalar();
                if(returnObj != null )
                {
                    productId = Convert.ToInt32(returnObj);
                }

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return productId;
            }
            catch { return null; }
        }
        public static object?[] Select(string query, int roxIndex, IConfiguration config)
        {
            List<object?[]> list = new List<object?[]>();
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
                    list.Add(row.ItemArray);
                    //list.Add(row[roxIndex]);
                }
                connection.Close();
                return list[roxIndex];
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static List<object?[]> SelectMultiple(string query, IConfiguration config)
        {
            List<object?[]> list = new List<object?[]>();
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
                    list.Add(row.ItemArray);
                    //list.Add(row[roxIndex]);
                }
                connection.Close();
                return list;
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public static User GetUserById(int id, IConfiguration config)
        {
            object?[] user = DbHelpers.Select($"Select * from Users where id = {id}", 0, config);
            return new User()
            {
                Id = Convert.ToInt32(user[0]),
                Firstname = user[1] is string ? Convert.ToString(user[1]) : "",
                Lastname = user[2] is string ? Convert.ToString(user[2]) : "",
                Email = user[3] is string ? Convert.ToString(user[3]) : "",
                Role = user[6] is string ? Convert.ToString(user[6]) : "",
            };
        }
        public static Product GetItemById(int id, IConfiguration config)
        {
            object?[] item = DbHelpers.Select($"Select * from Products where id = {id}", 0, config);
            return new Product()
            {
                Id = Convert.ToInt32(item[0]),
                Name = item[1] is string ? Convert.ToString(item[1]) : "",
                Quantity = item[2] is int ? Convert.ToInt32(item[2]) : 0,
                Price = item[3] is decimal ? Convert.ToDecimal(item[3]) : 0,
                CreatedBy = item[4] is int ? Convert.ToInt32(item[4]) : 0,
            };
        }
    }
}
