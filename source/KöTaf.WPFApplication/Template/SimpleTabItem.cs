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
    /// Dient zur Definition eines TabItems mit allen beinhalteten Elementen
    /// </summary>        
    class SimpleTabItem
    {
        public string label { get; private set; }
        public KPage page { get; private set; }
        public Frame frame { get; private set; }
        public Toolbar toolbar { get; private set; }

        /// <summary>
        /// Legt Attribute fst
        /// </summary>
        /// <param name="label">Titel der Seite</param>
        /// <param name="page">Anzuzeigende KPage-Instanz</param>
        public SimpleTabItem(string label, KPage page)
        {
            this.frame = new Frame();
            this.label = label;
            this.page = page;
            toolbar = new Toolbar(frame, page);
            DockPanel.SetDock(toolbar.dpToolbarPanel, Dock.Top);
        }
    }
}
