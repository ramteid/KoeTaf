using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Helper
{

    // Gibt eine neue Instanz einer Klasse durch Übergabe des Klassentypes zurück
    // Beispiel für die Nutzung: pFormletterAdministration i = new Tester<pFormletterAdministration>().returnNew();
    class GenericClassInstantiator<T> where T : new()
    {
        public T getNewInstance()
        {
            return new T();
        }
    }
}
