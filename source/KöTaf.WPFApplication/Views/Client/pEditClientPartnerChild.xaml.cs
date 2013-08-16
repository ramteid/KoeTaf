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
using KöTaf.DataModel;

namespace KöTaf.WPFApplication.Views.Client
{
    /// <summary>
    /// Interaktionslogik zum Editieren eines Partner bzw. Kinderdatensatzes
    /// Grundlogik/design: Antonios Fesenmeier
    /// Angepasst durch Georg Schmid
    /// </summary>
    public partial class pEditClientPartnerChild : KPage
    {
        private List<int> deletedChilds;
        private bool _PartnerChildIsValid = true;
        private ValidationTools _PartnerChildValidator;
        
        private IList<ChildModel> _ValidChilds;
        private readonly Person _currentPerson;
        private IList<ChildModel> _Childs;

        public pEditClientPartnerChild(Person currentPerson)
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
            this.deletedChilds = new List<int>();

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
            this.DataContext = _currentPerson;
        }

        public override void defineToolbarContent()
        {
            FillChildDataGrid(DB.Child.GetChildren(null, _currentPerson.PersonID));
        }

        public void clearErrorMsg()
        {
            this._PartnerChildValidator.clearSB();
            _PartnerChildIsValid = true;
        }

        public void getErrorMsg()
        {
            if (this._PartnerChildIsValid == false)
            {
                MessageBoxEnhanced.Error(_PartnerChildValidator.getErrorMsg().ToString());
                this._PartnerChildValidator.clearSB();
            }
        }

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
                aChild.ChildID = cM.ChildID;
                lChilds.Add(aChild);
            }

            return lChilds;
        }

        public List<int> getDeletedChilds()
        {
            return deletedChilds;
        }

        public void FillChildDataGrid(IEnumerable<Child> childs)
        {
            GrBChildren.IsEnabled = true;
            // Wenn das Datagrid nicht leer ist, müssen zunächst alle Zeilen gelöscht werden.
            // Folgedessen müssen soviele Zeilen erstellt werden wie es Kinder gibt!

            if (this._Childs.Count > 0)
                this._Childs.Clear();

            //Combobox belegen
            CBItem male = new CBItem();
            male.Name = "Männlich";
            CBItem female = new CBItem();
            female.Name = "Weiblich";


            if (!(childs.Count() == 0))
            {
                foreach (Child child in childs)
                {
                    List<CBItem> existingGenderTypes = new List<CBItem>();
                    if (child.IsFemale)
                    {
                        existingGenderTypes.Add(female);
                        existingGenderTypes.Add(male);
                    }
                    else
                    {
                        existingGenderTypes.Add(male);
                        existingGenderTypes.Add(female);
                    }
                    

                    ChildModel existingChild = new ChildModel();
                    existingChild.genderType = existingGenderTypes;
                    existingChild.ChildID = child.ChildID;
                    existingChild.FirstName = child.FirstName;
                    existingChild.LastName = child.LastName;
                    existingChild.Birthday = child.DateOfBirth;
                    existingChild.IsFemale = child.IsFemale;
                    existingChild.isAdded = true;
                    this._Childs.Add(existingChild);
                }
            }

            dtgChildren.Items.Refresh();
            dtgChildren.ItemsSource = this._Childs;
            dtgChildren.Items.Refresh();
            addNewChildtoDatagrid();
            
        }

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
            if(this._PartnerChildIsValid == false)
            {
                _Childs = tempChilds;
                dtgChildren.ItemsSource = _Childs;
            }

            return this._PartnerChildIsValid;
        }

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
                newChild.LastName = _currentPerson.LastName;
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
                    newChild.LastName = _currentPerson.LastName;
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
              
        #endregion



        #region Events

        private 
            
            void dpBirthday1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = dpBirthday1.SelectedDate.GetValueOrDefault(System.DateTime.Today);
            TimeSpan ts = System.DateTime.Now.Subtract(date);

            if (date > System.DateTime.Now || ts.TotalDays >= (365 * 120))
            {
                MessageBox.Show("Bitte geben Sie ein korrektes Geburtsdatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                dpBirthday1.SelectedDate = System.DateTime.Now;
            }
        }

        private void pbAddDelRev_Click(object sender, RoutedEventArgs e)
        {
            _PartnerChildIsValid = true;
            pEditClientPerson pPerson = getPageFromTabs<pEditClientPerson>();
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
                    deletedChilds.Add(currentSelectedRow.ChildID);
                    this._Childs.RemoveAt(rowId);
                    dtgChildren.ItemsSource = this._Childs;

                    if (dtgChildren.Items.Count == 0)
                        addNewChildtoDatagrid();

                    dtgChildren.CommitEdit();
                    dtgChildren.Items.Refresh();
                }



            }
            
            pPerson.setNumberOfChild((this._Childs.Count-1).ToString());
            

        }
        #endregion

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

            


       
    }
}
