namespace Vms.Domain.Core.Exceptions;

public class VmsDomainException : Exception
{
    public VmsDomainException()
    {
    }

    public VmsDomainException(string? message) : base(message)
    {
    }

    public VmsDomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
