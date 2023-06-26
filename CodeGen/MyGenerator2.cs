using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Diagnostics;

namespace CodeGen
{

    [Generator]
    public class MyGenerator2 : ISourceGenerator
    {
        //static readonly DiagnosticDescriptor InvalidXmlWarning =
        //    new (id: "MYXMLGEN001",
        //    title: "Couldn't parse XML file",
        //    messageFormat: "Couldn't parse XML file '{0}'",
        //    category: "MyXmlGenerator",
        //    DiagnosticSeverity.Warning,
        //    isEnabledByDefault: true);

        public void Execute(GeneratorExecutionContext context)
        {
            //context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None));
            Debug.WriteLine("Execute code generator");
            // retrieve the compilation
            Compilation compilation = context.Compilation;

            // get all the syntax trees
            foreach (var tree in compilation.SyntaxTrees)
            {
                // get the root of the tree
                var root = tree.GetRoot();

                // find all the partial classes
                foreach (var @class in root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Where(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))))
                {
                    // get the class name
                    string className = @class.Identifier.ValueText;

                    // create a new StringBuilder to hold our generated code
                    StringBuilder sb = new StringBuilder();

                    // write the namespace and class declaration
                    sb.AppendLine($"namespace {compilation.AssemblyName}");
                    sb.AppendLine("{");
                    sb.AppendLine($"\tpublic partial class {className}2 : ICopyable<{className}>");
                    sb.AppendLine("\t{");

                    //sb.AppendLine("\t\tpublic IEnumerable<string> GetProperties()");
                    //sb.AppendLine("\t\t{");
                    //sb.AppendLine("\t\t}");

                    // write the method declaration
                    sb.AppendLine($"\t\tpublic void CopyFrom({className} source)");
                    sb.AppendLine("\t\t{");

                    // write the method body
                    //foreach (var property in @class.DescendantNodes().OfType<PropertyDeclarationSyntax>())
                    //{
                    //    string propertyName = property.Identifier.ValueText;
                    //    sb.AppendLine($"\t\t\tyield return \"{propertyName}\";");
                    //}

                    // close the method and class declarations
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t}");
                    sb.AppendLine("}");

                    var x = sb.ToString();

                    // add the generated code to the compilation
                    context.AddSource($"{className}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
            Debug.WriteLine("Initalize code generator");
        }
    }
}