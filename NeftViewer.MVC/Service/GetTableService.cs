using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.MVC.Enums;
using NeftViewer.MVC.Filters;
using System.Data;
using System.Reflection;

namespace NeftViewer.MVC.Service
{
    public class GetTableService
    {

        private readonly string _connectionString;
        //private DbContextOptions<NeftViewerContext> options;

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

        public List<Dictionary<string, object>> GetViewDataFromProcedure(string procedureName, object paramValue1, object paramValue2)
        {
            var result = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    var dateFromParameter = new SqlParameter("@date_from", paramValue1);
                    command.Parameters.Add(dateFromParameter);

                    var dateToParameter = new SqlParameter("@date_to", paramValue2);
                    command.Parameters.Add(dateToParameter);

                    var returnValueParameter = new SqlParameter("@return_value", SqlDbType.Int);
                    returnValueParameter.Direction = ParameterDirection.ReturnValue;
                    command.Parameters.Add(returnValueParameter);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
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
                    int returnValue = (int)command.Parameters["@return_value"].Value;
                }
            }
            return result;
        }

    }
}

