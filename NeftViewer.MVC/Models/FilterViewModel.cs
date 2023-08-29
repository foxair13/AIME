using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.Models
{
    public class FilterViewModel
    {
        public List<RadioOption> TypeRadioOptions { get; set; }
        public List<RadioOption> CriteriaRadioOptions { get; set; }
        public List<DropDown> AreasDropDown { get; set; }
        public List<DropDown> OwnersDropDown { get; set; }
        public List<DropDown> ObjectsDropDown { get; set; }
        public List<DropDown> RoadsDropDown { get; set; }
        
        

        private readonly IAreaService _areaService;
        private readonly IOwnerService _ownerService;
        private readonly IObjectItemService _objectItemService;
        private readonly IRoadService _roadService;
        private readonly ICriteriaService _criteriaService;
        public FilterViewModel(IAreaService areaService, IOwnerService ownerService, IObjectItemService objectItemService, IRoadService roadService, ICriteriaService criteriaService)
        {
            _ownerService = ownerService;
            _areaService = areaService;
            _objectItemService = objectItemService;
            _roadService = roadService;
            _criteriaService = criteriaService;
        }
        private async Task<List<RadioOption>> GetCriteriaOptions()
        {
            IEnumerable<Criteria> criterias = await _criteriaService.GetCriterias();
            List<RadioOption> radioOptiosn = criterias.Select(criteria => new RadioOption { Id = criteria.Id.ToString(), Value = criteria.Name })
                                      .OrderBy(x => x.Value)
                                      .ToList();
            return radioOptiosn;

        }
        private async Task<List<RadioOption>> GetTypeOptions()
        {
            return new List<RadioOption>
            {
                new RadioOption { Id = "id1", Value = "СХН" },
                new RadioOption { Id = "AZS", Value = "АЗС" },
                new RadioOption { Id = "id3", Value = "ЭЗС" },
                new RadioOption { Id = "id4", Value = "C/Х" }
            };
        }
        private async Task<List<DropDown>> GetAreaList()
        { 
            IEnumerable<Area> areas = await _areaService.GetAreas();

            var dropDownOptions = areas.Select(area => new DropDown { Value = area.Id.ToString(), Text = area.Name })
                                       .OrderBy(x => x.Text)
                                       .ToList();


            dropDownOptions.Insert(0, new DropDown { Value = "0", Text = "ВЫБОР ОБЛАСТИ" });

            return dropDownOptions;
        }

        private async Task<List<DropDown>> GetObjectList()
        {
            IEnumerable<ObjectItem> objects = await _objectItemService.GetObjectItems();

            var dropDownOptions = objects.Select(obj => new DropDown { Value = obj.CodeSUID, Text = obj.Name })
                                       .OrderBy(x => x.Text)
                                       .ToList();


            dropDownOptions.Insert(0, new DropDown { Value = "0", Text = "ВЫБОР ОБЪЕКТА" });

            return dropDownOptions;
        }
        private async Task<List<DropDown>> GetOwnerList()
        {
            IEnumerable<Owner> owners = await _ownerService.GetOwners();

            var dropDownOptions = owners.Select(owner => new DropDown { Value = owner.Id.ToString(), Text = owner.Name })
                                       .OrderBy(x => x.Text)
                                       .ToList();


            dropDownOptions.Insert(0, new DropDown { Value = "0", Text = "ВЫБОР ПОН" });

            return dropDownOptions;
        }
        private async Task<List<DropDown>> GeRoadList()
        {
            IEnumerable<Road> roads = await _roadService.GetRoads();

            var dropDownOptions = roads.Select(road => new DropDown { Value = road.Id.ToString(), Text = road.Name })
                                       .OrderBy(x => x.Text)
                                       .ToList();


            dropDownOptions.Insert(0, new DropDown { Value = "0", Text = "ВЫБОР ДОРОГИ" });

            return dropDownOptions;
        }
        public static async Task<FilterViewModel> CreateAsync(IAreaService areaService, IOwnerService ownerService, IObjectItemService objectItemService, IRoadService roadService,ICriteriaService criteriaService)
        {

            var viewModel = new FilterViewModel(areaService, ownerService, objectItemService, roadService, criteriaService);
            viewModel.AreasDropDown = await viewModel.GetAreaList();
            viewModel.OwnersDropDown = await viewModel.GetOwnerList();
            viewModel.RoadsDropDown = await viewModel.GeRoadList();
            viewModel.ObjectsDropDown = await viewModel.GetObjectList();
            viewModel.TypeRadioOptions = await viewModel.GetTypeOptions();
            viewModel.CriteriaRadioOptions = await viewModel.GetCriteriaOptions();


            return viewModel;
        }


    }
}
