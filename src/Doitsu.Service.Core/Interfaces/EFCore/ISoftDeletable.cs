namespace Doitsu.Service.Core.Interfaces.EfCore
{
    public interface ISoftDeletable
    {
        bool Deleted { get; set; }
    }
}
