namespace Vms.Application.Extensions;

public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Throws an exception if the context is not in a transaction.
    /// </summary>
    public static void ThrowIfNoTransaction(this DbContext context)
    {
        if (context.Database.CurrentTransaction is null)
            throw new InvalidOperationException("A transaction is required.");
    }
}
