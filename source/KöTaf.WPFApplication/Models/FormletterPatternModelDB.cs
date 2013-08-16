using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Mit dieser Klasse wird ein Filter-Datenbankobjekt in ein reguläres Objekt konvertiert
    /// Terminologie: FormletterPattern = Serienbrief-Vorlage
    /// </summary>
    class FormletterPatternModelDB
    {
        // Liste aller filterSets zu diesem formletterPattern
        public List<FilterSetModel> filterSetModels { get; private set; }
        public List<FormletterTableAssignment> formletterTableAssignments { get; private set; }
        public List<FormletterColumnCsvDocumentAssignment> formletterColumnCsvDocumentAssignments { get; private set; }
        public string name { get; private set; }
        public string saluation_m { get; private set; }
        public string saluation_f { get; private set; }
        public string saluation_n { get; private set; }
        public string formletter_filename { get; private set; }
        public string letterText { get; private set; }
        public int pID { get; private set; }
        public string csv_filename { get; private set; }

        /// <summary>
        /// Erzeugt neues Filter-Datenbankobjekt anhand einer formletter_pattern-ID
        /// </summary>
        /// <param name="patternID"></param>
        public FormletterPatternModelDB(int patternID)
        {
            filterSetModels = new List<FilterSetModel>();
            //formletterTableAssignments = new List<FormletterTableAssignment>();
            formletterColumnCsvDocumentAssignments = new List<FormletterColumnCsvDocumentAssignment>();

            // Lese zurück
            IList<FormletterPattern> patterns = FormletterPattern.GetFormletterPatterns(patternID).ToList<FormletterPattern>();

            // Es gibt genau einen formletterPattern
            FormletterPattern pattern = patterns[0];

            // Strings füllen
            this.name = SafeStringParser.safeParseToStr(pattern.Name);
            this.saluation_m = SafeStringParser.safeParseToStr(pattern.SaluationM);
            this.saluation_f = SafeStringParser.safeParseToStr(pattern.SaluationF);
            this.saluation_n = SafeStringParser.safeParseToStr(pattern.SaluationNT);
            this.formletter_filename = SafeStringParser.safeParseToStr(pattern.FileName);
            this.csv_filename = SafeStringParser.safeParseToStr(pattern.FileName).Replace(".odt", ".csv");
            this.letterText = SafeStringParser.safeParseToStr(pattern.Text);

            // pattern ID
            pID = pattern.FormletterPatternID;

            // füge diesem formletterPattern alle filterSets hinzu
            IEnumerable<FilterSet> filterSets = FilterSet.GetFilterSets(null, pID);
            foreach (var set in filterSets)
            {
                // Hole die IDs alle Filter dieses FilterSets
                int fsID = set.FilterSetID;
                IEnumerable<Filter> filters = Filter.GetFilters(null, fsID);

                // Name des FilterSets
                FilterSetModel filterSetModel = new FilterSetModel(set.Linking);
                filterSetModel.name = set.Name;

                // füge diesem filterSet alle filter hinzu
                foreach (var filter in filters)
                {
                    FilterModel filterModel = new FilterModel();
                    // konvertiere die strings aus der datenbank in enums
                    filterModel.group = (FilterModel.Groups)Enum.Parse(typeof(FilterModel.Groups), filter.Table);
                    filterModel.criterion = (FilterModel.Criterions)Enum.Parse(typeof(FilterModel.Criterions), filter.Type);
                    filterModel.operation = (FilterModel.Operations)Enum.Parse(typeof(FilterModel.Operations), filter.Operation);
                    filterModel.value = SafeStringParser.safeParseToStr(filter.Value);
                    filterSetModel.filterList.Add(filterModel);
                }

                filterSetModels.Add(filterSetModel);
            }
            
            // Füge alle Spaltenverknüpfungen hinzu
            IEnumerable<ColumnAssignment> assignments = ColumnAssignment.GetColumnAssignments(null, pID);
            foreach (var assignment in assignments)
            {
                string csvCol = SafeStringParser.safeParseToStr(assignment.CsvColumn);
                string dbTable = SafeStringParser.safeParseToStr(assignment.DatabaseTable);
                string dbCol = SafeStringParser.safeParseToStr(assignment.DatabaseColumn);

                // parse diese strings in ihre Enum-Äquivalente
                FormletterTableAssignment.Groups eDbTable = (FormletterTableAssignment.Groups)Enum.Parse(typeof(FormletterTableAssignment.Groups), dbTable);
                FormletterTableAssignment.Fields eDbCol = (FormletterTableAssignment.Fields)Enum.Parse(typeof(FormletterTableAssignment.Fields), dbCol);

                // erstelle eine enum-basierte tabellen-spalten-verknüpfung damit
                FormletterTableAssignment fta = new FormletterTableAssignment(eDbTable, eDbCol);

                // jetzt haben wir ein komplettes csv-datenbank-spaltenverknüpfungs-objekt
                FormletterColumnCsvDocumentAssignment colAssignment = new FormletterColumnCsvDocumentAssignment(csvCol, fta);

                this.formletterColumnCsvDocumentAssignments.Add(colAssignment);
            }

        }

        /// <summary>
        /// Lösche diese Serienbrief-Vorlage aus der Datenbank
        /// </summary>
        public void deleteThisFormletterPattern()
        {
            // Zuerst die Spaltenzuweisungen
            IEnumerable<ColumnAssignment> assignments = ColumnAssignment.GetColumnAssignments(null, pID);
            foreach (var assignment in assignments)
            {
                int id = assignment.ColumnAssignmentID;
                ColumnAssignment.Delete(id);
            }

            // dann die FilterSets
            IEnumerable<FilterSet> filterSets = FilterSet.GetFilterSets(null, pID);
            foreach (var set in filterSets)
            {
                // Hole die IDs alle Filter dieses FilterSets
                int fsID = set.FilterSetID;
                IEnumerable<Filter> filters = Filter.GetFilters(null, fsID);

                foreach (var filter in filters)
                {
                    int fid = filter.FilterID;
                    Filter.Delete(fid);
                }

                FilterSet.Delete(fsID);
            }

            // dann das formletterPattern
            FormletterPattern.Delete(this.pID);
        }
    }
}
