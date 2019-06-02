using Aster.Common.Data.Core;
using Aster.Common.Data.Core.Configuration;
using Aster.Common.Data.Core.Implementor;
using Aster.Common.Data.Core.Repositories;
using Aster.Common.Data.Core.Sessions;
using Aster.Common.Data.Core.Sql;
using Aster.Common.Data.Implementor;
using Aster.Common.Data.Mapper;
using Aster.Common.Data.MySql;
using Aster.Common.Data.Repositories;
using Aster.Common.Data.Sql;
using Aster.Desktop.Views;
using Aster.Infrastructure;
using Aster.Infrastructure.Constants;
using Aster.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;
using PrismMahAppsSample.Infrastructure.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Unity.Lifetime;

namespace Aster.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Application commands
            containerRegistry.Register<IApplicationCommands, ApplicationCommandsProxy>();
            // Flyout service
            containerRegistry.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>());

            // Localizer service
            // Localizer-Service
            LoggerFactory var = new LoggerFactory();
            containerRegistry.RegisterInstance(typeof(ILocalizerService),new LocalizerService("de-DE"));
               var envBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
               var envConfig = envBuilder.Build();
               var dapperConfig=   DapperConfiguration
                .Use(GetAllConnectionStrings(envConfig), var)
                .UseClassMapper(typeof(AutoClassMapper<>))
                .UseSqlDialect(new MySqlDialect())
                .WithDefaultConnectionStringNamed("DefaultConnectionString")
                .FromAssemblies(GetEntityAssemblies())
                .Build();

            containerRegistry.RegisterInstance(typeof(IDapperConfiguration), dapperConfig);

            containerRegistry.RegisterSingleton<IConnectionStringProvider, StaticConnectionStringProvider>();
            containerRegistry.RegisterSingleton<IDapperSessionFactory, DapperSessionFactory>();
            containerRegistry.Register<IDapperSessionContext, DapperSessionContext>();
            containerRegistry.Register<ISqlGenerator, SqlGeneratorImpl>();
            containerRegistry.Register<IDapperImplementor, DapperImplementor>();
            containerRegistry.Register(typeof(IRepository<>), typeof(DapperRepository<>));


        }



        private static IDictionary<string, string> GetAllConnectionStrings(IConfiguration configuration)
        {
            var sections = configuration.GetSection("ConnectionStrings");

            return sections.GetChildren().ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<Assembly> GetEntityAssemblies()
        {
            var dllFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Directory.GetFiles(dllFolder, "Aster.Entities.dll")
                 .SelectMany(x => Assembly.LoadFrom(x).GetTypes())
                 .Where(x => typeof(IEntity).IsAssignableFrom(x))
                 .Select(x => x.Assembly)
                 .Distinct();
        }
    }
}
