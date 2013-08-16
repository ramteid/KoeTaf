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
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Helper;
using KöTaf.DataModel;
using KöTaf.Utils.ValidationTools;

namespace KöTaf.WPFApplication.Views.Accounting.AccountManager
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Interaktionslogik für pNewAccount.xaml
    /// </summary>
    public partial class pNewAccount : KPage
    {
        
        private ValidationTools validator;
        private bool isValid;

        public pNewAccount()
        {
            InitializeComponent();

            this.validator = new ValidationTools();

            lbIsOfficial.Content = IniParser.GetSetting("ACCOUNTING", "lbIsOfficial");
            lbIsCapital.Content = IniParser.GetSetting("ACCOUNTING", "lbIsCapital");
            lbWhenZeroBilance.Content = IniParser.GetSetting("ACCOUNTING", "lbWhenZeroBilance");
            lbFixedAccount.Content = IniParser.GetSetting("ACCOUNTING", "lbFixedAccount");

            // Initialisiere ComboBox mit Null-Zeiträumen
            List<AccountZeroPeriodEnum> zeroPeriods = new List<AccountZeroPeriodEnum>();
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.EveryCashClosure));
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.Annually));
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.Monthly));
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.Never));
            cbZeroPeriod.ItemsSource = zeroPeriods;
        }

        #region Methods
        /// <summary>
        /// Hinzufügen der Buttons "Speichern" und "Zurücksetzen"
        /// </summary>
        /// <see cref="Speichern_Click"/>
        /// <see cref="Reset_Click"/>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), Speichern_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "reset"), Reset_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), Cancel_Click);
        }

        /// <summary>
        /// Prüft das Formular auf valide Eingaben
        /// </summary>
        /// <returns>Falls alle Eingaben korrekt sind, wird TRUE zurück gegeben, anderenfalls FALSE</returns>
        private bool checkForm()
        {
            this.isValid = true;

            #region checkMandatoryFields

            if (string.IsNullOrEmpty(tbAccountName.Text) || !this.validator.IsName(IniParser.GetSetting("ACCOUNTING", "accountName"), tbAccountName.Text))
            {
                this.isValid = false;
            }

            int i = -1;
            if (!int.TryParse(tbAccountNumber.Text, out i) || i < 0)
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "accountNumber"), IniParser.GetSetting("ERRORMSG", "invalidAccountNumber"));
            }
            else
            {
                if (Account.GetAccounts().Where(a => a.Number == i).ToList().Count > 0)
                {
                    this.isValid = false;
                    this.validator.addError(IniParser.GetSetting("ACCOUNTING", "accountNumber"), IniParser.GetSetting("ERRORMSG", "doubleAccountNumber"));
                }
            }

            if (string.IsNullOrEmpty(tbAccountDescription.Text))
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "accountDescription"), IniParser.GetSetting("ERRORMSG", "invalidDescription"));
            }

            if (cbZeroPeriod.SelectedItem == null)
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "zeroTime"), IniParser.GetSetting("ERRORMSG", "invalidZeroTime"));
            }

            #endregion

            return this.isValid;
        }
        #endregion

        #region Events
        /// <summary>
        /// Legt ein neues Konto an
        /// </summary>
        /// <param name="button"></param>
        private void Speichern_Click(Button button)
        {
            this.validator.clearSB();
            this.checkForm();

            if (!this.isValid)
            {
                MessageBox.Show(this.validator.getErrorMsg().ToString(), IniParser.GetSetting("ERRORMSG", "noTextField"), MessageBoxButton.OK, MessageBoxImage.Hand);
                this.validator.clearSB();
            }
            else
            {
                int accountNumber;
                if (!Int32.TryParse(tbAccountNumber.Text, out accountNumber))
                    return;
                var accountName = tbAccountName.Text;
                var accountDescription = tbAccountDescription.Text;
                DataModel.Enums.ZeroPeriod period = ((AccountZeroPeriodEnum)cbZeroPeriod.SelectedItem).period;
                bool isOfficial = (bool)chkbIsOfficial.IsChecked;
                bool isCapital = (bool)chkbIsCapital.IsChecked;
                bool isFix = (bool)chkbIsFixAccount.IsChecked;

                var accountId = Account.Add(accountName, accountNumber, isOfficial, period, isFix, 0.0, accountDescription, isCapital);
                
                if (accountId > 0)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    Type pageType = typeof(pAccountManager);
                    mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "accountManagement"), pageType);
                }
                else
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "newAccount"));
                    this.validator.clearSB();
                }
            }
            this.validator.clearSB();
        }

        /// <summary>
        /// Leert alle Formularfelder
        /// </summary>
        /// <param name="button"></param>
        private void Reset_Click(Button button)
        {
            tbAccountName.Text = null;
            tbAccountNumber.Text = null;
            tbAccountDescription.Text = null;
            cbZeroPeriod.SelectedItem = null;
            chkbIsOfficial.IsChecked = false;
            chkbIsCapital.IsChecked = false;
        }

        /// <summary>
        /// Abbrechen
        /// </summary>
        /// <param name="button"></param>
        private void Cancel_Click(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pAccountManager);
            mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "accountManagement"), pageType);
        }
        #endregion

        /// <summary>
        /// Warnung bevor Fixkonto ausgewählt wird
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkbIsFixAccount_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxEnhanced.Warning(IniParser.GetSetting("ACCOUNTING", "warningCheckFixed"));
            if (result == MessageBoxResult.No)
                chkbIsFixAccount.IsChecked = false;
        }
    }
}