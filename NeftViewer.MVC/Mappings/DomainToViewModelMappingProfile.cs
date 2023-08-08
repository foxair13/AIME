using AutoMapper;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Dictionary<string, object>, Criterias>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["Id"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]));


        }

    }
}
