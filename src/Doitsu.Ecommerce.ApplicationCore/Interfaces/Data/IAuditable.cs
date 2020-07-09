using System;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Data
{
    public interface IAuditable
    {
        DateTime? CreatedDate { get; set; }
        DateTime? LastUpdatedDate { get; set; }
    }
}
