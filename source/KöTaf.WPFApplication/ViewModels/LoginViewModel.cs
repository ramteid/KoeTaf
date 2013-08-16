/**
 * Class: LoginViewModel
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-05
 * 
 * Last modification: 2013-04-18 / MM
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


namespace KöTaf.WPFApplication.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        List<UserModel> userList;
        private readonly string USER_IMAGE_PATH = @"\Images";

        #endregion

        #region Constructors

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

        public string UserName
        {
            get { return GetValue(() => UserName); }
            set
            {
                SetValue(() => UserName, value);

                this.UserImageSource = this.getUserImagePath();
            }
        }

        public string Password
        {
            get { return GetValue(() => Password); }
            set { SetValue(() => Password, value); }
        }

        public string UserImageSource
        {
            get { return GetValue(() => UserImageSource); }
            set { SetValue(() => UserImageSource, value); }
        }

        #endregion // Public Properties

        #region Submit Command Handler

        public ICommand SubmitCommand { get; private set; }

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
                    if (account != null)
                    {

                        var newWindow = new MainWindow(account);
                        App.Current.Windows[0].Close();
                        newWindow.ShowDialog();
                    }
                }
                else
                {
                    accessControlSystem.ShowWrongCredentialsMessage();
                }
            }
        }

        private bool CanExecuteSubmit(object commandParameter)
        {
            return !string.IsNullOrEmpty(this.Password);
        }

        #endregion

        #region Private Methods

        private void initializeAllCommands()
        {
            this.SubmitCommand = new ActionCommand(this.ExecuteSubmit, this.CanExecuteSubmit);
        }

        private void getAllUser()
        {
            this.userList = UserAccount.GetUserAccounts().Select(x => new UserModel
            {
                UserName = x.Username,
                Password = x.Password,
                ImageSourcePath = Path.Combine(this.USER_IMAGE_PATH,x.ImageName),

            }).ToList();
        }

        private bool validateUser(string username, string password)
        {
            UserModel validatedUser = this.userList.FirstOrDefault(user => user.UserName.Equals(username) &&
                                                                                user.Password.Equals(password));
            return validatedUser != null;
        }

        private string getUserImagePath()
        {
            while (this.userList == null) { }
            UserModel currentUser = this.userList.FirstOrDefault(user => user.UserName.Equals(this.UserName));
            
                if (currentUser != null)
            {
                return currentUser.ImageSourcePath;
            }
            return String.Empty;
        }
        #endregion
    }
}
