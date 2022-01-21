using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt ) : base(opt)
        {
            
        }

        public DbSet<Platformm> Platforms { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Platformm>()
                .HasMany(p => p.Commands)
                .WithOne(p=> p.Platformm!)
                .HasForeignKey(p => p.PlatformId);

            modelBuilder
                .Entity<Command>()
                .HasOne(p => p.Platformm)
                .WithMany(p => p.Commands)
                .HasForeignKey(p =>p.PlatformId);
        }
    }
}
