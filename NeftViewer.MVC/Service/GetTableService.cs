using AutoMapper;
using Microsoft.Data.SqlClient;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.Service
{
    public class GetTableService
    {
        private readonly string _connectionString;

        public GetTableService(string connectionString)
        {
            _connectionString = connectionString;
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
                                row[reader.GetName(i)] = reader[i];
                            }
                            //var criteria = _mapper.Map<Dictionary<string, object>, Criterias>(row);
                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }
    }
}
