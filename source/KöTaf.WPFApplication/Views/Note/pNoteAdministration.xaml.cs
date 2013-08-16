using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KöTaf.WPFApplication;
using KöTaf.DataModel;
using KöTaf.WPFApplication.Helper;
using System.Collections;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Interaktionslogik für pTeamAdministration.xaml
    /// </summary>
    public partial class pNoteAdministration : KPage
    {
        /// <summary>
        /// Liste aller Notizen
        /// </summary>
        private IEnumerable<Note> _Notes;
        /// <summary>
        /// Liste aller gelöschten Notizen
        /// </summary>
        private List<Note> _DeletedNotes;

        #region Constructor

        /// <summary>
        /// Konstruktor
        /// </summary>
        public pNoteAdministration()
        {
            InitializeComponent();

            Init();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            _DeletedNotes = new List<Note>();

            _Notes = Note.GetNotes();
            NotesDatagrid.ItemsSource = _Notes;
        }

        /// <summary>
        /// Toolbar Inhalt Definition
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("NOTES", "newNote"), NewNoteButton_Click);

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                NotesDatagrid.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
        }

        #endregion

        #region Events

        /// <summary>
        /// Notiz bearbeiten Klick Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Note currentSelectedNote = NotesDatagrid.SelectedItem as Note;
            if (currentSelectedNote != null)
            {
                KPage pageNoteAdmin = new KöTaf.WPFApplication.Views.pEditNote(currentSelectedNote); ;
                SinglePage singlePage = new SinglePage(IniParser.GetSetting("NOTES", "notes"), pageNoteAdmin);
            }
            else
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "loadNote"));
        }

        /// <summary>
        /// Methode zum löschen von Notizen
        /// </summary>
        /// <param name="note"></param>
        private void DeleteNote(Note note)
        {
            Note.Delete(note.NoteID);

            _DeletedNotes.Add(note);

            _Notes = _Notes.Except(_DeletedNotes);

            NotesDatagrid.ItemsSource = _Notes;
        }

        /// <summary>
        /// Lösch Button Klick Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Note currentSelectedNote = NotesDatagrid.SelectedItem as Note;
            if (currentSelectedNote != null)
            {
                var result = MessageBoxEnhanced.Question(IniParser.GetSetting("NOTES", "deleteNote"));
                if (result == MessageBoxResult.Yes)
                    DeleteNote(currentSelectedNote);
            }
        }

        /// <summary>
        /// Neue Notiz erstellen Klick Event
        /// </summary>
        /// <param name="button"></param>
        private void NewNoteButton_Click(Button button)
        {
            KPage pageNewNoteAdmin = new KöTaf.WPFApplication.Views.pNewNote();
            SinglePage singlePage = new SinglePage(IniParser.GetSetting("NOTES", "notes"), pageNewNoteAdmin);
        }

        #endregion
    }
}
