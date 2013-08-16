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
using KöTaf.Utils.ValidationTools;
using KöTaf.WPFApplication.Helper;
using DB = KöTaf.DataModel;
using KöTaf.WPFApplication.Models;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views.Client
{
    /// <summary>
    /// Interaktionslogik zum Editieren der Einkommen
    /// Grundlogik implementiert durch Antonios Fesenmeier
    /// Übernommen und zum Editieren angepasst/ erweitert durch Georg Schmid
    /// </summary>
    public partial class pEditClientRevenues : KPage
    {
        private int editCount = 0;
        private List<int> deletedRevenues;
        private ValidationTools _RevenuesValidator;
        private bool _RevenuesIsValid = true;
        private IList<RevenueModel> _Revenues;
        private IList<RevenueModel> _ValidRevenues;
        //private IEnumerable<RevenueModel> _Revenues;

        private readonly Person _currentPerson;

        public pEditClientRevenues(Person currentPerson)
        {
            _currentPerson = currentPerson;
            if (_currentPerson == null)
            {
                throw new Exception("Client cannot null");
            }
            InitializeComponent();

            Init();
        }

        #region Methods

        private void Init()
        {
            try
            {
                this._RevenuesValidator = new ValidationTools();
            }
            catch (Exception ex)
            {
                MessageBoxEnhanced.Error(ex.Message);
                return;
            }
            this._Revenues = new List<RevenueModel>();
            this.deletedRevenues = new List<int>();
            this._ValidRevenues = new List<RevenueModel>();

        }

        public override void defineToolbarContent()
        {
            //Datagrid erst jetzt befüllen
            fillRevenuesDataGrid(Revenue.GetRevenues(null, _currentPerson.PersonID));
        }

        public void clearErrorMsg()
        {
            this._RevenuesValidator.clearSB();
            _RevenuesIsValid = true;
        }

        public void getErrorMsg()
        {
            if (this._RevenuesIsValid == false)
            {
                MessageBoxEnhanced.Error(_RevenuesValidator.getErrorMsg().ToString());
                this._RevenuesValidator.clearSB();
            }
        }

        public void disableTabs()
        {
            pEditClientPerson pPerson = getPageFromTabs<pEditClientPerson>();
            int childs = Convert.ToInt32(pPerson.txtChildrens.Content);
            DB.FamilyState fS = pPerson.cbFamilyState.SelectedItem as DB.FamilyState;

            if (!(childs > 0) && !(fS.ShortName.Equals("VH") || fS.ShortName.Equals("LP")))
            {
                TabControl tC = this.parentTabControl;
                var childPartnerTab = ((Control)(tC.Items.GetItemAt(1))).IsEnabled = false;
            }
        }

        public IList<DB.Revenue> getRevenues()
        {
            IList<DB.Revenue> lRevs = new List<DB.Revenue>();
            foreach (RevenueModel rM in _ValidRevenues)
            {
                var type = rM.revType.ElementAt(0);
                var isType = type.Name;
                var types = DB.RevenueType.GetRevenueTypes();

                int actualType = 0;
                foreach (DB.RevenueType rt in types)
                {
                    if (rt.Name.Equals(isType))
                        actualType = rt.RevenueTypeID;
                }

                var aRev = new DB.Revenue();
                aRev.RevenueType = new RevenueType();
                aRev.Amount = Convert.ToDouble(rM.revAmount.Replace(".", ","));
                aRev.Description = rM.revDescription;
                aRev.RevenueType.RevenueTypeID = actualType;
                aRev.StartDate = rM.revStartDate;
                aRev.EndDate = rM.revEndDate;
                aRev.RevenueID = rM.revenueID;

                aRev.Person = _currentPerson;

                lRevs.Add(aRev);
            }
            return lRevs;
        }

        public List<int> getDeletedRevenues()
        {
            return deletedRevenues;
        }


        public bool checkRevenuesTab()
        {
            clearErrorMsg();
            int count = 1;
            if (_ValidRevenues.Count > 0)
                _ValidRevenues.Clear();

            // Durch die Revenues itterieren
            for (int rowIdx = 0; rowIdx < dtgRevenue2.Items.Count; rowIdx++)
            {
                var currentRow = dtgRevenue2.GetRow(rowIdx);
                if (currentRow == null)
                {
                    continue;
                }
                var actualRevenue = (RevenueModel)currentRow.Item;
                if (actualRevenue.isAdded == false && actualRevenue.revAmount == "0")
                    continue;


                // Revenue Typ
                ContentPresenter cpRevenueType = dtgRevenue2.Columns[0].GetCellContent(currentRow) as ContentPresenter;
                var combobox = DataGridHelper.GetVisualChild<ComboBox>(cpRevenueType);
                if (combobox.SelectedItem == null)
                {
                    this._RevenuesIsValid = false;
                    _RevenuesValidator.addError("Einkommen Nr. " + count, "Bitte geben Sie den Einkommenstyp von Einkommen Nr. " + count);
                }

                //StartDatum muss gesetzt sein
                ContentPresenter cpStartDate = dtgRevenue2.Columns[2].GetCellContent(currentRow) as ContentPresenter;
                var datepickerStart = DataGridHelper.GetVisualChild<DatePicker>(cpStartDate);
                if (datepickerStart.SelectedDate.HasValue)
                {
                    actualRevenue.revStartDate = datepickerStart.SelectedDate.Value;
                }
                else
                {
                    this._RevenuesIsValid = false;
                    _RevenuesValidator.addError("Startdatum Einkunft" + count, "Bitte geben Sie das Startdatum von Einkunft " + count + " an!");
                }

                //Enddatum muss nicht gesetzt sein, wenn aber dann nach Startdatum
                ContentPresenter cpEndDate = dtgRevenue2.Columns[3].GetCellContent(currentRow) as ContentPresenter;
                var datepickerEnd = DataGridHelper.GetVisualChild<DatePicker>(cpEndDate);
                if (datepickerEnd.SelectedDate.HasValue && datepickerStart.SelectedDate.HasValue)
                {
                    DateTime dateEnd = datepickerEnd.SelectedDate.GetValueOrDefault(System.DateTime.Today);
                    DateTime dateStart = datepickerStart.SelectedDate.GetValueOrDefault(System.DateTime.Today);
                    TimeSpan ts = dateEnd.Subtract(dateStart);

                    if (ts.TotalDays < (0))
                    {
                        MessageBox.Show("Bitte geben Sie ein korrektes Enddatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
                        datepickerEnd.SelectedDate = System.DateTime.Now;
                        this._RevenuesIsValid = false;
                    }
                    else
                        actualRevenue.revEndDate = datepickerEnd.SelectedDate.Value;
                }
                else
                {
                    this._RevenuesIsValid = false;
                    _RevenuesValidator.addError("Enddatum Einkunft" + count, "Bitte geben Sie das Enddatum von Einkunft " + count + " an!");
                }


                //Noch ein kleines Problem beim Wert überprüfen
                //Des mit isDouble geht noch nicht
                if (_RevenuesValidator.IsNullOrEmpty(actualRevenue.revAmount) == true)
                {
                    if (_RevenuesValidator.IsDouble("Betrag Einkunft" + count, actualRevenue.revAmount, "Bitte geben Sie den Betrag der Einkunft " + count + " an!") == false)
                        this._RevenuesIsValid = false;
                }


                //Hinzufügen zur Liste
                if (this._RevenuesIsValid == true)
                    _ValidRevenues.Add(actualRevenue);

                count++;
            }
            return this._RevenuesIsValid;

        }


        public void fillRevenuesDataGrid(IEnumerable<Revenue> revenues)
        {
            //Bisheriger Gesamtbetrag
            double amount = 0;

            IEnumerable<DB.RevenueType> revTypes = DB.RevenueType.GetRevenueTypes();

            // Combobox mit den RevenueTypes befüllen!
            var types = revTypes.ToList();
            var cbItems = types.ToList().Select(n => new CBItem
            {
                Name = n.Name
            });

            List<CBItem> tmpRevTypes = new List<CBItem>();
            tmpRevTypes.Add(null);

            foreach (CBItem cI in cbItems)
            {
                tmpRevTypes.Add(cI);
            }

            DateTime defaultDate = DateTime.Today;
            //DB.RevenueDetail revDetail = new RevenueDetail();
            //revDetail.Amount = 0;
            //revDetail.EndDate = defaultDate;
            //revDetail.StartDate = defaultDate;

            //vorhandene Revenues eintragen


            foreach (Revenue rev in revenues)
            {
                //Combobox individuell füllen Selektiertes zuerst.

                List<CBItem> existingRevType = new List<CBItem>();

                foreach (CBItem cI in cbItems)
                {
                    if (cI.Name == rev.RevenueType.Name)
                        existingRevType.Add(cI);
                }

                foreach (CBItem cI in cbItems)
                {
                    if (cI.Name != rev.RevenueType.Name)
                        existingRevType.Add(cI);
                }


                RevenueModel existingRev = new RevenueModel();
                existingRev.revType = existingRevType;
                existingRev.revDescription = rev.Description;
                existingRev.revAmount = rev.Amount.ToString();
                amount += rev.Amount;
                existingRev.isAdded = true;
                existingRev.revStartDate = rev.StartDate;
                existingRev.revEndDate = (DateTime)rev.EndDate;
                existingRev.revenueID = rev.RevenueID;


                this._Revenues.Add(existingRev);

            }


            //eine Neue Zeile hinzufügen
            RevenueModel newRev = new RevenueModel();
            newRev.revType = tmpRevTypes;
            newRev.revDescription = "";
            newRev.revStartDate = defaultDate;
            newRev.revEndDate = defaultDate;
            newRev.revenueID = 0;
            newRev.revAmount = "0";
            newRev.isAdded = false;

            this._Revenues.Add(newRev);
            dtgRevenue2.Items.Refresh();
            dtgRevenue2.ItemsSource = this._Revenues;
            dtgRevenue2.Items.Refresh();

            //Gesamtbetrag vorbelegen
            lblAmount.Content = SafeStringParser.safeParseToMoney(amount, true);
        }

        #endregion

        #region Events

        /// <summary>
        /// Dient zum Gültig setzen bzw. Löschen einer Einkunft
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbAddDelRev_Click(object sender, RoutedEventArgs e)
        {
            _RevenuesIsValid = true;
            var row = dtgRevenue2.SelectedCells.ElementAt(0);
            var currentSelectedRow = row.Item as RevenueModel;
            int rowId = dtgRevenue2.Items.IndexOf(currentSelectedRow);

            if (currentSelectedRow.isAdded == false)
            {
                //Prüfung der Einkunft
                //if (checkRevenuesTab())
                //{
                    if (currentSelectedRow != null)
                    {
                        currentSelectedRow.isAdded = true;
                        addNewRev();
                    }
                //}
                //else
                //{
                //    getErrorMsg();
                //}
            }
            else
            {

                var message = "Soll der gewählte Datensatz gelöscht werden?";

                var dialogResult = MessageBox.Show(message, "Bestätigung erfordert", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {

                    deletedRevenues.Add(currentSelectedRow.revenueID);
                    this._Revenues.RemoveAt(rowId);
                    dtgRevenue2.ItemsSource = this._Revenues;
                    

                    if (dtgRevenue2.Items.Count == 0)
                        addNewRev();
                }

            }

            dtgRevenue2.CommitEdit();
            dtgRevenue2.Items.Refresh();
            updateAmount();
        }


        private void addNewRev()
        {
            IEnumerable<DB.RevenueType> revTypes = DB.RevenueType.GetRevenueTypes();

            var types = revTypes.ToList();
            var cbItems = types.ToList().Select(n => new CBItem
            {
                Name = n.Name
            });
            List<CBItem> tmpRevTypes = new List<CBItem>();
            tmpRevTypes.Add(null);

            foreach (CBItem cI in cbItems)
            {
                tmpRevTypes.Add(cI);
            }

            DateTime defaultDate = DateTime.Today;

            RevenueModel newRev = new RevenueModel();
            newRev.revType = tmpRevTypes;
            newRev.revDescription = "";
            newRev.revStartDate = defaultDate;
            newRev.revEndDate = defaultDate;
            newRev.revenueID = 0;
            newRev.revAmount = "0";
            newRev.isAdded = false;

            this._Revenues.Add(newRev);
            dtgRevenue2.ItemsSource = this._Revenues;

            dtgRevenue2.CommitEdit();
            dtgRevenue2.Items.Refresh();
        }

        private void updateAmount()
        {
            dtgRevenue2.CommitEdit();
            double amount = 0;

                for (int rowIdx = 0; rowIdx < dtgRevenue2.Items.Count; rowIdx++)
                {
                    var currentRow = dtgRevenue2.GetRow(rowIdx);
                    if (currentRow == null)
                    {
                        continue;
                    }
                    var actualRevenue = (RevenueModel)currentRow.Item;

                    if (!actualRevenue.revAmount.Equals(""))
                    {
                        try
                        {
                            amount += Convert.ToDouble(actualRevenue.revAmount.Replace(".", ","));
                        }
                        catch
                        { }
                        lblAmount.Content = SafeStringParser.safeParseToMoney(amount, true);

                    }
                }
          
        }


        /// <summary>
        /// Mit Toni die Funktion besprechen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRevTyp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgRevenue2.SelectedCells.Count > 0)
            {
                var row = dtgRevenue2.SelectedCells.ElementAt(0);
                var currentSelectedRow = row.Item as RevenueModel;

                int rowId = dtgRevenue2.Items.IndexOf(currentSelectedRow);

                for (int rowIdx = 0; rowIdx < dtgRevenue2.Items.Count; rowIdx++)
                {
                    if (rowIdx == rowId)
                    {
                        var currentRow = dtgRevenue2.GetRow(rowIdx);
                        var actualChild = (RevenueModel)currentRow.Item;

                        ContentPresenter cbRevTyp = dtgRevenue2.Columns[0].GetCellContent(currentRow) as ContentPresenter;
                        var combobox = DataGridHelper.GetVisualChild<ComboBox>(cbRevTyp);
                        currentSelectedRow.revType = currentSelectedRow.revType.ToList();


                        List<CBItem> rowRevTypes = new List<CBItem>();
                        rowRevTypes.Add(combobox.SelectedItem as CBItem);

                        foreach (CBItem item in combobox.Items)
                        {
                            if (item != null)
                            {
                                if (!(item.Name.Equals((combobox.SelectedItem as CBItem).Name)))
                                {
                                    var newCBItem = new CBItem();
                                    newCBItem.Name = item.Name;
                                    rowRevTypes.Add(newCBItem);
                                }
                            }
                        }
                        currentSelectedRow.revType = rowRevTypes;
                    }

                }
                this._Revenues[rowId] = currentSelectedRow;
            }
        }

        /// <summary>
        /// Dient zum Berechnen des Gesamtbetrag
        /// </summary>
        /// <param name="sender">Auslösendes Objekt</param>
        /// <param name="e">EventArgs</param>
        private void dtgRevenue2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            updateAmount();
        }

        /// <summary>
        /// Dient zum Disablen der Tabs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KPage_Loaded(object sender, RoutedEventArgs e)
        {
            disableTabs();
        }
        #endregion
    }
}

