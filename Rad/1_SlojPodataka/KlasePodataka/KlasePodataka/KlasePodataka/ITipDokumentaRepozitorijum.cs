using System.Collections.Generic;

namespace KlasePodataka
{
    public interface ITipDokumentaRepozitorijum
    {
        List<TipDokumentaKlasa> DajSve();

        TipDokumentaKlasa DajPoID(
            int tipDokumentaID
        );

        bool Dodaj(
            TipDokumentaKlasa tipDokumenta
        );

        bool Izmeni(
            TipDokumentaKlasa tipDokumenta
        );

        bool Obrisi(
            int tipDokumentaID
        );
    }
}