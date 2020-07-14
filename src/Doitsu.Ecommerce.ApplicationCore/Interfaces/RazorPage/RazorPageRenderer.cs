using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.RazorPage
{
    public interface IRazorPageRenderer
    {
        Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
    }
}