using Utopia.Api.Services;

namespace Vms.Tests.Services;

public class UserProvider : IUserProvider
{
    public string UserId => "tester";

    public string TenantId => "TEST001";

    public string UserName => "Test";
    public string EmailAddress => "markb@utopiasoftware.co.uk";
}
