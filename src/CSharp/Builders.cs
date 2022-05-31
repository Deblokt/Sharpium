using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sharpium.Wpf.CSharp
{
    internal class Builders
    {
        public static Assembly EmitAssembly(string script)
        {
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            string fullScript = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;

                public class Script" +
                "{\n"
                + script +
                "\n}";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(fullScript);
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            EmitResult emitResult = compilation.Emit(ms);

            if (emitResult.Success)
            {
                ms.Seek(0, SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                return assembly;
            }

            IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (Diagnostic diagnostic in failures)
            {
                Trace.WriteLine(string.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage()));
            }

            return null;
        }
    }
}
