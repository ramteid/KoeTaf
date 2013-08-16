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
using KöTaf.DataModel;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Formletter
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Serienbrief-Vorlage löschen
    /// </summary>
    public partial class pFormletterDelete : KPage
    {
        private int formletterPatternID;

        public pFormletterDelete(int formletterPatternID)
        {
            InitializeComponent();
            this.formletterPatternID = formletterPatternID;
            listFormletterDetails(formletterPatternID);
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
        }

        /// <summary>
        /// Details zum Serienbrief auflisten
        /// </summary>
        /// <param name="formletterPatternID"></param>
        private void listFormletterDetails(int formletterPatternID)
        {
            // Baue FormletterPattern Instanz
            FormletterPatternModelDB formletterPatternModel = new FormletterPatternModelDB(formletterPatternID);

            // Liste triviale Eigenschaften auf 
            lbName.Content = formletterPatternModel.name;
            lbSaluationM.Content = formletterPatternModel.saluation_m;
            lbSaluationF.Content = formletterPatternModel.saluation_f;
            lbSaluationN.Content = formletterPatternModel.saluation_n;
            lbFileName.Content = formletterPatternModel.formletter_filename;

            spPreview.Visibility = Visibility.Visible;
            btnConfirm.Visibility = Visibility.Visible;

            if (!String.IsNullOrEmpty(formletterPatternModel.letterText))
            {
                Label lbLetterText = new Label();
                Label lbLetterText1 = new Label();
                lbLetterText.Content = "Brieftext:";
                lbLetterText1.Content = formletterPatternModel.letterText;
                spLetterText.Children.Add(lbLetterText);
                spLetterText.Children.Add(lbLetterText1);
                spLetterText.Visibility = Visibility.Visible;
            }

            // Liste alle FilterSets / Filter auf
            foreach (var set in formletterPatternModel.filterSetModels)
            {
                Label lb = new Label();
                lb.Content = "Filter-Set: ";
                lb.Width = 200;
                lb.Margin = new Thickness(0, 20, 0, 0);
                Label lb1 = new Label();
                lb1.Margin = new Thickness(0, 20, 0, 0);
                lb1.Content = set.name;
                WrapPanel wp5 = new WrapPanel();
                wp5.Children.Add(lb);
                wp5.Children.Add(lb1);
                spFilterSets.Children.Add(wp5);

                foreach (var filter in set.filterList)
                {
                    Label lbFilter = new Label();
                    lbFilter.Margin = new Thickness(30, 0, 0, 0);
                    lbFilter.Content = filter.ToString();
                    spFilterSets.Children.Add(lbFilter);
                }
            }

            // Liste Spaltenzuweisungen auf
            Label lbColumnAssignments = new Label();
            lbColumnAssignments.Margin = new Thickness(0,30,0,0);
            lbColumnAssignments.Content = IniParser.GetSetting("FORMLETTER", "printAssignment");
            spPreview.Children.Add(lbColumnAssignments);

            foreach (var assignment in formletterPatternModel.formletterColumnCsvDocumentAssignments)
			{
                Label lb = new Label();
                lb.Content = assignment.ToString();
                lb.Margin = new Thickness(30, 0, 0, 0);
                spPreview.Children.Add(lb);
			}

        }

        /// <summary>
        /// Löschen bestätigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxEnhanced.Warning(IniParser.GetSetting("FORMLETTER", "questionDelete"));
            if (result == MessageBoxResult.Yes)
            {
                FormletterPatternModelDB formletterPatternModel = new FormletterPatternModelDB(this.formletterPatternID);
                formletterPatternModel.deleteThisFormletterPattern();

                // Generiere wieder die Hauptseite der SB-Verwaltung
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                Type pageType = typeof(pFormletterAdministration);
                mainWindow.switchPage(IniParser.GetSetting("FORMLETTER", "formletterAdmin"), pageType);
                
            }
        }

        /// <summary>
        /// Alles zurücksetzen
        /// </summary>
        private void clearEverything()
        {
            lbName.Content = "";
            lbSaluationM.Content = "";
            lbSaluationF.Content = "";
            lbSaluationN.Content = "";
            lbFileName.Content = "";

            spFilterSets.Children.Clear();
            spLetterText.Children.Clear();

            spPreview.Visibility = Visibility.Hidden;
            btnConfirm.Visibility = Visibility.Hidden;
            spLetterText.Visibility = Visibility.Hidden;
        }
    }
}
