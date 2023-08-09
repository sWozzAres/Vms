using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Web.Shared;

public record DriverListDto(Guid Id, string FullName);
public enum DriverListOptions
{
    All = 0,
}