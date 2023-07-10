using System.Runtime.Serialization;

namespace Vms.Domain.Exceptions;

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
