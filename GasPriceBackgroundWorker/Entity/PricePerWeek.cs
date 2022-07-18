using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasPriceBackgroundWorker.Entity
{
    public class PricePerWeek
    {
        public int Id { get; set; }
        public string PriceDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
