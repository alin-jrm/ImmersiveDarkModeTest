using Microsoft.Win32;
using System;
using System.Management;
using System.Security.Principal;

namespace ImmersiveDarkModeTest
{
    public delegate void ThemeChangedEvent(ThemeListener sender);

    public class ThemeListener : IDisposable
    {
        private const string PersonalizeRegistryKeyPath = @"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
        private const string UseLightThemeRegistryKey = "AppsUseLightTheme";

        private readonly ManagementEventWatcher registryKeyChangeWatcher;

        public ThemeListener()
        {
            UpdateCurrentTheme();
            registryKeyChangeWatcher = new ManagementEventWatcher(WmiQuery);
        }

        public event ThemeChangedEvent ThemeChanged;

        public ApplicationTheme CurrentTheme { get; private set; }

        private static string WmiQuery
        {
            get
            {
                var currentUser = WindowsIdentity.GetCurrent();
                return $"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{currentUser.User.Value}\\\\{PersonalizeRegistryKeyPath}' AND ValueName = '{UseLightThemeRegistryKey}'";
            }
        }

        public void Dispose()
        {
            registryKeyChangeWatcher.Stop();
            registryKeyChangeWatcher.EventArrived -= RegistryKeyChanged;
        }

        public void Start()
        {
            registryKeyChangeWatcher.EventArrived += RegistryKeyChanged;
            registryKeyChangeWatcher.Start();
        }

        private void RegistryKeyChanged(object sender, EventArrivedEventArgs e)
        {
            UpdateCurrentTheme();
            ThemeChanged?.Invoke(this);
        }

        private void UpdateCurrentTheme()
        {
            var key = Registry.CurrentUser.OpenSubKey(PersonalizeRegistryKeyPath);
            var useLightTheme = key?.GetValue(UseLightThemeRegistryKey) as int?;
            CurrentTheme = !useLightTheme.HasValue || useLightTheme.Value > 0
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark;
        }
    }
}