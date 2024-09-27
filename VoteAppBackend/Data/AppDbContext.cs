using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VoteAppBackend.Models;

namespace VoteAppBackend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Vote> Votes { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la conversión para la propiedad Preferences
            modelBuilder.Entity<UserPreference>()
                .Property(up => up.Preferences)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v), // Convertir Dictionary a JSON al guardar
                    v => JsonConvert.DeserializeObject<Dictionary<string, int>>(v)); // Convertir JSON a Dictionary al leer
        }
    }
}
