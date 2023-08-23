using NeftViewer.BL.Services;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.Models
{
    public class FilterViewModel
    {
        public List<TypeRadioOption> TypeRadioOptions { get; set; }
        public List<DropDown> AreasDropDown { get; set; }
        public List<DropDown> OwnersDropDown { get; set; }
        private readonly IAreaService _areaService;
        private readonly IOwnerService _ownerService;
        public FilterViewModel(IAreaService areaService, IOwnerService ownerService)
        {
            _ownerService = ownerService;
            _areaService = areaService;
        }
        private List<TypeRadioOption> GetTypeOptions()
        {
            return new List<TypeRadioOption>
            {
                new TypeRadioOption { Id = "id1", Value = "СХН" },
                new TypeRadioOption { Id = "AZS", Value = "АЗС" },
                new TypeRadioOption { Id = "id3", Value = "ЭЗС" },
                new TypeRadioOption { Id = "id4", Value = "C/Х" }
            };
        }
        private async Task<List<DropDown>> GetAreaList()
        {
            IEnumerable<Area> areas = await _areaService.GetAreas();

            var dropDownOptions = areas.Select(area => new DropDown { Id = area.Id.ToString(), Value = area.Name })
                                       .OrderBy(x => x.Value)
                                       .ToList();

        
            dropDownOptions.Insert(0, new DropDown { Id = "0", Value = "ВЫБОР ОБЛАСТИ" });

            return dropDownOptions;
        }
        private async Task<List<DropDown>> GetOwnerList()
        {
            IEnumerable<Owner> owners = await _ownerService.GetOwners();

            var dropDownOptions = owners.Select(owner => new DropDown { Id = owner.Id.ToString(), Value = owner.Name })
                                       .OrderBy(x => x.Value)
                                       .ToList();

         
            dropDownOptions.Insert(0, new DropDown { Id = "0", Value = "ВЫБОР ВЛАДЕЛЬЦА" });

            return dropDownOptions;
        }
        public static async Task<FilterViewModel> CreateAsync(IAreaService areaService, IOwnerService ownerService)
        {
            var viewModel = new FilterViewModel(areaService, ownerService);
            viewModel.AreasDropDown = await viewModel.GetAreaList();
            viewModel.OwnersDropDown = await viewModel.GetOwnerList();
            viewModel.TypeRadioOptions = viewModel.GetTypeOptions();
            viewModel.TypeRadioOptions = viewModel.GetTypeOptions();
            return viewModel;
        }


    }
}
