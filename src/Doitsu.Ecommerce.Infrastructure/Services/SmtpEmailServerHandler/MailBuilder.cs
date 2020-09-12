using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.RazorPage;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Doitsu.Ecommerce.Infrastructure.Services.SmtpEmailServerHandler
{
    public class MailBuilder : IMailBuilder
    {
        private readonly IRazorPageRenderer renderer;
        private readonly IWebHostEnvironment env;

        public MailBuilder(
            IRazorPageRenderer renderer,
            IOptionsMonitor<SmtpMailServerOptions> optionsMonitor,
            IWebHostEnvironment env)
        {
            this.renderer = renderer;
            this.env = env;
        }

        public async Task<MessagePayload> PrepareMessagePayloadAsync<TData>(MailTemplate mailTemplate, TData data, string subject, string destinationMail, string destinationName)
        {
            var content = await renderer.RenderPartialToStringAsync<TData>(mailTemplate.Url, data);
            return new MessagePayload()
            {
                Body = content,
                DestEmails = new EmailAddressInformation[]
                {
                    new EmailAddressInformation()
                    {
                        Name = destinationName,
                        Mail = destinationMail
                    }
                },
                CcEmails = mailTemplate.CcEmails,
                BccEmails = mailTemplate.CcEmails,
                Subject = string.Format(subject)
            };
        }
    }
}