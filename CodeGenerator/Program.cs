using Microsoft.SqlServer.Management.Smo;

Server srv = new();
srv.ConnectionContext.ConnectionString = @"Server=SKYLAKE\SQL2019;Database=Vms_Test;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

Console.WriteLine($"Version: {srv.Information.Version}");

const string DatabaseName = "Vms_Test";
var db = srv.Databases[DatabaseName] ?? throw new InvalidOperationException($"Database {DatabaseName} not found.");


foreach (Table table in db.Tables)
{
    Console.WriteLine(table.Name);
}
