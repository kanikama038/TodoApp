using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;

namespace TodoApp.Services
{
    public class GmailEmailService
    {
        private readonly GmailAuthService _authService;

        public GmailEmailService(GmailAuthService authService)
        {
            _authService = authService;
        }

        public async Task SendAsync(string to, string subject, string bodyHtml)
        {
            var service = await _authService.GetGmailServiceAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NextSpark", "noreply.nextspark@gmail.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = bodyHtml };

            using var stream = new MemoryStream();
            await message.WriteToAsync(stream);
            var rawMessage = Convert.ToBase64String(stream.ToArray())
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            var gmailMessage = new Message
            {
                Raw = rawMessage
            };

            await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
        }
    }

}
