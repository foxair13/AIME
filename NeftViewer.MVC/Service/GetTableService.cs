using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Enums;
using NeftViewer.MVC.Filters;
using NeftViewer.MVC.Mappings;
using Org.BouncyCastle.Utilities;
using System.Collections;
using System.Reflection;

namespace NeftViewer.MVC.Service
{
    public class GetTableService
    {
        private readonly string _connectionString;

        public GetTableService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public static string GetTableText(TableEnum table)
        {
            FieldInfo fieldInfo = table.GetType().GetField(table.ToString());
            if (fieldInfo != null)
            {
                TableTextAttribute attribute = fieldInfo.GetCustomAttribute<TableTextAttribute>();
                if (attribute != null)
                {
                    return attribute.Text;
                }
            }
            return table.ToString(); 
        }


        public List<Dictionary<string, object>> GetViewData(string viewName)
        {
            var result = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = $"SELECT * FROM {viewName}";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                String str = reader.GetName(i).ToString();
                                if (reader[i] is byte[] byteArray)
                                {
                                    string hexValue = BitConverter.ToString(byteArray).Replace("-", "");
                                    row[reader.GetName(i)] = hexValue;
                                }
                                else
                                {
                                    row[reader.GetName(i)] = reader[i];
                                }
                            }
                            result.Add(row);
                        }
                    }
                }
            }
            return result;
        }
    }
}

