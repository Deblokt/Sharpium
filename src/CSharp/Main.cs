using CefSharp;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Sharpium.Wpf.CSharp
{
    internal class Main
    {
        public static async Task ParseAndBuildCSharpScriptsAsync(IBrowser browser, IList<Assembly> assemblies)
        {
            string html = await browser.GetSourceAsync();
            if (string.IsNullOrWhiteSpace(html))
            {
                return;
            }

            bool contentHasCSharpScripts = Parsers.ContentHasCSharpScripts(html);
            if (!contentHasCSharpScripts)
            {
                return;
            }

            IEnumerable<string> scripts = Parsers.ParseCSharpScriptsFromHtml(html);
            foreach (string script in scripts)
            {
                Assembly assembly = Builders.EmitAssembly(script);
                if (assembly != null)
                {
                    assemblies.Add(assembly);
                }
            }
        }
    }
}
