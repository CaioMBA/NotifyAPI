using AutoMapper;
using Domain.Interfaces;
using Domain.Mapping;
using Domain.Models.GeneralSettings;
using Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Data.API;
using Data.DataBase;
using Domain.Utils;
using Data.DataBase.SqlServerDAO;

namespace CrossCutting.DependencyInjection
{
    public class AllConfigurations
    {
        public static void ConfigureDependencies(IServiceCollection serviceCollection, AppSettingsModel appConfig)
        {
            serviceCollection.AddSingleton(appConfig);
            ConfigureAutoMapper(serviceCollection);
            ConfigureDependenciesService(serviceCollection);
            ConfigureDependenciesRepository(serviceCollection);
            ConfigureDependenciesExtras(serviceCollection);
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

        public static void ConfigureDependenciesService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<INotifyService, NotifyService>();
            /*serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddSingleton<IApiService, ApiService>();*/
        }

        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ApiDefaultAccess>();
            serviceCollection.AddTransient<DataBaseDefaultAccess>();
            serviceCollection.AddTransient<CredentialDAO>();
        }

        public static void ConfigureDependenciesExtras(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<Utils>();
        }
    }
}
