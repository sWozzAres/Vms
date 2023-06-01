using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
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

        modelBuilder.HasSequence<int>("CompanyIds");
        modelBuilder.HasSequence<int>("CustomerIds");
        modelBuilder.HasSequence<int>("VehicleIds");

        modelBuilder.Entity<Company>().HasQueryFilter(x => x.Code == UserProvider.TenantId);
        modelBuilder.Entity<Customer>().HasQueryFilter(x => x.Code == UserProvider.TenantId);
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
    public override void AddRange(IEnumerable<object> entities)
    {
        foreach(var entity in entities)
        {
            if (entity is IMultiTenantEntity m) m.CompanyCode = UserProvider.TenantId;
        }

        base.AddRange(entities);
    }
    public override Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            if (entity is IMultiTenantEntity m) m.CompanyCode = UserProvider.TenantId;
        }

        return base.AddRangeAsync(entities, cancellationToken);
    }

    #region Transaction
    private IDbContextTransaction? _currentTransaction;
    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction is not null;
    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction is not null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
    #endregion
}
