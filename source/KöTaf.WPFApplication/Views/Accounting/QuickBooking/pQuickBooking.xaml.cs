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
using KöTaf.WPFApplication.Models;
using KöTaf.Utils.UserSession;
using KöTaf.Utils.Parser;
using KöTaf.WPFApplication.Helper;
using System.Globalization;

namespace KöTaf.WPFApplication.Views.Accounting.QuickBooking
{
    /// <summary>
    /// Author: Dietmar Sach
    /// </summary>
    public partial class pQuickBooking : KPage
    {
        private Brush bg0;
        private Brush bg1;
        private int? selectedGroup;
        private ComboBox cbGroup;

        public pQuickBooking()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("ACCOUNTING", "quickBookingButton"), saveAmounts);

            // ComboBox für die Gruppenauswahl

            // Label "Gruppe:"
            Label lb = new Label();
            lb.Content = "Gruppe:";
            lb.Width = 60;
            lb.Margin = new Thickness(20, 0, 0, 0);
            lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            
            // Gruppenauswahl
            this.cbGroup = new ComboBox();
            cbGroup.Items.Add(1);
            cbGroup.Items.Add(2);
            //cbGroup.SelectedIndex = 0;
            cbGroup.Width = 40;
            cbGroup.Margin = new Thickness(5, 0, 0, 0);
            cbGroup.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbGroup.SelectionChanged += filterGroup_SelectionChanged;

            this.parentToolbar.dpToolbarPanel.Children.Add(lb);
            this.parentToolbar.dpToolbarPanel.Children.Add(cbGroup);

