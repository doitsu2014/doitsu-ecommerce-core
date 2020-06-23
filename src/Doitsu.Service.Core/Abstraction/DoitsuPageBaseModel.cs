namespace Doitsu.Service.Core.Abstraction
{
    public class DoitsuPageBaseModel<T> : SeoInformationModel
        where T : class
    {
        public T Data { get; set; }
    }

    public class DoitsuStringPageBaseModel : DoitsuPageBaseModel<string> 
    {}
}