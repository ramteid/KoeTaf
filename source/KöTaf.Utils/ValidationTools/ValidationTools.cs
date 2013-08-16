using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace KöTaf.Utils.ValidationTools
{
    /// <summary>
    /// Author: Antonios Fesenmeier
    /// Diese Klasse beinhält verschiedene Methoden zum Validieren der Benutzereingaben!
    /// Quelle: http://regexlib.com/
    /// </summary>
    public class ValidationTools
    {
        Dictionary<string, string> errorDict;
        private String _MOBILENO;
        private String _PHONE;
        private String _EMAIL;
        private String _PLZ;
        private String _NUMBER;
        private String _NAME;
        private String _DOUBLE;

        // Konstruktor
        public ValidationTools()
        {
            try
            {
                this._MOBILENO = Parser.IniParser.GetSetting("VALIDATION", "MOBILENO");
                this._PHONE = Parser.IniParser.GetSetting("VALIDATION", "PHONENO");
                this._EMAIL = Parser.IniParser.GetSetting("VALIDATION", "EMAIL");
                this._PLZ = Parser.IniParser.GetSetting("VALIDATION", "PLZ");
                this._NUMBER = Parser.IniParser.GetSetting("VALIDATION", "NUMBER");
                this._NAME = Parser.IniParser.GetSetting("VALIDATION", "NAME");
                this._DOUBLE = Parser.IniParser.GetSetting("VALIDATION", "DOUBLE");
            }
            catch
            {
                throw new Exception("Fehler in der Konfigurationsdatei (VALIDATION)");
            }
            if(_MOBILENO==null || _PHONE == null || _EMAIL==null ||_PLZ==null ||_NUMBER==null||_NAME==null||_DOUBLE==null)
                throw new Exception("Fehler in der Konfigurationsdatei (VALIDATION)");

            // Dictionary zum Speichern der Validierungsfehler!
            errorDict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Author: Antonios Fesenmeier
        /// Überprüft das angegebene Control auf eine gültige deutsche Telefonnummer
        /// Dabei kann eine alternative Fehlermeldung übergeben werden.
        /// </summary>
        /// <param name="controlElement"> Beschreibt das Feld in dem der Fehler aufgetreten ist</param>
        /// <param name="mobileNo"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public Boolean IsMobileNumber(String controlElement, String mobileNo, String errorMsg = "")
        {
            Regex checkMobileNo = new Regex(@_MOBILENO);

            if (!checkMobileNo.IsMatch(mobileNo))
            {
                if (!errorMsg.Equals(""))
                {
                    errorDict.Add(controlElement, errorMsg.ToString());
                    return false;
                }
                else
                {
                    errorDict.Add(controlElement, "Dies ist keine gültige Handy-Nr.!");
                    return false;
                }
            }
            else
                return true;

        }

        /// <summary>
        /// Überprüft das übergebene Feld auf eine gültige deutsche Telefonnummer
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="controlElement"></param>
        /// <param name="phoneNo"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public Boolean IsPhoneNumber(String controlElement, String phoneNo, String errorMsg = "")
        {
            Regex checkPhoneNo = new Regex(@_PHONE);

            if (!checkPhoneNo.IsMatch(phoneNo))
            {
                if (!errorMsg.Equals(""))
                {
                    errorDict.Add(controlElement, errorMsg.ToString());
                    return false;
                }
                else
                {
                    errorDict.Add(controlElement, "Dies ist keine Telefonnummer!");
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// Überprüft das angegebene Feld auf eine gültige Email-Adresse
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="controlElement"></param>
        /// <param name="email"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool IsEMail(String controlElement, String email, String errorMsg = "")
        {
            Regex checkEmail = new Regex(@_EMAIL);


            if (!checkEmail.IsMatch(email))
            {
                if (!errorMsg.Equals(""))
                {
                    errorDict.Add(controlElement, errorMsg.ToString());
                    return false;
                }
                else
                {
                    errorDict.Add(controlElement, "Dies ist keine E-Mail Adresse!");
                    return false;
                }
            }
            else
                return true;
        }


        /// <summary>
        /// Überprüft das angegebene Feld auf eine gültige deutsche PLZ
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="controlElement"></param>
        /// <param name="PLZ"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public bool IsPLZ(String controlElement, String PLZ, String errorMsg = "")
        {
            Regex checkZipCode = new Regex(@_PLZ);

            if (!checkZipCode.IsMatch(PLZ))
            {
                if (!errorMsg.Equals(""))
                {
                    errorDict.Add(controlElement, errorMsg.ToString());
                    return false;
                }
                else
                {

                    errorDict.Add(controlElement, "Bitte geben Sie eine korrekte Postleitzahl ein.");
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// Prüft ob das übergebene Value NullOrEmpty ist.
        /// Feld leer --> false (negativ Fall)
        /// Feld befüllt --> true (weil positiv)
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNullOrEmpty(String value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Author: Antonios Fesenmeier
        /// Überprüft Feld auf eine Ganzzahl
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsNumber(String value)
        {
            Regex checkNumber = new Regex(@_NUMBER);
            if (!checkNumber.IsMatch(value))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Überprüft das angegebene Feld auf einen gültigen Namen, dabei dürfen Namen keine Zahlen oder Sonderzeichen beinhalten
        /// und müssen aus 2 oder mehr Buchstaben bestehen
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="controlElement"></param>
        /// <param name="value"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public Boolean IsName(String controlElement, String value, String errorMsg = "")
        {
            Regex checkNames = new Regex(@_NAME);
            if (!checkNames.IsMatch(value))
            {
                if (errorMsg.Equals(""))
                {
                    errorDict.Add(controlElement, "Dieses Feld muss eine Zeichenfolge mit mind. zwei Buchstaben sein!");
                    return false;
                }
                else
                {
                    errorDict.Add(controlElement, errorMsg.ToString());
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// Überprüft den übergebenen Wert auf einen Double mit 2 Nachkommastellen
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="controlElement"></param>
        /// <param name="value"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public Boolean IsDouble(String controlElement, String value, String errorMsg = "")
        {
            //Regex checkDoubles = new Regex(@"\\d{0,*}\\,\\d{1,2}");
            Regex checkDoubles = new Regex(@_DOUBLE);
            if (!checkDoubles.IsMatch(value))
            {
                if (errorMsg.Equals(""))
                {
                    errorDict.Add(controlElement, "Dieses Feld darf nur einen Geldbetrag ohne Währungszeichen enthalten");
                    return false;
                }
                else
                {
                    errorDict.Add(controlElement, errorMsg.ToString());
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// Fügt dem ErrorDic eine neue Fehlermeldung hinzu
        /// Author: Antonios Fesenmeier 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="errorMsg"></param>
        public void addError(String fieldName, String errorMsg)
        {
            errorDict.Add(fieldName, errorMsg);
        }


        /// <summary>
        /// Gibt die Fehlermeldungen des Errordic zurück
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <returns>StringBuilder</returns>
        public StringBuilder getErrorMsg()
        {
            StringBuilder sb = new StringBuilder("Achtung, bei folgenden Feldern trat ein Konflikt auf!");
            sb.AppendLine();

            foreach (KeyValuePair<String, String> pair in errorDict)
            {
                sb.AppendLine();
                sb.AppendLine(pair.Key + ": " + pair.Value);
            }

            return sb;
        }
/// <summary>
/// Löscht ErrorDict
/// Author: Antonios Fesenmeier
/// </summary>
        public void clearSB()
        {
            errorDict.Clear();
        }


    }
}
