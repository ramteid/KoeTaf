/**
 * Class: CheckForUSB
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-05-05
 * 
 * Last modification: 2013-06-17 / Michael Müller
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Management;

namespace KöTaf.Utils.BackupRestore
{
    /// <summary>
    /// Diese Klasse prüft ob es sich um den richtige gültige USB-Gerät anhand des Volumen-Labels
    /// handelt, und prüft ab, an welchen USB Port ein Gerät angeschlossen ist.
    /// </summary>
    public class CheckForUSB
    {
        public DriveLetter _DriveL;
        private String _VOLUME_LABEL;
        #region Contructor
        /// <summary>
        /// Konstruktor
        /// </summary>
        public CheckForUSB() {

            try
            {
                this._DriveL = new DriveLetter();
            }
            catch (Exception ex) {
                throw ex;    
            }
            //Rufe alle relevanten Daten aus der Konfigurationsdatei ab.
            try
            {
                this._VOLUME_LABEL = Parser.IniParser.GetSetting("USB", "VOLUME_LABEL");
            }
            catch {
                throw new Exception("Fehler in der Konfigurationsdatei (VOLUME_LABEL). ");
            }
            if (_VOLUME_LABEL == "") {
                throw new Exception("Fehler in der Konfigurationsdatei (VOLUME_LABEL).");
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Hier wird das neu beschriebene DriveLetter Objekt zurückgegeben
        /// Anwendung für die Klasse USBIdentification
        /// </summary>
        /// <returns></returns>
        public DriveLetter getDLObject() {
            return _DriveL;
        }
        /// <summary>
        /// Prüft auf richtigen USB Stick.
        /// </summary>
        /// <returns></returns>
        public bool checkUSB()
        {
            bool isUSBRegistered = true;
            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                if (GetConnectedUsbDrives(driveInfo.Name)) 
                {
                    _DriveL._DriveLetter = driveInfo.Name;
                    if (driveInfo.VolumeLabel == this._VOLUME_LABEL)   
                        return true;
                    isUSBRegistered = false;
                }

            }
            if (!isUSBRegistered) 
            { 
                throw new Exception("Sie haben kein gültiges TAFEL Speichermedium eingelegt. Bitte wechseln Sie das Speichermedium. Sobald ein korrektes Speichermedium angeschlossen wurde, wird der Vorgang automatisch fortgesetzt.");
            }
            return false;
        }

        /// <summary>
        /// Prüft ob USB Geräte angeschlossen sind 
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        public bool GetConnectedUsbDrives(String driveLetter)
        {
            List<string> usbDriveLetters = new List<string>();
            var usbDrives = new ManagementObjectSearcher("SELECT DeviceID FROM Win32_DiskDrive WHERE InterfaceType='USB'");
            foreach (ManagementObject drive in usbDrives.Get())
            {
                var partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"].ToString() + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
                foreach (var partition in partitions.Get())
                {
                    var logicalDrives = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"].ToString() + "'} WHERE AssocClass = Win32_LogicalDiskToPartition");
                    foreach (var logicalDrive in logicalDrives.Get())
                    {
                        usbDriveLetters.Add(logicalDrive["Caption"].ToString());
                    }
                }
            }
            foreach (var s in usbDriveLetters)
            {
                if (driveLetter.Contains(s))
                    return true;
            }
            return false;
        }
    }
        #endregion
   /// <summary>
   /// Prüft auf zeitliche Gültigkeit der Wiederherstellungs Dateien (Kann in der Config.ini eingestellt werden.),
   /// und gibt eine Liste von *.tafel Dateien zurück 
   /// </summary>
   public class DriveLetter{
        Timer timer = new Timer();
        Hashtable list = new Hashtable();
        private String _FILENAME_DB_EXTENSION;
        public String _DriveLetter{get;set;}
        private String _FileName = "";
        #region Contructor
        public DriveLetter() {
            try
            {
                this._FILENAME_DB_EXTENSION = Parser.IniParser.GetSetting("USB", "FILENAME_DB_EXTENSION").ToLower();
            }
            catch {
                throw new Exception("Fehler in der Konfigurationsdatei.");
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Prüfung auf zeitliche Gültigkeit, und gibt Liste mit Restore - Dateinamen zurück.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Hashtable CheckSavedFiles(String str) {
            try
            {
                foreach (string d in Directory.GetDirectories(str))
                {
                    foreach (string file in Directory.GetFiles(d))
                    {
                        if (file.Contains(this._FILENAME_DB_EXTENSION))
                        {
                            FileInfo f = new FileInfo(file);
                            if (timer.deleteBackup(f.CreationTime.Date))
                            {
                                _FileName = Path.GetFileName(file);
                                list.Add(file, _FileName);
                            }
                            else {
                                File.Delete(file);
                            }
                        }
                    }
                    CheckSavedFiles(d);
                }
                return list;
            }
            catch (System.Exception excpt)
            {
                throw new Exception("Problem with Class DriveLetters" + excpt.Message);
            }
        }
    }
    
}
#endregion