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
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Author: Antonios Fesenmeier, Georg Schmid
    /// Interaktionslogik für pNewTeamMember.xaml
    /// </summary>
    public partial class pNewTeamMember : KPage
    {
        private ValidationTools _Validator;
        private bool _IsValid;

        #region Constructor

        public pNewTeamMember()
        {
            InitializeComponent();

            Init();
        }

        #endregion

        #region Events

        /// <summary>
        /// Prüft das Geburtsdatum welches nciht weiter als 120 Jahre zurück oder in der Zukunft liegen darf.
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpBirthday_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = dpBirthday.SelectedDate.GetValueOrDefault(System.DateTime.Today);
            TimeSpan ts = System.DateTime.Now.Subtract(date);

            if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
            {
                MessageBox.Show(IniParser.GetSetting("TEAM", "birthdateError"), IniParser.GetSetting("TEAM", "dateError"), MessageBoxButton.OK, MessageBoxImage.Error);
                dpBirthday.SelectedDate = System.DateTime.Now;
            }
        }

        /// Dient als Abbruch, weiterleitung an Übersicht
        /// Author: Antonios Fesenmeier
        private void pbBack_Click(Button button)
        {
            KPage pageTeamAdministration = new KöTaf.WPFApplication.Views.pTeamAdministration();

            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "teamAdministration"), pageTeamAdministration);
        }

        /// <summary>
        /// Überprüft alle Felder auf Korrektheit, anschließend entweder Fehlerausgabe oder speichern der Daten
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="button"></param>
        private void pbSave_Click(Button button)
        {
            _Validator.clearSB();
            // Wurde die Validierung positiv abgeschlossen müssen die Werte der einzelnen Felder in die Datenbank geschrieben werden!
            CheckForm();
            if (_IsValid == false)
            {
                MessageBox.Show(_Validator.getErrorMsg().ToString(), IniParser.GetSetting("ERRORMSG", "noTextField"), MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                try
                {
                    var title = cbTitle.SelectedItem as DataModel.Title;
                    var teamFunction = cBFunction.SelectedItem as TeamFunction;
                    var firstName = txtFirstName.Text;
                    var lastName = txtLastName.Text;
                    var street = txtStreet.Text;
                    var zipCode = int.Parse(txtZipCode.Text);
                    var city = txtCity.Text;
                    var dateOfBirth = (DateTime)dpBirthday.SelectedDate;
                    var mobileNo = txtMobileNo1.Text;
                    var phoneNo = txtTelNo1.Text;
                    var email = txtEMail1.Text;
                    var isFormLetterAllowed = (bool)chBIsFormletterAllowed.IsChecked;

                    var teamId = Team.Add(title.TitleID, teamFunction.TeamFunctionID, firstName, lastName, street, zipCode, city,
                        dateOfBirth, mobileNo, phoneNo, email, isFormLetterAllowed);

                    if (teamId > 0)
                    {
                        KPage pageTeamAdministration = new KöTaf.WPFApplication.Views.pTeamAdministration();

                        SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "teamAdministration"), pageTeamAdministration);
                    }
                    else
                        MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "saveDataRecord"));
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
                }
            }
            _Validator.clearSB();
        }

        #endregion

        #region Methods

        private void Init()
        {
            try
            {
                this._Validator = new ValidationTools();
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }

            FillComboBoxes();
        }

        /// <summary>
        /// Legt den Inhalt der Toolbar fest
        /// Author: Georg Schmid
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSave_Click);
        }


        /// <summary>
        /// Überprüfung aller Eingaben
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
        private bool CheckForm()
        {
            this._IsValid = true;

            // Zunächst muss geprüft werden ob alle Pflichtfelder erfüllt sind!
            // Wenn nicht muss eine passende Fehlermeldung hinzugefügt werden.

            // Noch recht unschön für Anregungen wäre ich dankbar!
            #region checkMandatoryFields

            if (_Validator.IsNullOrEmpty(txtFirstName.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError(IniParser.GetSetting("TEAM", "surname"), IniParser.GetSetting("TEAM", "surnameError"));
            }

            if (_Validator.IsNullOrEmpty(txtLastName.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError(IniParser.GetSetting("TEAM", "name"), IniParser.GetSetting("TEAM", "nameError"));
            }

            if (_Validator.IsNullOrEmpty(txtStreet.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError(IniParser.GetSetting("TEAM", "street"), IniParser.GetSetting("TEAM", "streetError"));
            }

            if (_Validator.IsNullOrEmpty(txtZipCode.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError(IniParser.GetSetting("TEAM", "zip"), IniParser.GetSetting("TEAM", "zipError"));
            }

            if (_Validator.IsNullOrEmpty(txtCity.Text) == false)
            {
                this._IsValid = false;
                _Validator.addError(IniParser.GetSetting("TEAM", "city"), IniParser.GetSetting("TEAM", "cityError"));
            }

            #endregion

            // Nachdem alle Pflichtfelder ausgefüllt wurden, wird auf deren korrekten Inhalt geprüft.
            if (this._IsValid == true)
            {
                if (_Validator.IsName(IniParser.GetSetting("TEAM", "surname"), txtFirstName.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsName(IniParser.GetSetting("TEAM", "name"), txtLastName.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsPLZ(IniParser.GetSetting("TEAM", "zip"), txtZipCode.Text) == false)
                    this._IsValid = false;
            }

            // Nachdem alle Pflichtfelder korrekt ausgefüllt wurden, ist es nötig weitere Felder auf Inhalte
            // und deren korrektheit zu testen!
            #region checkOptionalFields

            if (this._IsValid == true)
            {
                if (_Validator.IsNullOrEmpty(txtTelNo1.Text) == true &&
                _Validator.IsPhoneNumber(IniParser.GetSetting("TEAM", "phone"), txtTelNo1.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsNullOrEmpty(txtMobileNo1.Text) == true &&
                _Validator.IsMobileNumber(IniParser.GetSetting("TEAM", "mobile"), txtMobileNo1.Text) == false)
                    this._IsValid = false;

                if (_Validator.IsNullOrEmpty(txtEMail1.Text) == true &&
                     _Validator.IsEMail(IniParser.GetSetting("TEAM", "email"), txtEMail1.Text) == false)
                    this._IsValid = false;
            }

            #endregion

            return this._IsValid;
        }

        /// <summary>
        /// Befüllt die Comboboxen mit den korrekten Werten
        /// </summary>
        private void FillComboBoxes()
        {
            cBFunction.ItemsSource = TeamFunction.GetTeamFunctions();
            cBFunction.SelectedIndex = 0;

            cbTitle.ItemsSource = DataModel.Title.GetTitles();
            cbTitle.SelectedIndex = 0;
        }

        #endregion


    }
}
