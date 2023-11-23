using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using ApiSync.Controllers;

namespace ApiSync
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public MyBackgroundService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    using (var scope = _provider.CreateScope())
            //    {
            //        var controller = scope.ServiceProvider.GetRequiredService<AdmitidosController>();
            //        //await controller.YourMethodAsync();
            //    }

            //    // Intervalo de espera (por exemplo, a cada 5 minutos)
            //    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            //}
        }
    }
}
