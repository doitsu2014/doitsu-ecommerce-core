using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
{
    public class Sliders : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public string Slogan { get; set; }
        public string ReferenceUrl { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPopup { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
    }
}
