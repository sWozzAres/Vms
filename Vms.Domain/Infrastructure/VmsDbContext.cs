using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
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
    public DbSet<NetworkSupplier> NetworkSuppliers => Set<NetworkSupplier>();
    public DbSet<VehicleMot> NextMots => Set<VehicleMot>();
    public DbSet<ServiceBooking> ServiceBookings => Set<ServiceBooking>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleVrm> VehicleVrms => Set<VehicleVrm>();
    public DbSet<VehicleMake> VehicleMakes => Set<VehicleMake>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();

    protected readonly IUserProvider _userProvider;
    ILogger<VmsDbContext> _logger;
    public VmsDbContext(DbContextOptions<VmsDbContext> options, IUserProvider userProvider, ILogger<VmsDbContext> logger) : base(options)
        => (_userProvider, _logger) = (userProvider, logger);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.LogInformation("OnModelCreating. UserId '{userId}', TenantId '{tenantId}'.", 
            _userProvider.UserId, _userProvider.TenantId);

        //base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyEntityTypeConfiguration).Assembly);

        //modelBuilder.HasSequence<int>("CompanyIds");
        //modelBuilder.HasSequence<int>("CustomerIds");
        //modelBuilder.HasSequence<int>("FleetIds");
        //modelBuilder.HasSequence<int>("NetworkIds");
        //modelBuilder.HasSequence<int>("VehicleIds");
        //modelBuilder.HasSequence<int>("SupplierIds");

        //if (!string.IsNullOrEmpty(_userProvider.TenantId))
        //if (_userProvider.TenantId != "*")
        {
            modelBuilder.Entity<Driver>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
            modelBuilder.Entity<Company>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.Code == _userProvider.TenantId);
            modelBuilder.Entity<Customer>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
            modelBuilder.Entity<Network>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
            modelBuilder.Entity<Fleet>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
            modelBuilder.Entity<Vehicle>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        }
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
        //if (transaction is null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current.");

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
