using System;
using Quartz;
using System.Threading.Tasks;
using GasPriceBackgroundWorker.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using GasPriceBackgroundWorker.Repository;
using Microsoft.Extensions.Configuration;
using GasPriceBackgroundWorker.Entity;
using System.Linq;

namespace GasPriceBackgroundWorker.Jobs
{
    [DisallowConcurrentExecution]
    public class GetGassPrice : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var logger = context.MergedJobDataMap.Get("logger") as ILogger;
            var gasPriceService = context.MergedJobDataMap.Get("service") as GasPriceService;
            var pricePerWeekRepo = context.MergedJobDataMap.Get("repo") as PricePerWeekRepository;
            var _configuration = context.MergedJobDataMap.Get("config") as IConfiguration;

            try
            {
                var series = await gasPriceService.GetEIASeries();
                var prices = MapDataToEntity(series);

                int.TryParse(_configuration["DaysOld"], out int daysOld);
                var existingPrices = pricePerWeekRepo.GetPricePerWeekRange(daysOld);

                TryToAddNewPrices(prices, existingPrices, daysOld);
            }
            catch (Exception err) 
            {
                logger.LogError("The GetGassPrice Job Failed ", err.Message);
            }
        }

        private void TryToAddNewPrices(List<PricePerWeek> prices, List<PricePerWeek> existingPrices, int daysOld)
        {
   

            if (existingPrices != null)
            {
                prices.RemoveAll(x =>
                existingPrices.Select(y => y.PriceDate).Contains(x.PriceDate)
                &&
                DateTime.Parse(x.PriceDate,
                System.Globalization.CultureInfo.InvariantCulture)
                >= DateTime.Now.AddDays(-daysOld)
                );
            }
            var newPrices = prices;
        }

        private List<PricePerWeek> MapDataToEntity(DTO.EIASeries series)
        {
            var seriesData = series.series[0].data;

            List<PricePerWeek> pricesPerWeek = new List<PricePerWeek>();
            foreach (List<object> seriesItem in seriesData)
            {
                decimal.TryParse(seriesItem[1].ToString(), out decimal price);
                var date = seriesItem[0].ToString();

                pricesPerWeek.Add(
                    new PricePerWeek
                    {
                        Price = price,
                        PriceDate = date
                    }
                );
            }

            return pricesPerWeek;
        }
    }
}
