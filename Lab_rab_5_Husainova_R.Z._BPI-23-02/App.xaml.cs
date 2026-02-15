
using System;
using System.Windows;
using Lab_rab_5_Husainova_R.Z._BPI_23_02;

namespace Lab_rab_5_Husainova_R.Z._BPI_23_02
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ThemeManager.ApplyTheme("LightTheme");
        }
    }
}
