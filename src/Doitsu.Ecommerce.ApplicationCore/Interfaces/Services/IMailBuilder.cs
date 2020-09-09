using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services
{
    public interface IMailBuilder
    {
        Task<MessagePayload> PrepareCustomerOrderConfirmationMail<TData>(TData data);
    }
}