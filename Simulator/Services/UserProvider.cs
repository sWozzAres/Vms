using Utopia.Api.Services;

namespace Simulator.Services;

public class UserProvider : IUserProvider
{
    public string UserId => "simulator";

    public string TenantId => "TEST001";

    public string UserName => "Simulator";
    public string EmailAddress => "markb@utopiasoftware.co.uk";
}
