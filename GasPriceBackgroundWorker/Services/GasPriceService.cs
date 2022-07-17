using System.Net.Http;
using System.Threading.Tasks;
using GasPriceBackgroundWorker.DTO;

namespace GasPriceBackgroundWorker.Services
{
    public class GasPriceService : IGasPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl;

        public GasPriceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EIASeries>  GetEIASeries()
        {
            return new EIASeries();
        }
    }
}
