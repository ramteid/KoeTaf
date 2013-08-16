/**
 * Class: LoginViewModel
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-05-16
 * 
 * Last modification: 2013-06-25 / Michael Müller
 * bind to mainWindow
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using KöTaf.WPFApplication.Models;
using SoftArcs.WPFSmartLibrary.MVVMCommands;
using SoftArcs.WPFSmartLibrary.MVVMCore;
using SoftArcs.WPFSmartLibrary.SmartUserControls;
using System.Windows;
using System.Threading;
using KöTaf.DataModel;
using KöTaf.Utils.UserSession;
using KöTaf.WPFApplication.Helper;
using System.ComponentModel;


namespace KöTaf.WPFApplication.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        List<UserModel> userList;
        private readonly string USER_IMAGE_PATH = @"\Images";

        #endregion

        #region Constructors
        /// <summary>
        /// Constructor. Initialisiere das LoginViewModel
        /// </summary>
        public LoginViewModel()
        {
            if (ViewModelHelper.IsInDesignModeStatic == false)
            {
                this.initializeAllCommands();
                Thread newWindowThread = new Thread(new ThreadStart(this.getAllUser));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();
            }
        }

        #endregion // Constructors

        #region Public Properties

        /// <summary>
        /// Getter / Setter für UserName
        /// </summary>
        public string UserName
        {
            get { return GetValue(() => UserName); } //Here you must insert String.toLower()
            set
            {
                SetValue(() => UserName, value);
                string image= this.getUserImagePath();
                this.UserImageSource = image;
            }
        }
        /// <summary>
        /// Getter / Setter für Passwort
        /// </summary>
        public string Password
        {
            get { return GetValue(() => Password); }
            set { SetValue(() => Password, value); }
        }

        /// <summary>
        /// Getter / Setter UserImage (Angezeigtes Bild am beim Login des Users)
        /// </summary>
        public string UserImageSource
        {
            get { return GetValue(() => (UserImageSource == null ? "" : UserImageSource)); }
            set { SetValue(() => UserImageSource, value); }
        }

        #endregion // Public Properties

        #region Submit Command Handler

        public ICommand SubmitCommand { get; private set; }
        /// <summary>
        /// In dieser Methode wird Username und Passwort geprüft, und
        /// es wird bei erfolgreicher Übereinstimmung die MainWindow
        /// aufgerufen / eine neue Instanz erstellt.
        /// </summary>
        /// <param name="commandParameter"></param>
        private void ExecuteSubmit(object commandParameter)
        {
            var accessControlSystem = commandParameter as SmartLoginOverlay;
            if (accessControlSystem != null)
            {
                if (this.validateUser(this.UserName, this.Password) == true)
                {
                    //List<String> liste = null;
                    accessControlSystem.Unlock();
                    var account = UserAccount.GetUserAccounts(null, this.UserName, this.Password).FirstOrDefault();

                    // Initialisiere User-Session vor Laden des MainWindows
                    UserSession.userAccountID = account.UserAccountID;
                    UserSession.userName = account.Username;
                    UserSession.isAdmin = account.IsAdmin;
                    UserSession.userAccount = account;

                    if (account != null)
                    {
                        var newWindow = new MainWindow();
                        App.Current.Windows[0].Close();
                        Application.Current.MainWindow = newWindow;
                        newWindow.initPage();
                        newWindow.Show();
                    }
                }
                    // falls Login nicht erfolgreich gebe entsprechende Meldung aus.
                else
                {
                    accessControlSystem.ShowWrongCredentialsMessage();
                }
            }
        }
        /// <summary>
        /// Gibt boolean(true) Wert zurück, falls das Password nicht null oder leer ist. 
        /// </summary>
        /// <param name="commandParameter"></param>
        /// <returns></returns>
        private bool CanExecuteSubmit(object commandParameter)
        {
            return !string.IsNullOrEmpty(this.Password);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialisiere alle Eingaben.
        /// </summary>
        private void initializeAllCommands()
        {
            this.SubmitCommand = new ActionCommand(this.ExecuteSubmit, this.CanExecuteSubmit);
        }

        /// <summary>
        /// Lade userList; Mit Username, Password, und das zugehörige Bild aus der Datenbank.
        /// Achtung Bild/ Image liegt im extra SubFolder! Das Bild ist also nicht in der Datenbank gespeichert
        /// sondern nur der Datei-Bild-Name.
        /// </summary>
        private void getAllUser()
        {
            try
            {
                this.userList = new List<UserModel>();
                List<UserAccount> userAccounts = UserAccount.GetUserAccounts().ToList();
                foreach (var account in userAccounts)
                {
                    string imageName = (account.ImageName == null ? "" : account.ImageName);
                    string imageSourcePath = Path.Combine(this.USER_IMAGE_PATH, imageName);
                    UserModel userModel = new UserModel(account.Username, account.Password, "", imageSourcePath, account.IsActive);
                    this.userList.Add(userModel);
                }
            }
            catch
            {
            }

        }

        /// <summary>
        /// Validiere User (username, password)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool validateUser(string username, string password)
        {
            UserModel validatedUser = this.userList.FirstOrDefault(user => user.UserName.Equals(username) &&
                                                                                user.Password.Equals(password) &&
                                                                                user.IsActive);
            return validatedUser != null;
        }

        /// <summary>
        /// Gibt den Image Source Path bzw. einen leeren String zurück,
        /// falls der UseName nicht geladen werden konnte.
        /// </summary>
        /// <returns></returns>
        private string getUserImagePath()
        {
            string userName = this.UserName;
            UserModel currentUser = this.userList.Where(u => u.UserName == userName).FirstOrDefault();
            
            if (currentUser != null)
            {
                return currentUser.ImageSourcePath;
            }
            return String.Empty;
        }
        #endregion
    }
}
