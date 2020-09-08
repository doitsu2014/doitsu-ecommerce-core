using Xunit;
using Xunit.Abstractions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.RazorPage;

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
        private async Task Test_CreateBlogWithTags()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var renderer = scope.ServiceProvider.GetService<IRazorPageRenderer>();
                    var smtpEmailServerHandler = scope.ServiceProvider.GetService<ISmtpEmailServerHandler>();
                    var optionsMonitor = scope.ServiceProvider.GetService<IOptionsMonitor<SmtpMailServerOptions>>();
                    var smtpMailServerOptions = optionsMonitor.CurrentValue;

                    var env = scope.ServiceProvider.GetService<IWebHostEnvironment>();
                    var path = Path.Combine(smtpMailServerOptions.TemplateUrlInformation.OrderConfirmationUrl);
                    var sendMailModel = new SendMailModel() { OrderCode = "#239123ABI" };
                    var result = await renderer.RenderPartialToStringAsync<SendMailModel>(path, sendMailModel);
                    await smtpEmailServerHandler.SendEmailMultiplePayloadAsync(smtpMailServerOptions, new List<MessagePayload>()
                        {
                            new MessagePayload()
                            {
                                DestEmail = new MailPayloadInformation()
                                {
                                    Mail = "thd1152016@gmail.com",
                                    Name = "DucTH Destination Tester"
                                },
                                Body = result,
                                Subject = $"[Doitsu.Ecommerce.ApplicationCore.Test] Test Sending Email {DateTime.Now.ToString("dd/MM/yyyy")}"
                            }

                        }, CancellationToken.None);
                    Assert.True(true);
                }
            }
        }
    }
}