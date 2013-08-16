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

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Interaktionslogik für Restore.xaml
    /// </summary>
    public partial class Restore : KPage
    {
        public Restore()
        {
            InitializeComponent();
        }

        public override void defineToolbarContent()
        {
            this.parentToolbar.addButton(IniParser.GetSetting("RESTORE", "startRestoreBackup"), showBackup);
        }

        public void showBackup(Button btn)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            var newWindow = new KöTaf.WPFApplication.Views.USB_Identification(mainWindow, "restore");
            try
            {
                newWindow.Show();
            }
            catch { }
        }
    }
}
