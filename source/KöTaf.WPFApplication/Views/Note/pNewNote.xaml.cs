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
using KöTaf.WPFApplication;
using KöTaf.DataModel;
using KöTaf.Utils.ValidationTools;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// </summary>
    public partial class pNewNote : KPage
    {
        #region Constructor

        /// <summary>
        /// Konstruktor
        /// </summary>
        public pNewNote()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Toolbar Inhalt Definition
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), BackButton_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), SaveButton_Click);
        }

        #endregion

        #region Events

        /// <summary>
        /// Zurück Button Klick Event
        /// </summary>
        /// <param name="button"></param>
        private void BackButton_Click(Button button)
        {
            KPage pageNoteAdmin = new KöTaf.WPFApplication.Views.pNoteAdministration(); ;
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("NOTES", "notes"), pageNoteAdmin);
        }

        /// <summary>
        /// Speichern Button Klick Event
        /// </summary>
        /// <param name="button"></param>
        private void SaveButton_Click(Button button)
        {
            if (ValidateUserInput())
            {
                var name = NameTextBox.Text;
                var description = SafeStringParser.safeParseToStr(DescriptionTextBox.Text);

                var noteId = Note.Add(name, description);
                if (noteId > 0)
                {
                    KPage pageNoteAdmin = new KöTaf.WPFApplication.Views.pNoteAdministration(); ;
                    SinglePage singlePage = new SinglePage(IniParser.GetSetting("NOTES", "notes"), pageNoteAdmin);
                }
            }
        }
       
        /// <summary>
        /// Beschreibungsfeld Textänderung Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DescriptionLengthLabel.Content = DescriptionTextBox.Text.Length.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validierung der Benutzereingaben
        /// </summary>
        /// <returns>Gibt einen Wert zurück ob die Benutzereingaben valide sind</returns>
        private bool ValidateUserInput()
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(NameTextBox.Text))
                errors.AppendLine(IniParser.GetSetting("NOTES", "emptyName"));

            if (NameTextBox.Text.Length > 500)
                errors.AppendLine(IniParser.GetSetting("NOTES", "nameError"));

            if (string.IsNullOrEmpty(DescriptionTextBox.Text))
                errors.AppendLine(IniParser.GetSetting("NOTES", "emptyDescription"));

            if (DescriptionTextBox.Text.Length >= 4000)
                errors.AppendLine(IniParser.GetSetting("NOTES", "descriptionError"));

            if (errors.Length > 0)
            {
                errors.Insert(0, IniParser.GetSetting("NOTES", "errors") + "\n");

                MessageBox.Show(errors.ToString(), IniParser.GetSetting("NOTES", "saveError"), MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        #endregion
    }
}

