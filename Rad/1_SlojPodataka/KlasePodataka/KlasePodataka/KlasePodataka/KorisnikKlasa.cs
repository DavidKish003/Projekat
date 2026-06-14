using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlasePodataka
{
     [Table("Korisnici")]
     public class KorisnikKlasa : OsobaKlasa
     {
          [Key]
          public int KorisnikID { get; set; }

          public string KorisnickoIme { get; set; }

          [Column("Lozinka")]
          public string Sifra { get; set; }

          [NotMapped]
          public string Status { get; set; }

          public string Uloga { get; set; }
     }
}