namespace Doitsu.Service.Core.Abstraction
{
    public class SeoInformationModel
    {
        public string BreadcrumbTitle { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        public int DefaultImageWidth { get; set; }
        public int DefaultImageHeight { get; set; }
    }

}