using System;

namespace PoslovnaLogika
{
    public class ZahtevPoslovnaLogika
    {
        public bool ProveriDaLiKandidatIspunjavaUslove(
            DateTime rokKonkursa,
            bool cvDostavljen,
            int godineIskustva,
            int minimalneGodineIskustva)
        {
            bool konkursJeAktivan =
                rokKonkursa.Date >= DateTime.Today;

            bool imaDovoljnoIskustva =
                godineIskustva >= minimalneGodineIskustva;

            return konkursJeAktivan
                && cvDostavljen
                && imaDovoljnoIskustva;
        }

        public string OdrediPocetniStatus(
            DateTime rokKonkursa,
            bool cvDostavljen,
            int godineIskustva,
            int minimalneGodineIskustva)
        {
            bool ispunjavaUslove =
                ProveriDaLiKandidatIspunjavaUslove(
                    rokKonkursa,
                    cvDostavljen,
                    godineIskustva,
                    minimalneGodineIskustva
                );

            return ispunjavaUslove
                ? "Primljena"
                : "Odbijena";
        }
    }
}