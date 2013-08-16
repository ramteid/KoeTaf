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
using KöTaf.WPFApplication.Helper;

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Repräsentiert einen Subnavigations-Button
    /// </summary>        
    class SubnavigationButton
    {
        public Button btn { get; set; }
        public string label { get; set; }
        public TabControl tabControl { get; set; }
        public DockPanel pageWrapper { get; private set; }
        public Type pageType;
        private bool isTabControl;

        /// <summary>
        /// Konstruktor für eine Seite mit TabControl
        /// </summary>
        /// <param name="label">Titel der Seite</param>
        /// <param name="tabControl">anzuzeigender TabControl</param>
        public SubnavigationButton(string label, TabControl tabControl)
        {
            this.isTabControl = true;
            this.tabControl = tabControl;
            this.label = label;
            defineButton();
        }

        /// <summary>
        /// Konstruktor für eine einfache Seite
        /// </summary>
        /// <param name="pageType">Typ der anzuzeigenden Seite</param>
        /// <param name="label">Titel der Seite</param>
        public SubnavigationButton(Type pageType, string label)
        {
            this.isTabControl = false;
            this.pageType = pageType;
            this.label = label;
            defineButton();
        }

        /// <summary>
        /// Button-Eigenschaften festlegen
        /// </summary>
        private void defineButton()
        {
            this.btn = new Button();
            this.btn.Content = label;
            this.btn.Margin = new Thickness(10, 15, 10, 0);
            this.btn.Padding = new Thickness(6, 0, 6, 0);
            this.btn.VerticalAlignment = VerticalAlignment.Center;
            this.btn.Click += this.switchPage;
        }

        /// <summary>
        /// Wird bei Klick auf Button ausgelöst
        /// Wechselt die Seite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void switchPage(object sender, RoutedEventArgs args)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            if (this.isTabControl)
                mainWindow.switchPage(this.label, this.tabControl);
            else
                mainWindow.switchPage(this.label, this.pageType);
        }

    }
}
