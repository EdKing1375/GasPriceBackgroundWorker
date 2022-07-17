using System;
using System.Net.Http;
using System.Threading.Tasks;
using GasPriceBackgroundWorker.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GasPriceBackgroundWorker.Services
{
    public class GasPriceService : IGasPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public GasPriceService(HttpClient httpClient,   IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<EIASeries>  GetEIASeries()
        {
            EIASeries series = new EIASeries();
            try
            {
                var responseString = await _httpClient.GetStringAsync(_configuration["baseURL"]);

                 series = JsonConvert.DeserializeObject<EIASeries>(responseString);
            }
            catch(Exception err)
            {
                var message = err.Message;
            }
                return series;
        }
    }
}
