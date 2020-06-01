using System;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.Core.Services.Interface
{
    public interface ICacheManager
    {
        Task SetAsync(string key, byte[] content, TimeSpan expiration);
        Task<byte[]> GetAsync(string key);
        Task FlushAsync(string prefix = null);
    }
}