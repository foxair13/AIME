using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Enums;
using NeftViewer.MVC.Filters;
using NeftViewer.MVC.FinanceModels;
using Org.BouncyCastle.Utilities.Collections;

namespace NeftViewer.MVC.Service
{
    public class TaskService : BackgroundService
    {
        private readonly IMapper _mapper;
        private readonly string _BasePostgree;
        private readonly string _FinanceMssql;
        private readonly IServiceScopeFactory _serviceScopeFactory;
       
        public TaskService(IMapper mapper, string FinanceMssql, string basePostgree, IServiceScopeFactory serviceScopeFactory)
        {
            _mapper = mapper;
            _BasePostgree = basePostgree;
            _serviceScopeFactory = serviceScopeFactory;
            _FinanceMssql=FinanceMssql;
        }
        private readonly TimeSpan dailyInterval = TimeSpan.FromSeconds(10);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
               
                

              
                GetTableService _getTableService = new GetTableService(_FinanceMssql);
                while (!stoppingToken.IsCancellationRequested)
                {
                    foreach (TableEnum table in Enum.GetValues(typeof(TableEnum)))
                    {
                        // Теперь у вас есть доступ к каждому элементу перечисления `Table` внутри этого цикла
                        switch (table)
                        {
                            case TableEnum.Criterias:
                                // Обработка для таблицы Criterias
                                {
                                    var criteriaService = scope.ServiceProvider.GetRequiredService<ICriteriaService>();
                                    List<Criteria> criteriaList = new List<Criteria>();
                                    var viewData = _getTableService.GetViewData("[SUID].["+ GetTableService.GetTableText(table) + "]");

                                    foreach (var row in viewData)
                                    {
                                        var criteria = _mapper.Map<Dictionary<string, object>, Criteria>(row);
                                        criteriaList.Add(criteria);
                                    }
                                    await criteriaService.AddCriteriaRange(criteriaList, _BasePostgree);
                                    break;
                                }
                            case TableEnum.Roads:
                                // Обработка для таблицы Roads
                                {
                                    var roadService = scope.ServiceProvider.GetRequiredService<IRoadService>();
                                    List<Road> roadsList = new List<Road>();
                                    var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");


                                    foreach (var row in viewData)
                                    {
                                        var road = _mapper.Map<Dictionary<string, object>, Road>(row);
                                        roadsList.Add(road);
                                    }
                                    await roadService.AddRoadRange(roadsList, _BasePostgree);
                                    break;
                                }
                            case TableEnum.Customers:
                                // Обработка для таблицы Customers
                                break;
                            case TableEnum.Objects:
                                // Обработка для таблицы Objects
                                break;
                            case TableEnum.ObjectOnRoad:
                                // Обработка для таблицы ObjectOnRoad
                                break;
                            case TableEnum.IndicatorValues:
                                // Обработка для таблицы IndicatorValues
                                break;
                            default:
                                // Обработка для других значений, если необходимо
                                break;
                        }
                    }

                    await Task.Delay(dailyInterval, stoppingToken);
                }
            }
        } 
        public List<Dictionary<string, object>> GetViewData(string viewName)
        {
            var result = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_FinanceMssql))
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
