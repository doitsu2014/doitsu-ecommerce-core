namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class CatalogueViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReferenceUrl { get; set; }
        public string PdfUrl { get; set; }
        public string ImageUrl { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
    }
}