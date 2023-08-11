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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]));

            CreateMap<Dictionary<string, object>, Road>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["Id"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]))
                .ForMember(dest => dest.Indicator, opt => opt.MapFrom(src => src["Indicator"]));
            CreateMap<NeftViewer.Data.Models.Action, ActionViewModel>();

            CreateMap<Dictionary<string, object>, ObjectItem>()
                .ForMember(dest => dest.CodeSUID, opt => opt.MapFrom(src => src["CodeSUID"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]));
            CreateMap<NeftViewer.Data.Models.Action, ActionViewModel>();

            CreateMap<Dictionary<string, object>, Customer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["Id"]))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src["Name"]));
            CreateMap<NeftViewer.Data.Models.Action, ActionViewModel>();

            //CreateMap<Dictionary<string, object>, ObjectOnRoad>()
            //    .ForMember(dest => dest.RoadId, opt => opt.MapFrom(src => src["RoadId"]))
            //    .ForMember(dest => dest.UIDObject, opt => opt.MapFrom(src => src["UIDObject"]))
            //    .ForMember(dest => dest.Roads, opt => opt.MapFrom(src => new Road { Id = (string)src["RoadId"] }))
            //    .ForMember(dest => dest.Objects, opt => opt.MapFrom(src => new ObjectItem { CodeSUID = (string)src["UIDObject"] }));
            //CreateMap<NeftViewer.Data.Models.Action, ActionViewModel>();

            CreateMap<Dictionary<string, object>, IndicatorValue>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src["id"]))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src["CustomerId"]))
                .ForMember(dest => dest.CodeSUID, opt => opt.MapFrom(src => src["CodeSUID"]))
                .ForMember(dest => dest.DateStart, opt => opt.MapFrom(src => src["DateStart"]))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src["Value"]));
            CreateMap<NeftViewer.Data.Models.Action, ActionViewModel>();

        }
        //private string ByteArrayToString(byte[] byteArray)
        //{
        //    return BitConverter.ToString(byteArray).Replace("-", "");
        //}
    }
}
