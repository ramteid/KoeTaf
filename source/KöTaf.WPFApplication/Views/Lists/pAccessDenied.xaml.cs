/**
 * Class: pAccessDenied
 * @author Lucas Kögel
 */

namespace KöTaf.WPFApplication.Views.Lists
{
    /// <summary>
    /// Diese Seite wird sichtbar wenn der Nutzer auf die Listen-Erstellung zugreiffen moechte,
    /// aber nicht als ADMIN eingelogt ist.
    /// </summary>
    public partial class pAccessDenied : KPage
    {
        public pAccessDenied()
        {
            InitializeComponent();
        }

        // This function must be defined in each class implementing KPage
        // If you don't want a toolbar, just leave it empty and don't add any button
        public override void defineToolbarContent()
        {
        }
    }
}