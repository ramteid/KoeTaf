using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace KöTaf.Utils.Parser
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Beinhaltet Registry-Parsing-Funktionen
    /// </summary>
    public class RegistryParser
    {
        /// <summary>
        /// Hole den Installationspfad von LibreOffice
        /// </summary>
        /// <returns></returns>
        public static string getLibreOfficeInstallPath()
        {
            string path = "";
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", false);

                //Go through the registry keys and get OpenOffice.org
                foreach (string skName in rk.GetSubKeyNames())
                {
                    if (Regex.IsMatch(skName, "LibreOffice", RegexOptions.IgnoreCase))
                    {
                        RegistryKey installPath = rk.OpenSubKey(skName).OpenSubKey("UNO").OpenSubKey("InstallPath");
                        foreach (var name in installPath.GetValueNames())
                        {
                            if (Regex.IsMatch(name, "LibreOffice", RegexOptions.IgnoreCase))
                                path = (string)installPath.GetValue(name);
                        }
                    }
                }
            }
            catch
            { 
            }
            return path;
        }

    }
}
