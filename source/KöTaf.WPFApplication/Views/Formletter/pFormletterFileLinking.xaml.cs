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
using KöTaf.WPFApplication.Properties;
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.FileOperations;

namespace KöTaf.WPFApplication.Views.Formletter
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Dateiverknüpfung einer Serienbrief-Vorlage
    /// </summary>
    public partial class pFormletterFileLinking : KPage
    {
        public pFormletterFileLinking()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
        }

        /// <summary>
        /// Durchsucht das Dateisystem nach einem Serienbrief
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browse_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Parse odt path from config file App.Config
            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string path = IniParser.GetSetting("FORMLETTER", "path").Replace("%PROGRAMPATH%" , currentDir);

            // Set filter for file extension and default file extension
            dlg.DefaultExt = IniParser.GetSetting("FORMLETTER", "extension");
            dlg.Filter = IniParser.GetSetting("FORMLETTER", "extensionDetail");
            dlg.InitialDirectory = path;
            
            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                tbFileName.Text = filename;
                lbFileName.Content = filename;
                // Generate form with column assigments
                generateAssignmentsForm(filename);
            }
        }

        /// <summary>
        /// Zuordnungen zu den Serienbrief-Feldern generieren
        /// </summary>
        /// <param name="filename"></param>
        private void generateAssignmentsForm(string filename)
        {
            panelLinkings.Children.Clear();

            // Get the used CSV-Column-names from the formletter file
            IEnumerable<string> columns = ( LibreOffice.GetDatabaseFieldAttributeFromODT(filename, "text:column-name") ).Distinct<string>();

            Label info = new Label();
            info.Margin = new Thickness(10,20,10,10);
            info.Content = IniParser.GetSetting("FORMLETTER", "printAssignment");
            panelLinkings.Children.Add(info);

            foreach (var column in columns)
            {
                WrapPanel wp = new WrapPanel();
                wp.Margin = new Thickness(0);

                Label lb = new Label();
                lb.Content = column;
                lb.MinWidth = 150;
                lb.Margin = new Thickness(10);

                ComboBox cb = new ComboBox();
                cb.Margin = new Thickness(10, 10, 10, 10);
                cb.MinWidth = 250;

                List<FormletterTableAssignment> list = new List<FormletterTableAssignment>();
                list.Add(new FormletterTableAssignment());
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Datum, FormletterTableAssignment.Fields.Datum));

                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Einleitung));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Anrede));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Vorname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Nachname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Straße_Hsnr));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.PLZ));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Wohnort));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Nationalität));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Geburtsland));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Geburtsdatum));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Gültigkeitsbeginn));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Gültigkeitsende));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Email));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Telefon));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Mobil));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Letzter_Einkauf));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Familienstand));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Vorname_Partner));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Nachname_Partner));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Nationalität_Partner));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Geburtsland_Partner));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Telefon_Partner));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Mobil_Partner));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Kunde, FormletterTableAssignment.Fields.Email_Partner));

                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Firmenname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Einleitung));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Anrede));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Kontaktperson));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Vorname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Nachname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Straße_Hsnr));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.PLZ));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Wohnort));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Email));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Telefon));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Mobil));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Sponsor, FormletterTableAssignment.Fields.Faxnummer));

                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Teamfunktion));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Einleitung));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Anrede));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Vorname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Nachname));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Straße_Hsnr));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.PLZ));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Wohnort));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Geburtsdatum));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Email));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Telefon));
                list.Add(new FormletterTableAssignment(FormletterTableAssignment.Groups.Mitarbeiter, FormletterTableAssignment.Fields.Mobil));
                cb.ItemsSource = list;
                cb.SelectedIndex = 0;

                wp.Children.Add(lb);
                wp.Children.Add(cb);
                panelLinkings.Children.Add(wp);
            }
        }

        
        /// <summary>
        /// Hole die Spaltenzuweisungen aus den ComboBoxen
        /// </summary>
        /// <returns></returns>
        public List<FormletterColumnCsvDocumentAssignment> getAssignments() 
        {
            var columnAssignments = new List<FormletterColumnCsvDocumentAssignment>();

            int countW = VisualTreeHelper.GetChildrenCount(panelLinkings);
            for (int i = 0; i < countW; i++)
            {
                Visual childVisualW = (Visual)VisualTreeHelper.GetChild(panelLinkings, i);
                if (childVisualW is WrapPanel)  // Jedes WrapPanel
                {
                    string csv_col = "";
                    FormletterTableAssignment selectedItem = null;  // diese beiden variablen werden immer überschrieben

                    WrapPanel wp = childVisualW as WrapPanel;
                    int countC = VisualTreeHelper.GetChildrenCount(wp);
                    for (int j = 0; j < countC; j++)
                    {
                        Visual childVisualC = (Visual)VisualTreeHelper.GetChild(wp, j);
                        if (childVisualC is Label)
                        {
                            Label lb = childVisualC as Label;
                            csv_col = lb.Content as string;
                        }
                        if (childVisualC is ComboBox)
                        {
                            ComboBox cb = childVisualC as ComboBox;
                            if (cb.SelectedItem is FormletterTableAssignment)
                                selectedItem = cb.SelectedItem as FormletterTableAssignment;
                        }
                    }
                    columnAssignments.Add(new FormletterColumnCsvDocumentAssignment(csv_col, selectedItem));
                }
            }
            return columnAssignments;
        }

        /// <summary>
        /// Validiere die Verknüpfungen
        /// </summary>
        /// <returns></returns>
        public bool validateLinkings()
        {
            bool areLinkingsOk = true;
            List<FormletterColumnCsvDocumentAssignment> assignments = getAssignments();

            foreach (var assignment in assignments)
            {
                if (assignment.csv_col_name == "" ||
                    assignment.formletterTableAssignment == null ||
                    assignment.formletterTableAssignment.field.ToString() == "" ||
                    assignment.formletterTableAssignment.group.ToString() == "" )
                {
                    areLinkingsOk = false;
                }
            }
            return areLinkingsOk;
        }

        /// <summary>
        /// Alles zurücksetzen
        /// </summary>
        public void resetEverything()
        {
            tbFileName.Clear();
            lbFileName.Content = "";
            panelLinkings.Children.Clear();
        }
    }
}
