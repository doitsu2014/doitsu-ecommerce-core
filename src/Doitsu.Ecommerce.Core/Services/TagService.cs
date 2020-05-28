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
        Task<ImmutableList<TagViewModel>> GetTopBlogTagsAsync(int limit);
        Task<ImmutableList<TagViewModel>> ExceptNotExistName(List<string> titles);
    }

    public class TagService : BaseService<Tag>, ITagService
    {
        public TagService(EcommerceDbContext dbContext,
                          IMapper mapper,
                          ILogger<BaseService<Tag, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }

        public async Task<ImmutableList<TagViewModel>> ExceptNotExistName(List<string> titles)
        {
            var allExistTag = await this
                .Get(x => titles.Contains(x.Title))
                .ProjectTo<TagViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return allExistTag.ToImmutableList();
        }

        public async Task<ImmutableList<TagViewModel>> GetTopBlogTagsAsync(int limit)
        {
            var tags = this.GetAll().OrderByDescending(x => x.BlogTags.Count).Skip(0).Take(limit);
            var result = await tags.ProjectTo<TagViewModel>(Mapper.ConfigurationProvider).ToListAsync();
            return result.ToImmutableList();
        }
    }
}
