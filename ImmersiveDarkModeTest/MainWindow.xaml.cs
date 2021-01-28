using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ImmersiveDarkModeTest
{
    public partial class MainWindow : Window
    {
        public record Theme(Brush Background, Brush Foreground);

        readonly ThemeListener themeListener;
        readonly ApplicationTheme currentApplicationTheme;
        readonly bool followSystemTheme;

        readonly Dictionary<ApplicationTheme, Theme> themes = new Dictionary<ApplicationTheme, Theme>
        {
            { ApplicationTheme.Light, new Theme(new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.Black)) },
            { ApplicationTheme.Dark, new Theme(new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.White)) }
        };

        public MainWindow()
        {
            InitializeComponent();
            themeListener = new ThemeListener();
            currentApplicationTheme = ApplicationTheme.Light;
            followSystemTheme = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            UpdateTheme();
            themeListener.ThemeChanged += ThemeListener_ThemeChanged;
            themeListener.Start();
        }

        private void UpdateTheme()
        {
            var currentTheme = themes[CurrentApplicationTheme];
            Foreground = currentTheme.Foreground;
            Background = currentTheme.Background;
            DwmApi.ToggleImmersiveDarkMode(this, CurrentApplicationTheme == ApplicationTheme.Dark);
        }

        private ApplicationTheme CurrentApplicationTheme
            => followSystemTheme
                ? themeListener.CurrentTheme
                : currentApplicationTheme;

        private void ThemeListener_ThemeChanged(ThemeListener sender)
        {
            Dispatcher.Invoke(() => UpdateTheme());
        }
    }
}
