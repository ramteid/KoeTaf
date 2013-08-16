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

namespace KöTaf.WPFApplication.Views.Client
{
    /// <summary>
    /// Interaktionslogik zum Anlegen neuer Einkommen
    /// 
    /// Authors: Antonios Fesenmeier, Georg Schmid
    /// </summary>
    public partial class pNewClientRevenues : KPage
    {
        private int editCount = 0;
        private ValidationTools _RevenuesValidator;
        private bool _RevenuesIsValid = true;
        private IList<RevenueModel> _Revenues;
        private IList<RevenueModel> _ValidRevenues;
        //private IEnumerable<RevenueModel> _Revenues;

        public pNewClientRevenues()
        {
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
            this._ValidRevenues = new List<RevenueModel>();
        }

        public override void defineToolbarContent()
        {
            fillRevenuesDataGrid();
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


        /// <summary>
        /// Gibt eine Liste mit den abgelegten Einkommen zurück
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <returns></returns>
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

                lRevs.Add(aRev);
            }
            return lRevs;
        }

        /// <summary>
        /// Prüft die Einkommenstabelle auf Fehleingaben
        /// return: boolean
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <returns></returns>
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
            #region oldcheck
            //int count = 1;
            //if (_ValidRevenues.Count > 0)
            //    _ValidRevenues.Clear();

            //// Durch die Revenues itterieren
            //for (int rowIdx = 0; rowIdx < dtgRevenue2.Items.Count; rowIdx++)
            //{
            //    var currentRow = dtgRevenue2.GetRow(rowIdx);
            //    if (currentRow == null)
            //    {
            //        continue;
            //    }
            //    var actualRevenue = (RevenueModel)currentRow.Item;


            //    // Revenue Typ
            //    ContentPresenter cpRevenueType = dtgRevenue2.Columns[0].GetCellContent(currentRow) as ContentPresenter;
            //    var combobox = DataGridHelper.GetVisualChild<ComboBox>(cpRevenueType);
            //    if (combobox.SelectedItem == null)
            //    {
            //        this._RevenuesIsValid = false;
            //        _RevenuesValidator.addError("Einkommen Nr. " + count, "Bitte geben Sie den Einkommenstyp von Einkommen Nr. " + count);
            //    }

            //    //StartDatum muss gesetzt sein
            //    ContentPresenter cpStartDate = dtgRevenue2.Columns[2].GetCellContent(currentRow) as ContentPresenter;
            //    var datepickerStart = DataGridHelper.GetVisualChild<DatePicker>(cpStartDate);
            //    if (datepickerStart.SelectedDate.HasValue)
            //    {
            //        actualRevenue.revStartDate = datepickerStart.SelectedDate.Value;
            //    }
            //    else
            //    {
            //        this._RevenuesIsValid = false;
            //        _RevenuesValidator.addError("Startdatum Einkunft" + count, "Bitte geben Sie das Startdatum von Einkunft " + count + " an!");
            //    }

            //    //Enddatum muss nicht gesetzt sein, wenn aber dann nach Startdatum
            //    ContentPresenter cpEndDate = dtgRevenue2.Columns[3].GetCellContent(currentRow) as ContentPresenter;
            //    var datepickerEnd = DataGridHelper.GetVisualChild<DatePicker>(cpEndDate);
            //    if (datepickerEnd.SelectedDate.HasValue && datepickerStart.SelectedDate.HasValue)
            //    {
            //        DateTime dateEnd = datepickerEnd.SelectedDate.GetValueOrDefault(System.DateTime.Today);
            //        DateTime dateStart = datepickerStart.SelectedDate.GetValueOrDefault(System.DateTime.Today);
            //        TimeSpan ts = dateEnd.Subtract(dateStart);

