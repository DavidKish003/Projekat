using System;
using System.Collections.Generic;
using System.Data;
using DBUtils;

namespace KlasePodataka
{
    public class TabelaTipDokumentaRepozitoriju :
        TabelaKlasa,
        ITipDokumentaRepozitorijum
    {
        public TabelaTipDokumentaRepozitoriju(
            KonekcijaKlasa konekcija)
            : base(
                konekcija,
                "TipDokumenta"
            )
        {
        }

        public List<TipDokumentaKlasa> DajSve()
        {
            string upit =
                @"SELECT
                    TipDokumentaID,
                    Naziv,
                    Obavezan
                  FROM dbo.TipDokumenta
                  ORDER BY TipDokumentaID";

            DataSet skupPodataka =
                DajPodatke(upit);

            return PretvoriUListu(
                skupPodataka
            );
        }

        public TipDokumentaKlasa DajPoID(
            int tipDokumentaID)
        {
            string upit =
                @"SELECT
                    TipDokumentaID,
                    Naziv,
                    Obavezan
                  FROM dbo.TipDokumenta
                  WHERE TipDokumentaID = " +
                tipDokumentaID;

            DataSet skupPodataka =
                DajPodatke(upit);

            List<TipDokumentaKlasa> lista =
                PretvoriUListu(
                    skupPodataka
                );

            if (lista.Count == 0)
            {
                return null;
            }

            return lista[0];
        }

        public bool Dodaj(
            TipDokumentaKlasa tipDokumenta)
        {
            if (tipDokumenta == null)
            {
                throw new ArgumentNullException(
                    "tipDokumenta"
                );
            }

            if (string.IsNullOrWhiteSpace(
                tipDokumenta.Naziv))
            {
                throw new ArgumentException(
                    "Naziv tipa dokumenta je obavezan."
                );
            }

            string bezbedanNaziv =
                PripremiTekstZaUpit(
                    tipDokumenta.Naziv
                );

            int obavezan =
                tipDokumenta.Obavezan
                    ? 1
                    : 0;

            string upit =
                @"INSERT INTO dbo.TipDokumenta
                  (
                      Naziv,
                      Obavezan
                  )
                  VALUES
                  (
                      N'" + bezbedanNaziv + @"',
                      " + obavezan + @"
                  )";

            return IzvrsiAzuriranje(
                upit
            );
        }

        public bool Izmeni(
            TipDokumentaKlasa tipDokumenta)
        {
            if (tipDokumenta == null)
            {
                throw new ArgumentNullException(
                    "tipDokumenta"
                );
            }

            if (tipDokumenta.TipDokumentaID <= 0)
            {
                throw new ArgumentException(
                    "TipDokumentaID mora biti veći od nule."
                );
            }

            if (string.IsNullOrWhiteSpace(
                tipDokumenta.Naziv))
            {
                throw new ArgumentException(
                    "Naziv tipa dokumenta je obavezan."
                );
            }

            string bezbedanNaziv =
                PripremiTekstZaUpit(
                    tipDokumenta.Naziv
                );

            int obavezan =
                tipDokumenta.Obavezan
                    ? 1
                    : 0;

            string upit =
                @"UPDATE dbo.TipDokumenta
                  SET
                      Naziv = N'" + bezbedanNaziv + @"',
                      Obavezan = " + obavezan + @"
                  WHERE TipDokumentaID = " +
                tipDokumenta.TipDokumentaID;

            return IzvrsiAzuriranje(
                upit
            );
        }

        public bool Obrisi(
            int tipDokumentaID)
        {
            if (tipDokumentaID <= 0)
            {
                return false;
            }

            string upit =
                @"DELETE FROM dbo.TipDokumenta
                  WHERE TipDokumentaID = " +
                tipDokumentaID;

            return IzvrsiAzuriranje(
                upit
            );
        }

        private List<TipDokumentaKlasa>
            PretvoriUListu(
                DataSet skupPodataka)
        {
            List<TipDokumentaKlasa> lista =
                new List<TipDokumentaKlasa>();

            if (skupPodataka == null ||
                skupPodataka.Tables.Count == 0)
            {
                return lista;
            }

            DataTable tabela =
                skupPodataka.Tables[0];

            foreach (DataRow red in tabela.Rows)
            {
                TipDokumentaKlasa tipDokumenta =
                    new TipDokumentaKlasa
                    {
                        TipDokumentaID =
                            Convert.ToInt32(
                                red["TipDokumentaID"]
                            ),

                        Naziv =
                            Convert.ToString(
                                red["Naziv"]
                            ),

                        Obavezan =
                            Convert.ToBoolean(
                                red["Obavezan"]
                            )
                    };

                lista.Add(
                    tipDokumenta
                );
            }

            return lista;
        }

        private string PripremiTekstZaUpit(
            string tekst)
        {
            if (tekst == null)
            {
                return string.Empty;
            }

            return tekst
                .Trim()
                .Replace(
                    "'",
                    "''"
                );
        }
    }
}