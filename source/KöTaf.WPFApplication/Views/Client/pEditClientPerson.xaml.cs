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
using KöTaf.Utils.ValidationTools;
using DB = KöTaf.DataModel;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Client
{
    /// <summary>
    /// Interaktionslogik zum Editieren eines Personendatensatzes
    /// Grundlogik realisiert durch Antonios Fesenmeier
    /// Angepasst durch Georg Schmid
    /// </summary>
    public partial class pEditClientPerson : KPage
    {
        private bool _PersonIsValid = true;
        private bool _IsInitialized = false;
        private ValidationTools _PersonValidator;

        private IList<ChildModel> _Childs;
        private readonly Person _currentPerson;


        public pEditClientPerson(Person currentPerson)
        {
            _currentPerson = currentPerson;
            if (_currentPerson == null)
            {
                throw new Exception("Client cannot null");
            }
            InitializeComponent();

            Init();
        }

        #region Methods

        private void Init()
        {
            try
            {
                this._PersonValidator = new ValidationTools();
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
            this._Childs = new List<ChildModel>();

            //Datenkontext für die Seite festlegen
            this.DataContext = _currentPerson;

            // Title ComboBox
            var titles = DataModel.Title.GetTitles().ToList();
            cbTitle.ItemsSource = titles;
            if (_currentPerson.Title != null)
            {
                cbTitle.SelectedIndex = titles.FindIndex(t => t.TitleID == _currentPerson.Title.TitleID);
            }

            // FamilyState ComboBox 
            var familyStates = DB.FamilyState.GetFamilyStates().ToList();
            cbFamilyState.ItemsSource = familyStates;
            if (_currentPerson.FamilyState != null)
            {
                cbFamilyState.SelectedIndex = familyStates.FindIndex(f => f.FamilyStateID == _currentPerson.FamilyState.FamilyStateID);
            }


            // Gültig von/bis -> Personalien
            dpStartDate.SelectedDate = _currentPerson.ValidityStart;
            dpEndDate.SelectedDate = _currentPerson.ValidityEnd;

            // Gruppen ComboBox befüllen
            cbGroup.Items.Add("1");
            cbGroup.Items.Add("2");
            //Richtige Gruppe auswählen
            if (_currentPerson.Group == 1)
                cbGroup.SelectedIndex = 0;
            else
                cbGroup.SelectedIndex = 1;

            setNumberOfChild(_currentPerson.NumberOfChildren.ToString());

            

          

            this._IsInitialized = true;
        } 

        public void disableTabs()
        {
            int childs = Convert.ToInt32(this.txtChildrens.Content);
            DB.FamilyState fS = cbFamilyState.SelectedItem as DB.FamilyState;

            if (!(childs > 0) && (!(fS.ShortName.Equals("VH") || fS.ShortName.Equals("LP"))))
            {
                TabControl tC = this.parentTabControl;
                var childPartnerTab = ((Control)(tC.Items.GetItemAt(1))).IsEnabled = false;
            }
            else
            {
                this.EnableTabs();
            }

        }

        public override void defineToolbarContent()
        {
            
        }

        public void clearErrorMsg()
        {
            this._PersonValidator.clearSB();
        }

        public void getErrorMsg()
        {
            if (this._PersonIsValid == false)
            {
                MessageBoxEnhanced.Error(_PersonValidator.getErrorMsg().ToString());
                this._PersonValidator.clearSB();
            }
        }

        public int getPersonID()
        {
            return _currentPerson.PersonID;
        }

        public bool checkPersonTab()
        {
            this._PersonIsValid = true;
            clearErrorMsg();
            // Zunächst muss geprüft werden ob alle Pflichtfelder erfüllt sind!
            // Wenn nicht muss eine passende Fehlermeldung hinzugefügt werden.
            #region MandatoryFields
            if (this._PersonValidator.IsNullOrEmpty(txtFirstName.Text) == false)
            {
                this._PersonIsValid = false;
                this._PersonValidator.addError("Vorname", "Bitte geben Sie den Vornamen des Kunden an!");
            }

            if (this._PersonValidator.IsNullOrEmpty(txtLastName.Text) == false)
            {
                this._PersonIsValid = false;
                this._PersonValidator.addError("Nachname", "Bitte geben Sie den Nachnamen des Kunden an!");
            }

            if (this._PersonValidator.IsNullOrEmpty(txtStreet.Text) == false)
            {
                this._PersonIsValid = false;
                this._PersonValidator.addError("Straße", "Bitte geben Sie die Straße für den Wohnort des Kunden an!");
            }

            if (this._PersonValidator.IsNullOrEmpty(txtZipCode.Text) == false)
            {
                this._PersonIsValid = false;
                this._PersonValidator.addError("PLZ", "Bitte geben Sie eine Postleitzahl für den Wohnort des Kunden an!");
            }

            if (this._PersonValidator.IsNullOrEmpty(txtCity.Text) == false)
            {
                this._PersonIsValid = false;
                this._PersonValidator.addError("Stadt", "Bitte geben Sie eine Stadt für den Wohnort des Kunden an!");
            }

            if (dpBirthday.SelectedDate.HasValue == false)
            {
                this._PersonIsValid = false;
                this._PersonValidator.addError("Geburtstag des Kunden", "Bitte geben Sie den Geburtstag an!");
            }


            // Nachdem alle Pflichtfelder ausgefüllt wurden, wird auf deren korrekten Inhalt geprüft.
            if (this._PersonIsValid == true)
            {
                if (this._PersonValidator.IsName("Vorname", txtFirstName.Text, "Bitte geben Sie den Vornamen des Kunden an!") == false)
                    this._PersonIsValid = false;

                if (this._PersonValidator.IsName("Nachname", txtLastName.Text, "Bitte geben Sie den Nachnamen des Kunden an!") == false)
                    this._PersonIsValid = false;

                if (this._PersonValidator.IsName("Ort", txtCity.Text, "Bitte geben Sie den Wohnort des Kunden an!") == false)
                    this._PersonIsValid = false;

                if (this._PersonValidator.IsPLZ("PLZ", txtZipCode.Text, "Bitte geben Sie die PLZ des Kunden an!") == false)
                    this._PersonIsValid = false;
            }
            #endregion

            // Nachdem nun alle Pflichtfelder befüllt wurden und geprüft wurden,
            // werden die freiwilligen Felder geprüft, sind auch diese Korrekt
            // wird der nächste Tab freigegeben!
            #region CheckOptionalFields

            if (this._PersonValidator.IsNullOrEmpty(txtTelno.Text) == true &&
                this._PersonValidator.IsPhoneNumber("Telefonnummer ", txtTelno.Text, "Die Telefonnummer des Kunden ist falsch!") == false)
                this._PersonIsValid = false;

            if (this._PersonValidator.IsNullOrEmpty(txtMobileNo.Text) == true &&
              this._PersonValidator.IsMobileNumber("Mobilnummer ", txtMobileNo.Text, "Die Mobilnummer des Kunden ist falsch!") == false)
                this._PersonIsValid = false;

            if (this._PersonValidator.IsNullOrEmpty(txtEmail.Text) == true &&
                this._PersonValidator.IsEMail("E-Mail ", txtEmail.Text, "Die E-Mail des Kunden ist falsch!") == false)
                this._PersonIsValid = false;

            if (this._PersonValidator.IsNullOrEmpty(txtNationalCountry.Text) == true &&
                 this._PersonValidator.IsName("Geburtsland", txtNationalCountry.Text, "Das Geburtsland des Kunden ist falsch!") == false)
                this._PersonIsValid = false;

            if (this._PersonValidator.IsNullOrEmpty(txtNativeCountry.Text) == true &&

                 this._PersonValidator.IsName("Staatsangehörigkeit", txtNativeCountry.Text, "Die Staatsangehörigkeit des Kunden ist falsch!") == false)
                this._PersonIsValid = false;

            #endregion

            return this._PersonIsValid;
        }



        public void EnableTabs()
        {
            if (this._PersonIsValid == true)
            {
                Boolean haveChild = false, havePartner = false;
                int childs = Convert.ToInt32(txtChildrens.Content);

                pEditClientPartnerChild pPartnerChild = getPageFromTabs<pEditClientPartnerChild>();

                // Wurde angegeben dass Kinder vorhanden sind, wird die 
                // entsprechende Gruppe von Controls enabled
                if (childs > 0)
                {
                    pPartnerChild.GrBChildren.IsEnabled = true;
                    haveChild = true;
                }
                else
                    pPartnerChild.GrBChildren.IsEnabled = false;

                DB.FamilyState fS = cbFamilyState.SelectedItem as DB.FamilyState;
                
                if (fS.ShortName.Equals("VH") || fS.ShortName.Equals("LP"))
                {
                    pPartnerChild.GrBPartner.IsEnabled = true;
                    havePartner = true;

                    //KinderGroupbox aktivieren, damit Kinder hinzugefügt werden können
                    pPartnerChild.GrBChildren.IsEnabled = true;
                    
                    
                    //Daten des Partners eintragen
                    var titles = DataModel.Title.GetTitles().ToList();
                    pPartnerChild.cbTitle1.ItemsSource = titles;
                    if (_currentPerson.MaritalTitle != null)
                    {
                        pPartnerChild.cbTitle1.SelectedIndex = titles.FindIndex(t => t.TitleID == _currentPerson.MaritalTitle.TitleID);
                    }
                    if (_currentPerson.MaritalBirthday != null)
                    {
                        pPartnerChild.dpBirthday1.SelectedDate = _currentPerson.MaritalBirthday;
                    }
                    
                }
                else
                {
                    pPartnerChild.GrBPartner.IsEnabled = false;
                    havePartner = false;

                    //Daten für Partner löschen

                    pPartnerChild.cbTitle1.SelectedIndex = 0;
                    pPartnerChild.txtFirstName1.Text = "";
                    pPartnerChild.txtLastName1.Text = "";
                    pPartnerChild.txtNationalCountry1.Text = "";

                    //Datum auf leer setzten
                    //pPartnerChild.dpBirthday1.SelectedDate = DateTime.Now;

                    pPartnerChild.txtNativeCountry1.Text = "";
                    pPartnerChild.txtTelno1.Text = "";
                    pPartnerChild.txtMobileNo1.Text = "";
                    pPartnerChild.txtEmail1.Text = "";
                }

                TabControl tC = this.parentTabControl;
                var tab = tC.Items.GetItemAt(1);
                Control childPartnerTab = ((Control)tab);
                   

                tab = tC.Items.GetItemAt(2);
                Control revenuesTab = ((Control)tab);

                if (haveChild == true || havePartner == true)
                {
                    childPartnerTab.IsEnabled = true;
                    return;
                }
                else
                {
                    childPartnerTab.IsEnabled = false;
                    revenuesTab.IsEnabled = true;
                }
            }
        }

        public void setNumberOfChild(String number)
        {
            txtChildrens.Content = number;
            if (number != "0")
                pbNewChild.Visibility = System.Windows.Visibility.Hidden;
            else
                pbNewChild.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        #region Events
        private void KPage_Loaded(object sender, RoutedEventArgs e)
        {
            disableTabs();
        }


        private void cbFamilyState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nach Änderung des FamilyState muss nochmals geprüft werden ob alle eingaben korrekt sind
            // und die entsprechenden Änderungen vorgenommen werden!
            if (this._IsInitialized)
                this.EnableTabs();
        }

        private void dpBirthday_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DateTime date = dpBirthday.SelectedDate.GetValueOrDefault(System.DateTime.Today);
                TimeSpan ts = System.DateTime.Now.Subtract(date);

                if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
                {
                    MessageBox.Show("Bitte geben Sie ein korrektes Geburtsdatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                    dpBirthday.SelectedDate = System.DateTime.Now;
                }
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
            }
        }

        private void pbNewChild_Click(object sender, RoutedEventArgs e)
        {
            //checkPersonTab();
            if (this._PersonIsValid == true)
            {
                pEditClientPartnerChild pPartnerChild = getPageFromTabs<pEditClientPartnerChild>();
                TabControl tC = this.parentTabControl;
                var tab = tC.Items.GetItemAt(1);
                Control childPartnerTab = ((Control)tab);
                childPartnerTab.IsEnabled = true;
                
                pPartnerChild.addNewChildtoDatagrid();
                ((TabItem)tC.Items.GetItemAt(1)).Focus();
            }
            else
            {
                getErrorMsg();
            }
        }



        private void cbFamilyState_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (this._IsInitialized)
                this.EnableTabs();
        }

        #endregion

    }
}
