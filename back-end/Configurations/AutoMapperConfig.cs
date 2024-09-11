using AutoMapper;
using chopify.Mappings;

namespace chopify.Configurations
{
    public static class AutoMapperConfig
    {
        private static readonly Lazy<MapperConfiguration> LazyInstance = new(Configure);

        public static MapperConfiguration GetInstance() => LazyInstance.Value;

        private static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMapper>();
                cfg.AddProfile<SongMapper>();
                cfg.AddProfile<SuggestionMapper>();
            });
        }
    }
}
