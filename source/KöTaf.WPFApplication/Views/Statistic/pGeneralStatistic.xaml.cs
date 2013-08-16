/**
 * Class: pGeneralStatistic
 * @author Bjoern Bittner, Lucas Kögel
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
using KöTaf.WPFApplication.Helper;
using KöTaf.Utils.Parser;


namespace KöTaf.WPFApplication.Views.Statistic
{
    /// <summary>
    /// Page for Gerneral Statistic
    /// </summary>
    public partial class pGeneralStatistic : KPage
    {
        private double revenuesThisYear = 0;
        private double revenuesLastYear = 0;
        private double expensesThisYear = 0;
        private double expensesLastYear = 0;

        #region Constructor

        public pGeneralStatistic()
        {           
            InitializeComponent();
           
            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            
            GenerateGeneralStatistics();    
        }

        // This function must be defined in each class implementing KPage
        // If you don't want a toolbar, just leave it empty and don't add any button
        public override void defineToolbarContent()
        {
            // Here we declare a button for the toolbar
            // First parameter is the text of the button
            // Second parameter is a reference to any function which is called on button click
            // This function must have return value of type "void" and a single parameter of type "Button"
            //this.parentToolbar.addButton("Neue Serienbrief-Vorlage", createFormletter_Click);

            // This is an example how you can add your own custom elements to the toolbar if you need more than simple buttons
            //WrapPanel someControlElement = new WrapPanel();
            //this.parentToolbar.dpToolbarPanel.Children.Add( someControlElement );

            // This way you can remove everything from the toolbar, making it disappear and free for new buttons and stuff
            //this.parentToolbar.clearContent();
        }

        /// <summary>
        /// Ruft die einzelnen Setter auf, welche die GUI befüllen.
        /// </summary>
        private void GenerateGeneralStatistics()
        {
            SetCustomersCount();
            SetFullAgedPersonsCount();
            SetChildrenCount();
            SetChildrenFemaleCount();
            SetChildrenMaleCount();
            SetSponsorsCount();
            SetTeamMembersCount();
            SetRevenues();
            SetExpenses();
            SetProfit();
        }

        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven Kunden.
        /// </summary>
        private void SetCustomersCount()
        {
            int entireCustomers = 0;
            int activeCustomers = 0;
            
            // Count all persons
            entireCustomers = Person.Count();
            // Count only Activated persons
            activeCustomers = Person.Count(CountStateType.Activated);

            lEntireCustomers.Content = entireCustomers.ToString();
            lActiveCustomers.Content = activeCustomers.ToString();
        }

        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven volljährigen Kunden.
        /// </summary>
        private void SetFullAgedPersonsCount()
        {
            int entireFullAgedPersons = Person.Count(CountStateType.All, true);
            int activeFullAgedPersons = Person.Count(CountStateType.Activated, true);

            lEntireFullAgedPersons.Content = entireFullAgedPersons.ToString();
            lActiveFullAgedPersons.Content = activeFullAgedPersons.ToString();
        }

        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven Kinder.
        /// </summary>
        private void SetChildrenCount()
        {
            int entireChildren = 0;
            int activeChildren = 0;
            //Children
            entireChildren = Child.Count();
            activeChildren = Child.Count(CountChildGenderType.All, CountStateType.Activated);

            lEntireChildren.Content = entireChildren.ToString();
            lActiveChildren.Content = activeChildren.ToString();
        }

        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven weiblichen Kinder.
        /// </summary>
        private void SetChildrenFemaleCount()
        {
            int entireChildrenFemale = 0;
            int activeChildrenFemale = 0;
            //Children Female

            entireChildrenFemale = Child.Count(CountChildGenderType.Female);
            activeChildrenFemale = Child.Count(CountChildGenderType.Female, CountStateType.Activated);
            
            lEntireChildrenFemale.Content = entireChildrenFemale.ToString();
            lActiveChildrenFemale.Content = activeChildrenFemale.ToString();
        }

        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven männlichen Kinder.
        /// </summary>
        private void SetChildrenMaleCount()
        {
            int entireChildrenMale = 0;
            int activeChildrenMale = 0;
            //Children Male
            
            entireChildrenMale = Child.Count(CountChildGenderType.Male);
            activeChildrenMale = Child.Count(CountChildGenderType.Male, CountStateType.Activated);

            lEntireChildrenMale.Content = entireChildrenMale.ToString();
            lActiveChildrenMale.Content = activeChildrenMale.ToString();
        }

        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven Sponsoren.
        /// </summary>
        private void SetSponsorsCount()
        {
            int entireSponsors = 0;
            int activeSponsors = 0;
            //Sponsor
            entireSponsors = Sponsor.Count();
            activeSponsors = Sponsor.Count(CountStateType.Activated);
            
            lEntireSponsors.Content = entireSponsors.ToString();
            lActiveSponsors.Content = activeSponsors.ToString();
        }


        /// <summary>
        /// Setzt in der GUI die Anzahl der Aktiven und Inaktiven Team-Mitglieder.
        /// </summary>
        private void SetTeamMembersCount()
        {
            int entireTeamMembers = 0;
            int activeTeamMembers = 0;
            //Sponsor
            entireTeamMembers = Team.Count();
            activeTeamMembers = Team.Count(CountStateType.Activated);

            lEntireTeamMembers.Content = entireTeamMembers.ToString();
            lActiveTeamMembers.Content = activeTeamMembers.ToString();
        }

        /// <summary>
        /// Setzt in der GUI das Einkommen dieses und des letzten Jahres.
        /// </summary>
        private void SetRevenues()
        {
            CashClosureHelper helper = new CashClosureHelper(false);
            int currentYear = DateTime.Now.Year;            

            //Revenues of this year            
            DateTime from = BookingsHelper.makeDateSmall(new DateTime(currentYear, 1, 1));
            DateTime to = BookingsHelper.makeDateGreat(DateTime.Today);
            revenuesThisYear = helper.getAllRevenuesForPeriod(from, to);

            //Revenues of last year
            from = BookingsHelper.makeDateSmall(new DateTime(currentYear - 1, 1, 1));
            to = BookingsHelper.makeDateGreat(new DateTime(currentYear - 1, 12, 31));
            revenuesLastYear = helper.getAllRevenuesForPeriod(from, to);
            
            lRevenuesThisYear.Content = SafeStringParser.safeParseToMoney(revenuesThisYear, true);
            lRevenuesLastYear.Content = SafeStringParser.safeParseToMoney(revenuesLastYear, true);
        }

        /// <summary>
        /// Setzt in der GUI die Ausgaben dieses und des letzten Jahres.
        /// </summary>
        private void SetExpenses()
        {
            CashClosureHelper helper = new CashClosureHelper(false);
            int currentYear = DateTime.Now.Year;            

            //Expenses of this year            
            DateTime from = BookingsHelper.makeDateSmall(new DateTime(currentYear, 1, 1));
            DateTime to = BookingsHelper.makeDateGreat(DateTime.Today);
            expensesThisYear = helper.getAllExpensesForPeriod(from, to);

            //Expenses of last year
            from = BookingsHelper.makeDateSmall(new DateTime(currentYear - 1, 1, 1));
            to = BookingsHelper.makeDateGreat(new DateTime(currentYear - 1, 12, 31));
            expensesLastYear = helper.getAllExpensesForPeriod(from, to);

            lExpensesThisYear.Content = SafeStringParser.safeParseToMoney(expensesThisYear, true);
            lExpensesLastYear.Content = SafeStringParser.safeParseToMoney(expensesLastYear, true);
        }

        /// <summary>
        /// Setzt in der GUI den Gewinn dieses und des letzten Jahres.
        /// </summary>
        private void SetProfit()
        {
            lProfitThisYear.Content = SafeStringParser.safeParseToMoney((revenuesThisYear - expensesThisYear), true);
            lProfitLastYear.Content = SafeStringParser.safeParseToMoney((revenuesLastYear - expensesLastYear), true);
        }
        #endregion

    }
}
