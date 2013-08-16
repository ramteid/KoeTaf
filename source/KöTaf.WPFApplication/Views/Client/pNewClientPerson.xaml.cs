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

namespace KöTaf.WPFApplication.Views.Client
{
    /// <summary>
    /// Interaktionslogik zum Anlegen einer neuen Person
    /// Author: Antonios Fesenmeier
    /// </summary>
    public partial class pNewClientPerson : KPage
    {

        private bool _PersonIsValid = true;
        private bool _IsInitialized = false;
        private ValidationTools _PersonValidator;
        private int _TableNo;

        private IList<DB.Child> _Childs;


        public pNewClientPerson()
        {
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
            this._Childs = new List<DB.Child>();

            // Jeder neue Kunde bekommt zunächst eine TafelNummer
            this._TableNo = DB.Person.GetNewIdentityNo();
            txtTableNo.Text = this._TableNo.ToString();

            // Title ComboBox
            cbTitle.ItemsSource = DB.Title.GetTitles();

            // FamilyState ComboBox 
            cbFamilyState.ItemsSource = DB.FamilyState.GetFamilyStates();

            // Gültig von/bis -> Personalien
            dpStartDate.SelectedDate = DateTime.Now;
            dpEndDate.SelectedDate = DateTime.Now.AddMonths(6); // 6 Monate gültig

            // Gruppen ComboBox befüllen
            cbGroup.Items.Add("1");
            cbGroup.Items.Add("2");


            this._IsInitialized = true;
        }

        /// <summary>
        /// Deaktiviert je nach Familienstatus oder Kinderzahl die Tabs und dessen Groupboxen
        /// Author: Antonios Fesenmeier
        /// </summary>
        public void disableTabs()
        {
            int childs = Convert.ToInt32(this.txtChildrens.Content);
            DB.FamilyState fS = cbFamilyState.SelectedItem as DB.FamilyState;
            if (childs == 0 && !fS.ShortName.Equals("VH") && !fS.ShortName.Equals("LP"))
            {
                TabControl tC = this.parentTabControl;
                var childPartnerTab = ((Control)(tC.Items.GetItemAt(1))).IsEnabled = false;

                pNewClientPartnerChild pPartnerChild = getPageFromTabs<pNewClientPartnerChild>();
                pPartnerChild.GrBChildren.IsEnabled = false;
                pPartnerChild.GrBPartner.IsEnabled = false;
            }




        }

        public override void defineToolbarContent()
        { }

        /// <summary>
        /// Löscht den Fehlerspeicher
        /// Author: Antonios Fesenmeier
        /// </summary>
        public void clearErrorMsg()
        {
            this._PersonValidator.clearSB();
        }

        /// <summary>
        /// Gibt eine Messagebox mit den Fehlern aus
        /// Author: Antonios Fesenmeier
        /// </summary>
        public void getErrorMsg()
        {
            if (this._PersonIsValid == false)
            {
                MessageBoxEnhanced.Error(_PersonValidator.getErrorMsg().ToString());
                this._PersonValidator.clearSB();
            }
        }

        /// <summary>
        /// Überprüft die Eingaben des Personentabs auf Korrektheit
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Je nach Kinderzahl oder Familienstand müssen die jeweiligen Tabs enabled werden.
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        public void EnableTabs()
        {
            if (this._PersonIsValid == true)
            {
                Boolean haveChild = false, havePartner = false;
                int childs = Convert.ToInt32(txtChildrens.Content);

                pNewClientPartnerChild pPartnerChild = getPageFromTabs<pNewClientPartnerChild>();

                // Wurde angegeben dass Kinder vorhanden sind, wird die 
                // entsprechende Gruppe von Controls enabled
                if (childs > 0)
                {
                    pPartnerChild.GrBChildren.IsEnabled = true;
                    pPartnerChild.addNewChildtoDatagrid();
                    haveChild = true;
                }
                else
                    pPartnerChild.GrBChildren.IsEnabled = false;

                DB.FamilyState fS = cbFamilyState.SelectedItem as DB.FamilyState;


                // Gleiches Verfahren wie bei den Kindern implementieren!!!
                if (fS.ShortName.Equals("VH") || fS.ShortName.Equals("LP"))
                {
                    pPartnerChild.GrBPartner.IsEnabled = true;
                    pPartnerChild.GrBChildren.IsEnabled = true;
                    pPartnerChild.addNewChildtoDatagrid();
                    haveChild = true;
                    havePartner = true;

                    ////KinderGroupbox aktivieren, damit Kinder hinzugefügt werden können
                    //pPartnerChild.GrBChildren.IsEnabled = true;
                    //pPartnerChild.addNewChildtoDatagrid();
                }
                else
                {
                    pPartnerChild.GrBPartner.IsEnabled = false;
                    havePartner = false;
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

        /// <summary>
        /// Wird der Familienstatus geändert wird geprüft welche Tabs enabeld bzw disabled werden müssen
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFamilyState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nach Änderung des FamilyState muss nochmals geprüft werden ob alle eingaben korrekt sind
            // und die entsprechenden Änderungen vorgenommen werden!
            if (this._IsInitialized)
                this.EnableTabs();
        }

        /// <summary>
        /// Wird das Geburtsdatum des Kunden editiert, wird auf Gültigkeit geprüft:
        /// Geburtsdatum darf nicht in der Zukunft liegen oder nicht länger als 120 Jahre zurückliegen
        /// 
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
                MessageBox.Show("Bitte geben Sie ein korrektes Geburtsdatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                dpBirthday.SelectedDate = System.DateTime.Now;
            }
        }

        private void pbNewChild_Click(object sender, RoutedEventArgs e)
        {
            //checkPersonTab();
            if (this._PersonIsValid == true)
            {
                pNewClientPartnerChild pPartnerChild = getPageFromTabs<pNewClientPartnerChild>();
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



        private void txtLastName_LostFocus(object sender, RoutedEventArgs e)
        {
            pNewClientPartnerChild pPartnerChild = getPageFromTabs<pNewClientPartnerChild>();
            pPartnerChild.setPersonLastName(txtLastName.Text);
        }

        #endregion
    }
}
