using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using KlasePodataka;
using PrezentacionaLogika;

namespace RegistracijaVozilaMVC.Controllers
{
    public class ZahtevController : Controller
    {
        private readonly string connectionString =
            @"Data Source=.;Initial Catalog=KonkursiZaPosao;Integrated Security=True";

        protected override void OnActionExecuting(
            ActionExecutingContext filterContext)
        {
            if (Session["Korisnik"] == null)
            {
                filterContext.Result =
                    RedirectToAction("Index", "Login");

                return;
            }

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Index(
            string pretraga,
            string status)
        {
            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            DataSet dataSet;

            if (string.IsNullOrWhiteSpace(pretraga))
            {
                dataSet =
                    db.DajSveZahteveZaRegistraciju();
            }
            else
            {
                dataSet =
                    db.DajZahteveZaRegistracijuSaFilterom(
                        pretraga
                    );
            }

            if (dataSet.Tables.Count == 0)
            {
                return View(new DataTable());
            }

            DataTable tabela = dataSet.Tables[0];

            if (!string.IsNullOrWhiteSpace(status))
            {
                DataView pogled = tabela.DefaultView;

                string bezbedanStatus =
                    status.Replace("'", "''");

                pogled.RowFilter =
                    "StatusZahteva = '" +
                    bezbedanStatus +
                    "'";

                tabela = pogled.ToTable();
            }

            return View(tabela);
        }

        public ActionResult Detalji(int id)
        {
            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            DataSet dataSet =
                db.DajZahtevZaRegistracijuSaDokumentacijom(
                    id
                );

            if (dataSet.Tables.Count == 0 ||
                dataSet.Tables[0].Rows.Count == 0)
            {
                return HttpNotFound();
            }

            DataTable tabela = dataSet.Tables[0];
            DataRow redZahteva = tabela.Rows[0];

            ZahtevViewModel viewModel =
                KreirajZahtevViewModel(redZahteva);

            foreach (DataRow red in tabela.Rows)
            {
                if (red["DokumentacijaID"] == DBNull.Value)
                {
                    continue;
                }

                DokumentacijaViewModel dokument =
                    new DokumentacijaViewModel
                    {
                        DokumentacijaID =
                            Convert.ToInt32(
                                red["DokumentacijaID"]
                            ),

                        NazivDokumenta =
                            red["NazivDokumenta"]
                                .ToString(),

                        Obavezan =
                            Convert.ToBoolean(
                                red["Obavezan"]
                            ),

                        Dostavljen =
                            Convert.ToBoolean(
                                red["Dostavljen"]
                            ),

                        Napomena =
                            red["Napomena"]
                                .ToString()
                    };

                viewModel.Dokumentacija.Add(dokument);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Dodaj()
        {
            ZahtevZaRegistracijuKlasa zahtev =
                new ZahtevZaRegistracijuKlasa
                {
                    DatumPodnosenja = DateTime.Today,
                    GodineIskustva = 0
                };

            return View(zahtev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dodaj(
            ZahtevZaRegistracijuKlasa zahtev,
            int[] Dokumenta)
        {
            if (!ModelState.IsValid)
            {
                return View(zahtev);
            }

            List<int> dokumenta =
                PretvoriDokumentaUListu(Dokumenta);

            bool cvJeDostavljen =
                dokumenta.Contains(1);

            PoslovnaLogika.ZahtevPoslovnaLogika poslovnaLogika =
                new PoslovnaLogika.ZahtevPoslovnaLogika();

            zahtev.DatumPodnosenja =
                DateTime.Now;

            int minimalneGodineIskustva = 1;

            zahtev.IspunjavaOsnovneUslove =
                poslovnaLogika.ProveriDaLiKandidatIspunjavaUslove(
                    zahtev.RokKonkursa,
                    cvJeDostavljen,
                    zahtev.GodineIskustva,
                    minimalneGodineIskustva
                );

            zahtev.StatusZahteva =
                poslovnaLogika.OdrediPocetniStatus(
                    zahtev.RokKonkursa,
                    cvJeDostavljen,
                    zahtev.GodineIskustva,
                    minimalneGodineIskustva
                );

            zahtev.StatusZahteva =
                zahtev.IspunjavaOsnovneUslove
                    ? "Primljena"
                    : "Odbijena";

            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            db.SnimiNoviZahtevSaDokumentacijom(
                zahtev,
                dokumenta
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Izmeni(int id)
        {
            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            DataSet dataSet =
                db.DajZahtevZaRegistracijuSaDokumentacijom(
                    id
                );

            if (dataSet.Tables.Count == 0 ||
                dataSet.Tables[0].Rows.Count == 0)
            {
                return HttpNotFound();
            }

            return View(dataSet.Tables[0]);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Izmeni(
            ZahtevZaRegistracijuKlasa zahtev,
            int[] Dokumenta)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(
                    "Izmeni",
                    new { id = zahtev.ZahtevID }
                );
            }

            List<int> dokumenta =
                PretvoriDokumentaUListu(Dokumenta);

            bool cvJeDostavljen =
                dokumenta.Contains(1);

            PoslovnaLogika.ZahtevPoslovnaLogika poslovnaLogika =
      new PoslovnaLogika.ZahtevPoslovnaLogika();

            int minimalneGodineIskustva = 1;

            zahtev.IspunjavaOsnovneUslove =
                poslovnaLogika.ProveriDaLiKandidatIspunjavaUslove(
                    zahtev.RokKonkursa,
                    cvJeDostavljen,
                    zahtev.GodineIskustva,
                    minimalneGodineIskustva
                );

            if (string.IsNullOrWhiteSpace(
                    zahtev.StatusZahteva))
            {
                zahtev.StatusZahteva =
                    zahtev.IspunjavaOsnovneUslove
                        ? "U razmatranju"
                        : "Odbijena";
            }

            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            db.IzmeniZahtevSaDokumentacijom(
                zahtev,
                dokumenta
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Obrisi(int id)
        {
            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

                db.ObrisiZahtevZaRegistraciju(id);


            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Prihvati(int id)
        {
            PromeniStatus(
                id,
                "Izabran"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Odbij(int id)
        {
            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(connectionString);

            db.PromeniStatusZahteva(
                id,
                "Odbijen"
            );

            return RedirectToAction("Index");
        }

        private void PromeniStatus(
            int zahtevId,
            string noviStatus)
        {
            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            db.PromeniStatusZahteva(
                zahtevId,
                noviStatus
            );
        }

        private List<int> PretvoriDokumentaUListu(
            int[] dokumenta)
        {
            if (dokumenta == null)
            {
                return new List<int>();
            }

            return new List<int>(dokumenta);
        }

        private ZahtevViewModel KreirajZahtevViewModel(
            DataRow red)
        {
            return new ZahtevViewModel
            {
                ZahtevID =
                    Convert.ToInt32(
                        red["ZahtevID"]
                    ),

                ImePrezime =
                    red["ImePrezime"]
                        .ToString(),

                Email =
                    red["Email"]
                        .ToString(),

                KontaktTelefon =
                    red["KontaktTelefon"]
                        .ToString(),

                NazivKonkursa =
                    red["NazivKonkursa"]
                        .ToString(),

                StepenObrazovanja =
                    red["StepenObrazovanja"]
                        .ToString(),

                GodineIskustva =
                    Convert.ToInt32(
                        red["GodineIskustva"]
                    ),

                MotivacionoPismo =
                    red["MotivacionoPismo"] == DBNull.Value
                        ? string.Empty
                        : red["MotivacionoPismo"]
                            .ToString(),

                DatumPodnosenja =
                    Convert.ToDateTime(
                        red["DatumPodnosenja"]
                    ),

                RokKonkursa =
                    Convert.ToDateTime(
                        red["RokKonkursa"]
                    ),

                StatusZahteva =
                    red["StatusZahteva"]
                        .ToString(),

                IspunjavaOsnovneUslove =
                    Convert.ToBoolean(
                        red["IspunjavaOsnovneUslove"]
                    ),

                Dokumentacija =
                    new List<DokumentacijaViewModel>()
            };
        }
    }
}