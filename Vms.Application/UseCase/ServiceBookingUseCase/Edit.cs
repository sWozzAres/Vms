using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Vms.Application.Services;
using Vms.Domain.Entity;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IEdit
{
    Task<bool> EditAsync(Guid id, ServiceBookingDto command, CancellationToken cancellationToken);
}

public class Edit(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IEdit
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();

    public async Task<bool> EditAsync(Guid id, ServiceBookingDto command, CancellationToken cancellationToken)
    {
        //ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
        //    ?? throw new InvalidOperationException("Failed to load service booking."), this);

        var serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine("# Edit");

        bool isModified = false;

        if (serviceBooking.PreferredDate1 != command.PreferredDate1)
        {
            SummaryText.AppendLine($"* Preferred Date 1: {command.PreferredDate1}");
            serviceBooking.PreferredDate1 = command.PreferredDate1;
            isModified = true;
        }
        if (serviceBooking.PreferredDate2 != command.PreferredDate2)
        {
            SummaryText.AppendLine($"* Preferred Date 2: {command.PreferredDate2}");
            serviceBooking.PreferredDate2 = command.PreferredDate2;
            isModified = true;
        }
        if (serviceBooking.PreferredDate3 != command.PreferredDate3)
        {
            SummaryText.AppendLine($"* Preferred Date 3: {command.PreferredDate3}");
            serviceBooking.PreferredDate3 = command.PreferredDate3;
            isModified = true;
        }

        var sl = (Vms.Domain.Entity.ServiceBookingEntity.ServiceLevel)command.ServiceLevel;
        if (serviceBooking.ServiceLevel != sl)
        {
            SummaryText.AppendLine($"* Service Level: {sl.ToString()}");
            serviceBooking.ServiceLevel = sl;
            isModified = true;
        }

        if (serviceBooking.AssignedToUserId != command.AssignedToUserId)
        {
            if (command.AssignedToUserId is null)
            {
                SummaryText.AppendLine($"* Removed Assigned User");
            }
            else
            {
                var user = await DbContext.Users.FindAsync(new object[] { command.AssignedToUserId }, cancellationToken)
                    ?? throw new InvalidOperationException("Failed to load user.");

                SummaryText.AppendLine($"* Assigned To: {user.UserName}");
            }
            serviceBooking.AssignedToUserId = command.AssignedToUserId;
            isModified = true;
        }


        if (serviceBooking.Driver.Name != command.Driver_Name)
        {
            SummaryText.AppendLine($"* Driver Name: {command.Driver_Name}");
            serviceBooking.Driver.Name = command.Driver_Name;
            isModified = true;
        }
        if (serviceBooking.Driver.EmailAddress != command.Driver_EmailAddress)
        {
            SummaryText.AppendLine($"* Driver Email Address: {command.Driver_EmailAddress}");
            serviceBooking.Driver.EmailAddress = command.Driver_EmailAddress;
            isModified = true;
        }
        if (serviceBooking.Driver.MobileNumber != command.Driver_MobileNumber)
        {
            SummaryText.AppendLine($"* Driver Mobile Number: {command.Driver_MobileNumber}");
            serviceBooking.Driver.MobileNumber = command.Driver_MobileNumber;
            isModified = true;
        }
        if (serviceBooking.Contact.Name != command.Contact_Name)
        {
            SummaryText.AppendLine($"* Contact Name: {command.Contact_Name}");
            serviceBooking.Contact.Name = command.Contact_Name;
            isModified = true;
        }
        if (serviceBooking.Contact.EmailAddress != command.Contact_EmailAddress)
        {
            SummaryText.AppendLine($"* Contact Email Address: {command.Contact_EmailAddress}");
            serviceBooking.Contact.EmailAddress = command.Contact_EmailAddress;
            isModified = true;
        }
        if (serviceBooking.Contact.MobileNumber != command.Contact_MobileNumber)
        {
            SummaryText.AppendLine($"* Contact Mobile Number: {command.Contact_MobileNumber}");
            serviceBooking.Contact.MobileNumber = command.Contact_MobileNumber;
            isModified = true;
        }

        if (serviceBooking.Status == ServiceBookingStatus.None && serviceBooking.IsValid)
        {
            SummaryText.AppendLine($"* Status: {ServiceBookingStatus.Assign.ToString()}");
            serviceBooking.ChangeStatus(ServiceBookingStatus.Assign);
            isModified = true;
        }

        if (isModified)
        {
            await ActivityLog.LogAsync(id, SummaryText, cancellationToken);
            TaskLogger.Log(id, "Edit", command);
        }

        return isModified;
    }

    //class ServiceBookingRole(ServiceBooking self, Edit ctx)
    //{

    //}
}
