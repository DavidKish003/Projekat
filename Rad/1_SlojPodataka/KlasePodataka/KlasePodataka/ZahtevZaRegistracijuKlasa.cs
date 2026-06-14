using System;

namespace KlasePodataka
{
    public class ZahtevZaRegistracijuKlasa
    {
        public int ZahtevID { get; set; }

        public string ImePrezime { get; set; }

        public string Email { get; set; }

        public string KontaktTelefon { get; set; }

        public string NazivKonkursa { get; set; }

        public string StepenObrazovanja { get; set; }

        public int GodineIskustva { get; set; }

        public string MotivacionoPismo { get; set; }

        public DateTime DatumPodnosenja { get; set; }

        public DateTime RokKonkursa { get; set; }

        public string StatusZahteva { get; set; }

        public bool IspunjavaOsnovneUslove { get; set; }
    }
}