using System.Text;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IUnbookSupplier
{
    Task UnbookAsync(Guid id, TaskUnbookSupplierCommand command, CancellationToken cancellationToken);
}

public class UnbookSupplier(VmsDbContext dbContext, IUserProvider userProvider) : IUnbookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();

    public async Task UnbookAsync(Guid id, TaskUnbookSupplierCommand command, CancellationToken cancellationToken)
    {
        var serviceBooking = await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine("# Unbook Supplier");

        serviceBooking.Unbook();

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));
    }
}
