using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Enums;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;

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
        private readonly IHttpClientFactory _httpClientFactory;

        public TaskService(IMapper mapper, string FinanceMssql, string basePostgree, IServiceScopeFactory serviceScopeFactory, string coordsUrl, IHttpClientFactory httpClientFactory)
        {
            _mapper = mapper;
            _BasePostgree = basePostgree;
            _serviceScopeFactory = serviceScopeFactory;
            _FinanceMssql = FinanceMssql;
            _CoordsUrl = coordsUrl;
            _httpClientFactory = httpClientFactory;
        }
        private readonly TimeSpan dailyInterval = TimeSpan.FromDays(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (1==2){
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
                                            var criteriasList = new List<Criteria>();
                                            var criteriaService = scope.ServiceProvider.GetRequiredService<ICriteriaService>();
                                            var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

                                            foreach (var row in viewData)
                                            {
                                                var criteria = _mapper.Map<Dictionary<string, object>, Criteria>(row);
                                                criteriasList.Add(criteria);
                                            }

                                            criteriasList = criteriasList.DistinctBy(c => c.Name).ToList();

                                            await criteriaService.UpdateCriteriaRange(criteriasList, "Name");
                                            await criteriaService.AddCriteriaRange(criteriasList, "Name");
                                            break;
                                        }

                                    //case TableEnum.Roads:
                                    //    {
                                    //        var roadsList = new List<Road>();
                                    //        var roadService = scope.ServiceProvider.GetRequiredService<IRoadService>();
                                    //        var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

                                    //        foreach (var row in viewData)
                                    //        {
                                    //            var road = _mapper.Map<Dictionary<string, object>, Road>(row);
                                    //            roadsList.Add(road);
                                    //        }

                                    //        roadsList = roadsList.DistinctBy(c => c.Indicator).ToList();

                                    //        await roadService.UpdateRoadRange(roadsList, "Indicator");
                                    //        await roadService.AddRoadRange(roadsList, "Indicator");
                                    //        break;
                                    //    }

                                    case TableEnum.ObjectItems:
                                        {
                                            var objectItemsList = new List<ObjectItem>();
                                            var objectItemService = scope.ServiceProvider.GetRequiredService<IObjectItemService>();
                                            var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");

                                            foreach (var row in viewData)
                                            {
                                                var objectItem = _mapper.Map<Dictionary<string, object>, ObjectItem>(row);
                                                objectItemsList.Add(objectItem);
                                            }

                                            objectItemsList = objectItemsList.DistinctBy(i => i.CodeSUID).ToList();
                                            await objectItemService.UpdateObjectItemRange(objectItemsList, "CodeSUID");
                                            await objectItemService.AddObjectItemRange(objectItemsList, "CodeSUID");
                                            break;
                                        }

                                    //case TableEnum.IndicatorValues:
                                    //    {
                                    //        var indicatorValuesList = new List<IndicatorValue>();
                                    //        var indicatorValueService = scope.ServiceProvider.GetRequiredService<IIndicatorValueService>();
                                    //        var viewData = _getTableService.GetViewDataFromProcedure("[SUID].[sp_indicatorValues]", "20230101", "20230131");

                                    //        foreach (var row in viewData)
                                    //        {
                                    //            var indicatorValue = _mapper.Map<Dictionary<string, object>, IndicatorValue>(row);
                                    //            indicatorValuesList.Add(indicatorValue);
                                    //        }

                                    //        indicatorValuesList = indicatorValuesList.DistinctBy(i => (i.CodeSUID, i.DateStart, i.CriteriaId)).ToList();
                                    //        await indicatorValueService.UpdateIndicatorValueRange(indicatorValuesList, "CodeSUID", "DateStart", "CriteriaId");
                                    //        await indicatorValueService.AddIndicatorValueRange(indicatorValuesList, "CodeSUID", "DateStart", "CriteriaId");
                                    //        break;
                                    //    }

                                    //case TableEnum.ObjectOnRoad:
                                    //    {
                                    //        var objectOnRoadService = scope.ServiceProvider.GetRequiredService<IObjectOnRoadService>();
                                    //        List<ObjectOnRoad> objectOnRoadList = new List<ObjectOnRoad>();
                                    //        var viewData = _getTableService.GetViewData("[SUID].[" + GetTableService.GetTableText(table) + "]");
                                    //        var uniqUIDObject = new List<string>();

                                    //        foreach (var row in viewData)
                                    //        {
                                    //            var roadIdValue = row["RoadId"].ToString();
                                    //            if (!string.IsNullOrWhiteSpace(roadIdValue))
                                    //            {
                                    //                var objectOnRoad = _mapper.Map<Dictionary<string, object>, ObjectOnRoad>(row);
                                    //                objectOnRoadList.Add(objectOnRoad);
                                    //            }
                                    //        }
                                    //        await objectOnRoadService.AddObjectOnRoadRange(objectOnRoadList);
                                    //        break;
                                    //    }

                                    case TableEnum.JsonTrks:
                                        {
                                            var trkService = scope.ServiceProvider.GetRequiredService<ITrkService>();
                                            var tankService = scope.ServiceProvider.GetRequiredService<ITankService>();
                                            var objectItemService = scope.ServiceProvider.GetRequiredService<IObjectItemService>();
                                            
                                            var objectItems = await objectItemService.GetObjectItemsAsync();
                                            var asuClient = _httpClientFactory.CreateClient("GetAsuService");

                                            var trks = new List<Trk>();
                                            var tanks = new List<Tank>();

                                            foreach(var objectItem in objectItems)
                                            {
                                                var objectCodeSUID = objectItem.CodeSUID;
                                                var response = await asuClient.GetAsync(objectCodeSUID);

                                                if (!response.IsSuccessStatusCode)
                                                {
                                                    continue;
                                                }
                                                var json = await response.Content.ReadAsStringAsync();
                                                var jsonObject = JObject.Parse(json);
                                                var results = jsonObject["data"]["result"];
                                                if (!results.HasValues)
                                                {
                                                    continue;
                                                }

                                                foreach (var result in results)
                                                {
                                                    var metric = result["metric"];
                                                    var codeSUID = (string)metric["Code_SUID"];
                                                    var oil = (string)metric["Oil"];

                                                    if (codeSUID.IsNullOrEmpty())
                                                    {
                                                        continue;
                                                    }

                                                    if (oil.IsNullOrEmpty())
                                                    {
                                                        var number = (string)metric["tank"];
                                                        if (number.IsNullOrEmpty())
                                                        {
                                                            continue;
                                                        }
                                                        var newTank = new Tank
                                                        {
                                                            CodeSUID = objectCodeSUID,
                                                            Number = number,
                                                        };
                                                        tanks.Add(newTank);
                                                    }

                                                    else
                                                    {
                                                        var number = (string)metric["TRK"];
                                                        
                                                        var newTrk = new Trk
                                                        {
                                                            CodeSUID = objectCodeSUID,
                                                            Oil = oil,
                                                            Number = number
                                                        };
                                                        trks.Add(newTrk);
                                                    }

                                                }
                                            }
                                            await tankService.AddTankRange(tanks);
                                            await trkService.AddTrkRange(trks);
                                            
                                            break;
                                        }

                                    case TableEnum.JsonCoordinates:
                                        {
                                            var objectItemService = scope.ServiceProvider.GetRequiredService<IObjectItemService>();
                                            var objects = objectItemService.GetObjectItemsAsync();
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
