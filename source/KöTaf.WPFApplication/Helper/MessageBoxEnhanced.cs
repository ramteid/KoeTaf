using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Klasse MessageBoxEnhanced: Stellt Methoden zum anzeigen einer MessageBox bereit
    /// </summary>
    public class MessageBoxEnhanced
    {
        /// <summary>
        /// Der Programmname
        /// </summary>
        private readonly static string _AppName = string.Concat(Properties.Resources.ApplicationName, " ", IniParser.GetSetting("APPSETTINGS", "messageBoxHeadline"));

        /// <summary>
        /// Error Fehlermeldung
        /// </summary>
        /// <param name="format">Format</param>
        /// <param name="args">Argumente</param>
        /// <returns>Eine MessageBox</returns>
        public static MessageBoxResult Error(string format, params object[] args)
        {
            return CommonMessageBox(MessageBoxButton.OK, MessageBoxImage.Error, format, args);
        }

        /// <summary>
        /// Gibt das Ergebnis einer MessageBox zurück
        /// </summary>
        /// <param name="msg">Die Nachricht die angezeigt werden soll</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Error(string msg)
        {
            return CommonMessageBox(msg, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Stellt eine Informationsbox dar
        /// </summary>
        /// <param name="msg">Die Nachricht die angezeigt werde soll</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Info(string msg)
        {
            return CommonMessageBox(msg, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Info MessageBox
        /// </summary>
        /// <param name="format">Formatierungsstring</param>
        /// <param name="args">Argumente</param>
        /// <returns></returns>
        public static MessageBoxResult Info(string format, params object[] args)
        {
            return CommonMessageBox(MessageBoxButton.OK, MessageBoxImage.Information, format, args);
        }

        /// <summary>
        /// Frage MessageBox
        /// </summary>
        /// <param name="msg">Die Nachricht die angezeigt werden soll</param>
        /// <param name="button">Der Button der dargestellt werden soll</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Question(string msg, MessageBoxButton button = MessageBoxButton.YesNo)
        {
            return CommonMessageBox(msg, button, MessageBoxImage.Question);
        }

        /// <summary>
        /// Frage MessageBox
        /// </summary>
        /// <param name="format">Formatierungsstring</param>
        /// <param name="button">Der Button der angezeigt werden soll</param>
        /// <param name="args">Argumente</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Question(string format, MessageBoxButton button = MessageBoxButton.YesNo, params object[] args)
        {
            return CommonMessageBox(button, MessageBoxImage.Question, format, args);
        }

        /// <summary>
        /// Warnungs MessageBox
        /// </summary>
        /// <param name="msg">Die Nachricht die angezeigt werden soll</param>
        /// <param name="button">Button der angezeigt werden soll</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Warning(string msg, MessageBoxButton button = MessageBoxButton.YesNo)
        {
            return CommonMessageBox(msg, button, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Warnung MessageBox
        /// </summary>
        /// <param name="format">Formatierungsstring</param>
        /// <param name="button">Optionaler Button der dargestellt werden soll</param>
        /// <param name="args">Argumente</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult Warning(string format, MessageBoxButton button = MessageBoxButton.YesNo, params object[] args)
        {
            return CommonMessageBox(button, MessageBoxImage.Warning, format, args);
        }

        /// <summary>
        /// Allgemeine MessageBox
        /// </summary>
        /// <param name="message">Die Nachricht die dargestellt werden soll</param>
        /// <param name="btn">Der Button der angezeigt werden soll</param>
        /// <param name="img">Das Bild das angezeigt werden soll</param>
        /// <returns>MessageBoxResult</returns>
        public static MessageBoxResult CommonMessageBox(string message, MessageBoxButton btn, MessageBoxImage img)
        {
            return MessageBox.Show(message, _AppName, btn, img);
        }

        /// <summary>
        /// Allgemeine MesssageBox
        /// </summary>
        /// <param name="btn">Der Button der dargestellt werden </param>
        /// <param name="img">Das Bild das dargestellt werden soll</param>
        /// <param name="format">Der Formatierungsstring</param>
        /// <param name="args">Argumente</param>
        /// <returns></returns>
        public static MessageBoxResult CommonMessageBox(MessageBoxButton btn, MessageBoxImage img, string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, _AppName, btn, img);
        }
    }
}
