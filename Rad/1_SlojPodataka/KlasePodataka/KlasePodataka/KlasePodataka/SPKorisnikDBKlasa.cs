using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
     public class SPKorisnikDBKlasa
     {
          private string _stringKonekcije;

          public string StringKonekcije
          {
               get { return _stringKonekcije; }
          }

          public SPKorisnikDBKlasa(string noviStringKonekcije)
          {
               _stringKonekcije = noviStringKonekcije;
          }

          public DataSet DajKorisnika(string korisnickoIme, string lozinka)
          {
               DataSet podaciDataSet = new DataSet();

               SqlConnection konekcija = new SqlConnection(_stringKonekcije);
               konekcija.Open();

               SqlCommand komanda = new SqlCommand("DajKorisnikaPoKorisnickomImenuILozinci", konekcija);
               komanda.CommandType = CommandType.StoredProcedure;

               komanda.Parameters.Add("@KorisnickoIme", SqlDbType.NVarChar).Value = korisnickoIme;
               komanda.Parameters.Add("@Lozinka", SqlDbType.NVarChar).Value = lozinka;

               SqlDataAdapter adapter = new SqlDataAdapter();
               adapter.SelectCommand = komanda;
               adapter.Fill(podaciDataSet);

               konekcija.Close();
               konekcija.Dispose();

               return podaciDataSet;
          }
     }
}