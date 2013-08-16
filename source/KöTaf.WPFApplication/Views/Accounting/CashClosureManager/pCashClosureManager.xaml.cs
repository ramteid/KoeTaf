using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.UserSession;
using System.Threading;

namespace KöTaf.WPFApplication.Views.Accounting.CashClosureManager
{
    /// <summary>
    /// Author: Dietmar Sach
    /// </summary>
    public partial class pCashClosureManager : KPage
    {
        public pCashClosureManager()
        {
            InitializeComponent();
            createButtonToolTips();
            refreshCashClosureDataGrid();
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("ACCOUNTING", "createCashClosure"), createCashClosure);
        }

        /// <summary>
        /// Lade Bezeichnung für Kassenabschluss und Kasenabrechnung aus der Konfigurationsdatei
        /// </summary>
        private void createButtonToolTips()
        {
            try
            {
                var btn1 = FindResource("printCashClosureReport") as Button;
                btn1.ToolTip = IniParser.GetSetting("CASHCLOSURE", "cashClosureReportDocument");

                var btn2 = FindResource("printCashClosureReceipt") as Button;
                btn1.ToolTip = IniParser.GetSetting("CASHCLOSURE", "cashClosureReceiptDocument");
            }
            catch
            {
            }
        }

        /// <summary>
        /// Liste der Kassenabschlüsse aktualisieren
        /// </summary>
        private void refreshCashClosureDataGrid()
        {
            IEnumerable<CashClosure> cashClosures = CashClosure.GetCashClosures();
            IEnumerable<CashClosureReport> cashClosureReports = CashClosureReport.GetCashClosureReports();
            IEnumerable<CashClosureReceipt> cashClosureReceipts = CashClosureReceipt.GetCashClosureReceipts();
            List<CashClosureManagerDataGridModel> cashClosureDataGridModels = new List<CashClosureManagerDataGridModel>();

            foreach (var cashClosure in cashClosures)
            {
                if (cashClosure == null)
                    return;

                try
                {
                    CashClosureReport cashClosureReport = cashClosureReports.Where(c => c.CashClosure.CashClosureID == cashClosure.CashClosureID).FirstOrDefault();
                    CashClosureReceipt cashClosureReceipt = cashClosureReceipts.Where(c => c.CashClosure.CashClosureID == cashClosure.CashClosureID).FirstOrDefault();

                    if (cashClosureReport == null || cashClosureReceipt == null)
                        return;

                    cashClosureDataGridModels.Add(new CashClosureManagerDataGridModel(cashClosure, cashClosureReport, cashClosureReceipt));
                }
                catch 
                { }

            }
            CashClosureDataGrid.ItemsSource = cashClosureDataGridModels;
            CashClosureDataGrid.Items.Refresh();
        }

