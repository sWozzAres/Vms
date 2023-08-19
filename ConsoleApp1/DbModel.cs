using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Db;

[Table("MotEvents")]
public class MotEvent(Guid serviceBookingId, Guid vehicleId)
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ServiceBookingId { get; set; } = serviceBookingId;
    public ServiceBooking ServiceBooking { get; set; } = null!;
    public Guid VehicleId { get; set; } = vehicleId;
    public Vehicle Vehicle { get; set; } = null!;
}

[Table("ServiceBookings")]
public class ServiceBooking
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? MotEventId { get; set; }
    public MotEvent MotEvent { get; set; } = null!;
}

[Table("Vehicles")]
public class Vehicle
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? MotEventId { get; set; }
    public MotEvent MotEvent { get; set; } = null!;
}

public class TestContext : DbContext
{
    public DbSet<MotEvent> MotEvents => Set<MotEvent>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ServiceBooking> ServiceBookings => Set<ServiceBooking>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MotEvent>()
            .HasOne(e => e.Vehicle)
            .WithOne(e => e.MotEvent);

        modelBuilder.Entity<MotEvent>()
            .HasOne(e => e.ServiceBooking)
            .WithOne(e => e.MotEvent);
    }
    public TestContext(DbContextOptions<TestContext> options) : base(options) { }
}

public class TestContextFactory : IDesignTimeDbContextFactory<TestContext>
{
    public TestContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestContext>();
        optionsBuilder.UseSqlServer(@"Data Source=localhost\SQL2019;Initial Catalog=Test;Integrated Security=true;TrustServerCertificate=True;Encrypt=False",
            x =>
            {
                //x.UseNetTopologySuite();
                //x.UseDateOnlyTimeOnly();
            });

        return new TestContext(optionsBuilder.Options);
    }
}