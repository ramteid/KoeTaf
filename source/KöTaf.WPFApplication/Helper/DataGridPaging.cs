using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Helper
{
    /// <summary>
    /// Klasse um das Paging zu realisieren
    /// Autor: Georg Schmid
    /// </summary>
    /// <typeparam name="T">Typ der übergebenen Elemente</typeparam>
    class DataGridPaging<T>
    {
        private int _TotalDataGridItems;
        private int _StartOfDataGridItems;
        private int _EndOfDataGridItems;
        private int _StepSize = 15;


        private IEnumerable<T> _ListOfItems;

        #region Constructor

        /// <summary>
        /// Konstruktor des Pagings
        /// </summary>
        /// <param name="listOfItems">Typ der übergebenen Elemente</param>
        public DataGridPaging(IEnumerable<T> listOfItems)
        {
            //Seitengröße aus der config.ini auslesen
            try
            {
                _StepSize = Convert.ToInt32(IniParser.GetSetting("PAGING", "STEPSIZE"));
            }
            catch(Exception)
            {
                KöTaf.WPFApplication.Helper.MessageBoxEnhanced.Error(IniParser.GetSetting("ERRORMSG", "configFileError"));
            }

            this._ListOfItems = listOfItems;

            this._TotalDataGridItems = this._ListOfItems.Count();
            this._StartOfDataGridItems = 0;
            this._EndOfDataGridItems = _StartOfDataGridItems + _StepSize;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gibt alle Elemente des Pagings zurück
        /// </summary>
        /// <returns>Liste von Elementen</returns>
        public IEnumerable<T> getItems()
        {
            return _ListOfItems;
        }

        /// <summary>
        /// Gibt die nächste Seite zurück
        /// </summary>
        /// <returns>Nächste Seite</returns>
        public IEnumerable<T> NextSide()
        {
            return ShrinkList(_StartOfDataGridItems+_StepSize);
        }

        /// <summary>
        /// Gibt die vorherige Seite zurück
        /// </summary>
        /// <returns>Vorherige Seite</returns>
        public IEnumerable<T> PrevSide()
        {
            return ShrinkList(_StartOfDataGridItems-_StepSize);
        }

        /// <summary>
        /// Gibt die erste Seite zurück
        /// </summary>
        /// <returns>Erste Seite</returns>
        public IEnumerable<T> FirstSide()
        {
            return ShrinkList(0);
        }
        
        /// <summary>
        /// Gibt die letzte Seite zurück
        /// </summary>
        /// <returns>Letzte Seite</returns>
        public IEnumerable<T> LastSide()
        {
            int lastSideStart = ((int)(_TotalDataGridItems / _StepSize)) * _StepSize;
            _StartOfDataGridItems = lastSideStart;
            _EndOfDataGridItems = _TotalDataGridItems;
            return _ListOfItems.ToList().GetRange(((int)(_TotalDataGridItems / _StepSize))*_StepSize,_TotalDataGridItems-lastSideStart );
        }

        /// <summary>
        /// Gibt die aktuelle Seite zurück
        /// </summary>
        /// <returns>Erste Seite</returns>
        public IEnumerable<T> ActualSide()
        {
            return ShrinkList(_StartOfDataGridItems);
        }

        /// <summary>
        /// Gibt den Startwert der Seite zurück
        /// </summary>
        /// <returns>Startwert der Seite</returns>
        public int GetStart()
        {
            //Korrigiert den Startwert (wegen Index)
            if (_TotalDataGridItems == 0)
            {
                return 0;
            }
            else
            {
                return (_StartOfDataGridItems + 1);
            }
        }

        /// <summary>
        /// Gibt den Endwert der Seite zurück
        /// </summary>
        /// <returns>Endwert der Seite</returns>
        public int GetEnd()
        {
            return _EndOfDataGridItems;
        }

        /// <summary>
        /// Gibt Anzahl aller Elemente des Paging zurück
        /// </summary>
        /// <returns>Anzahl der Elemente</returns>
        public int GetTotal()
        {
            return _TotalDataGridItems;
        }

        /// <summary>
        /// Verkleinert die Liste um Sie zurückzugeben
        /// </summary>
        /// <param name="start">Startwert der aktuellen Seite</param>
        /// <returns>Liste der aktuellen Seite</returns>
        private IEnumerable<T> ShrinkList(int start)
        {
            _StartOfDataGridItems = start;
            _EndOfDataGridItems = start + _StepSize;
            if (_StartOfDataGridItems < 0)
            {
                return FirstSide();
            }
            if (_EndOfDataGridItems > _TotalDataGridItems)
            {
                return LastSide();
            }
            return _ListOfItems.ToList().GetRange(_StartOfDataGridItems, _StepSize);
        }

        /// <summary>
        /// Setzt den Startwert des Paging
        /// Für die Rückkehr auf eine bestimmte Seite benötigt
        /// </summary>
        /// <param name="start">Startwert des Pagings</param>
        public void setStartOfDataGridItems(int start)
        {
            _StartOfDataGridItems = start;
        }

        /// <summary>
        /// Gibt den aktuellen Startwert der Seite zurück
        /// </summary>
        /// <returns>Startwert der aktuellen Seite</returns>
        public int getStartOfDataGridItems()
        {
            return _StartOfDataGridItems;
        }

        #endregion
    }
}
