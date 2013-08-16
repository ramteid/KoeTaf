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
using KöTaf.WPFApplication;
using KöTaf.WPFApplication.Models;
using KöTaf.WPFApplication.Helper;
using KöTaf.WPFApplication.Template;
using KöTaf.Utils.Printer;
using KöTaf.Utils.FileOperations;
using KöTaf.Utils.Parser;


namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Author: Georg Schmid, Antonios Fesenmeier
    /// Interaktionslogik für pSponsorAdministration.xaml
    /// </summary>
    public partial class pSponsorAdministration : KPage
    {
        private IEnumerable<Sponsor> _Sponsors;
        private DataGridPaging<Sponsor> _DataGridPaging;
        private int pagingStartValue = 0;

        #region Constructor

        public pSponsorAdministration(int? pagingStartValue = null)
        {
            InitializeComponent();

            if (pagingStartValue.HasValue)
                this.pagingStartValue = pagingStartValue.Value;

            Init();
        }

        #endregion

        #region Method

        private void Init()
        {
            _Sponsors = Sponsor.GetSponsors();

            //Übergibt dem Paging die Gesamtliste
            _DataGridPaging = new DataGridPaging<Sponsor>(_Sponsors);
            _DataGridPaging.setStartOfDataGridItems(pagingStartValue);

        }

        /// <summary>
        ///  Legt den ToolbarContent fest
        ///  Author: Georg Schmid
        /// </summary>
        public override void defineToolbarContent()
        {
            List<SponsorSearchComboBoxItemModel> searchItems = new List<SponsorSearchComboBoxItemModel>()
                {
                    new SponsorSearchComboBoxItemModel { Value = "Name", SearchType = SponsorSearchComboBoxItemModel.Type.Name },
                    new SponsorSearchComboBoxItemModel { Value = "Wohnort", SearchType = SponsorSearchComboBoxItemModel.Type.Residence },
                    new SponsorSearchComboBoxItemModel { Value = "Typ", SearchType = SponsorSearchComboBoxItemModel.Type.FundingType },
                    new SponsorSearchComboBoxItemModel { Value = "Firmenname", SearchType = SponsorSearchComboBoxItemModel.Type.CompanyName }
                };

            this.parentToolbar.addButton("Neuer Sponsor", pbNewSponsor_Click);
            this.parentToolbar.addButton("Liste drucken", pbPrint_Click);
            this.parentToolbar.addPagingBar(firstSideProcessor, prevSideProcessor, nextSideProcessor, lastSideProcessor);
            
            this.parentToolbar.addSearchPanel(processKeyUp);

            // Combobox mit Liste von Objekten vom Typ "TeamSearchComboBoxItemModel" anzeigen
            this.parentToolbar.searchPanel.addComboBox<SponsorSearchComboBoxItemModel>(searchItems);
            
            // Der Textbox eine KeyUp-Funktion zuweisen
            this.parentToolbar.searchPanel.addActionKeyUpTextbox(processKeyUp);

            //DataGrid kann erst jetzt gefüllt werden, damit das Paging die PagingBar ansprechen kann.
            FillSponsorDataGrid(_DataGridPaging.ActualSide());

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                dGSponsorView.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
            
        }

        /// <summary>
        ///  KeyUp Event zum Auslösen der Suche
        ///  Author: Georg Schmid
        /// </summary>
        /// <param name="searchValue"></param>
        private void processKeyUp(string searchValue)
        {
            SponsorSearchComboBoxItemModel searchType = this.parentToolbar.searchPanel.getComboBoxSelectedItem<SponsorSearchComboBoxItemModel>();

            IEnumerable<Sponsor> sponsors = _Sponsors;

            if (searchType != null && !string.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.ToLower();

                switch (searchType.SearchType)
                {
                    case SponsorSearchComboBoxItemModel.Type.Residence:
                        sponsors = _Sponsors.Where(s => s.ResidentialAddress.ToLower().Contains(searchValue));
                        break;


                    case SponsorSearchComboBoxItemModel.Type.FundingType:
                        sponsors = _Sponsors.Where(s => s.FundingType.Name.ToLower().Contains(searchValue));
                        break;

                    case SponsorSearchComboBoxItemModel.Type.Name:
                        sponsors = _Sponsors.Where(s => s.FullName.ToLower().Contains(searchValue));
                        break;

                    case SponsorSearchComboBoxItemModel.Type.CompanyName:
                        sponsors = _Sponsors.Where(s => !(s.CompanyName == null));
                        sponsors = sponsors.Where(s => s.CompanyName.ToLower().Contains(searchValue));
                        break;
                }
            }
            //_DataGridPaging mit neuem Listeninhalt neu initialisieren
            _DataGridPaging = new DataGridPaging<Sponsor>(sponsors);
            FillSponsorDataGrid(_DataGridPaging.FirstSide());
        }


        /// <summary>
        ///  Befüllt Datagrid mit Daten
        ///  Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sponsors"></param>
        private void FillSponsorDataGrid(IEnumerable<Sponsor> sponsors)
        {
            dGSponsorView.ItemsSource = sponsors;
            ChangePagingBar();
        }

        /// <summary>
        ///  Ermöglicht Paging des Datagrids
        ///  Author: Georg Schmid
        /// </summary>
        #region ChagePagingBar
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
            FillSponsorDataGrid(_DataGridPaging.PrevSide());
        }

        private void nextSideProcessor(String str)
        {
            FillSponsorDataGrid(_DataGridPaging.NextSide());
        }

        private void firstSideProcessor(String str)
        {
            FillSponsorDataGrid(_DataGridPaging.FirstSide());
        }

        private void lastSideProcessor(String str)
        {
            FillSponsorDataGrid(_DataGridPaging.LastSide());
        }
        #endregion

        #endregion

        #region Events
        

        private void pbNewSponsor_Click(Button button)
        {
            KPage pageNewSponsor = new KöTaf.WPFApplication.Views.pNewSponsor();

            SinglePage singlePage = new SinglePage("Neuen Sponsor anlegen", pageNewSponsor);
        }

        private void pbEdit_Click(object sender, RoutedEventArgs e)
        {
            //Setzt den Rücksprungwert für das Paging
            pagingStartValue = _DataGridPaging.getStartOfDataGridItems();

            Sponsor sponsor = this.dGSponsorView.SelectedItem as Sponsor;
            if (sponsor != null)
            {
              
                KPage pageEditSponsor = new KöTaf.WPFApplication.Views.pEditSponsor(sponsor, pagingStartValue);

                SinglePage singlePage = new SinglePage("Sponsor bearbeiten", pageEditSponsor);
            }                
            else
                MessageBoxEnhanced.Error("Es ist ein Fehler beim Laden des aktuellen Sponsors aufgetreten");
        }

        /// <summary>
        ///  Ermöglicht das Aktivieren/Deaktivieren eines Sponsors
        ///  Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbIsActiv_Click(object sender, RoutedEventArgs e)
        {
            //Setzt den Rücksprungwert für das Paging
            pagingStartValue = _DataGridPaging.getStartOfDataGridItems();

            var currentSelectedSponsor = dGSponsorView.SelectedItem as Sponsor;
            if (currentSelectedSponsor != null)
            {
                var personId = currentSelectedSponsor.SponsorID;
                var state = currentSelectedSponsor.IsActive;
                var message = string.Format("Soll die ausgewählte Person {0} auf {1} gesetzt werden?",
                                    currentSelectedSponsor.FirstName,
                                    ((state) ? "inaktiv" : "aktiv"));

                var dialogResult = MessageBox.Show(message, "Bestätigung erfordert", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {
                    if (state)
                        Sponsor.Deactivate(personId);
                    else
                        Sponsor.Activate(personId);
                    _Sponsors = Sponsor.GetSponsors();
                    if (this.parentToolbar.searchPanel.searchBox.Text == "Suche ...")
                        processKeyUp("");
                    else
                        processKeyUp(this.parentToolbar.searchPanel.searchBox.Text);
                    _DataGridPaging.setStartOfDataGridItems(pagingStartValue);
                    FillSponsorDataGrid(_DataGridPaging.ActualSide());
                }
            }
        }



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
                KöTaf.Utils.Printer.PrintModul pM = new Utils.Printer.PrintModul(PrintType.Sponsor, allPrintData.ItemsSource);
            }catch (Exception ex){
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
        }

#endregion
    }        
}
