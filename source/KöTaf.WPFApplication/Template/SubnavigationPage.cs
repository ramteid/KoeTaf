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

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Initialisiert eine Seite mit einem Untermenü
    /// </summary>        
    class SubnavigationPage
    {
        private MainWindow mainWindow;
        private List<SubnavigationButton> subNavigationButtons;
        public TabControl tabControl { get; private set; }

        public SubnavigationPage(string pageTitle)
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            subNavigationButtons = new List<SubnavigationButton>();
            mainWindow.contentPanel.Children.Clear();
            mainWindow.subNavigation.Children.Clear();
            mainWindow.lbPageTitle.Content = pageTitle;
        }

        /// <summary>
        /// Fügt einen neuen Untermenüpunkt mit einfacher Seite hinzu
        /// </summary>
        /// <param name="label">Titel der Seite</param>
        /// <param name="pageType">Typ der Seite. Sollte ein von KPage abgeleiteter Typ sein.</param>
        public void addSubnavigation(string label, Type pageType)
        {
            SubnavigationButton sBtn = new SubnavigationButton(pageType, label);
            mainWindow.subNavigation.Children.Add(sBtn.btn);
            // frame.Margin ist definiert in MainWindow.switchPage()
        }

        /// <summary>
        /// Fügt einen neuen Untermenüpunkt mit einem TabControl hinzu
        /// </summary>
        /// <param name="label">Titel der Seite</param>
        /// <param name="tabs">Liste von SimpleTabItems, die angezeigt werden sollen</param>
        public void addSubnavigation(string label, List<SimpleTabItem> tabs)
        {
            // Erzeuge TabControl
            tabControl = new TabControl();
            tabControl.FontSize = 14;
            tabControl.Background = Brushes.White;
            tabControl.Margin = new Thickness(-1, 15, -1, -1);

            foreach (var tab in tabs)
            {
                DockPanel pageWrapper = new DockPanel();

                ScrollableFrame scrollableFrame = new ScrollableFrame();
                ExtScrollViewer scrollViewer = scrollableFrame.createScrollableFrame(tab.frame);

                tab.toolbar.relatedTabControl = tabControl;

                pageWrapper.Margin = new Thickness(-4);
                pageWrapper.Children.Add(tab.toolbar.dpToolbarPanel);
                pageWrapper.Children.Add(scrollViewer);

                tab.frame.Margin = new Thickness(15, 15, 0, 0);
                tab.frame.Content = tab.page;
                tab.page.parentFrame = tab.frame;
                tab.page.parentToolbar = tab.toolbar;
                tab.page.parentTabControl = tabControl;
                tab.page.parentScrollViewer = scrollViewer;

                tab.page.defineToolbarContent();

                TabItem tabItem = new TabItem();
                tabItem.Header = tab.label;
                tabItem.Content = pageWrapper;
                tabControl.Items.Add(tabItem);
            }

            SubnavigationButton sBtn = new SubnavigationButton(label, tabControl);
            mainWindow.subNavigation.Children.Add(sBtn.btn);
        }

    }




}
