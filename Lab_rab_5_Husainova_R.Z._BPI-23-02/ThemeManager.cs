using System;
using System.Windows;

public static class ThemeManager
{
    public static void ApplyTheme(string themeName)
    {
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        dictionaries.Clear();
        try
        {
            var uri = new Uri($"/Themes/{themeName}.xaml", UriKind.Relative);
            dictionaries.Add(new ResourceDictionary { Source = uri });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить тему: {themeName}\n{ex.Message}",
                "Ошибка темы", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
