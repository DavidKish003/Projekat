using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlasePodataka
{
    [Table("Korisnici")]
    public class KorisnikKlasa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KorisnikID
        {
            get;
            set;
        }

        [Required]
        [MaxLength(50)]
        public string KorisnickoIme
        {
            get;
            set;
        }

        [Required]
        [MaxLength(100)]
        public string Lozinka
        {
            get;
            set;
        }

        [Required]
        [MaxLength(20)]
        public string Uloga
        {
            get;
            set;
        }
    }
}