using System;
using System.ComponentModel.DataAnnotations;

namespace KlasePodataka
{
    public class ZahtevZaRegistracijuKlasa
    {
        public int ZahtevID { get; set; }

        public int KorisnikID { get; set; }

        [Required(
            ErrorMessage = "Unesite ime i prezime.")]
        public string ImePrezime { get; set; }

        [Required(
            ErrorMessage = "Unesite e-mail adresu.")]
        [EmailAddress(
            ErrorMessage = "Unesite ispravnu e-mail adresu.")]
        public string Email { get; set; }

        [Required(
            ErrorMessage = "Unesite kontakt telefon.")]
        public string KontaktTelefon { get; set; }

        [Required(
            ErrorMessage = "Unesite poslednju završenu školu.")]
        public string PoslednjaSkola { get; set; }

        public string MestoSkole { get; set; }

        [Required(
            ErrorMessage = "Unesite stečeno zanimanje.")]
        public string Zanimanje { get; set; }

        [Required(
            ErrorMessage = "Izaberite stepen obrazovanja.")]
        public string StepenObrazovanja { get; set; }

        [Required(
            ErrorMessage = "Unesite naziv konkursa.")]
        public string NazivKonkursa { get; set; }

        [Required(
            ErrorMessage = "Izaberite radno mesto.")]
        public string RadnoMesto { get; set; }

        [Range(
            0,
            50,
            ErrorMessage = "Godine iskustva moraju biti između 0 i 50.")]
        public int GodineIskustva { get; set; }

        public string MotivacionoPismo { get; set; }

        public DateTime DatumPodnosenja { get; set; }

        [Required(
            ErrorMessage = "Unesite rok konkursa.")]
        [DataType(DataType.Date)]
        public DateTime RokKonkursa { get; set; }

        public string StatusZahteva { get; set; }

        public bool IspunjavaOsnovneUslove { get; set; }
    }
}