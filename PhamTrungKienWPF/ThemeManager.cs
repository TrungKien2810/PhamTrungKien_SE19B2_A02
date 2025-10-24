using System;
using System.Linq;
using System.Windows;

namespace PhamTrungKienWPF
{
    public static class ThemeManager
    {
        private const string LightThemeUri = "/PhamTrungKienWPF;component/Themes/Light.xaml";
        private const string DarkThemeUri = "/PhamTrungKienWPF;component/Themes/Dark.xaml";

        public static void ApplyTheme(bool useDarkTheme)
        {
            var appResources = Application.Current?.Resources;
            if (appResources == null)
            {
                return;
            }

            var merged = appResources.MergedDictionaries;

            // Remove existing theme dictionaries
            var existingThemes = merged
                .Where(d => d.Source != null &&
                            (string.Equals(d.Source.ToString(), LightThemeUri, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(d.Source.ToString(), DarkThemeUri, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var theme in existingThemes)
            {
                merged.Remove(theme);
            }

            // Add the selected theme
            var newTheme = new ResourceDictionary
            {
                Source = new Uri(useDarkTheme ? DarkThemeUri : LightThemeUri, UriKind.RelativeOrAbsolute)
            };
            merged.Add(newTheme);
        }
    }
}


