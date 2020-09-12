using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services
{
    public interface IMailBuilder
    {
        Task<MessagePayload> PrepareMessagePayloadAsync<TData>(MailTemplate mailTemplate, TData data, string subject, string destinationMail, string destinationName);
    }
}