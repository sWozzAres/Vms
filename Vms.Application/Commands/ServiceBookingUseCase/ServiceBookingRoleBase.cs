using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public abstract class ServiceBookingRoleBase<TContext>(ServiceBooking self, TContext ctx)
    where TContext : IUseCase<VmsDbContext, Guid>
{
    protected ServiceBooking Self { get; private set; } = self;
    protected TContext Ctx { get; private set; } = ctx;
    public ServiceBooking Entity => Self;
    public async Task Reschedule(string code, DateTime rescheduleTime)
    {
        var reason = await Ctx.DbContext.RescheduleReasons.AsNoTracking()
            .SingleAsync(r => r.CompanyCode == Self.CompanyCode && r.Code == code, Ctx.CancellationToken);

        Ctx.SummaryText.AppendLine("## Rescheduled");
        Ctx.SummaryText.AppendLine($"* Time: {rescheduleTime.ToString("f")}");
        Ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
        Ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");
        Self.RescheduleTime = rescheduleTime;
    }
}