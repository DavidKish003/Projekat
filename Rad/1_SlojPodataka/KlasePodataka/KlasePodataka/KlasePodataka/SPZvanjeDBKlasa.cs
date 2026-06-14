using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPZvanjeDBKlasa
    {
        // atributi
        private string _stringKonekcije;

        // property
        // 1. nacin
        public string StringKonekcije
        {
            get
            {
                return _stringKonekcije;
            }
            set // OVO NIJE DOBRO, MOZE SE STRING KONEKCIJE STAVITI NA PRAZAN STRING
            {
                if (this._stringKonekcije != value)
                    this._stringKonekcije = value;
            }
        }
        // konstruktor
        // 2. nacin prijema vrednosti stringa konekcije spolja i dodele atributu
        public SPZvanjeDBKlasa(string noviStringKonekcije)
        // OVO JE DOBRO JER OBAVEZUJE DA SE PRILIKOM INSTANCIRANJA OVE KLASE
        // MORA OBEZBEDITI STRING KONEKCIJE
        {
            _stringKonekcije = noviStringKonekcije; 
        }

        // privatne metode

        // javne metode
        public DataSet DajSvaZvanja()
        {
            // MOGU biti jos neke procedure, mogu SE VRATITI VREDNOSTI I U LISTU, DATA TABLE...
            DataSet PodaciDataSet = new DataSet();

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();
            SqlCommand pomKomanda = new SqlCommand("DajSvaZvanja", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = pomKomanda;
            adapter.Fill(PodaciDataSet);
            pomKonekcija.Close();
            pomKonekcija.Dispose();
            
            return PodaciDataSet;
        }

        public string DajNazivPremaIDZvanja(string IDZvanjaFilter)
        {
            string pomNazivZvanja = "";

            // MOGU biti jos neke procedure, mogu SE VRATITI VREDNOSTI I U LISTU, DATA TABLE...
            DataSet PodaciDataSet = new DataSet();

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();
            SqlCommand pomKomanda = new SqlCommand("DajZvanjePoSifri", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@SifraZvanja", SqlDbType.Char).Value = IDZvanjaFilter;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = pomKomanda;
            adapter.Fill(PodaciDataSet);
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            pomNazivZvanja = PodaciDataSet.Tables[0].Rows[0].ItemArray[1].ToString();

            return pomNazivZvanja;
        }

        public string DajSifruZvanjaPoNazivu(string nazivZvanjaFilter)
        {
            // MOGU biti jos neke procedure, mogu SE VRATITI VREDNOSTI I U LISTU, DATA TABLE...
            DataSet PodaciDataSet = new DataSet();

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();
            SqlCommand pomKomanda = new SqlCommand("DajZvanjePoNazivu", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@NazivZvanja", SqlDbType.NVarChar).Value = nazivZvanjaFilter;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = pomKomanda;
            adapter.Fill(PodaciDataSet);
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return PodaciDataSet.Tables[0].Rows[0].ItemArray[0].ToString() ;
        }



        public DataSet DajZvanjaPoNazivu(string nazivZvanjaFilter)
        {
            // MOGU biti jos neke procedure, mogu SE VRATITI VREDNOSTI I U LISTU, DATA TABLE...
            DataSet dsPodaci = new DataSet();

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();
            SqlCommand pomKomanda = new SqlCommand("DajZvanjePoNazivu", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@NazivZvanja", SqlDbType.NVarChar).Value = nazivZvanjaFilter;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = pomKomanda;
            adapter.Fill(dsPodaci);
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return dsPodaci;
        }

        // overloading metoda - isto se zove, ima drugaciji parametar
        public DataSet DajZvanjaPoNazivu(ZvanjeKlasa zvanjeObjekatZaFilter)
        {
            // MOGU biti jos neke procedure, mogu SE VRATITI VREDNOSTI I U LISTU, DATA TABLE...
            DataSet dsPodaci = new DataSet();

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();
            SqlCommand pomKomanda = new SqlCommand("DajZvanjePoNazivu", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@NazivZvanja", SqlDbType.NVarChar).Value = zvanjeObjekatZaFilter.Naziv;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = pomKomanda;
            adapter.Fill(dsPodaci);
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return dsPodaci;
        }

        public bool SnimiNovoZvanje(ZvanjeKlasa novoZvanjeObjekat)
        {
            // LOKALNE PROMENLJIVE UVEK NA VRHU
            int brojSlogova =0;
            // 1. varijanta - skolska 
            //bool uspehSnimanja= false;

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();

            SqlCommand pomKomanda = new SqlCommand("DodajNovoZvanje", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@Sifra", SqlDbType.Char).Value = novoZvanjeObjekat.Sifra;
            pomKomanda.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = novoZvanjeObjekat.Naziv;
           
            brojSlogova = pomKomanda.ExecuteNonQuery();
            pomKonekcija.Close();
            pomKonekcija.Dispose();

                      
            
            return (brojSlogova > 0);

            //3. varijanta
            // NEMA SMISLA, ISTO KAO 2. VARIJANTA
            //return (brojSlogova > 0) ? true : false;
            
            //4. varijanta - NEGACIJA NEGACIJE, NIJE RAZUMLJIVO 
            //return (brojSlogova == 0) ? false : true;
            
        }

        public bool ObrisiZvanje(ZvanjeKlasa zvanjeObjekatZaBrisanje)
        {
            // LOKALNE PROMENLJIVE UVEK NA VRHU
            int brojSlogova = 0;
            // 1. varijanta - skolska 
            //bool uspehSnimanja= false;

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();

            SqlCommand pomKomanda = new SqlCommand("ObrisiZvanje", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@Sifra", SqlDbType.Char).Value = zvanjeObjekatZaBrisanje.Sifra;
            brojSlogova = pomKomanda.ExecuteNonQuery();
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return (brojSlogova > 0);

         }

        // method overloading - ista procedura sa razlicitim parametrom
        public bool ObrisiZvanje(string sifraZvanjaZaBrisanje)
        {
            // LOKALNE PROMENLJIVE UVEK NA VRHU
            int brojSlogova = 0;
            // 1. varijanta - skolska 
            //bool uspehSnimanja= false;

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();

            SqlCommand pomKomanda = new SqlCommand("ObrisiZvanje", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@Sifra", SqlDbType.Char).Value = sifraZvanjaZaBrisanje;
            brojSlogova = pomKomanda.ExecuteNonQuery();
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return (brojSlogova > 0);

        }

        public bool IzmeniZvanje(ZvanjeKlasa staroZvanjeObjekat, ZvanjeKlasa novoZvanjeObjekat)
        {
            // LOKALNE PROMENLJIVE UVEK NA VRHU
            int brojSlogova = 0;
            // 1. varijanta - skolska 
            //bool uspehSnimanja= false;

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();

            SqlCommand pomKomanda = new SqlCommand("IzmeniZvanje", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@StaraSifra", SqlDbType.Char).Value = staroZvanjeObjekat.Sifra;
            pomKomanda.Parameters.Add("@Sifra", SqlDbType.Char).Value = novoZvanjeObjekat.Sifra;
            pomKomanda.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = novoZvanjeObjekat.Naziv;
            brojSlogova = pomKomanda.ExecuteNonQuery();
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return (brojSlogova > 0);

        }

        // method overloading - ista metoda, samo drugaciji parametri
        public bool IzmeniZvanje(string sifraStarogZvanja, ZvanjeKlasa novoZvanjeObjekat)
        {
            // LOKALNE PROMENLJIVE UVEK NA VRHU
            int brojSlogova = 0;
            // 1. varijanta - skolska 
            //bool uspehSnimanja= false;

            SqlConnection pomKonekcija = new SqlConnection(_stringKonekcije);
            pomKonekcija.Open();

            SqlCommand pomKomanda = new SqlCommand("IzmeniZvanje", pomKonekcija);
            pomKomanda.CommandType = CommandType.StoredProcedure;
            pomKomanda.Parameters.Add("@StaraSifra", SqlDbType.Char).Value = sifraStarogZvanja;
            pomKomanda.Parameters.Add("@Sifra", SqlDbType.Char).Value = novoZvanjeObjekat.Sifra;
            pomKomanda.Parameters.Add("@Naziv", SqlDbType.NVarChar).Value = novoZvanjeObjekat.Naziv;
            brojSlogova = pomKomanda.ExecuteNonQuery();
            pomKonekcija.Close();
            pomKonekcija.Dispose();

            return (brojSlogova > 0);

        }


    }
}
