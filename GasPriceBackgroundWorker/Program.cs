using GasPriceBackgroundWorker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;

namespace GasPriceBackgroundWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddHttpClient<IGasPriceService, GasPriceService>(client =>
                    {
                        client.BaseAddress = new Uri("https://api.eia.gov/series/?api_key=ec92aacd6947350dcb894062a4ad2d08&series_id=PET.EMD_EPD2D_PTE_NUS_DPG.W");
                    });
                });
    }
}
