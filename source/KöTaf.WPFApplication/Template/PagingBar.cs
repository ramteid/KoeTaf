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
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Georg Schmid
    /// Dient der Erzeugung einer PagingBar in der Toolbar für ein DataGridPaging
    /// </summary>        
    public class PagingBar
    {
        public Button firstButton { get; private set; }
        public Button prevButton { get; private set; }
        public Button nextButton { get; private set; }
        public Button lastButton { get; private set; }
        public StackPanel innerPanel { get; private set; }
        public WrapPanel outerPanel { get; private set; }
        public TextBlock fromBlock;
        private TextBlock toTextBlock;
        public TextBlock toBlock;
        private TextBlock fromTextBlock;
        public TextBlock totalBlock;
        private Action<string> firstFunction;
        private Action<string> prevFunction;
        private Action<string> nextFunction;
        private Action<string> lastFunction;

        /// <summary>
        /// Definiert die PagingBar
        /// </summary>
        /// <param name="firstFunction">Funktion für Anfang, nimmt genau 1 Parameter vom Typ string (Funktion die zur Ersten Seite führt)</param>
        /// <param name="prevFunction">Funktion für vorheriges, nimmt genau 1 Parameter vom Typ string (Funktion die zur vorherigen Seite führt)</param>
        /// <param name="nextFunction">Funktion für nächstes, nimmt genau 1 Parameter vom Typ string (Funktion die zur nächsten Seite führt)</param>
        /// <param name="lastFunction">Funktion für Ende, nimmt genau 1 Parameter vom Typ string (Funktion die zur Ersten letzten führt)</param>
        public PagingBar(Action<string> firstFunction, Action<string> prevFunction, Action<string> nextFunction, Action<string> lastFunction)
        {
            this.firstFunction = firstFunction;
            this.prevFunction = prevFunction;
            this.nextFunction = nextFunction;
            this.lastFunction = lastFunction;

            firstButton = new Button();
            prevButton = new Button();
            nextButton = new Button();
            lastButton = new Button();

            fromBlock = new TextBlock();
            toTextBlock = new TextBlock();
            toBlock = new TextBlock();
            fromTextBlock = new TextBlock();
            totalBlock = new TextBlock();

            outerPanel = new WrapPanel();
            outerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            outerPanel.VerticalAlignment = VerticalAlignment.Center;
            outerPanel.Margin = new Thickness(200, 0, 20, 0);

            innerPanel = new StackPanel();
            innerPanel.VerticalAlignment = VerticalAlignment.Center;
            innerPanel.Orientation = Orientation.Horizontal;
            innerPanel.Margin = new Thickness(8, 0, 5, 0);

            firstButton.Content = "<<";
            firstButton.FontSize = 14;
            firstButton.Height = 25;
            firstButton.Padding = new Thickness(5, 0, 5, 0);
            firstButton.Click += firstFunctionProcessor;

            prevButton.Content = "<";
            prevButton.FontSize = 14;
            prevButton.Height = 25;
            prevButton.Padding = new Thickness(5, 0, 5, 0);
            prevButton.Click += prevFunctionProcessor;

            nextButton.Content = ">";
            nextButton.FontSize = 14;
            nextButton.Height = 25;
            nextButton.Padding = new Thickness(5, 0, 5, 0);
            nextButton.Click += nextFunctionProcessor;

            lastButton.Content = ">>";
            lastButton.FontSize = 14;
            lastButton.Height = 25;
            lastButton.Padding = new Thickness(5, 0, 5, 0);
            lastButton.Click += lastFunctionProcessor;
            
            fromBlock.FontSize = 14;
            fromBlock.Height = 25;
            fromBlock.Margin = new Thickness(0, 0, 5, 0);

            fromTextBlock.Text = IniParser.GetSetting("APPSETTINGS", "pagingBarFrom");
            toTextBlock.FontSize = 14;
            toTextBlock.Height = 25;
            toTextBlock.Margin = new Thickness(0, 0, 5, 0);

            toBlock.FontSize = 14;
            toBlock.Height = 25;
            toBlock.Margin = new Thickness(0, 0, 5, 0);

            toTextBlock.Text = IniParser.GetSetting("APPSETTINGS", "pagingBarTo");
            fromTextBlock.FontSize = 14;
            fromTextBlock.Height = 25;
            fromTextBlock.Margin = new Thickness(0, 0, 5, 0);

            totalBlock.FontSize = 14;
            totalBlock.Height = 25;
            totalBlock.Margin = new Thickness(0, 0, 5, 0);

            innerPanel.Children.Add(fromBlock);
            innerPanel.Children.Add(toTextBlock);
            innerPanel.Children.Add(toBlock);
            innerPanel.Children.Add(fromTextBlock);
            innerPanel.Children.Add(totalBlock);

            outerPanel.Children.Add(firstButton);
            outerPanel.Children.Add(prevButton);

            outerPanel.Children.Add(innerPanel);

            outerPanel.Children.Add(nextButton);
            outerPanel.Children.Add(lastButton);
        }

        /// <summary>
        /// Führe die entsprechende Funktion bei Klick auf entsprechenden Button aus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void firstFunctionProcessor(object sender, RoutedEventArgs e)
        {
            this.firstFunction("");
        }

        /// <summary>
        /// Führe die entsprechende Funktion bei Klick auf entsprechenden Button aus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void prevFunctionProcessor(object sender, RoutedEventArgs e)
        {
            this.prevFunction("");
        }

        /// <summary>
        /// Führe die entsprechende Funktion bei Klick auf entsprechenden Button aus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextFunctionProcessor(object sender, RoutedEventArgs e)
        {
            this.nextFunction("");
        }

        /// <summary>
        /// Führe die entsprechende Funktion bei Klick auf entsprechenden Button aus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lastFunctionProcessor(object sender, RoutedEventArgs e)
        {
            this.lastFunction("");
        }

    }
}
