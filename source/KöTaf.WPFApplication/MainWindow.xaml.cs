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
using KöTaf.DataModel;
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Views;
using KöTaf.Utils.Printer;
using System.Collections.Specialized;
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Views.Accounting;
using KöTaf.WPFApplication.Views.Accounting.AccountManager;
using KöTaf.WPFApplication.Views.Accounting.Bookings;
using KöTaf.WPFApplication.Views.Accounting.Sums;
using KöTaf.WPFApplication.Views.Accounting.CashClosureManager;
using KöTaf.Utils.UserSession;



namespace KöTaf.WPFApplication
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Interaktionslogik für das Hauptfenster
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Hole Stil-Parameter aus der Konfigurationsdatei
            string topBarColor = IniParser.GetSetting("APPSETTINGS", "topBarColor");
            Brush topbarColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(topBarColor));
            topBar.Background = topbarColorBrush;

            // Hole Titel aus der Konfigurationsdatei
            lbPogramTitle.Content = IniParser.GetSetting("APPSETTINGS", "programTitle");

            // Verwaltung-Button nur für Admin sichtbar machen
            if (UserSession.isAdmin)
                pbAdministration.Visibility = System.Windows.Visibility.Visible;
            else
            pbAdministration.Visibility = System.Windows.Visibility.Hidden;

            // Zeige Benutzernamen oben rechts an
            lblCurrentUser.Content = UserSession.userName;
        }

        #region Events

        /// <summary>
        /// Team
        /// </summary>
        private void pbTeamAdministration_Click(object sender, RoutedEventArgs e)
        {
            KPage pageTeamAdministration = new KöTaf.WPFApplication.Views.pTeamAdministration();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "teamAdministration"), pageTeamAdministration);
        }

        /// <summary>
        /// Kunden
        /// </summary>
        private void pbClientAdministration_Click(object sender, RoutedEventArgs e)
        {
            KPage pageClientAdministration = new KöTaf.WPFApplication.Views.pClientAdministration();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "clientAdministration"), pageClientAdministration);
        }

        /// <summary>
        /// Sponsor
        /// </summary>
        private void pbSponsorAdministration_Click(object sender, RoutedEventArgs e)
        {
            KPage pageSponsorAdministration = new KöTaf.WPFApplication.Views.pSponsorAdministration();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "sponsorAdministration"), pageSponsorAdministration);
        }

        /// <summary>
        /// Buchhaltung
        /// </summary>
        private void pbBuchhaltung_Click(object sender, RoutedEventArgs e)
        {
            SubnavigationPage accountingSubnav = new SubnavigationPage(pbBuchhaltung.Content.ToString());

            #region QuickBooking
            Type pageType1 = typeof(KöTaf.WPFApplication.Views.Accounting.QuickBooking.pQuickBooking);
            accountingSubnav.addSubnavigation(IniParser.GetSetting("ACCOUNTING", "quickBooking"), pageType1);
            #endregion

            #region Bookings
            Type pageType2 = typeof(KöTaf.WPFApplication.Views.Accounting.Bookings.pBookings);
            accountingSubnav.addSubnavigation(IniParser.GetSetting("ACCOUNTING", "bookings"), pageType2);
            #endregion

            #region Sums
            Type pageType3 = typeof(KöTaf.WPFApplication.Views.Accounting.Sums.pSums);
            accountingSubnav.addSubnavigation(IniParser.GetSetting("ACCOUNTING", "sums"), pageType3);
            #endregion

            #region CashClosure
            Type pageType4 = typeof(pCashClosureManager);
            accountingSubnav.addSubnavigation(IniParser.GetSetting("ACCOUNTING", "cashClosure"), pageType4);
            #endregion

        }

        /// <summary>
        /// Verwaltung
        /// </summary>
        private void pbAdministration_Click(object sender, RoutedEventArgs e)
        {
            if (UserSession.userAccount.IsAdmin)
            {
                SubnavigationPage subNavPage = new SubnavigationPage(IniParser.GetSetting("APPSETTINGS", "administration"));

                Type pageType1 = typeof(KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration);
                subNavPage.addSubnavigation(IniParser.GetSetting("FORMLETTER", "formletterAdmin"), pageType1);

                Type pageType2 = typeof(KöTaf.WPFApplication.Views.Restore);
                subNavPage.addSubnavigation(IniParser.GetSetting("APPSETTINGS", "restore"), pageType2);

                Type pageType3 = typeof(KöTaf.WPFApplication.Views.User.pUserManager);
                subNavPage.addSubnavigation(IniParser.GetSetting("APPSETTINGS", "userAdministration"), pageType3);

                Type pageType4 = typeof(KöTaf.WPFApplication.Views.Accounting.AccountManager.pAccountManager);
                subNavPage.addSubnavigation(IniParser.GetSetting("ACCOUNTING", "accountManagement"), pageType4);
            }
        }

        /// <summary>
        /// Druck
        /// </summary>
        private void pbPrint_Click(object sender, RoutedEventArgs e)
        {
            KPage printPage = new Print();
            SinglePage print = new SinglePage(IniParser.GetSetting("APPSETTINGS", "print"), printPage);
        }

        /// <summary>
        /// Programm beenden
        /// </summary>
        private void pbClose_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new KöTaf.WPFApplication.Views.CloseProgram();
        }

        /// <summary>
        /// Abmelden
        /// </summary>
        private void pbLogOut_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new KöTaf.WPFApplication.Views.LoginWindow();
            App.Current.Windows[0].Close();
            newWindow.ShowDialog();
        }

        /// <summary>
        /// Listen
        /// </summary>
        private void pbLists_Click(object sender, RoutedEventArgs e)
        {
            // Load the list initial view
            // Simple Page with tabs
            // The toolbar must be built in the pLists.cs file itself

            Cursor = Cursors.Wait;
            KPage pageLists = new KöTaf.WPFApplication.Views.Lists.pLists();
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Statistiken
        /// </summary>
        private void pbStatistic_Click(object sender, RoutedEventArgs e)
        {
            // Load the generalStatistic page
            // Simple Page
            // The toolbar must be built in the pGeneralStatistic.cs file itself

            Cursor = Cursors.Wait;
            KPage pageStatistic = new KöTaf.WPFApplication.Views.Statistic.pGeneralStatistic();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "statistic"), pageStatistic);
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Notizen
        /// </summary>
        private void pbNotes_Click(object sender, RoutedEventArgs e)
        {
            KPage pageNoteAdmin = new KöTaf.WPFApplication.Views.pNoteAdministration(); ;
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "notes"), pageNoteAdmin);
        }

        #endregion

        /// <summary>
        /// Seite initialisieren
        /// </summary>
        public void initPage(){
            KPage welcomeScr = new KöTaf.WPFApplication.Views.pWelcomeScreen();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "welcome"), welcomeScr);
        }


        #region Template-Logik

        /// <summary>
        /// Die anzuzeigende Seite wechseln
        /// Erstellt neue Instanzen anstatt existierende wieder zu verwenden, da es Probleme mit dem .net Framework gab
        /// </summary>
        /// <param name="label">Der Titel der neuen Seite</param>
        /// <param name="pageType">Der Typ der neuen Seite</param>
        /// <param name="args">Parameter für den Konstruktor der neuen Seite</param>
        public void switchPage(string label, Type pageType, params object[] args)
        {
            try
            {
                KPage page = (KPage)Activator.CreateInstance(pageType, args);

                Frame frame = new Frame();
                Toolbar toolbar = new Toolbar(frame, page);
                ScrollableFrame scrollableFrame = new ScrollableFrame();
                ExtScrollViewer extScrollViewer = scrollableFrame.createScrollableFrame(frame);
                frame.Margin = new Thickness(15, 0, 0, 0);

                DockPanel.SetDock(toolbar.dpToolbarPanel, Dock.Top);

                frame.Content = page;
                page.parentFrame = frame;
                page.parentToolbar = toolbar;
                page.parentScrollViewer = extScrollViewer;
                page.defineToolbarContent();

                this.contentPanel.Children.Clear();
                this.contentPanel.Children.Add(toolbar.dpToolbarPanel);
                this.contentPanel.Children.Add(extScrollViewer);
                this.lbPageTitle.Content = label;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Die anzuzeigende Seite wechseln
        /// </summary>
        /// <param name="label">Der Titel der neuen Seite</param>
        /// <param name="tabControl">Den zu ladenden TabControl</param>
        public void switchPage(string label, TabControl tabControl)
        {
            try
            {
                this.contentPanel.Children.Clear();
                this.contentPanel.Children.Add(tabControl);
                this.lbPageTitle.Content = label;
            }
            catch
            {
            }
        }

        #endregion
    }
}
