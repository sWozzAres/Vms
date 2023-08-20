namespace Vms.Application.Services;

public interface IEmailSender<TContext> where TContext : ISystemContext
{
    void Send(string recipients, string subject, string body);
    void Send(IEnumerable<string> recipients, string subject, string body);
}

public class EmailSender<TContext>(TContext context) : IEmailSender<TContext> where TContext : ISystemContext
{
    public void Send(string recipients, string subject, string body)
    {
        var email = new Email(recipients, subject, body);
        context.Emails.Add(email);
    }
    public void Send(IEnumerable<string> recipients, string subject, string body)
    {
        foreach (var recipient in recipients)
        {
            Send(recipient, subject, body);
        }
    }
}
