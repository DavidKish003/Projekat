using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlasePodataka
{
     public class DokumentacijaZahtevaKlasa
     {
          public int DokumentacijaID { get; set; }
          public int ZahtevID { get; set; }
          public int TipDokumentaID { get; set; }
          public bool Dostavljen { get; set; }
          public string Napomena { get; set; }
     }
}
