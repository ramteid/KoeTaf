/**
 * Class: pLists
 * @author Lucas Kögel
 */

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
using KöTaf.DataModel;
using KöTaf.DataModel.Enums;
using System.Collections;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.UserSession;


namespace KöTaf.WPFApplication.Views.Lists
{
    /// <summary>
    /// Diese Page initialisiert die SavedLists, die AnyLists und die Listen-Erstellung.
    /// Unter Attributes können die Suchkriterien und die Anzuzeigenden-Daten geändert werden.
    /// </summary>
    public partial class pLists : KPage
    {
        private readonly UserAccount _UserAccount;

        #region Attributes
        /// <summary>
        /// Anzuzeigende Daten für die Kinder
        /// </summary>
        private List<String> displayableDataChildren = new List<String>{ " ", "Ausweisnummer", "Vorname", "Nachname", "Name", "Alter", "Geburtsdatum", "Ausweisgültigkeit bis max.", 
                                                      "Postleitzahl", "Wohnort", "Adresse" };
        
        /// <summary>
        /// Anzuzeigende Daten für die Partner
        /// </summary>
        private List<String> displayableDataPartnersAll = new List<String>{ " ", "Staatsangehörigkeit", "Ausweisnummer", "Vorname", "Nachname", "Name", "Alter", "Geburtsdatum", "Ausweisgültigkeit bis max.", 
                                                           "Postleitzahl", "Wohnort", "Adresse" };
        
        /// <summary>
        /// Anzuzeigende Daten für die Ausweisinhaber
        /// </summary>
        private List<String> displayableDataPassHolder = new List<String>{ " ", "Ausweisnummer", "Familienstand", "Staatsangehörigkeit", "Anzahl Kinder", "Partner-Name" , "Vorname", "Nachname", "Name", "Alter", "Geburtsdatum",
                                                              "Ausweisgültigkeit bis","Postleitzahl", "Wohnort", "Adresse", "Letzter Einkauf" };
        
        /// <summary>
        /// Anzuzeigende Daten für die Sponsoren
        /// </summary>
        private List<String> displayableDataSponsor = new List<String> { " ", "Name", "Firmenname", "Wohnort", "Postleitzahl", "Adresse" };
        
        /// <summary>
        /// Anzuzeigende daten für die Team-Mitgleider
        /// </summary>
        private List<String> displayableDataTeamMember = new List<String> { " ", "Name", "Wohnort", "Postleitzahl", "Adresse" };
        
        /// <summary>
        /// Liste der Attribute nach denen gefiltert werden kann für Ausweisinhaber
        /// </summary>
        private List<String> attributesPassHolder = new List<String>{ "Ausweisgültigkeit bis max.", "Familienstand", "Anzahl Kinder", "Staatsangehörigkeit",
                                                              "Postleitzahl", "Wohnort", "Alter", "Letzter Einkauf" };
        
        /// <summary>
        /// Liste der Attribute nach denen gefiltert werden kann für Partner
        /// </summary>
        private List<String> attributesPartnersAll = new List<String> { "Staatsangehörigkeit", "Postleitzahl", "Wohnort", "Alter" };
        
        /// <summary>
        /// Liste der Attribute nach denen gefiltert werden kann für Kinder
        /// </summary>
        private List<String> attributesChildren = new List<String> { "Postleitzahl", "Wohnort", "Alter" };
        
        #endregion

        #region Constructor

        public pLists()
        {
            this._UserAccount = UserSession.userAccount;

            InitializeComponent();
            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            //Setup GUI
            List<SimpleTabItem> listsTabList = new List<SimpleTabItem>();

            //Saved Lists
            KPage savedLists = new pSavedLists(_UserAccount, displayableDataChildren, displayableDataPartnersAll, displayableDataPassHolder, displayableDataSponsor, displayableDataTeamMember);
            listsTabList.Add(new SimpleTabItem("Gespeicherte Listen", savedLists));

            //Any Lists
            KPage anyLists = new pAnyLists(_UserAccount, displayableDataChildren, displayableDataPartnersAll, displayableDataPassHolder, displayableDataSponsor, attributesPassHolder, attributesPartnersAll, attributesChildren);
            listsTabList.Add(new SimpleTabItem("Beliebige Listen", anyLists));

            //List-Creation
            //Grant access to FilterSelection for admins
            KPage filterSelection;
            if ((this._UserAccount != null) && (this._UserAccount.IsAdmin))
            {
                filterSelection = new Formletter.pFormletterFilterSelection(true);
            } 
            else
            {
                filterSelection = new pAccessDenied();
            }
            listsTabList.Add(new SimpleTabItem("Listen-Erstellung", filterSelection));
            
            //Show the Pages
            SinglePage singlePage = new SinglePage(this, "Listen", listsTabList);
        }

        // This function must be defined in each class implementing KPage
        // If you don't want a toolbar, just leave it empty and don't add any button
        public override void defineToolbarContent()
        {
        }
    }
    #endregion

}