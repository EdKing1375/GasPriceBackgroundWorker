using System;

namespace GasPriceBackgroundWorker.Entity
{
    class PricePerWeek
    {
        public int Id { get; set; }
        public DateTime PriceDate { get; set; }
        public decimal Price { get; set; }
    }
}
