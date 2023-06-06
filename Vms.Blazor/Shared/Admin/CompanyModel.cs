using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Blazor.Shared.Admin;

public record CompanyModel(string Code, string Name);
public record CompanyListModel(string Code, string Name);