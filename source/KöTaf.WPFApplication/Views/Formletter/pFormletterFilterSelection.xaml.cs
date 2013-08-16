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
using KöTaf.WPFApplication.Models;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;
using KöTaf.DataModel;

namespace KöTaf.WPFApplication.Views.Formletter
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Filter-Auswahl für Serienbriefe und Listen
    /// </summary>
    public partial class pFormletterFilterSelection : KPage
    {
        private FilterSetModel currentFilterSet;
        public List<FilterSetModel> allFilterSets { get; private set; }
        public bool useForListModule { get; private set; }

        public pFormletterFilterSelection(bool useForListModule = false)
        {
            allFilterSets = new List<FilterSetModel>();
            InitializeComponent();
            listBox1.IsEnabled = false;
            btDeleteButton.IsEnabled = false;
            saveFilterSet.IsEnabled = false;

            this.useForListModule = useForListModule;
            if (useForListModule)
            {
                lbName.Visibility = Visibility.Visible;
                tbName.Visibility = Visibility.Visible;
                filterSets.Visibility = Visibility.Collapsed;
            }
            else
            {
                lbName.Visibility = Visibility.Collapsed;
                tbName.Visibility = Visibility.Collapsed;
                filterSets.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
        }

        /// <summary>
        /// UND-Verknüpftes FilterSet initalisieren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilterSetAND(object sender, RoutedEventArgs e)
        {
            if (this.useForListModule)
                if (string.IsNullOrEmpty(tbName.Text))
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("FILTER", "errorFilter"));
                    return;
                }
            currentFilterSet = new FilterSetModel(IniParser.GetSetting("FILTER", "defaultAndString"));

            if (this.useForListModule)
                currentFilterSet.name = tbName.Text;

            clearForm();
            activateForm();
            lbLinking.Content = IniParser.GetSetting("FILTER", "defaultAndString");
        }

        /// <summary>
        /// ODER-Verknüpftes FilterSet initialisieren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilterSetOR(object sender, RoutedEventArgs e)
        {
            if (this.useForListModule)
                if (string.IsNullOrEmpty(tbName.Text))
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("FILTER", "errorFilterSet"));
                    return;
                }

            currentFilterSet = new FilterSetModel(IniParser.GetSetting("FILTER", "defaultOrString"));

            if (this.useForListModule)
                currentFilterSet.name = tbName.Text;

            clearForm();
            activateForm();
            lbLinking.Content = IniParser.GetSetting("FILTER", "defaultOrString");
        }

        /// <summary>
        /// Filter hinzufügen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFilter_Click(object sender, RoutedEventArgs e)
        {
            if ((cbGroup.SelectionBoxItem.ToString() == "" ||
                 cbCriterion.SelectionBoxItem.ToString() == "" ||
                 cbOperation.SelectionBoxItem.ToString() == "" ||
                 //tbValue.Text.ToString() == "" || // Text darf leer sein für z.B. Phone != ""
                 false )) 
            {
                return;
            }
            listBox1.IsEnabled = true;
            saveFilterSet.IsEnabled = true;

            FilterModel filter = new FilterModel();
            filter.group = (FilterModel.Groups)((cbGroup.SelectedItem as ComboBoxItems).enumVal);
            filter.criterion = (FilterModel.Criterions)((cbCriterion.SelectedItem as ComboBoxItems).enumVal);
            filter.operation = (FilterModel.Operations)((cbOperation.SelectedItem as ComboBoxItems).enumVal);
            filter.value = tbValue.Text;

            if (!checkForDuplicateFilters(filter))
            {
                listBox1.Items.Add(filter);
                btDeleteButton.IsEnabled = true;
                tbValue.Text = "";
            }
        }
        
        /// <summary>
        /// Prüfe ob ein gleicher Filter bereits existiert
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private bool checkForDuplicateFilters(FilterModel filter) {
            foreach (var subitem in listBox1.Items)
            {
                if (subitem.ToString() == filter.ToString())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Filter aus Filter-Liste löschen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFromFilterList_Click(object sender, RoutedEventArgs e)
        {
            FilterModel filter = listBox1.SelectedItem as FilterModel;

            if (filter != null)
            {
                listBox1.Items.Remove(filter);
                currentFilterSet.filterList.Remove(filter);
                listBox2.Items.Refresh();
            }
            if (listBox1.Items.Count == 0)
            {
                listBox1.IsEnabled = false;
                btDeleteButton.IsEnabled = false;
                saveFilterSet.IsEnabled = false;
            }
        }

        /// <summary>
        /// FilterSet aus FilterSet-Liste rechts löschen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteFromFilterSetList_Click(object sender, RoutedEventArgs e)
        {
            FilterSetModel filterSet = listBox2.SelectedItem as FilterSetModel;

            if (filterSet != null)
            {
                listBox2.Items.Remove(listBox2.SelectedItem);
                this.allFilterSets.Remove(filterSet);
            
                if (listBox2.Items.Count == 0)
                {
                    listBox2.IsEnabled = false;
                    btDeleteSetButton.IsEnabled = false;
                }

                listBox1.IsEnabled = false;
                refreshListBoxWithAllFilterSets();
                currentFilterSet = null;
                clearForm();
                tbName.Clear();
                deactivateForm();
            }
        }

        /// <summary>
        /// FilterSet speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFilterSet_Click(object sender, RoutedEventArgs e)
        {
            // Prüfe ob für das Statistiken/Listen-Modul
            if (this.useForListModule && string.IsNullOrEmpty(tbName.Text))
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("FORMLETTER", "noSetNameSpecified"));
                return;
            }

            // Keine leeren Filtersets zulassen
            if (!(listBox1.Items.Count > 0))
                return;

            // Lese Filter aus Filter-Liste
            foreach (var subitem in listBox1.Items)
            {
                currentFilterSet.addFilter( subitem as FilterModel );
            }

            // Wenn nicht für Listen-Modul, lege FilterSet im Speicher ab
            if (!this.useForListModule)
                allFilterSets.Add(currentFilterSet);

            // Für das Listen/Statistiken-Modul wird das FilterSet direkt in die Datenbank gespeichert
            if (this.useForListModule)
            {
                string name = tbName.Text;
                try
                {
                    int filterSetID = FilterSet.Add(name, currentFilterSet.linkingType);

                    // Bearbeite alle Filter zu jedem Set
                    foreach (var filter in currentFilterSet.filterList)
                    {
                        string table = filter.group.ToString();
                        string type = filter.criterion.ToString();
                        string operation = filter.operation.ToString();
                        string value = filter.value;
                        int filterID = Filter.Add(filterSetID, table, type, operation, value);
                    }
                }
                catch 
                {
                }
            }

            listBox1.IsEnabled = false;
            listBox2.IsEnabled = true;
            btDeleteSetButton.IsEnabled = true;
            refreshListBoxWithAllFilterSets();
            currentFilterSet = null;
            clearForm();
            tbName.Clear();
            deactivateForm();
        }

        /// <summary>
        /// Aktiviere Löschen-Button in Filter-Liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btDeleteButton.IsEnabled = true;
        }

        /// <summary>
        /// FilterSet-Formular leeren
        /// </summary>
        private void clearForm()
        {
            tbName.Text = "";
            cbGroup.SelectedItem = null;
            cbCriterion.SelectedItem = null;
            cbOperation.SelectedItem = null;
            tbValue.Text = "";
            lbLinking.Content = "";
            listBox1.Items.Clear();
        }

        /// <summary>
        /// FilterSet-Formular aktivieren
        /// </summary>
        private void activateForm()
        {
            cbGroup.IsEnabled = true;
            cbCriterion.IsEnabled = true;
            cbOperation.IsEnabled = true;
            tbValue.IsEnabled = true;
            button1.IsEnabled = true;

            List<ComboBoxItems> liste1 = new List<ComboBoxItems>();
            liste1.Add(new ComboBoxItems(FilterModel.Groups.Kunde.ToString(), (byte)FilterModel.Groups.Kunde));
            liste1.Add(new ComboBoxItems(FilterModel.Groups.Sponsor.ToString(), (byte)FilterModel.Groups.Sponsor));
            liste1.Add(new ComboBoxItems(FilterModel.Groups.Mitarbeiter.ToString(), (byte)FilterModel.Groups.Mitarbeiter));
            cbGroup.ItemsSource = liste1;

            // When refreshing ComboBoxes like this, global FontSize styles don't seem to apply
            cbGroup.FontSize = 14;
            cbCriterion.FontSize = 14;
            cbOperation.FontSize = 14;
            tbValue.FontSize = 14;

            tbValue.ToolTip = IniParser.GetSetting("FILTER", "tooltipTextBox");
        }

        /// <summary>
        /// FilterSet-Formular deaktivieren
        /// </summary>
        private void deactivateForm()
        {
            cbGroup.IsEnabled = false;
            cbCriterion.IsEnabled = false;
            cbOperation.IsEnabled = false;
            tbValue.IsEnabled = false;
            button1.IsEnabled = false;
            saveFilterSet.IsEnabled = false;
            btDeleteButton.IsEnabled = false;
        }

        /// <summary>
        /// alle Felder und Listen zurücksetzen
        /// </summary>
        public void resetEverything()
        {
            this.clearForm();
            listBox2.Items.Clear();
            allFilterSets = new List<FilterSetModel>();
            currentFilterSet = null;
            listBox1.IsEnabled = false;
            btDeleteButton.IsEnabled = false;
            saveFilterSet.IsEnabled = false;
            tbName.Clear();
        }
        
        /// <summary>
        /// FilterSet-Liste aktualisieren
        /// </summary>
        private void refreshListBoxWithAllFilterSets()
        {
            listBox2.Items.Clear();
            foreach (var item in allFilterSets)
                listBox2.Items.Add(item);
        }

        /// <summary>
        /// Auswahl in FilterSet-Liste geändert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox2.Items.Count > 0)
            {
                FilterSetModel selectedFilterSet = listBox2.SelectedItem as FilterSetModel;
                if (selectedFilterSet != null)
                {
                    lbLinking.Content = selectedFilterSet.linkingType;
                    clearForm();
                    deactivateForm();

                    foreach (var filter in selectedFilterSet.filterList)
                    {
                        listBox1.Items.Add(filter);
                    }
                }
            }
        }

        /// <summary>
        /// Gruppe geändert
        /// Filtert anhand von Gruppe die möglichen Kriterien
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbGroup_SelectionChanged(object sender, EventArgs e)
        {
            if (cbGroup.SelectedItem == null)
                return;

            FilterModel.Groups group = (FilterModel.Groups)((cbGroup.SelectedItem as ComboBoxItems).enumVal);

            List<ComboBoxItems> liste1 = new List<ComboBoxItems>();
            liste1.Add(new ComboBoxItems(group.ToString(), (byte)group));
            cbGroup.ItemsSource = liste1;
            cbGroup.SelectedIndex = 0;

            List<ComboBoxItems> liste2 = new List<ComboBoxItems>();

            switch (group)
            {
                case FilterModel.Groups.Kunde:
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Vorname.ToString(), (byte)FilterModel.Criterions.Vorname));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Nachname.ToString(), (byte)FilterModel.Criterions.Nachname));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Straße_Hsnr.ToString(), (byte)FilterModel.Criterions.Straße_Hsnr));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.PLZ.ToString(), (byte)FilterModel.Criterions.PLZ));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Wohnort.ToString(), (byte)FilterModel.Criterions.Wohnort));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Geburtsdatum.ToString(), (byte)FilterModel.Criterions.Geburtsdatum));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Alter.ToString(), (byte)FilterModel.Criterions.Alter));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Letzter_Einkauf.ToString(), (byte)FilterModel.Criterions.Letzter_Einkauf));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Anzahl_Kinder.ToString(), (byte)FilterModel.Criterions.Anzahl_Kinder));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Nationalität.ToString(), (byte)FilterModel.Criterions.Nationalität));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Geburtsland.ToString(), (byte)FilterModel.Criterions.Geburtsland));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Gültigkeitsbeginn.ToString(), (byte)FilterModel.Criterions.Gültigkeitsbeginn));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Gültigkeitsende.ToString(), (byte)FilterModel.Criterions.Gültigkeitsende));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Erfassungsdatum.ToString(), (byte)FilterModel.Criterions.Erfassungsdatum));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Anzahl_Personen.ToString(), (byte)FilterModel.Criterions.Anzahl_Personen));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Verheiratet.ToString(), (byte)FilterModel.Criterions.Verheiratet));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Aktiv.ToString(), (byte)FilterModel.Criterions.Aktiv));
                    break;

                case FilterModel.Groups.Sponsor:
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Firma.ToString(), (byte)FilterModel.Criterions.Firma));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Vorname.ToString(), (byte)FilterModel.Criterions.Vorname));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Nachname.ToString(), (byte)FilterModel.Criterions.Nachname));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Straße_Hsnr.ToString(), (byte)FilterModel.Criterions.Straße_Hsnr));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.PLZ.ToString(), (byte)FilterModel.Criterions.PLZ));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Wohnort.ToString(), (byte)FilterModel.Criterions.Wohnort));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Kontaktperson.ToString(), (byte)FilterModel.Criterions.Kontaktperson));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Serienbrief_erlaubt.ToString(), (byte)FilterModel.Criterions.Serienbrief_erlaubt));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Aktiv.ToString(), (byte)FilterModel.Criterions.Aktiv));
                    break;

                case FilterModel.Groups.Mitarbeiter:
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Vorname.ToString(), (byte)FilterModel.Criterions.Vorname));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Nachname.ToString(), (byte)FilterModel.Criterions.Nachname));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Straße_Hsnr.ToString(), (byte)FilterModel.Criterions.Straße_Hsnr));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.PLZ.ToString(), (byte)FilterModel.Criterions.PLZ));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Wohnort.ToString(), (byte)FilterModel.Criterions.Wohnort));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Geburtsdatum.ToString(), (byte)FilterModel.Criterions.Geburtsdatum));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Geburtsdatum.ToString(), (byte)FilterModel.Criterions.Alter));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Teamfunktion.ToString(), (byte)FilterModel.Criterions.Teamfunktion));
                    liste2.Add(new ComboBoxItems(FilterModel.Criterions.Aktiv.ToString(), (byte)FilterModel.Criterions.Aktiv));
                    break;
            }
            cbCriterion.ItemsSource = liste2;

        }

        /// <summary>
        /// Kriterium geändert
        /// Filtert anhand von Kriterium die möglichen Operatoren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbCriterion_SelectionChanged(object sender, EventArgs e)
        {
            if (cbCriterion.SelectedItem == null)
                return;

            FilterModel.Criterions criterion = (FilterModel.Criterions)((cbCriterion.SelectedItem as ComboBoxItems).enumVal);
            List<ComboBoxItems> liste3 = new List<ComboBoxItems>();

            switch (criterion)
            {
                case FilterModel.Criterions.Aktiv:
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.gleich.ToString(), (byte)FilterModel.Operations.gleich));
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.ungleich.ToString(), (byte)FilterModel.Operations.ungleich));
                    break;
                case FilterModel.Criterions.Kontaktperson:
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.gleich.ToString(), (byte)FilterModel.Operations.gleich));
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.ungleich.ToString(), (byte)FilterModel.Operations.ungleich));
                    break;
                case FilterModel.Criterions.Serienbrief_erlaubt:
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.gleich.ToString(), (byte)FilterModel.Operations.gleich));
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.ungleich.ToString(), (byte)FilterModel.Operations.ungleich));
                    break;
                default:
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.kleiner.ToString(), (byte)FilterModel.Operations.kleiner));
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.größer.ToString(), (byte)FilterModel.Operations.größer));
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.gleich.ToString(), (byte)FilterModel.Operations.gleich));
                    liste3.Add(new ComboBoxItems(FilterModel.Operations.ungleich.ToString(), (byte)FilterModel.Operations.ungleich));
                    break;
            }
            cbOperation.ItemsSource = liste3;
        }
    }
}
