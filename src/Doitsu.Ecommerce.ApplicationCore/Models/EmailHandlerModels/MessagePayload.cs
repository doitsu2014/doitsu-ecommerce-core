namespace Doitsu.Ecommerce.ApplicationCore.Models.EmailHandlerModels
{
    public class MessagePayload
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailAddressInformation[] DestEmails { get; set; }
        public EmailAddressInformation[] CcEmails { get; set; } = null;
        public EmailAddressInformation[] BccEmails { get; set; } = null;
    }
}