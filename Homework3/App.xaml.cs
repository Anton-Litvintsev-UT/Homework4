using Homework3.Data;
using Homework3.Generics;
using Homework3.Interfaces;
using Homework3.Models;
using Homework3.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Homework3
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<ViewModel>();
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ZooDbContext>(options =>
            {
                options.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Homework3Db;Trusted_Connection=True;");
            });

            services.AddScoped<IRepository<AbstractAnimal>, AnimalRepository>();
            services.AddSingleton<ILogger, JsonLogger>(); // Or XmlLogger
            services.AddTransient<ViewModel>();
            services.AddTransient<MainWindow>();
        }
    }
}