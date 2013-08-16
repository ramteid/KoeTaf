using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.Utils.Parser;
using KöTaf.Utils.FileOperations;
using KöTaf.DataModel;
using System.Windows;
using KöTaf.Utils.Printer;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Bietet Funktionen für den Druck von Formularen
    /// </summary>
    public class PrintForms
    {

        /// <summary>
        /// Druck des Kassenabschlussberichts
        /// </summary>
        /// <param name="cashClosureID">Kassenabschluss-ID</param>
        /// <param name="reprint">Nachdruck</param>
        public static void printCashClosureReport(int cashClosureID, bool reprint)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
            }

            CashClosureReport report;
            try
            {
                var reports = CashClosureReport.GetCashClosureReports(null, cashClosureID);
                report = reports.FirstOrDefault();
            }
            catch
            {
                return;
            }

            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string path = IniParser.GetSetting("DOCUMENTS", "path").Replace("%PROGRAMPATH%", currentDir) + "\\" + IniParser.GetSetting("DOCUMENTS", "cashClosureReport");

            List<string> toReplace = new List<string>();
            List<string> replaceSt = new List<string>();

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Nachdruck&gt;</text:placeholder>");
            if (reprint)
                replaceSt.Add(SafeStringParser.safeParseToStr(IniParser.GetSetting("DOCUMENTS", "reprint")));
            else
                replaceSt.Add("");

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kassenabschlussdatum&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(report.CashClosure.ClosureDate));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kassenabschlussnummer&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(report.CashClosure.CashClosureID));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Einnahmen&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToMoney(report.CashClosure.Revenue, true));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Ausgaben&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToMoney(report.CashClosure.Expense, true));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Saldo&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToMoney(report.CashClosure.Sum, true));

            string tmpFilePath;
            bool success = LibreOffice.replaceXMLstringInODT(path, toReplace, replaceSt, out tmpFilePath);
            if (success)
                LibreOffice.openWithWriter(tmpFilePath, true, true);
        }

        /// <summary>
        /// Druck der Kassenabrechnung
        /// </summary>
        /// <param name="cashClosureID">Kassenabschluss-ID</param>
        /// <param name="reprint">Nachdruck</param>
        public static void printCashClosureReceipt(int cashClosureID, bool reprint)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
            }

            CashClosureReceipt receipt;
            try
            {
                IEnumerable<CashClosureReceipt> receipts = CashClosureReceipt.GetCashClosureReceipts(null, cashClosureID);
                receipt = receipts.FirstOrDefault();
            }
            catch
            {
                return;
            }

            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string path = IniParser.GetSetting("DOCUMENTS", "path").Replace("%PROGRAMPATH%", currentDir) + "\\" + IniParser.GetSetting("DOCUMENTS", "cashClosureReceipt");

            List<string> toReplace = new List<string>();
            List<string> replaceSt = new List<string>();

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kassenabschlussdatum&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(receipt.CashClosure.ClosureDate));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Einnahmen&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToMoney(receipt.CashClosure.Revenue, true));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Ausgaben&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToMoney(receipt.CashClosure.Expense, true));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Saldo&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToMoney(receipt.CashClosure.Sum, true));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;abgerechnet_durch&gt;</text:placeholder>");
            try
            {
                // Hole CashClosure Objekt direkt aus der DB, da die Referenz von CashClosureReceipt auf CashClosure manchmal nicht gültig ist(?)
                CashClosure cashClosure = CashClosure.GetCashClosures(cashClosureID).FirstOrDefault();
                string username = cashClosure.ClosureUserAccount.Username;
                replaceSt.Add(SafeStringParser.safeParseToStr(username));
            }
            catch
            {
                replaceSt.Add("-");
            }

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Nachdruck&gt;</text:placeholder>");
            if (reprint)
                replaceSt.Add(SafeStringParser.safeParseToStr(IniParser.GetSetting("DOCUMENTS", "reprint")));
            else
                replaceSt.Add("");


            string tmpFilePath;
            bool success = LibreOffice.replaceXMLstringInODT(path, toReplace, replaceSt, out tmpFilePath);
            if (success)
                LibreOffice.openWithWriter(tmpFilePath, true, true);
        }

        /// <summary>
        /// Druck des Haftungsausschlusses
        /// </summary>
        /// <param name="personID">ID der Person</param>
        public static void printClientDisclaimer(int? personID = null)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
            }

            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string path = IniParser.GetSetting("DOCUMENTS", "path").Replace("%PROGRAMPATH%", currentDir) + "\\" + IniParser.GetSetting("DOCUMENTS", "disclaimer");

            List<string> toReplace = new List<string>();
            List<string> replaceSt = new List<string>();

            if (personID.HasValue)
            {
                Person person;
                try
                {
                    person = Person.GetPersons(personID).FirstOrDefault();
                }
                catch
                {
                    return;
                }

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Nachname&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(person.LastName));

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Vorname&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(person.FirstName));

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Ausweisnummer&gt;</text:placeholder>");
                if (person.TableNo.HasValue)
                    replaceSt.Add(SafeStringParser.safeParseToStr(person.TableNo));
                else
                    return;

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Datum&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(DateTime.Now));
            }
            else
            {
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Nachname&gt;</text:placeholder>");
                replaceSt.Add("");

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Vorname&gt;</text:placeholder>");
                replaceSt.Add("");

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Ausweisnummer&gt;</text:placeholder>");
                replaceSt.Add("");

                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Datum&gt;</text:placeholder>");
                replaceSt.Add("");

                toReplace.Add("<text:span text:style-name=\"T3\">, </text:span>");
                replaceSt.Add("");
            }

            string tmpFilePath;
            bool success = LibreOffice.replaceXMLstringInODT(path, toReplace, replaceSt, out tmpFilePath);
            if (success)
                LibreOffice.openWithWriter(tmpFilePath, true, true);
        }

        /// <summary>
        /// Druck des Aufnahmeformulars
        /// </summary>
        /// <param name="personID">ID der Person</param>
        public static void printClientEnrolmentForm(int personID)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
            }

            Person person;
            try
            {
                var persons = Person.GetPersons(personID).ToList();
                person = persons.FirstOrDefault();
            }
            catch
            {
                return;
            }

            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string path = IniParser.GetSetting("DOCUMENTS", "path").Replace("%PROGRAMPATH%", currentDir) + "\\" + IniParser.GetSetting("DOCUMENTS", "enrolmentForm");

            List<string> toReplace = new List<string>();
            List<string> replaceSt = new List<string>();
            int rowsChildren = 6;
            int rowsRevenues = 8;

            if (person == null)
                return;

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Vorname&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.FirstName));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Nachname&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.LastName));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Strasse&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.Street));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;PLZ&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.ZipCode));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Ort&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.City));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Geburtstag&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.DateOfBirth));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Staatsangehoerigkeit&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.Nationality));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Geburtsland&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.CountryOfBirth));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Familienstand&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.FamilyState.Name));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Vorname_Partner&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.MaritalFirstName));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Nachname_Partner&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.MaritalLastName));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Geburtstag_Partner&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.MaritalBirthday));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Staatsangehoerigkeit_Partner&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.MaritalNationality));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Datum&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(DateTime.Now));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Gueltigkeitsende&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.ValidityEnd));
            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;KNR&gt;</text:placeholder>");
            replaceSt.Add(SafeStringParser.safeParseToStr(person.TableNo));

            toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Erfasser&gt;</text:placeholder>");
            if (person.UserAccount != null)
                replaceSt.Add(SafeStringParser.safeParseToStr(person.UserAccount.Username));
            else
                replaceSt.Add("");

            // fülle Kinder
            IEnumerable<Child> children = Child.GetChildren(null, person.PersonID);
            int childIndex = 1;
            foreach (var child in children)
            {
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Index_" + childIndex + "&gt;</text:placeholder>");
                replaceSt.Add("#" + childIndex);
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Vorname_" + childIndex + "&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(child.FirstName));
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Nachname_" + childIndex + "&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(child.LastName));
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Geburtstag_" + childIndex + "&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(child.DateOfBirth));
                childIndex++;
            }
            // Sorge dafür, dass die übrigen Zeilen beim Druck leer sind
            for (int i = childIndex; i <= rowsChildren; i++)
            {
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Index_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Vorname_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Nachname_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Kind_Geburtstag_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
            }

            // fülle Einkünfte
            IEnumerable<Revenue> revenues = Revenue.GetRevenues(null, person.PersonID);
            int revenueIndex = 1;
            foreach (var revenue in revenues)
            {
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Einkunft_" + revenueIndex + "&gt;</text:placeholder>");
                replaceSt.Add(revenue.RevenueType.Name + ": " + revenue.Description);
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Bescheid_" + revenueIndex + "&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(revenue.StartDate));
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Bewilligt_" + revenueIndex + "&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToStr(revenue.EndDate));
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Hoehe_" + revenueIndex + "&gt;</text:placeholder>");
                replaceSt.Add(SafeStringParser.safeParseToMoney(revenue.Amount, true));
                revenueIndex++;
            }
            // Sorge dafür, dass die übrigen Zeilen beim Druck leer sind
            for (int i = revenueIndex; i <= rowsRevenues; i++)
            {
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Einkunft_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Bescheid_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Bewilligt_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
                toReplace.Add("<text:placeholder text:placeholder-type=\"text\">&lt;Hoehe_" + i + "&gt;</text:placeholder>");
                replaceSt.Add("");
            }

            string tmpFilePath;
            bool success = LibreOffice.replaceXMLstringInODT(path, toReplace, replaceSt, out tmpFilePath);
            if (success)
                LibreOffice.openWithWriter(tmpFilePath, true, true);
        }

        /// <summary>
        /// Druck des Berechtigungsnachweises
        /// </summary>
        public static void printCredential()
        {
            //Berechtigungsnachweis drucken
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
                return;
            }
            try
            {
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.LopOffList, null);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
            }
        }

        /// <summary>
        /// Druck des Caritas-Formulars
        /// </summary>
        public static void printCaritasForm()
        {
            string path = IniParser.GetSetting("DOCUMENTS", "path") + "\\CaritasDienste.odt";
            LibreOffice.openWithWriter(path, true, false);
        }

        /// <summary>
        /// Druck des Spendeneinreichers
        /// </summary>
        public static void printDonationsForm()
        {
            string path = IniParser.GetSetting("DOCUMENTS", "path") + "\\Spendeneinreicher.odt";
            LibreOffice.openWithWriter(path, true, false);
        }

    }

}
