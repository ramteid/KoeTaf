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

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Such-Panel für die Toolbar
    /// </summary>        
    public class SearchPanel
    {
        public ComboBox searchComboBox { get; private set; }
        public TextBox searchBox { get; private set; }
        //public Button searchButton { get; private set; }
        public WrapPanel panel { get; private set; }
        private Action<string> keyUpFunction;
        private Action<string> searchFunction;

        /// <summary>
        /// Definiert das SearchPanel
        /// </summary>
        /// <param name="searchFunction">Referenz zur Funktion, der der Suchstring übergeben wird. Muss genau 1 Parameter vom Typ String annehmen</param>
        public SearchPanel(Action<string> searchFunction)
        {
            searchComboBox = new ComboBox();
            searchBox = new TextBox();
            //searchButton = new Button();

            panel = new WrapPanel();
            panel.HorizontalAlignment = HorizontalAlignment.Right;
            panel.VerticalAlignment = VerticalAlignment.Top;
            panel.Margin = new Thickness(0, 10, 20, 0);

            searchComboBox.MinWidth = 150;
            searchComboBox.Visibility = Visibility.Hidden;
            searchComboBox.Margin = new Thickness(0, 5, 5, 5);
            
            searchBox.Text = "Suche ...";
            searchBox.FontSize = 14;
            searchBox.MinWidth = 125;
            searchBox.Height = 25;
            searchBox.Margin = new Thickness(0, 0, 5, 0);
            searchBox.GotFocus += emptyTextbox;

            //searchButton.Content = "Los";
            //searchButton.FontSize = 14;
            //searchButton.Height = 25;
            //searchButton.Padding = new Thickness(5, 0, 5, 0);
            //searchButton.Click += searchStringProcessor;

            panel.Children.Add(searchComboBox);
            panel.Children.Add(searchBox);
            //panel.Children.Add(searchButton);

            this.searchFunction = searchFunction;
            this.searchBox.KeyUp += searchStringProcessor;
        }

        /// <summary>
        /// Fügt eine ComboBox mit Auswahlwerten hinzu
        /// </summary>
        /// <typeparam name="T">Typisierung der Itemssource-Liste</typeparam>
        /// <param name="itemList">Itemssource-Liste</param>
        public void addComboBox<T>(List<T> itemList)
        {
            searchComboBox.ItemsSource = itemList;
            searchComboBox.Visibility = Visibility.Visible;
            searchComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Hole das ausgewählte Element aus der Liste
        /// </summary>
        /// <typeparam name="T">Typisierung des Elements</typeparam>
        /// <returns></returns>
        public T getComboBoxSelectedItem<T>()
        {
            try
            {
                if (searchComboBox.SelectedItem.GetType() == typeof(T))
                    return (T)(searchComboBox.SelectedItem);
            }
            catch { }

            return default(T);
        }

        /// <summary>
        /// KeyUp-Funktion für das KeyUp-Event definieren
        /// </summary>
        /// <param name="keyUpFunction">Referenz zur Suchfunktion, die genau 1 Parameter vom Typ string annimmt</param>
        public void addActionKeyUpTextbox(Action<string> keyUpFunction)
        {
            this.keyUpFunction = keyUpFunction;
        }
        
        /// <summary>
        /// Wird vom KeyUp-Event aufgerufen und ruft KeyUp-Funktion auf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchStringProcessor(object sender, RoutedEventArgs e)
        {
            if (this.searchFunction == null)
                return;

            string searchString = searchBox.Text;
            this.searchFunction(searchString);
        }

        /// <summary>
        /// Textbox leeren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void emptyTextbox(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }
    }
}
