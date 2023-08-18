namespace CountLines
{
    internal class Program
    {
        static void Main()
        {
            string[] paths = new string[]
            {
                "Utopia.Blazor.Application.Vms",
                "Utopia.Blazor.Application.Vms.Shared",
                "Vms.Api",
                "Vms.Application",
                "Vms.Domain",
                "Vms.Domain.Infrastructure",

                "Catalog.Api",
                "Catalog.Blazor",
                "Catalog.Shared",

                "Utopia.Api",
                "Utopia.Blazor.Application",
                "Utopia.Blazor.Application.Common",
                "Utopia.Blazor.Application.Shared",

                "Utopia.Blazor.Component",
                "Utopia.Shared",

                "Vms.Web\\Client",
                "Vms.Web\\Server",
            };
            string[] searchPatterns = new string[] { "*.cs", "*.razor", "*.css", "*.js" };
            const string rootPath = @"C:\Users\markb\Source\repos\Vms";

            int totalLines = 0;
            int ignoredLines = 0;
            foreach (var path in paths)
            {
                foreach (var searchPattern in searchPatterns)
                {
                    var files = Directory.EnumerateFiles(Path.Combine(rootPath, path), searchPattern, SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        var lines = File.ReadAllLines(file);
                        foreach (var line in lines)
                        {
                            var trimmedLine = line.Trim();

                            if (string.IsNullOrWhiteSpace(trimmedLine) || (trimmedLine == "{" || trimmedLine == "}"))
                                ignoredLines++;

                            totalLines++;
                        }
                        Console.WriteLine($"{file} {lines.Length}");
                    }
                }
            }

            Console.WriteLine($"Total lines: {totalLines} [{ignoredLines}].");
        }
    }
}