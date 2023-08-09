using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vms.Domain.Core;
using Vms.Domain.Core.Configuration;
using Vms.Domain.Infrastructure.Services;
using Vms.Domain.ServiceBookingProcess;
using Vms.Domain.System;

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
    public DbSet<SupplierRefusal> SupplierRefusals => Set<SupplierRefusal>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleVrm> VehicleVrms => Set<VehicleVrm>();
    public DbSet<VehicleMake> VehicleMakes => Set<VehicleMake>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<NotCompleteReason> NotCompleteReasons => Set<NotCompleteReason>();
    public DbSet<RefusalReason> RefusalReasons => Set<RefusalReason>();
    public DbSet<ConfirmBookedRefusalReason> ConfirmBookedRefusalReasons => Set<ConfirmBookedRefusalReason>();
    public DbSet<RescheduleReason> RescheduleReasons => Set<RescheduleReason>();
    public DbSet<NonArrivalReason> NonArrivalReasons => Set<NonArrivalReason>();
    public DbSet<Email> Emails => Set<Email>();
    public DbSet<MotEvent> MotEvents => Set<MotEvent>();
    public DbSet<ActivityLog> ActivityLog => Set<ActivityLog>();
    public DbSet<ServiceBookingLock> ServiceBookingLocks => Set<ServiceBookingLock>();
    public DbSet<Follower> Followers => Set<Follower>();
    public DbSet<TaskLog> TaskLogs => Set<TaskLog>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Login> Logins => Set<Login>();
    public DbSet<EntityTag> EntityTags => Set<EntityTag>();
    public DbSet<RecentView> RecentViews => Set<RecentView>();

    protected readonly IUserProvider _userProvider;
    readonly ILogger<VmsDbContext> _logger;
    public VmsDbContext(DbContextOptions<VmsDbContext> options, IUserProvider userProvider, ILogger<VmsDbContext> logger) : base(options)
        => (_userProvider, _logger) = (userProvider, logger);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _logger.LogInformation("OnModelCreating. UserId '{userId}', TenantId '{tenantId}'.",
            _userProvider.UserId, _userProvider.TenantId);

        //base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyEntityTypeConfiguration).Assembly);
        //modelBuilder.Entity<ActivityLog>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<EntityTag>().HasQueryFilter(x => x.CompanyCode == null || (_userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId));
        modelBuilder.Entity<NotCompleteReason>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<NonArrivalReason>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<ConfirmBookedRefusalReason>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<MotEvent>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<RefusalReason>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<RescheduleReason>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<ServiceBooking>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Driver>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<DriverVehicle>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Company>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.Code == _userProvider.TenantId);
        modelBuilder.Entity<Customer>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<CustomerNetwork>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Network>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<NetworkSupplier>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Fleet>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<FleetNetwork>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
        modelBuilder.Entity<Vehicle>().HasQueryFilter(x => _userProvider.TenantId == "*" || x.CompanyCode == _userProvider.TenantId);
    }

    #region Transaction
    //private IDbContextTransaction? _currentTransaction;
    //public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;
    //public bool HasActiveTransaction => _currentTransaction is not null;
    //public async Task<IDbContextTransaction?> BeginTransactionAsync()
    //{
    //    if (_currentTransaction is not null) return null;

    //    _currentTransaction = await Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

    //    return _currentTransaction;
    //}

    //public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    //{
    //    //if (transaction is null) throw new ArgumentNullException(nameof(transaction));
    //    if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current.");

    //    try
    //    {
    //        await SaveChangesAsync();
    //        transaction.Commit();
    //    }
    //    catch
    //    {
    //        RollbackTransaction();
    //        throw;
    //    }
    //    finally
    //    {
    //        if (_currentTransaction is not null)
    //        {
    //            _currentTransaction.Dispose();
    //            _currentTransaction = null;
    //        }
    //    }
    //}

    //public void RollbackTransaction()
    //{
    //    try
    //    {
    //        _currentTransaction?.Rollback();
    //    }
    //    finally
    //    {
    //        if (_currentTransaction is not null)
    //        {
    //            _currentTransaction.Dispose();
    //            _currentTransaction = null;
    //        }
    //    }
    //}
    #endregion
}
