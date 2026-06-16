using System;
using System.Collections.Generic;

namespace PrezentacionaLogika
{
    public class ZahtevViewModel
    {
        public int ZahtevID { get; set; }

    public string ImePrezime { get; set; }

        public string Email { get; set; }

        public string KontaktTelefon { get; set; }

        public string PoslednjaSkola { get; set; }

        public string MestoSkole { get; set; }

        public string Zanimanje { get; set; }

        public string NazivKonkursa { get; set; }

        public string RadnoMesto { get; set; }

        public string StepenObrazovanja { get; set; }

        public int GodineIskustva { get; set; }

        public string MotivacionoPismo { get; set; }

        public DateTime DatumPodnosenja { get; set; }

        public DateTime RokKonkursa { get; set; }

        public string StatusZahteva { get; set; }

        public bool IspunjavaOsnovneUslove { get; set; }

        public List<DokumentacijaViewModel> Dokumentacija
        {
            get;
            set;
        }
    }


}
