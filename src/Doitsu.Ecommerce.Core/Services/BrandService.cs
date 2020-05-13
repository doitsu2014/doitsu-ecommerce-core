using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;

using AutoMapper;

using Doitsu.Ecommerce.Core.Abstraction;

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Service.Core;

using Microsoft.Extensions.Logging;

using Optional;
using Optional.Async;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<Option<Brand, string>> SettingAsync(BrandViewModel data);
    }

    public class BrandService : BaseService<Brand>, IBrandService
    {
        public BrandService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<Brand, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        { }

        public async Task<Option<Brand, string>> SettingAsync(BrandViewModel data)
        {
            return await data
                .SomeNotNull()
                .WithException("Dữ liệu cấu hình không tồn tại.")
                .Filter(x => !x.Name.IsNullOrEmpty(), "Tên công ty không được bỏ trống.")
                .MapAsync(async x =>
                {
                    var exist = await FindByKeysAsync(x.Id);
                    if (exist != null)
                    {
                        var result = Update( this.Mapper.Map(x, exist));
                        await CommitAsync();
                        return result;
                    }
                    else
                    {
                        var result = await CreateAsync<BrandViewModel>(x);
                        await CommitAsync();
                        return result;
                    }
                });
        }
    }
}