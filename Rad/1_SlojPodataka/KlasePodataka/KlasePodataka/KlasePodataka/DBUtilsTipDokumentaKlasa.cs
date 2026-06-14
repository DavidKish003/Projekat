using System.Data;
using DBUtils;

namespace KlasePodataka
{
     public class DBUtilsTipDokumentaKlasa : TabelaKlasa
     {
          public DBUtilsTipDokumentaKlasa(KonekcijaKlasa konekcija)
               : base(konekcija, "TipDokumenta")
          {
          }

          public DataSet DajSveTipoveDokumenata()
          {
               string upit = "SELECT * FROM TipDokumenta";
               return DajPodatke(upit);
          }

          public DataSet DajObavezneTipoveDokumenata()
          {
               string upit = "SELECT * FROM TipDokumenta WHERE Obavezan = 1";
               return DajPodatke(upit);
          }
     }
}