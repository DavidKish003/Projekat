using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPZahtevZaRegistracijuDBKlasa
    {
        private readonly string stringKonekcije;

        public SPZahtevZaRegistracijuDBKlasa(
            string stringKonekcije)
        {
            this.stringKonekcije = stringKonekcije;
        }

        private DataSet IzvrsiSelectProceduru(
            string nazivProcedure,
            params SqlParameter[] parametri)
        {
            DataSet podaciSet = new DataSet();

            using (SqlConnection konekcija =
                   new SqlConnection(stringKonekcije))
            using (SqlCommand komanda =
                   new SqlCommand(nazivProcedure, konekcija))
            {
                komanda.CommandType =
                    CommandType.StoredProcedure;

                if (parametri != null &&
                    parametri.Length > 0)
                {
                    komanda.Parameters.AddRange(parametri);
                }

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(komanda))
                {
                    adapter.Fill(podaciSet);
                }
            }

            return podaciSet;
        }

        public DataSet DajSveZahteveZaRegistraciju()
        {
            return IzvrsiSelectProceduru(
                "DajSveZahteveZaRegistraciju"
            );
        }

        public DataSet DajPrijaveKorisnika(
            int korisnikId)
        {
            SqlParameter parametar =
                new SqlParameter(
                    "@KorisnikID",
                    SqlDbType.Int
                );

            parametar.Value = korisnikId;

            return IzvrsiSelectProceduru(
                "DajPrijaveKorisnika",
                parametar
            );
        }

        public DataSet DajZahteveZaRegistracijuSaFilterom(
            string filter)
        {
            SqlParameter parametar =
                new SqlParameter(
                    "@Filter",
                    SqlDbType.NVarChar,
                    200
                );

            parametar.Value =
                string.IsNullOrWhiteSpace(filter)
                    ? (object)DBNull.Value
                    : filter;

            return IzvrsiSelectProceduru(
                "DajZahteveZaRegistracijuSaFilterom",
                parametar
            );
        }

        public DataSet DajZahtevZaRegistracijuSaDokumentacijom(
            int zahtevId)
        {
            SqlParameter parametar =
                new SqlParameter(
                    "@ZahtevID",
                    SqlDbType.Int
                );

            parametar.Value = zahtevId;

            return IzvrsiSelectProceduru(
                "DajZahtevZaRegistracijuSaDokumentacijom",
                parametar
            );
        }

        private SqlCommand KreirajKomanduZaZahtev(
            SqlConnection konekcija,
            SqlTransaction transakcija,
            string nazivProcedure,
            ZahtevZaRegistracijuKlasa zahtev,
            bool dodajZahtevId)
        {
            SqlCommand komanda =
                new SqlCommand(
                    nazivProcedure,
                    konekcija,
                    transakcija
                );

            komanda.CommandType =
                CommandType.StoredProcedure;

            if (dodajZahtevId)
            {
                komanda.Parameters.Add(
                    "@ZahtevID",
                    SqlDbType.Int
                ).Value = zahtev.ZahtevID;
            }
            else
            {
                komanda.Parameters.Add(
                    "@KorisnikID",
                    SqlDbType.Int
                ).Value = zahtev.KorisnikID;
            }

            komanda.Parameters.Add(
                "@ImePrezime",
                SqlDbType.NVarChar,
                100
            ).Value = zahtev.ImePrezime;

            komanda.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                100
            ).Value = zahtev.Email;

            komanda.Parameters.Add(
                "@KontaktTelefon",
                SqlDbType.NVarChar,
                20
            ).Value = zahtev.KontaktTelefon;

            komanda.Parameters.Add(
                "@PoslednjaSkola",
                SqlDbType.NVarChar,
                150
            ).Value = zahtev.PoslednjaSkola;

            komanda.Parameters.Add(
                "@MestoSkole",
                SqlDbType.NVarChar,
                100
            ).Value = string.IsNullOrWhiteSpace(
                zahtev.MestoSkole)
                    ? (object)DBNull.Value
                    : zahtev.MestoSkole;

            komanda.Parameters.Add(
                "@Zanimanje",
                SqlDbType.NVarChar,
                150
            ).Value = zahtev.Zanimanje;

            komanda.Parameters.Add(
                "@NazivKonkursa",
                SqlDbType.NVarChar,
                150
            ).Value = zahtev.NazivKonkursa;

            komanda.Parameters.Add(
                "@RadnoMesto",
                SqlDbType.NVarChar,
                150
            ).Value = zahtev.RadnoMesto;

            komanda.Parameters.Add(
                "@StepenObrazovanja",
                SqlDbType.NVarChar,
                80
            ).Value = zahtev.StepenObrazovanja;

            komanda.Parameters.Add(
                "@GodineIskustva",
                SqlDbType.Int
            ).Value = zahtev.GodineIskustva;

            komanda.Parameters.Add(
                "@MotivacionoPismo",
                SqlDbType.NVarChar,
                -1
            ).Value = string.IsNullOrWhiteSpace(
                zahtev.MotivacionoPismo)
                    ? (object)DBNull.Value
                    : zahtev.MotivacionoPismo;

            komanda.Parameters.Add(
                "@DatumPodnosenja",
                SqlDbType.Date
            ).Value = zahtev.DatumPodnosenja.Date;

            komanda.Parameters.Add(
                "@RokKonkursa",
                SqlDbType.Date
            ).Value = zahtev.RokKonkursa.Date;

            komanda.Parameters.Add(
                "@StatusZahteva",
                SqlDbType.NVarChar,
                30
            ).Value = zahtev.StatusZahteva;

            komanda.Parameters.Add(
                "@IspunjavaOsnovneUslove",
                SqlDbType.Bit
            ).Value = zahtev.IspunjavaOsnovneUslove;

            return komanda;
        }

        private void DodajDokumentaciju(
            SqlConnection konekcija,
            SqlTransaction transakcija,
            int zahtevId,
            List<int> dokumenta)
        {
            if (dokumenta == null)
            {
                return;
            }

            foreach (int tipDokumentaId in dokumenta)
            {
                using (SqlCommand komanda =
                       new SqlCommand(
                           "DodajDokumentacijuZahteva",
                           konekcija,
                           transakcija))
                {
                    komanda.CommandType =
                        CommandType.StoredProcedure;

                    komanda.Parameters.Add(
                        "@ZahtevID",
                        SqlDbType.Int
                    ).Value = zahtevId;

                    komanda.Parameters.Add(
                        "@TipDokumentaID",
                        SqlDbType.Int
                    ).Value = tipDokumentaId;

                    komanda.Parameters.Add(
                        "@Dostavljen",
                        SqlDbType.Bit
                    ).Value = true;

                    komanda.Parameters.Add(
                        "@Napomena",
                        SqlDbType.NVarChar,
                        200
                    ).Value = string.Empty;

                    komanda.ExecuteNonQuery();
                }
            }
        }

        public bool SnimiNoviZahtevSaDokumentacijom(
            ZahtevZaRegistracijuKlasa zahtev,
            List<int> dokumenta)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                konekcija.Open();

                using (SqlTransaction transakcija =
                       konekcija.BeginTransaction())
                {
                    try
                    {
                        int zahtevId;

                        using (SqlCommand komanda =
                               KreirajKomanduZaZahtev(
                                   konekcija,
                                   transakcija,
                                   "DodajNoviZahtevZaRegistraciju",
                                   zahtev,
                                   false))
                        {
                            zahtevId = Convert.ToInt32(
                                komanda.ExecuteScalar()
                            );
                        }

                        DodajDokumentaciju(
                            konekcija,
                            transakcija,
                            zahtevId,
                            dokumenta
                        );

                        transakcija.Commit();
                        return true;
                    }
                    catch
                    {
                        transakcija.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool IzmeniZahtevSaDokumentacijom(
            ZahtevZaRegistracijuKlasa zahtev,
            List<int> dokumenta)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                konekcija.Open();

                using (SqlTransaction transakcija =
                       konekcija.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand komanda =
                               KreirajKomanduZaZahtev(
                                   konekcija,
                                   transakcija,
                                   "IzmeniZahtevZaRegistraciju",
                                   zahtev,
                                   true))
                        {
                            komanda.ExecuteNonQuery();
                        }

                        using (SqlCommand brisanje =
                               new SqlCommand(
                                   "ObrisiDokumentacijuZaZahtev",
                                   konekcija,
                                   transakcija))
                        {
                            brisanje.CommandType =
                                CommandType.StoredProcedure;

                            brisanje.Parameters.Add(
                                "@ZahtevID",
                                SqlDbType.Int
                            ).Value = zahtev.ZahtevID;

                            brisanje.ExecuteNonQuery();
                        }

                        DodajDokumentaciju(
                            konekcija,
                            transakcija,
                            zahtev.ZahtevID,
                            dokumenta
                        );

                        transakcija.Commit();
                        return true;
                    }
                    catch
                    {
                        transakcija.Rollback();
                        throw;
                    }
                }
            }
        }

        private bool IzvrsiKomanduZaZahtev(
            string nazivProcedure,
            int zahtevId,
            string statusZahteva)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            using (SqlCommand komanda =
                   new SqlCommand(nazivProcedure, konekcija))
            {
                komanda.CommandType =
                    CommandType.StoredProcedure;

                komanda.Parameters.Add(
                    "@ZahtevID",
                    SqlDbType.Int
                ).Value = zahtevId;

                if (statusZahteva != null)
                {
                    komanda.Parameters.Add(
                        "@StatusZahteva",
                        SqlDbType.NVarChar,
                        30
                    ).Value = statusZahteva;
                }

                konekcija.Open();
                return komanda.ExecuteNonQuery() > 0;
            }
        }

        public bool ObrisiZahtevZaRegistraciju(
            int zahtevId)
        {
            return IzvrsiKomanduZaZahtev(
                "ObrisiZahtevZaRegistraciju",
                zahtevId,
                null
            );
        }

        public bool PromeniStatusZahteva(
            int zahtevId,
            string statusZahteva)
        {
            return IzvrsiKomanduZaZahtev(
                "PromeniStatusZahteva",
                zahtevId,
                statusZahteva
            );
        }
    }
}
