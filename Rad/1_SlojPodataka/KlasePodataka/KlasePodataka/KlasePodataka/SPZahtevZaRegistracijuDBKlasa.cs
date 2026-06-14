using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPZahtevZaRegistracijuDBKlasa
    {
        private readonly string connectionString;

        public SPZahtevZaRegistracijuDBKlasa(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private DataSet IzvrsiSelectProceduru(
            string nazivProcedure,
            params SqlParameter[] parametri)
        {
            DataSet dataSet = new DataSet();

            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand komanda =
                       new SqlCommand(nazivProcedure, konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;

                    if (parametri != null && parametri.Length > 0)
                    {
                        komanda.Parameters.AddRange(parametri);
                    }

                    using (SqlDataAdapter adapter =
                           new SqlDataAdapter(komanda))
                    {
                        adapter.Fill(dataSet);
                    }
                }
            }

            return dataSet;
        }

        public DataSet DajSveZahteveZaRegistraciju()
        {
            return IzvrsiSelectProceduru(
                "DajSveZahteveZaRegistraciju"
            );
        }

        public DataSet DajZahteveZaRegistracijuSaFilterom(
            string filter)
        {
            SqlParameter parametarFilter =
                new SqlParameter("@Filter", SqlDbType.NVarChar, 200);

            parametarFilter.Value =
                string.IsNullOrWhiteSpace(filter)
                    ? (object)DBNull.Value
                    : filter;

            return IzvrsiSelectProceduru(
                "DajZahteveZaRegistracijuSaFilterom",
                parametarFilter
            );
        }

        public DataSet DajZahtevZaRegistracijuSaDokumentacijom(
            int zahtevId)
        {
            SqlParameter parametarZahtevId =
                new SqlParameter("@ZahtevID", SqlDbType.Int);

            parametarZahtevId.Value = zahtevId;

            return IzvrsiSelectProceduru(
                "DajZahtevZaRegistracijuSaDokumentacijom",
                parametarZahtevId
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

            komanda.CommandType = CommandType.StoredProcedure;

            if (dodajZahtevId)
            {
                komanda.Parameters.Add(
                    "@ZahtevID",
                    SqlDbType.Int
                ).Value = zahtev.ZahtevID;
            }

            komanda.Parameters.Add(
                "@ImePrezime",
                SqlDbType.NVarChar,
                150
            ).Value = zahtev.ImePrezime;

            komanda.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                150
            ).Value = zahtev.Email;

            komanda.Parameters.Add(
                "@KontaktTelefon",
                SqlDbType.NVarChar,
                50
            ).Value = zahtev.KontaktTelefon;

            komanda.Parameters.Add(
                "@NazivKonkursa",
                SqlDbType.NVarChar,
                200
            ).Value = zahtev.NazivKonkursa;

            komanda.Parameters.Add(
                "@StepenObrazovanja",
                SqlDbType.NVarChar,
                100
            ).Value = zahtev.StepenObrazovanja;

            komanda.Parameters.Add(
                "@GodineIskustva",
                SqlDbType.Int
            ).Value = zahtev.GodineIskustva;

            komanda.Parameters.Add(
                "@MotivacionoPismo",
                SqlDbType.NVarChar,
                -1
            ).Value =
                string.IsNullOrWhiteSpace(zahtev.MotivacionoPismo)
                    ? (object)DBNull.Value
                    : zahtev.MotivacionoPismo;

            komanda.Parameters.Add(
                "@DatumPodnosenja",
                SqlDbType.DateTime
            ).Value = zahtev.DatumPodnosenja;

            komanda.Parameters.Add(
                "@RokKonkursa",
                SqlDbType.Date
            ).Value = zahtev.RokKonkursa;

            komanda.Parameters.Add(
                "@StatusZahteva",
                SqlDbType.NVarChar,
                50
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
                        500
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
                            object rezultat =
                                komanda.ExecuteScalar();

                            zahtevId =
                                Convert.ToInt32(rezultat);
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

                        ObrisiDokumentacijuZaZahtev(
                            konekcija,
                            transakcija,
                            zahtev.ZahtevID
                        );

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

        private void ObrisiDokumentacijuZaZahtev(
            SqlConnection konekcija,
            SqlTransaction transakcija,
            int zahtevId)
        {
            using (SqlCommand komanda =
                   new SqlCommand(
                       "ObrisiDokumentacijuZaZahtev",
                       konekcija,
                       transakcija))
            {
                komanda.CommandType =
                    CommandType.StoredProcedure;

                komanda.Parameters.Add(
                    "@ZahtevID",
                    SqlDbType.Int
                ).Value = zahtevId;

                komanda.ExecuteNonQuery();
            }
        }

        private bool IzvrsiKomanduZaZahtev(
            string nazivProcedure,
            int zahtevId,
            string statusZahteva = null)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand komanda =
                       new SqlCommand(
                           nazivProcedure,
                           konekcija))
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
                            50
                        ).Value = statusZahteva;
                    }

                    konekcija.Open();

                    int brojPromenjenihRedova =
                        komanda.ExecuteNonQuery();

                    return brojPromenjenihRedova > 0;
                }
            }
        }

        public bool ObrisiZahtevZaRegistraciju(int zahtevId)
        {
            return IzvrsiKomanduZaZahtev(
                "ObrisiZahtevZaRegistraciju",
                zahtevId
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