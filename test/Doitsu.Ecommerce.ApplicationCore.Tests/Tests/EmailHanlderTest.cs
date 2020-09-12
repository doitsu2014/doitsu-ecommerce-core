using Xunit;
using Xunit.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;

namespace Doitsu.Ecommerce.ApplicationCore.Tests
{
    [Collection("EcommerceCoreCollection")]
    public class EmailHanlderTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(EmailHanlderTest);
        public EmailHanlderTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        private async Task Test_SendMultipleEmailAndMailBuilder()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var mailBuilder = scope.ServiceProvider.GetService<IMailBuilder>();
                    var smtpEmailServerHandler = scope.ServiceProvider.GetService<ISmtpEmailServerHandler>();
                    var mailTemplateConfiguration = scope.ServiceProvider.GetService<IOptions<MailTemplateConfiguration>>().Value;
                    var sendMailModel = new SendMailModel() { OrderCode = "AXP28319248129" };
                    var result = await mailBuilder.PrepareMessagePayloadAsync<SendMailModel>(mailTemplateConfiguration.CustomerOrderConfirmationMailTemplate,
                                                                                             sendMailModel,
                                                                                             $"[Doitsu.Ecommerce.ApplicationCore.Test] Bạn đã vừa hoàn thành đơn hàng #{sendMailModel.OrderCode}",
                                                                                             "thd1152016@gmail.com",
                                                                                             "DucTH Destination Name");
                    await smtpEmailServerHandler.SendEmailMultiplePayloadAsync(new List<MessagePayload>() { result }, CancellationToken.None);
                    Assert.True(true);
                }
            }
        }
    }
}