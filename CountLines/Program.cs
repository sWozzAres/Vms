namespace CountLines
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] paths = new string[]
            {
                "Vms.Application",
                "Vms.Domain",
                "Vms.Domain.Infrastructure",
                "Utopia.Blazor.Application",
                "Utopia.Blazor.Application.Shared",
                "Utopia.Blazor.Component",
                "Vms.Blazor.Application",
                "Vms.Web",
                "Vms.Web.Client.Common",
                "Vms.Web.Client.ServiceBookingProcess",
            };
            string[] searchPatterns = new string[] { "*.cs", "*.razor", "*.css", "*.js" };
            const string rootPath = @"C:\Users\markb\Source\repos\Vms";

            int totalLines = 0;
            foreach (var path in paths)
            {
                foreach (var searchPattern in searchPatterns)
                {
                    var files = Directory.EnumerateFiles(Path.Combine(rootPath, path), searchPattern, SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        var x = File.ReadAllLines(file);

                        Console.WriteLine($"{file} {x.Length}");
                        totalLines += x.Length;
                    }
                }
            }

            Console.WriteLine($"Total lines: {totalLines}.");
        }
    }
}