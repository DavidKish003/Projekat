using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data.SqlClient; 

namespace DBUtils
{
    public class KonekcijaKlasa
    {
         /* CRC: 
          * Responsibility - ODGOVORNOST: Konekcija na celinu baze podataka, SQL server tipa  
          Collaboration - zavisi od standardne klase SQlConnection iz biblioteke System.Data.SqlClient*/

        #region Atributi
        private SqlConnection _konekcija;
        //
        private string _putanjaBaze;
        private string _nazivBaze;
        private string _nazivDBMSinstance;
        private string _stringKonekcije;
        #endregion

        #region Konstruktor
        public KonekcijaKlasa(string nazivDBMSInstance, string putanjaBaze, string nazivBaze)
        {
            _putanjaBaze = putanjaBaze;
            _nazivBaze = nazivBaze;
            _nazivDBMSinstance = nazivDBMSInstance;
            _stringKonekcije = "";
        }

        // druga varijanta - preklapajuca (overload) metoda - sa razlicitim parametrima
        public KonekcijaKlasa(string noviStringKonekcije)
        {
            _putanjaBaze = "";
            _nazivBaze = "";
            _nazivDBMSinstance = "";
            _stringKonekcije = noviStringKonekcije;
        }
        #endregion

        #region Privatne metode
        private string DajStringKonekcije()
        {
            string pomStringKonekcije;
            if (_stringKonekcije.Length.Equals(0) || _stringKonekcije == null)
            {
                // AKO NEMAMO GOTOV STRING KONEKCIJE KOJI JE DAT PUTEM KONSTRUKTORA
                if (_putanjaBaze.Length.Equals(0) || _putanjaBaze == null)
                {
                    pomStringKonekcije = "Data Source=" + _nazivDBMSinstance + " ;Initial Catalog=" + _nazivBaze + ";Integrated Security=True";
                }
                else
                {
                    pomStringKonekcije = "Data Source=.\\" + _nazivDBMSinstance + ";AttachDbFilename=" + _putanjaBaze + "\\" + _nazivBaze + ";Integrated Security=True;Connect Timeout=30;User Instance=True";
                }
            }
            else
            {
                // AKO IMAMO VEC DAT GOTOV STRING KONEKCIJE
                pomStringKonekcije = _stringKonekcije;
            }
            return pomStringKonekcije;
        }
        #endregion

        #region Javne metode
        public bool OtvoriKonekciju()
        {
            bool uspeh;
            _konekcija = new SqlConnection();
            _konekcija.ConnectionString = DajStringKonekcije();

            try
            {
                _konekcija.Open();
                uspeh = true;
            }
            catch
            {
                uspeh = false;
            }
            return uspeh;
        }

        public SqlConnection DajKonekciju()
        {
            return _konekcija;
        }

        public void ZatvoriKonekciju()
        {
            _putanjaBaze = "";
            _konekcija.Close();
            _konekcija.Dispose();
        }

        #endregion

    }
}
