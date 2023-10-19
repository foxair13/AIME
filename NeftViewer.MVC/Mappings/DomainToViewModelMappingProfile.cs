using AutoMapper;
using NeftViewer.Data.Models;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Dictionary<string, object>, Criteria>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["Id"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]))
                .ForMember(dest => dest.AgregateId, opt => opt.MapFrom(src => src["AgregateId"]))
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src["Units"]))
                .ForMember(dest => dest.Periodicity, opt => opt.MapFrom(src => src["Periodicity"]));

            CreateMap<Dictionary<string, object>, Road>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["Id"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]))
                .ForMember(dest => dest.Indicator, opt => opt.MapFrom(src => src["Indicator"]));
            CreateMap<NeftViewer.Data.Models.Action, ActionViewModel>();

            CreateMap<Dictionary<string, object>, ObjectItem>()
                .ForMember(dest => dest.CodeSUID, opt => opt.MapFrom(src => src["CodeSUID"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]));

            CreateMap<Dictionary<string, object>, IndicatorValue>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["Id"]))
                .ForMember(dest => dest.CodeSUID, opt => opt.MapFrom(src => src["CodeSUID"]))
                .ForMember(dest => dest.DateStart, opt => opt.MapFrom(src => src["DateStart"]))
                .ForMember(dest => dest.CriteriaId, opt => opt.MapFrom(src => src["CriteriaId"]))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src["Value"]));

            CreateMap<Dictionary<string, object>, ObjectOnRoad>()
                .ForMember(dest => dest.RoadId, opt => opt.MapFrom(src => src["RoadId"]))
                .ForMember(dest => dest.CodeSUID, opt => opt.MapFrom(src => src["CodeSUID"]));

            CreateMap<ActionRole, ActionRoleViewModel>();            
            CreateMap<ActionRole, ActionRoleViewModel>();
            CreateMap<CriteriaCalcMethod, CriteriaCalcMethodViewModel>();
            
        }
    }
}
