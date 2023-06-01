using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vms.Domain.Entity;
using Vms.Domain.Entity.Configuration;
using Vms.Domain.Services;

namespace Vms.Domain.Infrastructure;

public class VmsDbContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerNetwork> CustomerNetworks => Set<CustomerNetwork>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<DriverVehicle> DriverVehicles => Set<DriverVehicle>();
    public DbSet<Fleet> Fleets => Set<Fleet>();
    public DbSet<FleetNetwork> FleetNetworks => Set<FleetNetwork>();
    public DbSet<Network> Networks => Set<Network>();
    public DbSet<VehicleMot> NextMots => Set<VehicleMot>();
    public DbSet<ServiceBooking> ServiceBookings => Set<ServiceBooking>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleVrm> VehicleVrms => Set<VehicleVrm>();
    public DbSet<VehicleMake> VehicleMakes => Set<VehicleMake>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();

    readonly IUserProvider UserProvider;
    public VmsDbContext(DbContextOptions<VmsDbContext> options, IUserProvider userProvider) : base(options)
        => UserProvider = userProvider;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyEntityTypeConfiguration).Assembly);

        modelBuilder.Entity<Company>().HasQueryFilter(x => x.Code == UserProvider.TenantId);
        modelBuilder.Entity<Vehicle>().HasQueryFilter(x => x.CompanyCode == UserProvider.TenantId);
    }

    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        if (entity is IMultiTenantEntity m)
        {
            m.CompanyCode = UserProvider.TenantId;
        }

        return base.Add(entity);
    }
    public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is IMultiTenantEntity m)
        {
            m.CompanyCode = UserProvider.TenantId;
        }

        return base.AddAsync(entity, cancellationToken);
    }
    public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
    {
        if (entity is IMultiTenantEntity m)
        {
            m.CompanyCode = UserProvider.TenantId;
        }

        return base.AddAsync(entity, cancellationToken);
    }
}