        /// <summary>
        /// Kasse schliessen
        /// </summary>
        /// <param name="btn">sendender Button</param>
        private void createCashClosure(Button btn)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pNewCashClosure);
            mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "cashClosure"), pageType);
        }

        /// <summary>
        /// Kassenabrechnung drucken
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printCashClosureReceipt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int closureID = (int)(((Button)sender).CommandParameter);

                if (!(closureID >= 0))
                    throw new Exception();

                CashClosureReceipt closureReceipt = CashClosureReceipt.GetCashClosureReceipts().Where(c => c.CashClosure.CashClosureID == closureID).FirstOrDefault();
                CashClosure cashClosure = CashClosure.GetCashClosures(closureID).FirstOrDefault();
                DateTime date = cashClosure.ClosureDate;
                string dateTime = SafeStringParser.safeParseToStr(date, true);
                bool isReprint = closureReceipt.PrintDone;

                MessageBoxResult result;

                // Wenn bereits das Original gedruckt wurde, drucke automatisch den Nachdruck
                if (isReprint)
                    result = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionReceiptCopyPrint").Replace("{0}", dateTime));
                else
                    result = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionReceiptOriginalPrint").Replace("{0}", dateTime));

                // Wenn der Benutzer möchte, kann ausgedruckt werden
                if (result == MessageBoxResult.Yes)
                    PrintForms.printCashClosureReceipt(closureID, isReprint);
                else
                    return;

                // Wenn Nachdruck, dann ändere nichts an dem Datensatz
                if (isReprint)
                    return;

                // Damit die Nachfrage nicht sofort aufploppt
                Thread.Sleep(1000);

                MessageBoxResult success = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionPrintReceiptSuccess"));

                if (success == MessageBoxResult.Yes)
                {
                    int receiptID = closureReceipt.CashClosureReceiptID;
                    bool printDone = true;
                    DateTime printDate = DateTime.Now;
                    int printUserID = UserSession.userAccountID;

                    CashClosureReceipt.Update(receiptID, closureID, printDone, printDate, printUserID);
                    refreshCashClosureDataGrid();
                }
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "printCashClosureReceipt"));
            }
        }

        /// <summary>
        /// Kassenabschlussbericht drucken
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printCashClosureReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int closureID = (int)(((Button)sender).CommandParameter);

                if (!(closureID >= 0))
                    throw new Exception();

                CashClosureReport closureReport = CashClosureReport.GetCashClosureReports().Where(c => c.CashClosure.CashClosureID == closureID).FirstOrDefault();
                CashClosure cashClosure = CashClosure.GetCashClosures(closureID).FirstOrDefault();
                DateTime date = cashClosure.ClosureDate;
                string dateTime = SafeStringParser.safeParseToStr(date, true);
                bool isReprint = closureReport.PrintDone;

                MessageBoxResult result;

                // Wenn bereits das Original gedruckt wurde, drucke automatisch den Nachdruck
                if (isReprint) 
                    result = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionReportCopyPrint").Replace("{0}", dateTime));
                else
                    result = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionReportOriginalPrint").Replace("{0}", dateTime));

                // Wenn der Benutzer möchte, kann ausgedruckt werden
                if (result == MessageBoxResult.Yes)
                    PrintForms.printCashClosureReport(closureID, isReprint);
                else
                    return;

                // Wenn Nachdruck, dann ändere nichts an dem Datensatz
                if (isReprint)
                    return;

                // Damit die Nachfrage nicht sofort aufploppt
                Thread.Sleep(1000);

                MessageBoxResult success = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionPrintReportSuccess"));

                if (success == MessageBoxResult.Yes)
                {
                    int reportID = closureReport.CashClosureReportID;
                    bool printDone = true;
                    DateTime printDate = DateTime.Now;
                    int printUserID = UserSession.userAccountID;
                    bool reportDone = closureReport.Done;
                    DateTime? reportDoneDate = closureReport.DoneDate;

                    int? reportDoneUser;
                    if (closureReport.DoneUserAccount != null)
                        reportDoneUser = closureReport.DoneUserAccount.UserAccountID;
                    else
                        reportDoneUser = null;

                    CashClosureReport.Update(reportID, closureID, printDone, printDate, printUserID, reportDone, reportDoneDate, reportDoneUser);
                    refreshCashClosureDataGrid();
                }
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "printCashClosureReport"));
            }
        }

        /// <summary>
        /// Kassenabschlussbericht als erledigt markieren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportDone_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cbSender = (CheckBox)sender;
                int closureID = (int)(cbSender).CommandParameter;

                if (!(closureID >= 0))
                    throw new Exception();

                CashClosureReport closureReport = CashClosureReport.GetCashClosureReports().Where(c => c.CashClosure.CashClosureID == closureID).FirstOrDefault();

                if (!closureReport.PrintDone)
                {
                    cbSender.IsChecked = false;
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "cashClosureReportDoneNotPrinted"));
                    return;
                }

                CashClosure cashClosure = CashClosure.GetCashClosures(closureID).FirstOrDefault();
                DateTime date = cashClosure.ClosureDate;
                string dateTime = SafeStringParser.safeParseToStr(date, true);
                bool isReprint = closureReport.PrintDone;

                MessageBoxResult result = MessageBoxEnhanced.Question(IniParser.GetSetting("CASHCLOSURE", "questionMarkDone").Replace("{0}", dateTime));

                if (result == MessageBoxResult.Yes)
                {
                    int reportID = closureReport.CashClosureReportID;
                    bool printDone = closureReport.PrintDone;
                    DateTime printDate = (DateTime)closureReport.PrintDate;
                    int printUserID = closureReport.PrintUserAccount.UserAccountID;
                    bool reportDone = true;
                    DateTime? reportDoneDate = DateTime.Now;
                    int reportDoneUser = UserSession.userAccountID;

                    CashClosureReport.Update(reportID, closureID, printDone, printDate, printUserID, reportDone, reportDoneDate, reportDoneUser);
                    refreshCashClosureDataGrid();
                }
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "doneCashClosureReport"));
            }
        }

        /// <summary>
        /// Wird beim Wechsel der Sichtbarkeit aufgerufen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((KPage)sender).IsVisible)     // Beim Verlassen der Seite nichts machen
                return;

            refreshCashClosureDataGrid();
        }

    }
}
