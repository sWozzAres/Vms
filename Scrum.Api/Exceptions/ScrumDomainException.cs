namespace Scrum.Api.Exceptions;

public class ScrumDomainException : Exception
{
    public ScrumDomainException()
    {
    }

    public ScrumDomainException(string? message) : base(message)
    {
    }

    public ScrumDomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
