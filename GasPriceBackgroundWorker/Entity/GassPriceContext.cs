using Microsoft.EntityFrameworkCore;

namespace GasPriceBackgroundWorker.Entity
{
    public class GassPriceContext : DbContext
    {
        public GassPriceContext(DbContextOptions<GassPriceContext> options) : base(options){}

        public DbSet<PricePerWeek> PricePerWeeks { get; set; }
    }
}