            //        if (ts.TotalDays < (0))
            //        {
            //            MessageBox.Show("Bitte geben Sie ein korrektes Enddatum an!", "Fehlerhaftes Datum", MessageBoxButton.OK, MessageBoxImage.Error);
            //            datepickerEnd.SelectedDate = System.DateTime.Now;
            //        }
            //        else
            //            actualRevenue.revEndDate = datepickerEnd.SelectedDate.Value;
            //    }
            //    else
            //    {
            //        this._RevenuesIsValid = false;
            //        _RevenuesValidator.addError("Enddatum Einkunft" + count, "Bitte geben Sie das Enddatum von Einkunft " + count + " an!");
            //    }


            //    //Noch ein kleines Problem beim Wert überprüfen
            //    //Des mit isDouble geht noch nicht
            //    if (_RevenuesValidator.IsNullOrEmpty(actualRevenue.revAmount) == true)
            //    {
            //        if (_RevenuesValidator.IsDouble("Betrag Einkunft" + count, actualRevenue.revAmount, "Bitte geben Sie den Betrag der Einkunft " + count + " an!") == false)
            //            this._RevenuesIsValid = false;
            //    }


            //    //Hinzufügen zur Liste
            //    if (this._RevenuesIsValid == true)
            //        _ValidRevenues.Add(actualRevenue);

