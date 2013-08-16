/**
 * Class: LoginWindow
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-18
 * 
 * Last modification: 2013-05-14 / Michael Müller
 */
using System.Windows;
using KöTaf.WPFApplication.ViewModels;
using System.Threading;
using System.Windows.Threading;
using System;
using KöTaf.Utils.UserSession;
using KöTaf.Utils.Parser;
using System.Collections.Generic;
using KöTaf.WPFApplication.Models;
using KöTaf.DataModel;
using System.Linq;
using KöTaf.WPFApplication.Helper;

namespace KöTaf.WPFApplication.Views
{
    public partial class LoginWindow
    {
        #region Fields
        public LoginViewModel ViewModel;
        DispatcherTimer dispatcherTimer = null;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor, lade und initialiere Login
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
            this.Visibility = System.Windows.Visibility.Hidden;
            this.ViewModel = new LoginViewModel();
            this.DataContext = this.ViewModel;

            // Neutralisiere User-Session
            UserSession.userAccountID = -1;
            UserSession.userName = "";
            UserSession.isAdmin = false;
            UserSession.userAccount = null;

            lbCreatedBy.Content = IniParser.GetSetting("APPSETTINGS", "createdBy");
        }

        #endregion

        #region Event handler

        /// <summary>
        /// Sperre andere Fenster bis Login abgeschlossen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            this.SmartLoginOverlayControl.Lock();
        }

        #endregion

        private void SmartLoginOverlayControl_Loaded(object sender, RoutedEventArgs e)
        {
            button1.IsEnabled = true;
        }

        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }
        /// <summary>
        /// Schließe Login Fenster / Programm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new KöTaf.WPFApplication.Views.CloseProgram();
            App.Current.Windows[0].Close();
        }
        /// <summary>
        /// Verlasse Login Fenster
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbClose_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new KöTaf.WPFApplication.Views.CloseProgram();
        }

        /// <summary>
        /// starte den Dispatcher Timer;Ist für einen wiederkehrenden 
        /// Prozess (Fenster anzeigen) realisiert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTime_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }

        /// <summary>
        /// Stoppe Dispatcher Timer, Zeige dezeitiges Objekt an.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTime_Tick(object sender, EventArgs e)
        {
                dispatcherTimer.Stop();
                if (!checkAccount()) {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "dbAccountFail"));
                    System.Environment.Exit(1);
                }
                this.Show();
        }
        #region
        /// <summary>
        /// Prüft ob gültige Accounts in der Datenbank vorhanden sind.
        /// </summary>
        /// <returns></returns>
        private bool checkAccount() 
        {
            try
            {
                int numberOfAccs = UserAccount.GetUserAccounts().Count();
                return (numberOfAccs > 0);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
