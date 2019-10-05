using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Doitsu.Service.Core.Services.EmailService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IEmailService
    {
        Option<bool, string> SendEmailWithBachMocWrapper(string subject, string content, MailPayloadInformation destEmail);
        Task<Option<bool, string>> SendEmailWithBachMocWrapperAsync(List<MessagePayload> messagePayloads);
    }

    public class EmailService : IEmailService
    {
        private readonly IOptionsMonitor<SmtpMailServerOptions> smtpMailServerOptionsMonitor;
        private readonly ISmtpEmailServerHandler smtpEmailServerHandler;
        private readonly ILogger<EmailService> logger;

        public EmailService(IOptionsMonitor<SmtpMailServerOptions> smtpMailServerOptionsMonitor, ILogger<EmailService> logger, ISmtpEmailServerHandler smtpEmailServerHandler)
        {
            this.smtpMailServerOptionsMonitor = smtpMailServerOptionsMonitor;
            this.logger = logger;
            this.smtpEmailServerHandler = smtpEmailServerHandler;
        }

        public Option<bool, string> SendEmailWithBachMocWrapper(string subject, string content, MailPayloadInformation destEmail)
        {
            return new
                {
                    subject,
                    content,
                    destEmail
                }
                .SomeNotNull()
                .WithException(string.Empty)
                .Filter(d => d.destEmail != null, "Không rõ địa chỉ người cần gửi mail.")
                .Filter(d => !d.content.IsNullOrEmpty(), "Nội dung gửi mail rỗng")
                .Filter(d => !d.subject.IsNullOrEmpty(), "Tiêu đề gửi mail rỗng")
                .Map(d =>
                {
                    try
                    {
                        var smtpMailServerOptions = smtpMailServerOptionsMonitor.CurrentValue;
                        var messagePayload = new MessagePayload()
                        {
                            Subject = subject,
                            DestEmail = destEmail
                        };
                        var body = $"<p>Kính gửi {messagePayload.DestEmail.Name},</p>";
                        body += content;
                        body += $"<p>Xin chân thành cảm ơn quý khách,</p>";
                        body += $"<p>________________________________</p>";
                        body += $"<p>Công ty TNHH Bách Mộc</p>";
                        messagePayload.Body = body;
                        this.smtpEmailServerHandler.SendEmail(smtpMailServerOptions, messagePayload);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"{nameof(EmailService)} exception: ", ex);
                        return false;
                    }
                });
        }

        public async Task<Option<bool, string>> SendEmailWithBachMocWrapperAsync(List<MessagePayload> messagePayloads)
        {
            return await messagePayloads
                .SomeNotNull()
                .WithException("List mail để gửi đi rỗng. Không thể gửi.")
                .Filter(d => d.All(x => x != null), "Có 1 số mail bị rỗng. Không thể gửi.")
                .Filter(d => d.All(x => !x.Subject.IsNullOrEmpty()), "Các mail gửi đi, có 1 số mail không rõ tiêu đề.")
                .Filter(d => d.All(x => !x.Body.IsNullOrEmpty()), "Các mail gửi đi, có 1 số mail không rõ nội dung.")
                .Filter(d => d.All(x => x.DestEmail != null), "Các mail gửi đi, có 1 số mail không rõ địa chỉ.")
                .MapAsync(async d =>
                {
                    try
                    {
                        foreach (var messagePayload in messagePayloads)
                        {
                            var body = $"<p>Kính gửi {messagePayload.DestEmail.Name},</p>";
                            body += messagePayload.Body;
                            body += $"<p>Xin chân thành cảm ơn quý khách,</p>";
                            body += $"<p>________________________________</p>";
                            body += $"<p>Công ty TNHH Bách Mộc</p>";
                            messagePayload.Body = body;
                        }

                        var smtpMailServerOptions = smtpMailServerOptionsMonitor.CurrentValue;
                        await this.smtpEmailServerHandler.SendEmailOldAsync(smtpMailServerOptions, messagePayloads, CancellationToken.None);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"{nameof(EmailService)} exception: ", ex);
                        return false;
                    }
                });
        }
    }
}