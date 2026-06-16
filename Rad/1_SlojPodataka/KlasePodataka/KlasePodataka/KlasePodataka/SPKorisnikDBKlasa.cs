using System;
using System.Data;
using System.Data.SqlClient;

namespace KlasePodataka
{
    public class SPKorisnikDBKlasa
    {
        private readonly string connectionString;

        public SPKorisnikDBKlasa(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public KorisnikKlasa ProveriKorisnika(
            string korisnickoIme,
            string lozinka)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand komanda =
                       new SqlCommand(
                           "dbo.ProveriKorisnika",
                           konekcija))
                {
                    komanda.CommandType =
                        CommandType.StoredProcedure;

                    komanda.Parameters.Add(
                        "@KorisnickoIme",
                        SqlDbType.NVarChar,
                        50
                    ).Value = korisnickoIme;

                    komanda.Parameters.Add(
                        "@Lozinka",
                        SqlDbType.NVarChar,
                        100
                    ).Value = lozinka;

                    konekcija.Open();

                    using (SqlDataReader reader =
                           komanda.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new KorisnikKlasa
                        {
                            KorisnikID =
                                Convert.ToInt32(
                                    reader["KorisnikID"]
                                ),

                            KorisnickoIme =
                                reader["KorisnickoIme"]
                                    .ToString(),

                            Uloga =
                                reader["Uloga"]
                                    .ToString()
                        };
                    }
                }
            }
        }

        public KorisnikKlasa ProveriKorisnikaPoKorisnickomImenu(
            string korisnickoIme)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand komanda =
                       new SqlCommand(
                           @"SELECT
                                 KorisnikID,
                                 KorisnickoIme,
                                 Uloga
                             FROM dbo.Korisnici
                             WHERE KorisnickoIme = @KorisnickoIme",
                           konekcija))
                {
                    komanda.CommandType =
                        CommandType.Text;

                    komanda.Parameters.Add(
                        "@KorisnickoIme",
                        SqlDbType.NVarChar,
                        50
                    ).Value = korisnickoIme;

                    konekcija.Open();

                    using (SqlDataReader reader =
                           komanda.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return new KorisnikKlasa
                        {
                            KorisnikID =
                                Convert.ToInt32(
                                    reader["KorisnikID"]
                                ),

                            KorisnickoIme =
                                reader["KorisnickoIme"]
                                    .ToString(),

                            Uloga =
                                reader["Uloga"]
                                    .ToString()
                        };
                    }
                }
            }
        }

        public bool RegistrujKorisnika(
            KorisnikKlasa korisnik)
        {
            using (SqlConnection konekcija =
                   new SqlConnection(connectionString))
            {
                using (SqlCommand komanda =
                       new SqlCommand(
                           "dbo.RegistrujKorisnika",
                           konekcija))
                {
                    komanda.CommandType =
                        CommandType.StoredProcedure;

                    komanda.Parameters.Add(
                        "@KorisnickoIme",
                        SqlDbType.NVarChar,
                        50
                    ).Value = korisnik.KorisnickoIme;

                    komanda.Parameters.Add(
                        "@Lozinka",
                        SqlDbType.NVarChar,
                        100
                    ).Value = korisnik.Lozinka;

                    komanda.Parameters.Add(
                        "@Uloga",
                        SqlDbType.NVarChar,
                        20
                    ).Value =
                        string.IsNullOrWhiteSpace(korisnik.Uloga)
                            ? "Korisnik"
                            : korisnik.Uloga;

                    konekcija.Open();

                    int brojPromenjenihRedova =
                        komanda.ExecuteNonQuery();

                    return brojPromenjenihRedova > 0;
                }
            }
        }
    }
}