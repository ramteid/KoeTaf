using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel.Enums;
using KöTaf.DataModel.Interfaces;
using KöTaf.DataModel.Utils;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Child :  IIndividual
    {
        #region Static Methods

        /// <summary>
        /// Gibt eine Liste von Kindern zurück
        /// </summary>
        /// <param name="childId">Filterung nach einem Kind</param>
        /// <param name="personID">Filterung nach Person</param>
        /// <returns>Liste von Kindern in der Datenbank</returns>
        public static IEnumerable<Child> GetChildren(int? childId = null, int? personID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var children = db.Children
                                .Include("Person")
                                .AsQueryable();

                if (childId.HasValue)
                    children = children.Where(c => c.ChildID == childId.Value);

                if (personID.HasValue)
                    children = children.Where(c => c.Person.PersonID == personID.Value);

                return children.ToList();
            }
        }

        /// <summary>
        /// Fügt ein neues Kind in der Datenbank hinzu
        /// </summary>
        /// <param name="personId">Die Personen ID</param>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachname</param>
        /// <param name="dateOfBirth">Geburtsdatum</param>
        /// <param name="isFemale">Wert ob es sich um ein Junge oder Mädchen handelt</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int personId, string firstname, string lastname, DateTime dateOfBirth, bool isFemale)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                Child child = new Child()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    DateOfBirth = dateOfBirth,
                    Person = db.Persons.Single(p => p.PersonID == personId),
                    IsFemale = isFemale,
                };

                db.Children.AddObject(child);
                db.SaveChanges();

                return child.ChildID;
            }
        }

        /// <summary>
        /// Methode zum aktualisieren eines Kinds
        /// </summary>
        /// <param name="childId">Die benötigte Kind ID</param>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachname</param>
        /// <param name="dateOfBirth">Geburtsdatum</param>
        /// <param name="isFemale">Wert ob es sich um ein Junge oder Mädchen handelt</param>
        public static void Update(int childId, string firstname = null, string lastname = null, DateTime? dateOfBirth = null, bool? isFemale = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                Child child = db.Children.Single(c => c.ChildID == childId);
                child.FirstName = firstname;
                child.LastName = lastname;
                child.DateOfBirth = dateOfBirth.Value;
                child.IsFemale = isFemale.Value;
                child.LastModified = DateTime.Now;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Methode zum löschen eines bestehenden Kindes
        /// </summary>
        /// <param name="childID">ID des Kinds in der Datenbank</param>
        public static void Delete(int childID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                Child child = db.Children.Single(c => c.ChildID == childID);
                db.Children.DeleteObject(child);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Zählt bestehende Kinder in der Datenbank
        /// </summary>
        /// <param name="childGenderType">Typ nach welchen welchen Kindern gefiltert werden soll</param>
        /// <param name="parentCountState">Typ nach welchen welchen Eltern gefiltert werden soll</param>
        /// <returns>Anzahl von Kindern in der Datenbank</returns>
        public static int Count(CountChildGenderType childGenderType = CountChildGenderType.All, CountStateType parentCountState = CountStateType.All)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var children = db.Children.AsQueryable();

                // Eltern filter
                if (parentCountState != CountStateType.All)
                {
                    bool activateState = (parentCountState == CountStateType.Activated) ? true : false;
                    children = children.Where(p => p.Person.IsActive == activateState);
                }
                if (childGenderType != CountChildGenderType.All)
                {
                    bool isFemale = (childGenderType == CountChildGenderType.Female) ? true : false;
                    children = children.Where(c => c.IsFemale == isFemale);
                }

                return children.Count();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gibt den vollständigen Namen zurück
        /// </summary>
        public string FullName
        {
            get
            {
                return FormatUtils.GetFullName(FirstName, LastName);
            }
        }

        /// <summary>
        /// Gibt das Alter des Kindes zurück
        /// </summary>
        public int Age
        {
            get
            {
                return FormatUtils.GetAge(DateOfBirth);
            }
        }

        #endregion
    }
}
