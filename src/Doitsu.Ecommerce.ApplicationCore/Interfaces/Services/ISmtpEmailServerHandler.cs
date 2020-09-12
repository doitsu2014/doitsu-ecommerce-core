using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services
{
    public interface ISmtpEmailServerHandler
    {
        void SendEmail(MessagePayload messagePayload);
        Task SendEmailMultiplePayloadAsync(List<MessagePayload> messagePayloads, CancellationToken token);
    }
}
