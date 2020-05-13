using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Doitsu.Service.Core.Services.EmailService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Optional;
using Optional.Async;

using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Microsoft.AspNetCore.Http;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IEmailService
    {
        Option<bool, string> SendEmailWithBachMocWrapper(string subject, string content, MailPayloadInformation destEmail);
        Option<bool, string> SendEmailWithYgflWrapper(string subject, string content, MailPayloadInformation destEmail);
        Task<Option<bool, string>> SendEmailWithBachMocWrapperAsync(List<MessagePayload> messagePayloads);
        Task<Option<bool, string>> SendEmailWithYgflWrapperAsync(List<MessagePayload> messagePayloads);
        Task<MessagePayload> PrepareLeaderOrderMailConfirmAsync(EcommerceIdentityUser user, Orders order);
        Task<MessagePayload> PrepareCustomerOrderMailConfirm(EcommerceIdentityUser user, Orders order);
        Task<MessagePayload> PrepareCustomerFeedback(CustomerFeedbackViewModel data);
        Task<MessagePayload> PrepareLeaderCustomerFeedback(CustomerFeedbackViewModel data);
    }

    public class EmailService : IEmailService
    {
        private readonly IOptionsMonitor<SmtpMailServerOptions> smtpMailServerOptionsMonitor;
        private readonly ISmtpEmailServerHandler smtpEmailServerHandler;
        private readonly ILogger<EmailService> logger;
        private readonly IBrandService brandService;
        private readonly LeaderMail leaderMailOption;
        private readonly IHttpContextAccessor httpContextAccessor;


        public EmailService(IOptionsMonitor<SmtpMailServerOptions> smtpMailServerOptionsMonitor,
                            ILogger<EmailService> logger,
                            ISmtpEmailServerHandler smtpEmailServerHandler,
                            IBrandService brandService,
                            IOptionsMonitor<LeaderMail> leaderMailOption,
                            IHttpContextAccessor httpContextAccessor)
        {
            this.smtpMailServerOptionsMonitor = smtpMailServerOptionsMonitor;
            this.logger = logger;
            this.smtpEmailServerHandler = smtpEmailServerHandler;
            this.brandService = brandService;
            this.leaderMailOption = leaderMailOption.CurrentValue;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Option<bool, string> SendEmailWithBachMocWrapper(string subject, string content, MailPayloadInformation destEmail)
        {
            return new
            {
                subject,
                content,
                destEmail
            }
                .SomeNotNull()
                .WithException(string.Empty)
                .Filter(d => d.destEmail != null, "Không rõ địa chỉ người cần gửi mail.")
                .Filter(d => !d.content.IsNullOrEmpty(), "Nội dung gửi mail rỗng")
                .Filter(d => !d.subject.IsNullOrEmpty(), "Tiêu đề gửi mail rỗng")
                .Map(d =>
                {
                    try
                    {
                        var smtpMailServerOptions = smtpMailServerOptionsMonitor.CurrentValue;
                        var messagePayload = new MessagePayload()
                        {
                            Subject = subject,
                            DestEmail = destEmail
                        };
                        var body = $"<p>Kính gửi {messagePayload.DestEmail.Name},</p>";
                        body += content;
                        body += $"<p>Xin chân thành cảm ơn quý khách,</p>";
                        body += $"<p>________________________________</p>";
                        body += $"<p>Công ty TNHH Bách Mộc</p>";
                        messagePayload.Body = body;
                        this.smtpEmailServerHandler.SendEmail(smtpMailServerOptions, messagePayload);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Email service exception: {ex}", ex);
                        return false;
                    }
                });
        }
        public async Task<Option<bool, string>> SendEmailWithBachMocWrapperAsync(List<MessagePayload> messagePayloads)
        {
            return await messagePayloads
                .SomeNotNull()
                .WithException("List mail để gửi đi rỗng. Không thể gửi.")
                .Filter(d => d.All(x => x != null), "Có 1 số mail bị rỗng. Không thể gửi.")
                .Filter(d => d.All(x => !x.Subject.IsNullOrEmpty()), "Các mail gửi đi, có 1 số mail không rõ tiêu đề.")
                .Filter(d => d.All(x => !x.Body.IsNullOrEmpty()), "Các mail gửi đi, có 1 số mail không rõ nội dung.")
                .Filter(d => d.All(x => x.DestEmail != null), "Các mail gửi đi, có 1 số mail không rõ địa chỉ.")
                .MapAsync(async d =>
                {
                    try
                    {
                        foreach (var messagePayload in messagePayloads)
                        {
                            var body = $"<p>Kính gửi {messagePayload.DestEmail.Name},</p>";
                            body += messagePayload.Body;
                            body += $"<p>Xin chân thành cảm ơn quý khách,</p>";
                            body += $"<p>________________________________</p>";
                            body += $"<p>Công ty TNHH Bách Mộc</p>";
                            messagePayload.Body = body;
                        }

                        var smtpMailServerOptions = smtpMailServerOptionsMonitor.CurrentValue;
                        await this.smtpEmailServerHandler.SendEmailMultiplePayloadAsync(smtpMailServerOptions, messagePayloads, CancellationToken.None);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"{nameof(EmailService)} exception: ", ex);
                        return false;
                    }
                });
        }
        
        public async Task<MessagePayload> PrepareCustomerOrderMailConfirm(EcommerceIdentityUser user, Orders order)
        {
            try
            {
                var currentBrand = await brandService.FirstOrDefaultAsync();
                var subject = $"[{currentBrand.Name}] XÁC NHẬN ĐƠN HÀNG #{order.Code} - {DateTime.UtcNow.ToVietnamDateTime().ToShortDateString()}";
                var destPayload = new MailPayloadInformation
                {
                    Mail = user.Email,
                    Name = user.Fullname
                };

                var body = $"<p>Bạn đã đặt thành công đơn hàng có mã đơn: <a href='{CreateHostWithScheme()}/nguoi-dung/thong-tin-tai-khoan'>#{order.Code}</a></p>";
                body += $"<p>Để có thể xem chi tiết đơn hàng, mong quý khách nhấp vào đường dẫn phía trên.</p><br/>";

                var messagePayload = new MessagePayload();
                messagePayload.Subject = subject;
                messagePayload.Body = body;
                messagePayload.DestEmail = destPayload;
                return messagePayload;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cannot send email to confirm Order Code {order.Code} of {user.Fullname} with id {user.Id}");
                return null;
            }
        }

        public async Task<MessagePayload> PrepareLeaderOrderMailConfirmAsync(EcommerceIdentityUser user, Orders order)
        {
            try
            {
                var currentBrand = await brandService.FirstOrDefaultAsync();
                var subject = $"[{currentBrand.Name}] ĐƠN HÀNG MỚI #{order.Code} - {DateTime.UtcNow.ToVietnamDateTime().ToShortDateString()}";
                var destPayload = new MailPayloadInformation
                {
                    Mail = leaderMailOption.Mail,
                    Name = leaderMailOption.Name
                };
                var body = "<p>";
                body += "Có đơn đặt hàng vào hệ thống. Thông tin chi tiết:<br/>";
                body += $"Người đặt: {user.Fullname}<br/>";
                body += $"Email: {user.Email}<br/>";
                body += $"Số điện thoại: {user.PhoneNumber}<br/>";
                body += $"Địa chỉ: {user.Address}<br/>";
                body += $"Mã đơn: #{order.Code}<br/>";
                body += $"Ngày đặt: {DateTime.UtcNow.ToVietnamDateTime().ToShortDateString()}<br/>";
                body += $"Tổng tiền: {order.FinalPrice}";
                body += "</p>";
                body += $"<p>Hãy vào trang quản lý để xem thông tin chi tiết: <a href='{CreateHostWithScheme()}/Admin'>Trang Quản Lý</a></p><br/>";

                var messagePayload = new MessagePayload();
                messagePayload.Subject = subject;
                messagePayload.Body = body;
                messagePayload.DestEmail = destPayload;

                return messagePayload;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"PrepareLeaderOrderMailConfirmAsync: cannot prepare mail to confirm Order Code {order.Code} of {user.Fullname} with id {user.Id}");
                return null;
            }
        }
        
        public async Task<MessagePayload> PrepareCustomerFeedback(CustomerFeedbackViewModel data)
        {
            try
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
                var messagePayload = new MessagePayload();
                messagePayload.Subject = subject;
                messagePayload.Body = body;
                messagePayload.DestEmail = mailPayloadInfor;

                return messagePayload;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"PrepareLeaderOrderMailConfirmAsync:Cannot send email to confirm Customer Feedback");
                return null;
            }
        }

        public async Task<MessagePayload> PrepareLeaderCustomerFeedback(CustomerFeedbackViewModel data)
        {
            try
            {
                var currentBrand = await brandService.FirstOrDefaultAsync();
                var subject = $"[{currentBrand.Name}] Thông tin yêu cầu liên hệ mới - {data.Phone} - {DateTime.Now.ToString("dd/MM/yyyy")}";
                var mailPayloadInfor = new MailPayloadInformation
                {
                    Mail = leaderMailOption.Mail,
                    Name = leaderMailOption.Name
                };

                var body = "<p>";
                body += $"Có 1 thông tin yêu cầu liên hệ mới.<br/>Thông tin yêu cầu liên hệ của số điện thoại {data.Phone} có nội dung:<br/>";
                body += $"Địa chỉ email: {data.Email}<br/>";
                body += $"{data.Content.Replace("\n", "<br/>")}<br/>";
                body += "</p>";

                var messagePayload = new MessagePayload();
                messagePayload.Subject = subject;
                messagePayload.Body = body;
                messagePayload.DestEmail = mailPayloadInfor;
                return messagePayload;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cannot send email to confirm Customer Feedback");
                return null;
            }
        }

        public Option<bool, string> SendEmailWithYgflWrapper(string subject, string content, MailPayloadInformation destEmail)
        {
            throw new NotImplementedException();
        }

        public Task<Option<bool, string>> SendEmailWithYgflWrapperAsync(List<MessagePayload> messagePayloads)
        {
            throw new NotImplementedException();
        }

        private string CreateHostWithScheme() => httpContextAccessor.HttpContext != null ? $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}" : "http://null";
    }
}