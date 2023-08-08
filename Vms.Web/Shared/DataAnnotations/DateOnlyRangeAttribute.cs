namespace Vms.Web.Shared.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
     AllowMultiple = false)]
public class DateOnlyRangeAttribute(int minimumYear, int minimumMonth, int minimumDay,
    int maximumYear, int maximumMonth, int maximumDay, bool allowNull = false) : ValidationAttribute
{
    public DateOnly Minimum { get; set; } = new DateOnly(minimumYear, minimumMonth, minimumDay);
    public DateOnly Maximum { get; set; } = new DateOnly(maximumYear, maximumMonth, maximumDay);
    public bool AllowNull { get; set; } = allowNull;

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