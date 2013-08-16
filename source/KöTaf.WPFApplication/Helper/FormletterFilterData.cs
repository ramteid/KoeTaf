using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.Parser;


namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Filtere die durch FilterSets festgelegten Empfängergruppen und erzeuge CSV-Daten
    /// </summary>
    class FormletterFilterData
    {
        private int pID;
        private FormletterPatternModelDB formletterPatternModel;
        private IEnumerable<Person> filteredPersons;
        private IEnumerable<Sponsor> filteredSponsors;
        private IEnumerable<Team> filteredTeamMembers;
        private SortedList<string, List<string>> csv_columns;
        private int columnHeight;

        /// <summary>
        /// Binde Instanz an formletter_pattern-ID
        /// </summary>
        /// <param name="formletterPatternID"></param>
        public FormletterFilterData(int formletterPatternID)
        {
            formletterPatternModel = new FormletterPatternModelDB(formletterPatternID);
            this.pID = formletterPatternID;

            filterDataSets();
            csv_columns = getColumns();
        }

        /// <summary>
        /// leerer Konstruktor für Listen-Modul
        /// </summary>
        public FormletterFilterData()
        {
        }

        /// <summary>
        /// Filtere alle Datensätze
        /// </summary>
        private void filterDataSets()
        {
            // Berücksichtige Serienbrief-Erlaubt-Flag
            IEnumerable<Person> dbPersons = Person.GetPersons();
            IEnumerable<Sponsor> dbSponsors = Sponsor.GetSponsors().Where(s => s.IsFormLetterAllowed);
            IEnumerable<Team> dbTeamMembers = Team.GetTeams().Where(t => t.IsFormLetterAllowed);

            List<Person> allPersons = new List<Person>();
            List<Sponsor> allSponsors = new List<Sponsor>();
            List<Team> allTeamMembers = new List<Team>();

            List<FilterSetModel> listOfFilterSetModels = formletterPatternModel.filterSetModels;

            filterRecordsByFilterSets(listOfFilterSetModels, dbPersons, dbSponsors, dbTeamMembers, ref allPersons, ref allSponsors, ref allTeamMembers);

            this.filteredPersons = allPersons;
            this.filteredSponsors = allSponsors;
            this.filteredTeamMembers = allTeamMembers;
        }

        /// <summary>
        /// Filtere Datensätze anhand von FilterSets
        /// </summary>
        /// <param name="listOfFilterSetModels">Liste mit FilterSetModel</param>
        /// <param name="dbPersons">Personen</param>
        /// <param name="dbSponsors">Sponsoren</param>
        /// <param name="dbTeamMembers">Teammitglieder</param>
        /// <param name="allPersons">Referenz zum Zurückschreiben der gefilterten Personen</param>
        /// <param name="allSponsors">Referenz zum Zurückschreiben der gefilterten Sponsoren</param>
        /// <param name="allTeamMembers">Referenz zum Zurückschreiben der gefilterten Teammitglieder</param>
        public void filterRecordsByFilterSets(List<FilterSetModel> listOfFilterSetModels, IEnumerable<Person> dbPersons, IEnumerable<Sponsor> dbSponsors, IEnumerable<Team> dbTeamMembers, ref List<Person> allPersons, ref List<Sponsor> allSponsors, ref List<Team> allTeamMembers)
        {

            List<List<Person>> personLists = new List<List<Person>>();
            List<List<Sponsor>> sponsorLists = new List<List<Sponsor>>();
            List<List<Team>> teamMemberLists = new List<List<Team>>();

            foreach (var set in listOfFilterSetModels)
            {
                List<Person> tmpP = dbPersons.ToList();
                List<Sponsor> tmpS = dbSponsors.ToList();
                List<Team> tmpT = dbTeamMembers.ToList();

                if (set.linkingType == IniParser.GetSetting("FILTER", "defaultAndString"))
                {
                    //filtere empfänger anhand und-verknüpften filtern (subtrahives/serielles verfahren)
                    foreach (var filter in set.filterList)
                    {
                        if (filter.group == FilterModel.Groups.Kunde)
                        {
                            tmpP = filterCustomer(filter, tmpP).ToList();
                            tmpS.Clear();
                            tmpT.Clear();
                        }

                        else if (filter.group == FilterModel.Groups.Sponsor)
                        {
                            tmpS = filterSponsor(filter, tmpS).ToList();
                            tmpP.Clear();
                            tmpT.Clear();
                        }

                        else if (filter.group == FilterModel.Groups.Mitarbeiter)
                        {
                            tmpT = filterTeamMember(filter, tmpT).ToList();
                            tmpP.Clear();
                            tmpS.Clear();
                        }
                    }
                }

                else if (set.linkingType == IniParser.GetSetting("FILTER", "defaultOrString"))
                {
                    // sammle empfängergruppen anhand oder-verknüpften filter (additives/paralleles verfahren)
                    List<IEnumerable<Person>> listOfPersonGroups = new List<IEnumerable<Person>>();
                    List<IEnumerable<Sponsor>> listOfSponsorGroups = new List<IEnumerable<Sponsor>>();
                    List<IEnumerable<Team>> listOfTeamMemberGroups = new List<IEnumerable<Team>>();
                    List<Person> filteredPersons = new List<Person>();
                    List<Sponsor> filteredSponsors = new List<Sponsor>();
                    List<Team> filteredTeamMembers = new List<Team>();

                    foreach (var filter in set.filterList)
                        if (filter.group == FilterModel.Groups.Kunde)
                        {
                            IEnumerable<Person> fp = filterCustomer(filter, tmpP);
                            listOfPersonGroups.Add(fp);
                        }
                        else if (filter.group == FilterModel.Groups.Sponsor)
                        {
                            IEnumerable<Sponsor> fs = filterSponsor(filter, tmpS);
                            listOfSponsorGroups.Add(fs);
                        }
                        else if (filter.group == FilterModel.Groups.Mitarbeiter)
                        {
                            IEnumerable<Team> ft = filterTeamMember(filter, tmpT);
                            listOfTeamMemberGroups.Add(ft);
                        }

                    // füge Person-Gruppen zusammen
                    foreach (var listOfPersonGroup in listOfPersonGroups)
                        foreach (var person in listOfPersonGroup)
                            filteredPersons.Add(person);

                    foreach (var listOfSponsorGroup in listOfSponsorGroups)
                        foreach (var sponsor in listOfSponsorGroup)
                            filteredSponsors.Add(sponsor);

                    foreach (var listOfTeamMemberGroup in listOfTeamMemberGroups)
                        foreach (var tMember in listOfTeamMemberGroup)
                            filteredTeamMembers.Add(tMember);

                    tmpP = filteredPersons;
                    tmpS = filteredSponsors;
                    tmpT = filteredTeamMembers;

                }

                // Jedes Filter-Set erzeugt seine eigene gefilterte Gruppe
                personLists.Add(tmpP);
                sponsorLists.Add(tmpS);
                teamMemberLists.Add(tmpT);
            }

            // bilde schnittmengen zwischen den gesammelten filter-set-ergebnissen
            if (personLists.Count > 0)
            {
                List<Person> personListUnion = personLists[0];
                foreach (var personList in personLists)
                    if (personList.Count > 0)
                        personListUnion = personListUnion.Intersect(personList).ToList();
                allPersons = personListUnion;
            }

            if (sponsorLists.Count > 0)
            {
                List<Sponsor> sponsorListUnion = sponsorLists[0];
                foreach (var sponsorList in sponsorLists)
                    if (sponsorList.Count > 0)
                        sponsorListUnion = sponsorListUnion.Intersect(sponsorList).ToList();
                allSponsors = sponsorListUnion;
            }

            if (teamMemberLists.Count > 0)
            {
                List<Team> teamMemberListUnion = teamMemberLists[0];
                foreach (var teamMemberList in teamMemberLists)
                    if (teamMemberList.Count > 0)
                        teamMemberListUnion = teamMemberListUnion.Intersect(teamMemberList).ToList();
                allTeamMembers = teamMemberListUnion;
            }

            // entferne doppelte Datensätze
            allPersons = allPersons.GroupBy(p => p.PersonID).Select(grp => grp.First()).ToList();
            allSponsors = allSponsors.GroupBy(s => s.SponsorID).Select(grp => grp.First()).ToList();
            allTeamMembers = allTeamMembers.GroupBy(t => t.TeamID).Select(grp => grp.First()).ToList();
        }

        /// <summary>
        /// Filtere Kunden anhand Filter
        /// </summary>
        /// <param name="filter">Filter-Objekt</param>
        /// <param name="persons">Liste von Personen</param>
        /// <returns></returns>
        private IEnumerable<Person> filterCustomer(FilterModel filter, IEnumerable<Person> persons)
        {
            FilterModel.Criterions criterion = filter.criterion;
            FilterModel.Operations operation = filter.operation;

            // dupliziere persons
            IEnumerable<Person> editPersons = persons.ToList<Person>();

            int value_int;
            DateTime value_date;

            #region Kriterien und Operationen Switch-Cases
            switch (filter.criterion)
            {
                case FilterModel.Criterions.Vorname:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.FirstName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.FirstName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => isStringGreaterThanString(p.FirstName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => isStringSmallerThanString(p.FirstName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Nachname:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.LastName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.LastName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => isStringGreaterThanString(p.LastName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => isStringSmallerThanString(p.LastName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Straße_Hsnr:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.Street.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.Street.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => isStringGreaterThanString(p.Street, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => isStringSmallerThanString(p.Street, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.PLZ:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.ZipCode.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.ZipCode.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.ZipCode > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.ZipCode < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Wohnort:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.City.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.City.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => isStringGreaterThanString(p.City, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => isStringSmallerThanString(p.City, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Geburtsdatum:
                    if (!(DateTime.TryParseExact(filter.value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value_date)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.DateOfBirth.Equals(value_date));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.DateOfBirth.Equals(value_date));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.DateOfBirth > value_date);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.DateOfBirth < value_date);
                            break;
                    } break;

                case FilterModel.Criterions.Alter:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.Age.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.Age.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.Age > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.Age < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Letzter_Einkauf:
                    if (!(DateTime.TryParseExact(filter.value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value_date)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.LastPurchase.Equals(value_date));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.LastPurchase.Equals(value_date));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.LastPurchase > value_date);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.LastPurchase < value_date);
                            break;
                    } break;

                case FilterModel.Criterions.Anzahl_Kinder:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.NumberOfChildren.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.NumberOfChildren.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.NumberOfChildren > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.NumberOfChildren < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Anzahl_Personen:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.NumberOfPersonInHousholdInteger.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.NumberOfPersonInHousholdInteger.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.NumberOfPersonInHousholdInteger > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.NumberOfPersonInHousholdInteger < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Nationalität:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.Nationality.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.Nationality.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => isStringGreaterThanString(p.Nationality, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => isStringSmallerThanString(p.Nationality, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Geburtsland:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.CountryOfBirth.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.CountryOfBirth.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => isStringGreaterThanString(p.CountryOfBirth, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => isStringSmallerThanString(p.CountryOfBirth, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Gültigkeitsbeginn:
                    if (!(DateTime.TryParseExact(filter.value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value_date)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.ValidityStart.Equals(value_date));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.ValidityStart.Equals(value_date));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.ValidityStart > value_date);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.ValidityStart < value_date);
                            break;
                    } break;

                case FilterModel.Criterions.Gültigkeitsende:
                    if (!(DateTime.TryParseExact(filter.value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value_date)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.ValidityEnd.Equals(value_date));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.ValidityEnd.Equals(value_date));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.ValidityEnd > value_date);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.ValidityEnd < value_date);
                            break;
                    } break;

                case FilterModel.Criterions.Erfassungsdatum:
                    if (!(DateTime.TryParseExact(filter.value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value_date)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.CreationDate.Equals(value_date));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.CreationDate.Equals(value_date));
                            break;
                        case FilterModel.Operations.größer:
                            editPersons = editPersons.Where(p => p.CreationDate > value_date);
                            break;
                        case FilterModel.Operations.kleiner:
                            editPersons = editPersons.Where(p => p.CreationDate < value_date);
                            break;
                    } break;

                case FilterModel.Criterions.Verheiratet:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.IsMarried == parseTextBoxValueToBoolean(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !p.IsMarried == parseTextBoxValueToBoolean(filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Aktiv:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editPersons = editPersons.Where(p => p.IsActive == parseTextBoxValueToBoolean(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editPersons = editPersons.Where(p => !(p.IsActive == parseTextBoxValueToBoolean(filter.value)));
                            break;
                    } break;
            }
            #endregion

            return editPersons;
        }

        /// <summary>
        /// Filtere Sponsoren anhand Filter
        /// </summary>
        /// <param name="filter">Filter-Objekt</param>
        /// <param name="sponsors">Liste von Sponsoren</param>
        /// <returns></returns>
        private IEnumerable<Sponsor> filterSponsor(FilterModel filter, IEnumerable<Sponsor> sponsors)
        {
            FilterModel.Criterions criterion = filter.criterion;
            FilterModel.Operations operation = filter.operation;

            // dupliziere sponsors
            IEnumerable<Sponsor> editSponsors = sponsors.ToList<Sponsor>();

            int value_int;

            #region Kriterien und Operationen Switch-Cases
            switch (filter.criterion)
            {
                case FilterModel.Criterions.Vorname:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.FirstName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.FirstName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editSponsors = editSponsors.Where(s => isStringGreaterThanString(s.FirstName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editSponsors = editSponsors.Where(s => isStringSmallerThanString(s.FirstName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Nachname:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.LastName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.LastName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editSponsors = editSponsors.Where(s => isStringGreaterThanString(s.LastName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editSponsors = editSponsors.Where(s => isStringSmallerThanString(s.LastName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Kontaktperson:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.ContactPerson.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.ContactPerson.Equals(filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Straße_Hsnr:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.Street.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.Street.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editSponsors = editSponsors.Where(s => isStringGreaterThanString(s.Street, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editSponsors = editSponsors.Where(s => isStringSmallerThanString(s.Street, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.PLZ:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.ZipCode.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.ZipCode.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editSponsors = editSponsors.Where(s => s.ZipCode > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editSponsors = editSponsors.Where(s => s.ZipCode < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Wohnort:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.City.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.City.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editSponsors = editSponsors.Where(s => isStringGreaterThanString(s.City, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editSponsors = editSponsors.Where(s => isStringSmallerThanString(s.City, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Firma:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.CompanyName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !s.CompanyName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editSponsors = editSponsors.Where(s => isStringGreaterThanString(s.CompanyName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editSponsors = editSponsors.Where(s => isStringSmallerThanString(s.CompanyName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Aktiv:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.IsActive == parseTextBoxValueToBoolean(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !(s.IsActive == parseTextBoxValueToBoolean(filter.value)));
                            break;
                    } break;

                case FilterModel.Criterions.Serienbrief_erlaubt:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editSponsors = editSponsors.Where(s => s.IsFormLetterAllowed == parseTextBoxValueToBoolean(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editSponsors = editSponsors.Where(s => !(s.IsFormLetterAllowed == parseTextBoxValueToBoolean(filter.value)));
                            break;
                    } break;
            }
            #endregion

            return editSponsors;
        }

        /// <summary>
        /// Filtere Teammitglieder anhand Filter
        /// </summary>
        /// <param name="filter">Filter-Objekt</param>
        /// <param name="teamMembers">Liste von Teammitgliedern</param>
        /// <returns></returns>
        private IEnumerable<Team> filterTeamMember(FilterModel filter, IEnumerable<Team> teamMembers)
        {
            FilterModel.Criterions criterion = filter.criterion;
            FilterModel.Operations operation = filter.operation;

            // dupliziere teamMembers
            IEnumerable<Team> editTeamMembers = teamMembers.ToList<Team>();

            int value_int;
            DateTime value_date;

            #region Kriterien und Operationen Switch-Cases
            switch (filter.criterion)
            {
                case FilterModel.Criterions.Vorname:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.FirstName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.FirstName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => isStringGreaterThanString(t.FirstName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => isStringSmallerThanString(t.FirstName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Nachname:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.LastName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.LastName.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => isStringGreaterThanString(t.LastName, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => isStringSmallerThanString(t.LastName, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Straße_Hsnr:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.Street.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.Street.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => isStringGreaterThanString(t.Street, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => isStringSmallerThanString(t.Street, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.PLZ:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.ZipCode.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.ZipCode.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => t.ZipCode > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => t.ZipCode < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Wohnort:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.City.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.City.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => isStringGreaterThanString(t.City, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => isStringSmallerThanString(t.City, filter.value));
                            break;
                    } break;

                case FilterModel.Criterions.Geburtsdatum:
                    if (!(DateTime.TryParseExact(filter.value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value_date)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.DateOfBirth.Equals(value_date));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.DateOfBirth.Equals(value_date));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => t.DateOfBirth > value_date);
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => t.DateOfBirth < value_date);
                            break;
                    } break;

                case FilterModel.Criterions.Alter:
                    if (!(int.TryParse(filter.value, out value_int)))
                        break;
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.Age.Equals(value_int));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.Age.Equals(value_int));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => t.Age > value_int);
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => t.Age < value_int);
                            break;
                    } break;

                case FilterModel.Criterions.Aktiv:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.IsActive == parseTextBoxValueToBoolean(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !(t.IsActive == parseTextBoxValueToBoolean(filter.value)));
                            break;
                    } break;

                case FilterModel.Criterions.Teamfunktion:
                    switch (filter.operation)
                    {
                        case FilterModel.Operations.gleich:
                            editTeamMembers = editTeamMembers.Where(t => t.TeamFunction.Name.Equals(filter.value));
                            break;
                        case FilterModel.Operations.ungleich:
                            editTeamMembers = editTeamMembers.Where(t => !t.TeamFunction.Name.Equals(filter.value));
                            break;
                        case FilterModel.Operations.größer:
                            editTeamMembers = editTeamMembers.Where(t => isStringGreaterThanString(t.TeamFunction.Name, filter.value));
                            break;
                        case FilterModel.Operations.kleiner:
                            editTeamMembers = editTeamMembers.Where(t => isStringSmallerThanString(t.TeamFunction.Name, filter.value));
                            break;
                    } break;

            #endregion

            }
            return editTeamMembers;
        }


        /// <summary>
        /// Liefert ein SortedList mit den CSV-Spalten
        /// key ist ein String, der den Spaltentitel bzw. im Serienbrief den Feldnamen repräsentiert
        /// value ist eine Liste von Strings, die alle gefilterten Datensätze repräsentieren
        /// </summary>
        /// <returns>CSV-Spalten</returns>
        private SortedList<string, List<String>> getColumns()
        {
            // Repräsentation der CSV-Spalten
            SortedList<string, List<String>> columns = new SortedList<string,List<string>>();

            // bearbeite Spaltenzuordnungen der Reihe nach
            foreach (var assignment in formletterPatternModel.formletterColumnCsvDocumentAssignments)
            {
                if (assignment.formletterTableAssignment.field == FormletterTableAssignment.Fields.Leer ||
                    assignment.formletterTableAssignment.group == FormletterTableAssignment.Groups.Leer)
                    continue;

                string csv_col_name = assignment.csv_col_name;
                FormletterTableAssignment fta = assignment.formletterTableAssignment;
                FormletterTableAssignment.Groups group = fta.group;
                FormletterTableAssignment.Fields field = fta.field;

                // gehe von spalte zu spalte
                List<string> column = new List<string>();

                if (group == FormletterTableAssignment.Groups.Kunde)
                {
                    #region Switch-Cases für FormletterTableAssignment
                    switch (field)
                    {
                        case FormletterTableAssignment.Fields.Einleitung:
                            foreach (var person in filteredPersons)
                                column.Add(getSaluation(person));
                            break;

                        case FormletterTableAssignment.Fields.Anrede:
                            foreach (var person in filteredPersons)
                                column.Add(person.Title.Name);
                            break;

                        case FormletterTableAssignment.Fields.Vorname:
                            foreach (var person in filteredPersons)
                                column.Add(person.FirstName);
                            break;

                        case FormletterTableAssignment.Fields.Nachname:
                            foreach (var person in filteredPersons)
                                column.Add(person.LastName);
                            break;

                        case FormletterTableAssignment.Fields.Straße_Hsnr:
                            foreach (var person in filteredPersons)
                                column.Add(person.Street);
                            break;

                        case FormletterTableAssignment.Fields.PLZ:
                            foreach (var person in filteredPersons)
                                column.Add(person.ZipCode.ToString());
                            break;

                        case FormletterTableAssignment.Fields.Wohnort:
                            foreach (var person in filteredPersons)
                                column.Add(person.City);
                            break;

                        case FormletterTableAssignment.Fields.Nationalität:
                            foreach (var person in filteredPersons)
                                column.Add(person.Nationality);
                            break;

                        case FormletterTableAssignment.Fields.Geburtsland:
                            foreach (var person in filteredPersons)
                                column.Add(person.CountryOfBirth);
                            break;

                        case FormletterTableAssignment.Fields.Geburtsdatum:
                            foreach (var person in filteredPersons)
                                column.Add(person.DateOfBirth.ToString("dd.MM.yyyy"));
                            break;

                        case FormletterTableAssignment.Fields.Gültigkeitsbeginn:
                            foreach (var person in filteredPersons)
                                column.Add(person.ValidityStart.ToString("dd.MM.yyyy"));
                            break;

                        case FormletterTableAssignment.Fields.Gültigkeitsende:
                            foreach (var person in filteredPersons)
                                column.Add(person.ValidityEnd.ToString("dd.MM.yyyy"));
                            break;

                        case FormletterTableAssignment.Fields.Email:
                            foreach (var person in filteredPersons)
                                column.Add(person.Email);
                            break;

                        case FormletterTableAssignment.Fields.Telefon:
                            foreach (var person in filteredPersons)
                                column.Add(person.Phone);
                            break;

                        case FormletterTableAssignment.Fields.Mobil:
                            foreach (var person in filteredPersons)
                                column.Add(person.MobileNo);
                            break;

                        case FormletterTableAssignment.Fields.Letzter_Einkauf:
                            foreach (var person in filteredPersons)
                                if (person.LastPurchase == null)
                                    column.Add("-");
                                else
                                    column.Add( (((DateTime)person.LastPurchase).ToString("dd.MM.yyyy")) );
                            break;

                        case FormletterTableAssignment.Fields.Familienstand:
                            foreach (var person in filteredPersons)
                                column.Add(person.FamilyState.ShortName + " - " + person.FamilyState.Name);
                            break;

                        case FormletterTableAssignment.Fields.Vorname_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalFirstName);
                            break;

                        case FormletterTableAssignment.Fields.Nachname_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalLastName);
                            break;

                        case FormletterTableAssignment.Fields.Nationalität_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalNationality);
                            break;

                        case FormletterTableAssignment.Fields.Geburtsland_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalCountryOfBirth);
                            break;

                        case FormletterTableAssignment.Fields.Telefon_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalPhone);
                            break;

                        case FormletterTableAssignment.Fields.Mobil_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalMobile);
                            break;

                        case FormletterTableAssignment.Fields.Email_Partner:
                            foreach (var person in filteredPersons)
                                column.Add(person.MaritalEmail);
                            break;
                    }
                    #endregion
                }

                else if (group == FormletterTableAssignment.Groups.Sponsor)
                {
                    #region Switch-Cases für FormletterTableAssignment
                    switch (field)
                    {
                        case FormletterTableAssignment.Fields.Einleitung:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(getSaluation(sponsor));
                            break;

                        case FormletterTableAssignment.Fields.Firmenname:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.CompanyName);
                            break;

                        case FormletterTableAssignment.Fields.Kontaktperson:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.ContactPerson);
                            break;

                        case FormletterTableAssignment.Fields.Anrede:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.Title.Name);
                            break;

                        case FormletterTableAssignment.Fields.Vorname:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.FirstName);
                            break;

                        case FormletterTableAssignment.Fields.Nachname:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.LastName);
                            break;

                        case FormletterTableAssignment.Fields.Straße_Hsnr:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.Street);
                            break;

                        case FormletterTableAssignment.Fields.PLZ:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.ZipCode.ToString());
                            break;

                        case FormletterTableAssignment.Fields.Wohnort:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.City);
                            break;

                        case FormletterTableAssignment.Fields.Email:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.Email);
                            break;

                        case FormletterTableAssignment.Fields.Telefon:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.PhoneNo);
                            break;

                        case FormletterTableAssignment.Fields.Mobil:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.MobileNo);
                            break;

                        case FormletterTableAssignment.Fields.Faxnummer:
                            foreach (var sponsor in filteredSponsors)
                                column.Add(sponsor.FaxNo);
                            break;
                    }
                    #endregion
                }

                else if (group == FormletterTableAssignment.Groups.Mitarbeiter)
                {
                    #region Switch-Cases für FormletterTableAssignment
                    switch (field)
                    {
                        case FormletterTableAssignment.Fields.Einleitung:
                            foreach (var member in filteredTeamMembers)
                                column.Add( getSaluation(member) );
                            break;

                        case FormletterTableAssignment.Fields.Teamfunktion:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.TeamFunction.Name);
                            break;

                        case FormletterTableAssignment.Fields.Vorname:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.FirstName);
                            break;

                        case FormletterTableAssignment.Fields.Anrede:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.Title.Name);
                            break;

                        case FormletterTableAssignment.Fields.Nachname:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.LastName);
                            break;

                        case FormletterTableAssignment.Fields.Straße_Hsnr:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.Street);
                            break;

                        case FormletterTableAssignment.Fields.PLZ:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.ZipCode.ToString("dd.MM.yyyy"));
                            break;

                        case FormletterTableAssignment.Fields.Wohnort:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.City);
                            break;

                        case FormletterTableAssignment.Fields.Geburtsdatum:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.DateOfBirth.ToString("dd.MM.yyyy"));
                            break;

                        case FormletterTableAssignment.Fields.Email:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.Email);
                            break;

                        case FormletterTableAssignment.Fields.Telefon:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.PhoneNo);
                            break;

                        case FormletterTableAssignment.Fields.Mobil:
                            foreach (var member in filteredTeamMembers)
                                column.Add(member.MobileNo);
                            break;
                    }
                    #endregion
                }

                else if (group == FormletterTableAssignment.Groups.Datum)
                {
                    #region Switch-Cases für FormletterTableAssignment
                    switch (field)
                    {
                        case FormletterTableAssignment.Fields.Datum:
                            foreach (var member in filteredTeamMembers)
                                column.Add(SafeStringParser.safeParseToStr(DateTime.Today));
                            break;
                    }
                    #endregion
                }
                columns.Add(csv_col_name, column);
                this.columnHeight = column.Count;
            }

            return columns;
        }

        /// <summary>
        /// Füllt die CSV-Datei mit den gefilterten Daten
        /// </summary>
        /// <returns></returns>
        public bool fillCsvFile()
        {
            try
            {
                if (this.formletterPatternModel == null || this.csv_columns == null || !(this.columnHeight > 0))
                    return false;

                string currentDir = System.IO.Directory.GetCurrentDirectory();
                string path = formletterPatternModel.csv_filename.Replace("%PROGRAMPATH%" , currentDir);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@path, false, System.Text.Encoding.Default))
                {
                    // Schreibe zuerst Spaltenüberschriften in die CSV
                    string titles = string.Join(";", this.csv_columns.Keys);
                    file.WriteLine(titles);

                    // Iteriere zuerst über die Zeilen, dann über die Spalten
                    for (int i = 0; i < this.columnHeight; i++)
                    {
                        string line = "";
                        for (int j = 0; j < this.csv_columns.Count; j++)
                        {
                            string key = this.csv_columns.Keys[j];
                            line += ('"' + this.csv_columns[key][i] + '"').Replace(";", ",");

                            if (j < this.csv_columns.Count - 1)
                                line += ";";
                        }
                        file.WriteLine(line);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Hilfsfunktionen

        /// <summary>
        /// prüft ob smallerString kleiner ist als greaterString
        /// </summary>
        /// <param name="smallerString">kleinerer String</param>
        /// <param name="greaterString">größerer String</param>
        /// <returns></returns>
        private bool isStringSmallerThanString(string smallerString, string greaterString)
        {
            int i = String.Compare(smallerString, greaterString);

            // -1: smallerString ist kleiner als greaterString
            if (i < 0)
                return true;
            // 0: Gleichheit, 1: smallerString ist größer als greaterString
            else
                return false;
        }

        /// <summary>
        /// prüft ob greaterString größer ist als smallerString
        /// </summary>
        /// <param name="greaterString">größerer String</param>
        /// <param name="smallerString">kleinerer String</param>
        /// <returns></returns>
        private bool isStringGreaterThanString(string greaterString, string smallerString)
        {
            int i = String.Compare(greaterString, smallerString);

            // 1: greaterString ist größer als smallerString
            if (i == 1)
                return true;
            // -1: greaterString ist kleiner als smallerString, 0: Gleichheit
            else
                return false;
        }

        /// <summary>
        /// parse den Wert der Textbox semantisch als Boolean
        /// </summary>
        /// <param name="text">Textbox-Text</param>
        /// <returns>boolean</returns>
        private bool parseTextBoxValueToBoolean(string text)
        {
            // wenn z.B. in "ja,j,yes,true,..." der Teilstring "ja" vorkommt...
            string boolReplacement = IniParser.GetSetting("FILTER", "booleanTextBox");
            if (text.Contains(boolReplacement))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Ermittelt die Anrede anhand des Geschlechts
        /// </summary>
        /// <param name="x">Person</param>
        /// <returns>Anrede</returns>
        private string getSaluation(Person x)
        {
            string defaultMaleTitle = IniParser.GetSetting("FORMLETTER", "defaultMaleTitle");
            string defaultFemaleTitle = IniParser.GetSetting("FORMLETTER", "defaultFemaleTitle");

            if (string.IsNullOrEmpty(x.LastName))
                return this.formletterPatternModel.saluation_n;
            else if (x != null && x.Title.Name.Equals(defaultMaleTitle))
                return this.formletterPatternModel.saluation_m;
            else if (x != null && x.Title.Name.Equals(defaultFemaleTitle))
                return this.formletterPatternModel.saluation_f;
            else
                return this.formletterPatternModel.saluation_n;
        }
        /// <summary>
        /// Ermittelt die Anrede anhand des Geschlechts
        /// </summary>
        /// <param name="x">Sponsor</param>
        /// <returns>Anrede</returns>
        private string getSaluation(Sponsor x)
        {
            string defaultMaleTitle = IniParser.GetSetting("FORMLETTER", "defaultMaleTitle");
            string defaultFemaleTitle = IniParser.GetSetting("FORMLETTER", "defaultFemaleTitle");

            if (string.IsNullOrEmpty(x.LastName))
                return this.formletterPatternModel.saluation_n;
            else if (x != null && x.Title.Name.Equals(defaultMaleTitle))
                return this.formletterPatternModel.saluation_m;
            else if (x != null && x.Title.Name.Equals(defaultFemaleTitle))
                return this.formletterPatternModel.saluation_f;
            else
                return this.formletterPatternModel.saluation_n;
        }
        /// <summary>
        /// Ermittelt die Anrede anhand des Geschlechts
        /// </summary>
        /// <param name="x">Teammitglied</param>
        /// <returns>Anrede</returns>
        private string getSaluation(Team x)
        {
            string defaultMaleTitle = IniParser.GetSetting("FORMLETTER", "defaultMaleTitle");
            string defaultFemaleTitle = IniParser.GetSetting("FORMLETTER", "defaultFemaleTitle");

            if (string.IsNullOrEmpty(x.LastName))
                return this.formletterPatternModel.saluation_n;
            else if (x != null && x.Title.Name.Equals(defaultMaleTitle))
                return this.formletterPatternModel.saluation_m;
            else if (x != null && x.Title.Name.Equals(defaultFemaleTitle))
                return this.formletterPatternModel.saluation_f;
            else
                return this.formletterPatternModel.saluation_n;
        }

        #endregion
    }
}
