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

namespace KöTaf.WPFApplication.Views.Accounting.AccountManager
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Logik für den AccountManager:
    ///     - Füllen des DataGrid
    ///     - Paging
    ///     - Event-Handling
    /// </summary>
    public partial class pAccountManager : KPage
    {

        #region Fields
        private DataGridPaging<AccountManagerDataGridModel> dataGridPaging;
        private List<AccountManagerDataGridModel> accounts;
        #endregion

        #region Constructor
        public pAccountManager()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialisiert alle Variablen
        /// </summary>
        private void Init()
        {
            this.accounts = new List<AccountManagerDataGridModel>();
            generateUnfilteredData();
            this.accounts = this.accounts.OrderByDescending(t => t.accountNumber).ToList();
            this.dataGridPaging = new DataGridPaging<AccountManagerDataGridModel>(this.accounts);
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            List<AccountSearchComboBoxItemModel> searchItems = new List<AccountSearchComboBoxItemModel>()
            {
                new AccountSearchComboBoxItemModel { Value = IniParser.GetSetting("ACCOUNTING", "accountName"), SearchType = AccountSearchComboBoxItemModel.Type.AccountName },
                new AccountSearchComboBoxItemModel { Value = IniParser.GetSetting("ACCOUNTING", "accountNumber"), SearchType = AccountSearchComboBoxItemModel.Type.AccountNumber },
            };
            this.parentToolbar.addButton(IniParser.GetSetting("ACCOUNTING", "newAccount"), NewAccount_Click);
            this.parentToolbar.addPagingBar(firstSideProcessor, prevSideProcessor, nextSideProcessor, lastSideProcessor);
            
            this.parentToolbar.addSearchPanel(processKeyUp);
            
            this.parentToolbar.searchPanel.addComboBox<AccountSearchComboBoxItemModel>(searchItems);
            
            // Der Textbox eine KeyUp-Funktion zuweisen
            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);
            
            // DataGrid kann erst jetzt gefüllt werden, damit das Paging die PagingBar ansprechen kann.
            this.fillAccountDataGrid(this.dataGridPaging.FirstSide().ToList());

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                AccountDataGrid.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
            
        }

        /// <summary>
        /// Generiert Daten fürs DataGrid
        /// </summary>
        private void generateUnfilteredData()
        {
            this.accounts.Clear();
            IEnumerable<Account> accounts = Account.GetAccounts();
            foreach (var account in accounts)
                this.accounts.Add(new AccountManagerDataGridModel(account));
        }

        /// <summary>
        /// Füllt das DataGrid
        /// </summary>
        /// <param name="accounts">Liste mit allen Konten-Modellen</param>
        private void fillAccountDataGrid(List<AccountManagerDataGridModel> accounts)
        {
            AccountDataGrid.ItemsSource = accounts;
            AccountDataGrid.Items.Refresh();
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
            AccountSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<AccountSearchComboBoxItemModel>();
            List<AccountManagerDataGridModel> accounts = this.accounts;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.SearchType)
                {
                    case AccountSearchComboBoxItemModel.Type.AccountName:
                        accounts = this.accounts.Where(a => a.accountName.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;

                    case AccountSearchComboBoxItemModel.Type.AccountNumber:
                        accounts = this.accounts.Where(a => a.accountNumber.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;

                }
            }
            // dataGridPaging mit neuem Listeninhalt neu initialisieren
            this.dataGridPaging = new DataGridPaging<AccountManagerDataGridModel>(accounts);
            fillAccountDataGrid(this.dataGridPaging.FirstSide().ToList());
        }

        /// <summary>
        /// Öffnet das Formular zum Anlegen eines neuen Kontos
        /// </summary>
        /// <param name="button"></param>
        private void NewAccount_Click(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pNewAccount);
            mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "newAccount"), pageType);
        }

        /// <summary>
        /// Öffnet das Formular zum Bearbeiten eines Kontos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accountID = (int)((Button)sender).CommandParameter;
                List<Account> account = Account.GetAccounts(accountID).ToList();
                Account currentAccount = account[0];

                if (currentAccount == null)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "loadAccount"));

                if (currentAccount.IsFixed)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "isFixedAccount"));

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pEditAccount);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "editAccount"), pageType, currentAccount);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
            }
        }

        /// <summary>
        /// Löscht ein Konto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accountID = (int)((Button)sender).CommandParameter;
                List<Account> account = Account.GetAccounts(accountID).ToList();
                Account currentAccount = account[0];

                if (currentAccount == null)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "deleteAccount"));

                if (currentAccount.IsFixed)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "deleteFixedAccount"));

                Account.Delete(currentAccount.AccountID);

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pAccountManager);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "accountManagement"), pageType);
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
            fillAccountDataGrid(this.dataGridPaging.PrevSide().ToList());
        }

        /// <summary>
        /// Geht eine Seite weiter
        /// </summary>
        /// <param name="str"></param>
        private void nextSideProcessor(String str)
        {
            fillAccountDataGrid(this.dataGridPaging.NextSide().ToList());
        }

        /// <summary>
        /// Geht zur ersten Seite
        /// </summary>
        /// <param name="str"></param>
        private void firstSideProcessor(String str)
        {
            fillAccountDataGrid(this.dataGridPaging.FirstSide().ToList());
        }

        /// <summary>
        /// Geht zur letzten Seite
        /// </summary>
        /// <param name="str"></param>
        private void lastSideProcessor(String str)
        {
            fillAccountDataGrid(this.dataGridPaging.LastSide().ToList());
        }

        /// <summary>
        /// Wird bei Seitenwechsel aufgerufen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((KPage)sender).IsVisible)     // Beim Verlassen der Seite nichts machen
                return;

            generateUnfilteredData();
            fillAccountDataGrid(this.accounts);
        }

        #endregion

    }
}