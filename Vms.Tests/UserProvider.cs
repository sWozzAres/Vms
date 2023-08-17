using Utopia.Api.Services;

namespace Vms.Tests;

public class UserProvider : IUserProvider
{
    public string UserId => string.Empty;

    public string TenantId => "TEST001";

    public string UserName => "Test";
    public string EmailAddress => "markb@utopiasoftware.co.uk";
}
