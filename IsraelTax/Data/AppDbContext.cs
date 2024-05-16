using IsraelTax.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace IsraelTax.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ClientCredential> ClientCredentials { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }

}
