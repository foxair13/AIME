using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Core.ActionFilters;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;
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

        public HomeController(ILogger<HomeController> logger, IAspNetUsersService aspNetUsersService, IAreaService areaService, IOwnerService ownerService, IObjectItemService objectItemService, IRoadService roadService, ICriteriaService criteriaService)
        {
            _logger = logger;
            _userService = aspNetUsersService;
            _areaService = areaService;
            _ownerService = ownerService;
            _objectItemService = objectItemService;
            _roadService = roadService;
            _criteriaService = criteriaService;
        }

        public async Task<IActionResult> Index()
        {
            FilterViewModel filterViewModel = await FilterViewModel.CreateAsync(_areaService, _ownerService, _objectItemService, _roadService, _criteriaService);


            return View(filterViewModel);
        }
        public async Task<IActionResult> GetObjects(int ownerId, int areaId,string TypeValue, int roadId)
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
                    objects = from o in objects
                              join orCode in objectCodesOnRoad on o.CodeSUID equals orCode
                              select o;
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