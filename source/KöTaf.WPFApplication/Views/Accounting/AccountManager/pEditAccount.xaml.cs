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
using KöTaf.Utils.ValidationTools;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Accounting.AccountManager
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Interaktionslogik für pEditAccount.xaml
    /// </summary>
    public partial class pEditAccount : KPage
    {
        
        private readonly Account currentAccount;
        private ValidationTools validator;
        private bool isValid;

        public pEditAccount(Account currentAccount)
        {
            InitializeComponent();

            if (currentAccount == null)
            {
                // Abbrechen
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pAccountManager);
                mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "accountManagement"), pageType);
            }

            lbIsOfficial.Content = IniParser.GetSetting("ACCOUNTING", "lbIsOfficial");
            lbIsCapital.Content = IniParser.GetSetting("ACCOUNTING", "lbIsCapital");
            lbWhenZeroBilance.Content = IniParser.GetSetting("ACCOUNTING", "lbWhenZeroBilance");
            lbFixedAccount.Content = IniParser.GetSetting("ACCOUNTING", "lbFixedAccount");

            this.currentAccount = currentAccount;
            Init();
        }

        #region Methods
        /// <summary>
        /// Fügt die Buttons "Speichern", "Zurücksetzen" und "Abbrechen" hinzu
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
        /// Initialisiert alle Variablen
        /// </summary>
        private void Init()
        {
            this.validator = new ValidationTools();
            this.DataContext = this.currentAccount;
            tbAccountName.Text = this.currentAccount.Name;
            tbAccountDescription.Text = this.currentAccount.Description;
            chkbIsOfficial.IsChecked = this.currentAccount.IsOfficial;
            chkbIsCapital.IsChecked = this.currentAccount.IsCapital;
            chkbIsFixAccount.IsChecked = this.currentAccount.IsFixed;

            // Initialisiere ComboBox mit Null-Zeiträumen
            List<AccountZeroPeriodEnum> zeroPeriods = new List<AccountZeroPeriodEnum>();
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.EveryCashClosure));
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.Annually));
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.Monthly));
            zeroPeriods.Add(new AccountZeroPeriodEnum(DataModel.Enums.ZeroPeriod.Never));
            cbZeroPeriod.ItemsSource = zeroPeriods;

            // wähle das richtige aus
            if (this.currentAccount.ZeroPeriod == (int)DataModel.Enums.ZeroPeriod.EveryCashClosure)
                cbZeroPeriod.SelectedIndex = 0;
            if (this.currentAccount.ZeroPeriod == (int)DataModel.Enums.ZeroPeriod.Annually)
                cbZeroPeriod.SelectedIndex = 1;
            if (this.currentAccount.ZeroPeriod == (int)DataModel.Enums.ZeroPeriod.Monthly)
                cbZeroPeriod.SelectedIndex = 2;
            if (this.currentAccount.ZeroPeriod == (int)DataModel.Enums.ZeroPeriod.Never)
                cbZeroPeriod.SelectedIndex = 3;
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

            int i;
            if (!int.TryParse(tbAccountNumber.Text, out i) || i < 0)
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("ACCOUNTING", "accountNumber"), IniParser.GetSetting("ERRORMSG", "invalidAccountNumber"));
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
        /// Speichert die Überarbeitung eines Kontos
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
                try
                {
                    string accountName = tbAccountName.Text;
                    int accountNumber;
                    bool success = Int32.TryParse(tbAccountNumber.Text, out accountNumber);
                    string accountDescription = tbAccountDescription.Text;
                    DataModel.Enums.ZeroPeriod period = ((AccountZeroPeriodEnum)cbZeroPeriod.SelectedItem).period;
                    bool isOfficial = (bool)chkbIsOfficial.IsChecked;
                    bool isCapital = (bool)chkbIsCapital.IsChecked;
                    double latestBalance = this.currentAccount.LatestBalance;
                    bool isFixed = (bool)chkbIsFixAccount.IsChecked;

                    Account.Update(this.currentAccount.AccountID, accountName, accountNumber, period, isOfficial, accountDescription, latestBalance, isCapital, isFixed);

                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    Type pageType = typeof(pAccountManager);
                    mainWindow.switchPage(IniParser.GetSetting("ACCOUNTING", "accountManagement"), pageType);
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "editAccount"));
                }
            }

            this.validator.clearSB();
        }

        /// <summary>
        /// Setzt alle Formularfelder zurück
        /// </summary>
        /// <param name="button"></param>
        private void Reset_Click(Button button)
        {
            tbAccountName.Text = null;
            tbAccountNumber.Text = null;
            tbAccountDescription.Text = null;
            chkbIsOfficial.IsChecked = false;
            chkbIsCapital.IsChecked = false;
            cbZeroPeriod.SelectedItem = null;
            chkbIsFixAccount.IsChecked = false;
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