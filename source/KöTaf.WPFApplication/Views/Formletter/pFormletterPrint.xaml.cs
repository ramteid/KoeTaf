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
using KöTaf.WPFApplication.Models;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.FileOperations;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Formletter
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Serienbrief-Druck: Auflistung aller Serienbrief-Vorlagen
    /// </summary>
    public partial class pFormletterPrint : KPage
    {
        public pFormletterPrint()
        {
            InitializeComponent();
            refreshFormletterPatternList();
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
        }

        /// <summary>
        /// SB-Liste aktualisieren
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
        public void buttonPrint_Click(object sender, RoutedEventArgs e)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
            }

            try
            {
                var item = (sender as FrameworkElement).DataContext;
                int index = listView.Items.IndexOf(item);

                List<FormletterPatternItem> formletterPatternItems = (List<FormletterPatternItem>)DataContext;
                FormletterPatternItem toPrint = formletterPatternItems[index];
                int pID = toPrint.formletterPatternId;

                KPage pagePrint = new pFormletterPrintDetail(pID);
                SinglePage singlePage = new SinglePage(IniParser.GetSetting("FORMLETTER", "formletterPatternPrint"), pagePrint);
            }
            catch
            {
            }
        }
    }
}
