using System;
using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{

    public class Brand : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Name { get; set; }
        public string HotLine { get; set; }
        public string Fax { get; set; }
        public string Mail { get; set; }
        public string Address { get; set; }
        public string AlternativeAddress { get; set; }
        public string InstagramUrl { get; set; }
        public string GoogleScript { get; set; }
        public string FacebookScript { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string FacebookUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GooglePlusUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string LogoSquareUrl { get; set; }
        public string LogoRectangleUrl { get; set; }
        public string Description { get; set; }
        public string YoutubeUrl { get; set; }
        public int? OpenDayOfWeek { get; set; }
        public int? CloseDayOfWeek { get; set; }
        public string FaviconUrl { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public ICollection<WareHouse> WareHouses { get; set; }
    }
}
