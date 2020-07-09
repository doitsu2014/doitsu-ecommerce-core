using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services
{
    public interface ISmtpEmailServerHandler
    {
        void SendEmail(SmtpMailServerOptions options, MessagePayload messagePayload);

        void SendEmailNonBlocking(SmtpMailServerOptions options, MessagePayload messagePayload, object userToken = null);

        Task SendEmailMultiplePayloadAsync(SmtpMailServerOptions options, List<MessagePayload> messagePayloads, CancellationToken token);
    }
}
