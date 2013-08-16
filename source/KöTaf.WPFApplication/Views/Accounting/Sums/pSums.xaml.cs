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
using KöTaf.Utils.FileOperations;
using KöTaf.Utils.Printer;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Accounting.Sums
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Kontensummen
    /// </summary>
    public partial class pSums : KPage
    {
        private List<BookingDataGridModel> bookingModelsUnchanged;
        private DateTime dateFrom;
        private DateTime dateTo;
        private ComboBox cbFilterAccount;
        private int? currentAccountNumber;
        private DatePicker datePickerFrom;
        private DatePicker datePickerTo;

        public pSums()
        {
            InitializeComponent();

            this.datePickerFrom = new DatePicker();
            this.datePickerTo = new DatePicker();

            cbFilterAccount = new ComboBox();
            cbFilterAccount.MinWidth = 150;
            cbFilterAccount.SelectionChanged += filterAccounts_SelectionChanged;

            generateDataGridDataUnfiltered();
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            refreshCbFilterAccount();

            WrapPanel filterWrapPanel = new WrapPanel();
            filterWrapPanel.Margin = new Thickness(30, 10, 0, 10);
            Label lb = new Label();
            lb.Content = IniParser.GetSetting("ACCOUNTING", "labelFilterAccounts");

            filterWrapPanel.Children.Add(lb);
            filterWrapPanel.Children.Add(cbFilterAccount);

            //this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "print"), pbListPrint_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "reset"), resetFilter);
            this.parentToolbar.dpToolbarPanel.Children.Add(filterWrapPanel);
            this.parentToolbar.addDateRangeSelector(dateFromProcessingFunction, dateToProcessingFunction, ref this.datePickerFrom, ref this.datePickerTo);

            List<BookingSumsSearchComboBoxItemModel> searchItems = new List<BookingSumsSearchComboBoxItemModel>()
            {
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Quellkontoname },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Zielkontoname },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Quellkontonummer },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Zielkontonummer },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Betrag },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Beschreibung }
            };
            this.parentToolbar.addSearchPanel(processKeyUp);
            this.parentToolbar.searchPanel.addComboBox<BookingSumsSearchComboBoxItemModel>(searchItems);

            // Der Textbox eine KeyUp-Funktion zuweisen
            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                SumsDataGrid.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;

        }

        /// <summary>
        /// Ungefilterte Buchungen abrufen
        /// </summary>
        private void generateDataGridDataUnfiltered()
        {
            this.bookingModelsUnchanged = new List<BookingDataGridModel>();
            List<Booking> bookings = Booking.GetBookings().ToList();
            DateTime lastCashClosure = BookingsHelper.getDateOfLastCashClosure();

            foreach (var booking in bookings)
                this.bookingModelsUnchanged.Add(new BookingDataGridModel(booking, lastCashClosure));
        }

        /// <summary>
        /// Aktualisiert die Konten-Liste in der Konten-ComboBox
        /// </summary>
        private void refreshCbFilterAccount()
        {
            List<AccountComboBoxModel> accountModels = new List<AccountComboBoxModel>();
            List<Account> accounts = Account.GetAccounts().ToList();
            foreach (var account in accounts)
                if (account != null)
                    try
                    {
                        accountModels.Add(new AccountComboBoxModel(account));
                    }
                    catch
                    {
                    }

            accountModels.Add(new AccountComboBoxModel());

            cbFilterAccount.ItemsSource = accountModels;
        }

        /// <summary>
        /// Änderung der Konten-Auswahl bearbeiten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void filterAccounts_SelectionChanged(Object sender, EventArgs args)
        {
            try
            {
                ComboBox cbSender = sender as ComboBox;
                AccountComboBoxModel accountModel = cbSender.SelectedItem as AccountComboBoxModel;
                this.currentAccountNumber = accountModel.accountNumber;

                // falls durch ein Event diese Funktion mit SelectedItem = null ausgelöst werden sollte
                if (accountModel == null)
                    return;

                // Filtere Buchungen nach ausgewähltem Konto
                List<BookingDataGridModel> bookingModels = filterBookingModels(this.bookingModelsUnchanged);
                refreshDataGrid(bookingModels);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Buchungen nach Konto filtern
        /// </summary>
        /// <param name="bookingModels"></param>
        /// <returns></returns>
        private List<BookingDataGridModel> filterBookingModels(List<BookingDataGridModel> bookingModels)
        {
            List<BookingDataGridModel> returnBookingModels;
            if (this.currentAccountNumber == -1)
                // Filtere Eigenkapital-relevanten Buchungen: Wo entweder Ziel- oder Quellkonto Eigenkapitalkonten sind
                returnBookingModels = bookingModels.Where(p => p.booking.SourceAccount.IsCapital ^ p.booking.TargetAccount.IsCapital).ToList();
            else
                // Filtere nach ausgewähltem Konto
                returnBookingModels = bookingModels.Where(p => p.sourceAccountNumberINT == this.currentAccountNumber ||
                                                          p.targetAccountNumberINT == this.currentAccountNumber).ToList();
            return returnBookingModels;
        }

        /// <summary>
        /// Suchfunktion
        /// </summary>
        /// <param name="searchValue"></param>
        private void processKeyUp(string searchValue)
        {
            BookingSumsSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<BookingSumsSearchComboBoxItemModel>();
            List<BookingDataGridModel> bookingModels = this.bookingModelsUnchanged;

            // Suche nur, wenn ein Konto angegeben ist
            if (this.currentAccountNumber == null)
                return;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.type)
                {
                    case BookingSumsSearchComboBoxItemModel.Type.Quellkontoname:
                        bookingModels = bookingModels.Where(p => p.sourceAccountName.ToLower().Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Zielkontoname:
                        bookingModels = bookingModels.Where(p => p.targetAccountName.ToLower().Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Quellkontonummer:
                        bookingModels = bookingModels.Where(p => p.sourceAccountNumber.Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Zielkontonummer:
                        bookingModels = bookingModels.Where(p => p.targetAccountNumber.Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Betrag:
                        bookingModels = bookingModels.Where(p => p.amount.ToString().Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Beschreibung:
                        bookingModels = bookingModels.Where(p => p.description.ToLower().Contains(searchValue)).ToList();
                        break;
                }
            }
            refreshDataGrid(bookingModels);
        }

        /// <summary>
        /// Datum "Von" aktualisieren
        /// </summary>
        /// <param name="dateFrom">Datum "von"</param>
        private void dateFromProcessingFunction(DateTime dateFrom)
        {
            this.dateFrom = BookingsHelper.makeDateSmall(dateFrom);

            // Filtere nach Datum
            List<BookingDataGridModel> bookingModels = this.bookingModelsUnchanged.Where(b => this.dateFrom < b.date && b.date < this.dateTo).ToList();

            // Filtere Buchungen nach ausgewähltem Konto
            bookingModels = filterBookingModels(bookingModels);
            refreshDataGrid(bookingModels);
        }

        /// <summary>
        /// Datum "Bis" aktualisieren
        /// </summary>
        /// <param name="dateTo">Datum "bis"</param>
        private void dateToProcessingFunction(DateTime dateTo)
        {
            this.dateTo = BookingsHelper.makeDateGreat(dateTo);

            // Filtere nach Datum
            List<BookingDataGridModel> bookingModels = this.bookingModelsUnchanged.Where(b => this.dateFrom < b.date && b.date < this.dateTo).ToList();

            // Filtere Buchungen nach ausgewähltem Konto
            bookingModels = filterBookingModels(bookingModels);
            refreshDataGrid(bookingModels);

            refreshDataGrid(bookingModels.ToList());
        }

        /// <summary>
        /// Datum zurücksetzen
        /// </summary>
        public void resetDateRange()
        {
            // Zeige standardmäßig alle Buchungen des jeweiligen Kontos vom letzten Kassenschluss bis jetzt an
            this.dateFrom = BookingsHelper.getDateOfLastCashClosure();
            this.dateTo = BookingsHelper.makeDateGreat(DateTime.Today);
            this.datePickerFrom.SelectedDate = this.dateFrom;
            this.datePickerTo.SelectedDate = this.dateTo;
        }

        /// <summary>
        /// DataGrid mit Buchungen neu laden
        /// </summary>
        /// <param name="bookingModels">Liste mit Buchungs-Modellen</param>
        private void refreshDataGrid(List<BookingDataGridModel> bookingModels)
        {
            lbExpenses.Content = "";
            lbRevenues.Content = "";
            lbSum.Content = "";

            // Zeite nur Buchungen an wenn ein Konto angegeben ist
            if (this.currentAccountNumber == null)
                return;

            bookingModels = bookingModels.Where(b => this.dateFrom < b.date && b.date < this.dateTo).ToList();
            
            // Sortiere absteigend
            bookingModels = bookingModels.OrderByDescending(t => t.bookingID).ToList();

            SumsDataGrid.ItemsSource = bookingModels;
            SumsDataGrid.Items.Refresh();

            if (this.currentAccountNumber != null)
                calculateAndShowSums(bookingModels);
        }

        /// <summary>
        /// Summen berechnen
        /// </summary>
        /// <param name="bookingModels"></param>
        private void calculateAndShowSums(List<BookingDataGridModel> bookingModels)
        {
            double revenues, sum, expenses;

            // Wenn "alle Eigenkapitalkonten" ausgewählt, dann summiere alle Eigenkapitalkonten
            if (this.currentAccountNumber == -1)
            {
                // Einnahmen: Buchungen mit Eigenkapital-Konto als Zielkonto
                revenues = bookingModels.Where(b => b.targetAccountIsCapital).Sum(b => b.amountDOUBLE);

                // Ausgaben: Buchungen mit Eigenkapital-Konto als QuellKonto
                expenses = bookingModels.Where(b => b.sourceAccountIsCapital).Sum(b => b.amountDOUBLE);

                sum = revenues - expenses;
            }
            // Ansonsten summiere für das ausgewählte Konto
            else
            {
                // Einnahmen: Buchungen mit Filter-Konto als Zielkonto
                revenues = bookingModels.Where(b => b.targetAccountNumberINT == this.currentAccountNumber).Sum(b => b.amountDOUBLE);

                // Ausgaben: Buchungen mit Filter-Konto als QuellKonto
                expenses = bookingModels.Where(b => b.sourceAccountNumberINT == this.currentAccountNumber).Sum(b => b.amountDOUBLE);

                sum = revenues - expenses;
            }

            // Zeige Beträge an
            lbExpenses.Content = SafeStringParser.safeParseToMoney(expenses, true);
            lbRevenues.Content = SafeStringParser.safeParseToMoney(revenues, true);
            lbSum.Content = SafeStringParser.safeParseToMoney(sum, true);

            Account currentAccount = Account.GetAccounts(null, null, this.currentAccountNumber).FirstOrDefault();
            lbLatestBalance.Content = SafeStringParser.safeParseToMoney(currentAccount.LatestBalance, true);
        }

        /// <summary>
        /// Drucken
        /// </summary>
        /// <param name="button">Referenz zu sendenem Button</param>
        public void pbListPrint_Click(Button button)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
                return;
            }
            DataGrid allPrintData = new DataGrid();
            try
            {
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Client, allPrintData.ItemsSource);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Zurücksetzen
        /// </summary>
        /// <param name="btn">Referenz zu sendenem Button</param>
        private void resetFilter(Button btn)
        {
            resetDateRange();
            generateDataGridDataUnfiltered();
            refreshCbFilterAccount();
            cbFilterAccount.SelectedItem = null;
            refreshDataGrid(this.bookingModelsUnchanged);
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

            generateDataGridDataUnfiltered();
            resetDateRange();
            refreshCbFilterAccount();
        }
        
    }
}
