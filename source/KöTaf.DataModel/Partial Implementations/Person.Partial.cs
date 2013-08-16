using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel.Enums;
using KöTaf.DataModel.Interfaces;
using KöTaf.DataModel.Utils;
using System.Diagnostics;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Person : IIndividual
    {
        /// <summary>
        /// Startwert der Tafelnummer
        /// </summary>
        const int TABLE_NO_START_VALUE = 1;

        #region Static Methods

        /// <summary>
        /// Gibt eine Liste von Personen in der Datenbank zurück
        /// </summary>
        /// <param name="personID">Filterung nach Personen ID</param>
        /// <param name="userAccountID">Filterung nach einem Benutzerkonto</param>
        /// <returns>Liste aller Personen</returns>
        public static IEnumerable<Person> GetPersons(int? personID = null, int? userAccountID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var persons = db.Persons
                    .Include("Child")
                    .Include("Title")
                    .Include("UserAccount")
                    .Include("FamilyState")
                    .Include("Booking")
                    .AsQueryable();

                if (personID.HasValue)
                    persons = persons.Where(p => p.PersonID == personID.Value);

                if (userAccountID.HasValue)
                    persons = persons.Where(p => p.UserAccount != null && p.UserAccount.UserAccountID == userAccountID.Value);

                // Erst mit ToList() wird SQL Command ausgeführt!
                return persons
                    .OrderBy(p => p.TableNo)
                    .OrderBy(p => p.Group)
                    .ToList();
            }
        }

        /// <summary>
        /// Fügt eine neue Person der Datenbank hinzu
        /// </summary>
        /// <param name="titleID">Die ID des Titels</param>
        /// <param name="familyStateId">Die ID des Familienstatus</param>
        /// <param name="firstName">Vorname</param>
        /// <param name="lastName">Nachname</param>
        /// <param name="street">Strasse</param>
        /// <param name="nationality">Nationalität</param>
        /// <param name="dateOfBirth">Geburtsdatum</param>
        /// <param name="group">Die zugeordnete Gruppe</param>
        /// <param name="zipCode">Die Postleitzahl</param>
        /// <param name="city">Stadt</param>
        /// <param name="validityStart">Gültigkeits Startdatum</param>
        /// <param name="validityEnd">Gültigkeits Enddatum</param>
        /// <param name="teamId">Die ID eines Teammitglieds, welcher die Person hinzugefügt werden soll</param>
        /// <param name="countryOfBirth">Geburtsland</param>
        /// <param name="email">Emailadresse</param>
        /// <param name="mobileNo">Handynummer</param>
        /// <param name="phone">Telefonnummer</param>
        /// <param name="maritalFirstName">Verheiratete Person Vorname</param>
        /// <param name="maritalLastName">Verheiratete Person Nachname</param>
        /// <param name="maritalNationality">Verheiratete Person Nationalität</param>
        /// <param name="maritalBirthday">Verheiratete Person Geburtsdatum</param>
        /// <param name="maritalCountryOfBirth">Verheiratete Person Geburtsland</param>
        /// <param name="maritalPhone">Verheiratete Person Telefonnummer</param>
        /// <param name="maritalMobile">Verheiratete Person Handynummer</param>
        /// <param name="maritalEmail">Verheiratete Person Emailadresse</param>
        /// <param name="martialTitleID">Verheiratete Person Title ID</param>
        /// <param name="comment">Kommentar</param>
        /// <param name="tableNo">Tafel Nummer</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int titleID, int familyStateId, string firstName, string lastName, string street, string nationality,
            DateTime dateOfBirth, int group, int zipCode, string city, DateTime validityStart, DateTime validityEnd,
            int? userAccountID = null, string countryOfBirth = null, string email = null, string mobileNo = null, string phone = null,
            string maritalFirstName = null, string maritalLastName = null, string maritalNationality = null,
            DateTime? maritalBirthday = null, string maritalCountryOfBirth = null, string maritalPhone = null,
            string maritalMobile = null, string maritalEmail = null, int? martialTitleID = null,
            string comment = null, int? tableNo = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var person = new Person()
                {
                    City = city,
                    Comment = comment,
                    CountryOfBirth = countryOfBirth,
                    CreationDate = DateTime.Now,
                    DateOfBirth = dateOfBirth,
                    Email = email,
                    FamilyState = db.FamilyStates.Single(fs => fs.FamilyStateID == familyStateId),
                    FirstName = firstName,
                    Group = group,
                    IsActive = true,
                    LastModified = null,
                    LastName = lastName,
                    LastPurchase = null,
                    MaritalBirthday = maritalBirthday,
                    MaritalFirstName = maritalFirstName,
                    MaritalLastName = maritalLastName,
                    MaritalNationality = maritalNationality,
                    MaritalCountryOfBirth = maritalCountryOfBirth,
                    MaritalPhone = maritalPhone,
                    MaritalMobile = maritalMobile,
                    MaritalEmail = maritalEmail,
                    MaritalTitle = (martialTitleID.HasValue) ? db.Titles.Single(mt => mt.TitleID == martialTitleID.Value) : null,
                    MobileNo = mobileNo,
                    Nationality = nationality,
                    Phone = phone,
                    Street = street,
                    TableNo = tableNo,
                    Title = db.Titles.Single(t => t.TitleID == titleID),
                    ValidityEnd = validityEnd,
                    ValidityStart = validityStart,
                    ZipCode = zipCode,
                    UserAccount = (userAccountID.HasValue) ? db.UserAccounts.SingleOrDefault(u => u.UserAccountID == userAccountID.Value) : null,
                };

                db.Persons.AddObject(person);
                db.SaveChanges();

                return person.PersonID;
            }
        }

        /// <summary>
        /// Aktualisiert eine Person in der Datenbank
        /// </summary>
        /// <param name="personID">Die Personen ID in der Datenbank</param>
        /// <param name="titleID">Die ID des Titels</param>
        /// <param name="familyStateId">Die ID des Familienstatus</param>
        /// <param name="firstName">Vorname</param>
        /// <param name="lastName">Nachname</param>
        /// <param name="street">Strasse</param>
        /// <param name="nationality">Nationalität</param>
        /// <param name="dateOfBirth">Geburtsdatum</param>
        /// <param name="group">Die zugeordnete Gruppe</param>
        /// <param name="zipCode">Die Postleitzahl</param>
        /// <param name="city">Stadt</param>
        /// <param name="validityStart">Gültigkeits Startdatum</param>
        /// <param name="validityEnd">Gültigkeits Enddatum</param>
        /// <param name="teamId">Die ID eines Teammitglieds, welcher die Person hinzugefügt werden soll</param>
        /// <param name="countryOfBirth">Geburtsland</param>
        /// <param name="email">Emailadresse</param>
        /// <param name="mobileNo">Handynummer</param>
        /// <param name="phone">Telefonnummer</param>
        /// <param name="maritalFirstName">Verheiratete Person Vorname</param>
        /// <param name="maritalLastName">Verheiratete Person Nachname</param>
        /// <param name="maritalNationality">Verheiratete Person Nationalität</param>
        /// <param name="maritalBirthday">Verheiratete Person Geburtsdatum</param>
        /// <param name="maritalCountryOfBirth">Verheiratete Person Geburtsland</param>
        /// <param name="maritalPhone">Verheiratete Person Telefonnummer</param>
        /// <param name="maritalMobile">Verheiratete Person Handynummer</param>
        /// <param name="maritalEmail">Verheiratete Person Emailadresse</param>
        /// <param name="martialTitleID">Verheiratete Person Title ID</param>
        /// <param name="comment">Kommentar</param>
        /// <param name="tableNo">Tafel Nummer</param>
        public static void Update(int personID, int titleID, int familyStateID, int? userAccountID, string firstname, string lastname,
            int zipCode, string city, string street, string nationality, int? tableNo, string phone, string mobileNo,
            string comment, string countryOfBirth, DateTime dateOfBirth, string email, int group,
            DateTime validityStart, DateTime validityEnd, DateTime? maritalBirthday, string maritalFirstname,
            string maritalLastname, string maritalNationality, DateTime? lastPurchase = null,
            string maritalCountryOfBirth = null, string maritalPhone = null, string maritalMobile = null,
            string maritalEmail = null, int? maritalTitleID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var person = db.Persons.Single(p => p.PersonID == personID);

                person.LastPurchase = lastPurchase;
                person.FirstName = firstname;
                person.LastName = lastname;
                person.LastModified = DateTime.Now;
                person.City = city;
                person.ZipCode = zipCode;
                person.TableNo = tableNo;
                person.Title = db.Titles.Single(t => t.TitleID == titleID);
                person.Street = street;
                person.Phone = phone;
                person.Comment = comment;
                person.CountryOfBirth = countryOfBirth;
                person.DateOfBirth = dateOfBirth;
                person.Email = email;
                person.FamilyState = db.FamilyStates.Single(fs => fs.FamilyStateID == familyStateID);
                person.Group = group;
                person.MaritalBirthday = maritalBirthday;
                person.MaritalFirstName = maritalFirstname;
                person.MaritalLastName = maritalLastname;
                person.MaritalNationality = maritalNationality;
                person.MaritalCountryOfBirth = maritalCountryOfBirth;
                person.MaritalPhone = maritalPhone;
                person.MaritalMobile = maritalMobile;
                person.MaritalEmail = maritalEmail;

                if (maritalTitleID.HasValue)
                    person.MaritalTitle = db.Titles.Single(t => t.TitleID == maritalTitleID.Value);

                person.MobileNo = mobileNo;
                person.Nationality = nationality;

                if (userAccountID.HasValue)
                    person.UserAccount = db.UserAccounts.Single(u => u.UserAccountID == userAccountID.Value);

                person.ValidityStart = validityStart;
                person.ValidityEnd = validityEnd;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Aktiviert eine Person
        /// </summary>
        /// <param name="personID">Benötigte Personen ID</param>
        public static void Activate(int personID)
        {
            ToogleActivateState(personID, true);
        }

        /// <summary>
        /// Deaktiviert eine Person
        /// </summary>
        /// <param name="personID">Benötigte Personen ID</param>
        public static void Deactivate(int personID)
        {
            ToogleActivateState(personID, false);
        }

        /// <summary>
        /// Schaltet zwischen verschiedenen Aktivierungsstati um
        /// </summary>
        /// <param name="personID">Benötigte Personen ID</param>
        /// <param name="activate">Person Aktiveren oder Deaktivieren</param>
        private static void ToogleActivateState(int personID, bool activate)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var person = db.Persons.Single(p => p.PersonID == personID);
                person.IsActive = activate;

                if (activate)
                    person.TableNo = GetNewIdentityNo();
                else
                    person.TableNo = null;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Generiert eine neue Ausweisnummer
        /// </summary>
        /// <returns>Die neue Ausweisnummer</returns>
        public static int GetNewIdentityNo()
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                // get all identities
                var tableNoList = db.Persons.Where(p => p.TableNo.HasValue).Select(p => p.TableNo.Value).ToList();

                int maxValue = 2;
                int minValue = 1;

                if (tableNoList.Count > 0)
                {
                    maxValue = tableNoList.Max();
                    minValue = tableNoList.Min();
                }

                var newIdentityNo = -1;

                if (minValue > TABLE_NO_START_VALUE)
                    newIdentityNo = TABLE_NO_START_VALUE;
                else
                {
                    for (int i = TABLE_NO_START_VALUE; i < maxValue; i++)
                    {
                        var resultValue = tableNoList.Where(o => o == i);
                        if (!resultValue.Any())
                        {
                            // Neue Ausweisnummer gefunden
                            newIdentityNo = i;
                            break;
                        }
                    }
                }
                return ((newIdentityNo > 0) ? newIdentityNo : (maxValue + 1));
            }
        }

        /// <summary>
        /// Zählt die Personen in der Datenbank
        /// </summary>
        /// <param name="type">Filterung nach Zähltyp</param>
        /// <param name="countFullAged">Filterung nach volljährigen Personen</param>
        /// <returns>Die Anzahl der Personen in der Datenbank</returns>
        public static int Count(CountStateType type = CountStateType.All, bool countFullAged = false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var persons = db.Persons.AsQueryable();

                switch (type)
                {
                    case CountStateType.Activated:
                        persons = persons.Where(p => p.IsActive);
                        break;

                    case CountStateType.Deactivated:
                        persons = persons.Where(p => !p.IsActive);
                        break;
                }

                var personCountList = persons.ToList();
                if (countFullAged)
                {
                    personCountList = personCountList.Where(p => p.Age >= 18).ToList();
                }

                return personCountList.Count();
            }
        }

        /// <summary>
        /// Sucht Personen die seit einem halben Jahr nicht mehr eingekauft haben und setzt diese auf Inaktiv
        /// </summary>
        /// <param name="state"></param>
        public static void DeactivatePersonsByLastPurchaseAutomatically(object state)
        {
            using (var db = new TafelModelContainer())
            {
                var halfYear = DateTime.Now.AddMonths(-6).Date;
                var persons = db.Persons
                    .Where(p => p.LastPurchase.HasValue)
                    .ToList()
                    .Where(p => p.LastPurchase.Value.Date < halfYear);
                if (persons.Any())
                {
                    foreach (var p in persons)
                    {
                        p.IsActive = false;
                        p.TableNo = null;
                    }

                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gibt den vollständigen Namen der Person zurück
        /// </summary>
        public string FullName
        {
            get
            {
                return FormatUtils.GetFullName(FirstName, LastName);
            }
        }

        /// <summary>
        /// Gibt das Alter der Person zurück
        /// </summary>
        public int Age
        {
            get
            {
                return FormatUtils.GetAge(this.DateOfBirth);
            }
        }

        /// <summary>
        /// Gibt den vollständigen Wohnort der Person zurück
        /// </summary>
        public string ResidentialAddress
        {
            get
            {
                return FormatUtils.GetResidentialAddress(ZipCode, City, Street);
            }
        }

        /// <summary>
        /// Gibt ein Boolean Flag zurück ob die jeweilige Person einen Partner hat oder nicht
        /// </summary>
        public bool IsMarried
        {
            get
            {
                if (!string.IsNullOrEmpty(this.MaritalFirstName)
                    && !string.IsNullOrEmpty(this.MaritalLastName))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der Kinder zurück
        /// </summary>
        public int NumberOfChildren
        {
            get
            {
                return this.Child.Count;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der Personen im Haushalt als String formatiert zurück
        /// </summary>
        public string NumberOfPersonInHousholdString
        {
            get
            {
                var child = this.NumberOfChildren;
                var person = 1;
                if (IsMarried)
                {
                    person = person + 1;
                }
                return string.Format("{0} / {1}", person, child);
            }
        }

        /// <summary>
        /// Gibt die Anzahl der Personen im Haushalt als Integer zurück
        /// </summary>
        public int NumberOfPersonInHousholdInteger
        {
            get
            {
                var child = this.NumberOfChildren;
                var person = 1;
                if (IsMarried)
                {
                    person = person + 1;
                }
                return person + child;
            }
        }

        /// <summary>
        /// Gibt den vollständigen Namen des Partners zurück
        /// </summary>
        public string MaritalFullname
        {
            get
            {
                if (!string.IsNullOrEmpty(MaritalFirstName)
                    && !string.IsNullOrEmpty(MaritalLastName))
                {
                    return FormatUtils.GetFullName(MaritalFirstName, MaritalLastName);
                }
                return null;
            }
        }

        #endregion
    }
}
