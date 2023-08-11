namespace Vms.Application.Services;

public interface IEmailSender
{
    void Send(string recipients, string subject, string body);
    void Send(IEnumerable<string> recipients, string subject, string body);
}

public class EmailSender(VmsDbContext context) : IEmailSender
{
    readonly VmsDbContext _context = context;

    public void Send(string recipients, string subject, string body)
    {
        var email = new Email(recipients, subject, body);
        _context.Emails.Add(email);
    }
    public void Send(IEnumerable<string> recipients, string subject, string body)
    {
        foreach (var recipient in recipients)
        {
            Send(recipient, subject, body);
        }
    }
}
