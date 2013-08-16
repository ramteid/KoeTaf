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
using KöTaf.WPFApplication.Helper;
using DB = KöTaf.DataModel;
using KöTaf.WPFApplication.Models;

namespace KöTaf.WPFApplication.Views.Client
{
    /// <summary>
    /// Interaktionslogik zum Anlegen eines Partners oder Kindern
    /// 
    /// Author: Antonios Fesenmeier
    /// </summary>
    public partial class pNewClientPartnerChild : KPage
    {
        private bool _PartnerChildIsValid = true;
        private ValidationTools _PartnerChildValidator;
        private IList<ChildModel> _Childs;
        private IList<ChildModel> _ValidChilds;
        private String personLastName;

        public pNewClientPartnerChild()
        {
            InitializeComponent();

            Init();
        }

        #region Methods
        private void Init()
        {
            try
            {
                this._PartnerChildValidator = new ValidationTools();
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
            this._Childs = new List<ChildModel>();
            this._ValidChilds = new List<ChildModel>();
            personLastName = "";

            // Title ComboBox
            cbTitle1.ItemsSource = DB.Title.GetTitles();
        }

        public override void defineToolbarContent()
        {
        }


        /// <summary>
        /// Löscht den Inhalt des Fehlerspeichers
        /// AUthor: Antonios Fesenmeier
        /// </summary>
        public void clearErrorMsg()
        {
            this._PartnerChildValidator.clearSB();
            _PartnerChildIsValid = true;
        }


        /// <summary>
        /// Gibt den Inhalt des Fehlerspeichers zurück
        /// Author Antonios Fesenmeier
        /// </summary>
        public void getErrorMsg()
        {
            if (this._PartnerChildIsValid == false)
            {
                MessageBoxEnhanced.Error(_PartnerChildValidator.getErrorMsg().ToString());
                this._PartnerChildValidator.clearSB();
            }
        }

        /// <summary>
        /// Rückgabe aller gültigen Kinder des Datagrids!
        /// return IList<DB.Child>
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
        public IList<DB.Child> getChilds()
        {
            IList<DB.Child> lChilds = new List<DB.Child>();
            foreach (ChildModel cM in _ValidChilds)
            {
                var aChild = new DB.Child();
                aChild.DateOfBirth = cM.Birthday;
                aChild.FirstName = cM.FirstName;
                aChild.LastName = cM.LastName;
                aChild.IsFemale = cM.IsFemale;
                lChilds.Add(aChild);
            }

            return lChilds;
        }


        /// <summary>
        /// Überprüft alle Pflichtfelder und optionalen Felder der Partnergruppenbox!
        /// Bei auftretenden Fehlern erfolgt eine entsprechende Meldung!
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
        public bool checkPartnerChildTab()
        {
            clearErrorMsg();

            #region checkMandatoryFieldsPartner
            if (GrBPartner.IsEnabled)
            {
                if (this._PartnerChildValidator.IsNullOrEmpty(txtFirstName1.Text) == false)
                {
                    this._PartnerChildIsValid = false;
                    this._PartnerChildValidator.addError("Vorname", "Bitte geben Sie den Vornamen des Partners an!");
                }

                if (this._PartnerChildValidator.IsNullOrEmpty(txtLastName1.Text) == false)
                {
                    this._PartnerChildIsValid = false;
                    this._PartnerChildValidator.addError("Nachname", "Bitte geben Sie den Nachnamen des Partners an!");
                }

                if (dpBirthday1.SelectedDate.HasValue == false)
                {
                    this._PartnerChildIsValid = false;
                    this._PartnerChildValidator.addError("Geburtstag des Partners", "Bitte geben Sie den Geburtstag an!");
                }

                if (this._PartnerChildIsValid == true)
                {
                    if (this._PartnerChildValidator.IsName("Vorname", txtFirstName1.Text, "Bitte geben Sie den Vornamen des Partners an!") == false)
                        this._PartnerChildIsValid = false;

                    if (this._PartnerChildValidator.IsName("Nachname", txtLastName1.Text, "Bitte geben Sie den Nachnamen des Partners an!") == false)
                        this._PartnerChildIsValid = false;
                }
            }
            #endregion


            // Nachdem nun alle Pflichtfelder befüllt wurden und geprüft wurden,
            // werden die freiwilligen Felder geprüft, sind auch diese Korrekt
            // wird der nächste Tab freigegeben!
            #region CheckOptionalFields

            if (this._PartnerChildValidator.IsNullOrEmpty(txtTelno1.Text) == true &&
                this._PartnerChildValidator.IsPhoneNumber("Telefonnummer ", txtTelno1.Text, "Die Telefonnummer des Kunden ist falsch!") == false)
                this._PartnerChildIsValid = false;

            if (this._PartnerChildValidator.IsNullOrEmpty(txtMobileNo1.Text) == true &&
              this._PartnerChildValidator.IsMobileNumber("Mobilnummer ", txtMobileNo1.Text, "Die Mobilnummer des Kunden ist falsch!") == false)
                this._PartnerChildIsValid = false;

            if (this._PartnerChildValidator.IsNullOrEmpty(txtEmail1.Text) == true &&
                this._PartnerChildValidator.IsEMail("E-Mail ", txtEmail1.Text, "Die E-Mail des Kunden ist falsch!") == false)
                this._PartnerChildIsValid = false;

            if (this._PartnerChildValidator.IsNullOrEmpty(txtNationalCountry1.Text) == true &&
                 this._PartnerChildValidator.IsName("Geburtsland", txtNationalCountry1.Text, "Das Geburtsland des Kunden ist falsch!") == false)
                this._PartnerChildIsValid = false;

            if (this._PartnerChildValidator.IsNullOrEmpty(txtNativeCountry1.Text) == true &&

                 this._PartnerChildValidator.IsName("Staatsangehörigkeit", txtNativeCountry1.Text, "Die Staatsangehörigkeit des Kunden ist falsch!") == false)
                this._PartnerChildIsValid = false;

            #endregion

            if (GrBChildren.IsEnabled)
            {
                if (CheckChildDataGrid() == false)
                    this._PartnerChildIsValid = false;
            }

            return this._PartnerChildIsValid;
        }


        /// <summary>
        /// Überprüfung der Kinder im Datagrid
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <returns></returns>
        private bool CheckChildDataGrid()
        {
            int count = 1;
            if (_ValidChilds.Count > 0)
                _ValidChilds.Clear();

            //Datenbestand aus Datagrid sichern
            IList<ChildModel> tempChilds = _Childs.ToList();

            //Letztes (leeres) Child entfernen
            if (_Childs.Count > 0)
                _Childs.Remove(_Childs.Last());
            dtgChildren.ItemsSource = _Childs;
            // Durch die Kinder itterieren
            //for (int rowIdx = 0; rowIdx < dtgChildren.Items.Count; rowIdx++)

            foreach (ChildModel actualChild in dtgChildren.ItemsSource)
            {

                if (_PartnerChildValidator.IsNullOrEmpty(actualChild.FirstName) == false)
                {
                    this._PartnerChildIsValid = false;
                    _PartnerChildValidator.addError("Vorname Kind" + count, "Bitte geben Sie den Vornamen von Kind " + count + " an!");
                }

                if (_PartnerChildValidator.IsNullOrEmpty(actualChild.FirstName) == true)
                {
                    if (_PartnerChildValidator.IsName("Vorname Kind" + count, actualChild.FirstName, "Bitte geben Sie den Vornamen von Kind " + count + " an!") == false)
                        this._PartnerChildIsValid = false;
                }

                if (_PartnerChildValidator.IsNullOrEmpty(actualChild.LastName) == false)
                {
                    this._PartnerChildIsValid = false;
                    _PartnerChildValidator.addError("Nachname Kind" + count, "Bitte geben Sie den Nachnamen von Kind " + count + " an!");
                }

                if (_PartnerChildValidator.IsNullOrEmpty(actualChild.FirstName) == true)
                {
                    if (_PartnerChildValidator.IsName("Nachname Kind" + count, actualChild.LastName, "Bitte geben Sie den Nachnamen von Kind " + count + " an!") == false)
                        this._PartnerChildIsValid = false;
                }

                DateTime date = actualChild.Birthday;
                TimeSpan ts = System.DateTime.Now.Subtract(date);

                if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
                {
                    MessageBox.Show("Bitte geben Sie ein korrektes Geburtsdatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                    this._PartnerChildIsValid = false;
                }

                count++;

                //Hinzufügen zur Liste
                if (this._PartnerChildIsValid == true)
                    _ValidChilds.Add(actualChild);
            }

            //löschen des letzten leeren kindes rückgängig machen
            if (this._PartnerChildIsValid == false)
            {
                _Childs = tempChilds;
                dtgChildren.ItemsSource = _Childs;
            }
            return this._PartnerChildIsValid;
        }


        /// <summary>
        /// Hinzufügen eines Kindobjektes zum Datagrid
        /// Author: Georg Schmid
        /// </summary>
        public void addNewChildtoDatagrid()
        {
            GrBChildren.IsEnabled = true;

            //Combobox belegen
            CBItem male = new CBItem();
            male.Name = "Männlich";
            CBItem female = new CBItem();
            female.Name = "Weiblich";
            List<CBItem> newGenderTypes = new List<CBItem>();
            newGenderTypes.Add(male);
            newGenderTypes.Add(female);

            if (_Childs.Count == 0)
            {
                //Hinzufügen eines Kindes ermöglichen
                ChildModel newChild = new ChildModel();
                newChild.genderType = newGenderTypes;
                newChild.ChildID = 0;
                newChild.FirstName = "";
                newChild.LastName = personLastName;
                newChild.Birthday = DateTime.Now;
                newChild.IsFemale = false;
                newChild.isAdded = false;
                this._Childs.Add(newChild);
            }
            else
            {
                //Prüfung ob alle zum Speichern vorgemerkt sind Nur dann Neue Zeile
                if (_Childs.Last().isAdded == true)
                {
                    //Hinzufügen eines Kindes ermöglichen
                    ChildModel newChild = new ChildModel();
                    newChild.genderType = newGenderTypes;
                    newChild.ChildID = 0;
                    newChild.FirstName = "";
                    newChild.LastName = personLastName;
                    newChild.Birthday = DateTime.Now;
                    newChild.IsFemale = false;
                    newChild.isAdded = false;
                    this._Childs.Add(newChild);
                }
            }

            dtgChildren.Items.Refresh();
            dtgChildren.ItemsSource = this._Childs;
            dtgChildren.Items.Refresh();

        }
        
        /// <summary>
        /// Ermöglicht es den Nachnamen zu setzen, damit er default beim Kind eingetragen wird
        /// </summary>
        /// <param name="lastname">Nachname der Person</param>
        public void setPersonLastName(String lastname)
        {
            personLastName = lastname;
        }


        #endregion



        #region Events
        /// <summary>
        /// Wird das Geburtsdatum des Partners editiert, wird auf Gültigkeit geprüft:
        /// Geburtsdatum darf nicht in der Zukunft liegen oder nicht länger als 120 Jahre zurückliegen
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpBirthday1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
            {
            DateTime date = dpBirthday1.SelectedDate.GetValueOrDefault(System.DateTime.Today);
            TimeSpan ts = System.DateTime.Now.Subtract(date);

            if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
            {
                MessageBox.Show("Bitte geben Sie ein korrektes Geburtsdatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                dpBirthday1.SelectedDate = System.DateTime.Now;
            }
        }

  
        /// <summary>
        /// Wird das Geburtsdatum des Kindes editiert, wird auf Gültigkeit geprüft:
        /// Geburtsdatum darf nicht in der Zukunft liegen oder nicht länger als 120 Jahre zurückliegen
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dpChildrensBirthDay_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = dpBirthday1.SelectedDate.GetValueOrDefault(System.DateTime.Today);
            TimeSpan ts = System.DateTime.Now.Subtract(date);

            if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
            {
                MessageBox.Show("Bitte geben Sie ein korrektes Geburtsdatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                dpBirthday1.SelectedDate = System.DateTime.Now;
            }
        }

        private void pbAddDelChild_Click(object sender, RoutedEventArgs e)
        {
            _PartnerChildIsValid = true;
            pNewClientPerson pPerson = getPageFromTabs<pNewClientPerson>();
            var row = dtgChildren.SelectedCells.ElementAt(0);
            var currentSelectedRow = row.Item as ChildModel;
            int rowId = dtgChildren.Items.IndexOf(currentSelectedRow);
            DataGridRow currRow = dtgChildren.GetRow(rowId);

            if (currentSelectedRow.isAdded == false)
            {
                ////Prüfen des Kindes
                //if (CheckChildDataGrid())
                //{
                if (currentSelectedRow != null)
                {
                    ContentPresenter cpGender = dtgChildren.Columns[3].GetCellContent(currRow) as ContentPresenter;
                    var combobox = DataGridHelper.GetVisualChild<ComboBox>(cpGender);
                    CBItem cbGenderItem = (CBItem)combobox.SelectedItem;

                    if (cbGenderItem.Name == "Männlich")
                    {
                        currentSelectedRow.IsFemale = false;
                    }
                    else
                    {
                        currentSelectedRow.IsFemale = true;
                    }

                    //Combobox umsortieren
                    CBItem male = new CBItem();
                    male.Name = "Männlich";
                    CBItem female = new CBItem();
                    female.Name = "Weiblich";
                    List<CBItem> newGenderTypes = new List<CBItem>();
                    if (currentSelectedRow.IsFemale)
                    {
                        newGenderTypes.Add(female);
                        newGenderTypes.Add(male);
                    }
                    else
                    {
                        newGenderTypes.Add(male);
                        newGenderTypes.Add(female);
                    }

                    currentSelectedRow.genderType = newGenderTypes;
                    currentSelectedRow.isAdded = true;

                    dtgChildren.CommitEdit();
                    dtgChildren.Items.Refresh();


                    dtgChildren.Visibility = System.Windows.Visibility.Visible;
                    addNewChildtoDatagrid();
                    pPerson.pbNewChild.Visibility = System.Windows.Visibility.Hidden;
                }
                //}
                //else
                //{
                //    getErrorMsg();
                //}
            }
            else
            {

                var message = "Soll der gewählte Datensatz gelöscht werden?";

                var dialogResult = MessageBox.Show(message, "Bestätigung erfordert", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {
                    this._Childs.RemoveAt(rowId);
                    dtgChildren.ItemsSource = this._Childs;

                    if (dtgChildren.Items.Count == 0)
                        addNewChildtoDatagrid();

                    dtgChildren.CommitEdit();
                    dtgChildren.Items.Refresh();
                }



            }

            pPerson.setNumberOfChild((this._Childs.Count - 1).ToString());


        }
        #endregion
    }
}
