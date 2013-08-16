/**
 * Class: OldpStatistiken
 * @author Lucas Kögel
 */




// Veraltet!!!!!!!
//
//
//So war die Implementieriung vor dem Redesign!
//
//
//
//Stellt nur noch das Backup des Codes dar.




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
using KöTaf.DataModel.Enums;
using System.Collections;
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;


namespace KöTaf.WPFApplication.Views.Statistic
{
    /// <summary>
    /// Interaktionslogik für pStatistiken.xaml
    /// </summary>
    public partial class OldpStatistiken : KPage
    {
        private string[] cbOtherItemsPassHolder = { "Ausweisgültigkeit bis max.", "Familienstand", "Anzahl Kinder", "Staatsangehörigkeit",
                                                              "Postleitzahl", "Wohnort", "Alter" };
        private string[] cbOtherItemsPartnersAll = { "Staatsangehörigkeit", "Postleitzahl", "Wohnort", "Alter" };
        private string[] cbOtherItemsChildren = { "Postleitzahl", "Wohnort", "Alter" };
        private string[] displayableDataChildren = { "Ausweisnummer", "Vorname", "Nachname", "Alter", "Ausweisgültigkeit bis max.", 
                                                      "Postleitzahl", "Wohnort" };
        private string[] displayableDataPartnersAll = { "Staatsangehörigkeit", "Ausweisnummer", "Vorname", "Nachname", "Name", "Alter", "Ausweisgültigkeit bis max.", 
                                                           "Postleitzahl", "Wohnort", "Adresse"  };
        private string[] displayableDataPassHolder = { "Familienstand", "Anzahl Kinder", "Partner Name" , "Ausweisnummer", "Vorname", "Nachname", "Alter",
                                                              "Ausweisgültigkeit bis","Postleitzahl", "Wohnort", "Adresse" };
        private string[] displayableDataSponsor = { "Name", "Wohnort", "Postleitzahl", "Adresse" };
        private ArrayList wPDynamicContent = new ArrayList();
        private readonly UserAccount _UserAccount;
        private double revenuesThisYear = 0;
        private double revenuesLastYear = 0;
        private double expensesThisYear = 0;
        private double expensesLastYear = 0;
        private int fontSize = 16;
        private int generatedItemsHeight = 27; //the Height for dynamic generated Boxes
        private ArrayList displayedDataComboBoxes = new ArrayList();
        private TextBox tbInput;
       

        #region Constructor

        public OldpStatistiken(UserAccount userAccount)
        {
            this._UserAccount = userAccount;
           
            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            //Setup GUI
            List<SimpleTabItem> myTabList = new List<SimpleTabItem>();

            KPage p1 = new KöTaf.WPFApplication.Views.Statistic.pGeneralStatistic();
            myTabList.Add(new SimpleTabItem("Beliebige Statistik", p1));
            
            SinglePage singlePage = new SinglePage(this, "Statistiken", myTabList);


            GenerateGeneralStatistics();
            fillSavedFilter();

            //Grant access to FilterSets for admins
            //if ((this._UserAccount != null) && (this._UserAccount.IsAdmin))
            //    frame_filter.Content = new Views.Formletter.pFormletterFilterSelection();
            //else
            //{
            //    frame_filter.Content = new Label
            //    {
            //        Content = "Sie müssen als Administrator eingeloggt sein, um Filter-Sets erstellen zu können!"
            //    };
            //    bDeleteSavedFilter.Visibility = Visibility.Hidden;
            //}

            //Save Displayed-Data-Combo-Boxes in an array
            displayedDataComboBoxes.Add(cbDisplayedData1);
            displayedDataComboBoxes.Add(cbDisplayedData2);
            displayedDataComboBoxes.Add(cbDisplayedData3);
            displayedDataComboBoxes.Add(cbDisplayedData4);
            displayedDataComboBoxes.Add(cbDisplayedData5);


        }

