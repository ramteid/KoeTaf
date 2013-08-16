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
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Views
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Simple Willkommensseite
    /// </summary>
    public partial class pWelcomeScreen : KPage
    {
        public pWelcomeScreen()
        {
            InitializeComponent();
            welcomeMsg.Content = IniParser.GetSetting("APPSETTINGS", "welcomeMessage");
        }

        public override void defineToolbarContent()
        { 
        }
    }
}
