using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KöTaf.WPFApplication.Views.Accounting
{
    /// <summary>
    /// Author: Patrick Vogt
    /// </summary>
    class TestDaten
    {
        public String name { get; set; }
        public int nummer { get; set; }
        public String grafik { get; set; }

        public List<TestDaten> LoadAccountCollectionData()
        {
            List<TestDaten> accounts = new List<TestDaten>();
            accounts.Add(new TestDaten()
            {
                name = "Kasse",
                nummer = 2880,
                grafik = "/Images/coins_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Bürobedarf",
                nummer = 1000,
                grafik = "/Images/office_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Benzin",
                nummer = 1100,
                grafik = "/Images/gas_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Sponsoren",
                nummer = 3000,
                grafik = "/Images/sponsor_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Kunden",
                nummer = 2400,
                grafik = "/Images/customer_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Barentnahme",
                nummer = 1200,
                grafik = "/Images/coins_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Fuhrpark",
                nummer = 1300,
                grafik = "/Images/car_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Rückstellungen",
                nummer = 2900,
                grafik = "/Images/empty_icon.png"
            });

            accounts.Add(new TestDaten()
            {
                name = "Post- und Kommunikationsgebühren",
                nummer = 6800,
                grafik = "/Images/office_icon.png"
            });

            return accounts;
        }
    }
}
