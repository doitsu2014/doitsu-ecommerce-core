namespace Doitsu.Service.Core.Interfaces.EfCore
{
    public interface IConcurrencyCheckVers
    {
        byte[] Vers { get; set; }
    }
}
