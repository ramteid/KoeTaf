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
using Microsoft.Win32;
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Template;
using KöTaf.WPFApplication.Helper;
using KöTaf.DataModel;
using KöTaf.Utils.ValidationTools;

namespace KöTaf.WPFApplication.Views.User
{
    /// <summary>
    /// Author: Patrick Vogt, Dietmar Sach
    /// Interaktionslogik für pNewAccount.xaml
    /// </summary>
    public partial class pNewUser : KPage
    {
        private ValidationTools validator;
        private bool isValid;
        private string fileName;

        public pNewUser()
        {
            InitializeComponent();
            this.validator = new ValidationTools();
        }

        #region Methods
        /// <summary>
        /// Hinzufügen der Buttons "Speichern", "Zurücksetzen" und "Abbrechen"
        /// </summary>
        /// <see cref="Speichern_Click"/>
        /// <see cref="Reset_Click"/>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "save"), Speichern_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "reset"), Reset_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("BUTTONS", "cancel"), Cancel_Click);
        }

        /// <summary>
        /// Prüft das Formular auf valide Eingaben
        /// </summary>
        /// <returns>Falls alle Eingaben korrekt sind, wird TRUE zurück gegeben, anderenfalls FALSE</returns>
        private bool checkForm()
        {
            this.isValid = true;

            #region checkMandatoryFields

            if (string.IsNullOrEmpty(tbBenutzername.Text) || !this.validator.IsName(IniParser.GetSetting("USER", "username"), tbBenutzername.Text))
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("USER", "username"), IniParser.GetSetting("ERRORMSG", "noUsername"));
            }

            if (string.IsNullOrEmpty(tbPasswort.Password))
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("USER", "password"), IniParser.GetSetting("ERRORMSG", "invalidPassword"));
            }

            if (!tbPasswort.Password.Equals(tbPasswortConfirmation.Password))
            {
                this.isValid = false;
                this.validator.addError(IniParser.GetSetting("USER", "password"), IniParser.GetSetting("ERRORMSG", "passwordConfirmation"));
            }
            #endregion

            return this.isValid;
        }

        /// <summary>
        /// Validiert die Größe des ausgewählten Bildes
        /// </summary>
        /// <param name="dlg"></param>
        /// <returns></returns>
        private bool checkImageFormat(OpenFileDialog dlg)
        {
            try
            {
                int ImageWidth = Convert.ToInt32(IniParser.GetSetting("IMAGES", "imageWidth"));
                int ImageHight = Convert.ToInt32(IniParser.GetSetting("IMAGES", "imageHeight"));
                System.Drawing.Image image = System.Drawing.Image.FromFile(dlg.FileName);
                int ImageX = image.Width;
                int ImageY = image.Height;
                if (ImageX > ImageWidth || ImageY > ImageHight)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Legt ein neues Benutzerkonto an
        /// </summary>
        /// <param name="button"></param>
        private void Speichern_Click(Button button)
        {
            this.validator.clearSB();
            this.checkForm();

            if (!this.isValid)
            {
                MessageBox.Show(this.validator.getErrorMsg().ToString(), IniParser.GetSetting("ERRORMSG", "noTextField"), MessageBoxButton.OK, MessageBoxImage.Hand);
                this.validator.clearSB();
            }
            else
            {
                var username = tbBenutzername.Text;
                var password = tbPasswort.Password;
                bool isAdmin = (bool)chkbIsAdmin.IsChecked;

                try
                {
                    string currentDir = System.IO.Directory.GetCurrentDirectory();

                    string sourcePath = tbImage.Text;
                    string targetPath = @IniParser.GetSetting("SETTINGS", "imagePath").Replace("%PROGRAMPATH%", currentDir);
                    
                    string destFile = System.IO.Path.Combine(targetPath, this.fileName);
                    System.IO.File.Copy(sourcePath, destFile, true);
                }
                catch
                {
                    this.fileName = "";
                }

                var accountId = UserAccount.Add(username, password, isAdmin, this.fileName);

                if (accountId > 0)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    Type pageType = typeof(pUserManager);
                    mainWindow.switchPage(IniParser.GetSetting("USER", "userAdministration"), pageType);
                }
                else
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "newUser"));
                    this.validator.clearSB();
                }
            }
        }

        /// <summary>
        /// Leert alle Formularfelder
        /// </summary>
        /// <param name="button"></param>
        private void Reset_Click(Button button)
        {
            tbBenutzername.Text = null;
            tbPasswort.Password = null;
            tbPasswortConfirmation.Password = null;
            chkbIsAdmin.IsChecked = false;
            tbImage = null;
        }

        /// <summary>
        /// Bricht den Vorgang ab und kehrt zur Benutzerübersicht zurück
        /// </summary>
        /// <param name="button"></param>
        private void Cancel_Click(Button button)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Type pageType = typeof(pUserManager);
            mainWindow.switchPage(IniParser.GetSetting("USER", "userAdministration"), pageType);
        }

        /// <summary>
        /// Öffnet einen Dateiexplorer zur Auswahl eines Benutzerbildes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = IniParser.GetSetting("IMAGES", "imageFileExtensions");
            dlg.FileOk += new System.ComponentModel.CancelEventHandler(fileDialog_fileOK);

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                
                if (!checkImageFormat(dlg))
                {
                    tbImage.Text = "";
                    KöTaf.WPFApplication.Helper.MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "imageError"));
                }
                else
                {
                    tbImage.Text = dlg.FileName;
                    this.fileName = dlg.SafeFileName;
                }
            }
        }

        /// <summary>
        /// Stellt das ausgewählte Benutzerbild dar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileDialog_fileOK(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string selectedFile = (sender as OpenFileDialog).FileName;
                userImage.Source = BitmapFrame.Create(new Uri(selectedFile));
            }
            catch { }
        }
        #endregion
    }
}