using System;
using Quartz;
using System.Threading.Tasks;
using GasPriceBackgroundWorker.Services;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Collections.Generic;
using GasPriceBackgroundWorker.Entity;

namespace GasPriceBackgroundWorker.Jobs
{
    [DisallowConcurrentExecution]
    public class GetGassPrice : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var logger = context.MergedJobDataMap.Get("logger") as ILogger;
            var gasPriceService = context.MergedJobDataMap.Get("service") as GasPriceService;

            var series = await gasPriceService.GetEIASeries();
            var PricePerWeek = MapDataToEntity(series);
            //logger.LogInformation("Worker ran series with count " + seriesData.ToString());
        }

        private static List<PricePerWeek> MapDataToEntity(DTO.EIASeries series)
        {
            var seriesData = series.series[0].data;

            List<PricePerWeek> pricesPerWeek = new List<PricePerWeek>();
            foreach (List<object> seriesItem in seriesData)
            {
                decimal.TryParse(seriesItem[1].ToString(), out decimal price);
                long.TryParse(seriesItem[0].ToString(), out long unixTime);

                DateTime priceDateTime = new DateTime(unixTime);
                pricesPerWeek.Add(
                    new PricePerWeek
                    {
                        Price = price,
                        PriceDate = priceDateTime.ToString("yyyyMMdd")
                    }
                );
            }

            return pricesPerWeek;
        }
    }
}
