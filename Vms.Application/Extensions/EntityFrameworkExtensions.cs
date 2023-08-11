namespace Vms.Application.Extensions;

public static class EntityFrameworkExtensions
{
    public static void ThrowIfNoTransaction(this DbContext context)
    {
        if (context.Database.CurrentTransaction is null)
            throw new InvalidOperationException("A transaction is required.");
    }
}
