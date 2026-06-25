using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Configuration;
using System.Web.Hosting;
using KlasePodataka;
using PrezentacionaLogika;
using Datoteka = System.IO.File;

namespace KonkursiZaPosaoMVC.Controllers
{
    public class ZahtevController : Controller
    {
        private readonly string stringKonekcije;

        public ZahtevController()
        {
            stringKonekcije =
                DajStringKonekcije();
        }

        private string DajStringKonekcije()
        {
            ConnectionStringSettings podesavanjeKonekcije =
                ConfigurationManager.ConnectionStrings["Konekcija"];

            if (podesavanjeKonekcije == null ||
                string.IsNullOrWhiteSpace(podesavanjeKonekcije.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "U Web.config nije podešen connection string sa imenom Konekcija."
                );
            }

            return podesavanjeKonekcije.ConnectionString;
        }

        private string DajPutanjuDoOgranicenja()
        {
            string putanja =
                ConfigurationManager.AppSettings["PutanjaDoOgranicenjaSistematizacije"];

            if (string.IsNullOrWhiteSpace(putanja))
            {
                throw new ConfigurationErrorsException(
                    "U Web.config nije podešena vrednost PutanjaDoOgranicenjaSistematizacije."
                );
            }

            if (Path.IsPathRooted(putanja))
            {
                return putanja;
            }

            if (putanja.StartsWith("~"))
            {
                string mapiranaPutanja =
                    HostingEnvironment.MapPath(putanja);

                if (string.IsNullOrWhiteSpace(mapiranaPutanja))
                {
                    throw new ConfigurationErrorsException(
                        "Nije moguće mapirati virtuelnu putanju: " +
                        putanja
                    );
                }

                return mapiranaPutanja;
            }

            string korenAplikacije =
                HostingEnvironment.ApplicationPhysicalPath;

            if (string.IsNullOrWhiteSpace(korenAplikacije))
            {
                korenAplikacije =
                    AppDomain.CurrentDomain.BaseDirectory;
            }

            return Path.GetFullPath(
                Path.Combine(
                    korenAplikacije,
                    putanja
                )
            );
        }

        private class UskladjenostZanimanja
        {
            public string Zanimanje { get; set; }

            public string RadnoMesto { get; set; }
        }

        private bool KorisnikJePrijavljen()
        {
            return Session["KorisnikID"] != null;
        }

        private bool KorisnikJeAdministrator()
        {
            string uloga =
                Convert.ToString(
                    Session["Uloga"]
                );

            return
                string.Equals(
                    uloga,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    uloga,
                    "Administrator",
                    StringComparison.OrdinalIgnoreCase
                );
        }

        [ActionName("Index")]
        public ActionResult PrikaziSvePrijave(
            string pretraga,
            string status)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (!KorisnikJeAdministrator())
            {
                return RedirectToAction(
                    "MojePrijave",
                    "Zahtev"
                );
            }

            SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                new SPZahtevZaRegistracijuDBKlasa(
                    stringKonekcije
                );

            DataSet skupPodataka;

            if (string.IsNullOrWhiteSpace(pretraga))
            {
                skupPodataka =
                    bazaPodataka.DajSveZahteveZaRegistraciju();
            }
            else
            {
                skupPodataka =
                    bazaPodataka.DajZahteveZaRegistracijuSaFilterom(
                        pretraga
                    );
            }

            ViewBag.Pretraga =
                pretraga;

            ViewBag.Status =
                status;

            if (skupPodataka.Tables.Count == 0)
            {
                return View(
                    new DataTable()
                );
            }

            DataTable tabela =
                skupPodataka.Tables[0];

            if (!string.IsNullOrWhiteSpace(status))
            {
                DataView pogled =
                    tabela.DefaultView;

                string bezbedanStatus =
                    status.Replace(
                        "'",
                        "''"
                    );

                pogled.RowFilter =
                    "StatusZahteva = '" +
                    bezbedanStatus +
                    "'";

                tabela =
                    pogled.ToTable();
            }

            return View(tabela);
        }

        [HttpGet]
        [ActionName("MojePrijave")]
        public ActionResult PrikaziMojePrijave()
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (KorisnikJeAdministrator())
            {
                return RedirectToAction(
                    "Index",
                    "Zahtev"
                );
            }

