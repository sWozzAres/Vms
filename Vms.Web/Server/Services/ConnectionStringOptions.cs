namespace Vms.Web.Server.Services;

public class ConnectionStringOptions
{
    public string VmsDbConnection { get; set; } = null!;
    public string CatalogDbConnection { get; set; } = null!;
    public IEnumerable<string> ConnectionStrings()
    {
        yield return VmsDbConnection;
        yield return CatalogDbConnection;
    }
}
