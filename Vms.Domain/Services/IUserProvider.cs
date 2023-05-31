using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Domain.Services;

public interface IUserProvider
{
    Guid UserId { get; }
    Guid TenantId { get; }
}
