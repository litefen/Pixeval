using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Pixeval
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        [STAThread]
        public static int Main()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            var app = _serviceProvider.GetRequiredService<App>();
            app.InitializeComponent();
            return app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<App>();
        }
    }
}
