using System;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBrandService : IBaseService<Brand>
    {
        Task<Option<Brand, string>> SettingAsync(BrandViewModel data);
    }

    public class BrandService : BaseService<Brand>, IBrandService
    {
        public BrandService(IEcommerceUnitOfWork unitOfWork, ILogger<BaseService<Brand>> logger) : base(unitOfWork, logger)
        {
        }

        public async Task<Option<Brand, string>> SettingAsync(BrandViewModel data)
        {
            return await data
                .SomeNotNull()
                .WithException("Dữ liệu cấu hình không tồn tại.")
                .Filter(x => !x.Name.IsNullOrEmpty(), "Tên công ty không được bỏ trống.")
                .MapAsync(async x =>
                {
                    var exist = await FirstOrDefaultAsync(comp => comp.Id == x.Id);
                    if (exist != null)
                    {
                        var result = Update<BrandViewModel>(x);
                        await this.UnitOfWork.CommitAsync();
                        return result;
                    }
                    else
                    {
                        var result = await CreateAsync<BrandViewModel>(data);
                        await this.UnitOfWork.CommitAsync();
                        return result;
                    }
                });
        }
    }
}
