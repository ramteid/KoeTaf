/**
 * Class: LoginModel
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-15
 * 
 * Last modification: 2013-04-15 / MM
 * bind to mainWindow
 */

// ReSharper disable RedundantUsingDirective
using System;
// ReSharper restore RedundantUsingDirective

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// In dieser Klasse sind die Member UserName, Password, EmailAdress, ImageSourcePath deklariert,
    /// und ist somit als eigenes Objekt verfügbar.
    /// </summary>
	public class UserModel
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string EMailAddress { get; set; }
        public string ImageSourcePath { get; set; }
        public bool IsActive { get; set; }

        public UserModel(string userName, string password, string email, string imageSourcePath, bool isActive)
        {
            this.UserName = userName;
            this.Password = password;
            this.EMailAddress = email;
            this.ImageSourcePath = imageSourcePath;
            this.IsActive = isActive;
        }
	}
}
