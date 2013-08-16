using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using KöTaf.WPFApplication.Views;

namespace KöTaf.WPFApplication
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e) {
            SplashWindow splash = new SplashWindow();
            //KöTaf.WPFApplication.Views.LoginWindow newWindow = new KöTaf.WPFApplication.Views.LoginWindow();
            //MainWindow newWindow = new MainWindow(null);
            //newWindow.Show();
            splash.Show();
            splash.Close();
            
        }
    }
}
