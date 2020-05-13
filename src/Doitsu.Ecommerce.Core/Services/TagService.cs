using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ITagService : IBaseService<Tag>
    {
        Task<ImmutableList<TagViewModel>> GetTopTagsAsync(int limit, int cachingMinutes = 30);
        Task<ImmutableList<TagViewModel>> ExceptNotExistName(List<string> titles);

    }

    public class TagService : BaseService<Tag>, ITagService
    {
        private readonly IMemoryCache memoryCache;

        public TagService(EcommerceDbContext dbContext,
                          IMapper mapper,
                          ILogger<BaseService<Tag, EcommerceDbContext>> logger,
                          IMemoryCache memoryCache) : base(dbContext, mapper, logger)
        {
              this.memoryCache = memoryCache;
        }

        public async Task<ImmutableList<TagViewModel>> ExceptNotExistName(List<string> titles)
        {  
            var allExistTag = await this
                .Get(x => titles.Contains(x.Title))
                .ProjectTo<TagViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return allExistTag.ToImmutableList();
        }

        public async Task<ImmutableList<TagViewModel>> GetTopTagsAsync(int limit, int cachingMinutes)
        {
            var key = $"{Constants.CacheKey.TOP_TAGS}_{limit}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<TagViewModel> topTags))
            {
                var tags = this.GetAll().OrderByDescending(x => x.BlogTags.Count).Skip(0).Take(limit);
                var result = await tags.ProjectTo<TagViewModel>(Mapper.ConfigurationProvider).ToListAsync();
                topTags = result.ToImmutableList();
                memoryCache.Set(key, topTags, TimeSpan.FromMinutes(cachingMinutes));
            }
            return topTags;
        }
    }
}
