using System;

namespace Doitsu.Service.Core.Interfaces.EfCore
{
    public interface IAuditable
    {
        DateTime? CreatedDate { get; set; }
        DateTime? LastUpdatedDate { get; set; }
    }

    // public interface IAuditable<TEntityKey>
    // {   
    //     string CreatedBy { get; set; }
    //     DateTime? CreatedDate { get; set; }
    //     string LastUpdatedBy { get; set; }
    //     DateTime? LastUpdatedDate { get; set; }
    // }
}
