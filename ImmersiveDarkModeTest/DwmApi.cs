﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ImmersiveDarkModeTest
{
    internal static class DwmApi
    {
        private const int S_OK = 0;

        // This two flags are not currently documented
        // and they might change in the future
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private const int WINDOWS10_MIN_BUILD_NUMBER = 17763;
        private const int WINDOWS10_20H1_BUILD_NUMBER = 18985;

        public static void ToggleImmersiveDarkMode(Window window, bool enable)
        {
            var helper = new WindowInteropHelper(window);
            ToggleImmersiveDarkMode(helper.Handle, enable);
        }

        public static void ToggleImmersiveDarkMode(IntPtr window, bool enable)
        {
            if (!IsWindows10OrGreater(WINDOWS10_MIN_BUILD_NUMBER))
            {
                // Dark mode is not supported
                return;
            }

            int useImmersiveDarkMode = enable ? 1 : 0;
            CheckHResult(DwmSetWindowAttribute(window, ImmersiveDarkModeAttribute, ref useImmersiveDarkMode, sizeof(int)));
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attributeValue, int attributeSize);

        private static bool IsWindows10OrGreater(int build)
            => IsWindow10 && HasAtLeastBuildNumber(build);

        private static bool IsWindow10
            => Environment.OSVersion.Version.Major >= 10;

        private static bool HasAtLeastBuildNumber(int build)
            => Environment.OSVersion.Version.Build >= build;

        private static int ImmersiveDarkModeAttribute
            => HasAtLeastBuildNumber(WINDOWS10_20H1_BUILD_NUMBER)
                ? DWMWA_USE_IMMERSIVE_DARK_MODE
                : DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;

        private static void CheckHResult(int hResult)
        {
            if (hResult != S_OK)
                throw new Win32Exception(hResult);
        }
    }
}
