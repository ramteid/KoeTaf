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
using KöTaf.Utils.ValidationTools;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;
using KöTaf.Utils.UserSession;
using System.Globalization;
using KöTaf.WPFApplication.Models;

namespace KöTaf.WPFApplication.Views.Accounting.Bookings
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Interaktionslogik für pNewBooking.xaml
    /// </summary>
    public partial class pNewBooking : KPage
    {
        #region Fields
        
        private ValidationTools validator;
        private bool isValid;
        
        #endregion

        #region Constructors
        public pNewBooking()
        {
            InitializeComponent();
            this.validator = new ValidationTools();

            // Generiere Menüeinträge
            List<Account> accounts = Account.GetAccounts().ToList();
            cbSourceAccount.ItemsSource = accounts;
            cbTargetAccount.ItemsSource = accounts;

            cbTargetAccount.SelectionChanged -= cbTargetAccount_SelectionChanged;
            cbSourceAccount.SelectionChanged -= cbSourceAccount_SelectionChanged;
            cbSourceAccount.SelectedItem = null;
            cbTargetAccount.SelectedItem = null;
            cbTargetAccount.SelectionChanged += cbTargetAccount_SelectionChanged;
            cbSourceAccount.SelectionChanged += cbSourceAccount_SelectionChanged;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), Speichern_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "reset"), Reset_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), Cancel_Button);
        }

        /// <summary>
        /// Formular überprüfen
        /// </summary>
        /// <returns></returns>
        private bool checkForm()
        {
            this.isValid = true;

            #region checkMandatoryFields

            try
            {
                Account sollAcc = cbSourceAccount.SelectedItem as Account;
                Account habenAcc = cbTargetAccount.SelectedItem as Account;
                if (sollAcc.AccountID == habenAcc.AccountID)
                {
                    this.isValid = false;
                    this.validator.addError(IniParser.GetSetting("ACCOUNTING", "accounts"), IniParser.GetSetting("ERRORMSG", "identicalAccounts"));
                }

                // Um die Konsistenz der Kontensummen-Anzeige zu wahren, wird hier der Grundgedanke bewahrt, dass die Berechnung von Einnahmen und Ausgaben nur über Eigenkapital-Konten laufen kann
                if (sollAcc.IsCapital == false && habenAcc.IsCapital == false)
                {
                    this.isValid = false;
                    this.validator.addError(IniParser.GetSetting("ACCOUNTING", "accounts"), IniParser.GetSetting("ERRORMSG", "twoInofficialAccounts"));
                }
            }
            catch
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "sollAccount"), IniParser.GetSetting("ERRORMSG", "accountSelection"));
            }


            if (cbSourceAccount.SelectedItem == null)
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "sollAccount"), IniParser.GetSetting("ERRORMSG", "missingSollAccount"));
            }

            if (cbTargetAccount.SelectedItem == null)
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "habenAccount"), IniParser.GetSetting("ERRORMSG", "missingHabenAccount"));
            }

            double amount;
            if (!double.TryParse(tbAmount.Text.Replace(".", ","), out amount) || amount < 0)
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "amount"), IniParser.GetSetting("ERRORMSG", "invalidAmount"));
            }

            return this.isValid;

            #endregion
        }
        #endregion

        #region Events
        /// <summary>
        /// Buchung speichern
        /// </summary>
        /// <param name="button">sendender Button</param>
        private void Speichern_Click(Button button)
        {
            this.validator.clearSB();
            this.checkForm();

            if (!this.isValid)
            {
                MessageBox.Show(this.validator.getErrorMsg().ToString(), IniParser.GetSetting("ERRORMSG", "noTextField"), MessageBoxButton.OK);
                this.validator.clearSB();
            }
            else
            {
                try
                {
                    ComboBox cbSoll = cbSourceAccount as ComboBox;
                    Account accSoll = cbSoll.SelectedItem as Account;
                    int srcAccountID = accSoll.AccountID;

                    ComboBox cbHaben = cbTargetAccount as ComboBox;
                    Account accHaben = cbHaben.SelectedItem as Account;
                    int targetAccountID = accHaben.AccountID;

                    double amount;
                    bool parsed = Double.TryParse(tbAmount.Text.Replace(".", ","), out amount);

                    string description = tbDescription.Text;

                    int bookingId = Booking.Add(srcAccountID, targetAccountID, amount, null, UserSession.userAccountID, description);
                    
                    if (bookingId < 0)
                        throw new Exception();
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "newBooking"));
                    this.validator.clearSB();
                    return;
                }

                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pBookings);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "bookings"), pageType);
            }
            this.validator.clearSB();
        }

        /// <summary>
        /// Formular zurücksetzen
        /// </summary>
        /// <param name="button">sendender Button</param>
        private void Reset_Click(Button button)
        {
            cbSourceAccount.ItemsSource = Account.GetAccounts();
            cbTargetAccount.ItemsSource = Account.GetAccounts();
            cbTargetAccount.SelectionChanged -= cbTargetAccount_SelectionChanged;
            cbSourceAccount.SelectionChanged -= cbSourceAccount_SelectionChanged;
            cbSourceAccount.SelectedItem = null;
            cbTargetAccount.SelectedItem = null;
            cbTargetAccount.SelectionChanged += cbTargetAccount_SelectionChanged;
            cbSourceAccount.SelectionChanged += cbSourceAccount_SelectionChanged;
            tbAmount.Text = "";
            tbDescription.Text = "";
        }

        /// <summary>
        /// Abbrechen
        /// </summary>
        /// <param name="button"></param>
        private void Cancel_Button(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pBookings);
            mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "bookings"), pageType);
        }

        /// <summary>
        /// Anhand gewähltem Quellkonto das Zielkonto filtern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSourceAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Account targetAccount = cbTargetAccount.SelectedItem as Account;
            Account srcAccount = cbSourceAccount.SelectedItem as Account;

            if (srcAccount == null)
                return;
            else
            {
                List<Account> accounts = Account.GetAccounts().ToList();
                accounts = accounts.Where(account => account.AccountID != srcAccount.AccountID)        // nicht auf gleiches Konto
                                                    .ToList();

                // Hole ausgewähltes Konto der anderen ComboBox
                Account selectedAccount = null;
                if (targetAccount != null)
                {
                    selectedAccount = accounts.Where(a => a.Number == targetAccount.Number).ToList().FirstOrDefault();
                }

                // Entferne vorübergehend das SelectionChanged der anderen ComboBox, damit es bei der Neuzuweisung nicht ausgelöst wird
                cbTargetAccount.SelectionChanged -= cbTargetAccount_SelectionChanged;

                cbTargetAccount.ItemsSource = accounts;

                if (selectedAccount != null)
                    cbTargetAccount.SelectedItem = selectedAccount;

                // Füge das SelectionChanged-Event wieder hinzu
                cbTargetAccount.SelectionChanged += cbTargetAccount_SelectionChanged;
            }
        }

        /// <summary>
        /// Anhand gewähltem Zielkonto das Quellkonto filtern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbTargetAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Account srcAccount = cbSourceAccount.SelectedItem as Account;
            Account targetAccount = cbTargetAccount.SelectedItem as Account;

            if (targetAccount == null)
                return;
            else
            {
                List<Account> accounts = Account.GetAccounts().ToList();
                accounts = accounts.Where(account => account.AccountID != targetAccount.AccountID)        // nicht auf gleiches Konto
                                                    .ToList();

                // Hole ausgewähltes Konto der anderen ComboBox
                Account selectedAccount = null;
                if (srcAccount != null)
                {
                    selectedAccount = accounts.Where(a => a.Number == srcAccount.Number).ToList().FirstOrDefault();
                }

                // Entferne vorübergehend das SelectionChanged der anderen ComboBox, damit es bei der Neuzuweisung nicht ausgelöst wird
                cbSourceAccount.SelectionChanged -= cbSourceAccount_SelectionChanged;

                cbSourceAccount.ItemsSource = accounts;

                if (selectedAccount != null)
                    cbSourceAccount.SelectedItem = selectedAccount;

                // Füge das SelectionChanged-Event wieder hinzu
                cbSourceAccount.SelectionChanged += cbSourceAccount_SelectionChanged;
            }
        }

        #endregion
    }
}
