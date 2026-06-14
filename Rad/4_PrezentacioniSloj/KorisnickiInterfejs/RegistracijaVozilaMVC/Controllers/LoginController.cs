using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using KlasePodataka;

namespace RegistracijaVozilaMVC.Controllers
{
     public class LoginController : Controller
     {
          private string stringKonekcije =
              @"Data Source=.;Initial Catalog=KonkursiZaPosao;Integrated Security=True";

          // GET: Login
          public ActionResult Index()
          {
               return View();
          }

          [HttpPost]
          public ActionResult Index(string korisnickoIme, string lozinka)
          {
               EFKorisnikDBKlasa ef = new EFKorisnikDBKlasa();

               KorisnikKlasa korisnik = ef.DajKorisnikaPoKorisnickomImenuILozinci(korisnickoIme, lozinka);

               if (korisnik != null)
               {
                    Session["Korisnik"] = korisnik.KorisnickoIme;
                    Session["Uloga"] = korisnik.Uloga;

                    return RedirectToAction("Index", "Zahtev");
               }

               ViewBag.Greska = "Pogrešno korisničko ime ili lozinka.";
               return View();
          }

          public ActionResult Logout()
          {
               Session.Clear();

               return RedirectToAction("Index");
          }
     }
}