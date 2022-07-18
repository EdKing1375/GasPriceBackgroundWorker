using GasPriceBackgroundWorker.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GasPriceBackgroundWorker.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GasPriceBackgroundWorker.Repository;

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
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddHostedService<Worker>();
                    // var optionBuilder = new DbContextOptionsBuilder<GassPriceContext>();
                    // optionBuilder.UseSqlServer(configuration.GetConnectionString("GasPriceConneciton"));
                    // services.AddScoped<GassPriceContext>(d => new GassPriceContext(optionBuilder.Options));
                    services.AddTransient<IPricePerWeekRepository, PricePerWeekRepository>();
                    services.AddScoped<IGasPriceService, GasPriceService>();
                    services.AddHttpClient<IGasPriceService, GasPriceService>();
                });
    }
}
