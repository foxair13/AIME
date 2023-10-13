using Microsoft.AspNetCore.Mvc;
using NeftViewer.Api.ViewModels;
using NeftViewer.BL.Services.Contracts;

namespace NeftViewer.Api.Controllers
{
    [ApiController]

    [Route("[controller]")]
    public class MapController : ControllerBase
    {


        //private readonly ILogger<> _logger;
        private readonly IObjectItemService _objectItemService;
        public MapController(IObjectItemService objectItemService)
        {
            _objectItemService = objectItemService;
        }

        [HttpGet]
        public IEnumerable<PointViewModel> Get()
        {
            var objects =  _objectItemService.GetObjectItems();
            
            return  objects.Select(obj => new PointViewModel
            {
                id = obj.CodeSUID,
                Lon = obj.Longitude,
                Lat = obj.Latitude,
                Name = obj.Name
            }).ToList(); 
     
        }
    }
}