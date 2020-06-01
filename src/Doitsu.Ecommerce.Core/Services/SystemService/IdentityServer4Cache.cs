using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Services.Interface;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.Services.SystemService
{
    public class IdentityServer4Cache<T> : ICache<T> where T : class
    {
        private readonly ICacheManager _cacheManager;
        private readonly ILogger<IdentityServer4Cache<T>> _logger;

        public IdentityServer4Cache(ILogger<IdentityServer4Cache<T>> logger, ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _logger = logger;
        }

        public async Task<T> GetAsync(string key)
        {
            _logger.LogDebug("GetAsync, Key = {Key}", key);
            var content = await _cacheManager.GetAsync(key);
            if (content == null) return null;

            var item = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(content));
            if (item != null && item is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext())
            {
                return null;
            }

            return item;
        }

        public async Task SetAsync(string key, T item, TimeSpan expiration)
        {
            _logger.LogDebug("SetAsync, Key = {Key}, expiration = {Expiration} seconds", key, expiration.TotalSeconds);
            if (item == null) return;

            var content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
            await _cacheManager.SetAsync(key, content, expiration);
        }
    }
}