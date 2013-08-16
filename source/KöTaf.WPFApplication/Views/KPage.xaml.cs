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
using KöTaf.WPFApplication.Template;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Interaktionslogik für KPage.xaml
    /// </summary>
    public abstract partial class KPage : Page
    {
        public TabControl parentTabControl { get; set; }
        public Frame parentFrame { get; set; }
        public Toolbar parentToolbar { get; set; }
        public ExtScrollViewer parentScrollViewer { get; set; }
        public bool isToolbarDefined { get; set; }

        public KPage()
        {
            this.isToolbarDefined = false;
        }

        public abstract void defineToolbarContent();
        
        /// <summary>
        /// Hole eine KPage Instanz aus den Tabs eines TabControls
        /// </summary>
        /// <typeparam name="T">Normalerweise ein KPage-Typ</typeparam>
        /// <returns>Die gesuchte KPage-Instanz</returns>
        public T getPageFromTabs<T>()
        {
            try
            {
                if (parentTabControl == null)
                    //return default(T);  // null
                    throw new Exception("You tried to get pages from a TabControl that was not initialized. Check if the parentTabControl of the calling KPanel class really is being initialized");

                foreach (TabItem t in this.parentTabControl.Items)              // iteriere über alle tabs
                {
                    DockPanel pageWrapper = t.Content as DockPanel;             // ein tab beinhaltet ein pageWrapper DockPanel welches die Toolbar und den ExtScrollViewer / Frame beinhaltet
                    foreach (var x in pageWrapper.Children)                     // iteriere über den pageWrapper
                    {
                        if (x.GetType() == typeof(ExtScrollViewer))             // wir suchen den ExtScrollViewer
                        {
                            ExtScrollViewer s = x as ExtScrollViewer;
                            if (s.Content.GetType() == typeof(Frame))           // normalerweise ist der content von ExtScrollViewer ein Frame
                            {
                                Frame f = s.Content as Frame;                   // hole den Frame
                                if (f.Content.GetType() == typeof(T))           // versichern, dass im frame wirklich eine Instanz des gewünschten Typs ist 
                                {
                                    T page = (T)(f.Content);                    // hole die gesuchte Instanz
                                    return page;
                                }
                            }
                        }
                    }
                }
                return default(T);  // null
            }
            catch
            {
                return default(T);  // null
            }
        }
    }
}