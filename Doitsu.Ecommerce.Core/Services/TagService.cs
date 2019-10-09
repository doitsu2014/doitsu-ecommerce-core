using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ITagService : IBaseService<Tag>
    {
        Task<ImmutableList<TagViewModel>> GetTopTagsAsync(int limit, int cachingMinutes = 30);
    }

    public class TagService : BaseService<Tag>, ITagService
    {
        private readonly IMemoryCache memoryCache;

        public TagService(IUnitOfWork unitOfWork, ILogger<BaseService<Tag>> logger, IMemoryCache memoryCache) : base(unitOfWork, logger)
        {
            this.memoryCache = memoryCache;
        }

        public async Task<ImmutableList<TagViewModel>> GetTopTagsAsync(int limit, int cachingMinutes)
        {
            var key = $"{Constants.CacheKey.TOP_TAGS}_{limit}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<TagViewModel> topTags))
            {
                var tags = this.GetAll().OrderByDescending(x => x.BlogTags.Count).Skip(0).Take(limit);
                var result = await tags.ProjectTo<TagViewModel>(this.UnitOfWork.Mapper.ConfigurationProvider).ToListAsync();
                topTags = result.ToImmutableList();
                memoryCache.Set(key, topTags, TimeSpan.FromMinutes(cachingMinutes));
            }
            return topTags;
        }
    }
}
