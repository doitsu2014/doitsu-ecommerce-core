using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Optional;
using Dasync.Collections;
using Optional.Async;
using Microsoft.Extensions.Logging;
using System;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services;
using Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels;
using Microsoft.Extensions.Options;

namespace Doitsu.Ecommerce.Infrastructure.Services.SmtpEmailServerHandler
{
    public class SmtpEmailServerHandler : ISmtpEmailServerHandler
    {
        private readonly ILogger<SmtpEmailServerHandler> logger;
        private readonly SmtpMailServerOptions options;

        public SmtpEmailServerHandler(ILogger<SmtpEmailServerHandler> logger,
            IOptionsMonitor<SmtpMailServerOptions> smtpMailServerOptions)
        {
            this.logger = logger;
            this.options = smtpMailServerOptions.CurrentValue;
        }

        private Option<MailMessage> PrepareMailMessage(MessagePayload messagePayload)
        {
            return (options, messagePayload).Some()
                .Map(data =>
                {
                    var message = new MailMessage();
                    message.From = new MailAddress(options.FromMail.Mail, options.FromMail.Name);

                    messagePayload.DestEmails.Select(x => new MailAddress(x.Mail, x.Name)).ToList().ForEach(x => message.To.Add(x));
                    messagePayload.CcEmails?.Select(x => new MailAddress(x.Mail, x.Name)).ToList().ForEach(x => message.CC.Add(x));
                    messagePayload.BccEmails?.Select(x => new MailAddress(x.Mail, x.Name)).ToList().ForEach(x => message.Bcc.Add(x));
                    options.DefaultListCc?.ForEach(x =>
                    {
                        message.CC.Add(new MailAddress(x.Mail, x.Name));
                    });
                    options.DefaultListBcc?.ForEach(x =>
                    {
                        message.Bcc.Add(new MailAddress(x.Mail, x.Name));
                    });

                    message.Subject = messagePayload.Subject;
                    message.Body = messagePayload.Body;
                    message.IsBodyHtml = true;
                    return message;
                });
        }

        public void SendEmail(MessagePayload messagePayload)
        {
            if (options.Enabled)
            {
                PrepareMailMessage(messagePayload).MatchSome(
                    message =>
                    {
                        using (var client = new SmtpClient(options.CredentialServerAddress))
                        {
                            try
                            {
                                client.Port = options.CredentialServerPort;
                                client.Credentials = new NetworkCredential(options.CredentialEmail, options.CredentialPassword);
                                client.EnableSsl = options.CredentialServerEnableSsl;
                                client.Send(message);
                                logger.LogDebug("Smtp Email Service - {MethodName} - {Response}", nameof(SendEmail), $"Send email with title {message.Subject} succfully!");
                            }
                            catch (Exception ex)
                            {
                                logger.LogError("Email service exception: {Exception}", ex);
                            }
                            finally
                            {
                                message.Dispose();
                            }
                        }
                    }
                );
            }
        }

        public async Task SendEmailMultiplePayloadAsync(List<MessagePayload> messagePayloads, CancellationToken token)
        {
            if (options.Enabled)
            {
                await messagePayloads.Select(mp => PrepareMailMessage(mp))
                    .ParallelForEachAsync(async optionMp => await optionMp
                        .MapAsync(async mp =>
                        {
                            try
                            {
                                using (SmtpClient smtpClient = new SmtpClient(options.CredentialServerAddress))
                                {
                                    smtpClient.Port = options.CredentialServerPort;
                                    smtpClient.Credentials = new NetworkCredential(options.CredentialEmail, options.CredentialPassword);
                                    smtpClient.EnableSsl = options.CredentialServerEnableSsl;
                                    await smtpClient.SendMailAsync(mp);
                                    token.ThrowIfCancellationRequested();
                                    using (token.Register(() => smtpClient.SendAsyncCancel()))
                                    {
                                        await smtpClient.SendMailAsync(mp).ConfigureAwait(false);
                                    }
                                    logger.LogDebug("Smtp Email Service - {MethodName} - {Response}", nameof(SendEmailMultiplePayloadAsync), mp.Subject);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogError("Email service exception: {Exception}", ex);
                            }
                            finally
                            {
                                mp.Dispose();
                            }
                            return $"Send email with title {mp.Subject} succfully!";
                        }),
                        maxDegreeOfParallelism: 4,
                        cancellationToken: CancellationToken.None
                    );
            }
        }
    }
}