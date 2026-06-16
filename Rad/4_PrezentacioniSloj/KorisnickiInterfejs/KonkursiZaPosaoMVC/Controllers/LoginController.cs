using System;
using System.Web.Mvc;
using KlasePodataka;

namespace KonkursiZaPosaoMVC.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(
            string korisnickoIme,
            string lozinka)
        {
            EFKorisnikDBKlasa efKorisnikDb =
                new EFKorisnikDBKlasa();

            KorisnikKlasa korisnik =
                efKorisnikDb
                    .DajKorisnikaPoKorisnickomImenuILozinci(
                        korisnickoIme,
                        lozinka
                    );

            if (korisnik == null)
            {
                ViewBag.Greska =
                    "Pogrešno korisničko ime ili lozinka.";

                return View();
            }

            Session["KorisnikID"] =
                korisnik.KorisnikID;

            Session["Korisnik"] =
                korisnik.KorisnickoIme;

            Session["Uloga"] =
                korisnik.Uloga;

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        [HttpGet]
        public ActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registracija(
            string korisnickoIme,
            string lozinka,
            string potvrdaLozinke)
        {
            korisnickoIme =
                korisnickoIme == null
                    ? string.Empty
                    : korisnickoIme.Trim();

            if (string.IsNullOrWhiteSpace(
                    korisnickoIme)
                ||
                string.IsNullOrWhiteSpace(
                    lozinka)
                ||
                string.IsNullOrWhiteSpace(
                    potvrdaLozinke))
            {
                ViewBag.Greska =
                    "Sva polja su obavezna.";

                return View();
            }

            if (korisnickoIme.Length < 3)
            {
                ViewBag.Greska =
                    "Korisničko ime mora imati najmanje 3 karaktera.";

                return View();
            }

            if (lozinka.Length < 4)
            {
                ViewBag.Greska =
                    "Lozinka mora imati najmanje 4 karaktera.";

                return View();
            }

            if (lozinka != potvrdaLozinke)
            {
                ViewBag.Greska =
                    "Lozinka i potvrda lozinke se ne podudaraju.";

                return View();
            }

            EFKorisnikDBKlasa efKorisnikDb =
                new EFKorisnikDBKlasa();

            KorisnikKlasa postojeciKorisnik =
                efKorisnikDb
                    .DajKorisnikaPoKorisnickomImenu(
                        korisnickoIme
                    );

            if (postojeciKorisnik != null)
            {
                ViewBag.Greska =
                    "Korisničko ime je već zauzeto.";

                return View();
            }

            KorisnikKlasa noviKorisnik =
                new KorisnikKlasa
                {
                    KorisnickoIme =
                        korisnickoIme,

                    Lozinka =
                        lozinka,

                    Uloga =
                        "Korisnik"
                };

            bool uspesno =
                efKorisnikDb.DodajKorisnika(
                    noviKorisnik
                );

            if (!uspesno)
            {
                ViewBag.Greska =
                    "Registracija nije uspela.";

                return View();
            }

            TempData["Poruka"] =
                "Registracija je uspešna. Sada se možete prijaviti.";

            return RedirectToAction(
                "Index"
            );
        }

        public ActionResult Odjava()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }
}