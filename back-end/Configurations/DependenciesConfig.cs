﻿using chopify.Data.Repositories.Implementations;
using chopify.Data.Repositories.Interfaces;
using chopify.Services.Implementations;
using chopify.Services.Interfaces;

namespace chopify.Configurations
{
    public static class DependenciesConfig
    {
        public static IServiceCollection DependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMusicRepository, MusicRepository>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IMusicService, MusicService>();

            return services;
        }
    }
}
