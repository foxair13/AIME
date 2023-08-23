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

        public HomeController(ILogger<HomeController> logger, IAspNetUsersService aspNetUsersService, IAreaService areaService, IOwnerService ownerService)
        {
            _logger = logger;
            _userService = aspNetUsersService;
            _areaService = areaService;
            _ownerService = ownerService;
        }

        public async Task<IActionResult> Index()
        {
            FilterViewModel filterViewModel = await FilterViewModel.CreateAsync(_areaService, _ownerService);


            return View(filterViewModel);
        }
        //[HttpGet("GetObjects")]
        public async Task<IActionResult> GetObjects(string ownerId, string areaId)
        {
            try
            {
                //// Предположим, что ваш сервис может получить объекты на основе ownerId и areaId
                //var objects = await _objectService.GetObjectsByOwnerAndAreaAsync(ownerId, areaId);

                //var result = objects.Select(obj => new DropDownOption
                //{
                //    Id = obj.Id.ToString(),
                //    Value = obj.Name // или другое соответствующее поле
                //}).ToList();

                return Json(null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}