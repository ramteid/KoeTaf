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
    class SinglePage
    {
        /// <summary>
        /// Author: Dietmar Sach
        /// Einfache Seite ohne Untermenü
        /// </summary>        
        public TabControl tabControl { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Seitenansicht mit einfacher Seite
        /// </summary>
        /// <param name="label">Titel der Seite</param>
        /// <param name="page">Instanz der anzuzeigenden Seite</param>
        public SinglePage(string label, KPage page)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            Frame frame = new Frame();
            Toolbar toolbar = new Toolbar(frame, page);
            ScrollableFrame scrollableFrame = new ScrollableFrame();
            ExtScrollViewer scrollViewer = scrollableFrame.createScrollableFrame(frame);

            DockPanel.SetDock(toolbar.dpToolbarPanel, Dock.Top);

            mainWindow.subNavigation.Children.Clear();

            frame.Margin = new Thickness(15, 0, 0, 0);
            frame.Content = page;
            page.parentFrame = frame;
            page.parentToolbar = toolbar;
            page.parentScrollViewer = scrollViewer;

            page.defineToolbarContent();

            mainWindow.contentPanel.Children.Clear();
            mainWindow.contentPanel.Children.Add(toolbar.dpToolbarPanel);
            mainWindow.contentPanel.Children.Add(scrollViewer);
            mainWindow.lbPageTitle.Content = label;
        }

        /// <summary>
        /// Erzeugt eine neue Seitenansicht mit TabControl
        /// </summary>
        /// <param name="source">Die aufrufende Seite vom Typ KPage</param>
        /// <param name="label">Titel der Seite</param>
        /// <param name="tabs">Liste von anzuzeigenden SimpleTabItem</param>
        public SinglePage(KPage source, string label, List<SimpleTabItem> tabs)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            // Erzeuge TabControl
            tabControl = new TabControl();
            tabControl.FontSize = 14;
            tabControl.Background = Brushes.White;
            tabControl.Margin = new Thickness(-1, 15, -1, -1);

            source.parentTabControl = this.tabControl;

            mainWindow.subNavigation.Children.Clear();

            // macht aus der tabs-Liste einen TabControl
            foreach (var tab in tabs)
            {
                DockPanel pageWrapper = new DockPanel();

                ScrollableFrame scrollableFrame = new ScrollableFrame();
                ExtScrollViewer scrollViewer = scrollableFrame.createScrollableFrame(tab.frame);

                tab.toolbar.relatedTabControl = tabControl;

                pageWrapper.Margin = new Thickness(-4);
                pageWrapper.Children.Add(tab.toolbar.dpToolbarPanel);
                pageWrapper.Children.Add(scrollViewer);

                tab.frame.Margin = new Thickness(15, 0, 0, 0);
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

            mainWindow.contentPanel.Children.Clear();
            mainWindow.contentPanel.Children.Add(tabControl);
            mainWindow.lbPageTitle.Content = label;
        }
        public void createFormletter_Click(Button btn) { }
    }
}
