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
using System.Text.RegularExpressions;

using KöTaf.DataModel;
using KöTaf.Utils.ValidationTools;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik zum Anlegen eines neuen Sponsors
    /// Author: Antonios Fesenmeier
    /// Anpassung an Redisgn: Georg Schmid
    /// </summary>
    public partial class pNewSponsor : KPage
    {

        private ValidationTools _Validator;
        private bool _IsValid;

        public pNewSponsor()
        {
            InitializeComponent();

            Init();
        }

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



        private void pbBack_Click(Button button)
        {
            KPage pageSponsorAdministration = new KöTaf.WPFApplication.Views.pSponsorAdministration();
            SinglePage singlePage = new SinglePage("Sponsorverwaltung", pageSponsorAdministration);
        }

        /// <summary>
        /// Nach Validierung der einzelnen Felder wird der Sponsor in der DB abgelegt
        /// Author: Anotnios Fesenmeier
        /// </summary>
        /// <param name="button"></param>
        private void pbSave_Click(Button button)
        {
            //  bool formLetter = false;
            _Validator.clearSB();
            // Wurde die Validierung positiv abgeschlossen müssen die Werte der einzelnen Felder in die Datenbank geschrieben werden!
            CheckForm();

            if (!this._IsValid)
                MessageBoxEnhanced.Error(_Validator.getErrorMsg().ToString());
            else
            {
                var fundingType = cBFundingTyp.SelectedItem as FundingType;
                var title = cbTitle.SelectedItem as Title;
                var isFormLetterAllowed = chBformLetter.IsChecked;
                var isCompany = chBIsCompany.IsChecked;
                int _sponsorID;

                if (isCompany == true)
                {
                    _sponsorID = Sponsor.Add(fundingType.FundingTypeID, title.TitleID, txtFirstName.Text, txtLastName.Text, txtCity.Text, txtStreet.Text, Convert.ToInt32(txtZipCode.Text),
                         isFormLetterAllowed.Value, txtCompanyName.Text, null, txtEMail.Text, txtFax.Text, txtMobileNo.Text, txtTelNo.Text, isCompany.Value);
                }
                else
                {
                    _sponsorID= Sponsor.Add(fundingType.FundingTypeID, title.TitleID, txtFirstName.Text, txtLastName.Text, txtCity.Text, txtStreet.Text, Convert.ToInt32(txtZipCode.Text),
                        isFormLetterAllowed.Value, txtCompanyName.Text, null, txtEMail.Text, txtFax.Text, txtMobileNo.Text, txtTelNo.Text, isCompany.Value);
                }
                if (_sponsorID > 0)
                {
                    KPage pageSponsorAdministration = new KöTaf.WPFApplication.Views.pSponsorAdministration();
                    SinglePage singlePage = new SinglePage("Sponsorverwaltung", pageSponsorAdministration);
                }
                else
                    MessageBoxEnhanced.Error("Es ist ein Fehler beim speichern des Datensatzes aufgetreten");

            }
            _Validator.clearSB();
        }
        #endregion

        #region Methods

        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton("Zurück", pbBack_Click);
            this.parentToolbar.addButton("Speichern", pbSave_Click);
        }

        /// <summary>
        ///  Überprüfung der einzelnen Eingabefelder
        ///  return: boolean
        ///  Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
        private bool CheckForm()
        {
            this._IsValid = true;

            // Zunächst muss geprüft werden ob alle Pflichtfelder erfüllt sind!
            // Wenn nicht muss eine passende Fehlermeldung hinzugefügt werden.

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
                this._IsValid = false;
                _Validator.addError("Vorname", "Bitte geben Sie einen Vornamen an!");
            }

            if (_Validator.IsNullOrEmpty(txtLastName.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError("Nachname", "Bitte geben Sie einen Nachnamen an!");
            }

            if (_Validator.IsNullOrEmpty(txtStreet.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError("Straße", "Bitte geben Sie eine Straße an!");
            }
            else

                if (_Validator.IsNullOrEmpty(txtZipCode.Text) == false)
                {
                    this._IsValid = false;
                    _Validator.addError("PLZ", "Bitte geben Sie eine Postleitzahl an!");
                }

            if (_Validator.IsNullOrEmpty(txtCity.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError("Stadt", "Bitte geben Sie eine Stadt an!");
            }

            var fundingType = cBFundingTyp.SelectedItem as FundingType;
            if (fundingType == null)
            {
                this._IsValid = false;
                _Validator.addError("Typ", "Bitte geben Sie einen Typ an!");
            }

            #endregion

            // Nachdem alle Pflichtfelder ausgefüllt wurden, wird auf deren korrekten Inhalt geprüft.
            if (this._IsValid == true)
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

            if (this._IsValid == true)
            {
                if (_Validator.IsNullOrEmpty(txtTelNo.Text) &&
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

            return this._IsValid;
        }

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

            // Title ComboBox
            cbTitle.ItemsSource = DataModel.Title.GetTitles();

            // FundingType ComboBox
            cBFundingTyp.ItemsSource = FundingType.GetFundingTypes();

            gbCompanySponsor.Visibility = System.Windows.Visibility.Collapsed;
            gbCompanySponsor.IsEnabled = false;


        }



        #endregion






    }
}
