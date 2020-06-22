namespace Doitsu.Service.Core.Interfaces.EfCore
{
    public interface IAuditService
    {
        string CurrentUserId { get; }
    }
}
