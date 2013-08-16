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
using KöTaf.WPFApplication.Models;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.UserSession;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Accounting.CashClosureManager
{
    /// <summary>
    /// Author: Dietmar Sach
    /// </summary>
    public partial class pNewCashClosure : KPage
    {
        private double revenuesTotal;
        private double expensesTotal;
        
        private List<CashClosureAccountsDataGridModel> accountModels;

        public pNewCashClosure()
        {
            this.accountModels = new List<CashClosureAccountsDataGridModel>();
            InitializeComponent();
            refreshDataGrid();
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("ACCOUNTING", "closeCash"), submitCashClosure);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), Cancel_Button);
        }

        /// <summary>
        /// DataGrid aktualisieren
        /// </summary>
        private void refreshDataGrid()
        {
            CashClosureHelper cashClosureHelper = new CashClosureHelper();
            List<Account> closureAccounts = cashClosureHelper.closureAccounts;
            this.accountModels.Clear();

            double sumRevenues = 0;
            double sumExpenses = 0;
            double sumNewBalances = 0;

            // Berechne für jedes Kassenschluss-Konto die Summen
            foreach (var account in closureAccounts)
            {
                double oldBalance = cashClosureHelper.getOldBalanceForAccount(account.AccountID);
                double revenues = cashClosureHelper.getRevenuesForAccount(account.AccountID);
                double expenses = cashClosureHelper.getExpensesForAccount(account.AccountID);
                double newBalance = oldBalance + revenues - expenses;

                CashClosureAccountsDataGridModel model = new CashClosureAccountsDataGridModel(account, oldBalance, revenues, expenses, newBalance);
                accountModels.Add(model);

                sumRevenues += revenues;
                sumExpenses += expenses;
                sumNewBalances += newBalance;
            }
            AccountsDataGrid.ItemsSource = accountModels;
            AccountsDataGrid.Items.Refresh();

            // Zeige Gesamt-Summen an
            lbRevenues.Content = SafeStringParser.safeParseToMoney(sumRevenues, true);
            lbExpenses.Content = SafeStringParser.safeParseToMoney(sumExpenses, true);
            lbSum.Content = SafeStringParser.safeParseToMoney(sumNewBalances, true);

            this.expensesTotal = sumExpenses;
            this.revenuesTotal = sumRevenues;
        }

        /// <summary>
        /// Kassenabschluss durchführen
        /// </summary>
        /// <param name="btn">sendender Button</param>
        private void submitCashClosure(Button btn)
        {
            MessageBoxResult result = MessageBoxEnhanced.Warning(IniParser.GetSetting("ACCOUNTING", "cashClosureWarning"));
            if (result == MessageBoxResult.No)
                return;

            string comment = tbComment.Text;

            int cashClosureID = -1;
            int cashClosureReportID = -1;
            int cashClosureReceiptID = -1;
            CashClosureHelper cashClosureHelper = new CashClosureHelper();

            try
            {
                // Lege CashClosure, CashClosureReport und CashClosureReceipt an
                cashClosureID = CashClosure.Add(UserSession.userAccountID, DateTime.Now, this.revenuesTotal, this.expensesTotal, comment);
                cashClosureReportID = CashClosureReport.Add(cashClosureID, false, null, false, null, null, null);
                cashClosureReceiptID = CashClosureReceipt.Add(cashClosureID, false, null, null);

                // Setze für Kassenabschluss-Konten NewBalance als LatestBalance 
                foreach (var accModel in this.accountModels)
                {
                    double latestBalance = accModel.newBalanceDOUBLE;
                    Account.Update(accModel.account.AccountID, accModel.account.Name, accModel.account.Number, accModel.account.ZeroPeriodEnum, accModel.account.IsOfficial, accModel.account.Description, latestBalance, accModel.account.IsCapital, accModel.account.IsFixed);
                }

                // Setze für alle anderen Konten den LatestBalance auf 0 bei Überschreitung des Nullzeitraums
                List<Account> noClosureAccounts = Account.GetAccounts().Where(a => !(a.IsOfficial && a.IsCapital)).ToList();
                foreach (var account in noClosureAccounts)
                {
                    double latestBalance;
                    if (CashClosureHelper.exceededZeroPeriod(account.ZeroPeriodEnum))
                        latestBalance = 0.0;
                    else
                        latestBalance = account.LatestBalance;
                    Account.Update(account.AccountID, account.Name, account.Number, account.ZeroPeriodEnum, account.IsOfficial, account.Description, latestBalance, account.IsCapital, account.IsFixed);
                }

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pCashClosureManager);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "cashClosure"), pageType);
            }
            catch
            {
                try
                {
                    if (cashClosureID != -1)
                        CashClosure.Delete(cashClosureID);
                    if (cashClosureReportID != -1)
                        CashClosureReport.Delete(cashClosureReportID);
                    if (cashClosureReceiptID != 1)
                        CashClosureReceipt.Delete(cashClosureReceiptID);
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "newCashClosure"));
                }
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "newCashClosure"));
            }
        }

        /// <summary>
        /// Abbrechen
        /// </summary>
        /// <param name="button"></param>
        private void Cancel_Button(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pCashClosureManager);
            mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "cashClosure"), pageType);
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

            refreshDataGrid();
        }

    }
}
