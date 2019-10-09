namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class SliderViewModel
    {
      public int Id { get; set; }
        public string Title { get; set; }
        public string Slogan { get; set; }
        public string ReferenceUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPopup { get; set; }
        public bool Active { get; set; }
    }
}