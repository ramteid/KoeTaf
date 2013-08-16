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
using KöTaf.WPFApplication.Views.Formletter;

namespace KöTaf.WPFApplication.Template
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Diese Klasse bietet ein paar verschiedene Testlayouts
    /// Einfach in der MainWindow.xaml.cs diese Klasse instanziieren
    /// </summary>
    class Examples : KPage
    {
        public Examples()
        {
            int x = 1;

            // Untermenü, nur Frame
            if (x == 1)
            {
                SubnavigationPage mySubnavigation = new SubnavigationPage("Seitentitel 123");

                Type pageType1 = typeof(KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration);
                mySubnavigation.addSubnavigation("subpage1", pageType1);

                Type pageType2 = typeof(KöTaf.WPFApplication.Views.pWelcomeScreen);
                mySubnavigation.addSubnavigation("subpage2", pageType2);
            }


            // Untermenü mit Tabs und jeweils einem Frame
            // Jeder Frame hat eine Toolbar mit Buttons und SuchBox
            if (x == 2)
            {
                SubnavigationPage mySubnavigation = new SubnavigationPage("Seitentitel 234");
                List<SimpleTabItem> myTabList = new List<SimpleTabItem>();

                KPage p1 = new KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration();
                KPage p2 = new KöTaf.WPFApplication.Views.pWelcomeScreen();

                SimpleTabItem simpleTabItem1 = new SimpleTabItem("tab1", p1);
                SimpleTabItem simpleTabItem2 = new SimpleTabItem("tab2", p2);

                myTabList.Add(simpleTabItem1);
                myTabList.Add(simpleTabItem2);
                
                simpleTabItem1.toolbar.addButton("Button1", myButtonFunction);
                simpleTabItem1.toolbar.addButton("Button2", myButtonFunction);
                simpleTabItem1.toolbar.addSearchPanel(mySearchFunction);

                simpleTabItem2.toolbar.addButton("Button3", myButtonFunction);
                simpleTabItem2.toolbar.addButton("Button4", myButtonFunction);
                simpleTabItem2.toolbar.addSearchPanel(mySearchFunction);

                mySubnavigation.addSubnavigation("sometabs", myTabList);
            }


            // Einfache Seite mit einem Frame
            if (x == 3)
            {
                KPage pageFormletterAdmin = new KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration();
                SinglePage singlePage = new SinglePage("Formletter Admin", pageFormletterAdmin);
            }


            // Einfache Seite mit Tabs mit jeweils einem Frame
            if (x == 4)
            {
                List<SimpleTabItem> myTabList = new List<SimpleTabItem>();

                KPage p1 = new KöTaf.WPFApplication.Views.Formletter.pFormletterAdministration();
                myTabList.Add(new SimpleTabItem("tab1", p1));

                KPage p2 = new KöTaf.WPFApplication.Views.pWelcomeScreen();
                myTabList.Add(new SimpleTabItem("tab2", p2));

                SinglePage singlePage = new SinglePage(this, "Formletter Admin", myTabList);
            }

        }

        /// <summary>
        /// Toolbar definieren
        /// </summary>
        public override void defineToolbarContent()
        {
        }

        /// <summary>
        /// Testdaten auf die Konsole ausgeben
        /// </summary>
        /// <param name="str">Testdaten</param>
        private void mySearchFunction(string str)
        {
            Console.WriteLine(str);
        }

        /// <summary>
        /// Testdaten auf die Konsole ausgeben
        /// </summary>
        /// <param name="btn">aufrufender Button</param>
        private void myButtonFunction(Button btn)
        {
            Console.WriteLine(btn.Content);
        }
    }
}
