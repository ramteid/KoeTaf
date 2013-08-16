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
using KöTaf.WPFApplication;
using DB = KöTaf.DataModel;
using KöTaf.Utils.ValidationTools;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik zum Editieren eines Sponsors
    /// Author: Antonios Fesenmeier, Georg Schmid
    /// </summary>
    public partial class pEditSponsor : KPage
    {
        private ValidationTools _Validator;
        private readonly DB.Sponsor _CurrentSponsor;
        private bool _IsValid;
        private int pagingStartValue = 0;

        #region Constructor

        public pEditSponsor(DB.Sponsor currentSponsor, int? pagingStartValue = null)
        {
            this._CurrentSponsor = currentSponsor;
            if (this._CurrentSponsor == null)
            {
                throw new Exception("Sponsor cannot null");
            }

            InitializeComponent();

            if (pagingStartValue.HasValue)
                this.pagingStartValue = pagingStartValue.Value;

            Init();
        }

        #endregion

        #region Events

        /// <summary>
        /// Je nachdem ob der Sponsor ein Firmen oder PrivatSponsor ist muss der passende Expander geladen und angezeigt werden,
        /// Author: Antonios Fesenmeier 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chBIsCompany_Checked(object sender, RoutedEventArgs e)
        {
            if (chBIsCompany.IsChecked == true)
            {
                gbCompanySponsor.Visibility = System.Windows.Visibility.Visible;
                gbCompanySponsor.IsEnabled = true;
            }
        }

        private void chBIsCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            gbCompanySponsor.Visibility = System.Windows.Visibility.Collapsed;
            gbCompanySponsor.IsEnabled = false;
        }


        /// <summary>
        ///  Button zum Abbruch
        /// </summary>
        /// <param name="button"></param>
        private void pbBack_Click(Button button)
        {
            KPage pageSponsorAdministration = new KöTaf.WPFApplication.Views.pSponsorAdministration(pagingStartValue);
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "sponsorAdministration"), pageSponsorAdministration);
        }

        /// <summary>
        /// Nach dem Editieren wird auf Korrektheit geprüft und ggfs. in die DB gespeichert
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="button"></param>
        private void pbSave_Click(Button button)
        {
            _Validator.clearSB();
            // Wurde die Validierung positiv abgeschlossen müssen die Werte der einzelnen Felder in die Datenbank geschrieben werden!
            CheckForm();

            var isCompany = chBIsCompany.IsChecked;

            if (_IsValid == false)
                MessageBoxEnhanced.Error(_Validator.getErrorMsg().ToString());
            else
            {
                var title = cbTitle.SelectedItem as DB.Title;
                var fundingType = cBFundType.SelectedItem as DB.FundingType;

                if (isCompany == true)
                {

                    DB.Sponsor.Update(_CurrentSponsor.SponsorID, title.TitleID, fundingType.FundingTypeID, txtStreet.Text, txtCity.Text, Convert.ToInt32(txtZipCode.Text),
                     txtFirstName.Text, txtLastName.Text, txtCompanyName.Text, txtMobileNo.Text, txtTelNo.Text, txtFax.Text, txtEMail.Text, null, isCompany.Value);
                }
                else
                {
                    DB.Sponsor.Update(_CurrentSponsor.SponsorID, title.TitleID, fundingType.FundingTypeID, txtStreet.Text, txtCity.Text, Convert.ToInt32(txtZipCode.Text),
                     txtFirstName.Text, txtLastName.Text, "", txtMobileNo.Text, txtTelNo.Text, txtFax.Text, txtEMail.Text, null, isCompany.Value);
                }

                KPage pageSponsorAdministration = new KöTaf.WPFApplication.Views.pSponsorAdministration(pagingStartValue);
                SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "sponsorAdministration"), pageSponsorAdministration);
            }
            _Validator.clearSB();
        }

        #endregion

        #region Methods

        private void Init()
        {
            try
            {
                _Validator = new ValidationTools();
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }

            gbCompanySponsor.Visibility = System.Windows.Visibility.Collapsed;
            gbCompanySponsor.IsEnabled = false;

            this.DataContext = this._CurrentSponsor;

            // Title ComboBox
            var titles = DB.Title.GetTitles();
            cbTitle.ItemsSource = titles;
            cbTitle.SelectedItem = titles.SingleOrDefault(t => t.TitleID == _CurrentSponsor.Title.TitleID);

            // FundingType ComboBox
            var fundingTypes = DB.FundingType.GetFundingTypes();
            cBFundType.ItemsSource = fundingTypes;
            cBFundType.SelectedItem = fundingTypes.SingleOrDefault(ft => ft.FundingTypeID == _CurrentSponsor.FundingType.FundingTypeID);
        }

        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSave_Click);
        }

        /// <summary>
        ///  Prüfen der Felder auf Korrektheit, ggfs. Fehlermeldung
        ///  Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
        private bool CheckForm()
        {
            _IsValid = false;

            // Zunächst muss geprüft werden ob alle Pflichtfelder erfüllt sind!
            // Wenn nicht muss eine passende Fehlermeldung hinzugefügt werden.

            // Noch recht unschön für Anregungen wäre ich dankbar!
            #region checkMandatoryFields

            if (chBIsCompany.IsChecked == true)
            {
                if (_Validator.IsNullOrEmpty(txtCompanyName.Text) == false)
                {
                    this._IsValid = false;
                    _Validator.addError("Firmenname", "Bitte geben Sie den Firmennamen an!");
                }
            }

            if (_Validator.IsNullOrEmpty(txtFirstName.Text) == false)
            {
                _IsValid = false;
                _Validator.addError("Vorname", "Bitte geben Sie einen Vornamen an!");
            }


            if (_Validator.IsNullOrEmpty(txtLastName.Text) == false)
            {
                _IsValid = false;
                _Validator.addError("Nachname", "Bitte geben Sie einen Nachnamen an!");
            }

            if (_Validator.IsNullOrEmpty(txtStreet.Text) == false)
            {
                _IsValid = false;
                _Validator.addError("Straße", "Bitte geben Sie eine Straße an!");
            }
            else
                _IsValid = true;
            if (_Validator.IsNullOrEmpty(txtZipCode.Text) == false)
            {
                _IsValid = false;
                _Validator.addError("PLZ", "Bitte geben Sie eine Postleitzahl an!");

            }
            else
                _IsValid = true;
            if (_Validator.IsNullOrEmpty(txtCity.Text) == false)
            {
                _IsValid = false;
                _Validator.addError("Stadt", "Bitte geben Sie eine Stadt an!");
            }

            #endregion

            // Nachdem alle Pflichtfelder ausgefüllt wurden, wird auf deren korrekten Inhalt geprüft.
            if (_IsValid == true)
            {
                if (_Validator.IsName("Vorname", txtFirstName.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsName("Nachname", txtLastName.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsPLZ("PLZ", txtZipCode.Text) == false)
                    this._IsValid = false;
            }

            // Nachdem alle Pflichtfelder korrekt ausgefüllt wurden, ist es nötig weitere Felder auf Inhalte
            // und deren korrektheit zu testen!
            #region checkOptionalFields

            if (_IsValid == true)
            {
                if (_Validator.IsNullOrEmpty(txtTelNo.Text) == true &&
                _Validator.IsPhoneNumber("Telefonnummer ", txtTelNo.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsNullOrEmpty(txtFax.Text) == true &&
                _Validator.IsPhoneNumber("Fax-Nr.", txtFax.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsNullOrEmpty(txtMobileNo.Text) == true &&
                _Validator.IsMobileNumber("Mobilnummer ", txtMobileNo.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsNullOrEmpty(txtEMail.Text) == true &&
                _Validator.IsEMail("E-Mail ", txtEMail.Text) == false)
                    this._IsValid = false;
            }

            #endregion

            return _IsValid;
        }


        #endregion
    }
}

