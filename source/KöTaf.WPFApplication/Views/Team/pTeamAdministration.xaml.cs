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
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.Printer;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.FileOperations;
using KöTaf.Utils.Parser;



namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik für pTeamAdministration.xaml
    /// Author: Antonios Fesenmeier, Georg Schmid
    /// </summary>
    public partial class pTeamAdministration : KPage
    {
        private DataGridPaging<Team> _DataGridPaging;
        private IEnumerable<Team> _Teams;
        private int pagingStartValue = 0;

        #region Constructor

        public pTeamAdministration(int? pagingStartValue = null)
        {
            InitializeComponent();

            if (pagingStartValue.HasValue)
                this.pagingStartValue = pagingStartValue.Value;

            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            _Teams = Team.GetTeams();
            IEnumerable<Team> teams = null; ;

            //Zeigt nur die Aktiven an
            if (_Teams != null)
            {
                teams = _Teams.OrderByDescending(t => t.IsActive);
            }

            //Übergibt dem Paging die Gesamtliste
            _DataGridPaging = new DataGridPaging<Team>(teams);
            _DataGridPaging.setStartOfDataGridItems(pagingStartValue);

            this.AddLogicalChild(new Views.pEditTeamMember(new Team()));

        }

        /// <summary>
        /// Legt den Toolbarcontent fest.
        /// Author: Georg Schmid
        /// </summary>
        public override void defineToolbarContent()
        {
            List<TeamSearchComboBoxItemModel> searchItems = new List<TeamSearchComboBoxItemModel>()
                {
                    new TeamSearchComboBoxItemModel { Value = IniParser.GetSetting("TEAM", "searchName"), SearchType = TeamSearchComboBoxItemModel.Type.FullName },
                    new TeamSearchComboBoxItemModel { Value = IniParser.GetSetting("TEAM", "searchLocation"), SearchType = TeamSearchComboBoxItemModel.Type.Residence },
                    new TeamSearchComboBoxItemModel { Value = IniParser.GetSetting("TEAM", "searchFunction"), SearchType = TeamSearchComboBoxItemModel.Type.TeamFunction }
                };

            this.parentToolbar.addButton(IniParser.GetSetting("TEAM", "newTeamMember"), pbNewTeamMember_Click);
            this.parentToolbar.addButton(IniParser.GetSetting("TEAM", "printList"), pbPrint_Click);
            this.parentToolbar.addPagingBar(firstSideProcessor, prevSideProcessor, nextSideProcessor, lastSideProcessor);
            this.parentToolbar.addSearchPanel(processKeyUp);

            // Combobox mit Liste von Objekten vom Typ "TeamSearchComboBoxItemModel" anzeigen
            this.parentToolbar.searchPanel.addComboBox<TeamSearchComboBoxItemModel>(searchItems);

            // Der Textbox eine KeyUp-Funktion zuweisen
            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);


            //DataGrid kann erst jetzt gefüllt werden, damit das Paging die PagingBar ansprechen kann.
            FillTeamDataGrid(_DataGridPaging.ActualSide());

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                dGTeamView.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
            
        }

        /// <summary>
        /// KeyUP-Event zum auslösen der Suche
        /// Author: Georg Schmid
        /// </summary>
        /// <param name="searchValue"></param>
        private void processKeyUp(string searchValue)
        {
            TeamSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<TeamSearchComboBoxItemModel>();

            IEnumerable<Team> teams = _Teams;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.SearchType)
                {
                    case TeamSearchComboBoxItemModel.Type.Residence:
                        teams = _Teams.Where(t => t.City.ToLower().Contains(searchValue));
                        break;

                    case TeamSearchComboBoxItemModel.Type.TeamFunction:
                        teams = _Teams.Where(t => t.TeamFunction.Name.ToLower().Contains(searchValue));
                        break;

                    case TeamSearchComboBoxItemModel.Type.FullName:
                        teams = _Teams.Where(t => t.FullName.ToLower().Contains(searchValue));
                        break;
                }
            }
            //_DataGridPaging mit neuem Listeninhalt neu initialisieren
            _DataGridPaging = new DataGridPaging<Team>(teams);
            //Das Datagrid befüllen lassen 
            FillTeamDataGrid(_DataGridPaging.FirstSide());
        }

        /// <summary>
        /// Befüllt das Datagrid mithilfe des Paginfs
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="teams"></param>
        public void FillTeamDataGrid(IEnumerable<Team> teams)
        {

            dGTeamView.ItemsSource = teams;
            ChangePagingBar();
        }


        #region ChangePagingBar


        /// <summary>
        /// Ermöglicht das Paging des Datagrids
        /// Author: Georg Schmid
        /// </summary>
        private void ChangePagingBar()
        {
            //Alle Buttons aktivieren
            this.parentToolbar.pagingBar.firstButton.IsEnabled = true;
            this.parentToolbar.pagingBar.prevButton.IsEnabled = true;
            this.parentToolbar.pagingBar.nextButton.IsEnabled = true;
            this.parentToolbar.pagingBar.lastButton.IsEnabled = true;

            //Label der PagingBar aktualisieren
            this.parentToolbar.pagingBar.fromBlock.Text = _DataGridPaging.GetStart().ToString();
            this.parentToolbar.pagingBar.toBlock.Text = _DataGridPaging.GetEnd().ToString();
            this.parentToolbar.pagingBar.totalBlock.Text = _DataGridPaging.GetTotal().ToString();

            //Nur Buttons zulassen, die möglich sind
            if (_DataGridPaging.GetStart() == 1 || _DataGridPaging.GetStart() == 0)
            {
                this.parentToolbar.pagingBar.firstButton.IsEnabled = false;
                this.parentToolbar.pagingBar.prevButton.IsEnabled = false;
            }
            if (_DataGridPaging.GetEnd() == _DataGridPaging.GetTotal())
            {
                this.parentToolbar.pagingBar.nextButton.IsEnabled = false;
                this.parentToolbar.pagingBar.lastButton.IsEnabled = false;
            }


        }

        private void prevSideProcessor(String str)
        {
            FillTeamDataGrid(_DataGridPaging.PrevSide());
        }

        private void nextSideProcessor(String str)
        {
            FillTeamDataGrid(_DataGridPaging.NextSide());
        }

        private void firstSideProcessor(String str)
        {
            FillTeamDataGrid(_DataGridPaging.FirstSide());
        }

        private void lastSideProcessor(String str)
        {
            FillTeamDataGrid(_DataGridPaging.LastSide());
        }

        #endregion


        #endregion

        #region Events

        /// <summary>
        /// Funktion des Druckbutton in der Toolbar
        /// </summary>
        /// <param name="button"></param>
        private void pbPrint_Click(Button button)
        {
            if (!LibreOffice.isLibreOfficeInstalled())
            {
                string warning = IniParser.GetSetting("ERRORMSG", "libre");
                MessageBoxEnhanced.Error(warning);
                return;
            }
            DataGrid allPrintData = new DataGrid();
            allPrintData.ItemsSource = _DataGridPaging.getItems();
            try
            {
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Team, allPrintData.ItemsSource);
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
        }


        /// <summary>
        /// Button zum erstellen eines neuen Teammitglieds
        /// Author: Georg Schmid
        /// </summary>
        /// <param name="Button"></param>
        private void pbNewTeamMember_Click(Button Button)
        {
            KPage pageNewTeamMember = new KöTaf.WPFApplication.Views.pNewTeamMember();

            SinglePage singlePage = new SinglePage(IniParser.GetSetting("TEAM", "newTeam"), pageNewTeamMember);
        }

        /// <summary>
        /// Button zum Bearbeiten eines Teammitgliedes
        /// /// Author: Georg Schmid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbEdit_Click(object sender, RoutedEventArgs e)
        {

            //Setzt den Rücksprungwert für das Paging
            pagingStartValue = _DataGridPaging.getStartOfDataGridItems();

            Team team = dGTeamView.SelectedItem as Team;
            if (team != null)
            {
                //_PageTools.ChangePage(new pEditTeamMember(team));
                KPage pageEditTeamMember = new KöTaf.WPFApplication.Views.pEditTeamMember(team, pagingStartValue);

                SinglePage singlePage = new SinglePage(IniParser.GetSetting("TEAM", "editSponsor"), pageEditTeamMember);
            }
            else
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "loadTeamMember"));
        }

        /// <summary>
        /// Ändert den Status des Teammitgliedes
        /// Author: Antonios Fesenmeier, Florian Wasilewski, Georg Schmid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbIsActive_Click(object sender, RoutedEventArgs e)
        {
            //Setzt den Rücksprungwert für das Paging
            pagingStartValue = _DataGridPaging.getStartOfDataGridItems();

            var currentSelectedPerson = dGTeamView.SelectedItem as Team;
            if (currentSelectedPerson != null)
            {
                var personId = currentSelectedPerson.TeamID;
                var state = currentSelectedPerson.IsActive;
                var message = string.Format(IniParser.GetSetting("TEAM", "confirmationFormatString"),
                                    currentSelectedPerson.LastName + " " + currentSelectedPerson.FirstName,
                                    ((state) ? IniParser.GetSetting("FILTER", "inactive") : IniParser.GetSetting("FILTER", "active")));

                var dialogResult = MessageBox.Show(message, IniParser.GetSetting("TEAM", "confirmationNeeded"), MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {
                    if (state)
                        Team.Deactivate(personId);
                    else
                        Team.Activate(personId);
                    //Veränderte Daten auf variable Schreiben
                    _Teams = Team.GetTeams();
                    if (this.parentToolbar.searchPanel.searchBox.Text == IniParser.GetSetting("APPSETTINGS", "search"))
                        processKeyUp("");
                    else
                        processKeyUp(this.parentToolbar.searchPanel.searchBox.Text);
                    _DataGridPaging.setStartOfDataGridItems(pagingStartValue);
                    FillTeamDataGrid(_DataGridPaging.ActualSide());
                }
            }
        }

        #endregion
    }
}
