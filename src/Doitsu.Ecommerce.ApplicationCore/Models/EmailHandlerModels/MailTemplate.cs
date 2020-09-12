namespace Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels
{
    public class MailTemplate
    {
        public string Url { get; set; }
        public string Subject { get; set; }
        public EmailAddressInformation[] CcEmails { get; set; }
        public EmailAddressInformation[] BccEmails { get; set; }
    }
}