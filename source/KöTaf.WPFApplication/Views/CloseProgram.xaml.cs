/**
 * Class: CloseProgram
 *
 * @author Michael Müller
 * @version 1.0
 * @since 2013-04-05
 * 
 * Last modification: 2013-05-18 / Michael Müller
 */
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
using System.Windows.Shapes;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik für CloseProgram.xaml
    /// Klasse ist ausschließlich für für das Schliessen
    /// des Kötaf Programms zuständig.
    /// </summary>
    public partial class CloseProgram : Window
    {
        public CloseProgram()
        {
            InitializeComponent();
            makeBackupFromDatabase();
            this.Close();
        }

        private void makeBackupFromDatabase() {
            var newWindow = new USB_Identification();
            App.Current.Windows[0].Close();
            try
            {
                newWindow.ShowDialog();
            }
            catch { 
                //if Backup-Process was canceled
            }
        }
    }
}
