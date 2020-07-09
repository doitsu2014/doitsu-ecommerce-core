namespace Doitsu.Ecommerce.ApplicationCore
{
    public class DoitsuPageBaseModel<T> : SeoInformationModel
        where T : class
    {
        public T Data { get; set; }
    }

    public class DoitsuStringPageBaseModel : DoitsuPageBaseModel<string> 
    {}
}