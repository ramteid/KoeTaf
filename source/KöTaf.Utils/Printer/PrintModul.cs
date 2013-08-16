/**
 * Class: PrintModul
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-05-15
 * 
 * Last modification: 2013-06-21 / Michael Müller
 */
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using KöTaf.DataModel;
using System.Linq;


namespace KöTaf.Utils.Printer
{
    /// <summary>
    /// Klasse für das Druck- Handeln der Daten welche aus verschiedenen 
    /// Datagrids des Programms kommen. Aus den Daten wird ei
    /// ne CSV Datei erzeugt.
    /// Ein jeweiliger LIBRE_Office-Converter wird ausgewählt.
    /// LIBRE_OFFICE wird gestartet.
    /// </summary>
    public class PrintModul
    {
        public static String PATH_TO_CSV = Path.Combine(Environment.CurrentDirectory, "OO_CSV.csv");
        private Enum dataGridInfo = null;
        #region Contructor
        public PrintModul(PrintType dataGridInfo, dynamic detail = null)
        {
            
            this.dataGridInfo = dataGridInfo;
            PrintType ch = dataGridInfo;
            // Entscheide was gedruckt werden soll
            switch (ch)
            {
                case PrintType.Team: 
                     this.CheckCSV();
                     Teams teams = new Teams(detail);
                     break;
                case PrintType.Sponsor:
                     this.CheckCSV();
                     Sponsor sponsor = new Sponsor(detail);
                     break;
                case PrintType.Client:
                     this.CheckCSV();
                     Client customer = new Client(detail);
                     break;
                case PrintType.LopOffList:
                     this.CheckCSV();
                     LopOffList lopOff = new LopOffList();
                     break;
                case PrintType.Statistic:
                     KöTaf.Utils.Printer.CSVExporter csv = new Utils.Printer.CSVExporter(detail);
                     var header = csv.GetHeader();
                     var content = csv.GetData();
                     var csvFull = csv.GetCsv();
                     csv.Write();
                     try
                     {
                         Libre_TeSpClConverter ooConv = new Libre_TeSpClConverter(PrintType.Statistic);  
                     }
                     catch (Exception ex) {
                         throw ex;
                     } break;
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// Generiere neue CSV - Datei.
        /// </summary>
        private void CheckCSV()
        {
            if (File.Exists(PATH_TO_CSV))
            {
                File.Delete(PATH_TO_CSV);
                var file = File.Create(PATH_TO_CSV);
                file.Close();
            }
            using (var sw = new StreamWriter(PATH_TO_CSV, true, Encoding.Default))
                sw.Close();
        }

        /// <summary>
        /// Schreibe die CSV Datei mit allen relevanten Daten.
        /// </summary>
        /// <param name="HeaderLine"></param>
        /// <param name="DataLine"></param>
        /// <param name="wasWritten"></param>
        public static void WriterCSV(String HeaderLine = "", String DataLine="", bool wasWritten=false)
        {
            using (var sw = new StreamWriter(PATH_TO_CSV, true, Encoding.Default))
            {
                if ((!wasWritten) && HeaderLine != "")
                {
                    sw.WriteLine(HeaderLine);
                }
                sw.WriteLine(DataLine);
            }
        }
        #endregion
        /// <summary>
        /// Erzeuge eine Vorlage für Rubrik Teams
        /// </summary>
        class Teams
        {
            public String HeaderLine = "Name;Postleitzahl;Wohnort;Straße;Aktiv;Funktion";
            String DataLine = "";
            String dL = ";";
            private bool wasWritten = false;
            #region Contructor
            public Teams(dynamic dG)
            
            {
                foreach (var look in dG)
                {
                    var ts = look as KöTaf.DataModel.Team;
                    DataLine = String.Concat(ts.FullName, dL, ts.ZipCode, dL,
                        ts.City, dL, ts.Street, dL, ts.IsActive.ToString(), dL, ts.TeamFunction.Name);
                    PrintModul.WriterCSV(HeaderLine, DataLine, this.wasWritten);
                    this.wasWritten = true;
                }
                this.wasWritten = false;
                try
                {
                    Libre_TeSpClConverter ooConv = new Libre_TeSpClConverter(PrintType.Team);
                }
                catch (Exception ex) {
                    throw ex;
                }
            }
        }
        #endregion
        /// <summary>
        /// Erzeuge eine Vorlage für Rubrik Kunden
        /// </summary>
        class Client
        {
            public String HeaderLine = "Gruppe;Ausweis Nummer;Name;PLZ;Wohnort;Straße;Geburtsdatum;Nationalität";
            String DataLine = "";
            String dL = ";";
            private bool wasWritten = false;
            #region Contructor
            public Client(dynamic dG)
            {
                foreach (var look in dG)
                {
                    var ts = look as KöTaf.DataModel.Person;
                    DataLine = String.Concat(ts.Group,dL,ts.TableNo, dL, ts.FullName, dL,
                        ts.ZipCode, dL, ts.City, dL, ts.Street, dL, ts.DateOfBirth.ToShortDateString(), dL, ts.Nationality);
                    PrintModul.WriterCSV(HeaderLine, DataLine, this.wasWritten);
                    this.wasWritten = true;
                }
                this.wasWritten = false;
                try
                {
                    Libre_TeSpClConverter ooConv = new Libre_TeSpClConverter(PrintType.Client);
                }
                catch (Exception ex) {
                    throw ex;
                }
            }
        }
        #endregion
        /// <summary>
        /// Erzeuge eine Vorlage für Rubrik Sponsor
        /// </summary>
        class Sponsor
        {
            public String HeaderLine = "Name;Postleitzahl;Wohnort;Straße;Aktiv;Typ";
            String DataLine = "";
            String dL = ";";
            private bool wasWritten = false;
            #region Contructor
            public Sponsor(dynamic dG)
            {
                foreach (var look in dG)
                {
                    var ts = look as KöTaf.DataModel.Sponsor;
                    DataLine = String.Concat(ts.FullName, dL,
                        ts.ZipCode, dL, ts.City, dL, ts.Street, dL, ts.IsActive.ToString(),dL, ts.FundingType.Name);
                    PrintModul.WriterCSV(HeaderLine, DataLine,this.wasWritten);
                    this.wasWritten = true;
                }
                this.wasWritten = false;
                try
                {
                    Libre_TeSpClConverter ooConv = new Libre_TeSpClConverter(PrintType.Sponsor);
                }
                catch (Exception ex) {
                    throw ex;
                }
            }
        }
        #endregion
        /// <summary>
        /// Erzeuge eine Vorlage für Rubrik Berechtigungsasuweis.
        /// </summary>
        class LopOffList
        {
            String DataLine = "";
            String dL = ";";
            private bool wasWritten = false;
            private IEnumerable<Person> _Persons;
            #region Contructor
            public LopOffList()
            {
                _Persons = Person.GetPersons();
                
                foreach (var look in _Persons)
                {
                    var ts = look as KöTaf.DataModel.Person;
                    DataLine = String.Concat(ts.TableNo,dL,ts.NumberOfPersonInHousholdString,dL,
                        ts.Group.ToString(), dL, ts.FullName,dL,ts.City,dL,ts.ValidityEnd.ToShortDateString(),"","");
                    PrintModul.WriterCSV("", DataLine,this.wasWritten);
                    this.wasWritten = true;
                }
                    this.wasWritten = false;
                    try
                    {
                        Libre_LopOffListConverter LopOff = new Libre_LopOffListConverter(PrintType.LopOffList);
                    }
                    catch (Exception ex) {
                        throw ex;
                    }
                }
        }
        }
    }
#endregion