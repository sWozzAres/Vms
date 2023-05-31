using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

    protected VmsDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
