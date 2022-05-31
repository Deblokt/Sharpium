using System;
using System.Collections.Generic;

namespace Sharpium.Wpf.CSharp
{
    internal class Parsers
    {
        public static bool ContentHasCSharpScripts(string html)
        {
            return
                html.Contains("text/csharp", StringComparison.OrdinalIgnoreCase) ||
                html.Contains("text\\csharp", StringComparison.OrdinalIgnoreCase);
        }

        public static IEnumerable<string> ParseCSharpScriptsFromHtml(string html)
        {
            int start = 0;
            IList<string> scripts = new List<string>();

            while (true)
            {
                int startOfScript = html.IndexOf("<script type=\"text/csharp\">", start, StringComparison.OrdinalIgnoreCase);
                if (startOfScript < 0)
                {
                    startOfScript = html.IndexOf("<script type='text/csharp'>", start, StringComparison.OrdinalIgnoreCase);
                    if (startOfScript < 0)
                    {
                        startOfScript = html.IndexOf("<script type=\"text\\csharp\">", start, StringComparison.OrdinalIgnoreCase);
                        if (startOfScript < 0)
                        {
                            startOfScript = html.IndexOf("<script type='text\\csharp'>", start, StringComparison.OrdinalIgnoreCase);
                            if (startOfScript < 0)
                            {
                                break;
                            }
                        }
                    }
                }
                int endOfScript = html.IndexOf("</script>", startOfScript, StringComparison.OrdinalIgnoreCase);
                if (endOfScript < 0)
                {
                    break;
                }

                scripts
                    .Add(html[startOfScript..endOfScript]
                    .Replace("<script type=\"text/csharp\">", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("<script type='text/csharp'>", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("<script type=\"text\\csharp\">", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("<script type='text\\csharp'>", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("</script>", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Trim());

                start = endOfScript;
            }

            return scripts;
        }
    }
}
