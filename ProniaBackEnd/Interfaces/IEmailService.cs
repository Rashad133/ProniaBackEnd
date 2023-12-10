namespace ProniaBackEnd.Interfaces
{
    public interface IEmailService
    {
        Task SendMailAsync(string emailto, string subject, string body, bool isHtml = false);
    }
}
