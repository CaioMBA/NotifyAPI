using AutoMapper;
using Data.DataBaseConnections;
using DataBaseConnections.OracleSqlDao;
using Domain.Interfaces;
using Domain.Mapping;
using Domain.Models.Settings;
using Domain.Utils;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;

namespace Infrastructure.CrossCutting
{
    public class AllConfigurations
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection, AppSettingsModel appConfig)
        {
            serviceCollection.AddSingleton(appConfig);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesOuters(serviceCollection);
        }

        public static void ConfigureDependenciesService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IApiService, ApiService>();
            serviceCollection.AddTransient<INotifyService, NotifyService>();
        }


        public static void ConfigureDependenciesOuters(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Utils>();
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<DefaultAccess>();
            serviceCollection.AddTransient<CredentialDao>();
        }


        public static void ConfigureAutoMapper(IServiceCollection serviceCollection)
        {
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DtoToModel());
            });
            IMapper mapper = config.CreateMapper();
            serviceCollection.AddSingleton(mapper);
        }
    }
}
