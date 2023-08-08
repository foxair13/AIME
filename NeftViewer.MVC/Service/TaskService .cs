using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.MVC.FinanceModels;
using Org.BouncyCastle.Utilities.Collections;

namespace NeftViewer.MVC.Service
{
    public class TaskService : BackgroundService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public TaskService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }
        private readonly TimeSpan dailyInterval = TimeSpan.FromSeconds(10);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GetTableService _getTableService = new GetTableService(_connectionString);
            while (!stoppingToken.IsCancellationRequested)
            {
               
                var viewData = _getTableService.GetViewData("[SUID].[Criterias]");
                List<Criterias> criteriaList = new List<Criterias>();

                foreach (var row in viewData)
                {
                    var criteria = _mapper.Map<Dictionary<string, object>, Criterias>(row);
                    criteriaList.Add(criteria);
                }

                await Task.Delay(dailyInterval, stoppingToken);
            }
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
                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }
    }

}
