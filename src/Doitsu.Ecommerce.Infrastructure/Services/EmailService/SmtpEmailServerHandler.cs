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

namespace Doitsu.Ecommerce.Infrastructure.Services.EmailService
{
    public class SmtpEmailServerHandler : ISmtpEmailServerHandler
    {
        private readonly ILogger<SmtpEmailServerHandler> logger;

        public SmtpEmailServerHandler(ILogger<SmtpEmailServerHandler> logger)
        {
            this.logger = logger;
        }

        private Option<MailMessage> PrepareMessage(SmtpMailServerOptions options, MessagePayload messagePayload)
        {
            return (options, messagePayload).Some()
                .Map(data =>
                {
                    var message = new MailMessage();
                    message.From = new MailAddress(options.FromMail.Mail, options.FromMail.Name);
                    message.To.Add(new MailAddress(messagePayload.DestEmail.Mail, messagePayload.DestEmail.Name));

                    options.DefaultListCc?.ForEach(x =>
                        {
                            message.CC.Add(new MailAddress(x.Mail, x.Name));
                        });

                    if (messagePayload.CcEmail != null)
                    {
                        message.CC.Add(new MailAddress(messagePayload.CcEmail.Mail, messagePayload.CcEmail.Name));
                    }

                    options.DefaultListBcc?.ForEach(x =>
                        {
                            message.Bcc.Add(new MailAddress(x.Mail, x.Name));
                        });

                    if (messagePayload.BccEmail != null)
                    {
                        message.Bcc.Add(new MailAddress(messagePayload.BccEmail.Mail, messagePayload.BccEmail.Name));
                    }

                    message.Subject = messagePayload.Subject;
                    message.Body = messagePayload.Body;
                    message.IsBodyHtml = true;
                    return message;
                });
        }

        public void SendEmail(SmtpMailServerOptions options, MessagePayload messagePayload)
        {
            if (options.Enabled)
            {
                PrepareMessage(options, messagePayload).MatchSome(
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

        public async Task SendEmailMultiplePayloadAsync(SmtpMailServerOptions options, List<MessagePayload> messagePayloads, CancellationToken token)
        {
            if (options.Enabled)
            {
                await messagePayloads.Select(mp => PrepareMessage(options, mp))
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

        public async Task SendEmailAsync(SmtpMailServerOptions options, List<MessagePayload> messagePayloads)
        {
            if(options.Enabled)
            {
                using (SmtpClient smtpClient = new SmtpClient(options.CredentialServerAddress))
                {
                    smtpClient.Port = options.CredentialServerPort;
                    smtpClient.Credentials = new NetworkCredential(options.CredentialEmail, options.CredentialPassword);
                    smtpClient.EnableSsl = options.CredentialServerEnableSsl;
                    List<Task> sentEmailActions = new List<Task>();
                    foreach (MessagePayload messagePayload in messagePayloads)
                    {
                        PrepareMessage(options, messagePayload).MatchSome(
                            mp =>
                            {
                                sentEmailActions.Add(smtpClient.SendMailAsync(mp));
                            }
                        );
                    }
                    await Task.WhenAll(sentEmailActions);
                }
            }
        }

        public void SendEmailNonBlocking(SmtpMailServerOptions options, MessagePayload messagePayload, object userToken = null)
        {
            if (options.Enabled)
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(options.FromMail.Mail, options.FromMail.Name);
                    message.To.Add(new MailAddress(messagePayload.DestEmail.Mail, messagePayload.DestEmail.Name));

                    options.DefaultListCc?.ForEach(x =>
                    {
                        message.CC.Add(new MailAddress(x.Mail, x.Name));
                    });

                    if (messagePayload.CcEmail != null)
                    {
                        message.CC.Add(new MailAddress(messagePayload.CcEmail.Mail, messagePayload.CcEmail.Name));
                    }

                    options.DefaultListBcc?.ForEach(x =>
                    {
                        message.Bcc.Add(new MailAddress(x.Mail, x.Name));
                    });
                    if (messagePayload.BccEmail != null)
                    {
                        message.Bcc.Add(new MailAddress(messagePayload.BccEmail.Mail, messagePayload.BccEmail.Name));
                    }

                    message.Subject = messagePayload.Subject;
                    message.Body = messagePayload.Body;
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(options.CredentialServerAddress))
                    {
                        client.Port = options.CredentialServerPort;
                        client.Credentials = new NetworkCredential(options.CredentialEmail, options.CredentialPassword);
                        client.EnableSsl = options.CredentialServerEnableSsl;
                        client.SendAsync(message, userToken);
                    }
                }

            }
        }
    }
}