            int korisnikId =
                Convert.ToInt32(
                    Session["KorisnikID"]
                );

            SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                new SPZahtevZaRegistracijuDBKlasa(
                    stringKonekcije
                );

            DataSet skupPodataka =
                bazaPodataka.DajPrijaveKorisnika(
                    korisnikId
                );

            if (skupPodataka.Tables.Count == 0)
            {
                return View(
                    new DataTable()
                );
            }

            return View(
                skupPodataka.Tables[0]
            );
        }

        [HttpGet]
        [ActionName("Detalji")]
        public ActionResult PrikaziDetaljePrijave(int id)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                new SPZahtevZaRegistracijuDBKlasa(
                    stringKonekcije
                );

            DataSet skupPodataka =
                bazaPodataka.DajZahtevZaRegistracijuSaDokumentacijom(
                    id
                );

            if (skupPodataka.Tables.Count == 0 ||
                skupPodataka.Tables[0].Rows.Count == 0)
            {
                return HttpNotFound();
            }

            DataTable tabela =
                skupPodataka.Tables[0];

            DataRow redZahteva =
                tabela.Rows[0];

            if (!KorisnikJeAdministrator())
            {
                if (!tabela.Columns.Contains("KorisnikID") ||
                    redZahteva["KorisnikID"] == DBNull.Value)
                {
                    return new HttpStatusCodeResult(
                        403,
                        "Nemate pravo pristupa ovoj prijavi."
                    );
                }

                int prijavljeniKorisnikId =
                    Convert.ToInt32(
                        Session["KorisnikID"]
                    );

                int vlasnikPrijaveId =
                    Convert.ToInt32(
                        redZahteva["KorisnikID"]
                    );

                if (prijavljeniKorisnikId != vlasnikPrijaveId)
                {
                    return new HttpStatusCodeResult(
                        403,
                        "Nemate pravo pristupa ovoj prijavi."
                    );
                }
            }

            ZahtevViewModel modelZaPrikaz =
                KreirajModelZaPrikazZahteva(
                    redZahteva
                );

            foreach (DataRow red in tabela.Rows)
            {
                if (!tabela.Columns.Contains("DokumentacijaID") ||
                    red["DokumentacijaID"] == DBNull.Value)
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
                            Convert.ToString(
                                red["NazivDokumenta"]
                            ),

                        Obavezan =
                            red["Obavezan"] != DBNull.Value
                            &&
                            Convert.ToBoolean(
                                red["Obavezan"]
                            ),

                        Dostavljen =
                            red["Dostavljen"] != DBNull.Value
                            &&
                            Convert.ToBoolean(
                                red["Dostavljen"]
                            ),

                        Napomena =
                            red["Napomena"] == DBNull.Value
                                ? string.Empty
                                : Convert.ToString(
                                    red["Napomena"]
                                )
                    };

                modelZaPrikaz.Dokumentacija.Add(
                    dokument
                );
            }

            return View(modelZaPrikaz);
        }

        [HttpGet]
        [ActionName("Dodaj")]
        public ActionResult PrikaziFormuZaDodavanje()
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            PopuniListeIzXml();

            ViewBag.IzabranaDokumenta =
                new List<int>();

            ZahtevZaRegistracijuKlasa zahtev =
                new ZahtevZaRegistracijuKlasa
                {
                    DatumPodnosenja =
                        DateTime.Today,

                    RokKonkursa =
                        DateTime.Today.AddDays(7),

                    GodineIskustva =
                        0
                };

            return View(zahtev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Dodaj")]
        public ActionResult SacuvajNovuPrijavu(
            ZahtevZaRegistracijuKlasa zahtev,
            int[] Dokumenta)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            PopuniListeIzXml();

            if (zahtev == null)
            {
                zahtev =
                    new ZahtevZaRegistracijuKlasa();
            }

            zahtev.KorisnikID =
                Convert.ToInt32(
                    Session["KorisnikID"]
                );

            List<int> dokumenta =
                PretvoriDokumentaUListu(
                    Dokumenta
                );

            ViewBag.IzabranaDokumenta =
                dokumenta;

            if (!dokumenta.Contains(1))
            {
                ModelState.AddModelError(
                    "",
                    "Biografija (CV) mora biti označena kao dostavljena dokumentacija."
                );
            }

            if (!dokumenta.Contains(3))
            {
                ModelState.AddModelError(
                    "",
                    "Diploma mora biti označena kao dostavljena dokumentacija."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(zahtev);
            }

            try
            {
                PoslovnaLogika.ZahtevPoslovnaLogika poslovnaLogika =
                    new PoslovnaLogika.ZahtevPoslovnaLogika();

                zahtev.DatumPodnosenja =
                    DateTime.Today;

                zahtev.IspunjavaOsnovneUslove =
                    poslovnaLogika.ProveriDaLiKandidatIspunjavaUslove(
                        zahtev.DatumPodnosenja,
                        zahtev.RokKonkursa,
                        zahtev.Zanimanje,
                        zahtev.RadnoMesto
                    );

                zahtev.StatusZahteva =
                    poslovnaLogika.OdrediPocetniStatus(
                        zahtev.DatumPodnosenja,
                        zahtev.RokKonkursa,
                        zahtev.Zanimanje,
                        zahtev.RadnoMesto
                    );

                SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                    new SPZahtevZaRegistracijuDBKlasa(
                        stringKonekcije
                    );

                bazaPodataka.SnimiNoviZahtevSaDokumentacijom(
                    zahtev,
                    dokumenta
                );

                TempData["Poruka"] =
                    "Prijava kandidata i dokumentacija su uspešno sačuvani.";

                if (KorisnikJeAdministrator())
                {
                    return RedirectToAction(
                        "Index",
                        "Zahtev"
                    );
                }

                return RedirectToAction(
                    "MojePrijave",
                    "Zahtev"
                );
            }
            catch (Exception ex)
            {
                string detaljnaGreska =
                    ex.InnerException != null
                        ? ex.InnerException.Message
                        : ex.Message;

                ModelState.AddModelError(
                    "",
                    "Greška prilikom čuvanja prijave i dokumentacije: " +
                    detaljnaGreska
                );

                return View(zahtev);
            }
        }

        [HttpGet]
        [ActionName("Izmeni")]
        public ActionResult PrikaziFormuZaIzmenu(int id)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (!KorisnikJeAdministrator())
            {
                return new HttpStatusCodeResult(
                    403,
                    "Samo administrator može da menja prijave."
                );
            }

            PopuniListeIzXml();

            SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                new SPZahtevZaRegistracijuDBKlasa(
                    stringKonekcije
                );

            DataSet skupPodataka =
                bazaPodataka.DajZahtevZaRegistracijuSaDokumentacijom(
                    id
                );

            if (skupPodataka.Tables.Count == 0 ||
                skupPodataka.Tables[0].Rows.Count == 0)
            {
                return HttpNotFound();
            }

            DataTable tabela =
                skupPodataka.Tables[0];

            DataRow redZahteva =
                tabela.Rows[0];

            ZahtevZaRegistracijuKlasa zahtev =
                KreirajZahtevZaIzmenu(
                    redZahteva
                );

            ViewBag.IzabranaDokumenta =
                DajIzabraneDokumente(
                    tabela
                );

            return View(zahtev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Izmeni")]
        public ActionResult SacuvajIzmenuPrijave(
            ZahtevZaRegistracijuKlasa zahtev,
            int[] Dokumenta)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (!KorisnikJeAdministrator())
            {
                return new HttpStatusCodeResult(
                    403,
                    "Samo administrator može da menja prijave."
                );
            }

            PopuniListeIzXml();

            List<int> dokumenta =
                PretvoriDokumentaUListu(
                    Dokumenta
                );

            ViewBag.IzabranaDokumenta =
                dokumenta;

            if (!dokumenta.Contains(1))
            {
                ModelState.AddModelError(
                    "",
                    "Biografija (CV) mora biti označena kao dostavljena dokumentacija."
                );
            }

            if (!dokumenta.Contains(3))
            {
                ModelState.AddModelError(
                    "",
                    "Diploma mora biti označena kao dostavljena dokumentacija."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(zahtev);
            }

            try
            {
                PoslovnaLogika.ZahtevPoslovnaLogika poslovnaLogika =
                    new PoslovnaLogika.ZahtevPoslovnaLogika();

                zahtev.IspunjavaOsnovneUslove =
                    poslovnaLogika.ProveriDaLiKandidatIspunjavaUslove(
                        zahtev.DatumPodnosenja,
                        zahtev.RokKonkursa,
                        zahtev.Zanimanje,
                        zahtev.RadnoMesto
                    );

                zahtev.StatusZahteva =
                    poslovnaLogika.OdrediPocetniStatus(
                        zahtev.DatumPodnosenja,
                        zahtev.RokKonkursa,
                        zahtev.Zanimanje,
                        zahtev.RadnoMesto
                    );

                SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                    new SPZahtevZaRegistracijuDBKlasa(
                        stringKonekcije
                    );

                bazaPodataka.IzmeniZahtevSaDokumentacijom(
                    zahtev,
                    dokumenta
                );

                TempData["Poruka"] =
                    "Prijava kandidata i dokumentacija su uspešno izmenjeni.";

                return RedirectToAction(
                    "Index"
                );
            }
            catch (Exception ex)
            {
                string detaljnaGreska =
                    ex.InnerException != null
                        ? ex.InnerException.Message
                        : ex.Message;

                ModelState.AddModelError(
                    "",
                    "Greška prilikom izmene prijave i dokumentacije: " +
                    detaljnaGreska
                );

                return View(zahtev);
            }
        }

        [HttpPost]
        [ActionName("Obrisi")]
        public ActionResult ObrisiPrijavu(int id)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (!KorisnikJeAdministrator())
            {
                return new HttpStatusCodeResult(
                    403,
                    "Samo administrator može da briše prijave."
                );
            }

            SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                new SPZahtevZaRegistracijuDBKlasa(
                    stringKonekcije
                );

            bazaPodataka.ObrisiZahtevZaRegistraciju(
                id
            );

            TempData["Poruka"] =
                "Prijava je uspešno obrisana.";

            return RedirectToAction(
                "Index"
            );
        }

        [HttpPost]
        [ActionName("Prihvati")]
        public ActionResult PrihvatiKandidata(int id)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (!KorisnikJeAdministrator())
            {
                return new HttpStatusCodeResult(
                    403,
                    "Samo administrator može da izabere kandidata."
                );
            }

            PromeniStatus(
                id,
                "Izabran"
            );

            TempData["Poruka"] =
                "Kandidat je uspešno izabran.";

            return RedirectToAction(
                "Index"
            );
        }

        [HttpPost]
        [ActionName("Odbij")]
        public ActionResult OdbijKandidata(int id)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            if (!KorisnikJeAdministrator())
            {
                return new HttpStatusCodeResult(
                    403,
                    "Samo administrator može da odbije kandidata."
                );
            }

            PromeniStatus(
                id,
                "Odbijena"
            );

            TempData["Poruka"] =
                "Prijava je uspešno odbijena.";

            return RedirectToAction(
                "Index"
            );
        }

        private void PopuniListeIzXml()
        {
            try
            {
                List<UskladjenostZanimanja> uskladjenosti =
                    UcitajUskladjenostiIzXml();

                List<string> zanimanja =
                    uskladjenosti
                        .Where(
                            stavka =>
                                !string.IsNullOrWhiteSpace(
                                    stavka.Zanimanje
                                )
                        )
                        .Select(
                            stavka =>
                                stavka.Zanimanje.Trim()
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                        .OrderBy(
                            zanimanje =>
                                zanimanje
                        )
                        .ToList();

                List<string> radnaMesta =
                    uskladjenosti
                        .Where(
                            stavka =>
                                !string.IsNullOrWhiteSpace(
                                    stavka.RadnoMesto
                                )
                        )
                        .Select(
                            stavka =>
                                stavka.RadnoMesto.Trim()
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                        .OrderBy(
                            radnoMesto =>
                                radnoMesto
                        )
                        .ToList();

                ViewBag.Zanimanja =
                    zanimanja;

                ViewBag.RadnaMesta =
                    radnaMesta;
            }
            catch (Exception ex)
            {
                ViewBag.Zanimanja =
                    new List<string>();

                ViewBag.RadnaMesta =
                    new List<string>();

                ModelState.AddModelError(
                    "",
                    "Nije moguće učitati zanimanja i radna mesta iz XML fajla. " +
                    ex.Message
                );
            }
        }

        private List<UskladjenostZanimanja> UcitajUskladjenostiIzXml()
        {
            string putanjaDoOgranicenja =
                DajPutanjuDoOgranicenja();

            if (!Datoteka.Exists(putanjaDoOgranicenja))
            {
                throw new FileNotFoundException(
                    "XML fajl nije pronađen na putanji: " +
                    putanjaDoOgranicenja
                );
            }

            XDocument dokument =
                XDocument.Load(
                    putanjaDoOgranicenja
                );

            List<UskladjenostZanimanja> uskladjenosti =
                new List<UskladjenostZanimanja>();

            IEnumerable<XElement> elementi =
                dokument.Descendants(
                    "UskladjenostZanimanja"
                );

            foreach (XElement element in elementi)
            {
                XElement elementZanimanja =
                    element.Element(
                        "Zanimanje"
                    );

                XElement elementRadnogMesta =
                    element.Element(
                        "RadnoMesto"
                    );

                string zanimanje =
                    elementZanimanja == null
                        ? string.Empty
                        : elementZanimanja.Value.Trim();

                string radnoMesto =
                    elementRadnogMesta == null
                        ? string.Empty
                        : elementRadnogMesta.Value.Trim();

                if (string.IsNullOrWhiteSpace(zanimanje) ||
                    string.IsNullOrWhiteSpace(radnoMesto))
                {
                    continue;
                }

                uskladjenosti.Add(
                    new UskladjenostZanimanja
                    {
                        Zanimanje =
                            zanimanje,

                        RadnoMesto =
                            radnoMesto
                    }
                );
            }

            if (uskladjenosti.Count == 0)
            {
                throw new InvalidOperationException(
                    "XML fajl ne sadrži nijedan element " +
                    "UskladjenostZanimanja sa zanimanjem i radnim mestom."
                );
            }

            return uskladjenosti;
        }

        private void PromeniStatus(
            int zahtevId,
            string noviStatus)
        {
            SPZahtevZaRegistracijuDBKlasa bazaPodataka =
                new SPZahtevZaRegistracijuDBKlasa(
                    stringKonekcije
                );

            bazaPodataka.PromeniStatusZahteva(
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

            return new List<int>(
                dokumenta
            );
        }

        private List<int> DajIzabraneDokumente(
            DataTable tabela)
        {
            List<int> izabranaDokumenta =
                new List<int>();

            if (tabela == null ||
                !tabela.Columns.Contains("TipDokumentaID"))
            {
                return izabranaDokumenta;
            }

            foreach (DataRow red in tabela.Rows)
            {
                if (red["TipDokumentaID"] == DBNull.Value)
                {
                    continue;
                }

                bool dokumentJeDostavljen =
                    !tabela.Columns.Contains("Dostavljen")
                    ||
                    red["Dostavljen"] == DBNull.Value
                    ||
                    Convert.ToBoolean(
                        red["Dostavljen"]
                    );

                if (!dokumentJeDostavljen)
                {
                    continue;
                }

                int tipDokumentaId =
                    Convert.ToInt32(
                        red["TipDokumentaID"]
                    );

                if (!izabranaDokumenta.Contains(
                    tipDokumentaId))
                {
                    izabranaDokumenta.Add(
                        tipDokumentaId
                    );
                }
            }

            return izabranaDokumenta;
        }

        private ZahtevZaRegistracijuKlasa KreirajZahtevZaIzmenu(
            DataRow red)
        {
            ZahtevZaRegistracijuKlasa zahtev =
                new ZahtevZaRegistracijuKlasa
                {
                    ZahtevID =
                        ProcitajInt(
                            red,
                            "ZahtevID"
                        ),

                    KorisnikID =
                        ProcitajInt(
                            red,
                            "KorisnikID"
                        ),

                    ImePrezime =
                        ProcitajString(
                            red,
                            "ImePrezime"
                        ),

                    Email =
                        ProcitajString(
                            red,
                            "Email"
                        ),

                    KontaktTelefon =
                        ProcitajString(
                            red,
                            "KontaktTelefon"
                        ),

                    PoslednjaSkola =
                        ProcitajString(
                            red,
                            "PoslednjaSkola"
                        ),

                    MestoSkole =
                        ProcitajString(
                            red,
                            "MestoSkole"
                        ),

                    Zanimanje =
                        ProcitajString(
                            red,
                            "Zanimanje"
                        ),

                    StepenObrazovanja =
                        ProcitajString(
                            red,
                            "StepenObrazovanja"
                        ),

                    NazivKonkursa =
                        ProcitajString(
                            red,
                            "NazivKonkursa"
                        ),

                    RadnoMesto =
                        ProcitajString(
                            red,
                            "RadnoMesto"
                        ),

                    GodineIskustva =
                        ProcitajInt(
                            red,
                            "GodineIskustva"
                        ),

                    MotivacionoPismo =
                        ProcitajString(
                            red,
                            "MotivacionoPismo"
                        ),

                    DatumPodnosenja =
                        ProcitajDatum(
                            red,
                            "DatumPodnosenja"
                        ),

                    RokKonkursa =
                        ProcitajDatum(
                            red,
                            "RokKonkursa"
                        ),

                    StatusZahteva =
                        ProcitajString(
                            red,
                            "StatusZahteva"
                        ),

                    IspunjavaOsnovneUslove =
                        ProcitajBool(
                            red,
                            "IspunjavaOsnovneUslove"
                        )
                };

            return zahtev;
        }

        private ZahtevViewModel KreirajModelZaPrikazZahteva(
     DataRow red)
        {
            ZahtevViewModel modelZaPrikaz =
                new ZahtevViewModel
                {
                    ZahtevID =
                        ProcitajInt(
                            red,
                            "ZahtevID"
                        ),

                    ImePrezime =
                        ProcitajString(
                            red,
                            "ImePrezime"
                        ),

                    Email =
                        ProcitajString(
                            red,
                            "Email"
                        ),

                    KontaktTelefon =
                        ProcitajString(
                            red,
                            "KontaktTelefon"
                        ),

                    PoslednjaSkola =
                        ProcitajString(
                            red,
                            "PoslednjaSkola"
                        ),

                    MestoSkole =
                        ProcitajString(
                            red,
                            "MestoSkole"
                        ),

                    Zanimanje =
                        ProcitajString(
                            red,
                            "Zanimanje"
                        ),

                    NazivKonkursa =
                        ProcitajString(
                            red,
                            "NazivKonkursa"
                        ),

                    RadnoMesto =
                        ProcitajString(
                            red,
                            "RadnoMesto"
                        ),

                    StepenObrazovanja =
                        ProcitajString(
                            red,
                            "StepenObrazovanja"
                        ),

                    GodineIskustva =
                        ProcitajInt(
                            red,
                            "GodineIskustva"
                        ),

                    MotivacionoPismo =
                        ProcitajString(
                            red,
                            "MotivacionoPismo"
                        ),

                    DatumPodnosenja =
                        ProcitajDatum(
                            red,
                            "DatumPodnosenja"
                        ),

                    RokKonkursa =
                        ProcitajDatum(
                            red,
                            "RokKonkursa"
                        ),

                    StatusZahteva =
                        ProcitajString(
                            red,
                            "StatusZahteva"
                        ),

                    IspunjavaOsnovneUslove =
                        ProcitajBool(
                            red,
                            "IspunjavaOsnovneUslove"
                        ),

                    Dokumentacija =
                        new List<DokumentacijaViewModel>()
                };

            return modelZaPrikaz;
        }

        private string ProcitajString(
            DataRow red,
            string nazivKolone)
        {
            if (red == null ||
                red.Table == null ||
                !red.Table.Columns.Contains(nazivKolone) ||
                red[nazivKolone] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(
                red[nazivKolone]
            );
        }

        private int ProcitajInt(
            DataRow red,
            string nazivKolone)
        {
            if (red == null ||
                red.Table == null ||
                !red.Table.Columns.Contains(nazivKolone) ||
                red[nazivKolone] == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(
                red[nazivKolone]
            );
        }

        private DateTime ProcitajDatum(
            DataRow red,
            string nazivKolone)
        {
            if (red == null ||
                red.Table == null ||
                !red.Table.Columns.Contains(nazivKolone) ||
                red[nazivKolone] == DBNull.Value)
            {
                return DateTime.MinValue;
            }

            return Convert.ToDateTime(
                red[nazivKolone]
            );
        }

        private bool ProcitajBool(
            DataRow red,
            string nazivKolone)
        {
            if (red == null ||
                red.Table == null ||
                !red.Table.Columns.Contains(nazivKolone) ||
                red[nazivKolone] == DBNull.Value)
            {
                return false;
            }

            return Convert.ToBoolean(
                red[nazivKolone]
            );
        }
    }
}