using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GasPriceBackgroundWorker.DTO;
using GasPriceBackgroundWorker.Entity;
using GasPriceBackgroundWorker.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
namespace GasPriceBackgroundWorker.Services
{
    public class GasPriceService : IGasPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GasPriceService(HttpClient httpClient,  IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<EIASeries> GetEIASeries()
        {
            EIASeries series = new EIASeries();

            var responseString = await _httpClient.GetStringAsync(_configuration["BaseURL"]);

            series = JsonConvert.DeserializeObject<EIASeries>(responseString);

            return series;
        }


    }
}
