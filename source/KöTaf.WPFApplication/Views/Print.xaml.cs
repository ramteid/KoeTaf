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
using System.Windows.Navigation;
using System.Windows.Shapes;
using KöTaf.Utils.Printer;
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Views.Formletter;
using KöTaf.Utils.Parser;
using KöTaf.Utils.FileOperations;
using KöTaf.WPFApplication.Helper;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Druck
    /// </summary>
    public partial class Print : KPage
    {
        public Print()
        {
            InitializeComponent();
            createButtonToolTips();
        }

        public override void defineToolbarContent()
        {
        }

        /// <summary>
        /// Lade Bezeichnung für Kassenabschluss und Kasenabrechnung aus der Konfigurationsdatei 
        /// </summary>
        private void createButtonToolTips()
        {
            btCredential.Content = IniParser.GetSetting("DOCUMENTS", "credential");
            btDisclaimer.Content = IniParser.GetSetting("DOCUMENTS", "disclaimerDocument");
            btDonations.Content = IniParser.GetSetting("DOCUMENTS", "donations");
            btCaritas.Content = IniParser.GetSetting("DOCUMENTS", "caritas");
        }

        /// <summary>
        /// Serienbrief-Druck öffnen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btFormletter_Click(object sender, RoutedEventArgs e)
        {
            KPage formletterPrint = new pFormletterPrint();
            SinglePage formletter = new SinglePage(IniParser.GetSetting("FORMLETTER", "formLetterPrint"), formletterPrint);
        }

        /// <summary>
        /// Berechtigungsnachweis 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCredential_Click(object sender, RoutedEventArgs e)
        {
            PrintForms.printCredential();
        }

        /// <summary>
        /// Haftungsausschluss
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDisclaimer_Click(object sender, RoutedEventArgs e)
        {
            PrintForms.printClientDisclaimer();
        }
        
        /// <summary>
        /// Caritas-Formular
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCaritas_Click(object sender, RoutedEventArgs e)
        {
            PrintForms.printCaritasForm();
        }

        /// <summary>
        /// Spendeneinreicher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDonations_Click(object sender, RoutedEventArgs e)
        {
            PrintForms.printDonationsForm();
        }
    }
}