        // This function must be defined in each class implementing KPage
        // If you don't want a toolbar, just leave it empty and don't add any button
        public override void defineToolbarContent()
        {
            // Here we declare a button for the toolbar
            // First parameter is the text of the button
            // Second parameter is a reference to any function which is called on button click
            // This function must have return value of type "void" and a single parameter of type "Button"
            //this.parentToolbar.addButton("Neue Serienbrief-Vorlage", createFormletter_Click);

            // This is an example how you can add your own custom elements to the toolbar if you need more than simple buttons
            //WrapPanel someControlElement = new WrapPanel();
            //this.parentToolbar.dpToolbarPanel.Children.Add( someControlElement );

            // This way you can remove everything from the toolbar, making it disappear and free for new buttons and stuff
            //this.parentToolbar.clearContent();
        }

        #region GeneralStatistic
        private void GenerateGeneralStatistics()
        {
            SetCustomersCount();
            SetFullAgedPersonsCount();
            SetChildrenCount();
            SetChildrenFemaleCount();
            SetChildrenMaleCount();
            SetSponsorsCount();
            SetTeamMembersCount();
            SetRevenues();
            SetExpenses();
            SetProfit();
        }

        private void SetCustomersCount()
        {
            int entireCustomers = 0;
            int activeCustomers = 0;
            //Families
            // Count all persons
            entireCustomers = Person.Count();
            // Count only Activated persons
            activeCustomers = Person.Count(CountStateType.Activated);

            tBEntireCustomers.Text = entireCustomers.ToString();
            tBActiveCustomers.Text = activeCustomers.ToString();
        }

        private void SetFullAgedPersonsCount()
        {
            int entireFullAgedPersons = Person.Count(CountStateType.All, true);
            int activeFullAgedPersons = Person.Count(CountStateType.Activated, true);

            tBEntireFullAgedPersons.Text = entireFullAgedPersons.ToString();
            tBActiveFullAgedPersons.Text = activeFullAgedPersons.ToString();
        }

        private void SetChildrenCount()
        {
            int entireChildren = 0;
            int activeChildren = 0;
            //Children
            entireChildren = Child.Count();
            activeChildren = Child.Count(CountChildGenderType.All, CountStateType.Activated);

            tBEntireChildren.Text = entireChildren.ToString();
            tBActiveChildren.Text = activeChildren.ToString();
        }

        private void SetChildrenFemaleCount()
        {
            int entireChildrenFemale = 0;
            int activeChildrenFemale = 0;
            //Children Female

            entireChildrenFemale = Child.Count(CountChildGenderType.Female);
            activeChildrenFemale = Child.Count(CountChildGenderType.Female, CountStateType.Activated);
            
            tBEntireChildrenFemale.Text = entireChildrenFemale.ToString();
            tBActiveChildrenFemale.Text = activeChildrenFemale.ToString();
        }

        private void SetChildrenMaleCount()
        {
            int entireChildrenMale = 0;
            int activeChildrenMale = 0;
            //Children Male
            
            entireChildrenMale = Child.Count(CountChildGenderType.Male);
            activeChildrenMale = Child.Count(CountChildGenderType.Male, CountStateType.Activated);

            tBEntireChildrenMale.Text = entireChildrenMale.ToString();
            tBActiveChildrenMale.Text = activeChildrenMale.ToString();
        }

        private void SetSponsorsCount()
        {
            int entireSponsors = 0;
            int activeSponsors = 0;
            //Sponsor
            entireSponsors = Sponsor.Count();
            activeSponsors = Sponsor.Count(CountStateType.Activated);
            
            tBEntireSponsors.Text = entireSponsors.ToString();
            tBActiveSponsors.Text = activeSponsors.ToString();
        }

        private void SetTeamMembersCount()
        {
            int entireTeamMembers = 0;
            int activeTeamMembers = 0;
            //Sponsor
            entireTeamMembers = Team.Count();
            activeTeamMembers = Team.Count(CountStateType.Activated);

            tBEntireTeamMembers.Text = entireTeamMembers.ToString();
            tBActiveTeamMembers.Text = activeTeamMembers.ToString();
        }

        private void SetRevenues()
        {
            //Revenues of this year and last year
            revenuesThisYear = Booking.GetAmountSum(YearType.Current);
            revenuesLastYear = Booking.GetAmountSum(YearType.Past);

            tBRevenuesThisYear.Text = revenuesThisYear.ToString();
            tBRevenuesLastYear.Text = revenuesLastYear.ToString(); 
        }

