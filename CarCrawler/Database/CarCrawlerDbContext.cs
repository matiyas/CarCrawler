using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace CarCrawler.Database;

internal class CarCrawlerDbContext : DbContext
{
    private readonly IAppLogger? _logger;

    public DbSet<AdDetails> AdDetails { get; set; } = null!;
    public DbSet<VehicleHistoryReport> VehicleHistoryReport { get; set; } = null!;

    public string DbPath { get; }

    public CarCrawlerDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "carCrawler.db");
    }

    public CarCrawlerDbContext(IAppLogger logger) : this()
    {
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(
            $"Data Source={DbPath}",
            optionsBuilder => optionsBuilder.UseNetTopologySuite());
        optionsBuilder.EnableSensitiveDataLogging();

        if (_logger is not null) optionsBuilder.LogTo(_logger.Log);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdDetails>()
            .Property(p => p.SellerPhones)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IEnumerable<string>>(v));

        modelBuilder.Entity<AdDetails>()
            .HasOne(adDetails => adDetails.VehicleHistoryReport)
            .WithOne(report => report.AdDetails)
            .HasForeignKey<VehicleHistoryReport>(report => report.AdDetailsId)
            .OnDelete(DeleteBehavior.Cascade);
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