﻿using System;
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
                var prices = MapDataToPricesPerWork(series);

                int.TryParse(_configuration["DaysOld"], out int daysOld);
                var existingPrices = pricePerWeekRepo.GetPricePerWeekRange(daysOld);

                var addPrices = TryToFindNewPricesToAdd(prices, existingPrices, daysOld);
                if (addPrices.Count > 0)
                {
                    pricePerWeekRepo.AddPricesPerWeek(addPrices);
                }
            }
            catch (Exception err)
            {
                logger.LogError("The GetGassPrice Job Failed ", err.Message);
            }
        }

        private List<PricePerWeek> TryToFindNewPricesToAdd(List<PricePerWeek> prices, List<PricePerWeek> existingPrices, int daysOld)
        {
            var StartDate = DateTime.Now.AddDays(-daysOld).ToString("yyyyMMdd");
            if (existingPrices != null && existingPrices.Count > 0)
            {
                prices.RemoveAll(x =>
                existingPrices
                .Select(y =>
                    y.PriceDate)
                    .Contains(x.PriceDate)
                );
            }
            prices.RemoveAll(x =>
               string.Compare(x.PriceDate, StartDate) <= 0
            );
            return prices;
        }

        private List<PricePerWeek> MapDataToPricesPerWork(DTO.EIASeries series)
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
