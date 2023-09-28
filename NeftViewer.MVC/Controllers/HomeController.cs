using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Core.ActionFilters;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;
using NeftViewer.MVC.Binders;
using NeftViewer.MVC.Models;
using NeftViewer.MVC.Options;
using System.Diagnostics;

namespace NeftViewer.MVC.Controllers
{

    [CustomAuthorize("HomeIndex")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAspNetUsersService _userService;
        private readonly IAreaService _areaService;
        private readonly IOwnerService _ownerService;
        private readonly IObjectItemService _objectItemService;
        private readonly IRoadService _roadService;
        private readonly ICriteriaService _criteriaService;
        private readonly IAgregateService _agregateService;
        private readonly ICriteriaCalcMethodService _criteriaCalcMethodService;
        private readonly IIndicatorValueService _indicatorValueService;

        public HomeController(ILogger<HomeController> logger, IAspNetUsersService aspNetUsersService, IAreaService areaService, IOwnerService ownerService, IObjectItemService objectItemService, IRoadService roadService, ICriteriaService criteriaService, IAgregateService agregateService, ICriteriaCalcMethodService criteriaCalcMethodService, IIndicatorValueService indicatorValueService)
        {
            _logger = logger;
            _userService = aspNetUsersService;
            _areaService = areaService;
            _ownerService = ownerService;
            _objectItemService = objectItemService;
            _roadService = roadService;
            _criteriaService = criteriaService;
            _agregateService = agregateService;
            _criteriaCalcMethodService = criteriaCalcMethodService;
            _indicatorValueService = indicatorValueService;
        }

        public async Task<IActionResult> Index()
        {
            FilterViewModel filterViewModel = await FilterViewModel.CreateAsync(_areaService, _ownerService, _objectItemService, _roadService, _criteriaService, _agregateService, _criteriaCalcMethodService);


            return View(filterViewModel);
        }
        public IActionResult GetObjectParams(string TabId, string ObjectId, [ModelBinder(typeof(RussianDateBinder))] DateTime Date)
        {
			IEnumerable<IndicatorValue> iv = _indicatorValueService.GetIndicatorByObject(Date, ObjectId).Result;
            iv = iv.OrderBy(x => x.Criterias.Name);

            if (TabId == "#ObjectParams")
            {
                return PartialView("_ObjectParamsPartialView", iv);
            }
            else if (TabId == "#Dashboards")
            {
                return PartialView("_DashboardsPartialView", null);
            }
            return PartialView("_ObjectParamsPartialView", iv);
        }
        public async Task<IActionResult> GetExtremumCriteria(int CriteriId)
        {
            var (min, max,datemin,datemax,dates) = await _indicatorValueService.GetMinMaxValues(CriteriId);

            return Json(new { Min = min, Max = max, DateMin= datemin, DateMax= datemax, Dates= dates });
        }

        public async Task<IActionResult> GetPointsForRegion(int ownerId, int areaId, string TypeValue, int roadId, string objectId,bool IsBest,int EntriesID, [ModelBinder(typeof(RussianDateBinder))] DateTime Dates, decimal slideMin, decimal slideMax,int CriteriaValue)
        {
            try
            {
                var result = new object();
                var items = new object();
                var objects = await _objectItemService.GetObjectItems();
                if (ownerId != 0)
                {
                    objects = objects.Where(x => x.OwnerId == ownerId);
                }
                if (areaId != 0)
                {
                    objects = objects.Where(x => x.AreaId == areaId);
                }
                if (TypeValue != null)
                {
                    objects = objects.Where(x => x.CodeSUID.Contains(TypeValue));
                }
                if (objectId != "0")
                {
                    objects = objects.Where(x => x.CodeSUID == objectId);
                }
                //if (roadId != 0)
                //{
                //    var objectCodesOnRoad = await _roadService.GetCodeSUIDByRoadIdAsync(roadId);
                //    var checkobjects = objects.Where(o => objectCodesOnRoad.Contains(o.CodeSUID));
                //    objects = checkobjects;
                //}
                if (CriteriaValue == 0)
                {
                    result = objects.Select(obj => new Point
                    {
                        id = obj.CodeSUID,
                        Lon = obj.Longitude,
                        Lat = obj.Latitude,
                        Name = obj.Name,
                        HasValue=false,
                        Value=0,
                        scaleUnit=""
                    }).ToList();
                }
                else 
                {
                    IEnumerable<CriteriaCalcMethod> criteriaCalc = await _criteriaCalcMethodService.GetCriteriaCalcMethods();
                    bool CalculationByMax= criteriaCalc.Where(x=>x.CriteriaId==CriteriaValue).Select(x=>x.СalculationByMax).FirstOrDefault();
                    IEnumerable<IndicatorValue> iv = _indicatorValueService.GetIndicatorByCriteria(Dates, CriteriaValue).Result;
                    string  Units = _criteriaService.FindCriteriaAsync(CriteriaValue).Result.Units;
                    iv =iv.Where(x=>x.Value >= slideMin && x.Value <= slideMax).Distinct().OrderBy(x => x.Value);

                    var objwithval = from x in objects
                              join y in iv on x.CodeSUID equals y.CodeSUID
                              select new { x,y.Value};

                    if (IsBest)
                    {
                        if (CalculationByMax)
                        {
                            objwithval = objwithval.OrderByDescending(x => x.Value).Take(EntriesID);
                        }
                        else 
                        {
                            objwithval = objwithval.OrderBy(x => x.Value).Take(EntriesID);
                        }
                        
                    }
                    else 
                    {
                        if (CalculationByMax)
                        {
                            objwithval = objwithval.OrderBy(x => x.Value).Take(EntriesID);
                        }
                        else
                        {
                            objwithval = objwithval.OrderByDescending(x => x.Value).Take(EntriesID);
                        }
                    }
                    result = objwithval.Select(obj => new Point
                    {
                        id = obj.x.CodeSUID,
                        Lon = obj.x.Longitude,
                        Lat = obj.x.Latitude,
                        Name = obj.x.Name,
                        HasValue = true,
                        Value = obj.Value,
                        scaleUnit = Units
                    }).ToList();
                }
              
                return Json(result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IActionResult> GetObjects(int ownerId, int areaId, string TypeValue, int roadId)
        {
            try
            {
                var objects = await _objectItemService.GetObjectItems();
                if (ownerId != 0)
                {
                    objects = objects.Where(x => x.OwnerId == ownerId);
                }
                if (areaId != 0)
                {
                    objects = objects.Where(x => x.AreaId == areaId);
                }
                if (TypeValue != null)
                {
                    objects = objects.Where(x => x.CodeSUID.Contains(TypeValue));
                }
                if (roadId != 0)
                {
                    var objectCodesOnRoad = await _roadService.GetCodeSUIDByRoadIdAsync(roadId);
                    var checkobjects = objects.Where(o => objectCodesOnRoad.Contains(o.CodeSUID));
                    objects = checkobjects;
                }
                var result = objects.Select(obj => new DropDown
                {
                    Value = obj.CodeSUID,
                    Text = obj.Name
                }).ToList();
                result.Insert(0, new DropDown { Value = "0", Text = "ВЫБОР ОБЪЕКТА" });
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        public async Task<IActionResult> GetDetailsForObject(string objectId)
        {
            var objectCodesOnRoad = await _roadService.GetParamsBySUIDAsync(objectId);
            return Json(objectCodesOnRoad);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}