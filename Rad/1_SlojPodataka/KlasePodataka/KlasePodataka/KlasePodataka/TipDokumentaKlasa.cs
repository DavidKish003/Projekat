using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KlasePodataka
{
     [Table("TipDokumenta")]
     public class TipDokumentaKlasa
     {
          [Key]
          public int TipDokumentaID { get; set; }

          public string Naziv { get; set; }

          public bool Obavezan { get; set; }
     }
}