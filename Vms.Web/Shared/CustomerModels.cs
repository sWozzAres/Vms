﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Web.Shared;

public record CustomerListDto(string CompanyCode, string Code, string Name);
public enum CustomerListOptions
{
    All = 0,
}