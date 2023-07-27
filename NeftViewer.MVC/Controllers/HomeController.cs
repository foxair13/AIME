using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Models;
using System.Diagnostics;

namespace NeftViewer.MVC.Controllers
{

    [Authorize]
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAspNetUsersService _userService;
        public HomeController(ILogger<HomeController> logger, IAspNetUsersService aspNetUsersService)
        {
            _logger = logger;
            _userService = aspNetUsersService;
        }

        public IActionResult Index()
        {
            //AspNetUsers user = new AspNetUsers();
            //user.="Fox";
            //user.Age = 32;
            //_userService.AddUser(user);
            //_userService.CommitChangesAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}