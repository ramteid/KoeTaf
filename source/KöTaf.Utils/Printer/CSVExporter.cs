using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace KöTaf.Utils.Printer
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Klasse CSVExporter: Um ein dynamisches DataGrid als CSV zu exportieren
    /// </summary>
    public class CSVExporter
    {
        /// <summary>
        /// Trennzeichen der CSV Datei
        /// </summary>
        const char CSV_DELIMITOR = ';';
        /// <summary>
        /// Ausgabe Dateiname
        /// </summary>
        private string _OutputFileName;
        /// <summary>
        /// OpenOffice Pfad
        /// </summary>
        private string _OpenOfficePath;
        /// <summary>
        /// Daten
        /// </summary>
        private readonly IEnumerable<KeyValuePair<string, string>> _Data;
        /// <summary>
        /// Teiler
        /// </summary>
        private int _Divider;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Die Daten als KeyValue Liste</param>
        /// <param name="outputFile">OPtionaler Ausgabe Dateiname</param>
        /// <param name="openofficePath">Optionaler OpenOffice Pfad</param>
        public CSVExporter(IEnumerable<KeyValuePair<string, string>> data, string outputFile = null, string openofficePath = null)
        {
            this._Data = data;
            this._OutputFileName = outputFile;
            this._OpenOfficePath = openofficePath;
            this._Divider = 0;

            Init();
        }

        /// <summary>
        /// Initialisierung 
        /// </summary>
        private void Init()
        {
            if (string.IsNullOrEmpty(_OutputFileName))
                _OutputFileName = string.Concat(Environment.CurrentDirectory, @"\", "OO_CSV.csv");

            if (string.IsNullOrEmpty(_OpenOfficePath))
                _OpenOfficePath = @"C:\Program Files (x86)\OpenOffice.org 3\program\soffice.exe";
            else
            {
                //  if (!File.Exists(_OpenOfficePath))
                //      throw new Exception("OpenOffice cannot found");
            }
        }

        /// <summary>
        /// Gibt den CSV Header zurück
        /// </summary>
        /// <returns></returns>
        public string GetHeader()
        {
            if (this._Data != null)
            {
                this._Divider = 0;

                IList<string> header = new List<string>();
                this._Data.Select(d => d.Key).ToList()
                    .ForEach(d =>
                    {
                        if (!header.Contains(d))
                        {
                            _Divider = _Divider + 1;
                            header.Add(d);
                        }
                    });
                var sHeader = string.Join(CSV_DELIMITOR.ToString(), header);
                return sHeader;
            }
            return null;
        }

        /// <summary>
        /// Gibt die Daten als CSV zurück
        /// </summary>
        /// <returns></returns>
        public string GetData()
        {
            if (this._Data != null)
            {
                StringBuilder data = new StringBuilder();

                int header = _Divider;
                int divider = _Divider;
                var dataList = this._Data.ToList();

                for (int i = 0; i < dataList.Count; i++)
                {
                    var currentData = dataList[i];

                    //   if ((i == divider) && (i > 1))
                    if ((i == divider))
                    {
                        data.AppendLine();
                        divider = divider + header;
                    }

                    if (i == divider - 1)
                    {
                        data.Append(string.Format("{0}", currentData.Value));
                    }
                    else
                    {
                        data.Append(string.Format("{0}{1}", currentData.Value, CSV_DELIMITOR));
                    }

                }

                return data.ToString();
            }
            return null;
        }

        /// <summary>
        /// Gibt die komplette CSV zurück
        /// </summary>
        /// <returns></returns>
        public string GetCsv()
        {
            var csvHeader = GetHeader();
            var csvData = GetData();
            return string.Concat(csvHeader, Environment.NewLine, csvData);
        }

        /// <summary>
        /// Schreibt die Daten in eine CSV-Datei
        /// </summary>
        public void Write()
        {
            var data = GetCsv();
            // Datei überschreiben wenn existiert, andernfalls erzeugen
            using (var writer = new StreamWriter(_OutputFileName, false, System.Text.Encoding.UTF8, 512))
            {
                writer.Write(data);
            }
        }

        /// <summary>
        /// Öffnet die Datei
        /// </summary>
        public void OpenFile()
        {
            FileInfo fi = new FileInfo(_OpenOfficePath);

            Process proc = Process.Start(new ProcessStartInfo()
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                WorkingDirectory = fi.DirectoryName, // OO Verzeichnis
                FileName = fi.Name,          // OO Exe 
                Arguments = _OutputFileName
            });

            proc.Close();
        }
    }
}
