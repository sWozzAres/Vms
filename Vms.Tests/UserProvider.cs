using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vms.Domain.Services;

namespace Vms.Tests;

public class UserProvider : IUserProvider
{
    public string UserId => string.Empty;

    public string TenantId => "TEST001";
}
