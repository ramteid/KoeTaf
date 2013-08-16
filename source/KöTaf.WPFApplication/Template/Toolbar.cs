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
using KöTaf.WPFApplication.Views;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Toolbar des Templates
    /// </summary>       
    public class Toolbar
    {
        public DockPanel dpToolbarPanel { get; private set; }   // DARF KEIN GRID SEIN
        public WrapPanel buttonShrinkPanel { get; private set; }
        public Frame relatedFrame { get; private set; }
        public KPage relatedPage { get; private set; }
        public TabControl relatedTabControl { get; set; }
        public SearchPanel searchPanel { get; private set; }
        public PagingBar pagingBar { get; private set; }
        public DateRangePanel dateRangePanel { get; private set; }
        private List<ButtonFunctionMapping> buttonFunctionMappings;

        public Toolbar(Frame relatedFrame, KPage relatedPage)
        {
            this.relatedFrame = relatedFrame;
            this.relatedPage = relatedPage;
            this.buttonFunctionMappings = new List<ButtonFunctionMapping>();
            dpToolbarPanel = new DockPanel();
            dpToolbarPanel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(IniParser.GetSetting("APPSETTINGS", "toolbarColor")));
            dpToolbarPanel.VerticalAlignment = VerticalAlignment.Top;
            clearContent();
        }

        /// <summary>
        /// Inhalt löschen
        /// </summary>
        public void clearContent()
        {
            this.dpToolbarPanel.Children.Clear();
            buttonShrinkPanel = new WrapPanel();
            dpToolbarPanel.Children.Add(buttonShrinkPanel);
        }

        /// <summary>
        /// Einen Button zur Toolbar hinzufügen
        /// </summary>
        /// <param name="label">Beschriftung</param>
        /// <param name="buttonFunction">Funktion, die bei Click aufgerufen wird. Benötigt genau 1 Parameter vom Typ Button</param>
        /// <returns>der geklickte Button</returns>
        public Button addButton(string label, Action<Button> buttonFunction)
        {
            Button button = new Button();
            button.Content = label;
            button.Margin = new Thickness(16,15,5,15);
            button.Click += buttonProcessor;
            buttonShrinkPanel.Children.Add(button);
            buttonFunctionMappings.Add( new ButtonFunctionMapping(button, buttonFunction) );
            return button;
        }

        /// <summary>
        /// Wird direkt vom Button bei Click aufgerufen
        /// Ruft die zum Button gehörige Funktion auf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonProcessor(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() != typeof(Button))
            {
                return;
            }

            foreach (var buttonFunctionMapping in buttonFunctionMappings)
            {
                if (buttonFunctionMapping.button.Equals(sender))
                    buttonFunctionMapping.buttonFunction((Button)sender);
            }
        }

        /// <summary>
        /// Panel mit Suche hinzufügen
        /// </summary>
        /// <param name="searchFunction"></param>
        public void addSearchPanel(Action<string> searchFunction)
        {
            searchPanel = new SearchPanel(searchFunction);
            dpToolbarPanel.Children.Add(searchPanel.panel);
        }

        /// <summary>
        /// PagingBar hinzufügen
        /// </summary>
        /// <param name="firstFunction">Funktion für Anfang. Benötigt genau 1 Parameter vom Typ string</param>
        /// <param name="prevFunction">Funktion für Vorheriges. Benötigt genau 1 Parameter vom Typ string</param>
        /// <param name="nextFunction">Funktion für Nächstes. Benötigt genau 1 Parameter vom Typ string</param>
        /// <param name="lastFunction">Funktion für Ende. Benötigt genau 1 Parameter vom Typ string</param>
        public void addPagingBar(Action<string> firstFunction, Action<string> prevFunction, Action<string> nextFunction, Action<string> lastFunction)
        {
            pagingBar = new PagingBar(firstFunction,prevFunction,nextFunction,lastFunction);
            dpToolbarPanel.Children.Add(pagingBar.outerPanel);
        }

        /// <summary>
        /// Ein Panel hinzufügen, das die Angabe von Start-Datum und End-Datum ermöglicht
        /// </summary>
        /// <param name="dateFromProcessingFunction">Funktion, die beim Ändern des Start-Datums aufgerufen wird. Benötigt genau 1 Parameter vom Typ DateTime</param>
        /// <param name="dateToProcessingFunction">Funktion, die beim Ändern des End-Datums aufgerufen wird. Benötigt genau 1 Parameter vom Typ DateTime</param>
        /// <param name="datePickerFrom">DatePicker-Referenz für Start-Datum</param>
        /// <param name="datePickerTo">DatePicker-Referenz für End-Datum</param>
        public void addDateRangeSelector(Action<DateTime> dateFromProcessingFunction, Action<DateTime> dateToProcessingFunction, ref DatePicker datePickerFrom, ref DatePicker datePickerTo)
        {
            this.dateRangePanel = new DateRangePanel(dateFromProcessingFunction, dateToProcessingFunction, ref datePickerFrom, ref datePickerTo);
            dpToolbarPanel.Children.Add(dateRangePanel.panel);
        }
    }

    /// <summary>
    /// Klasse, die eine Zuordnung von Funktionen mit beliebiger Parameterliste zu Buttons ermöglicht
    /// </summary>
    class ButtonFunctionMapping
    {
        public Button button { get; private set; }
        public Action<Button> buttonFunction { get; private set; }

        public ButtonFunctionMapping(Button button, Action<Button> buttonFunction)
        {
            this.button = button;
            this.buttonFunction = buttonFunction;
        }
    }
}
