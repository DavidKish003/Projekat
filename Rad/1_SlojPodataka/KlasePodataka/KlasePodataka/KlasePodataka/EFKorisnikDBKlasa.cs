using System;
using System.Data.Entity;
using System.Linq;

namespace KlasePodataka
{
    public class EFKorisnikDBKlasa
    {
        public KorisnikKlasa
            DajKorisnikaPoKorisnickomImenuILozinci(
                string korisnickoIme,
                string lozinka)
        {
            if (string.IsNullOrWhiteSpace(
                    korisnickoIme)
                ||
                string.IsNullOrWhiteSpace(
                    lozinka))
            {
                return null;
            }

            string unesenoKorisnickoIme =
                korisnickoIme.Trim();

            using (KonkursiZaPosaoContext db =
                   new KonkursiZaPosaoContext())
            {
                return db.Korisnici
                    .AsNoTracking()
                    .FirstOrDefault(
                        korisnik =>
                            korisnik.KorisnickoIme ==
                            unesenoKorisnickoIme
                            &&
                            korisnik.Lozinka ==
                            lozinka
                    );
            }
        }

        public KorisnikKlasa
            DajKorisnikaPoKorisnickomImenu(
                string korisnickoIme)
        {
            if (string.IsNullOrWhiteSpace(
                    korisnickoIme))
            {
                return null;
            }

            string unesenoKorisnickoIme =
                korisnickoIme.Trim();

            using (KonkursiZaPosaoContext db =
                   new KonkursiZaPosaoContext())
            {
                return db.Korisnici
                    .AsNoTracking()
                    .FirstOrDefault(
                        korisnik =>
                            korisnik.KorisnickoIme ==
                            unesenoKorisnickoIme
                    );
            }
        }

        public bool DodajKorisnika(
            KorisnikKlasa korisnik)
        {
            if (korisnik == null)
            {
                throw new ArgumentNullException(
                    "korisnik"
                );
            }

            using (KonkursiZaPosaoContext db =
                   new KonkursiZaPosaoContext())
            {
                db.Korisnici.Add(
                    korisnik
                );

                return db.SaveChanges() > 0;
            }
        }
    }
}