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
using KöTaf.DataModel;
using KöTaf.Utils.ValidationTools;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Parser;


namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik für pEditTeamMember.xaml
    /// Grundlogik von Antonios Fesenmeier
    /// Anpassungen und Erweiterungen zum Editieren durch Georg Schmid
    /// </summary>
    public partial class pEditTeamMember : KPage
    {
        private readonly Team _CurrentTeamMember;

        private ValidationTools _Validator;
        private bool _IsValid;
        private int pagingStartValue = 0;

        #region Constructor

        public pEditTeamMember(Team currentTeamMember, int? pagingStartValue = null)
        {
            _CurrentTeamMember = currentTeamMember;
            if (_CurrentTeamMember == null)
            {
                throw new Exception("Team member cannot null");
            }

            InitializeComponent();

            if (pagingStartValue.HasValue)
                this.pagingStartValue = pagingStartValue.Value;

            Init();
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
            

            this.DataContext = _CurrentTeamMember;

            FillComboBoxes();

        }

        /// <summary>
        /// Definiert die Toolbar
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSave_Click);
        }

        /// <summary>
        /// Überprüft die Eingaben im Formular
        /// </summary>
        /// <returns>Boolean, ob die Eingaben valide sind</returns>
        private bool CheckForm()
        {
            this._IsValid = true;

            // Zunächst muss geprüft werden ob alle Pflichtfelder erfüllt sind!
            // Wenn nicht muss eine passende Fehlermeldung hinzugefügt werden.

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
                _Validator.addError(IniParser.GetSetting("TEAM", "streat"), IniParser.GetSetting("TEAM", "streetError"));
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
        /// Befüllt die Comboboxen
        /// </summary>
        private void FillComboBoxes()
        {
            var teamFunctions = TeamFunction.GetTeamFunctions().ToList();
            cBFunction.ItemsSource = teamFunctions;
            if (_CurrentTeamMember.TeamFunction != null)
            {
                cBFunction.SelectedIndex = teamFunctions.FindIndex(t => t.TeamFunctionID == _CurrentTeamMember.TeamFunction.TeamFunctionID);
            }

            var titles = DataModel.Title.GetTitles().ToList();
            cbTitle.ItemsSource = titles;
            if (_CurrentTeamMember.Title != null)
            {
                cbTitle.SelectedIndex = titles.FindIndex(t => t.TitleID == _CurrentTeamMember.Title.TitleID);
            }
        }

        #endregion

        #region Events
        
        /// <summary>
        /// Überprüft das Datum, wenn das Feld den Focus verliert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpBirthday_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime date = dpBirthday.SelectedDate.GetValueOrDefault(System.DateTime.Today);
                TimeSpan ts = System.DateTime.Now.Subtract(date);

                if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
                {
                    MessageBox.Show(IniParser.GetSetting("TEAM", "birthdateError"), IniParser.GetSetting("TEAM", "dateError"), MessageBoxButton.OK, MessageBoxImage.Error);
                    dpBirthday.SelectedDate = System.DateTime.Now;
                }
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
            }
        }

        /// <summary>
        /// Zurück-Button
        /// </summary>
        /// <param name="button"></param>
        private void pbBack_Click(Button button)
        {
            KPage pageTeamAdministration = new KöTaf.WPFApplication.Views.pTeamAdministration(pagingStartValue);

            SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "teamAdministration"), pageTeamAdministration);
        }

        /// <summary>
        /// Speichern-Button
        /// </summary>
        /// <param name="button"></param>
        private void pbSave_Click(Button button)
        {

            _Validator.clearSB();
            CheckForm();

            if (_IsValid == false)
            {
                MessageBox.Show(_Validator.getErrorMsg().ToString(), IniParser.GetSetting("ERRORMSG", "noTextField"), MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                try
                {
                    var teamId = this._CurrentTeamMember.TeamID;
                    var teamTitle = cbTitle.SelectedItem as DataModel.Title;
                    var teamFunction = cBFunction.SelectedItem as TeamFunction;
                    var dateOfBirth = (DateTime)dpBirthday.SelectedDate;
                    var firstName = txtFirstName.Text;
                    var lastName = txtLastName.Text;
                    var street = txtStreet.Text;
                    var zipCode = int.Parse(txtZipCode.Text);
                    var city = txtCity.Text;
                    var mobileNo = txtMobileNo1.Text;
                    var phoneNo = txtTelNo1.Text;
                    var email = txtEMail1.Text;
                    var isFormletterAllowed = (bool)chBIsFormletterAllowed.IsChecked;

                    Team.Update(teamId, dateOfBirth, teamTitle.TitleID, teamFunction.TeamFunctionID, firstName, lastName, street,
                        zipCode, city, mobileNo, phoneNo, email, isFormletterAllowed);

                    if (cbIsActive.IsChecked == true)
                        Team.Activate(teamId);
                    else
                        Team.Deactivate(teamId);

                    KPage pageTeamAdministration = new KöTaf.WPFApplication.Views.pTeamAdministration(pagingStartValue);

                    SinglePage singlePage = new SinglePage(IniParser.GetSetting("APPSETTINGS", "teamAdministration"), pageTeamAdministration);
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
                }
            }
            _Validator.clearSB();
        }

        #endregion

    

    }
}

