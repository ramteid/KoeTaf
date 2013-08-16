using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel;
using KöTaf.Utils.Parser;

namespace KöTaf.WPFApplication.Models
{
    /// <summary>
    /// Author: Dietmar Sach
    /// Datenmodell für Personen in der Schnellbuchung
    /// </summary>
    class QuickBookingDataGridModel
    {
        public Person person { get; private set; }
        public int tableNo { get; private set; }
        public string lastName { get; private set; }
        public string firstName { get; private set; }
        public string validityEnd { get; private set; }
        public string dateOfBirth { get; private set; }
        public string lastPurchase { get; private set; }
        public string amount { get; set; }
        public double parsedAmount { get; set; }
        public int group { get; set; }

        /// <summary>
        /// Füllt Attribute
        /// </summary>
        /// <param name="person">Person</param>
        public QuickBookingDataGridModel(Person person)
        {
            if (!(person.TableNo.HasValue))
                return;

            this.person = person;
            this.tableNo = (int)person.TableNo;
            this.lastName = person.LastName;
            this.firstName = person.FirstName;
            this.validityEnd = SafeStringParser.safeParseToStr(person.ValidityEnd);
            this.dateOfBirth = SafeStringParser.safeParseToStr(person.DateOfBirth);
            this.lastPurchase = SafeStringParser.safeParseToStr(person.LastPurchase);
            this.group = person.Group;
        }

        /// <summary>
        /// Erzeugt eigenen String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return tableNo + ", " + lastName + ", " + firstName + ", " + validityEnd + ", " + dateOfBirth + ", " + amount + ", " + lastPurchase;
        }
    }
}
