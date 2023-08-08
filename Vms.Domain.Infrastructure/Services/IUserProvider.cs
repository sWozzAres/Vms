namespace Vms.Domain.Infrastructure.Services;

public interface IUserProvider
{
    string UserId { get; }
    string UserName { get; }
    string EmailAddress { get; }
    string TenantId { get; }
    public bool HasAccessToTenant(string companyCode) => TenantId == "*" || TenantId == companyCode;
}
