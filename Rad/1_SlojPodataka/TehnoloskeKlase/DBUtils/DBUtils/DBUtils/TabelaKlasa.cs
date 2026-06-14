using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//
using System.Data.SqlClient;
using System.Data;


namespace DBUtils
{
    public class TabelaKlasa
    {
        /* CRC: 
          * Responsibility - ODGOVORNOST: Konekcija na celinu baze podataka, SQL server tipa  
          Collaboration - zavisi od standardne klase SQlDataAdapter iz biblioteke System.Data.SqlClient
                          kao i klase Dataset iz standardne biblioteke System.Data*/

        #region Atributi

        private string _nazivTabele;
        private KonekcijaKlasa _konekcijaObjekat;
        private SqlDataAdapter _adapterObjekat;
        private System.Data.DataSet _dataSetObjekat;
        
        #endregion

        #region Konstruktor

        public TabelaKlasa(KonekcijaKlasa novaKonekcija, string noviNazivTabele)
        {
            _konekcijaObjekat = novaKonekcija;
            _nazivTabele = noviNazivTabele;
        }
        
        #endregion

        #region Privatne metode

        private void KreirajAdapter(string selectUpit, string insertUpit, string deleteUpit, string updateUpit)
        {
            SqlCommand pomSelectKomanda, pomInsertKomanda, pomDeleteKomanda, pomUpdateKomanda;

            pomSelectKomanda = new SqlCommand();
            pomSelectKomanda.CommandText = selectUpit;
            pomSelectKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            pomInsertKomanda = new SqlCommand();
            pomInsertKomanda.CommandText = insertUpit;
            pomInsertKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            pomDeleteKomanda = new SqlCommand();
            pomDeleteKomanda.CommandText = deleteUpit;
            pomDeleteKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            pomUpdateKomanda = new SqlCommand();
            pomUpdateKomanda.CommandText = updateUpit;
            pomUpdateKomanda.Connection = _konekcijaObjekat.DajKonekciju();

            _adapterObjekat = new SqlDataAdapter();
            _adapterObjekat.SelectCommand = pomSelectKomanda;
            _adapterObjekat.InsertCommand = pomInsertKomanda;
            _adapterObjekat.UpdateCommand = pomUpdateKomanda;
            _adapterObjekat.DeleteCommand = pomDeleteKomanda;
        }

        private void KreirajDataset()
        {
            _dataSetObjekat = new System.Data.DataSet();
            _adapterObjekat.Fill(_dataSetObjekat, _nazivTabele);
            
        }

        private void ZatvoriAdapterDataset()
        {
            _adapterObjekat.Dispose();
            _dataSetObjekat.Dispose();
        }
        
        #endregion

        #region Javne metode

        public DataSet DajPodatke(string selectUpit)
            // izdvaja podatke u odnosu na dat selectupit
        {
            KreirajAdapter(selectUpit, "", "", "");
            KreirajDataset();
            return _dataSetObjekat;
        }

        public int DajBrojSlogova()
        {
            int BrojSlogova = _dataSetObjekat.Tables[0].Rows.Count; 
            return BrojSlogova;
        }

        public bool IzvrsiAzuriranje(string Upit)
            // izvrzava azuriranje unos/brisanje/izmena u odnosu na dati i upit
        {

            //
            bool uspeh = false;
           SqlConnection pomKonekcija;
           SqlCommand pomKomanda;
           SqlTransaction pomTransakcija = null; 
            try
            {
                pomKonekcija = _konekcijaObjekat.DajKonekciju();
                // aktivan kod  

                // povezivanje
                pomKomanda = new SqlCommand();
                pomKomanda.Connection = pomKonekcija;
                pomKomanda = pomKonekcija.CreateCommand();
                // pokretanje
                // NE TREBA OPEN JER DOBIJAMO OTVORENU KONEKCIJU KROZ KONSTRUKTOR
                // mKonekcija.Open();
                pomTransakcija = pomKonekcija.BeginTransaction();
                pomKomanda.Transaction = pomTransakcija;
                pomKomanda.CommandText = Upit;
                pomKomanda.ExecuteNonQuery();
                pomTransakcija.Commit();
                uspeh = true;
            }
            catch
            {
                pomTransakcija.Rollback();
                uspeh = false;
            }
            return uspeh;
        }

        // druga varijanta - preklapajuca (overload) metoda kada dobijemo vise upita da se izvrsi u transakciji
        public bool IzvrsiAzuriranje(List<string> listaUpita)
        // izvrzava azuriranje unos/brisanje/izmena 
            // moze se dodeliti kao parametar lista od vise upita
            // sada transakcija ima smisla, jer izvrsava vise upita u paketu
        {

            //
            bool uspeh = false;
            SqlConnection pomKonekcija;
            SqlCommand pomKomanda;
            SqlTransaction pomTransakcija = null;
            try
            {
                pomKonekcija = _konekcijaObjekat.DajKonekciju();
                // aktivan kod  

                // povezivanje
                pomKomanda = new SqlCommand();
                pomKomanda.Connection = pomKonekcija;
                pomKomanda = pomKonekcija.CreateCommand();
                // pokretanje
                // NE TREBA OPEN JER DOBIJAMO OTVORENU KONEKCIJU KROZ KONSTRUKTOR
                // mKonekcija.Open();
                string pomUpit="";
                pomTransakcija = pomKonekcija.BeginTransaction();
                pomKomanda.Transaction = pomTransakcija;
                for (int i = 0; i < listaUpita.Count(); i++)
                {
                    pomUpit = listaUpita[i];
                    pomKomanda.CommandText = pomUpit;
                    pomKomanda.ExecuteNonQuery();
                }
                pomTransakcija.Commit();
                uspeh = true;
            }
            catch
            {
                pomTransakcija.Rollback();
                uspeh = false;
            }
            return uspeh;
        }


        #endregion

    }
}
