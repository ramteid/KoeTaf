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
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Formletter
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Serienbrief-Vorlage erstellen
    /// Seite Textfelder
    /// </summary>     
    public partial class pFormletterTextFields : KPage
    {
        public pFormletterTextFields()
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
        /// Felder zurücksetzen
        /// </summary>
        public void resetEverything()
        {
            tbPatternName.Clear();
            tbSaluationM.Text = IniParser.GetSetting("FORMLETTER", "defaultSaluationM");
            tbSaluationF.Text = IniParser.GetSetting("FORMLETTER", "defaultSaluationF");
            tbSaluationN.Text = IniParser.GetSetting("FORMLETTER", "defaultSaluationN");
            tbLetterText.Clear();
        }

        /// <summary>
        /// Felder validieren
        /// </summary>
        /// <returns></returns>
        public bool validateTextFields()
        {
            if (tbPatternName.Text == "" ||
                tbSaluationM.Text == "" ||
                tbSaluationF.Text == "" ||
                tbSaluationN.Text == "" ||
                false)
            {
                return false;
            }
            else
                return true;

        }
    }
}
