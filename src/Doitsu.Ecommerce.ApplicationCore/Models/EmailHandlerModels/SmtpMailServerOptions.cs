using System.Collections.Generic;

namespace Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels
{
    public class SmtpMailServerOptions
    {
        public bool Enabled { get; set; }
        public string CredentialEmail { get; set; }
        public string CredentialPassword { get; set; }
        public string CredentialServerAddress { get; set; }
        public int CredentialServerPort { get; set; }
        public bool CredentialServerEnableSsl { get; set; }

        // Default Email Configuration
        public EmailAddressInformation FromMail { get; set; }
        public List<EmailAddressInformation> DefaultListBcc { get; set; }
        public List<EmailAddressInformation> DefaultListCc { get; set; }
    }
}