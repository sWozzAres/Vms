using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vms.Web.Shared.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
     AllowMultiple = false)]
public class DateOnlyRangeAttribute : ValidationAttribute
{
    public DateOnly Minimum { get; set; }
    public DateOnly Maximum { get; set; }
    public bool AllowNull { get; set; }

    public DateOnlyRangeAttribute(int minimumYear, int minimumMonth, int minimumDay, 
        int maximumYear, int maximumMonth, int maximumDay, bool allowNull = false)
    {
        Minimum = new DateOnly(minimumYear, minimumMonth, minimumDay);
        Maximum = new DateOnly(maximumYear, maximumMonth, maximumDay);
        AllowNull = allowNull;
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return AllowNull;
        }
        else if (value is DateOnly date)
        {
            return date >= Minimum && date <= Maximum;
        }
        else
            return false;
    }
}