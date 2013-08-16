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


namespace KöTaf.WPFApplication.Views.Statistic
{
    /// <summary>
    /// Page for Gerneral Statistic
    /// </summary>
    public partial class pGeneralStatistic : KPage
    {
        private readonly UserAccount _UserAccount;
        private double revenuesThisYear = 0;
        private double revenuesLastYear = 0;
        private double expensesThisYear = 0;
        private double expensesLastYear = 0;

        #region Constructor

        public pGeneralStatistic(UserAccount userAccount)
        {
            this._UserAccount = userAccount;
           
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

        private void SetCustomersCount()
        {
            int entireCustomers = 0;
            int activeCustomers = 0;
            //Families
            // Count all persons
            entireCustomers = Person.Count();
            // Count only Activated persons
            activeCustomers = Person.Count(CountStateType.Activated);

            tBEntireCustomers.Text = entireCustomers.ToString();
            tBActiveCustomers.Text = activeCustomers.ToString();
        }

        private void SetFullAgedPersonsCount()
        {
            int entireFullAgedPersons = Person.Count(CountStateType.All, true);
            int activeFullAgedPersons = Person.Count(CountStateType.Activated, true);

            tBEntireFullAgedPersons.Text = entireFullAgedPersons.ToString();
            tBActiveFullAgedPersons.Text = activeFullAgedPersons.ToString();
        }

        private void SetChildrenCount()
        {
            int entireChildren = 0;
            int activeChildren = 0;
            //Children
            entireChildren = Child.Count();
            activeChildren = Child.Count(CountChildGenderType.All, CountStateType.Activated);

            tBEntireChildren.Text = entireChildren.ToString();
            tBActiveChildren.Text = activeChildren.ToString();
        }

        private void SetChildrenFemaleCount()
        {
            int entireChildrenFemale = 0;
            int activeChildrenFemale = 0;
            //Children Female

            entireChildrenFemale = Child.Count(CountChildGenderType.Female);
            activeChildrenFemale = Child.Count(CountChildGenderType.Female, CountStateType.Activated);
            
            tBEntireChildrenFemale.Text = entireChildrenFemale.ToString();
            tBActiveChildrenFemale.Text = activeChildrenFemale.ToString();
        }

        private void SetChildrenMaleCount()
        {
            int entireChildrenMale = 0;
            int activeChildrenMale = 0;
            //Children Male
            
            entireChildrenMale = Child.Count(CountChildGenderType.Male);
            activeChildrenMale = Child.Count(CountChildGenderType.Male, CountStateType.Activated);

            tBEntireChildrenMale.Text = entireChildrenMale.ToString();
            tBActiveChildrenMale.Text = activeChildrenMale.ToString();
        }

        private void SetSponsorsCount()
        {
            int entireSponsors = 0;
            int activeSponsors = 0;
            //Sponsor
            entireSponsors = Sponsor.Count();
            activeSponsors = Sponsor.Count(CountStateType.Activated);
            
            tBEntireSponsors.Text = entireSponsors.ToString();
            tBActiveSponsors.Text = activeSponsors.ToString();
        }

        private void SetTeamMembersCount()
        {
            int entireTeamMembers = 0;
            int activeTeamMembers = 0;
            //Sponsor
            entireTeamMembers = Team.Count();
            activeTeamMembers = Team.Count(CountStateType.Activated);

            tBEntireTeamMembers.Text = entireTeamMembers.ToString();
            tBActiveTeamMembers.Text = activeTeamMembers.ToString();
        }

        private void SetRevenues()
        {
            //Revenues of this year and last year
            revenuesThisYear = AdditionalRecord.GetAmountSum(true, YearType.Current);
            revenuesLastYear = AdditionalRecord.GetAmountSum(true, YearType.Past);

            tBRevenuesThisYear.Text = revenuesThisYear.ToString();
            tBRevenuesLastYear.Text = revenuesLastYear.ToString(); 
        }

        private void SetExpenses()
        {
            //Expenses of this year and last year
            expensesThisYear = AdditionalRecord.GetAmountSum(false, YearType.Current);
            expensesLastYear = AdditionalRecord.GetAmountSum(false, YearType.Past);

            tBExpensesThisYear.Text = expensesThisYear.ToString();
            tBExpensesLastYear.Text = expensesLastYear.ToString();
        }

        private void SetProfit()
        {
            tBProfitThisYear.Text = (revenuesThisYear - expensesThisYear).ToString();
            tBProfitLastYear.Text = (revenuesLastYear - expensesLastYear).ToString();
        }
        #endregion

    }
}
