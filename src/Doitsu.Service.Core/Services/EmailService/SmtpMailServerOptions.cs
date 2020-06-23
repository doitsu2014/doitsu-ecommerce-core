using System.Collections.Generic;

namespace Doitsu.Service.Core.Services.EmailService
{
    public class SmtpMailServerOptions
    {
        public bool Enabled { get; set; }
        public string CredentialEmail { get; set; }
        public string CredentialPassword { get; set; }
        public string CredentialServerAddress { get; set; }
        public int CredentialServerPort { get; set; }
        public bool CredentialServerEnableSsl { get; set; }
        public MailPayloadInformation FromMail { get; set; }
        public List<MailPayloadInformation> DefaultListBcc { get; set; }
        public List<MailPayloadInformation> DefaultListCc { get; set; }
    }
}