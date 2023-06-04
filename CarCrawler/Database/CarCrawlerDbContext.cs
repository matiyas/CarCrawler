using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace CarCrawler.Database;

internal class CarCrawlerDbContext : DbContext
{
    public DbSet<AdDetails> AdDetails { get; set; } = null!;

    public string DbPath { get; }

    public CarCrawlerDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "carCrawler.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            $"Data Source={DbPath}",
            optionsBuilder => optionsBuilder.UseNetTopologySuite());
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Logger.Log);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdDetails>()
            .Property(p => p.SellerPhones)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IEnumerable<string>>(v));
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(IsEntryAddedOrModified);

        foreach (var entityEntry in entries)
        {
            ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }

    private static bool IsEntryAddedOrModified(EntityEntry entry)
    {
        return entry.Entity is BaseEntity &&
            (entry.State == EntityState.Added || entry.State == EntityState.Modified);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<IEnumerable<string>>()
            .HaveConversion<EnumerableConverter>();
    }
}