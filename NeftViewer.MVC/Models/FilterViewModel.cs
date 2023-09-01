using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.Models
{
    public class FilterViewModel
    {
        public List<RadioOption> TypeRadioOptions { get; set; }
      
        public List<DropDown> AreasDropDown { get; set; }
        public List<DropDown> OwnersDropDown { get; set; }
        public List<DropDown> ObjectsDropDown { get; set; }
        public List<DropDown> RoadsDropDown { get; set; }

        public List<CriteriaRadioModel> CriteriaRadioModels { get; set; }

        private readonly IAreaService _areaService;
        private readonly IOwnerService _ownerService;
        private readonly IObjectItemService _objectItemService;
        private readonly IRoadService _roadService;
        private readonly ICriteriaService _criteriaService;
        private readonly IAgregateService _agregateService;
        public FilterViewModel(IAreaService areaService, IOwnerService ownerService, IObjectItemService objectItemService, IRoadService roadService, ICriteriaService criteriaService, IAgregateService agregateService)
        {
            _ownerService = ownerService;
            _areaService = areaService;
            _objectItemService = objectItemService;
            _roadService = roadService;
            _criteriaService = criteriaService;
            _agregateService = agregateService;
        }
        private async Task<List<CriteriaRadioModel>> GetCriteriaOptions()
        {
            List<CriteriaRadioModel> crm = new List<CriteriaRadioModel>();
            var agregates = await _agregateService.GetAgregates();
            foreach (var item in agregates)
            {
                IEnumerable<Criteria> criterias = await _criteriaService.GetCriterias();
                    criterias = criterias.Where(x => x.AgregateId == item.Id);
                List<RadioOption> radioOptions = criterias.Select(criteria => new RadioOption { Id = criteria.Id.ToString(), Value = criteria.Name })
                                    .OrderBy(x => x.Value)
                                    .ToList();
                crm.Add(new CriteriaRadioModel { AgregateId=item.Id, AgregateName=item.Name, CriteriaRadioOptions = radioOptions });
                crm = crm.OrderBy(x => x.AgregateId).ToList();
            }
            return crm;
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
        public static async Task<FilterViewModel> CreateAsync(IAreaService areaService, IOwnerService ownerService, IObjectItemService objectItemService, IRoadService roadService,ICriteriaService criteriaService,IAgregateService agregateService)
        {

            var viewModel = new FilterViewModel(areaService, ownerService, objectItemService, roadService, criteriaService, agregateService);
            viewModel.AreasDropDown = await viewModel.GetAreaList();
            viewModel.OwnersDropDown = await viewModel.GetOwnerList();
            viewModel.RoadsDropDown = await viewModel.GeRoadList();
            viewModel.ObjectsDropDown = await viewModel.GetObjectList();
            viewModel.TypeRadioOptions = await viewModel.GetTypeOptions();
            viewModel.CriteriaRadioModels = await viewModel.GetCriteriaOptions();


            return viewModel;
        }


    }
}
