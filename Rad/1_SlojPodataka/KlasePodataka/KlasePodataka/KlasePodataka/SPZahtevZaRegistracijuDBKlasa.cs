using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPDokumentacijaZahtevaDBKlasa
    {
        private readonly string _stringKonekcije;

        public string StringKonekcije
        {
            get { return _stringKonekcije; }
        }

        public SPDokumentacijaZahtevaDBKlasa(
            string noviStringKonekcije)
        {
            if (string.IsNullOrWhiteSpace(noviStringKonekcije))
            {
                throw new ArgumentException(
                    "String konekcije nije podešen.",
                    "noviStringKonekcije"
                );
            }

            _stringKonekcije = noviStringKonekcije;
        }

        public DataSet DajDokumentacijuZaZahtev(
            int zahtevID)
        {
            DataSet podaciDataSet = new DataSet();

            using (SqlConnection konekcija =
                   new SqlConnection(_stringKonekcije))
            using (SqlCommand komanda =
                   new SqlCommand(
                       "DajDokumentacijuZaZahtev",
                       konekcija))
            {
                komanda.CommandType =
                    CommandType.StoredProcedure;

                komanda.Parameters.Add(
                    "@ZahtevID",
                    SqlDbType.Int
                ).Value = zahtevID;

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(komanda))
                {
                    adapter.Fill(podaciDataSet);
                }
            }

            return podaciDataSet;
        }

        private bool IzvrsiTransakcionuProceduru(
            string nazivProcedure,
            params SqlParameter[] parametri)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(_stringKonekcije))
            {
                konekcija.Open();

                using (SqlTransaction transakcija =
                       konekcija.BeginTransaction(
                           IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        using (SqlCommand komanda =
                               new SqlCommand(
                                   nazivProcedure,
                                   konekcija,
                                   transakcija))
                        {
                            komanda.CommandType =
                                CommandType.StoredProcedure;

                            if (parametri != null &&
                                parametri.Length > 0)
                            {
                                komanda.Parameters.AddRange(
                                    parametri
                                );
                            }

                            komanda.ExecuteNonQuery();
                        }

                        transakcija.Commit();

                        return true;
                    }
                    catch
                    {
                        if (transakcija.Connection != null)
                        {
                            transakcija.Rollback();
                        }

                        throw;
                    }
                }
            }
        }

        public bool DodajDokumentaciju(
            int zahtevID,
            int tipDokumentaID,
            bool dostavljen,
            string napomena)
        {
            return IzvrsiTransakcionuProceduru(
                "DodajDokumentacijuZahteva",
                new SqlParameter(
                    "@ZahtevID",
                    SqlDbType.Int
                )
                {
                    Value = zahtevID
                },
                new SqlParameter(
                    "@TipDokumentaID",
                    SqlDbType.Int
                )
                {
                    Value = tipDokumentaID
                },
                new SqlParameter(
                    "@Dostavljen",
                    SqlDbType.Bit
                )
                {
                    Value = dostavljen
                },
                new SqlParameter(
                    "@Napomena",
                    SqlDbType.NVarChar,
                    200
                )
                {
                    Value = string.IsNullOrWhiteSpace(napomena)
                        ? (object)DBNull.Value
                        : napomena
                }
            );
        }

        public bool IzmeniDokumentaciju(
            int dokumentacijaID,
            int zahtevID,
            int tipDokumentaID,
            bool dostavljen,
            string napomena)
        {
            return IzvrsiTransakcionuProceduru(
                "IzmeniDokumentacijuZahteva",
                new SqlParameter(
                    "@DokumentacijaID",
                    SqlDbType.Int
                )
                {
                    Value = dokumentacijaID
                },
                new SqlParameter(
                    "@ZahtevID",
                    SqlDbType.Int
                )
                {
                    Value = zahtevID
                },
                new SqlParameter(
                    "@TipDokumentaID",
                    SqlDbType.Int
                )
                {
                    Value = tipDokumentaID
                },
                new SqlParameter(
                    "@Dostavljen",
                    SqlDbType.Bit
                )
                {
                    Value = dostavljen
                },
                new SqlParameter(
                    "@Napomena",
                    SqlDbType.NVarChar,
                    200
                )
                {
                    Value = string.IsNullOrWhiteSpace(napomena)
                        ? (object)DBNull.Value
                        : napomena
                }
            );
        }

        public bool ObrisiDokumentaciju(
            int dokumentacijaID)
        {
            return IzvrsiTransakcionuProceduru(
                "ObrisiDokumentacijuZahteva",
                new SqlParameter(
                    "@DokumentacijaID",
                    SqlDbType.Int
                )
                {
                    Value = dokumentacijaID
                }
            );
        }

        public bool ObrisiDokumentacijuPoZahtevu(
            int zahtevID)
        {
            return IzvrsiTransakcionuProceduru(
                "ObrisiDokumentacijuZaZahtev",
                new SqlParameter(
                    "@ZahtevID",
                    SqlDbType.Int
                )
                {
                    Value = zahtevID
                }
            );
        }
    }
}
