using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KöTaf.DataModel.Enums;
using KöTaf.DataModel.Utils;

namespace KöTaf.DataModel
{
    /// <summary>
    /// (c) Florian Wasielewski
    /// 
    /// Partielle Klasse: Besitzt Methoden zum holen, hinzufügen und manipulieren von Einträgen. 
    /// </summary>
    public partial class Sponsor
    {
        #region Static Methods

        /// <summary>
        /// Gibt eine Liste aller Sponsoren zurück
        /// </summary>
        /// <param name="sponsorID">Filterung nach Sponsoren ID</param>
        /// <returns>Liste aller Sponsoren</returns>
        public static IEnumerable<Sponsor> GetSponsors(int? sponsorID = null)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var sponsors = db.Sponsors
                    .Include("FundingType")
                    .Include("Title")
                    .AsQueryable();

                if (sponsorID.HasValue)
                    sponsors = sponsors.Where(s => s.SponsorID == sponsorID.Value);

                return sponsors
                    .OrderBy(t => t.LastName)
                    .ThenBy(t => t.FirstName)
                    .ToList();
            }
        }

        /// <summary>
        /// Fügt einen neuen Sponsor der Datenbank hinzu
        /// </summary>
        /// <param name="fundingTypeId">Die ID der Finanzierungsart</param>
        /// <param name="titleId">Die ID des Titels</param>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachname</param>
        /// <param name="city">Stadt</param>
        /// <param name="street">Strasse</param>
        /// <param name="zipCode">Postleitzahl</param>
        /// <param name="isFormLetterAllowed">Serienbrief erlaubt</param>
        /// <param name="contactPerson">Kontaktperson</param>
        /// <param name="email">Emailadresse</param>
        /// <param name="faxNo">Faxnummer</param>
        /// <param name="mobileNo">Handynumer</param>
        /// <param name="phoneNo">Telefonnummer</param>
        /// <returns>Die ID des Eintrags in der Datenbank</returns>
        public static int Add(int fundingTypeId, int titleId, string firstname, string lastname, string city, string street, int zipCode,
            bool isFormLetterAllowed, string companyName = null, string contactPerson = null, string email = null, string faxNo = null,
            string mobileNo = null, string phoneNo = null,bool isCompany=false)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                Sponsor sponsor = new Sponsor()
                {
                    CompanyName = companyName,
                    ContactPerson = contactPerson,
                    Email = email,
                    FaxNo = faxNo,
                    FirstName = firstname,
                    LastName = lastname,
                    MobileNo = mobileNo,
                    PhoneNo = phoneNo,
                    Street = street,
                    ZipCode = zipCode,
                    _IsFormLetterAllowed = isFormLetterAllowed,
                    City = city,
                    IsActive = true,
                    IsCompany=isCompany,
                    Title = db.Titles.Single(t => t.TitleID == titleId),
                    FundingType = db.FundingTypes.Single(f => f.FundingTypeID == fundingTypeId),
                };

                db.Sponsors.AddObject(sponsor);
                db.SaveChanges();

                return sponsor.SponsorID;
            }
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Sponsor in der Datenbank
        /// </summary>
        /// <param name="sponsorId">Die benötigte ID des Sponsors in der Datenbank</param>
        /// <param name="fundingTypeId">Die ID der Finanzierungsart</param>
        /// <param name="titleId">Die ID des Titels</param>
        /// <param name="firstname">Vorname</param>
        /// <param name="lastname">Nachname</param>
        /// <param name="city">Stadt</param>
        /// <param name="street">Strasse</param>
        /// <param name="zipCode">Postleitzahl</param>
        /// <param name="isFormLetterAllowed">Serienbrief erlaubt</param>
        /// <param name="contactPerson">Kontaktperson</param>
        /// <param name="email">Emailadresse</param>
        /// <param name="faxNo">Faxnummer</param>
        /// <param name="mobileNo">Handynumer</param>
        /// <param name="phoneNo">Telefonnummer</param>
        public static void Update(int sponsorId, int titleId, int fundingTypeId, string street, string city, int zipCode, string firstname, 
            string lastname, string companyName, string mobileNo, string phoneNo, string faxNo, string email, string contactPerson, bool isCompany)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var sponsor = db.Sponsors.Single(s => s.SponsorID == sponsorId);
                sponsor.Street = street;
                sponsor.Title = db.Titles.Single(t => t.TitleID == titleId);
                sponsor.FundingType = db.FundingTypes.Single(f => f.FundingTypeID == fundingTypeId);
                sponsor.City = city;
                sponsor.CompanyName = companyName;
                sponsor.ZipCode = zipCode;
                sponsor.FirstName = firstname;
                sponsor.LastName = lastname;
                sponsor.MobileNo = mobileNo;
                sponsor.PhoneNo = phoneNo;
                sponsor.FaxNo = faxNo;
                sponsor.Email = email;
                sponsor.ContactPerson = contactPerson;
                sponsor.LastModified = DateTime.Now;
                sponsor.IsCompany = isCompany;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Schaltet zwischen verschiedenen Aktivierungsstati einens Sponsors um
        /// </summary>
        /// <param name="sponsorId"></param>
        /// <param name="state"></param>
        private static void ToogleActivateState(int sponsorId, bool state)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var sponsor = db.Sponsors.Single(s => s.SponsorID == sponsorId);
                sponsor.IsActive = state;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Aktiviert einen bestehenden Sponsor in der Datenbank
        /// </summary>
        /// <param name="sponsorID">Die benötigte Sponsoren ID</param>
        public static void Activate(int sponsorID)
        {
            ToogleActivateState(sponsorId: sponsorID, state: true);
        }

        /// <summary>
        ///  Deaktiviert einen bestehenden Sponsor in der Datenbank
        /// </summary>
        /// <param name="sponsorID">Die benötigte Sponsoren ID</param>
        public static void Deactivate(int sponsorID)
        {
            ToogleActivateState(sponsorId: sponsorID, state: false);
        }

        /// <summary>
        /// Zählt Sponsoren in der Datenbank
        /// </summary>
        /// <param name="type">Filterung nach Typ</param>
        /// <returns>Anzahl der gezählten Sponsoren</returns>
        public static int Count(CountStateType type = CountStateType.All)
        {
            using (TafelModelContainer db = new TafelModelContainer())
            {
                var sponsors = db.Sponsors.AsQueryable();

                switch (type)
                {
                    case CountStateType.Activated:
                        sponsors = sponsors.Where(s => s.IsActive);
                        break;

                    case CountStateType.Deactivated:
                        sponsors = sponsors.Where(s => !s.IsActive);
                        break;
                }

                return sponsors.Count();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gibt die vollständige Adresse des Sponsors zurück
        /// </summary>
        public string ResidentialAddress
        {
            get
            {
                return FormatUtils
                    .GetResidentialAddress(ZipCode, City, Street);
            }
        }

        /// <summary>
        /// Gibt den vollständigen Namen des Sponsors zurück
        /// </summary>
        public string FullName
        {
            get
            {
                return FormatUtils.GetFullName(FirstName, LastName);
            }
        }

        #endregion
    }
}
