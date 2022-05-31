using CefSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Sharpium.Wpf.CSharp
{
    internal class Runners
    {
        public static async Task InvokeOnInit(IBrowser browser, IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                string html = await browser.GetSourceAsync();

                Type type = assembly.GetType("Script");
                object obj = Activator.CreateInstance(type);

                MethodInfo method = type.GetMethod("OnInit");
                if (method is null)
                {
                    return;
                }

                var methodParams = method.GetParameters();

                object[] inputParams = null;
                switch (methodParams.Length)
                {
                    case 1:
                        inputParams = new object[] { methodParams[0].DefaultValue };
                        break;
                    case 2:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue };
                        break;
                    case 3:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue, methodParams[2].DefaultValue };
                        break;
                    case 4:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue, methodParams[2].DefaultValue, methodParams[3].DefaultValue };
                        break;
                    case 5:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue, methodParams[2].DefaultValue, methodParams[3].DefaultValue, methodParams[4].DefaultValue };
                        break;
                    case 6:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue, methodParams[2].DefaultValue, methodParams[3].DefaultValue, methodParams[4].DefaultValue, methodParams[5].DefaultValue };
                        break;
                    case 7:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue, methodParams[2].DefaultValue, methodParams[3].DefaultValue, methodParams[4].DefaultValue, methodParams[5].DefaultValue, methodParams[6].DefaultValue };
                        break;
                    case 8:
                        inputParams = new object[] { methodParams[0].DefaultValue, methodParams[1].DefaultValue, methodParams[2].DefaultValue, methodParams[3].DefaultValue, methodParams[4].DefaultValue, methodParams[5].DefaultValue, methodParams[6].DefaultValue, methodParams[7].DefaultValue };
                        break;
                }

                object result = method.Invoke(obj, BindingFlags.Default | BindingFlags.InvokeMethod, null, inputParams, null);

                Type t = result.GetType();
                var properties = t.GetProperties();
                foreach (var prop in properties)
                {
                    html = html
                        .Replace($"${{{prop.Name}}}", prop.GetValue(result).ToString(), StringComparison.OrdinalIgnoreCase)
                        .Replace($"${{ {prop.Name} }}", prop.GetValue(result).ToString(), StringComparison.OrdinalIgnoreCase);
                }

                await SetHtmlContentAsync(browser, html);
            }
        }

        public static async Task InvokeOnOneSecondTimer(IBrowser browser, IEnumerable<Assembly> assemblies)
        {
            foreach (Assembly assembly in assemblies)
            {
                string html = await browser.GetSourceAsync();

                Type type = assembly.GetType("Script");
                object obj = Activator.CreateInstance(type);

                MethodInfo method = type.GetMethod("OnEverySecond");
                if (method is null)
                {
                    return;
                }

                object result = method.Invoke(obj, BindingFlags.Default | BindingFlags.InvokeMethod, null, null, null);

                Type t = result.GetType();
                var properties = t.GetProperties();
                foreach (var prop in properties)
                {
                    html = html
                        .Replace($"${{{prop.Name}}}", $"${{{prop.Name}}} {prop.GetValue(result)}", StringComparison.OrdinalIgnoreCase)
                        .Replace($"${{ {prop.Name} }}", $"${{ {prop.Name} }} {prop.GetValue(result)}", StringComparison.OrdinalIgnoreCase);
                }

                await SetHtmlContentAsync(browser, html);
            }
        }

        private static async Task SetHtmlContentAsync(IBrowser browser, string html)
        {
            await browser.SetMainFrameDocumentContentAsync(html);
        }
    }
}
