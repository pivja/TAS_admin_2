using System.Data.Entity;

namespace TAS_admin.Models
{
    public class TasDbContext : DbContext
    {
        public TasDbContext() : base("TasDbContext")
        {
        }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Truck> Trucks { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
    }
}
