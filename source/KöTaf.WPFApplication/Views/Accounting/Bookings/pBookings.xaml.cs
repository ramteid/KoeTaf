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
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Models;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Accounting.Bookings
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Interaktionslogik für pBookings.xaml
    /// </summary>
    public partial class pBookings : KPage
    {
        #region Fields
        private DataGridPaging<BookingDataGridModel> dataGridPaging;
        private List<BookingDataGridModel> bookingModelsUnchanged;
        #endregion
        
        public pBookings()
        {
            InitializeComponent();
            this.bookingModelsUnchanged = new List<BookingDataGridModel>();
            generateDataGridDataUnfiltered();
        }

        #region Methods

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            List<BookingSumsSearchComboBoxItemModel> searchItems = new List<BookingSumsSearchComboBoxItemModel>()
            {
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Quellkontoname },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Zielkontoname },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Quellkontonummer },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Zielkontonummer },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Betrag },
                new BookingSumsSearchComboBoxItemModel { type = BookingSumsSearchComboBoxItemModel.Type.Beschreibung }
            };
            
            this.parentToolbar.addButton(IniParser.GetSetting("ACCOUNTING", "newBooking"), enterAccounting_Click);

            this.parentToolbar.addPagingBar(firstSideProcessor, prevSideProcessor, nextSideProcessor, lastSideProcessor);

            this.parentToolbar.addSearchPanel(processKeyUp);

            this.parentToolbar.searchPanel.addComboBox<BookingSumsSearchComboBoxItemModel>(searchItems);

            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);
        }

        /// <summary>
        /// Liste mit ungefilterten Daten generieren
        /// </summary>
        private void generateDataGridDataUnfiltered()
        {
            this.bookingModelsUnchanged.Clear();
            IEnumerable<Booking> bookings = Booking.GetBookings();
            DateTime lastCashClosure = BookingsHelper.getDateOfLastCashClosure();

            foreach (var booking in bookings)
                bookingModelsUnchanged.Add(new BookingDataGridModel(booking, lastCashClosure));
        }

        /// <summary>
        /// Füllt das DataGrid
        /// </summary>
        private void refreshDataGrid(List<BookingDataGridModel> bookingModels)
        {
            // Sortiere absteigend
            bookingModels = bookingModels.OrderByDescending(t => t.bookingID).ToList();
            
            this.dataGridPaging = new DataGridPaging<BookingDataGridModel>(bookingModels);
            AccountingRecordDataGrid.ItemsSource = this.dataGridPaging.FirstSide();
            AccountingRecordDataGrid.Items.Refresh();
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
        /// Sucht nach übergebenen Parameter und passt das DataGrid an
        /// </summary>
        /// <param name="searchValue">Suchwert</param>
        private void processKeyUp(string searchValue)
        {
            BookingSumsSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<BookingSumsSearchComboBoxItemModel>();
            List<BookingDataGridModel> bookingModels = this.bookingModelsUnchanged;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.type)
                {
                    case BookingSumsSearchComboBoxItemModel.Type.Quellkontoname:
                        bookingModels = bookingModels.Where(p => p.sourceAccountName.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Zielkontoname:
                        bookingModels = bookingModels.Where(p => p.targetAccountName.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Quellkontonummer:
                        bookingModels = bookingModels.Where(p => p.sourceAccountNumber.ToString().Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Zielkontonummer:
                        bookingModels = bookingModels.Where(p => p.targetAccountNumber.ToString().Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Betrag:
                        bookingModels = bookingModels.Where(p => p.amount.ToString().Contains(searchValue)).ToList();
                        break;
                    case BookingSumsSearchComboBoxItemModel.Type.Beschreibung:
                        bookingModels = bookingModels.Where(p => p.description.ToLower().Contains(searchValue.ToLower())).ToList();
                        break;
                }
            }
            // dataGridPaging mit neuem Listeninhalt neu initialisieren
            refreshDataGrid(bookingModels);
        }

        /// <summary>
        /// Öffnet das Formular zum Erfassen einer neuen Buchung
        /// </summary>
        /// <param name="button"></param>
        private void enterAccounting_Click(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pNewBooking);
            mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "newBooking"), pageType);
        }

        /// <summary>
        /// Führt eine Korrekturbuchung durch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Correct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int bookingID = (int)((Button)sender).CommandParameter;
                List<Booking> booking = Booking.GetBookings(bookingID).ToList();
                Booking currentBooking = booking[0];

                if (currentBooking == null)
                    throw new Exception();

                int result = NegativeBooking.correct(currentBooking);

                if (result == -1)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "negBookingMinusOne"));

                if (result == -2)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "negBookingMinusTwo"));

                if (result == -3)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "negBookingMinusThree"));

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pBookings);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "bookings"), pageType);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
            }
        }

        /// <summary>
        /// Öffnet das Formular zum Bearbeiten einer Buchung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int bookingID = (int)((Button)sender).CommandParameter;
                List<Booking> booking = Booking.GetBookings(bookingID).ToList();
                Booking currentBooking = booking[0];

                if (currentBooking == null)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "loadBooking"));

                if (currentBooking.IsCorrection)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "correctionEdit"));

                if (Booking.GetBookings().Where(b => b.IsCorrection).Where(b => b.Description.Contains("#" + currentBooking.BookingID.ToString())).ToList().Count > 0)
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "correctionExists"));

                if (currentBooking.Date <= BookingsHelper.getDateOfLastCashClosure())
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "bookingBeforeLastClosure"));

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pEditBooking);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "editBooking"), pageType, currentBooking);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
            }
        }

        /// <summary>
        /// Löscht eine Buchung
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int bookingID = (int)((Button)sender).CommandParameter;
                List<Booking> booking = Booking.GetBookings(bookingID).ToList();
                Booking currentBooking = booking[0];

                if (currentBooking == null)
                    throw new Exception();

                string date = SafeStringParser.safeParseToStr(currentBooking.Date, true);
                string warning = IniParser.GetSetting("ACCOUNTING", "warningBookingDelete").Replace("{0}", date);

                if (MessageBoxEnhanced.Warning(warning) == MessageBoxResult.No)
                    return;

                if (currentBooking.Date <= BookingsHelper.getDateOfLastCashClosure())
                    throw new Exception(IniParser.GetSetting("ERRORMSG", "bookingBeforeLastClosure"));

                Booking.Delete(currentBooking.BookingID);

                // Refresh page
                generateDataGridDataUnfiltered();
                refreshDataGrid(this.bookingModelsUnchanged);
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "deleteBooking"));
            }
        }

        /// <summary>
        /// Geht eine Seite zurück
        /// </summary>
        /// <param name="str"></param>
        private void prevSideProcessor(String str)
        {
            AccountingRecordDataGrid.ItemsSource = this.dataGridPaging.PrevSide();
            AccountingRecordDataGrid.Items.Refresh();
            ChangePagingBar();
        }

        /// <summary>
        /// Geht eine Seite vor
        /// </summary>
        /// <param name="str"></param>
        private void nextSideProcessor(String str)
        {
            AccountingRecordDataGrid.ItemsSource = this.dataGridPaging.NextSide();
            AccountingRecordDataGrid.Items.Refresh();
            ChangePagingBar();
        }

        /// <summary>
        /// Geht zur ersten Seite
        /// </summary>
        /// <param name="str"></param>
        private void firstSideProcessor(String str)
        {
            AccountingRecordDataGrid.ItemsSource = this.dataGridPaging.FirstSide();
            AccountingRecordDataGrid.Items.Refresh();
            ChangePagingBar();
        }


        /// <summary>
        /// Geht zur letzten Seite
        /// </summary>
        /// <param name="str"></param>
        private void lastSideProcessor(String str)
        {
            AccountingRecordDataGrid.ItemsSource = this.dataGridPaging.LastSide();
            AccountingRecordDataGrid.Items.Refresh();
            ChangePagingBar();
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
            refreshDataGrid(this.bookingModelsUnchanged);
        }

        #endregion

    }
}
