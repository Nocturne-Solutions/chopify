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
            services.AddTransient<ISongRepository, SongRepository>();
            services.AddTransient<ISuggestionRepository, SuggestionRepository>();
            services.AddTransient<IVoteRepository, VoteRepository>();
            services.AddTransient<IWinnerRepository, WinnerRepository>();
            services.AddTransient<ICooldownRepository, CooldownRepository>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISongService, SongService>();
            services.AddTransient<ISuggestionService, SuggestionService>();
            services.AddTransient<IVoteService, VoteService>();
            services.AddTransient<IVotingSystemService, VotingSystemService>();
            services.AddTransient<IWinnerService, WinnerService>();
            services.AddTransient<ICooldownService, CooldownService>();

            return services;
        }
    }
}
