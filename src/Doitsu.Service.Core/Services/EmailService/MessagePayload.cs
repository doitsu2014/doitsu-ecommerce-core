namespace Doitsu.Service.Core.Services.EmailService
{
    public class MessagePayload
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public MailPayloadInformation DestEmail { get; set; }
        public MailPayloadInformation CcEmail { get; set; } = null;
        public MailPayloadInformation BccEmail { get; set; } = null;

    }
}