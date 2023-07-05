namespace Vms.DomainApplication.Services;

public interface IEmailSender
{
    void Send(string recipients, string subject, string body);
}

public class EmailSender(VmsDbContext context) : IEmailSender
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public void Send(string recipients, string subject, string body)
    {
        var email = new Email(recipients, subject, body);
        _context.Emails.Add(email);
    }
}
