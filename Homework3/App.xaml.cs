using System.Windows;
using Homework3.Classes;
using Homework3.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Homework3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services;

        public App()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<ILogger>(new JsonLogger());
            // sc.AddSingleton<ILogger>(new XmlLogger());
            sc.AddTransient<ViewModel>();
            Services = sc.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Services.GetRequiredService<ViewModel>();
        }
    }

}
