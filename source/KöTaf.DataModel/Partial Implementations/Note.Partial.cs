using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Note
    {
        // Statisch deswegen, da meist die Notizen nur gelesen werden, aber wenige neue hinzu kommen -> Laufzeit Verbesserung, sowie weniger DB Zugriffe!
        /// <summary>
        /// Liste aller Notizen
        /// </summary>
        private static IEnumerable<Note> _Notes;

        #region Static Methods

        /// <summary>
        /// Gibt alle Notizen zurück
        /// </summary>
        /// <param name="noteID">Filterung nach Notiz ID</param>
        /// <returns>Liste aller Notizen</returns>
        public static IEnumerable<Note> GetNotes(int? noteID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var notes = db.Notes.AsQueryable();

                if (noteID.HasValue)
                    notes = notes.Where(n => n.NoteID == noteID);

                return notes.ToList();
            }
        }

        /// <summary>
        /// Fügt eine neue Notiz der Datenbank hinzu
        /// </summary>
        /// <param name="name">Der Name der Notiz</param>
        /// <param name="description">Die Beschreibung der Notiz</param>
        /// <returns>Gibt die ID des Eintrags aus der Datenbank zurück</returns>
        public static int Add(string name, string description)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var note = new Note()
                {
                    Name = name,
                    Description = description, // max. Länge liegt bei 4000 Zeichen -> DB spezifisch
                    CreationDate = DateTime.Now
                };

                db.Notes.AddObject(note);
                db.SaveChanges();

                RefreshNoteList();

                return note.NoteID;
            }
        }

        /// <summary>
        /// Aktualisiert eine bestehende Notiz
        /// </summary>
        /// <param name="noteID">Notiz ID</param>
        /// <param name="name">Name der Notiz</param>
        /// <param name="description">Beschreibung der Notiz</param>
        public static void Update(int noteID, string name, string description)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var note = db.Notes.Single(n => n.NoteID == noteID);
                note.Name = name;
                note.Description = description;

                db.SaveChanges();

                RefreshNoteList();
            }
        }

        /// <summary>
        /// Löscht eine Notiz
        /// </summary>
        /// <param name="noteID">Die zu löschende Notiz ID</param>
        public static void Delete(int noteID)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var note = db.Notes.Single(n => n.NoteID == noteID);

                db.Notes.DeleteObject(note);
                db.SaveChanges();
            }

            RefreshNoteList();
        }

        /// <summary>
        /// Statische Notizliste zum holen von Notizen
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Note> GetNoteList()
        {
            if (_Notes == null)
                RefreshNoteList();
            return _Notes;
        }

        /// <summary>
        /// Aktualisiert die kompletten Notizen
        /// </summary>
        private static void RefreshNoteList()
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                _Notes = db.Notes.ToList();
            }
        }

        #endregion
    }
}
