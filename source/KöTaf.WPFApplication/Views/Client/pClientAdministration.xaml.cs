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
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Models;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Printer;
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Views.Client;
using KöTaf.Utils.Parser;
using KöTaf.Utils.FileOperations;
using DB = KöTaf.DataModel;
using KöTaf.Utils.UserSession;


namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik für ClientAdministration
    /// Author: Antonios Fesenmeier, Georg Schmid
    /// </summary>
    public partial class pClientAdministration : KPage
    {
        private readonly UserAccount _UserAccount;
        private DataGridPaging<Person> _DataGridPaging;
        private IEnumerable<Person> _Persons;
        private List<SimpleTabItem> _myTabList;
        private List<SimpleTabItem> _myTabListEdit;
        private int pagingStartValue = 0;


        #region Constructor

        public pClientAdministration(int? pagingStartValue = null)
        {
            this._UserAccount = UserSession.userAccount;
            InitializeComponent();

            if (pagingStartValue.HasValue)
                this.pagingStartValue = pagingStartValue.Value;

            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            _Persons = Person.GetPersons();
            IEnumerable<Person> persons = _Persons;
            _myTabList = new List<SimpleTabItem>();
            _myTabListEdit = new List<SimpleTabItem>();

            //Sortierung Aktiv/Inaktiv
            if (_Persons != null)
                _Persons = _Persons.OrderByDescending(t => t.IsActive);

            //Übergibt dem Paging die Gesamtliste
            _DataGridPaging = new DataGridPaging<Person>(_Persons);
            _DataGridPaging.setStartOfDataGridItems(pagingStartValue);

            List<String> filters = new List<string>();
            filters.Add(IniParser.GetSetting("FILTER", "all"));
            filters.Add(IniParser.GetSetting("FILTER", "active"));
            filters.Add(IniParser.GetSetting("FILTER", "inactive"));
            cmbActiveFilter.ItemsSource = filters;

            // Lade Bezeichnung für Kassenabschluss und Kasenabrechnung aus der Konfigurationsdatei
            try
            {
                var btn1 = FindResource("pbPrintEnrolmentForm") as Button;
                btn1.ToolTip = IniParser.GetSetting("DOCUMENTS", "Aufnahmeformular");

                var btn2 = FindResource("pbPrintDisclaimer") as Button;
                btn1.ToolTip = IniParser.GetSetting("DOCUMENTS", "Haftungsausschluss");
            }
            catch
            {
            }
        }

        /// <summary>
        /// Festlegen des ToolbarInhaltes
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        public override void defineToolbarContent()
        {

            List<ClientSearchComboBoxItemModel> searchItems = new List<ClientSearchComboBoxItemModel>()
                {
                    new ClientSearchComboBoxItemModel { Value = IniParser.GetSetting("CLIENT", "clientName"), SearchType = ClientSearchComboBoxItemModel.Type.FullName },
                    new ClientSearchComboBoxItemModel { Value = IniParser.GetSetting("CLIENT", "clientLocation"), SearchType = ClientSearchComboBoxItemModel.Type.ResidentialAddress },
                    new ClientSearchComboBoxItemModel { Value = IniParser.GetSetting("CLIENT", "clientTafelNumber"), SearchType = ClientSearchComboBoxItemModel.Type.TableNo }
                };

            this.parentToolbar.addButton(IniParser.GetSetting("CLIENT", "newClient"), pbNewClient_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "print"), pbListPrint_Click);
            this.parentToolbar.addPagingBar(firstSideProcessor, prevSideProcessor, nextSideProcessor, lastSideProcessor);

            this.parentToolbar.addSearchPanel(processKeyUp);

            this.parentToolbar.searchPanel.addComboBox<ClientSearchComboBoxItemModel>(searchItems);

            // Der Textbox eine KeyUp-Funktion zuweisen
            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);

            //DataGrid kann erst jetzt gefüllt werden, damit das Paging die PagingBar ansprechen kann.
            FillClientDataGrid(_DataGridPaging.ActualSide());

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                ClientDataGrid.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
            
        }

        /// <summary>
        /// Je nach Filteroptionen, wird die Datensicht angepasst
        /// Status: aktiv, inaktiv, alle
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        private void FillClientDataGrid_toggleActiveState()
        {
            if (cmbActiveFilter.SelectedItem.ToString().Equals(IniParser.GetSetting("FILTER", "all")))
            {
                _Persons = Person.GetPersons();
            }
            else if (cmbActiveFilter.SelectedItem.ToString().Equals(IniParser.GetSetting("FILTER", "inactive")))
            {
                _Persons = Person.GetPersons();
                _Persons = _Persons.Where(p => p.IsActive == false);
            }
            else if (cmbActiveFilter.SelectedItem.ToString().Equals(IniParser.GetSetting("FILTER", "active")))
            {
                _Persons = Person.GetPersons();
                _Persons = _Persons.Where(p => p.IsActive == true);
            }
            _DataGridPaging = new DataGridPaging<Person>(_Persons);
            FillClientDataGrid(_DataGridPaging.FirstSide());
        }

        /// <summary>
        /// Befüllt das Datagrid mit den Datensätzen
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="persons"></param>
        private void FillClientDataGrid(IEnumerable<Person> persons)
        {
            ClientDataGrid.ItemsSource = persons;
            ChangePagingBar();
        }

        /// <summary>
        /// Erlaubt das Paging des Datagrids
        /// 
        /// Author: GeorgSchmid, Antonios Fesenmeier
        /// </summary>
        private void ChangePagingBar()
        {
            //Alle Buttons aktivieren
            this.parentToolbar.pagingBar.firstButton.IsEnabled = true;
            this.parentToolbar.pagingBar.prevButton.IsEnabled = true;
            this.parentToolbar.pagingBar.nextButton.IsEnabled = true;
            this.parentToolbar.pagingBar.lastButton.IsEnabled = true;

            //Label der PagingBar aktualisieren
            this.parentToolbar.pagingBar.fromBlock.Text = _DataGridPaging.GetStart().ToString();
            this.parentToolbar.pagingBar.toBlock.Text = _DataGridPaging.GetEnd().ToString();
            this.parentToolbar.pagingBar.totalBlock.Text = _DataGridPaging.GetTotal().ToString();

            //Nur Buttons zulassen, die möglich sind
            if (_DataGridPaging.GetStart() == 1 || _DataGridPaging.GetStart() == 0)
            {
                this.parentToolbar.pagingBar.firstButton.IsEnabled = false;
                this.parentToolbar.pagingBar.prevButton.IsEnabled = false;
            }
            if (_DataGridPaging.GetEnd() == _DataGridPaging.GetTotal())
            {
                this.parentToolbar.pagingBar.nextButton.IsEnabled = false;
                this.parentToolbar.pagingBar.lastButton.IsEnabled = false;
            }
        }


        #endregion

        #region Events

        /// <summary>
        /// KeyUp Event für die Suchfunktion. Suchergebnis wird mit Paging angepasst
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="searchValue"></param>
        private void processKeyUp(string searchValue)
        {
            ClientSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<ClientSearchComboBoxItemModel>();

            IEnumerable<Person> persons = _Persons;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.SearchType)
                {
                    case ClientSearchComboBoxItemModel.Type.ResidentialAddress:
                        persons = _Persons.Where(p => p.City.ToLower().Contains(searchValue));
                        break;

                    case ClientSearchComboBoxItemModel.Type.FullName:
                        persons = _Persons.Where(p => p.FullName.ToLower().Contains(searchValue));
                        break;

                    case ClientSearchComboBoxItemModel.Type.TableNo:
                        persons = _Persons.Where(p => p.TableNo.Equals(Convert.ToInt32((searchValue.ToString()))));
                        break;
                }
            }

            //_DataGridPaging mit neuem Listeninhalt neu initialisieren
            _DataGridPaging = new DataGridPaging<Person>(persons);
            FillClientDataGrid(_DataGridPaging.FirstSide());
        }


        /// <summary>
        /// Author: 
        /// </summary>
        /// <param name="button"></param>
        public void pbListPrint_Click(Button button)
        {

            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
                return;
            }
            DataGrid allPrintData = new DataGrid();
            allPrintData.ItemsSource = _DataGridPaging.getItems();
            try
            {
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Client, allPrintData.ItemsSource);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Button zum hinzufügen eines neuen Kunden, laden der jeweiligen Tabseiten und festlegen des ToolbarContents
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="Button"></param>
        private void pbNewClient_Click(Button Button)
        {
            /*  var keyValueList = ClientDataGrid.ToKeyValueList();
              KöTaf.Utils.Printer.CSVExporter csv = new Utils.Printer.CSVExporter(keyValueList);
              var header = csv.GetHeader();
              var content = csv.GetData();
              var csvFull = csv.GetCsv();
              csv.Write();
              */


            // Adding a new tab with the according page shown under this tab
            KPage person = new KöTaf.WPFApplication.Views.Client.pNewClientPerson();
            SimpleTabItem tabPerson = new SimpleTabItem(IniParser.GetSetting("CLIENT", "personalData"), person);
            this._myTabList.Add(tabPerson);

            KPage partnerChild = new KöTaf.WPFApplication.Views.Client.pNewClientPartnerChild();
            SimpleTabItem tabPartnerChild = new SimpleTabItem(IniParser.GetSetting("CLIENT", "partnerAndChildren"), partnerChild);
            this._myTabList.Add(tabPartnerChild);

            KPage revenues = new KöTaf.WPFApplication.Views.Client.pNewClientRevenues();
            SimpleTabItem tabRevenues = new SimpleTabItem(IniParser.GetSetting("CLIENT", "revenue"), revenues);
            this._myTabList.Add(tabRevenues);

            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), pbAbort_Click);
            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "next"), pbNext_Click);
            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSave_Click);

            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), pbAbort_Click);
            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "next"), pbNext_Click);
            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSave_Click);

            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), pbAbort_Click);
            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "next"), pbNext_Click);
            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSave_Click);

            SinglePage singlePage = new SinglePage(this, IniParser.GetSetting("CLIENT", "createNewClient"), this._myTabList);
        }


        /// <summary>
        /// Abbrechen der Kundenanlage, navigiert zurück zur Startseite der Kundenverwaltung
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="button"></param>
        public void pbAbort_Click(Button button)
        {
            KPage pageClientAdministration = new KöTaf.WPFApplication.Views.pClientAdministration(pagingStartValue);
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("CLIENT", "clientManagement"), pageClientAdministration);
        }

        /// <summary>
        /// Erlaubt das zurück gehen in den Tabs
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="button"></param>
        public void pbBack_Click(Button button)
        {
            TabControl tC = this.parentTabControl;

            if (((TabItem)tC.Items.GetItemAt(1)).IsSelected)
            {
                ((TabItem)tC.Items.GetItemAt(0)).Focus();
            }

            else if (((TabItem)tC.Items.GetItemAt(2)).IsSelected)
            {
                if (!(((TabItem)tC.Items.GetItemAt(1)).IsEnabled))
                {
                    ((TabItem)tC.Items.GetItemAt(0)).Focus();
                }
                else
                    ((TabItem)tC.Items.GetItemAt(1)).Focus();

            }
        }
        /// <summary>
        /// 
        /// Erlaubt das vorwärts gehen in den Tabs
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="senderButton"></param>
        public void pbNext_Click(Button senderButton)
        {

            pNewClientPerson pPerson = getPageFromTabs<pNewClientPerson>();
            TabControl tC = this.parentTabControl;

            pPerson.EnableTabs();

            if (((TabItem)tC.Items.GetItemAt(0)).IsSelected)
            {
                if (((TabItem)tC.Items.GetItemAt(1)).IsEnabled)
                {
                    ((TabItem)tC.Items.GetItemAt(1)).Focus();
                }
                else
                {
                    ((TabItem)tC.Items.GetItemAt(2)).Focus();
                }
            }
            else if (((TabItem)tC.Items.GetItemAt(1)).IsSelected)
            {
                ((TabItem)tC.Items.GetItemAt(2)).Focus();
            }
        }


        /// <summary>
        /// Überprüft alle Untertabs auf Vollständigkeit/ Korrektheit und speichert 
        /// bei Korrektheit alle Kinder, Partner, die Person selbst und deren Einkommen
        /// mit allen Abhängigkeiten in die Datenbank
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="button"></param>
        public void pbSave_Click(Button button)
        {
            TabControl tC = this.parentTabControl;

            pNewClientPerson pPerson = getPageFromTabs<pNewClientPerson>();
            pNewClientPartnerChild pPartnerChild = getPageFromTabs<pNewClientPartnerChild>();
            pNewClientRevenues pRevenues = getPageFromTabs<pNewClientRevenues>();

            bool _PersonIsValid = pPerson.checkPersonTab(),
                _ChildPartnerIsValid = pPartnerChild.checkPartnerChildTab(),
                _RevenuesIsValid = pRevenues.checkRevenuesTab();

            if (_PersonIsValid == false)
                pPerson.getErrorMsg();
            pPerson.clearErrorMsg();

            if (((TabItem)tC.Items.GetItemAt(1)).IsEnabled)
            {
                if (_PersonIsValid && _ChildPartnerIsValid == false)
                    pPartnerChild.getErrorMsg();
            }
            pPartnerChild.clearErrorMsg();

            if (((TabItem)tC.Items.GetItemAt(2)).IsEnabled)
            {
                if (_PersonIsValid && _ChildPartnerIsValid && _RevenuesIsValid == false)
                    pRevenues.getErrorMsg();
            }
            pRevenues.clearErrorMsg();

            if (_PersonIsValid && _ChildPartnerIsValid && _RevenuesIsValid)
            {
                try
                {
                    // Daten für Person holen
                    DB.Title title = pPerson.cbTitle.SelectedItem as DB.Title;
                    DB.FamilyState fS = pPerson.cbFamilyState.SelectedItem as DB.FamilyState;

                    String firstName = pPerson.txtFirstName.Text;
                    String lastName = pPerson.txtLastName.Text;
                    String street = pPerson.txtStreet.Text;
                    String nationality = pPerson.txtNationalCountry.Text;
                    DateTime birthday = (DateTime)pPerson.dpBirthday.SelectedDate;
                    int group = Convert.ToInt32(pPerson.cbGroup.SelectedValue.ToString());
                    int zipCode = Convert.ToInt32(pPerson.txtZipCode.Text);
                    String city = pPerson.txtCity.Text;
                    DateTime startDate = (DateTime)pPerson.dpStartDate.SelectedDate;
                    DateTime endDate = (DateTime)pPerson.dpEndDate.SelectedDate;
                    int childs = Convert.ToInt32(pPerson.txtChildrens.Content);
                    String nativeCountry = pPerson.txtNativeCountry.Text;
                    String email = pPerson.txtEmail.Text;
                    String mobileNo = pPerson.txtMobileNo.Text;
                    String phoneNo = pPerson.txtTelno.Text;
                    int tableNo = Convert.ToInt32(pPerson.txtTableNo.Text);
                    // Daten für Partner holen

                    String firstNamePartner = pPartnerChild.txtFirstName1.Text;
                    String lastNamePartner = pPartnerChild.txtLastName1.Text;
                    String nationalityPartner = pPartnerChild.txtNationalCountry1.Text;
                    DateTime? birthdayPartner = pPartnerChild.dpBirthday1.SelectedDate;
                    String nativeCountryPartner = pPartnerChild.txtNativeCountry1.Text;
                    String maritalPhone = pPartnerChild.txtTelno1.Text;
                    String maritalMobile = pPartnerChild.txtMobileNo1.Text;
                    String maritalEmail = pPartnerChild.txtEmail1.Text;

                    DB.Title maritalTitle = pPartnerChild.cbTitle1.SelectedItem as DB.Title;
                    int maritalTitleID = maritalTitle.TitleID;

                    //UserID beschreiben
                    int userID = _UserAccount.UserAccountID;
                            
                    // Person speichern              
                    var personID = DB.Person.Add(title.TitleID, fS.FamilyStateID, firstName, lastName, street, nationality, birthday, group, zipCode, city, startDate, endDate,
                        userID, nativeCountry, email, mobileNo, phoneNo, firstNamePartner, lastNamePartner, nationalityPartner, birthdayPartner, nativeCountryPartner, maritalPhone, maritalMobile, maritalEmail, maritalTitle.TitleID, null, tableNo);

                    // Alle Kinder holen und speichern
                    IList<DB.Child> lChilds = new List<DB.Child>();
                    lChilds = pPartnerChild.getChilds();

                    foreach (DB.Child aChild in lChilds)
                    {
                        String aChildLastName = aChild.LastName;
                        String aChildFirstName = aChild.FirstName;
                        bool aChildIsFemale = aChild.IsFemale;
                        DateTime aChildsBirthday = aChild.DateOfBirth;

                        var childID = DB.Child.Add(personID, aChildFirstName, aChildLastName, aChildsBirthday, aChildIsFemale);
                    }

                    // Alle Revenues holen und speichern
                    IList<DB.Revenue> lRevs = new List<DB.Revenue>();
                    lRevs = pRevenues.getRevenues();

                    foreach (DB.Revenue aRev in lRevs)
                    {
                        Double amount = aRev.Amount;
                        String desc = aRev.Description;
                        int revTypID = aRev.RevenueType.RevenueTypeID;
                    
                
                        var revenueID = DB.Revenue.Add(personID, revTypID, amount, aRev.StartDate, aRev.EndDate, desc);
                    }

                    //Wenn erfolgreich Dokumente Drucken
                    MessageBoxResult result1 = MessageBoxEnhanced.Question("Möchten Sie für diesen neuen Kunden das Aufnahmeformular jetzt drucken?");
                    if (result1 == MessageBoxResult.Yes)
                        PrintForms.printClientEnrolmentForm(personID);

                    MessageBoxResult result2 = MessageBoxEnhanced.Question("Möchten Sie für diesen neuen Kunden die Widerrufbelehrung jetzt drucken?");
                    if (result2 == MessageBoxResult.Yes)
                        PrintForms.printClientDisclaimer(personID);

                    //Zurück zur Verwaltung
                    KPage pageClientAdministration = new KöTaf.WPFApplication.Views.pClientAdministration();
                    SinglePage singlePage = new SinglePage(IniParser.GetSetting("CLIENT", "clientManagement"), pageClientAdministration);
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
                }
            }
        }


        /// <summary>
        /// Generiert die Untertabs und deren ToolbarContent zum Editieren eines Kunden
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditPersonButton_Click(object sender, RoutedEventArgs e)
        {
            //Setzt den Rücksprungwert für das Paging
            pagingStartValue = _DataGridPaging.getStartOfDataGridItems();

            Person currentPerson = ClientDataGrid.SelectedItem as Person;

            // Tabs hinzufügen
            KPage person = new KöTaf.WPFApplication.Views.Client.pEditClientPerson(currentPerson);
            SimpleTabItem tabPerson = new SimpleTabItem(IniParser.GetSetting("CLIENT", "personalData"), person);
            this._myTabListEdit.Add(tabPerson);

            KPage partnerChild = new KöTaf.WPFApplication.Views.Client.pEditClientPartnerChild(currentPerson);
            SimpleTabItem tabPartnerChild = new SimpleTabItem(IniParser.GetSetting("CLIENT", "partnerAndChildren"), partnerChild);
            this._myTabListEdit.Add(tabPartnerChild);

            KPage revenues = new KöTaf.WPFApplication.Views.Client.pEditClientRevenues(currentPerson);
            SimpleTabItem tabRevenues = new SimpleTabItem(IniParser.GetSetting("CLIENT", "revenue"), revenues);
            this._myTabListEdit.Add(tabRevenues);

            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), pbAbort_Click);
            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "next"), pbNextEdit_Click);
            tabPerson.toolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSaveEdit_Click);

            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), pbAbort_Click);
            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "next"), pbNextEdit_Click);
            tabPartnerChild.toolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSaveEdit_Click);

            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), pbAbort_Click);
            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "back"), pbBack_Click);
            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "next"), pbNextEdit_Click);
            tabRevenues.toolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), pbSaveEdit_Click);

            SinglePage singlePage = new SinglePage(this, IniParser.GetSetting("CLIENT", "editClient"), this._myTabListEdit);
        }


        /// <summary>
        /// Author: Georg Schmid
        /// </summary>
        /// <param name="senderButton"></param>
        public void pbNextEdit_Click(Button senderButton)
        {

            pEditClientPerson pPerson = getPageFromTabs<pEditClientPerson>();
            TabControl tC = this.parentTabControl;

            pPerson.EnableTabs();

            if (((TabItem)tC.Items.GetItemAt(0)).IsSelected)
            {

                if (((TabItem)tC.Items.GetItemAt(1)).IsEnabled)
                {
                    ((TabItem)tC.Items.GetItemAt(1)).Focus();
                }
                else
                {
                    ((TabItem)tC.Items.GetItemAt(2)).Focus();
                }
            }
            else if (((TabItem)tC.Items.GetItemAt(1)).IsSelected)
            {
                ((TabItem)tC.Items.GetItemAt(2)).Focus();
            }
        }


        /// <summary>
        /// Speichert bei Korrektheit aller Tabs die veränderten Kunden, Kinder, Partner und Einkommensdaten
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="button"></param>
        public void pbSaveEdit_Click(Button button)
        {
            TabControl tC = this.parentTabControl;

            pEditClientPerson pPerson = getPageFromTabs<pEditClientPerson>();
            pEditClientPartnerChild pPartnerChild = getPageFromTabs<pEditClientPartnerChild>();
            pEditClientRevenues pRevenues = getPageFromTabs<pEditClientRevenues>();

            bool _PersonIsValid = pPerson.checkPersonTab(),
                _ChildPartnerIsValid = pPartnerChild.checkPartnerChildTab(),
                _RevenuesIsValid = pRevenues.checkRevenuesTab();

            if (_PersonIsValid == false)
                pPerson.getErrorMsg();
            pPerson.clearErrorMsg();

            if (((TabItem)tC.Items.GetItemAt(1)).IsEnabled)
            {
                if (_PersonIsValid && _ChildPartnerIsValid == false)
                    pPartnerChild.getErrorMsg();
            }
            pPartnerChild.clearErrorMsg();

            if (((TabItem)tC.Items.GetItemAt(2)).IsEnabled)
            {
                if (_PersonIsValid && _ChildPartnerIsValid && _RevenuesIsValid == false)
                    pRevenues.getErrorMsg();
            }
            pRevenues.clearErrorMsg();

            if (_PersonIsValid && _ChildPartnerIsValid && _RevenuesIsValid)
            {
                try
                {
                    // Daten für Person holen
                    DB.Title title = pPerson.cbTitle.SelectedItem as DB.Title;
                    DB.FamilyState fS = pPerson.cbFamilyState.SelectedItem as DB.FamilyState;

                    String firstName = pPerson.txtFirstName.Text;
                    String lastName = pPerson.txtLastName.Text;
                    String street = pPerson.txtStreet.Text;
                    String nationality = pPerson.txtNationalCountry.Text;
                    DateTime birthday = (DateTime)pPerson.dpBirthday.SelectedDate;
                    int group = Convert.ToInt32(pPerson.cbGroup.SelectedValue.ToString());
                    int zipCode = Convert.ToInt32(pPerson.txtZipCode.Text);
                    String city = pPerson.txtCity.Text;
                    DateTime startDate = (DateTime)pPerson.dpStartDate.SelectedDate;
                    DateTime endDate = (DateTime)pPerson.dpEndDate.SelectedDate;
                    int childs = Convert.ToInt32(pPerson.txtChildrens.Content);
                    String nativeCountry = pPerson.txtNativeCountry.Text;
                    String email = pPerson.txtEmail.Text;
                    String mobileNo = pPerson.txtMobileNo.Text;
                    String phoneNo = pPerson.txtTelno.Text;
                    int tableNo = Convert.ToInt32(pPerson.txtTableNo.Text);
                    // Daten für Partner holen

                    String firstNamePartner = pPartnerChild.txtFirstName1.Text;
                    String lastNamePartner = pPartnerChild.txtLastName1.Text;
                    String nationalityPartner = pPartnerChild.txtNationalCountry1.Text;
                    DateTime? birthdayPartner = pPartnerChild.dpBirthday1.SelectedDate;
                    String nativeCountryPartner = pPartnerChild.txtNativeCountry1.Text;
                    String maritalPhone = pPartnerChild.txtTelno1.Text;
                    String maritalMobile = pPartnerChild.txtMobileNo1.Text;
                    String maritalEmail = pPartnerChild.txtEmail1.Text;

                    DB.Title maritalTitle = pPartnerChild.cbTitle1.SelectedItem as DB.Title;

                    //TeamID mit UserID beschreiben
                    int userID = _UserAccount.UserAccountID;


                    //Person aktualisieren
                    int personID = pPerson.getPersonID();
                    if (maritalTitle != null)
                        DB.Person.Update(personID, title.TitleID, fS.FamilyStateID, userID, firstName, lastName, zipCode, city, street, nationality, tableNo, phoneNo, mobileNo, null, nativeCountry, birthday, email, group, startDate, endDate, birthdayPartner, firstNamePartner, lastNamePartner, nationalityPartner, null, nativeCountryPartner, maritalPhone, maritalMobile, maritalEmail, maritalTitle.TitleID);
                    else
                        DB.Person.Update(personID, title.TitleID, fS.FamilyStateID, userID, firstName, lastName, zipCode, city, street, nationality, tableNo, phoneNo, mobileNo, null, nativeCountry, birthday, email, group, startDate, endDate, birthdayPartner, firstNamePartner, lastNamePartner, nationalityPartner, null, nativeCountryPartner, maritalPhone, maritalMobile, maritalEmail);


                    // Alle Kinder holen und speichern
                    IList<DB.Child> lChilds = new List<DB.Child>();
                    lChilds = pPartnerChild.getChilds();

                    foreach (DB.Child aChild in lChilds)
                    {
                        String aChildLastName = aChild.LastName;
                        String aChildFirstName = aChild.FirstName;
                        bool aChildIsFemale = aChild.IsFemale;
                        DateTime aChildsBirthday = aChild.DateOfBirth;

                        if (aChild.ChildID == 0)
                        {
                            var childID = DB.Child.Add(personID, aChildFirstName, aChildLastName, aChildsBirthday, aChildIsFemale);
                        }
                        else
                        {
                            DB.Child.Update(aChild.ChildID, aChildFirstName, aChildLastName, aChildsBirthday, aChildIsFemale);
                        }
                    }

                    // Alle Revenues holen und speichern
                    IList<DB.Revenue> lRevs = new List<DB.Revenue>();
                    lRevs = pRevenues.getRevenues();

                    foreach (DB.Revenue aRev in lRevs)
                    {
                            Double amount = aRev.Amount;
                            String desc = aRev.Description;
                            int revTypID = aRev.RevenueType.RevenueTypeID;

                        if (aRev.RevenueID == 0)
                        {
                            var revenueID = DB.Revenue.Add(personID, revTypID, amount, aRev.StartDate, aRev.EndDate, desc);
                        }
                        else
                        {
                            DB.Revenue.Update(aRev.RevenueID, revTypID, amount, desc, aRev.StartDate, aRev.EndDate);
                        }
                    }

                    //Gelöschte Childs aus Datenbank entfernen
                    List<int> deletedChilds = pPartnerChild.getDeletedChilds();
                    foreach (int dchildID in deletedChilds)
                    {
                        if (dchildID != 0)
                            DB.Child.Delete(dchildID);
                    }

                    //Gelöschte Revenues aus Datenbank entfernen
                    List<int> deletedRevenues = pRevenues.getDeletedRevenues();
                    foreach (int drevID in deletedRevenues)
                    {
                        if(drevID != 0)
                            DB.Revenue.Delete(drevID);
                    }

                    //Zurück zur Verwaltung
                    KPage pageClientAdministration = new KöTaf.WPFApplication.Views.pClientAdministration(pagingStartValue);
                    SinglePage singlePage = new SinglePage(IniParser.GetSetting("CLIENT", "clientManagement"), pageClientAdministration);
                }
                catch
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
                }
            }
        }


        /// <summary>
        /// Ermöglicht es einen Kunden auf inatkiv/aktiv zu setzen, dabei wird die jeweilige Ansicht neu geladen.
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePersonActivateStateButton_Click(object sender, RoutedEventArgs e)
        {
            var currentSelectedPerson = ClientDataGrid.SelectedItem as Person;
            if (currentSelectedPerson != null)
            {
                var personId = currentSelectedPerson.PersonID;
                var state = currentSelectedPerson.IsActive;
                var message = string.Format(IniParser.GetSetting("CLIENT", "confirmationFormatString"),
                                    currentSelectedPerson.FullName,
                                    ((state) ? IniParser.GetSetting("FILTER", "inactive") : IniParser.GetSetting("FILTER", "active")));

                var dialogResult = MessageBox.Show(message, IniParser.GetSetting("CLIENT", "confirmationNeeded"), MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {
                    if (state)
                        Person.Deactivate(personId);
                    else
                        Person.Activate(personId);

                    _Persons = Person.GetPersons();
                    if (_Persons != null)
                        _Persons = _Persons.OrderByDescending(t => t.IsActive);

                    if (this.parentToolbar.searchPanel.searchBox.Text == IniParser.GetSetting("APPSETTINGS", "search"))
                        processKeyUp("");
                    else
                        processKeyUp(this.parentToolbar.searchPanel.searchBox.Text);
                }
            }
        }

        private void prevSideProcessor(String str)
        {
            FillClientDataGrid(_DataGridPaging.PrevSide());
        }

        private void nextSideProcessor(String str)
        {
            FillClientDataGrid(_DataGridPaging.NextSide());
        }

        private void firstSideProcessor(String str)
        {
            FillClientDataGrid(_DataGridPaging.FirstSide());
        }

        private void lastSideProcessor(String str)
        {
            FillClientDataGrid(_DataGridPaging.LastSide());
        }

        private void cmbActiveFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.parentToolbar != null)
            {
                this.parentToolbar.searchPanel.searchBox.Text = IniParser.GetSetting("APPSETTINGS", "search");
                FillClientDataGrid_toggleActiveState();
            }
        }

        private void pbPrint_Click(object sender, RoutedEventArgs e)
        {
            KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Client, ClientDataGrid.ItemsSource);
        }

        private void pbPrintDisclaimer_Click(object sender, RoutedEventArgs e)
        {
            Person currentPerson = ClientDataGrid.SelectedItem as Person;
            PrintForms.printClientDisclaimer(currentPerson.PersonID);
        }

        private void pbPrintEnrolmentForm_Click(object sender, RoutedEventArgs e)
        {
            Person currentPerson = ClientDataGrid.SelectedItem as Person;
            PrintForms.printClientEnrolmentForm(currentPerson.PersonID);
        }
        
        #endregion
    }
}
