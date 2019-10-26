using System;
using System.Threading.Tasks;

using Doitsu.Service.Core;
using Doitsu.Service.Core.Services.EmailService;
using Doitsu.Ecommerce.Core.ViewModels;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICustomerFeedbackService : IBaseService<CustomerFeedbacks>
    {
        Task<CustomerFeedbacks> CreateWithConstraintAsync(CustomerFeedbackViewModel data, int userId);
    }

    public class CustomerFeedbackService : BaseService<CustomerFeedbacks>, ICustomerFeedbackService
    {
        private readonly IEmailService emailService;
        private readonly IBrandService brandService;
        private readonly LeaderMail leaderMail;

        public CustomerFeedbackService(EcommerceDbContext dbContext, 
        IMapper mapper,
        ILogger<BaseService<CustomerFeedbacks>> logger, 
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
            await Task.WhenAll(SendEmailToCustomer(data), SendEmailToLeader(data));
            return result;
        }

        private async Task SendEmailToCustomer(CustomerFeedbackViewModel data) 
        {
            var currentBrand = await brandService.FirstOrDefaultAsync();
            var subject = $"[{currentBrand.Name}] Xác nhận thông tin yêu cầu liên hệ - {DateTime.Now.ToString("dd/MM/yyyy")}";
            var mailPayloadInfor = new MailPayloadInformation
            {
                Mail = data.Email,
                Name = data.CustomerName
            };
            
            var body = "<p>";
            body += "Thông tin yêu cầu liên hệ của bạn:<br/>";
            body += $"Số điện thoại: {data.Phone}<br/>";
            body += $"Địa chỉ email: {data.Email}<br/>";
            body += $"{data.Content.Replace("\n", "<br/>")}<br/>";
            body += "Chúng tôi sẽ xử lý yêu cầu và liên lạc với quý khách.<br/>";
            body += "</p>";
            emailService.SendEmailWithBachMocWrapper(subject, body, mailPayloadInfor);
        }

        private async Task SendEmailToLeader(CustomerFeedbackViewModel data) 
        {
            var currentBrand = await brandService.FirstOrDefaultAsync();
            var subject = $"[{currentBrand.Name}] Thông tin yêu cầu liên hệ mới - {data.Phone} - {DateTime.Now.ToString("dd/MM/yyyy")}";
            var mailPayloadInfor = new MailPayloadInformation
            {
                Mail = leaderMail.Mail,
                Name = leaderMail.Name
            };

            var body = "<p>";
            body += $"Có 1 thông tin yêu cầu liên hệ mới.<br/>Thông tin yêu cầu liên hệ của số điện thoại {data.Phone} có nội dung:<br/>";
            body += $"Địa chỉ email: {data.Email}<br/>";
            body += $"{data.Content.Replace("\n", "<br/>")}<br/>";
            body += "</p>";
            emailService.SendEmailWithBachMocWrapper(subject, body, mailPayloadInfor);
        }
    }
}