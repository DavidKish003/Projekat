using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
     public class SPDokumentacijaZahtevaDBKlasa
     {
          private string _stringKonekcije;

          public string StringKonekcije
          {
               get { return _stringKonekcije; }
          }

          public SPDokumentacijaZahtevaDBKlasa(string noviStringKonekcije)
          {
               _stringKonekcije = noviStringKonekcije;
          }

          public DataSet DajDokumentacijuZaZahtev(int zahtevID)
          {
               DataSet podaciDataSet = new DataSet();

               SqlConnection konekcija = new SqlConnection(_stringKonekcije);
               konekcija.Open();

               SqlCommand komanda = new SqlCommand("DajDokumentacijuZaZahtev", konekcija);
               komanda.CommandType = CommandType.StoredProcedure;
               komanda.Parameters.Add("@ZahtevID", SqlDbType.Int).Value = zahtevID;

               SqlDataAdapter adapter = new SqlDataAdapter();
               adapter.SelectCommand = komanda;
               adapter.Fill(podaciDataSet);

               konekcija.Close();
               konekcija.Dispose();

               return podaciDataSet;
          }

          public bool DodajDokumentaciju(int zahtevID, int tipDokumentaID, bool dostavljen, string napomena)
          {
               int brojSlogova = 0;

               SqlConnection konekcija = new SqlConnection(_stringKonekcije);
               konekcija.Open();

               SqlCommand komanda = new SqlCommand("DodajDokumentacijuZahteva", konekcija);
               komanda.CommandType = CommandType.StoredProcedure;

               komanda.Parameters.Add("@ZahtevID", SqlDbType.Int).Value = zahtevID;
               komanda.Parameters.Add("@TipDokumentaID", SqlDbType.Int).Value = tipDokumentaID;
               komanda.Parameters.Add("@Dostavljen", SqlDbType.Bit).Value = dostavljen;
               komanda.Parameters.Add("@Napomena", SqlDbType.NVarChar).Value = napomena;

               brojSlogova = komanda.ExecuteNonQuery();

               konekcija.Close();
               konekcija.Dispose();

               return brojSlogova > 0;
          }

          public bool IzmeniDokumentaciju(int dokumentacijaID, int zahtevID, int tipDokumentaID, bool dostavljen, string napomena)
          {
               int brojSlogova = 0;

               SqlConnection konekcija = new SqlConnection(_stringKonekcije);
               konekcija.Open();

               SqlCommand komanda = new SqlCommand("IzmeniDokumentacijuZahteva", konekcija);
               komanda.CommandType = CommandType.StoredProcedure;

               komanda.Parameters.Add("@DokumentacijaID", SqlDbType.Int).Value = dokumentacijaID;
               komanda.Parameters.Add("@ZahtevID", SqlDbType.Int).Value = zahtevID;
               komanda.Parameters.Add("@TipDokumentaID", SqlDbType.Int).Value = tipDokumentaID;
               komanda.Parameters.Add("@Dostavljen", SqlDbType.Bit).Value = dostavljen;
               komanda.Parameters.Add("@Napomena", SqlDbType.NVarChar).Value = napomena;

               brojSlogova = komanda.ExecuteNonQuery();

               konekcija.Close();
               konekcija.Dispose();

               return brojSlogova > 0;
          }

          public bool ObrisiDokumentaciju(int dokumentacijaID)
          {
               int brojSlogova = 0;

               SqlConnection konekcija = new SqlConnection(_stringKonekcije);
               konekcija.Open();

               SqlCommand komanda = new SqlCommand("ObrisiDokumentacijuZahteva", konekcija);
               komanda.CommandType = CommandType.StoredProcedure;

               komanda.Parameters.Add("@DokumentacijaID", SqlDbType.Int).Value = dokumentacijaID;

               brojSlogova = komanda.ExecuteNonQuery();

               konekcija.Close();
               konekcija.Dispose();

               return brojSlogova > 0;
          }

          public bool ObrisiDokumentacijuPoZahtevu(int zahtevID)
          {
               int brojSlogova = 0;

               SqlConnection konekcija = new SqlConnection(_stringKonekcije);
               konekcija.Open();

               SqlCommand komanda = new SqlCommand("ObrisiDokumentacijuPoZahtevu", konekcija);
               komanda.CommandType = CommandType.StoredProcedure;

               komanda.Parameters.Add("@ZahtevID", SqlDbType.Int).Value = zahtevID;

               brojSlogova = komanda.ExecuteNonQuery();

               konekcija.Close();
               konekcija.Dispose();

               return brojSlogova > 0;
          }
     }
}