        private void SetExpenses()
        {
            //Expenses of this year and last year
            expensesThisYear = Booking.GetAmountSum(YearType.Current);
            expensesLastYear = Booking.GetAmountSum(YearType.Past);

            tBExpensesThisYear.Text = expensesThisYear.ToString();
            tBExpensesLastYear.Text = expensesLastYear.ToString();
        }

        private void SetProfit()
        {
            tBProfitThisYear.Text = (revenuesThisYear - expensesThisYear).ToString();
            tBProfitLastYear.Text = (revenuesLastYear - expensesLastYear).ToString();
        }
        #endregion

        private void fillSavedFilter() 
        { 
//TODO: Filter aus Datenbank
            cBSavedFilter.Items.Add("servus");
            cBSavedFilter.Items.Add("tschuessi");
        }
     
        private void removeWPDynamicContent()
        {
            //Remove dynamic generated content from Wrappanel
            foreach (UIElement item in wPDynamicContent)
            {
                wPFilter.Children.Remove(item);
            }
            wPDynamicContent = new ArrayList();
        }

        private DatePicker wPAddDatePicker()
        {
            DatePicker dP = new DatePicker
            {
                Height = generatedItemsHeight,
                Width = 160,
                Margin = new Thickness(10, 10, 10, 10)
            };
            wPFilter.Children.Add(dP);
            wPDynamicContent.Add(dP);

            return dP;
        }

        private ComboBox wPAddComboBox()
        {
            ComboBox cB = new ComboBox 
            { 
                Width = 120,
                Margin = new Thickness(10, 10, 10, 10)
            };            
            wPFilter.Children.Add(cB);
            wPDynamicContent.Add(cB);

            return cB;
        }

        private TextBox wPAddTextBox()
        {
            TextBox tB = new TextBox
            {
                Width = 200,
                Height = generatedItemsHeight,
                Margin = new Thickness(10, 10, 10, 10)
            };
            wPFilter.Children.Add(tB);
            wPDynamicContent.Add(tB);

            return tB;
        }


        /// <summary>
        /// //Method is setting up 2 Labels and 2 Comboboxes. Returning an array which contains the 2 Comboboxes
        /// </summary>
        private UIElement[] wPAddFromTo()
        {
            Thickness thickness10 = new Thickness(10, 10, 10, 10);

            //Create ComboBoxes
            ComboBox cbFrom = new ComboBox
            {
                Width = 50,
                Height = 27,
                Margin = thickness10
            };
            ComboBox cbTo = new ComboBox
            {
                Width = 50,
                Height = 27,
                Margin = thickness10
            };

            //Create Labels
            Label labelVon = new Label
            {
                Content = "von",
                Width = 35,
                Height = 27,
                Margin = thickness10,
                FontSize = fontSize
            };
            Label labelBis = new Label
            {
                Content = "bis",
                Width = 35,
                Height = 27,
                Margin = thickness10,
                FontSize = fontSize
            };

            //Add Labels and ComboBoxes to WrapPanel
            wPFilter.Children.Add(labelVon);
            wPFilter.Children.Add(cbFrom);
            wPFilter.Children.Add(labelBis);
            wPFilter.Children.Add(cbTo);

            //Add Labels and ComboBoxes to ArrayList for later remove
            wPDynamicContent.Add(cbFrom);
            wPDynamicContent.Add(labelVon);
            wPDynamicContent.Add(labelBis);
            wPDynamicContent.Add(cbTo);

            return new UIElement[]{ cbFrom, cbTo};
        }

        private void addItemsToComboBoxes(string[] items, ArrayList boxes)
        {
            foreach (ComboBox box in boxes)
            {
                foreach (string item in items)
                {
                    box.Items.Add(item);
                    box.IsEnabled = true;
                }
            }
        }

