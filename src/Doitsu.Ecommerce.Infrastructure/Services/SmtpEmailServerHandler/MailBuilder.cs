using System.IO;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.RazorPage;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using Doitsu.Ecommerce.Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Doitsu.Ecommerce.Infrastructure.Services.SmtpEmailServerHandler
{
    public class MailBuilder : IMailBuilder
    {
        private readonly IRazorPageRenderer renderer;
        private readonly IWebHostEnvironment env;
        private readonly SmtpMailServerOptions smtpMailServerOptions;

        public MailBuilder(
            IRazorPageRenderer renderer,
            IOptionsMonitor<SmtpMailServerOptions> optionsMonitor,
            IWebHostEnvironment env)
        {
            this.renderer = renderer;
            this.env = env;
            this.smtpMailServerOptions = optionsMonitor.CurrentValue;
        }

        public async Task<MessagePayload> PrepareCustomerOrderConfirmationMail<TData>(TData data, string customerMail, string customerName, string subject)
        {
            var path = Path.Combine(smtpMailServerOptions.TemplateUrlInformation.OrderConfirmationTemplateUrl);
            var content = await renderer.RenderPartialToStringAsync<TData>(path, data);
            return new MessagePayload()
            {
                Body = content,
                DestEmail = new MailPayloadInformation()
                {
                    Name = customerName,
                    Mail = customerMail
                },
                Subject = subject
            };
        }
    }
}