using System;

namespace PoslovnaLogika
{
    public class ZahtevPoslovnaLogika
    {
        public bool ProveriDaLiKandidatIspunjavaUslove(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            bool prijavaJeUroku =
                datumPodnosenja.Date <=
                rokKonkursa.Date;

            if (!prijavaJeUroku)
            {
                return false;
            }

            WSKadrovskiPodaci
                .OgranicenjaZaposljavanja servis =
                    new WSKadrovskiPodaci
                        .OgranicenjaZaposljavanja();

            servis.Url =
                "http://localhost:1718/" +
                "OgranicenjaZaposljavanja.asmx";

            return servis
                .DaLiZanimanjeOdgovaraRadnomMestu(
                    zanimanje,
                    radnoMesto
                );
        }

        public string OdrediPocetniStatus(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            if (datumPodnosenja.Date >
                rokKonkursa.Date)
            {
                return "Odbijena";
            }

            bool zanimanjeOdgovara =
                ProveriDaLiKandidatIspunjavaUslove(
                    datumPodnosenja,
                    rokKonkursa,
                    zanimanje,
                    radnoMesto
                );

            if (zanimanjeOdgovara)
            {
                return "U razmatranju";
            }

            return "Primljena";
        }
    }
}
