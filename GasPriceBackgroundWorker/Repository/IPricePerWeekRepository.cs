using GasPriceBackgroundWorker.Entity;
using System.Collections.Generic;

namespace GasPriceBackgroundWorker.Repository
{
    public interface IPricePerWeekRepository
    {
        void AddPricesPerWeek(List<PricePerWeek> pricePerWeek);
        List<PricePerWeek> GetPricePerWeekRange(int startOfRange);
    }
}