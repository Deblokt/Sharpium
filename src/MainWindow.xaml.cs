using CefSharp;
using Sharpium.Wpf.CSharp;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Sharpium.Wpf
{
    public partial class MainWindow : Window
    {
        private static bool _ignoreStateChange = false;
        private static readonly IList<Assembly> _assemblies = new List<Assembly>();
        private static System.Timers.Timer _oneSecondTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            Browser.AddressChanged += Browser_AddressChanged;
            Browser.LoadingStateChanged += Browser_LoadingStateChanged;

            _oneSecondTimer = new System.Timers.Timer()
            {
                Interval = 1000,
                AutoReset = true,
                Enabled = false
            };
            _oneSecondTimer.Elapsed += OneSecondTimer_Elapsed;
        }

        private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _ignoreStateChange = false;
        }

        private async void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            _oneSecondTimer.Enabled = false;
            _assemblies.Clear();

            if (!e.IsLoading)
            {
                if (_ignoreStateChange)
                {
                    return;
                }
                _ignoreStateChange = true;

                await Main.ParseAndBuildCSharpScriptsAsync(e.Browser, _assemblies);
                await Runners.InvokeOnInit(e.Browser, _assemblies);

                _oneSecondTimer.Enabled = true;
            }
        }

        private async void OneSecondTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Browser?.BrowserCore is null)
            {
                _oneSecondTimer.Enabled = false;
                return;
            }
            await Runners.InvokeOnOneSecondTimer(Browser.BrowserCore, _assemblies);
        }
    }
}
