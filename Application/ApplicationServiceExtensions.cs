using Application.Interfaces;
using Application.ServiceImplementation;
using Application.ServiceInterfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            // AutoMapper
            Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

            // Service Manager (Facade Pattern)
            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IServiceManger, ServiceManager>();
            Services.AddScoped<ICoachService, CoachService>();
            Services.AddScoped<ISwimmerService, SwimmerService>();
            Services.AddScoped<ITeamService, TeamService>();
            Services.AddScoped<IPerformanceRecordService, PerformanceRecordService>();
            Services.AddScoped<IPerformanceNoteService, PerformanceNoteService>();
            return Services;
        }
    }
}
