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
    /// Dient zur Erzeugung eines ExtScrollViewers
    /// </summary>        
    class ScrollableFrame
    {
        /// <summary>
        /// Erzeugt einen ExtScrollViewer, der ein Scrollen der aktuellen Seite ermöglicht
        /// </summary>
        /// <param name="frame">der zu beinhaltende Frame</param>
        /// <returns>ExtScrollViewer-Instanz</returns>
        public ExtScrollViewer createScrollableFrame(Frame frame)
        {
            ExtScrollViewer scrollViewer = new ExtScrollViewer();

            frame.FontSize = 14;
            frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            ScrollViewer.SetCanContentScroll(frame, true);
            ScrollViewer.SetVerticalScrollBarVisibility(frame, ScrollBarVisibility.Visible);
            ScrollViewer.SetHorizontalScrollBarVisibility(scrollViewer, ScrollBarVisibility.Visible);

            scrollViewer.Content = frame;

            return scrollViewer;
        }
    }
}
