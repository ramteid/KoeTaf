/**
 * Class: SplashWindow
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-06-03
 * 
 * Last modification: 2013-06-10 / Michael Müller
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using KöTaf.Utils.Parser;
using KöTaf.Utils.FileOperations;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Views.Formletter;
using KöTaf.DataModel;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Background Logik für die SplashWindow.xaml
    /// Diese Klasse zeigt das Anfangsfenster (das Ladefenster) des 
    /// Programms Kötaf.
    /// </summary>
    public partial class SplashWindow : Window
    {
        #region Constructor

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SplashWindow()
        {
            try
            {
                // Prüfe auf Konfigurationsdatei config.ini -> Kritisch!
                string iniPath = System.IO.Path.Combine(Environment.CurrentDirectory, "config.ini");
                if (!(System.IO.File.Exists(iniPath)))
                {
                    MessageBox.Show("Die Konfigurationsdatei config.ini wurde nicht im Programmverzeichnis gefunden. Das Programm wird beendet!", "Programm", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Environment.Exit(0);
                }

                // Prüfe ob Konfigurationsdatei funktioniert durch Test-Aufruf
                try
                {
                    IniParser.GetSetting("SETTINGS", "splashImage");
                }
                catch
                {
                    MessageBox.Show("Es liegt eine Inkonsistenz in der Konfigurationsdatei vor. Das Programm wird beendet!", "Programm", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Environment.Exit(0);
                }

                // Falls Libre Office nicht installiert ist, gebe Meldung uas
                if (!LibreOffice.isLibreOfficeInstalled())
                {
                    string warning = IniParser.GetSetting("ERRORMSG", "libre");
                    MessageBoxEnhanced.Error(warning);
                }

                this.load();
            }
            catch
            {
                MessageBox.Show("Es ist ein unbekannter Fehler aufgetreten. Das Programm wird beendet.", "Programm", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Environment.Exit(0);
            }
        }

        #endregion
        
        #region Methods        
        
        /// Führe diese Methode aus, wenn das Fenster vollständig geladen wurde. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.load();
        }

        /// <summary>
        /// Layout Konfiguration
        /// </summary>
        private void load()
        {
            SplashScreen splash = new SplashScreen(IniParser.GetSetting("SETTINGS", "splashImage"));
            splash.Show(true);
            // lade das erste mal die Datenbank
            var person = DataModel.Person.GetPersons();
            var sponsor = DataModel.Sponsor.GetSponsors();
            var team = DataModel.Team.GetTeams();
            // Initialisiere / erstelle ein LoginWindow() Objekt
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
#endregion