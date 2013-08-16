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
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.Parser;
using KöTaf.Utils.UserSession;

namespace KöTaf.WPFApplication.Views.User
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Logik für den UserManager:
    ///     - Füllen des DataGrid
    ///     - Paging
    ///     - Event-Handling
    /// </summary>
    public partial class pUserManager : KPage
    {

        #region Fields
        private IEnumerable<UserAccount> userAccounts;
        private DataGridPaging<UserAccount> dataGridPaging;
        #endregion

        #region Constructor
        public pUserManager()
        {
            InitializeComponent();
            this.userAccounts = UserAccount.GetUserAccounts().OrderByDescending(u => u.Username);
            this.dataGridPaging = new DataGridPaging<UserAccount>(this.userAccounts);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            List<UserAccountSearchComboBoxItemModel> searchItems = new List<UserAccountSearchComboBoxItemModel>()
            {
                new UserAccountSearchComboBoxItemModel { Value = IniParser.GetSetting("USER", "username"), SearchType = UserAccountSearchComboBoxItemModel.Type.Benutzername },
                new UserAccountSearchComboBoxItemModel { Value = IniParser.GetSetting("USER", "password"), SearchType = UserAccountSearchComboBoxItemModel.Type.Password },
            };
            this.parentToolbar.addButton(IniParser.GetSetting("USER", "newUser"), NewUser_Click);
            this.parentToolbar.addPagingBar(firstSideProcessor, prevSideProcessor, nextSideProcessor, lastSideProcessor);
            
            this.parentToolbar.addSearchPanel(processKeyUp);
            
            this.parentToolbar.searchPanel.addComboBox<UserAccountSearchComboBoxItemModel>(searchItems);
            
            // Der Textbox eine KeyUp-Funktion zuweisen
            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);
            
            // DataGrid kann erst jetzt gefüllt werden, damit das Paging die PagingBar ansprechen kann.
            this.fillUserDataGrid(this.dataGridPaging.FirstSide().ToList());

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                UserDataGrid.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
            
        }

        /// <summary>
        /// Füllt das DataGrid
        /// </summary>
        /// <param name="accounts">Liste mit allen Konten-Modellen</param>
        private void fillUserDataGrid(IEnumerable<UserAccount> accounts)
        {
            UserDataGrid.ItemsSource = accounts;
            UserDataGrid.Items.Refresh();
            this.ChangePagingBar();
        }

        /// <summary>
        /// PagingBar
        /// </summary>
        private void ChangePagingBar()
        {
            // Alle Buttons aktivieren
            this.parentToolbar.pagingBar.firstButton.IsEnabled = true;
            this.parentToolbar.pagingBar.prevButton.IsEnabled = true;
            this.parentToolbar.pagingBar.nextButton.IsEnabled = true;
            this.parentToolbar.pagingBar.lastButton.IsEnabled = true;

            // Label der PagingBar aktualisieren
            this.parentToolbar.pagingBar.fromBlock.Text = this.dataGridPaging.GetStart().ToString();
            this.parentToolbar.pagingBar.toBlock.Text = this.dataGridPaging.GetEnd().ToString();
            this.parentToolbar.pagingBar.totalBlock.Text = this.dataGridPaging.GetTotal().ToString();

            // Nur Buttons zulassen, die möglich sind
            if (this.dataGridPaging.GetStart() == 1 || this.dataGridPaging.GetStart() == 0)
            {
                this.parentToolbar.pagingBar.firstButton.IsEnabled = false;
                this.parentToolbar.pagingBar.prevButton.IsEnabled = false;
            }
            if (this.dataGridPaging.GetEnd() == this.dataGridPaging.GetTotal())
            {
                this.parentToolbar.pagingBar.nextButton.IsEnabled = false;
                this.parentToolbar.pagingBar.lastButton.IsEnabled = false;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Suchfunktion
        /// </summary>
        /// <param name="searchValue">Wert, nach dem gesucht werden soll</param>
        private void processKeyUp(string searchValue)
        {
            UserAccountSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<UserAccountSearchComboBoxItemModel>();
            IEnumerable<UserAccount> accounts = this.userAccounts;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.SearchType)
                {
                    case UserAccountSearchComboBoxItemModel.Type.Benutzername:
                        accounts = this.userAccounts.Where(u => u.Username.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;

                    case UserAccountSearchComboBoxItemModel.Type.Password:
                        accounts = this.userAccounts.Where(u => u.Password.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;
                }
            }
            // dataGridPaging mit neuem Listeninhalt neu initialisieren
            this.dataGridPaging = new DataGridPaging<UserAccount>(accounts);
            fillUserDataGrid(this.dataGridPaging.FirstSide().ToList());
        }

        /// <summary>
        /// Öffnet das Formular zum Anlegen eines neuen Benutzerkontos
        /// </summary>
        /// <param name="button"></param>
        private void NewUser_Click(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pNewUser);
            mainWindow.switchPage(IniParser.GetSetting("USER", "newUser"), pageType);
        }

        /// <summary>
        /// Öffnet das Formular zum Bearbeiten eines Benutzerkontos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserAccount currentUserAccount = UserDataGrid.SelectedItem as UserAccount;

                if (currentUserAccount == null)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "loadUserAccount"));

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pEditUser);
                mainWindow.switchPage(IniParser.GetSetting("USER", "editUser"), pageType, currentUserAccount);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
            }
        }

        /// <summary>
        /// Aktiviert oder deaktiviert einen Benutzer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleUserActivateStateButton_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                UserAccount currentUserAccount = UserDataGrid.SelectedItem as UserAccount;

                if (currentUserAccount != null)
                {
                    
                    if (currentUserAccount.IsAdmin && currentUserAccount.IsActive && UserAccount.GetUserAccounts().Where(u => u.IsAdmin).ToList().Count <= 1)
                        throw new Exception(IniParser.GetSetting("ERRORMSG", "deactivateAdmin"));

                    if (UserSession.userAccountID.Equals(currentUserAccount.UserAccountID))
                        throw new Exception(IniParser.GetSetting("ERRORMSG", "selfDeactivation"));

                    var state = currentUserAccount.IsActive;
                    var message = string.Format(IniParser.GetSetting("USER", "confirmationFormatString"),
                        currentUserAccount.Username,
                        ((state) ? IniParser.GetSetting("FILTER", "inactive") : IniParser.GetSetting("FILTER", "active")));

                    var dialogResult = MessageBox.Show(message, IniParser.GetSetting("USER", "confirmationNeeded"), MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dialogResult == MessageBoxResult.OK)
                    {
                        if (state)
                            UserAccount.Deactivate(currentUserAccount.UserAccountID);
                        else
                            UserAccount.Activate(currentUserAccount.UserAccountID);

                        this.userAccounts = UserAccount.GetUserAccounts();
                        if (this.userAccounts != null)
                            this.userAccounts.OrderByDescending(u => u.IsActive);

                        if (this.parentToolbar.searchPanel.searchBox.Text == IniParser.GetSetting("APPSETTINGS", "search"))
                            processKeyUp("");
                        else
                            processKeyUp(this.parentToolbar.searchPanel.searchBox.Text);
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBoxEnhanced.Error(ex.Message);
            }
        }

        /// <summary>
        /// Geht eine Seite zurück
        /// </summary>
        /// <param name="str"></param>
        private void prevSideProcessor(String str)
        {
            fillUserDataGrid(this.dataGridPaging.PrevSide().ToList());
        }

        /// <summary>
        /// Geht eine Seite weiter
        /// </summary>
        /// <param name="str"></param>
        private void nextSideProcessor(String str)
        {
            fillUserDataGrid(this.dataGridPaging.NextSide().ToList());
        }

        /// <summary>
        /// Geht zur ersten Seite
        /// </summary>
        /// <param name="str"></param>
        private void firstSideProcessor(String str)
        {
            fillUserDataGrid(this.dataGridPaging.FirstSide().ToList());
        }

        /// <summary>
        /// Geht zur letzten Seite
        /// </summary>
        /// <param name="str"></param>
        private void lastSideProcessor(String str)
        {
            fillUserDataGrid(this.dataGridPaging.LastSide().ToList());
        }

        private void KPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((KPage)sender).IsVisible)     // Beim Verlassen der Seite nichts machen
                return;

            this.userAccounts = UserAccount.GetUserAccounts().OrderByDescending(u => u.Username);
            fillUserDataGrid(this.userAccounts);
        }

        #endregion
    }
}