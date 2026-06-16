using System.Collections.Generic;
using System.Data;

namespace KlasePodataka
{
    public interface IZahtevRepository
    {
        DataSet DajSve();

        DataSet DajSaFilterom(
            string filter
        );

        DataSet DajZaKorisnika(
            int korisnikId
        );

        DataSet DajPoIdSaDokumentacijom(
            int zahtevId
        );

        bool Dodaj(
            ZahtevZaRegistracijuKlasa zahtev,
            List<int> dokumenta
        );

        bool Izmeni(
            ZahtevZaRegistracijuKlasa zahtev,
            List<int> dokumenta
        );

        bool Obrisi(
            int zahtevId
        );

        bool PromeniStatus(
            int zahtevId,
            string statusZahteva
        );
    }
}