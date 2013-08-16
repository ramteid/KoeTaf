/**
 * Class: pAnyLists
 * @author Lucas Kögel
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
using KöTaf.Utils.Printer;
using KöTaf.Utils.FileOperations;
using KöTaf.Utils.Parser;



namespace KöTaf.WPFApplication.Views.Lists
{
    /// <summary>
    /// Page for Any Lists
    /// </summary>
    public partial class pAnyLists : KPage
    {
        private List<String> attributesPassHolder;
        private List<String> attributesPartnersAll;
        private List<String> attributesChildren;
        private List<String> displayableDataChildren;
        private List<String> displayableDataPartnersAll;
        private List<String> displayableDataPassHolder;
        private List<String> displayableDataSponsor;
        private ArrayList wPDynamicContent = new ArrayList();
        private readonly UserAccount _UserAccount;
        private ArrayList displayedDataComboBoxes = new ArrayList();
        private pDisplayedData displayedDataPage;
       

        #region Constructor

        public pAnyLists(UserAccount userAccount, List<String> displayableDataChildren, List<String> displayableDataPartnersAll,
            List<String> displayableDataPassHolder, List<String> displayableDataSponsor, List<String> attributesPassHolder,
            List<String> attributesPartnersAll, List<String> attributesChildren)
        {
            this._UserAccount = userAccount;
            this.displayableDataChildren = displayableDataChildren;
            this.displayableDataPartnersAll = displayableDataPartnersAll;
            this.displayableDataPassHolder = displayableDataPassHolder;
            this.displayableDataSponsor = displayableDataSponsor;
            this.attributesPassHolder = attributesPassHolder;
            this.attributesPartnersAll = attributesPartnersAll;
            this.attributesChildren = attributesChildren;

            InitializeComponent();

            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            displayedDataPage = new pDisplayedData();
            fDisplayedData.Content = displayedDataPage;

            //Save Displayed-Data-Combo-Boxes in an array
            displayedDataComboBoxes = displayedDataPage.getDisplayedDataComboBoxes();
            

        }

        public override void defineToolbarContent()
        {
            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                dGAnyStatistics.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
        }
     
        /// <summary>
        /// Löscht die zur Laufzeit generierten Komponenten und setzt damit die GUI,
        /// auf den Stand des Startes, zurück.
        /// </summary>
        private void removeWPDynamicContent()
        {
            //Remove dynamic generated content from Wrappanel
            foreach (UIElement item in wPDynamicContent)
            {
                wPFilter.Children.Remove(item);
            }
            wPDynamicContent = new ArrayList();
        }

        /// <summary>
        /// Erstellt zur Laufzeit einen DatePicker und fügt ihn dem Filter-Wrap-Panel zu.
        /// Dieser wird in der Liste wPDynamicContent gespeichert, um später wieder gelöscht werden zu können.
        /// </summary>
        /// <returns> Der erzeugte DatePicker</returns>
        private DatePicker wPAddDatePicker()
        {
            DatePicker dP = new DatePicker
            {
                Width = 130,
                Margin = new Thickness(10, 10, 10, 10),
                SelectedDate = DateTime.Today
            };
            wPFilter.Children.Add(dP);
            wPDynamicContent.Add(dP);

            return dP;
        }

        /// <summary>
        /// Erstellt zur Laufzeit eine ComboBox und fügt sie dem Filter-Wrap-Panel zu.
        /// Diese wird in der Liste wPDynamicContent gespeichert, um später wieder gelöscht werden zu können.
        /// </summary>
        /// <returns> Die erzeugte ComboBox</returns>
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

        /// <summary>
        /// Erstellt zur Laufzeit eine TextBox und fügt sie dem Filter-Wrap-Panel zu.
        /// Diese wird in der Liste wPDynamicContent gespeichert, um später wieder gelöscht werden zu können.
        /// </summary>
        /// <returns> Die erzeugte TextBox</returns>
        private TextBox wPAddTextBox()
        {
            TextBox tB = new TextBox
            {
                Width = 200,
                Margin = new Thickness(10, 10, 10, 10)
            };
            wPFilter.Children.Add(tB);
            wPDynamicContent.Add(tB);

            return tB;
        }


        /// <summary>
        /// Erstellt 2 Label und 2 ComboBoxen und fügt sie dem Filter-Wrap-Panel zu.
        /// Von [] Bis []
        /// </summary>
        /// <returns> Ein Array mit den beiden erzeugten ComboBoxen</returns>
        private UIElement[] wPAddFromTo()
        {
            Thickness thickness = new Thickness(5, 7, 5, 5);

            //Create ComboBoxes
            ComboBox cbFrom = new ComboBox
            {
                Width = 50,
                Margin = thickness
            };
            ComboBox cbTo = new ComboBox
            {
                Width = 50,
                Margin = thickness
            };

            //Create Labels
            Label labelFrom = new Label
            {
                Content = "von",
                Width = 35,
                Margin = thickness,
            };
            Label labelTo = new Label
            {
                Content = "bis",
                Width = 35,
                Margin = thickness,
            };

            //Add Labels and ComboBoxes to WrapPanel
            wPFilter.Children.Add(labelFrom);
            wPFilter.Children.Add(cbFrom);
            wPFilter.Children.Add(labelTo);
            wPFilter.Children.Add(cbTo);

            //Add Labels and ComboBoxes to ArrayList for later remove
            wPDynamicContent.Add(labelFrom);
            wPDynamicContent.Add(cbFrom);
            wPDynamicContent.Add(labelTo);
            wPDynamicContent.Add(cbTo);

            return new UIElement[]{ cbFrom, cbTo};
        }

        /// <summary>
        /// Erstellt 2 Label und 2 DatePicker und fügt sie dem Filter-Wrap-Panel zu.
        /// Von [] Bis []
        /// </summary>
        /// <returns> Ein Array mit den beiden erzeugten DatePicker</returns>
        private UIElement[] wPAddFromToDate()
        {
            Thickness thickness = new Thickness(5, 7, 5, 5);

            //Create ComboBoxes
            DatePicker dpFrom = new DatePicker
            {
                Margin = thickness,
                Width = 130,                
                SelectedDate = DateTime.Today.AddDays(-1)
            };
            DatePicker dpTo = new DatePicker
            {
                Margin = thickness,
                Width = 130,
                SelectedDate = DateTime.Today
            };

            //Create Labels
            Label labelFrom = new Label
            {
                Content = "von",
                Width = 35,
                Margin = thickness,
            };
            Label labelTo = new Label
            {
                Content = "bis",
                Width = 35,
                Margin = thickness,
            };

            //Add Labels and ComboBoxes to WrapPanel
            wPFilter.Children.Add(labelFrom);
            wPFilter.Children.Add(dpFrom);
            wPFilter.Children.Add(labelTo);
            wPFilter.Children.Add(dpTo);

            //Add Labels and ComboBoxes to ArrayList for later remove
            wPDynamicContent.Add(labelFrom);
            wPDynamicContent.Add(dpFrom);
            wPDynamicContent.Add(labelTo);
            wPDynamicContent.Add(dpTo);

            return new UIElement[] { dpFrom, dpTo };
        }

        /// <summary>
        /// Methode welche für das Filtern der Kinder verantwortlich ist.
        /// Diese filtert, je nach ausgewählten Attributen, die Kinder und fügt sie dem DataGrid zu.
        /// </summary>
        private void filterChildren()
        {
            //Nur Kinder holen
            IEnumerable<Child> filteredChildren = Child.GetChildren();

            //Nach Status filtern
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

            //Nach dem ausgewähltem Attribut filtern.
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
                    string fromAgeString = ((ComboBox)wPDynamicContent[1]).Text;
                    string toAgeString = ((ComboBox)wPDynamicContent[3]).Text;
                    if (fromAgeString == "" || toAgeString == "")
                    {
                        MessageBox.Show("Bitte wählen Sie ein Alter aus!", "Fehlendes Alter", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    int fromAge = Convert.ToInt32(fromAgeString);
                    int toAge = Convert.ToInt32(toAgeString);
                    filteredChildren = filteredChildren.Where(p => (p.Age >= fromAge) && (p.Age <= toAge));
                    break;
            }
            //Ergebnis dem DataGrid zuführen.
            dGAnyStatistics.ItemsSource = filteredChildren;
        }

        /// <summary>
        /// Methode welche für das Filtern der Ausweisinhaber verantwortlich ist.
        /// Diese filtert, je nach ausgewählten Attributen, die Ausweisinhaber und fügt sie dem DataGrid zu.
        /// </summary>
        private void filterPassHolder()
        {
            //Alle Ausweisinhaber holen.
            IEnumerable<Person> filteredPersons = Person.GetPersons();

            //Nach Status filtern.
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

            //Nach dem ausgewähltem Attribut filtern.
            switch (cbOther.Text)
            {
                case "Wohnort":
                    string city = ((TextBox)wPDynamicContent[0]).Text;
                    filteredPersons = filteredPersons.Where(p => p.City == city);
                    break;
                case "Postleitzahl":
                    // FormatException abfangen
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
                    string fromString = ((ComboBox)wPDynamicContent[1]).Text;
                    string toString = ((ComboBox)wPDynamicContent[3]).Text;
                    //Falls nichts ausgewählt wurde -> Fehler zeigen.
                    if (fromString == "" || toString == "")
                    {
                        MessageBox.Show("Bitte wählen Sie ein Alter aus!", "Fehlendes Alter", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    int from = Convert.ToInt32(fromString);
                    int to = Convert.ToInt32(toString);
                    filteredPersons = filteredPersons.Where(p => (p.Age >= from) && (p.Age <= to));
                    break;
                case "Ausweisgültigkeit bis max.":
                    try
                    {
                        DateTime? end = ((DatePicker)wPDynamicContent[0]).SelectedDate;
                        filteredPersons = filteredPersons.Where(p => p.ValidityEnd.CompareTo(end) <= 0);
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
                    string fromAgeString = ((ComboBox)wPDynamicContent[1]).Text;//wPDynamicContent[0] und ...[2] sind die Label "von" und "bis"
                    string toAgeString = ((ComboBox)wPDynamicContent[3]).Text;
                    //Falls nichts ausgewählt wurde -> Fehler zeigen.
                    if (fromAgeString == "" || toAgeString == "")
                    {
                        MessageBox.Show("Bitte wählen Sie eine Anzahl aus!", "Fehlende Anzahl", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    int fromAge = Convert.ToInt32(fromAgeString);
                    int toAge = Convert.ToInt32(toAgeString);
                    filteredPersons = filteredPersons.Where(p => (p.NumberOfChildren >= fromAge) && (p.NumberOfChildren <= toAge));
                    break;
                case "Letzter Einkauf":
                    DateTime? fromDate = ((DatePicker)wPDynamicContent[1]).SelectedDate;
                    DateTime? toDate = ((DatePicker)wPDynamicContent[3]).SelectedDate;
                    toDate = BookingsHelper.makeDateGreat(toDate.Value);
                    filteredPersons = filteredPersons.Where(p => (p.LastPurchase.GetValueOrDefault().CompareTo(fromDate) >= 0) && (p.LastPurchase.GetValueOrDefault().CompareTo(toDate) <= 0));
                    break;
            }

            //Ergebnis dem DataGrid zuführen.
            dGAnyStatistics.ItemsSource = filteredPersons;
        }

        /// <summary>
        /// Methode welche für das Filtern der Partner verantwortlich ist.
        /// Diese filtert, je nach ausgewählten Attributen, die Partner und fügt sie dem DataGrid zu.
        /// </summary>
        private void filterPartners()
        {
            //Nur die Partner holen.
            IEnumerable<Person> filteredPartners = Person.GetPersons().Where(p => p.MaritalFirstName != null);

            //Nach Status filtern.
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

            //Nach ausgewähltem Attribut filtern
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
                    filteredPartners = filteredPartners.Where(p => p.MaritalNationality == nationality);
                    break;
                case "Alter":
                    string fromAgeString = ((ComboBox)wPDynamicContent[1]).Text;
                    string toAgeString = ((ComboBox)wPDynamicContent[3]).Text;
                    if (fromAgeString == "" || toAgeString == "")
                    {
                        MessageBox.Show("Bitte wählen Sie eine Anzahl aus!", "Fehlende Anzahl", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    int fromAge = Convert.ToInt32(fromAgeString);
                    int toAge = Convert.ToInt32(toAgeString);
                    filteredPartners = filteredPartners.Where(p => (p.NumberOfChildren >= fromAge) && (p.NumberOfChildren <= toAge));
                    break;
            }
            //Ergebnis dem DataGrid zuführen.
            dGAnyStatistics.ItemsSource = filteredPartners;
        }

        /// <summary>
        /// Methode welche für das Filtern der Sponsoren verantwortlich ist.
        /// Diese filtert, je nach ausgewählten Attributen, die Sponsoren und fügt sie dem DataGrid zu.
        /// </summary>
        private void filterSponsors()
        {
            //Nur die Sponsoren holen
            IEnumerable<Sponsor> filteredSponsors = Sponsor.GetSponsors();

            //Nach Status filtern
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

            //Nach ausgewähltem Sponsoren-Typ filtern.
            string selectedFundingType = cbOther.Text;
 
            //Ergebnis dem DataGrid zuführen.
            dGAnyStatistics.ItemsSource = filteredSponsors.Where(s => s.FundingType.Name == selectedFundingType);
        }


        /// <summary>
        /// Die Funktion setzt das DataGrid-Binding für die Elemente, die bei
        /// Anzuzeigende Daten ausgewählt wurden.
        /// </summary>
        private void setDataGridBindings()
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
                    case "Partner-Name":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Partner-Name",
                            Binding = new Binding("MaritalFullname")
                        });
                        break;
                    case "Firmenname":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Firmenname",
                            Binding = new Binding("CompanyName")
                        });
                        break;
                    case "Letzter Einkauf":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Letzter Einkauf",
                            Binding = new Binding("LastPurchase")
                        });
                        break;
                    case "Ausweisnummer":
                        //Kinder brauchen das Binding auf das Attribut ihrer Eltern,
                        //da sie es selbst nicht in der DB besitzen.
                        if (cbPerson.Text == "Kinder")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Ausweisnummer",
                                Binding = new Binding("Person.TableNo")
                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Ausweisnummer",
                                Binding = new Binding("TableNo")
                            });
                        }

                        break;
                    case "Alter":
                        dGAnyStatistics.Columns.Add(new DataGridTextColumn
                        {
                            Header = "Alter",
                            Binding = new Binding("Age")
                        });
                        break;
                    case "Geburtsdatum":
                        //Partner braucht ein anderes Binding
                        if (cbPerson.Text == "Partner")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Geburtsdatum",
                                Binding = new Binding("MaritalBirthday")
                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Geburtsdatum",
                                Binding = new Binding("DateOfBirth")
                            });
                        }
                        break;
                    case "Staatsangehörigkeit":
                        //Partner braucht ein anderes Binding
                        if (cbPerson.Text == "Partner")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Staatsangehörigkeit",
                                Binding = new Binding("MaritalNationality")
                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Staatsangehörigkeit",
                                Binding = new Binding("Nationality")
                            });
                        }
                        break;
                    case "Ausweisgültigkeit bis":
                        //Kinder brauchen das Binding auf das Attribut ihrer Eltern,
                        //da sie es selbst nicht in der DB besitzen.
                        if (cbPerson.Text == "Kinder")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Ausweisgültigkeit bis",
                                Binding = new Binding("Person.ValidityEnd")

                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Ausweisgültigkeit bis",
                                Binding = new Binding("ValidityEnd")

                            });
                        }
                        break;
                    case "Postleitzahl":
                        //Kinder brauchen das Binding auf das Attribut ihrer Eltern,
                        //da sie es selbst nicht in der DB besitzen.
                        if (cbPerson.Text == "Kinder")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Postleitzahl",
                                Binding = new Binding("Person.ZipCode")
                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Postleitzahl",
                                Binding = new Binding("ZipCode")
                            });
                        }
                        break;
                    case "Wohnort":
                        //Kinder brauchen das Binding auf das Attribut ihrer Eltern,
                        //da sie es selbst nicht in der DB besitzen.
                        if (cbPerson.Text == "Kinder")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Wohnort",
                                Binding = new Binding("Person.City")
                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Wohnort",
                                Binding = new Binding("City")
                            });
                        }
                        break;
                    case "Adresse":
                        //Kinder brauchen das Binding auf das Attribut ihrer Eltern,
                        //da sie es selbst nicht in der DB besitzen.
                        if (cbPerson.Text == "Kinder")
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Adresse",
                                Binding = new Binding("Person.ResidentialAddress")
                            });
                        }
                        else
                        {
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Adresse",
                                Binding = new Binding("ResidentialAddress")
                            });
                        }
                        break;
                    case "Name":
                        //Partner braucht ein anderes Binding
                        if (cbPerson.Text == "Partner"){
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Name",
                                Binding = new Binding("MaritalFullname")
                            });
                        }else{
                            dGAnyStatistics.Columns.Add(new DataGridTextColumn
                            {
                                Header = "Name",
                                Binding = new Binding("FullName")
                            });
                        }
                        break;                        
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Wird aufgerufen wenn eine Personen-Art (Kinder, Sponsor...) ausgewählt wurde.
        /// </summary>
        /// <param name="sender">Die geschlossene ComboBox</param>
        private void cbPerson_DropDownClosed(object sender, EventArgs e)
        {
            //Reset GUI
            lCount.Content = "";
            bSearch.IsEnabled = false;
            bPrint.IsEnabled = false;
            displayedDataPage.resetAll();
            dGAnyStatistics.ItemsSource = null;
            

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
                    foreach (FundingType type in FundingType.GetFundingTypes())
                    {
                        cbOther.Items.Add(type.Name);
                    }
                    displayedDataPage.setDisplayableData(displayableDataSponsor);
                    break;
                case "Ausweisinhaber":
                    foreach (string item in attributesPassHolder)
                    {
                        cbOther.Items.Add(item);
                    }
                    cbOther.Width = 210;
                    displayedDataPage.setDisplayableData(displayableDataPassHolder);
                    break;
                case "Kinder":
                    foreach (string item in attributesChildren)
                    {
                        cbOther.Items.Add(item);
                    }
                    cbOther.Width = 210;
                    displayedDataPage.setDisplayableData(displayableDataChildren);
                    break;
                default:
                    foreach (string item in attributesPartnersAll)
                    {
                        cbOther.Items.Add(item);
                    }
                    cbOther.Width = 210;
                    displayedDataPage.setDisplayableData(displayableDataPartnersAll);
                    break;
            }
            if (!String.IsNullOrEmpty(cbPerson.Text))
            {
                cbOther.IsEnabled = true;
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn ein Attribut, nach dem gefiltert werden soll, ausgewählt wurde.
        /// </summary>
        /// <param name="sender">Die geschlossene ComboBox</param>
        private void cbOther_DropDownClosed(object sender, EventArgs e)
        {
            //Reset GUI
            lCount.Content = "";
            bPrint.IsEnabled = false;
            bSearch.IsEnabled = true;
            displayedDataPage.resetAll();
            dGAnyStatistics.ItemsSource = null;
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
                    wPAddTextBox();
                    break;
                case "Wohnort":
                    wPAddTextBox();
                    break;
                case "Staatsangehörigkeit":
                    wPAddTextBox();
                    break;
                case "Alter":
                    UIElement[] arrayAge = wPAddFromTo();
                    ComboBox cBFromAge = (ComboBox)arrayAge[0];
                    ComboBox cBToAge = (ComboBox)arrayAge[1];
                    for (int i = 0; i < 150; i++)
                    {
                        cBFromAge.Items.Add(i);
                        cBToAge.Items.Add(i);
                    }
                    break;
                case "Letzter Einkauf":
                    wPAddFromToDate();
                    break;
            }
            if (cbOther.Text != "")
            {
                ((ComboBox)displayedDataComboBoxes[0]).IsEnabled = true;
                bPrint.IsEnabled = false;
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn auf den "Suchen"-Button geklickt wurde.
        /// </summary>
        private void bSearch_Click(object sender, RoutedEventArgs e)
        {
            //Drucken ermöglichen
            bPrint.IsEnabled = true;

            //DataGrid leeren bevor es wiederbefuellt wird
            dGAnyStatistics.Columns.Clear();

            //DataGrid-Bindings erstellen.
            setDataGridBindings();

            //Je nach Personen-Art die zuständige Methode aufrufen.
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
                case "Sponsoren":
                    filterSponsors();
                    break;
            }

            //Anzahl gefundener Personen ausgeben
            int count = dGAnyStatistics.Items.Count;
            if (count == 1)
            {
                lCount.Content = "1 Person gefunden";
            }
            else
            {
                lCount.Content = count + " Personen gefunden";
            }
        }

        /// <summary>
        /// Wird aufgerufen wenn auf "Drucken"-Button geklickt wurde.
        /// </summary>
        private void bPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
                return;
            }

            var keyValueList = dGAnyStatistics.ToKeyValueList();
            try
            {
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Statistic, keyValueList);
            }
            catch (Exception ex) {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
            }

        #endregion
    }
}
