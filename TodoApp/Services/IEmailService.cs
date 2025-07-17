// https://marunaka-blog.com/asp-net-core-mailkit/9860/

namespace TodoApp.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toMail, string subject, string text);
    }
}
