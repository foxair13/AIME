using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NeftViewer.BL.Services;
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
        private readonly TimeSpan dailyInterval = TimeSpan.FromSeconds(60);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                GetTableService _getTableService = new GetTableService(_FinanceMssql);
                while (!stoppingToken.IsCancellationRequested)
                {
                    foreach (TableEnum table in Enum.GetValues(typeof(TableEnum)))
                    {
                       
                        switch (table)
                        {
                            case TableEnum.Criterias:
                                {
                                    var criteriaService = scope.ServiceProvider.GetRequiredService<ICriteriaService>();
                                    List<Criteria> criteriaList = new List<Criteria>();
                                    var viewData = _getTableService.GetViewData("[SUID].["+ GetTableService.GetTableText(table) + "]");

                                    foreach (var row in viewData)
                                    {
                                        var criteria = _mapper.Map<Dictionary<string, object>, Criteria>(row);
                                        criteriaList.Add(criteria);
                                    }
                                    await criteriaService.AddCriteriaRange(criteriaList);
                                    break;
                                }

                            case TableEnum.Roads:
                                {
                                    var roadService = scope.ServiceProvider.GetRequiredService<IRoadService>();
                                    List<Road> roadsList = new List<Road>();
                                    var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");


                                    foreach (var row in viewData)
                                    {
                                        var road = _mapper.Map<Dictionary<string, object>, Road>(row);
                                        roadsList.Add(road);
                                    }
                                    await roadService.AddRoadRange(roadsList);
                                    break;
                                }

                            case TableEnum.Customers:
                                {
                                    var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
                                    List<Customer> customersList = new List<Customer>();
                                    var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

                                    var uniqItems = new List<string>();
                                    foreach (var row in viewData)
                                    {
                                        var codeSuidValue = row["Id"].ToString();
                                        if (!string.IsNullOrWhiteSpace(codeSuidValue) && !uniqItems.Contains(codeSuidValue))
                                        {
                                            uniqItems.Add(codeSuidValue);
                                            var customer = _mapper.Map<Dictionary<string, object>, Customer>(row);
                                            customersList.Add(customer);
                                        }
                                    }
                                    await customerService.AddCustomerRange(customersList);
                                    break;
                                }

                            case TableEnum.ObjectItems:
                                {
                                    var objectItemService = scope.ServiceProvider.GetRequiredService<IObjectItemService>();
                                    List<ObjectItem> objectItemsList = new List<ObjectItem>();
                                    var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

                                    var uniqItems = new List<string>();
                                    foreach (var row in viewData)
                                    {
                                        var codeSuidValue = row["CodeSUID"].ToString();
                                        if (!string.IsNullOrWhiteSpace(codeSuidValue) && !uniqItems.Contains(codeSuidValue))
                                        {
                                            uniqItems.Add(codeSuidValue);
                                            var objectItem = _mapper.Map<Dictionary<string, object>, ObjectItem>(row);
                                            objectItemsList.Add(objectItem);
                                        }
                                    }
                                    await objectItemService.AddObjectItemRange(objectItemsList);
                                    break;
                                }

                            case TableEnum.IndicatorValues:
                                {
                                    var indicatorValueService = scope.ServiceProvider.GetRequiredService<IIndicatorValueService>();
                                    List<IndicatorValue> indicatorValueList = new List<IndicatorValue>();
                                    var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");
                                    //var uniqItems = new List<string>();

                                    foreach (var row in viewData)
                                    {
                                        //var codeSuidValue = row["CodeSUID"].ToString();
                                        //if (!string.IsNullOrWhiteSpace(codeSuidValue) && !uniqItems.Contains(codeSuidValue))
                                        //{
                                        var indicatorValue = _mapper.Map<Dictionary<string, object>, IndicatorValue>(row);
                                        indicatorValueList.Add(indicatorValue);
                                        //}
                                    }
                                    await indicatorValueService.AddIndicatorValueRange(indicatorValueList);
                                    break;
                                }

                            case TableEnum.ObjectOnRoad:
                                {
                                    var objectOnRoadService = scope.ServiceProvider.GetRequiredService<IObjectOnRoadService>();
                                    List<ObjectOnRoad> objectOnRoadList = new List<ObjectOnRoad>();
                                    var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");
                                    var uniqItems = new List<string>();

                                    foreach (var row in viewData)
                                    {
                                        var roadIdValue = row["RoadId"].ToString();
                                        if (!string.IsNullOrWhiteSpace(roadIdValue) && !uniqItems.Contains(roadIdValue))
                                        {
                                            var objectOnRoad = _mapper.Map<Dictionary<string, object>, ObjectOnRoad>(row);
                                            uniqItems.Add(roadIdValue);
                                            objectOnRoadList.Add(objectOnRoad);
                                        }
                                    }
                                    await objectOnRoadService.AddObjectOnRoadRange(objectOnRoadList);
                                    break;
                                }

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
