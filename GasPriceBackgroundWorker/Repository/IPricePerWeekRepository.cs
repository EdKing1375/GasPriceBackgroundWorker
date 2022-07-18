using GasPriceBackgroundWorker.Entity;
using System.Collections.Generic;

namespace GasPriceBackgroundWorker.Repository
{
    public interface IPricePerWeekRepository
    {
        void AddPricePerWeek(List<PricePerWeek> pricePerWeek);
        List<PricePerWeek> GetPricePerWeekRange(int startOfRange);
    }
}