        private void filterChildren()
        {
            //Only the Children
            IEnumerable<Child> filteredChildren = Child.GetChildren();

            switch (cbStatus.Text)
            {
                case "Aktiv":
                    filteredChildren = filteredChildren.Where(c => c.Person.IsActive);
                    break;
                case "Inaktiv":
                    filteredChildren = filteredChildren.Where(c => c.Person.IsActive == false);
                    break;
                case "Alle":
                    //filteredChildren stays the same;
                    break;
            }

            //Filter the selected attribute
            switch (cbOther.Text)
            {
                case "Wohnort":
                    string city = ((TextBox)wPDynamicContent[0]).Text;
                    filteredChildren = filteredChildren.Where(c => c.Person.City == city);
                    break;
                case "Postleitzahl":
                    try
                    {
                        int zipCode = Convert.ToInt32(((TextBox)wPDynamicContent[0]).Text);
                        filteredChildren = filteredChildren.Where(c => c.Person.ZipCode == zipCode);
                    }
                    catch
                    {
                        MessageBox.Show("Bitte geben Sie ein korrekte Postleitzahl ein!", "Fehlerhafte Postleitzahl", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "Alter":
                    int age = Convert.ToInt32(((ComboBox)wPDynamicContent[0]).Text);
                    filteredChildren = filteredChildren.Where(c => c.Age == age);
                    break;
            }

            dGAnyStatistics.ItemsSource = filteredChildren;
        }

        private void filterPassHolder()
        {
            //Only Persons with an TableNo
            IEnumerable<Person> filteredPersons = Person.GetPersons().Where(p => p.TableNo.HasValue);

            switch (cbStatus.Text)
            {
                case "Aktiv":
                    filteredPersons = filteredPersons.Where(p => p.IsActive);
                    break;
                case "Inaktiv":
                    filteredPersons = filteredPersons.Where(p => p.IsActive == false);
                    break;
                case "Alle":
                    //filteredPersons stays the same;
                    break;
            }

            //Filter the selected attribute
            switch (cbOther.Text)
            {
                case "Wohnort":
                    string city = ((TextBox)wPDynamicContent[0]).Text;
                    filteredPersons = filteredPersons.Where(p => p.City == city);
                    break;
                case "Postleitzahl":
                    try
                    {
                        int zipCode = Convert.ToInt32(((TextBox)wPDynamicContent[0]).Text);
                        filteredPersons = filteredPersons.Where(p => p.ZipCode == zipCode);
                    }
                    catch
                    {
                        MessageBox.Show("Bitte geben Sie ein korrekte Postleitzahl ein!", "Fehlerhafte Postleitzahl", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;
                case "Staatsangehörigkeit":
                    string nationality = ((TextBox)wPDynamicContent[0]).Text;
                    filteredPersons = filteredPersons.Where(p => p.Nationality == nationality);
                    break;
                case "Alter":
                    int age = Convert.ToInt32(((ComboBox)wPDynamicContent[0]).Text);
                    filteredPersons = filteredPersons.Where(p => p.Age == age);
                    break;
                case "Ausweisgültigkeit bis max.":
                    try
                    {
                        DateTime? end = ((DatePicker)wPDynamicContent[0]).SelectedDate;
                        filteredPersons = filteredPersons.Where(p => p.ValidityEnd <= end);
                    }
                    catch
                    {
                        MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "common"));
                    }
                    break;
                case "Familienstand":
                    string state = ((ComboBox)wPDynamicContent[0]).Text;
                    filteredPersons = filteredPersons.Where(p => p.FamilyState.Name == state);
                    break;
                case "Anzahl Kinder":
                    int from = Convert.ToInt32(((ComboBox)wPDynamicContent[1]).Text);
                    int to = Convert.ToInt32(((ComboBox)wPDynamicContent[3]).Text);
                    filteredPersons = filteredPersons.Where(p => (p.NumberOfChildren >= from) && (p.NumberOfChildren <= to));
                    break;
            }

            dGAnyStatistics.ItemsSource = filteredPersons;
        }

        private void filterPartners()
        {
            //Only the marital partners
            IEnumerable<Person> filteredPartners = Person.GetPersons().Where(p => p.MaritalFirstName != null);

            switch (cbStatus.Text)
            {
                case "Aktiv":
                    filteredPartners = filteredPartners.Where(p => p.IsActive);
                    break;
                case "Inaktiv":
                    filteredPartners = filteredPartners.Where(p => p.IsActive == false);
                    break;
                case "Alle":
                    //filteredPartners stays the same;
                    break;
            }

            //Filter the selected attribute
            switch (cbOther.Text)
            {
                case "Wohnort":
                    string city = ((TextBox)wPDynamicContent[0]).Text;
                    filteredPartners = filteredPartners.Where(p => p.City == city);
                    break;
                case "Postleitzahl":
                    try {
                    int zipCode = Convert.ToInt32(((TextBox)wPDynamicContent[0]).Text);
                    filteredPartners = filteredPartners.Where(p => p.ZipCode == zipCode);
                    }
                    catch{
                        MessageBox.Show("Bitte geben Sie ein korrekte Postleitzahl ein!", "Fehlerhafte Postleitzahl", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }                    
                    break;
                case "Staatsangehörigkeit":
                    string nationality = ((TextBox)wPDynamicContent[0]).Text;
                    filteredPartners = filteredPartners.Where(p => p.Nationality == nationality);
                    break;
                case "Alter":                    
                        int age = Convert.ToInt32(((ComboBox)wPDynamicContent[0]).Text);
                        filteredPartners = filteredPartners.Where(p => p.Age == age);
                    break;
            }

            dGAnyStatistics.ItemsSource = filteredPartners;
        }

        private void filterSponsors()
        {
            //Only the sponsors
            IEnumerable<Sponsor> filteredSponsors = Sponsor.GetSponsors();

            switch (cbStatus.Text)
            {
                case "Aktiv":
                    filteredSponsors = filteredSponsors.Where(s => s.IsActive);
                    break;
                case "Inaktiv":
                    filteredSponsors = filteredSponsors.Where(s => s.IsActive == false);
                    break;
                case "Alle":
                    //filteredSponsors stays the same;
                    break;
            }

            string selectedFundingType = ((ComboBox)wPDynamicContent[0]).Text;
            switch (cbOther.Text)
            {
                case "Geschäftlich":
                    break;
                case "Privat":
                    break;
            }

            dGAnyStatistics.ItemsSource = filteredSponsors.Where(s => s.FundingType.Name == selectedFundingType);
        }

        private void setDataGridColumns()
        {
            dGAnyStatistics.AutoGenerateColumns = false;

            foreach (ComboBox box in displayedDataComboBoxes)
            {
                switch (box.Text)
                {
                    case "Vorname":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Vorname",
                            Binding = new Binding("FirstName")
                        });
                        break;
                    case "Nachname":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Nachname",
                            Binding = new Binding("LastName")
                        });
                        break;
                    case "Familienstand":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Familienstand",
                            Binding = new Binding("FamilyState.Name")
                        });
                        break;
                    case "Anzahl Kinder":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Anzahl Kinder",
                            Binding = new Binding("NumberOfPerson")
                        });
                        break;
                    case "Partner Name":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Partner Name",
                            Binding = new Binding("MaritalFirstName")
