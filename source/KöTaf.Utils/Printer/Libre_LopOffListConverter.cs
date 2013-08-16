/**
 * Class: Libre_LopOffListConverter
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-05
 * 
 * Last modification: 2013-05-24 / Michael Müller
 */
using System;
using System.Xml;
using CarlosAg.ExcelXmlWriter;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace KöTaf.Utils.Printer
{
    /// <summary>
    /// Klasse für das Erstellen einer XML Datei (Kalkulationssheet/ Berechtigungsausweis)
    /// für den Berechtigungsausweis.
    /// @note: Es existiert für das LIBRE_OFFICE ein VBA-Code welcher bei dem aktivieren 
    ///        des Dokuments geladen wird. Der Code selbst ist auch u.a. im Verzeichnis \src\LibreOffice\
    ///        zu finden.
    ///        Was macht der VBA CODE ?
    ///        a) Unterscheidung zwischen Spreadsheet und normales Dokument
    ///        b) Es dreht das Dokument (Hochformat / Querformat) falls notwendig.
    ///        c) Diese Notwendigkeit wird errechnet durch summieren der Spaltenbreite aller
    ///           beschriebenen Spalten.
    ///        d) Der Code ist verantwortlich dafür das der HEADER auf jeder nachfolgenden Seite wieder
    ///           mitangezeigt wird.
    /// </summary>
    public class Libre_LopOffListConverter
    {
        public String _Filename = Path.Combine(Environment.CurrentDirectory, "tmp.xml");
        public String _PATH_TO_CSV = Path.Combine(Environment.CurrentDirectory, "OO_CSV.csv");
        List<String> dataLine = new List<String>();
        Dictionary<String, int> dicDistance = new Dictionary<String, int>();
        private Worksheet _Sheet;
        private Workbook _Book;
        private String _SheetName, _HEADERCOLOR = "#e6e6e6";
        private int _EmptyLinesCounter = 0, _EMPTYLINES;
        #region Contructor
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="sheetName"></param>
        public Libre_LopOffListConverter(PrintType sheetName)
        {
            // Hole relevante Daten aus der Konfigurationsdatei.
            try
            {
                this._EMPTYLINES = Convert.ToInt32(Parser.IniParser.GetSetting("LIBRECONVERTER", "LOPOFFLIST_EMPTYLINES"));
            }
            catch { 
                throw new Exception("Fehler in der Konfigurationsdatei (LOPOFFLIST_EMPTYLINES).");
            }
            if (sheetName == PrintType.LopOffList)
                this._SheetName = "B E R E C H T I G U N G S N A C H W E I S";
            Generate(this._Filename);
            this.CSVReader();
            this.saveFileAndOpenOO();
        }
        #endregion
        #region Methods
        /// <summary>
        /// Lese die zuvor zusammengebaute CSV Datei mit Header und dazugehörigen Daten ein.
        /// </summary>
        private void CSVReader()
        {
            foreach (string line in File.ReadAllLines(_PATH_TO_CSV, Encoding.Default))
            {
                _SetEmptyRows(_EMPTYLINES);
                string[] parts = line.Split(';');
                foreach (string part in parts)
                {
                    dataLine.Add(part);
                }
                _SetEmptyCells(7);
                toXLS();
            }
        }
        /// <summary>
        /// Setze leere Zeilen in den Berechtigungsausweis;
        /// Wird benötigt damit TAFEL Angestellte, Datensätze von Hand ergänzen können.
        /// Diese Einstellung ist variabel und in der Config.ini abänderbar.
        /// </summary>
        /// <param name="EmptyLines"></param>
        private void _SetEmptyRows(int EmptyLines)
        {
            _EmptyLinesCounter += 1;
            if (_EmptyLinesCounter > 20-EmptyLines && _EmptyLinesCounter <= 20)
            {
                for (int i = 1; i <= EmptyLines; i++)
                {
                    _SetEmptyCells(13);
                    toXLS();
                }
                _EmptyLinesCounter = 1;
            }
        }

        /// <summary>
        /// Setze leere Zellen. Wird benötigt, um Styles (Boarders etc.)
        /// z.B. für Kalenderwochen, und Bemerkungen anzeigen zu lassen.
        /// </summary>
        /// <param name="Column"></param>
        private void _SetEmptyCells(int Column) {
            for (int i = 1; i <= Column; i++)
            {
                dataLine.Add("");
            }
        }
        /// <summary>
        /// Erzeuge neues ZeilenObjekt
        /// </summary>
        /// <returns></returns>
        private WorksheetRow makeRow() {
            WorksheetRow row = this._Sheet.Table.Rows.Add();
            return row;

        }
        /// <summary>
        /// Fülle das Calc - Sheet mit Daten
        /// </summary>
        public void toXLS()
        {
            WorksheetRow row = makeRow();
            row.Height = 20;
            for (int i = 0; i <= this.dataLine.Count - 1; i++)
            {
                //row.Index = i;
                row.Height = 20;
                row.Cells.Add(new WorksheetCell(this.dataLine[i].ToString(), "ce11"));
            }
            this.dataLine.Clear();
        }

        /// <summary>
        /// Baue die XML Datei zusammen
        /// </summary>
        /// <param name="filename"></param>
        public void Generate(string filename) {
            Workbook book = new Workbook();
            this._Book = book;
            book.ExcelWorkbook.WindowHeight = 9000;
            book.ExcelWorkbook.WindowWidth = 13860;
            book.ExcelWorkbook.WindowTopX = 240;
            book.ExcelWorkbook.WindowTopY = 75;
            book.ExcelWorkbook.ProtectWindows = false;
            book.ExcelWorkbook.ProtectStructure = false;
            // -----------------------------------------------
            //  Generiere Styles
            // -----------------------------------------------
            this.GenerateStyles(book.Styles);
            // -----------------------------------------------
            //  Generiere den Berechtigungsausweis Worksheet
            // -----------------------------------------------
            this.GenerateWorksheetAbhackeListe(book.Worksheets);
        }
        
        /// <summary>
        /// Erstelle die Layout Styles
        /// </summary>
        /// <param name="styles"></param>
        private void GenerateStyles(WorksheetStyleCollection styles) {
            // -----------------------------------------------
            //  Allgemein
            // -----------------------------------------------
            WorksheetStyle Default = styles.Add("Default");
            Default.Name = "Default";
            // -----------------------------------------------
            //  Ergebnis1
            // -----------------------------------------------
            WorksheetStyle Result = styles.Add("Result");
            Result.Name = "Result";
            Result.Font.Bold = true;
            Result.Font.Italic = true;
            Result.Font.Underline = UnderlineStyle.Single;
            Result.Font.Size = 10;
            // -----------------------------------------------
            //  Ergebnis2
            // -----------------------------------------------
            WorksheetStyle Result2 = styles.Add("Result2");
            Result2.Name = "Result2";
            Result2.Font.Bold = true;
            Result2.Font.Italic = true;
            Result2.Font.Underline = UnderlineStyle.Single;
            Result2.Font.Size = 10;
            // -----------------------------------------------
            //  Überschrift
            // -----------------------------------------------
            WorksheetStyle Heading = styles.Add("Heading");
            Heading.Name = "Heading";
            Heading.Font.Bold = true;
            Heading.Font.Italic = true;
            Heading.Font.Size = 11;
            // -----------------------------------------------
            //  Überschrift2
            // -----------------------------------------------
            WorksheetStyle Heading1 = styles.Add("Heading1");
            Heading1.Name = "Heading1";
            Heading1.Font.Bold = true;
            Heading1.Font.Italic = true;
            Heading1.Font.Size = 11;
            // -----------------------------------------------
            //  Normales Layout
            // -----------------------------------------------
            WorksheetStyle Normal = styles.Add("Normal");
            Normal.Name = "Normal";
            Normal.Font.FontName = "Tahoma1";
            Normal.Font.Size = 11;
            // -----------------------------------------------
            //  Spalte1
            // -----------------------------------------------
            WorksheetStyle co1 = styles.Add("co1");
            // -----------------------------------------------
            //  Spalte2
            // -----------------------------------------------
            WorksheetStyle co2 = styles.Add("co2");
            // -----------------------------------------------
            //  Spalte3
            // -----------------------------------------------
            WorksheetStyle co3 = styles.Add("co3");
            // -----------------------------------------------
            //  Spalte4
            // -----------------------------------------------
            WorksheetStyle co4 = styles.Add("co4");
            // -----------------------------------------------
            //  Spalte5
            // -----------------------------------------------
            WorksheetStyle co5 = styles.Add("co5");
            // -----------------------------------------------
            //  Spalte6
            // -----------------------------------------------
            WorksheetStyle co6 = styles.Add("co6");
            // -----------------------------------------------
            //  Spalte7
            // -----------------------------------------------
            WorksheetStyle co7 = styles.Add("co7");
            // -----------------------------------------------
            //  Spalte8
            // -----------------------------------------------
            WorksheetStyle co8 = styles.Add("co8");
            // -----------------------------------------------
            //  Spalte9
            // -----------------------------------------------
            WorksheetStyle co9 = styles.Add("co9");
            // -----------------------------------------------
            //  Spalte10
            // -----------------------------------------------
            WorksheetStyle co10 = styles.Add("co10");
            // -----------------------------------------------
            //  ta1
            // -----------------------------------------------
            WorksheetStyle ta1 = styles.Add("ta1");
            // -----------------------------------------------
            //  neuer Style "normal"
            // -----------------------------------------------
            WorksheetStyle ce1 = styles.Add("ce1");
            ce1.Parent = "Normal";
            ce1.Font.Bold = true;
            ce1.Font.FontName = "Tahoma1";
            ce1.Font.Size = 11;
            ce1.Interior.Color = _HEADERCOLOR;
            ce1.Interior.Pattern = StyleInteriorPattern.Solid;
            ce1.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            ce1.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce1.Alignment.WrapText = true;
            ce1.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle2
            // -----------------------------------------------
            WorksheetStyle ce2 = styles.Add("ce2");
            ce2.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            ce2.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce2.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            ce2.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle3
            // -----------------------------------------------
            WorksheetStyle ce3 = styles.Add("ce3");
            ce3.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            ce3.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce3.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle4
            // -----------------------------------------------
            WorksheetStyle ce4 = styles.Add("ce4");
            ce4.Parent = "Normal";
            ce4.Font.Bold = true;
            ce4.Font.FontName = "Tahoma1";
            ce4.Font.Size = 11;
            ce4.Interior.Color = _HEADERCOLOR;
            ce4.Interior.Pattern = StyleInteriorPattern.Solid;
            ce4.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            ce4.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce4.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle5
            // -----------------------------------------------
            WorksheetStyle ce5 = styles.Add("ce5");
            ce5.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            ce5.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce5.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            ce5.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle6
            // -----------------------------------------------
            WorksheetStyle ce6 = styles.Add("ce6");
            ce6.Parent = "Normal";
            ce6.Font.Bold = true;
            ce6.Font.FontName = "Tahoma1";
            ce6.Font.Size = 11;
            ce6.Interior.Color = _HEADERCOLOR;
            ce6.Interior.Pattern = StyleInteriorPattern.Solid;
            ce6.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            ce6.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce6.Alignment.WrapText = true;
            ce6.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            ce6.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle7
            // -----------------------------------------------
            WorksheetStyle ce7 = styles.Add("ce7");
            ce7.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            ce7.Alignment.Vertical = StyleVerticalAlignment.Center;
            // -----------------------------------------------
            //  WorksheetStyle8
            // -----------------------------------------------
            WorksheetStyle ce8 = styles.Add("ce8");
            ce8.Font.Bold = true;
            ce8.Font.FontName = "Tahoma";
            ce8.Font.Size = 11;
            ce8.Interior.Color = _HEADERCOLOR;
            ce8.Interior.Pattern = StyleInteriorPattern.Solid;
            ce8.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            ce8.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce8.Alignment.WrapText = true;
            ce8.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            ce8.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            ce8.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "#000000");
            ce8.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "#000000");
            // -----------------------------------------------
            //  WorksheetStyle9
            // -----------------------------------------------
            WorksheetStyle ce9 = styles.Add("ce9");
            ce9.Parent = "Normal";
            ce9.Font.Bold = true;
            ce9.Font.FontName = "Tahoma1";
            ce9.Font.Size = 11;
            ce9.Interior.Color = _HEADERCOLOR;
            ce9.Interior.Pattern = StyleInteriorPattern.Solid;
            ce9.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            ce9.Alignment.Vertical = StyleVerticalAlignment.Center;
            ce9.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            ce9.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            //
            // WorksheetStyle10
            //
            WorksheetStyle cell10 = styles.Add("ce10");
            cell10.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            cell10.Alignment.Vertical = StyleVerticalAlignment.Center;
            cell10.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
            //
            // WorksheetStyle11
            //
            WorksheetStyle cell11 = styles.Add("ce11");
            cell11.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "#000000");
            cell11.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            //cell11.Alignment.WrapText = true;
            cell11.Alignment.Vertical = StyleVerticalAlignment.Center;
            cell11.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "#000000");
        }
        
        /// <summary>
        /// Formatiere das Dokument
        /// </summary>
        /// <param name="sheets"></param>
        private void GenerateWorksheetAbhackeListe(WorksheetCollection sheets) {
            Worksheet sheet = sheets.Add(this._SheetName + "   vom   " + DateTime.Now.ToLongDateString());
            this._Sheet = sheet;
            sheet.Table.StyleID = "ta1";
            WorksheetColumn column0 = sheet.Table.Columns.Add(58);
            WorksheetColumn column1 = sheet.Table.Columns.Add(60);
            WorksheetColumn column2 = sheet.Table.Columns.Add(48);
            WorksheetColumn column3 = sheet.Table.Columns.Add();
            column3.Width = 120;
            WorksheetColumn column44 = sheet.Table.Columns.Add();
            column44.Width = 80;
            WorksheetColumn column4 = sheet.Table.Columns.Add();
            column4.Width = 64;
            WorksheetColumn column7 = sheet.Table.Columns.Add();
            column7.Width = 20;
            WorksheetColumn column8 = sheet.Table.Columns.Add();
            column8.Width = 20;
            WorksheetColumn column9 = sheet.Table.Columns.Add();
            column9.Width = 20;
            WorksheetColumn column10 = sheet.Table.Columns.Add();
            column10.Width = 20;
            WorksheetColumn column11 = sheet.Table.Columns.Add();
            column11.Width = 20;
            WorksheetColumn column12 = sheet.Table.Columns.Add();
            column12.Width = 20;
            WorksheetColumn column13 = sheet.Table.Columns.Add();
            column13.Width = 170;  

            // Generierung Daten Zellen
            WorksheetRow Row0 = sheet.Table.Rows.Add();
            Row0.Height = 18;
            Row0.AutoFitHeight = true;
            WorksheetCell cell;
           
            cell = Row0.Cells.Add();
            cell.StyleID = "ce1";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Ausweis Nummer";
            cell.MergeDown = 1;
            
            cell = Row0.Cells.Add();
            cell.StyleID = "ce1";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Personen Haushalt";
            cell.MergeDown = 1;

            cell = Row0.Cells.Add();
            cell.StyleID = "ce1";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Gruppe";
            cell.MergeDown = 1;
            
            cell = Row0.Cells.Add();
            cell.StyleID = "ce1";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Name";
            cell.MergeDown = 1;
            
            cell = Row0.Cells.Add();
            cell.StyleID = "ce1";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Ort";
            cell.MergeDown = 1;
            
            cell = Row0.Cells.Add();
            cell.StyleID = "ce6";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Ausweis gültig bis";
            cell.MergeDown = 1;
            
            cell = Row0.Cells.Add();
            cell.StyleID = "ce8";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Wochen";
            cell.MergeAcross = 5;
            
            cell = Row0.Cells.Add();
            cell.StyleID = "ce9";
            cell.Data.Type = DataType.String;
            cell.Data.Text = "Bemerkungen";
            cell.MergeDown = 1;
            cell = Row0.Cells.Add();
            cell.Index = 1024;
            
            // -----------------------------------------------
           
            WorksheetRow Row1 = sheet.Table.Rows.Add();
            Row1.Height = 12;
            Row1.AutoFitHeight = false;
            // Hole Kalenderwochen, für die nächsten 6 Wochen aus Klasse: CalendarWeekDays, und
            // schreibe diese in das DataSheet
            for (int rows = 1; rows <= 6; rows++)
            {
                Row1.Cells.Add();
            }
            CalendarWeekDays cal = new CalendarWeekDays();
            List<int> weekList = new List<int>();
            weekList = cal.NumberOfWeek(DateTime.Now);
            for (int week = 0; week <= 5; week++)
            {
                Row1.Cells.Add(weekList[week].ToString(), DataType.Number, "ce10");
            }
            cell.Index = 1016;
            // -----------------------------------------------
            //  Optionen
            // -----------------------------------------------
            sheet.Options.ProtectObjects = false;
            sheet.Options.ProtectScenarios = false;
        }
        
        /// <summary>
        /// Speichere LibreOffice XML Datei ab.
        /// </summary>
        public void saveFileAndOpenOO()
        {
            List<String> list = new List<String>();
            Process[] pp = Process.GetProcessesByName("soffice.bin");
            foreach (Process p in pp)
            {
                p.CloseMainWindow();// normal end
                p.Kill();
                System.Threading.Thread.Sleep(2000);
            }
            _Book.Save(_Filename);
            // Starte die generierte Datei 
            string pfad = Utils.Parser.RegistryParser.getLibreOfficeInstallPath() + "\\scalc.exe";
            string argument = _Filename;
            Process.Start(pfad, argument);
        }
    }
}
#endregion