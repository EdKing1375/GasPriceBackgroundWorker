using GasPriceBackgroundWorker.DTO;
using System.Threading.Tasks;

namespace GasPriceBackgroundWorker.Services
{
    public interface IGasPriceService
    {
        Task<EIASeries> GetEIASeries();
    }
}