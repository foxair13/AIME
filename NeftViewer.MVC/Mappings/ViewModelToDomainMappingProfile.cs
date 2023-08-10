using AutoMapper;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ActionViewModel,NeftViewer.Data.Models.Action>();
        }


    }
}
