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
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Models;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Formletter
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Serienbrief-Verwaltung
    /// </summary>
    /// 
    public partial class pFormletterAdministration : KPage
    {
        public pFormletterAdministration()
        {
            InitializeComponent();
            refreshFormletterPatternList();
        }
        
        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton("Neue Serienbriefvorlage", createFormletter_Click);
        }

        /// <summary>
        /// Serienbrief-Vorlage speichern
        /// </summary>
        /// <param name="button"></param>
        private void createFormletter_Click(Button button)
        {
            // Liste aller anzuzeigenden Tabs
            List<SimpleTabItem> myTabList = new List<SimpleTabItem>();

            // Füge einen neuen Tab mit der zugehörigen Seite hinzu, die darin angezeigt werden soll
            KPage filter = new pFormletterFilterSelection();
            SimpleTabItem tabFilter = new SimpleTabItem(IniParser.GetSetting("FORMLETTER", "formletterReceiver"), filter);
            myTabList.Add(tabFilter);

            // Füge einen neuen Tab mit der zugehörigen Seite hinzu, die darin angezeigt werden soll
            KPage text = new pFormletterTextFields();
            SimpleTabItem tabText = new SimpleTabItem(IniParser.GetSetting("FORMLETTER", "formletterTextFields"), text);
            myTabList.Add(tabText);

            // Füge einen neuen Tab mit der zugehörigen Seite hinzu, die darin angezeigt werden soll
            KPage file = new pFormletterFileLinking();
            SimpleTabItem tabFile = new SimpleTabItem(IniParser.GetSetting("FORMLETTER", "formletterLinking"), file);
            myTabList.Add(tabFile);

            // Füge drei Buttons hinzu, damit der Speichern-Button unter jedem Tab gleich ist
            tabFilter.toolbar.addButton(IniParser.GetSetting("FORMLETTER", "formletterSave"), saveFormletterTemplate);
            tabFile.toolbar.addButton(IniParser.GetSetting("FORMLETTER", "formletterSave"), saveFormletterTemplate);
            tabText.toolbar.addButton(IniParser.GetSetting("FORMLETTER", "formletterSave"), saveFormletterTemplate);

            tabFilter.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), cancelFormletterTemplate);
            tabFile.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), cancelFormletterTemplate);
            tabText.toolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), cancelFormletterTemplate);

            // Zeige die Tabs in einer SinglePage an
            SinglePage singlePage = new SinglePage(this, IniParser.GetSetting("FORMLETTER", "formletterNew"), myTabList);
        }

        /// <summary>
        /// Liste mit Serienbriefen (formletter_pattern) aktualisieren
        /// </summary>
        private void refreshFormletterPatternList()
        {
            // Fülle Liste mit Serienbriefen
            List<FormletterPattern> formletterPatterns = FormletterPattern.GetFormletterPatterns().ToList<FormletterPattern>();
            List<FormletterPatternItem> formletterPatternItems = new List<FormletterPatternItem>();

            foreach (var pattern in formletterPatterns)
                formletterPatternItems.Add(new FormletterPatternItem(pattern.Name, pattern.FormletterPatternID));

            DataContext = formletterPatternItems;
        }

        /// <summary>
        /// Löscht den ausgewählten Serienbrief
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (sender as FrameworkElement).DataContext;
                int index = listView.Items.IndexOf(item);

                List<FormletterPatternItem> formletterPatternItems = (List<FormletterPatternItem>)DataContext;
                FormletterPatternItem toDelete = formletterPatternItems[index];
                int pID = toDelete.formletterPatternId;

                KPage pageDelete = new pFormletterDelete(pID);
                SinglePage singlePage = new SinglePage(IniParser.GetSetting("FORMLETTER","formletterDelete"), pageDelete);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Button-Klick-Funktion für "Serienbrief-Vorlage speichern"-Button
        /// </summary>
        /// <param name="senderButton">die Instanz des sendenden Buttons</param>
        private void saveFormletterTemplate(Button senderButton)
        {
            try
            {
                // When using tabs, you can retrieve a page instance included in a tab using this function, giving the page type
                pFormletterFilterSelection pageFilter = getPageFromTabs<pFormletterFilterSelection>();
                pFormletterFileLinking pageFile = getPageFromTabs<pFormletterFileLinking>();
                pFormletterTextFields pageText = getPageFromTabs<pFormletterTextFields>();

                // Prüfe ob alle Eingaben richtig sind
                if (pageFilter.allFilterSets.Count == 0)
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "noFilterSet"));
                    pageFilter.parentTabControl.SelectedIndex = 0;
                    return;
                }
                if (pageFile.lbFileName.Content.Equals(""))
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "noFormletter"));
                    pageFile.parentTabControl.SelectedIndex = 1;
                    return;
                }
                if (!pageFile.validateLinkings())
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "noAssignment"));
                    pageFile.parentTabControl.SelectedIndex = 1;
                    return;
                }
                if (!pageText.validateTextFields())
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "noTextField"));
                    pageText.parentTabControl.SelectedIndex = 2;
                    return;
                }

                // Hole alle Filter-Sets
                List<FilterSetModel> filterSets = pageFilter.allFilterSets;

                // Neues FormletterPatternModelDB (Serienbriefvorlage) anlegen
                string pName = pageText.tbPatternName.Text;
                string saluationM = pageText.tbSaluationM.Text;
                string saluationF = pageText.tbSaluationF.Text;
                string saluationN = pageText.tbSaluationN.Text;
                string filename = pageFile.tbFileName.Text.Replace(System.IO.Directory.GetCurrentDirectory(), "%PROGRAMPATH%");
                string text = "";
                int frmltrPatternID = FormletterPattern.Add(pName, saluationM, saluationF, saluationN, filename, text);

                // Bearbeite alle FilterSets
                foreach (var set in filterSets)
                {
                    string name = "";   // Für Filter-Sets leer, nur von Listen/Statistiken verwendet
                    string linking = set.linkingType;
                    int filterSetID = FilterSet.Add(name, linking, frmltrPatternID);

                    // Bearbeite alle Filter zu jedem Set
                    foreach (var filter in set.filterList)
                    {
                        string table = filter.group.ToString();
                        string type = filter.criterion.ToString();
                        string operation = filter.operation.ToString();
                        string value = filter.value;
                        int filterID = Filter.Add(filterSetID, table, type, operation, value);
                    }
                }

                // Hole alle Spaltenzuordnungen
                List<FormletterColumnCsvDocumentAssignment> list = pageFile.getAssignments();

                // Schreibe Spaltenzuordnungen in die Datenbank
                foreach (var assignment in list)
                {
                    string csvColumn = assignment.csv_col_name;
                    string databaseTable = assignment.formletterTableAssignment.group.ToString();
                    string databaseColumn = assignment.formletterTableAssignment.field.ToString();
                    ColumnAssignment.Add(frmltrPatternID, csvColumn, databaseTable, databaseColumn);
                }

                //Leere Formular
                pageFilter.resetEverything();
                pageFile.resetEverything();
                pageText.resetEverything();

                KPage pageFormletterAdmin = new KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration();
                SinglePage singlePage = new SinglePage(IniParser.GetSetting("FORMLETTER", "formletterAdmin"), pageFormletterAdmin);
            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "formletterCreate"));
            }
        }

        /// <summary>
        /// Das Erstellen einer neuen SB-Vorlage abbrechen
        /// </summary>
        /// <param name="btn">sendender Button</param>
        private void cancelFormletterTemplate(Button btn)
        {
            KPage pageFormletterAdmin = new KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("FORMLETTER", "formletterAdmin"), pageFormletterAdmin);
        }
    }
}
