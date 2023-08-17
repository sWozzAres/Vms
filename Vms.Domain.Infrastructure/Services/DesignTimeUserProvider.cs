using Utopia.Api.Services;

namespace Vms.Domain.Infrastructure.Services;

public class DesignTimeUserProvider : IUserProvider
{
    public string UserId => string.Empty;

    public string TenantId => "DES001";

    public string UserName => "Test User Name";
    public string EmailAddress => "markb@utopiasoftware.co.uk";
}
