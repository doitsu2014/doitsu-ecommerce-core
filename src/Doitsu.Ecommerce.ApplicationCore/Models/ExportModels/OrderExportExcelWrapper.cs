namespace Doitsu.Ecommerce.ApplicationCore.Models.ExportModels
{
    public class OrderExportExcelWrapper
    {
        public static OrderExportExcelWrapper CreateInstance(byte[] content, string fileDownloadName)
        {
            return new OrderExportExcelWrapper
            {
                Content = content,
                FileDownloadName = fileDownloadName
            };
        }

        public byte[] Content { get; set; }
        public string FileDownloadName { get; set; }
        public string ContentType { get => Constants.FileExtension.EXCEL; }
    }
}