            //    count++;
            //}
            //return this._RevenuesIsValid;
            #endregion
        }

        /// <summary>
        /// Befüllt initial das Datagrid mit einer Einkommens-Zeile, welche editiert werden kann.
        /// Anschließend wird die Itemsource refreshed.
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        public void fillRevenuesDataGrid()
        {
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

            RevenueModel newRev = new RevenueModel();
            newRev.revType = tmpRevTypes;
            newRev.revDescription = "";
            newRev.revStartDate = defaultDate;
            newRev.revEndDate = defaultDate;
            newRev.revAmount = "0";
            newRev.isAdded = false;

            this._Revenues.Add(newRev);
            dtgRevenue2.Items.Refresh();
            dtgRevenue2.ItemsSource = this._Revenues;
            dtgRevenue2.Items.Refresh();
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegiert das Löschen/Hinzufügen eines Einkommens
        /// 
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbAddDelRev_Click(object sender, RoutedEventArgs e)
        {
            var row = dtgRevenue2.SelectedCells.ElementAt(0);
            var currentSelectedRow = row.Item as RevenueModel;
            int rowId = dtgRevenue2.Items.IndexOf(currentSelectedRow);

            if (currentSelectedRow.isAdded == false)
            {
                if (currentSelectedRow != null)
                {
                    currentSelectedRow.isAdded = true;
                    addNewRev();
                }
            }
            else
            {
                this._Revenues.RemoveAt(rowId);
                dtgRevenue2.ItemsSource = this._Revenues;
            }
           
            dtgRevenue2.CommitEdit();
            dtgRevenue2.Items.Refresh();
        }

        /// <summary>
        /// Entfernt die aktuell markierte Zeile aus dem Datagrid, außer das Datagrid ist leer.
        ///  
        /// Author: Antonios Fesenmeier, Georg Schmid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbDelRev_Click(object sender, RoutedEventArgs e)
        {
            var row = dtgRevenue2.SelectedCells.ElementAt(0);
            var currentSelectedRow = row.Item as RevenueModel;

            int rowId = dtgRevenue2.Items.IndexOf(currentSelectedRow);

            #region old
            //if (currentSelectedRow.isAdded == false)
            //{
            //    if (currentSelectedRow != null)
            //    {
            //        currentSelectedRow.isAdded = true;

            //        for (int rowIdx = 0; rowIdx < dtgRevenue2.Items.Count; rowIdx++)
            //        {
            //            var currentRow = dtgRevenue2.GetRow(rowIdx);
            //            var actualChild = (RevenueModel)currentRow.Item;

            //            ContentPresenter cbRevTyp = dtgRevenue2.Columns[0].GetCellContent(currentRow) as ContentPresenter;
            //            var combobox = DataGridHelper.GetVisualChild<ComboBox>(cbRevTyp);
            //            currentSelectedRow.revType = currentSelectedRow.revType.ToList();

            //            List<CBItem> rowRevTypes = new List<CBItem>();
            //            rowRevTypes.Add(combobox.SelectedItem as CBItem);

            //            foreach (CBItem item in combobox.Items)
            //            {

            //                if (!(item.Name.Equals((combobox.SelectedItem as CBItem).Name)))
            //                {
            //                    var newCBItem = new CBItem();
            //                    newCBItem.Name = item.Name;
            //                    rowRevTypes.Add(newCBItem);
            //                }
            //            }
            //            currentSelectedRow.revType = rowRevTypes;
            //        }
            //        this._Revenues[rowId] = currentSelectedRow;
            //    }

            // Neue Row mit Revenue anlegen!                
            //IEnumerable<DB.RevenueType> revTypes = DB.RevenueType.GetRevenueTypes();

            //var types = revTypes.ToList();
            //var cbItems = types.ToList().Select(n => new CBItem
            //{
            //    Name = n.Name
            //});
            //List<CBItem> tmpRevTypes = new List<CBItem>();

            //foreach (CBItem cI in cbItems)
            //{
            //    tmpRevTypes.Add(cI);
            //}

            //DateTime defaultDate = DateTime.Today;
            //DB.RevenueDetail revDetail = new RevenueDetail();
            //revDetail.Amount = 0;
            //revDetail.EndDate = defaultDate;
            //revDetail.StartDate = defaultDate;

            //RevenueModel newRev = new RevenueModel();
            //newRev.revType = tmpRevTypes;
            //newRev.revDescription = "";
            //newRev.revDetail = revDetail;
            //newRev.revStartDate = defaultDate;
            //newRev.revEndDate = defaultDate;
            //newRev.revAmount = "0";
            //newRev.isAdded = false;

            //this._Revenues.Add(newRev);
            //dtgRevenue2.ItemsSource = this._Revenues;
            //}
            //else
            #endregion
            {
                this._Revenues.RemoveAt(rowId);
                dtgRevenue2.ItemsSource = this._Revenues;
            }

            if (dtgRevenue2.Items.Count == 0)
                addNewRev();

            dtgRevenue2.CommitEdit();
            dtgRevenue2.Items.Refresh();
        }

        /// <summary>
        /// Fügt dem Datagrid eine neue Zeile mit einem Einkommen hinzu, welches editiert werden kann.
        /// Anschließend erfolgt ein Commit und ein Refresh.
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
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
            newRev.revAmount = "0";
            newRev.isAdded = false;

            this._Revenues.Add(newRev);
            dtgRevenue2.ItemsSource = this._Revenues;

            dtgRevenue2.CommitEdit();
            dtgRevenue2.Items.Refresh();
        }

        /// <summary>
        /// Hiermit werden die Änderungen in der Einkommensart-Combobox registriert und abgespeichert!!!
        /// 
        /// Author: Antonios Fesenmeier
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

        #endregion

        /// <summary>
        /// Beim wechseln der aktiv markierten Zelle wird das Datagrid auf Beträge geprüft! Der Gesamtbetrag 
        /// wird folgedessen auf das Label lblAmount geschrieben.
        /// 
        /// Author: Antonios Fesenmeier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtgRevenue2_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            dtgRevenue2.CommitEdit();
            double amount = 0;

            if (dtgRevenue2.SelectedCells.Count > 0)
            {
                var row = dtgRevenue2.SelectedCells.ElementAt(0);
                var currentSelectedRow = row.Item as RevenueModel;

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
                        amount += Convert.ToDouble(actualRevenue.revAmount.Replace(".", ","));
                        lblAmount.Content = amount.ToString();
                    }                                   
                }
                amount += Convert.ToDouble(currentSelectedRow.revAmount.Replace(".", ","));
            }
        }

    }
}

