﻿using GasPriceBackgroundWorker.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GasPriceBackgroundWorker.Repository
{
    public class PricePerWeekRepository : IPricePerWeekRepository
    {
        private readonly IConfiguration _configuration;
        private GassPriceContext context;
        public PricePerWeekRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private DbContextOptions<GassPriceContext> GetAllOptions()
        {
            var connection = _configuration.GetConnectionString("GasPriceConneciton");
            var optionBuilder = new DbContextOptionsBuilder<GassPriceContext>();
            optionBuilder.UseSqlServer(connection);
            return optionBuilder.Options;
        }
        public List<PricePerWeek> GetPricePerWeekRange(int daysOld)
        {
            using (context = new GassPriceContext(GetAllOptions()))
            {
                var StartDate = DateTime.Now.AddDays(-daysOld).ToString("yyyyMMdd");
                return context.PricePerWeeks
                     .Where(x =>
                     string.Compare(x.PriceDate, StartDate) >=0
                     ).ToList();
            }
        }
        public void AddPricesPerWeek(List<PricePerWeek> pricesPerWeek)
        {
            using (context = new GassPriceContext(GetAllOptions()))
            {

                context.PricePerWeeks.AddRange(pricesPerWeek);
                context.SaveChanges();
            }
        }

    }
}
