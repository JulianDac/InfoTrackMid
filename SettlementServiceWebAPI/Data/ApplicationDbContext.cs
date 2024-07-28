using Microsoft.EntityFrameworkCore;
using SettlementServiceWebAPI.Models;

namespace SettlementServiceWebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Booking> Bookings { get; set; }
    }
}
