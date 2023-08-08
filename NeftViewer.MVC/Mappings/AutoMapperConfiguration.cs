using AutoMapper;

namespace NeftViewer.MVC.Mappings
{
    public static class AutoMapperConfiguration
    {
        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToViewModelMappingProfile>();
                cfg.AddProfile<ViewModelToDomainMappingProfile>();

            });

            IMapper mapper = config.CreateMapper();
            return mapper;
        }
    }
}