//TODO - Ganzer Name anzeigen
                        });
                        break;                        
                    case "Ausweisnummer":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Ausweisnummer",
                            Binding = new Binding("TableNo")
                        });
                        break;
                    case "Alter":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Alter",
                            Binding = new Binding("Age")
                        });
                        break;
                    case "Ausweisgültigkeit bis":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Ausweisgültigkeit bis",
                            Binding = new Binding("ValidityEnd")
                        });
                        break;
                    case "Postleitzahl":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Postleitzahl",
                            Binding = new Binding("ZipCode")
                        });
                        break;
                    case "Wohnort":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Wohnort",
                            Binding = new Binding("City")
                        });
                        break;
                    case "Adresse":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Adresse",
                            Binding = new Binding("ResidentialAddress")
                        });
                        break;
                    case "Name":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Name",
                            Binding = new Binding("FullName")
                        });
                        break;
                }
            }
        }

        #endregion

        #region Events

        #region AnyStatistics

        private void cbPerson_DropDownClosed(object sender, EventArgs e)
        {
            //Clear Comboboxes before refilling
            cbOther.Items.Clear();
            foreach (ComboBox cb in displayedDataComboBoxes)
            {
                cb.Items.Clear();
            }


            //Clear WrapPanel DynamicContent
            if (wPDynamicContent.Count != 0)
            {
                removeWPDynamicContent();
            }

            //Fill cbOther and update displayaedDataComboBoxes
            switch (cbPerson.Text)
            {
                case "Sponsoren":
                    ComboBox cBSponsor = wPAddComboBox();
                    cBSponsor.Width = 130;
                    foreach (FundingType type in FundingType.GetFundingTypes())
                    {
                        cBSponsor.Items.Add(type.Name);
                    }
                    addItemsToComboBoxes(displayableDataSponsor, displayedDataComboBoxes);
                    break;
                case "Ausweisinhaber":
                    foreach (string item in cbOtherItemsPassHolder)
                    {
                        cbOther.Items.Add(item);
                    }
                    cbOther.Width = 210;
                    addItemsToComboBoxes(displayableDataPassHolder, displayedDataComboBoxes);
                    break;
                case "Kinder":
                    foreach (string item in cbOtherItemsChildren)
                    {
                        cbOther.Items.Add(item);
                    }
                    cbOther.Width = 210;
                    addItemsToComboBoxes(displayableDataChildren, displayedDataComboBoxes);
                    break;
                default:
                    foreach (string item in cbOtherItemsPartnersAll)
                    {
                        cbOther.Items.Add(item);
                    }
                    cbOther.Width = 210;
                    addItemsToComboBoxes(displayableDataPartnersAll, displayedDataComboBoxes);
                    break;
            }
            if (cbPerson.Text != "")
            {
                cbOther.IsEnabled = true;
            }
        }

        private void cbOther_DropDownClosed(object sender, EventArgs e)
        {
            if (wPDynamicContent.Count != 0)
            {
                removeWPDynamicContent();
            }

            //Fill cbOther
            switch (cbOther.Text)
            {
                case "Ausweisgültigkeit bis max.":
                    wPAddDatePicker();
                    break;
                case "Familienstand":
                    ComboBox cBF = wPAddComboBox();
                    cBF.Width = 230;
                    foreach (FamilyState state in FamilyState.GetFamilyStates())
                    {
                        cBF.Items.Add(state.Name);
                    }
                    break;
                case "Anzahl Kinder":
                    UIElement[] array = wPAddFromTo();
                    ComboBox cBFrom = (ComboBox)array[0];
                    ComboBox cBTo = (ComboBox)array[1];
                    for (int i = 0; i < 20; i++)
                    {
                        cBFrom.Items.Add(i);
                        cBTo.Items.Add(i);
                    }
                    break;
                case "Postleitzahl":
                    tbInput = wPAddTextBox();
                    break;
                case "Wohnort":
                    tbInput = wPAddTextBox();
                    break;
                case "Staatsangehörigkeit":
                    tbInput = wPAddTextBox();
                    break;
                case "Alter":
                    ComboBox cBA = wPAddComboBox();
                    cBA.Width = 60;
                    for (int i = 1; i < 200; i++)
                    {
                        cBA.Items.Add(i);
                    }
                    break;
            }

        }

        private void bSearch_Click(object sender, RoutedEventArgs e)
        {
            //Clear DataGrid before refilling
            dGAnyStatistics.Columns.Clear();

            setDataGridColumns();

            switch (cbPerson.Text)
            {
                case "Ausweisinhaber":
                    filterPassHolder();
                    break;
                case "Kinder":
                    filterChildren();
                    break;
                case "Partner":
                    filterPartners();
                    break;
                /*case "Alle (ohne Sponsoren)":
                    filterAlle();
                    break;*/
                case "Sponsoren":
                    filterSponsors();
                    break;
            }
        }

        #endregion AnyStatistics

        #region SavedFilter

        private void cBSavedFilter_DropDownClosed(object sender, EventArgs e)
        {
            // Loeschen nur bei Statistiken die nicht für Serienbrief verwendet werden
            bLoadSavedFilter.IsEnabled = true;
            bDeleteSavedFilter.IsEnabled = true;
        }

        private void bLoadSavedFilter_Click(object sender, RoutedEventArgs e)
        {
            //Filter sollte geladen werden
            cBSavedFilter.Items.Add("Laden");
        }

        private void bDeleteSavedFilter_Click(object sender, RoutedEventArgs e)
        {
            //Filter sollte gelöscht werden
            cBSavedFilter.Items.RemoveAt(cBSavedFilter.SelectedIndex);
            bLoadSavedFilter.IsEnabled = false;
            bDeleteSavedFilter.IsEnabled = false;
        }

        #endregion SavedFilter

        #endregion

    }
}
