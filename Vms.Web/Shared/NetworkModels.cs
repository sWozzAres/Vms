using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Web.Shared;

public record NetworkListDto(string CompanyCode, string Code, string Name);
public enum NetworkListOptions
{
    All = 0,
}