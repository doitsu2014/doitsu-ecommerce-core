using System;
using System.Threading.Tasks;

using Doitsu.Service.Core;
using Doitsu.Service.Core.Services.EmailService;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using System.Collections.Immutable;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICustomerFeedbackService : IEcommerceBaseService<CustomerFeedbacks>
    {
        Task<CustomerFeedbacks> CreateWithConstraintAsync(CustomerFeedbackViewModel data, int userId);
        Task<ImmutableList<CustomerFeedbackOverviewViewModel>> GetAllByTypeAsync(CustomerFeedBackTypeEnum type);
    }

    public class CustomerFeedbackService : EcommerceBaseService<CustomerFeedbacks>, ICustomerFeedbackService
    {
        private readonly IEmailService emailService;
        private readonly IBrandService brandService;
        private readonly LeaderMail leaderMail;

        public CustomerFeedbackService(EcommerceDbContext dbContext,
        IMapper mapper,
        ILogger<EcommerceBaseService<CustomerFeedbacks>> logger,
        IEmailService emailService,
        IBrandService brandService,
        IOptionsMonitor<LeaderMail> leaderMail) : base(dbContext, mapper, logger)
        {
            this.emailService = emailService;
            this.brandService = brandService;
            this.leaderMail = leaderMail.CurrentValue;
        }

        public async Task<CustomerFeedbacks> CreateWithConstraintAsync(CustomerFeedbackViewModel data, int userId)
        {
            var exist = await this.FirstOrDefaultAsync(x => x.Email == data.Email);
            var result = await this.CreateAsync<CustomerFeedbackViewModel>(data);
            if (userId > 0)
            {
                result.UserId = userId;
            }
            await CommitAsync();

            var messagePayloads = new List<MessagePayload>()
            {
                await emailService.PrepareCustomerFeedback(data),
                await emailService.PrepareLeaderCustomerFeedback(data)
            };

            await emailService.SendEmailWithBachMocWrapperAsync(messagePayloads);
            return result;
        }
        public async Task<ImmutableList<CustomerFeedbackOverviewViewModel>> GetAllByTypeAsync(CustomerFeedBackTypeEnum type)
        {
            var typeInteger = (int)type;
            var result = await this
                  .Get(x => x.Type == typeInteger)
                  .OrderByDescending(cf => cf.CreatedDate)
                  .ProjectTo<CustomerFeedbackOverviewViewModel>(Mapper.ConfigurationProvider)
                  .ToListAsync();

            return result.ToImmutableList();
        }
    }
}