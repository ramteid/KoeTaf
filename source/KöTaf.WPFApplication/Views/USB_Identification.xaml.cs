/**
 * Class: USB_Identification
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-31
 * 
 * Last modification: 2013-06-22 / Michael Müller
 */
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
using System.Windows.Threading;
using KöTaf.Utils.BackupRestore;
using System.Collections;
using System.IO;
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktions Logik für die USB_Identification
    /// Steuerungsklasse für das Backup und die Wiederherstellung von der Datenbank
    /// eines USB - Speichergeräts
    /// </summary>
    public partial class USB_Identification : Window
    {
        Hashtable hList = new Hashtable();
        // Fest kodierte Werte: Diese sind unmittelbar für die USB-Erkennung(Zeisteuerung) notwendig/zuständig.
        // _DISPATCHER_TIMER = Zeit in sek., in welcher nach einem gültigen USB-Gerät gesucht wird.
        private int _TIME_FACTOR_TO_ENABLED_BACKUP_CANCEL_BUTTON, _DISPATCHER_TIMER = 1, _ADD_COUNT_FACTOR = 1, 
                    _Counter = 0;
        private String _USB_FOLDER_NAME, _DbLevel = "", _FailDuringRestore = "0",_FILENAME_DB_EXTENSION,
                       _DOWNLOAD_PIC = "/Images/db_update.png", _UPLOAD_PIC = "/Images/db_comit.png";
        private MainWindow _MainWindowObj;
        private CheckForUSB _Cusb;
        List<String> liste = new List<String>(); 
        BackupAndRestoreDatabase br = new BackupAndRestoreDatabase();
        DispatcherTimer dispatcherTimer = null;
        private Timer _Timer;
        #region Contructor
        /// <summary>
        /// Konstruktor, holt entsprechende Variablen aus der Konfigurationsdatei,
        /// und initialisiert die Klassen CheckForUSB und Timer.
        /// </summary>
        /// <param name="mainW"></param>
        /// <param name="_DbLevel"></param>
        public USB_Identification(MainWindow mainW = null, string _DbLevel = "backup"){
            InitializeComponent();
            // Überprüfe CheckForUSB und Timer. Wenn ein Fehler durch die Konfigurationsdatei verursacht wurde,
            // beende den dezeitigen Zustand.
            try
            {
                this._Cusb = new CheckForUSB();
                this._Timer = new Timer();
            }
            catch (Exception ex) {
                MessageBoxEnhanced.Error(ex.Message);
                this.Close();
                return;
            }
            this._MainWindowObj = mainW;
            // Zuornung aller relevanten Werte aus der Konfiguationsdatei.
            try
            {
                this._USB_FOLDER_NAME = IniParser.GetSetting("USB", "USB_FOLDER_NAME");
                this._TIME_FACTOR_TO_ENABLED_BACKUP_CANCEL_BUTTON = Convert.ToInt32(IniParser.GetSetting("USB", "TIME_FACTOR_TO_ENABLED_BACKUP_CANCEL_BUTTON"));
                this._FILENAME_DB_EXTENSION = IniParser.GetSetting("USB", "FILENAME_DB_EXTENSION").ToLower();
                // __FILENAME_DB_EXTENSION darf nicht null sein.
                if (this._FILENAME_DB_EXTENSION == "") {
                    throw new Exception("FAIL");
                }        
            }
            // Wenn ein Fehler durch die Konfigurationsdatei verursacht wurde, beende den
            // dezeitigen Zustand
            catch(Exception){
                KöTaf.WPFApplication.Helper.MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "configFileError"));
                this.Close();
                return;
            }
            this._DbLevel = _DbLevel;
            this.Topmost = true;
            // Lese das aktuelle Datum aus XML Datei. Entscheide dann, ob ein Backup notwendig ist. (beim schließen des Programms)
            liste = _Timer.readXML(Environment.CurrentDirectory + @"\TimeStamp.xml","backup","date","iswrite");
            if (liste[0] == DateTime.Now.ToShortDateString().ToString() && liste[1] == "True" && _DbLevel != "restore") {
                this._DbLevel = "backupExist";
            }
            switch (this._DbLevel) { 
                case "backupExist":
                    this.Close();
                    break;
                default:
                    startApplication();
                    break;
            }  
        }
        #endregion
        #region Methods
        /// <summary>
        /// Methode zur Unterscheidung eines Backups oder einer Wiederherstellung(Labels).
        /// CheckUSB Klasse wird ausfegührt um USB Gerät zu erkennen.
        /// </summary>
        private void startApplication(){
            try
            {
                if (_Cusb.checkUSB())
                {
                    label1.Content = IniParser.GetSetting("RESTORE", "recogniseStorageMedia");
                }
                else {
                    decide_DbLevel(_DbLevel);
                }
            }
            catch (Exception exception) {
                isNotCorrectUSB(exception.Message);
            }
            
            if (this._DbLevel == "backup")
            {
                BitmapImage bi = new BitmapImage(new Uri(_DOWNLOAD_PIC, UriKind.Relative));
                image1.Source = bi;
            }
            if (this._DbLevel == "restore")
            {
                _MainWindowObj.IsEnabled = false;
                BitmapImage bi = new BitmapImage(new Uri(_UPLOAD_PIC, UriKind.Relative));
                image1.Source = bi;
            }
            changeLayout("startApplication");
        }
        
        /// <summary>
        /// Label Auswahl: Backup / Restore
        /// </summary>
        /// <param name="_DbLevel"></param>
        private void decide_DbLevel(string _DbLevel){
            switch (this._DbLevel)
            {
                case "backupExist":
                    break;
                case "backup":
                    label1.Content = IniParser.GetSetting("RESTORE", "connectStorageMediaToBackup");
                    break;
                case "restore":
                    label1.Content = IniParser.GetSetting("RESTORE", "connectStorageMediaToRestore");
                    break;
            }
        }

        /// <summary>
        /// Überarbeite das Layout, falls USB nicht gültig war.
        /// </summary>
        /// <param name="exception"></param>
        private void isNotCorrectUSB(String exception)
        {
            label1.Visibility = System.Windows.Visibility.Hidden;
            textBlock1.Visibility = System.Windows.Visibility.Visible;
            textBlock1.Foreground = Brushes.Red;
            textBlock1.Text = exception;
        }

        /// <summary>
        /// Verändere das derzeitiges Layout
        /// </summary>
        /// <param name="subLevel"></param>
        /// <param name="exception"></param>
        private void changeLayout(String subLevel, String exception = "")
        {
            switch (subLevel)
            {
                case "restore":
                    label1.Content = IniParser.GetSetting("RESTORE", "selectAFile");
                    listBox1.Visibility = System.Windows.Visibility.Visible;
                    btCancelProcess.Visibility = System.Windows.Visibility.Visible;
                    btRestore.Visibility = System.Windows.Visibility.Visible;
                    btRestore.IsEnabled = false;
                    break;
                case "restoreProcess":

                    textBlock1.Foreground = Brushes.Green;
                    textBlock1.Visibility = System.Windows.Visibility.Visible;
                    if (exception == "")
                    {
                        image2.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        image2.Visibility = System.Windows.Visibility.Hidden;
                    }
                    textBlock1.Text = IniParser.GetSetting("RESTORE", "dataRestored");
                    this.btCompleteWindow.Content = IniParser.GetSetting("RESTORE", "finish");
                    label1.Visibility = System.Windows.Visibility.Hidden;
                    btCancelProcess.Visibility = System.Windows.Visibility.Hidden;
                    btCompleteWindow.Visibility = System.Windows.Visibility.Visible;
                    listBox1.Visibility = System.Windows.Visibility.Hidden;
                    btRestore.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "backup":
                    textBlock1.Foreground = Brushes.Green;
                    textBlock1.Visibility = System.Windows.Visibility.Visible;
                    image2.Visibility = System.Windows.Visibility.Visible;
                    textBlock1.Text = IniParser.GetSetting("RESTORE", "backupReady");
                    this.btCompleteWindow.Content = IniParser.GetSetting("RESTORE", "finish");
                    label1.Visibility = System.Windows.Visibility.Hidden;
                    btCancelProcess.Visibility = System.Windows.Visibility.Hidden;
                    btCompleteWindow.Visibility = System.Windows.Visibility.Visible;
                    break;
                case "listBoxSelectionChanged":
                    btRestore.IsEnabled = true;
                    break;
                case "startApplication":
                    listBox1.Visibility = System.Windows.Visibility.Hidden;
                    btCompleteWindow.Visibility = System.Windows.Visibility.Hidden;
                    btCancelProcess.Visibility = System.Windows.Visibility.Hidden;
                    btRestore.Visibility = System.Windows.Visibility.Hidden;
                    image2.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "startDispatcherTimer":
                    label1.Visibility = System.Windows.Visibility.Visible;
                    textBlock1.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "isNotCorrectUSB":
                    label1.Visibility = System.Windows.Visibility.Hidden;
                    textBlock1.Visibility = System.Windows.Visibility.Visible;
                    textBlock1.Foreground = Brushes.Red;
                    textBlock1.Text = exception;
                    break;
            }
        }
        #endregion
        #region Events
        /// <summary>
        /// Starte den dispatcherTimer, wenn das USB Gerät angeschlossen wurde.
        /// Dieser ist für das permanente Suchen nach einem angeschlossenen
        /// USB - Geräten in regelmäßigen Sekunden-Abständen zuständig. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, _DISPATCHER_TIMER);
            dispatcherTimer.Start();
        }
        /// <summary>
        /// Die dispatcherTimer Methode; ruft die Backup oder die Restore Klasse auf.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            changeLayout("startDispatcherTimer");
            this._Counter += _ADD_COUNT_FACTOR;
            if (_Counter == _TIME_FACTOR_TO_ENABLED_BACKUP_CANCEL_BUTTON || this._DbLevel == "restore")
            {
                btCancelProcess.Visibility = System.Windows.Visibility.Visible;
            }
            try
            {
                if (_Cusb.checkUSB())
                {

                    KöTaf.Utils.BackupRestore.DriveLetter drivelet = _Cusb._DriveL;
                    String dl = drivelet._DriveLetter;
                    switch (this._DbLevel)
                    {
                        case "backupExist":
                            break;
                        case "backup":
                            dispatcherTimer.Stop();
                            try
                            {
                                br.StartBR("backupDB", dl + _USB_FOLDER_NAME);
                            }
                            catch (Exception ex)
                            {
                                textBlock1.Text = IniParser.GetSetting("ERRORMSG", "backupRuntimeError") + ex.ToString();
                            }
                            changeLayout("backup");
                            Timer timer = new Timer();
                            timer.writeToXML(Environment.CurrentDirectory + @"\TimeStamp.xml", "True", DateTime.Now.ToShortDateString(), "timeStamp/backup/date", "timeStamp/backup/iswrite");
                            break;

                        case "restore":
                            string[] FileList;
                            dispatcherTimer.Stop();
                            hList= drivelet.CheckSavedFiles(drivelet._DriveLetter);
                            //Rufe alle Backup Dateien von USB - Datenspeicher ab.
                            var Files = new DirectoryInfo(dl + _USB_FOLDER_NAME).GetFiles()
                                                                                 .OrderBy(f => f.CreationTime)
                                                                                 .ToArray();
                            for (int i = Files.Count() -1; i >= 0; i--)
                            {
                                try
                                {
                                    FileList = Files[i].ToString().Split(new Char[] { '.' });
                                    if (FileList[2].ToString() == this._FILENAME_DB_EXTENSION.ToString())
                                        listBox1.Items.Add(Files[i]);
                                }
                                catch 
                                {
                                    throw new Exception(IniParser.GetSetting("ERRORMSG", "falseFilesInFolder"));
                                }
                            }
                            changeLayout("restore");
                            break;
                    }
                    return;
                }
                decide_DbLevel(this._DbLevel);
            }
            catch (Exception ex)
            {
                isNotCorrectUSB(ex.Message);
            }
        }

        /// <summary>
        /// Falls ein gültiges USB - Gerät erkannt wurde, wird der dispatcher timer gestoppt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompleteWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _MainWindowObj.IsEnabled = true;
            }
            catch
            {
                //Falls MainWindow keine Intanz besitzt.
            }
            this.Close();
            dispatcherTimer.Stop();
        }

        /// <summary>
        /// Verlasse den USB Prozess
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Process_Click(object sender, RoutedEventArgs e)
        {
            if (_MainWindowObj != null)
                _MainWindowObj.IsEnabled = true;
            this.Close();
            dispatcherTimer.Stop();
        }

        /// <summary>
        /// Falls eine Wiederhestellungs - Datei angeklickt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changeLayout("listBoxSelectionChanged");        
        }

        /// <summary>
        ///Wiederherstellungs Methode.
        ///die Liste hList beinhaltet Keys, welche den kompletten Pfad zu der
        ///wiederherzustellenden Datei vom USB Gerät enthält. Die Values der hList sind 
        ///nur die Dateinamen ohne vollen Pfad. Es wird also das SelectItem mit dem Value verglichen.
        ///wenn diese übereinstimmen wird der dazugehörige Key aus hList herangezogen und als Übergabeparameter 
        ///in die BackupRestore Klasse übergeben. Damit wird die Datenbank durch die Backup Datei vom USB - Gerät
        ///ersetzt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRestoreFunc_Click(object sender, RoutedEventArgs e)
        {
            foreach (DictionaryEntry dictionaryEntry in hList)
            {     
                if (dictionaryEntry.Value.ToString() == listBox1.SelectedItem.ToString())
                    try
                    {
                        br.StartBR("restoreDB", dictionaryEntry.Key.ToString());
                    }
                    catch (Exception ex)
                    {
                        textBlock1.Foreground = Brushes.Red;
                        textBlock1.Text = IniParser.GetSetting("ERRORMSG", "restoreRuntimeError") + ex.Message;
                        this._FailDuringRestore = "1";
                        return;
                    }
                //changeLayout("restoreProcess",this._FailDuringRestore);
            }
            changeLayout("restoreProcess", this._FailDuringRestore);
        }

        /// <summary>
        /// Schliesse Fenster
        /// </summary>
        private void close(){
            
            this.Close();
        }
        #endregion  
    }
}
