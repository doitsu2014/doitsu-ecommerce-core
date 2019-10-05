using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
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
