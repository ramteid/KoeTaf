using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Erweitert ScrollViewer um die Funktion, die MouseWheel-Events weiterzuleiten, sodass bei DataGrids gescrollt werden kann
    /// </summary>
    public class ExtScrollViewer : ScrollViewer
    {
        private ScrollBar verticalScrollbar;

        /// <summary>
        /// Überschreibt Template-Definition
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Call base class
            base.OnApplyTemplate();

            // Obtain the vertical scrollbar
            this.verticalScrollbar = this.GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
        }

        /// <summary>
        /// Überschreibt OnMouseWheel-Event und leitet dies weiter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Only handle this message if the vertical scrollbar is in use
            if ((this.verticalScrollbar != null) && (this.verticalScrollbar.Visibility == System.Windows.Visibility.Visible) && this.verticalScrollbar.IsEnabled)
            {
                // Perform default handling
                base.OnMouseWheel(e);
            }
        }
    }
}
