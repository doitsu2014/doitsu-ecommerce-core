using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Xunit.Abstractions;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Doitsu.Ecommerce.Infrastructure.Data;
using Doitsu.Ecommerce.ApplicationCore.Services.IdentityManagers;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore;
using System;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Microsoft.Extensions.Options;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using System.IO;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.RazorPage;

namespace Doitsu.Ecommerce.ApplicationCore.Tests.Tests
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
                    var path = Path.Combine( smtpMailServerOptions.TemplateUrlInformation.OrderConfirmationUrl);
                    using (var reader = new StreamReader(path))
                    {
                        var body = await reader.ReadToEndAsync();
                        var sendMailModel = new SendMailModel() { OrderCode = "#239123ABI" };
                        var result = await renderer.RenderPartialToStringAsync<SendMailModel>(path, sendMailModel);
                        await smtpEmailServerHandler.SendEmailMultiplePayloadAsync(smtpMailServerOptions, new List<MessagePayload>() {});
                    }
                    var a = 5;

                }
            }
        }
    }
}