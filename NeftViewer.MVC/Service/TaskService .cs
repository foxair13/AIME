using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Enums;
using Newtonsoft.Json.Linq;
using System.Net;

namespace NeftViewer.MVC.Service
{
    public class TaskService : BackgroundService
    {
        private readonly IMapper _mapper;
        private readonly string _BasePostgree;
        private readonly string _FinanceMssql;
        private readonly string _CoordsUrl;
        //private readonly string _CoordsUrl;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TaskService(IMapper mapper, string FinanceMssql, string basePostgree, IServiceScopeFactory serviceScopeFactory, string coordsUrl)
        {
            _mapper = mapper;
            _BasePostgree = basePostgree;
            _serviceScopeFactory = serviceScopeFactory;
            _FinanceMssql = FinanceMssql;
            _CoordsUrl = coordsUrl;
        }
        private readonly TimeSpan dailyInterval = TimeSpan.FromDays(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (1==1){
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    GetTableService _getTableService = new GetTableService(_FinanceMssql);

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (TableEnum table in Enum.GetValues(typeof(TableEnum)))
                        {
                            try
                            {
                                switch (table)
                                {

                                    case TableEnum.Criterias:
                                        {
                                            var criteriaService = scope.ServiceProvider.GetRequiredService<ICriteriaService>();
                                            List<Criteria> criteriaList = new List<Criteria>();
                                            var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

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
                                                    objectItem.OwnerId = 5;
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

                                            //foreach (var row in viewData)
                                            //{
                                            //    //var codeSuidValue = row["CodeSUID"].ToString();
                                            //    //if (!string.IsNullOrWhiteSpace(codeSuidValue) && !uniqItems.Contains(codeSuidValue))
                                            //    //{
                                            //    var indicatorValue = _mapper.Map<Dictionary<string, object>, IndicatorValue>(row);
                                            //    indicatorValueList.Add(indicatorValue);
                                            //    //}
                                            //}
                                            //await indicatorValueService.AddIndicatorValueRange(indicatorValueList);
                                            break;
                                        }

                                    case TableEnum.ObjectOnRoad:
                                        {
                                            var objectOnRoadService = scope.ServiceProvider.GetRequiredService<IObjectOnRoadService>();
                                            List<ObjectOnRoad> objectOnRoadList = new List<ObjectOnRoad>();
                                            var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

                                            var uniqUIDObject = new List<string>();

                                            foreach (var row in viewData)
                                            {
                                                var roadIdValue = row["RoadId"].ToString();
                                                if (!string.IsNullOrWhiteSpace(roadIdValue))
                                                {
                                                    var objectOnRoad = _mapper.Map<Dictionary<string, object>, ObjectOnRoad>(row);

                                                    objectOnRoadList.Add(objectOnRoad);
                                                }
                                            }
                                            await objectOnRoadService.AddObjectOnRoadRange(objectOnRoadList);
                                            break;
                                        }

                                    case TableEnum.JsonCoordinates:
                                        {
                                            var objectItemService = scope.ServiceProvider.GetRequiredService<IObjectItemService>();
                                            var objects = objectItemService.GetObjectItems();
                                            string url = _CoordsUrl;
                                            string json;
                                            using (var client = new WebClient())
                                            {
                                                json = client.DownloadString(url);
                                            }
                                            var jsonObject = JObject.Parse(json);
                                            JArray headers = (JArray)jsonObject["headers"];
                                            foreach (JToken header in headers)
                                            {
                                                var codeSUID = (string)header["suid"];
                                                var latitude = (double)header["coordinates"]["latitude"];
                                                var longitude = (double)header["coordinates"]["longitude"];
                                                var ownerName = (string)header["ownerName"];
                                                await objectItemService.UpdateObjectItemCoordinatesAsync(codeSUID, latitude, longitude, ownerName);
                                            }
                                            break;
                                        }

                                    case TableEnum.JsonOwner:
                                        {
                                            var areaService = scope.ServiceProvider.GetRequiredService<IOwnerService>();

                                            string url = _CoordsUrl;
                                            string json;
                                            using (var client = new WebClient())
                                            {
                                                json = client.DownloadString(url);
                                            }
                                            var jsonObject = JObject.Parse(json);
                                            JArray headers = (JArray)jsonObject["headers"];
                                            foreach (JToken header in headers)
                                            {
                                                string codeSUID = (string)header["suid"];

                                                string ownerName = (string)header["ownerName"];
                                                if (!ownerName.IsNullOrEmpty())
                                                {
                                                    await areaService.CreateOwner(codeSUID, ownerName);
                                                }
                                            }
                                            break;
                                        }

                                    case TableEnum.JsonArea:
                                        {
                                            var areaService = scope.ServiceProvider.GetRequiredService<IAreaService>();

                                            string url = _CoordsUrl;
                                            string json;
                                            using (var client = new WebClient())
                                            {
                                                json = client.DownloadString(url);
                                            }
                                            var jsonObject = JObject.Parse(json);
                                            JArray headers = (JArray)jsonObject["headers"];
                                            foreach (JToken header in headers)
                                            {
                                                string codeSUID = (string)header["suid"];
                                                string areaName = "";
                                                if (!codeSUID.IsNullOrEmpty())
                                                {
                                                    if (codeSUID.Contains("370_01"))
                                                    {
                                                        areaName = "Гомельская";
                                                    }
                                                    if (codeSUID.Contains("650_02") || codeSUID.Contains("600_03") || codeSUID.Contains("720_06"))
                                                    {
                                                        areaName = "Минская";
                                                    }
                                                    if (codeSUID.Contains("160_07"))
                                                    {
                                                        areaName = "Витебская";
                                                    }
                                                    if (codeSUID.Contains("800_08"))
                                                    {
                                                        areaName = "Могилевская";
                                                    }
                                                    if (codeSUID.Contains("020_10"))
                                                    {
                                                        areaName = "Брестская";
                                                    }
                                                    if (codeSUID.Contains("520_11") || codeSUID.Contains("530_12"))
                                                    {
                                                        areaName = "Гродненская";
                                                    }
                                                    if (!areaName.IsNullOrEmpty())
                                                    {
                                                        await areaService.CreateArea(codeSUID, areaName);
                                                    }
                                                }
                                            }
                                            break;
                                        }

                                    //case TableEnum.Energies:
                                    //    {
                                    //        var energyService = scope.ServiceProvider.GetRequiredService<IEnergyService>();
                                    //        var objects = energyService.GetEnergies();
                                    //        string url = _CoordsUrl;
                                    //        string json;
                                    //        using (var client = new WebClient())
                                    //        {
                                    //            json = client.DownloadString(url);
                                    //        }
                                    //        var jsonObject = JObject.Parse(json);
                                    //        JArray headers = (JArray)jsonObject["headers"];
                                    //        foreach (JToken header in headers)
                                    //        {
                                    //            var codeSUID = (string)header["suid"];
                                    //            var latitude = (double)header["coordinates"]["latitude"];
                                    //            var longitude = (double)header["coordinates"]["longitude"];
                                    //            var ownerName = (string)header["ownerName"];
                                    //            //await energyService.UpdateObjectItemCoordinatesAsync(codeSUID, latitude, longitude, ownerName);
                                    //        }
                                    //        break;
                                    //    }
                                    default:
                                        // Обработка для других значений, если необходимо
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {

                                throw;
                            }

                        }

                        await Task.Delay(dailyInterval, stoppingToken);
                    }
                }
            }
        }
        private JObject NewMethod()
        {
            string url = _CoordsUrl;
            string json;
            using (var client = new WebClient())
            {
                json = client.DownloadString(url);
            }
            var jsonObject = JObject.Parse(json);
            return jsonObject;
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
