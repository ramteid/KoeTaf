/**
 * Class: pDisplayedData
 * @author Lucas Kögel
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using KöTaf.DataModel;
using KöTaf.DataModel.Enums;
using System.Collections;
using KöTaf.WPFApplication.Template;

namespace KöTaf.WPFApplication.Views.Lists
{
    /// <summary>
    /// Diese Page stellt die Auswahl "Anzuzeigende Daten" dar.
    /// Sie erlaubt dem Nutzer bis zu 5 verschiedene Attribute auszuwählen,
    /// die für die gefundenen Persone angezeigt werden sollen.
    /// </summary>
    public partial class pDisplayedData : KPage
    {

        private ArrayList displayedDataComboBoxes = new ArrayList();
        private List<String> displayableData;

        public pDisplayedData()
        {
            InitializeComponent();
            Init();
        }

        // This function must be defined in each class implementing KPage
        // If you don't want a toolbar, just leave it empty and don't add any button
        public override void defineToolbarContent()
        {
        }

        private void Init()
        {
            displayedDataComboBoxes.Add(cbDisplayedData1);
            displayedDataComboBoxes.Add(cbDisplayedData2);
            displayedDataComboBoxes.Add(cbDisplayedData3);
            displayedDataComboBoxes.Add(cbDisplayedData4);
            displayedDataComboBoxes.Add(cbDisplayedData5);

            cbDisplayedData2.Visibility = Visibility.Hidden;
            cbDisplayedData3.Visibility = Visibility.Hidden;
            cbDisplayedData4.Visibility = Visibility.Hidden;
            cbDisplayedData5.Visibility = Visibility.Hidden;
        }

        public ArrayList getDisplayedDataComboBoxes(){
            return displayedDataComboBoxes;
        }

        public void setDisplayableData(List<String> list)
        {
            this.displayableData = list;
        }

        /// <summary>
        /// Stellt den Start-Zustand für die Auswahl wieder her.
        /// </summary>
        public void resetAll()
        {
            foreach (ComboBox box in displayedDataComboBoxes)
            {
                box.IsEnabled = false;
                box.SelectedItem = null;
                box.Visibility = Visibility.Hidden;
            }
            cbDisplayedData1.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Macht der Reihe nach alle ComboBoxen sichtbar.
        /// </summary>
        /// <param name="sender"> Die gerade geschlossene ComboBox. </param>
        private void cbDisplayedData_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            // Je nachdem welche ComboBox gerade geschlossen wurde, die in der Reihe als naechstes
            // kommende sichtbar machen.
            switch (cb.Name)
            {
                case ("cbDisplayedData1"):
                    if (cbDisplayedData1.Text != "")
                    {
                        cbDisplayedData2.Visibility = Visibility.Visible;
                        cbDisplayedData2.IsEnabled = true;
                    }
                    break;
                case ("cbDisplayedData2"):
                    if (cbDisplayedData2.Text != "")
                    {
                        cbDisplayedData3.Visibility = Visibility.Visible;
                        cbDisplayedData3.IsEnabled = true;
                    }
                    break;
                case ("cbDisplayedData3"):
                    if (cbDisplayedData3.Text != "")
                    {
                        cbDisplayedData4.Visibility = Visibility.Visible;
                        cbDisplayedData4.IsEnabled = true;
                    }
                    break;
                case ("cbDisplayedData4"):
                    if (cbDisplayedData4.Text != "")
                    {
                        cbDisplayedData5.Visibility = Visibility.Visible;
                        cbDisplayedData5.IsEnabled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Füllt die ComboBox mit den auswählbaren Attributen.
        /// </summary>
        /// <param name="sender"> Die gerade geöffnete ComboBox. </param>
        private void cbDisplayedData_DropDownOpened(object sender, EventArgs e)
        {
            //ComboBox leeren, bevor sie neu befuellt wird.
            ((ComboBox)sender).Items.Clear();

            //Alle bereits ausgewaehlten Items - welche in den anderen ComboBoxen ausgewaehlt sind -
            //aus der Liste für diese ComboBox (Sender) entfernen.
            List<String> myDisplayableData = new List<String>(displayableData);
            foreach (ComboBox box in displayedDataComboBoxes)
            {
                if (!((ComboBox)sender).Equals(box))
                {
                    if (box.Text == " ") { continue; };

                    myDisplayableData.Remove(box.Text);
                }                
            }
   
            //ComboBox (Sender) neu befüllen.
            foreach (string item in myDisplayableData)
            {
                ((ComboBox)sender).Items.Add(item);
            }
        }

    }
}