/**
 * Class: pSavedLists
 * @author Bjoern Bittner
 */

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
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.Printer;
using KöTaf.Utils.FileOperations;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Lists
{
    /// <summary>
    /// Interaktionslogik für pSavedLists.xaml
    /// </summary>
    public partial class pSavedLists : KPage
    {
        private List<String> displayableDataChildren;
        private List<String> displayableDataPartnersAll;
        private List<String> displayableDataPassHolder;
        private List<String> displayableDataSponsor;
        private List<String> displayableDataTeamMember;
        private readonly UserAccount _UserAccount;
        private ArrayList displayedDataComboBoxes = new ArrayList();
        private pDisplayedData displayedDataPage;

        #region Constructor

        public pSavedLists(UserAccount userAccount, List<String> displayableDataChildren, List<String> displayableDataPartnersAll,
            List<String> displayableDataPassHolder, List<String> displayableDataSponsor, List<String> displayableDataTeamMember)
        {
            this._UserAccount = userAccount;
            this.displayableDataChildren = displayableDataChildren;
            this.displayableDataPartnersAll = displayableDataPartnersAll;
            this.displayableDataPassHolder = displayableDataPassHolder;
            this.displayableDataSponsor = displayableDataSponsor;
            this.displayableDataTeamMember = displayableDataTeamMember;

            InitializeComponent();
           
            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            displayedDataPage = new pDisplayedData();
            fDisplayedData.Content = displayedDataPage;

            // Array mit Anzuzeigende Daten Boxen speichern
            displayedDataComboBoxes = displayedDataPage.getDisplayedDataComboBoxes();
        }

        public override void defineToolbarContent()
        {
            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                dGSavedFilter.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
        }

        /// <summary>
        /// Fuellt die Combobox "cBSavedFilter" mit allen Filtern aus der Datenbank die keinem Serienbrief zugeordnet sind.
        /// </summary>
        private void fillSavedFilter() 
        {
            // Zunaechst wird die Combobox geleert
            cBSavedFilter.Items.Clear();
            // Jedes FilterSet bei dem der Name leer ist wird hinzugefuegt (FilterSet ohne Namen gehoeren zu Serienbriefen)
            foreach (FilterSet filterSet in FilterSet.GetFilterSets().Where(p => p.Name != ""))
            {
                cBSavedFilter.Items.Add(filterSet.Name);
            }
        }

        /// <summary>
        /// Die Methode fuehrt die noetigen Schritte aus um eine Gruppe von Personen mit Hilfe eines FilterSets zu filtern
        /// und anschliessend im DataGrid anzuzeigen
        /// </summary>
        private void filterGroupWithFilterSet(IEnumerable<FilterSet> filterSets)
        {
            // Liste fuer die "geparsten" FilterSetModels
            List<FilterSetModel> filterSetModels = new List<FilterSetModel>();

            // Liste mit "geparsten" FilterSetModels befuellen indem jedes Element aus filterSets geparst wird
            foreach (var set in filterSets)
                filterSetModels.Add( FilterSetModelDB.getFilterSetModelFromFilterSet(set) );

            // Es werden jeweils Ein- und Ausgabelisten fuer Personen, Sponsoren und Teammitglieder angelegt
            IEnumerable<Person> unfilteredPersons = Person.GetPersons();
            IEnumerable<Sponsor> unfilteredSponsors = Sponsor.GetSponsors();
            IEnumerable<Team> unfilteredTeamMembers = Team.GetTeams();
            List<Person> filteredPersons = new List<Person>();
            List<Sponsor> filteredSponsors = new List<Sponsor>();
            List<Team> filteredTeamMembers = new List<Team>();

            // Es wird die Klasse FormletterFilterData verwendet um die Listen zu filtern
            FormletterFilterData fd = new FormletterFilterData();
            fd.filterRecordsByFilterSets(filterSetModels, unfilteredPersons, unfilteredSponsors, unfilteredTeamMembers, ref filteredPersons, ref filteredSponsors, ref filteredTeamMembers);
            
            // Je nachdem ob im Filter Kunde, Sponsor, Mitarbeiter ausgewaehlt ist wird die jeweilige Liste an das DataGrid uebergeben
            FilterSet filterSet = filterSets.ElementAt(0);
            IEnumerable<Filter> filters = Filter.GetFilters(null, filterSet.FilterSetID, null, null);
            switch (filters.ElementAt(0).Table)
            {
                case "Kunde":
                    dGSavedFilter.ItemsSource = filteredPersons;
                    break;
                case "Sponsor":
                    dGSavedFilter.ItemsSource = filteredSponsors;
                    break;
                case "Mitarbeiter":
                    dGSavedFilter.ItemsSource = filteredTeamMembers;
                    break;
            }
        }

        /// <summary>
        /// Die Methode setzt das DataGrid-Binding für die Elemente, die bei Anzuzeigende Daten ausgewaehlt wurden.
        /// </summary>
        private void setDataGridBindings()
        {
            dGSavedFilter.AutoGenerateColumns = false;

            foreach (ComboBox box in displayedDataComboBoxes)
            {
                switch (box.Text)
                {
                    case "Vorname":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Vorname",
                            Binding = new Binding("FirstName")
                        });
                        break;
                    case "Nachname":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Nachname",
                            Binding = new Binding("LastName")
                        });
                        break;
                    case "Familienstand":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Familienstand",
                            Binding = new Binding("FamilyState.Name")
                        });
                        break;
                    case "Anzahl Kinder":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Anzahl Kinder",
                            Binding = new Binding("NumberOfPerson")
                        });
                        break;
                    case "Partner-Name":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Partner-Name",
                            Binding = new Binding("MaritalFullname")
                        });
                        break;
                    case "Staatsangehörigkeit":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                       {
                           Header = "Staatsangehörigkeit",
                           Binding = new Binding("Nationality")
                       });
                       break;
                    case "Ausweisnummer":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Ausweisnummer",
                            Binding = new Binding("TableNo")
                        });
                        break;
                    case "Alter":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Alter",
                            Binding = new Binding("Age")
                        });
                        break;
                    case "Geburtsdatum":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Geburtsdatum",
                            Binding = new Binding("DateOfBirth")
                        });
                        break;
                    case "Ausweisgültigkeit bis":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Ausweisgültigkeit bis",
                            Binding = new Binding("ValidityEnd")

                        });
                        break;
                    case "Postleitzahl":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Postleitzahl",
                            Binding = new Binding("ZipCode")
                        });
                        break;
                    case "Wohnort":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Wohnort",
                            Binding = new Binding("City")
                        });
                        break;
                    case "Adresse":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Adresse",
                            Binding = new Binding("ResidentialAddress")
                        });
                        break;
                    case "Name":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Name",
                            Binding = new Binding("FullName")
                        });
                        break;
                    case "Firmenname":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Firmenname",
                            Binding = new Binding("CompanyName")
                        });
                        break;
                    case "Letzter Einkauf":
                        dGSavedFilter.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Letzter Einkauf",
                            Binding = new Binding("LastPurchase")
                        });
                        break;
                }
            }
        }

        /// <summary>
        /// Die Methode setzt das DataGrid zurueck, ruft die Filtermethode auf und zaehlt anschliessend die gefunden Personen
        /// </summary>
        private void search(IEnumerable<FilterSet> filterSets)
        {
            // Setzt das DataGrid und die Bindings des DataGrid zurueck
            dGSavedFilter.Columns.Clear();
            setDataGridBindings();

            // Ruft die Filtermethode auf
            filterGroupWithFilterSet(filterSets);

            // Anzahl gefundener Personen ausgeben (-1 da liste ein Datensatz mehr hat) und Druckenbutton aktivieren/deaktivieren
            int count = dGSavedFilter.Items.Count - 1;
            if (count == 1)
            {
                lCount.Content = "1 Person gefunden";
                bPrint.IsEnabled = true;
            }
            else if (count == 0)
            {
                lCount.Content = count + " Personen gefunden";
                bPrint.IsEnabled = false;
            }
            else
            {
                lCount.Content = count + " Personen gefunden";
                bPrint.IsEnabled = true;
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Das Event füllt die ComboBox mit gespeicherten Filtern sobald diese geoeffnet wird
        /// </summary>
        private void cBSavedFilter_DropDownOpened(object sender, EventArgs e)
        {
            fillSavedFilter();
        }

        /// <summary>
        /// Das Event aktiviert/deaktiviert verschiedene Buttons und sucht sofort nach Datensaetzen sobald ein Filter ausgewaehlt wurde
        /// </summary>
        private void cBSavedFilter_DropDownClosed(object sender, EventArgs e)
        {
            // Cursor auf arbeitenden Kreis setzen, um zu zeigen dass das Programm was macht
            Cursor = Cursors.Wait;

            // Suchen-, Drucken-, Loeschenbutton deaktivieren wenn kein Filter ausgewaehlt wurde
            if (cBSavedFilter.Text == "")
            {
                bSearchSavedFilter.IsEnabled = false;
                bDeleteSavedFilter.IsEnabled = false;
                bPrint.IsEnabled = false;
                displayedDataPage.resetAll();
                lCount.Content = "";
                dGSavedFilter.ItemsSource = null;
                dGSavedFilter.Columns.Clear();
            }
            // Suchen-, Loeschenbutton aktivieren wenn ein Filter ausgewaehlt wurde und automatisch suchen mit Defaultwerten
            else
            {
                bSearchSavedFilter.IsEnabled = true;
                bDeleteSavedFilter.IsEnabled = true;
                displayedDataPage.resetAll();

                // Die ersten beiden Comboboxen werden mit den Defaultwerten belegt
                ((ComboBox)displayedDataComboBoxes[0]).Items.Add("Name");
                ((ComboBox)displayedDataComboBoxes[0]).SelectedItem = "Name";
                ((ComboBox)displayedDataComboBoxes[0]).IsEnabled = true;
                ((ComboBox)displayedDataComboBoxes[1]).Items.Add("Adresse");
                ((ComboBox)displayedDataComboBoxes[1]).SelectedItem = "Adresse";
                ((ComboBox)displayedDataComboBoxes[1]).IsEnabled = true;
                ((ComboBox)displayedDataComboBoxes[1]).Visibility = Visibility.Visible; 
                ((ComboBox)displayedDataComboBoxes[2]).IsEnabled = true;
                ((ComboBox)displayedDataComboBoxes[2]).Visibility = Visibility.Visible;

                // Comboboxen für Anzuzeigende Daten fuellen, je nachdem ob die Filter mit Kunde, Sponsor, Mitarbeiter ist
                IEnumerable<FilterSet> filterSets = FilterSet.GetFilterSets(null, null, cBSavedFilter.Text);
                FilterSet filterSet = filterSets.ElementAt(0);
                IEnumerable<Filter> filters = Filter.GetFilters(null, filterSet.FilterSetID, null, null);
                switch (filters.ElementAt(0).Table)
                {
                    case "Kunde":
                        displayedDataPage.setDisplayableData(displayableDataPassHolder);
                        break;
                    case "Sponsor":
                        displayedDataPage.setDisplayableData(displayableDataSponsor);
                        break;
                    case "Mitarbeiter":
                        displayedDataPage.setDisplayableData(displayableDataTeamMember);
                        break; 
                }
                search(filterSets);
            }

            //Cursor zuruecksetzen
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Das Event loest eine Suche nach Datensaetzen aus und das DataGrid wird an Anzuzeigende Daten angepasst
        /// </summary>
        private void bSearchSavedFilter_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            IEnumerable<FilterSet> filterSets = FilterSet.GetFilterSets(null, null, cBSavedFilter.Text);
            search(filterSets);
            Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Das Event loescht den ausgewaehlten Filter nach einer erneuten bestaetigung
        /// </summary>
        private void bDeleteSavedFilter_Click(object sender, RoutedEventArgs e)
        {
            // Bestaetigung abfragen
            var result = MessageBoxEnhanced.Question(IniParser.GetSetting("LISTS", "deleteFilterSet"));
            if (result == MessageBoxResult.No)
            {
                return;
            }

            // Cursor auf arbeitenden Kreis setzen, um zu zeigen dass das Programm was macht
            Cursor = Cursors.Wait;

            // Zu loeschendes FilterSet aus DB holen
            IEnumerable<FilterSet> filterSets = FilterSet.GetFilterSets(null, null, cBSavedFilter.Text);

            // Alle Filter für das jeweilige FilterSet und anschließend FilterSet loeschen            
            foreach (FilterSet filterSet in filterSets) 
            {
                IEnumerable<Filter> filters = Filter.GetFilters(null, filterSet.FilterSetID, null, null);
                foreach (Filter filter in filters)
                {
                    Filter.Delete(filter.FilterID);
                }
                FilterSet.Delete(filterSet.FilterSetID);
            }

            //ComboBox mit Filtern neu befuellen, da einer geloescht wurde.
            fillSavedFilter();

            // Drucken-, Suche-, Loeschenbutton deaktivieren
            bSearchSavedFilter.IsEnabled = false;
            bDeleteSavedFilter.IsEnabled = false;
            bPrint.IsEnabled = false;

            // Alle Boxen für Anzuzeigende Daten reseten
            displayedDataPage.resetAll();

            // DataGrid wird resetet da Filter geloescht wurde
            lCount.Content = "";
            dGSavedFilter.ItemsSource = null;
            dGSavedFilter.Columns.Clear();

            //Cursor zuruecksetzen
            Cursor = Cursors.Arrow;
        }

        private void bPrint_Click(object sender, RoutedEventArgs e)
        {

            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
                return;
            }

            var keyValueList = dGSavedFilter.ToKeyValueList();
            try
            {
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Statistic, keyValueList);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
        }

        #endregion

    }
}