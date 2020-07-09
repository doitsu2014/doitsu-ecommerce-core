using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class Catalogues : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public string ReferenceUrl { get; set; }
        public string PdfUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
    }
}