            // Das DataGrid schluckt standardmäßig MouseWheel-Events, gebe daher das Event an den ScrollViewer weiter
            if (this.parentScrollViewer != null)
                QuickBookingDataGrid.PreviewMouseWheel += this.parentScrollViewer.OnMouseWheel;
            
        }

        /// <summary>
        /// DataGrid mit Personen füllen
        /// </summary>
        public void fillDataGrid()
        {
            IEnumerable<Person> persons = Person.GetPersons();

            if (this.selectedGroup == null)
                return;

            // Filtere hier nach Gruppe 1 oder 2
            if (this.selectedGroup == 1)
                persons = persons.Where(p => p.IsActive && p.Group == 1);

            else if (this.selectedGroup == 2)
                persons = persons.Where(p => p.IsActive && p.Group == 2);

            List<QuickBookingDataGridModel> personModels = new List<QuickBookingDataGridModel>();

            foreach (var person in persons)
                personModels.Add(new QuickBookingDataGridModel(person));

            QuickBookingDataGrid.ItemsSource = personModels;
            QuickBookingDataGrid.Items.Refresh();
        }

        /// <summary>
        /// Validiere die Eingaben der Beträge
        /// </summary>
        /// <param name="quickBookingDataGridModels">Buchungs-Modelle</param>
        /// <returns>ob valide</returns>
        private bool validateTextBoxAmount(out List<QuickBookingDataGridModel> quickBookingDataGridModels)
        {
            try
            {
                quickBookingDataGridModels = null;
                List<QuickBookingDataGridModel> successfulParsedModels = new List<QuickBookingDataGridModel>();

                // Hole aktuelle itemsSource aus dem DataGrid
                var itemsSource = QuickBookingDataGrid.ItemsSource as IEnumerable<QuickBookingDataGridModel>;

                if (itemsSource == null)
                    return false;

                // Hintergrund definieren
                string dataGridRowBackgroundError = IniParser.GetSetting("ACCOUNTING", "dataGridRowBackgroundError");
                Brush bgRed = (SolidColorBrush)(new BrushConverter().ConvertFrom(dataGridRowBackgroundError));
                
                bool valid = true;

                // Iteriere über DataGrid-Zeilen
                foreach (var item in itemsSource)
                {
                    var row = QuickBookingDataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;

                    // Undefinierter Fall
                    if (row == null)
                        continue;

                    // Wenn noch nicht passiert, hole Default-Werte für Hintergrundfarben
                    if (this.bg0 == null || this.bg1 == null)
                        if (row.AlternationIndex == 0)
                            this.bg0 = row.Background;
                        else if (row.AlternationIndex == 1)
                            this.bg1 = row.Background;

                    // Setze Hintergrundfarbe auf Default-Werte
                    if (row.AlternationIndex == 0)
                        row.Background = this.bg0;
                    else if (row.AlternationIndex == 1)
                        row.Background = this.bg1;

                    // Wandle Zeile in Model-Objekt um
                    QuickBookingDataGridModel model = (QuickBookingDataGridModel)row.Item;

                    // Wenn Textbox einfach leer gelassen wurde, handelt es sich nicht um einen validen Fall
                    if (string.IsNullOrEmpty(model.amount))
                        continue;

                    // Versuche Textbox auszulesen
                    double parsedAmount;
                    bool success = double.TryParse(model.amount.Replace(".", ","), out parsedAmount);

                    // Null ist auch keine gültige Eingabe
                    if (!(parsedAmount > 0))
                        success = false;

                    if (success)
                    {
                        // Schreibe in das Model den erfolgreich geparsten Betrag und füge Model zu temporärer Liste hinzu
                        model.parsedAmount = parsedAmount;
                        successfulParsedModels.Add(model);
                    }
                    else
                    {
                        // Färbe Hintergrund rot
                        row.Background = bgRed;
                        valid = false;
                    }
                }

                // Nur wenn alle Zeilen valide waren, schreibe Liste mit Beträgen zurück
                if (valid)
                    quickBookingDataGridModels = successfulParsedModels;

                return valid;

            }
            catch
            {
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "quickBookingError"));
                quickBookingDataGridModels = null;
                return false;
            }
        }

        /// <summary>
        /// Gibt die ID eines Kontos im Tausch für die Nummer eines Kontos im String-Format zurück
        /// </summary>
        /// <param name="accountNumberStr">Kontonummer als string</param>
        /// <param name="accountID">Konto-ID</param>
        /// <returns>gibt zurück ob geparst werden konnte</returns>
        private bool getAccountIDfromAccountNumberStr(string accountNumberStr, out int accountID)
        {
            try
            {
                int accNr = int.Parse(accountNumberStr);
                Account acc = Account.GetAccounts(null, null, accNr).FirstOrDefault();
                accountID = acc.AccountID;

                return true;
            }
            catch
            {
                accountID = -1;
                return false;
            }
        }

        /// <summary>
        /// Eingaben speichern und in Buchungen umwandeln
        /// </summary>
        /// <param name="btn">Referenz zum sendendem Button</param>
        private void saveAmounts(Button btn)
        {
            // Standard Hintergrund definieren
            Brush bgNormal = QuickBookingDataGrid.Background;

            // Validiere Textboxen und hole gleichzeitig eine Liste von Models, die alle geparsten Beträge und zugehörige Kunden enthält
            List<QuickBookingDataGridModel> quickBookingDataGridModels;
            bool allTextBoxesValid = validateTextBoxAmount(out quickBookingDataGridModels);

            if (allTextBoxesValid)
            {
                // Hole Standardkonten für Kunden und für Einnahmen-Kasse aus der config.ini
                string srcAccountNumberStr = IniParser.GetSetting("ACCOUNTING", "defaultCustomerAccountNr");
                int srcAccountID;
                bool successSrcAccParse = getAccountIDfromAccountNumberStr(srcAccountNumberStr, out srcAccountID);

                string targetAccountNumberStr = IniParser.GetSetting("ACCOUNTING", "defaultCashBoxAccountNr");
                int targetAccountID;
                bool successTargetAccParse = getAccountIDfromAccountNumberStr(targetAccountNumberStr, out targetAccountID);

                // Wenn diese Einträge in der Konfigurationsdatei nicht korrekt sind, breche ab
                if (!(successSrcAccParse && successTargetAccParse))
                {
                    MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "quickBookingError"));
                    return;
                }

                bool success = true;

                // Ansonsten führe die Buchungen durch
                foreach (var model in quickBookingDataGridModels)
                {
                    success = newQuickBooking(model.person, model.parsedAmount, srcAccountID, targetAccountID);
                    if (!success)
                        break;
                }

                if (success)
                {
                    MessageBoxEnhanced.Info(IniParser.GetSetting("ACCOUNTING", "quickBookingSuccess"));
                    QuickBookingDataGrid.ItemsSource = null;
                    QuickBookingDataGrid.Items.Refresh();
                    cbGroup.SelectedItem = null;
                }

            }
            else
                MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "quickBookingParsed"));
        }

        /// <summary>
        /// Legt neue Buchung an und aktualisiert Person
        /// </summary>
        /// <param name="person">Instanz von Person</param>
        /// <param name="amount">Betrag</param>
        /// <param name="srcAccountID">Quellkonto</param>
        /// <param name="targetAccountID">Zielkonto</param>
        /// <returns></returns>
        private bool newQuickBooking(Person person, double amount, int srcAccountID, int targetAccountID)
        {
            try
            {
                // aktualisiere lastPurchase des Kunden
                var personID = person.PersonID;
                var firstName = person.FirstName;
                var lastName = person.LastName;
                var zipCode = person.ZipCode;
                var city = person.City;
                var street = person.Street;
                var nationality = person.Nationality;
                var tableNo = person.TableNo;
                var phone = person.Phone;
                var mobileNo = person.MobileNo;
                var comment = person.Comment;
                var countryOfBirth = person.CountryOfBirth;
                var dateOfBirth = person.DateOfBirth;
                var email = person.Email;
                var group = person.Group;
                var validityStart = person.ValidityStart;
                var validityEnd = person.ValidityEnd;
                var maritalBirthday = person.MaritalBirthday;
                var maritalFirstName = person.MaritalFirstName;
                var maritalLastName = person.MaritalLastName;
                var maritalNationality = person.MaritalNationality;
                var lastPurchase = DateTime.Now;
                var maritalCountryOfBirth = person.MaritalCountryOfBirth;
                var maritalPhone = person.MaritalPhone;
                var maritalMobile = person.MaritalMobile;
                var maritalEmail = person.MaritalEmail;
                int familyStateID = person.FamilyState.FamilyStateID;
                int titleID = person.Title.TitleID;

                int? maritalTitleID;
                if (person.MaritalTitle != null)
                    maritalTitleID = person.MaritalTitle.TitleID;
                else
                    maritalTitleID = null;

                int? userAccountID;     // UserAccount ist manchmal null - wahrscheinlich ein Problem der Datenbank
                if (person.UserAccount != null)
                    userAccountID = person.UserAccount.UserAccountID;
                else
                    userAccountID = UserSession.userAccountID;

                Person.Update(personID, titleID, familyStateID, userAccountID, firstName,
                            lastName, zipCode, city, street, nationality, tableNo, phone,
                            mobileNo, comment, countryOfBirth, dateOfBirth, email, group,
                            validityStart, validityEnd, maritalBirthday, maritalFirstName,
                            maritalLastName, maritalNationality, lastPurchase, maritalCountryOfBirth,
                            maritalPhone, maritalMobile, maritalEmail, maritalTitleID);

                // Trage Buchung ein
                int userAccountIDbooking = UserSession.userAccountID;
                string description = IniParser.GetSetting("ACCOUNTING", "quickBookingDescription");

                Booking.Add(srcAccountID, targetAccountID, amount, person.PersonID, userAccountIDbooking, description);

                return true;
            }
            catch
            {
                // Bei Fehler gebe Kundendetails aus
                string error = IniParser.GetSetting("ERRORMSG", "newQuickBookingError").Replace("{0}", person.FirstName + " " + person.LastName + " (" + person.TableNo + ")");
                MessageBoxEnhanced.Error(error);
                return false;
            }
            
        }

        /// <summary>
        /// Änderung der Gruppe durch die ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void filterGroup_SelectionChanged(Object sender, EventArgs args)
        {
            try
            {
                ComboBox cbSender = sender as ComboBox;
                int group = (int)cbSender.SelectedItem;
                this.selectedGroup = group;
                this.fillDataGrid();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Wird beim Seitenwechsel aufgerufen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((KPage)sender).IsVisible)     // Beim Verlassen der Seite nichts machen
                return;

            // Fülle beim Seitenwechsel die Tabelle neu
            fillDataGrid();
        }
    }
}
