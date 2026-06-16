using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using KlasePodataka;
using PrezentacionaLogika;

using Datoteka = System.IO.File;

namespace KonkursiZaPosaoMVC.Controllers
{
    public class ZahtevController : Controller
    {
        private readonly string connectionString =
            @"Data Source=.;Initial Catalog=KonkursiZaPosao;Integrated Security=True";

        private readonly string putanjaDoOgranicenja =
            @"C:\Users\Korisnik\Desktop\seminarski\Rad\3_SlojServisa\WebServis\KadrovskiPodaci\KadrovskiPodaci\XML\OgranicenjaSistematizacije.XML";

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

        public ActionResult Index(
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

            ViewBag.Pretraga =
                pretraga;

            ViewBag.Status =
                status;

            if (dataSet.Tables.Count == 0)
            {
                return View(
                    new DataTable()
                );
            }

            DataTable tabela =
                dataSet.Tables[0];

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
        public ActionResult MojePrijave()
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

            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            DataSet dataSet =
                db.DajPrijaveKorisnika(
                    korisnikId
                );

            if (dataSet.Tables.Count == 0)
            {
                return View(
                    new DataTable()
                );
            }

            return View(
                dataSet.Tables[0]
            );
        }

        [HttpGet]
        public ActionResult Detalji(int id)
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

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

            DataTable tabela =
                dataSet.Tables[0];

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

            ZahtevViewModel viewModel =
                KreirajZahtevViewModel(
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

                viewModel.Dokumentacija.Add(
                    dokument
                );
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Dodaj()
        {
            if (!KorisnikJePrijavljen())
            {
                return RedirectToAction(
                    "Index",
                    "Login"
                );
            }

            PopuniListeIzXml();

            ZahtevZaRegistracijuKlasa zahtev =
                new ZahtevZaRegistracijuKlasa
                {
                    DatumPodnosenja =
                        DateTime.Today,

                    GodineIskustva =
                        0
                };

            return View(zahtev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dodaj(
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

            zahtev.KorisnikID =
                Convert.ToInt32(
                    Session["KorisnikID"]
                );

            ViewBag.IzabranaDokumenta =
                PretvoriDokumentaUListu(
                    Dokumenta
                );

            if (!ModelState.IsValid)
            {
                return View(zahtev);
            }

            try
            {
                List<int> dokumenta =
                    PretvoriDokumentaUListu(
                        Dokumenta
                    );

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

                SPZahtevZaRegistracijuDBKlasa db =
                    new SPZahtevZaRegistracijuDBKlasa(
                        connectionString
                    );

                db.SnimiNoviZahtevSaDokumentacijom(
                    zahtev,
                    dokumenta
                );

                TempData["Poruka"] =
                    "Prijava je uspešno podneta.";

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
                    "Greška prilikom čuvanja prijave: " +
                    detaljnaGreska
                );

                return View(zahtev);
            }
        }

        [HttpGet]
        public ActionResult Izmeni(int id)
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

            DataTable tabela =
                dataSet.Tables[0];

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
        public ActionResult Izmeni(
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

                SPZahtevZaRegistracijuDBKlasa db =
                    new SPZahtevZaRegistracijuDBKlasa(
                        connectionString
                    );

                db.IzmeniZahtevSaDokumentacijom(
                    zahtev,
                    dokumenta
                );

                TempData["Poruka"] =
                    "Prijava je uspešno izmenjena.";

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
                    "Greška prilikom izmene prijave: " +
                    detaljnaGreska
                );

                return View(zahtev);
            }
        }

        [HttpPost]
        public ActionResult Obrisi(int id)
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

            SPZahtevZaRegistracijuDBKlasa db =
                new SPZahtevZaRegistracijuDBKlasa(
                    connectionString
                );

            db.ObrisiZahtevZaRegistraciju(
                id
            );

            return RedirectToAction(
                "Index"
            );
        }

        [HttpPost]
        public ActionResult Prihvati(int id)
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

            return RedirectToAction(
                "Index"
            );
        }

        [HttpPost]
        public ActionResult Odbij(int id)
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
                        Convert.ToInt32(
                            red["ZahtevID"]
                        ),

                    ImePrezime =
                        Convert.ToString(
                            red["ImePrezime"]
                        ),

                    Email =
                        Convert.ToString(
                            red["Email"]
                        ),

                    KontaktTelefon =
                        Convert.ToString(
                            red["KontaktTelefon"]
                        ),

                    PoslednjaSkola =
                        Convert.ToString(
                            red["PoslednjaSkola"]
                        ),

                    MestoSkole =
                        Convert.ToString(
                            red["MestoSkole"]
                        ),

                    Zanimanje =
                        Convert.ToString(
                            red["Zanimanje"]
                        ),

                    StepenObrazovanja =
                        Convert.ToString(
                            red["StepenObrazovanja"]
                        ),

                    NazivKonkursa =
                        Convert.ToString(
                            red["NazivKonkursa"]
                        ),

                    RadnoMesto =
                        Convert.ToString(
                            red["RadnoMesto"]
                        ),

                    GodineIskustva =
                        red["GodineIskustva"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(
                                red["GodineIskustva"]
                            ),

                    MotivacionoPismo =
                        red["MotivacionoPismo"] == DBNull.Value
                            ? string.Empty
                            : Convert.ToString(
                                red["MotivacionoPismo"]
                            ),

                    DatumPodnosenja =
                        Convert.ToDateTime(
                            red["DatumPodnosenja"]
                        ),

                    RokKonkursa =
                        Convert.ToDateTime(
                            red["RokKonkursa"]
                        ),

                    StatusZahteva =
                        Convert.ToString(
                            red["StatusZahteva"]
                        ),

                    IspunjavaOsnovneUslove =
                        red["IspunjavaOsnovneUslove"] != DBNull.Value
                        &&
                        Convert.ToBoolean(
                            red["IspunjavaOsnovneUslove"]
                        )
                };

            if (red.Table.Columns.Contains("KorisnikID") &&
                red["KorisnikID"] != DBNull.Value)
            {
                zahtev.KorisnikID =
                    Convert.ToInt32(
                        red["KorisnikID"]
                    );
            }

            return zahtev;
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
                    Convert.ToString(
                        red["ImePrezime"]
                    ),

                Email =
                    Convert.ToString(
                        red["Email"]
                    ),

                KontaktTelefon =
                    Convert.ToString(
                        red["KontaktTelefon"]
                    ),

                PoslednjaSkola =
                    Convert.ToString(
                        red["PoslednjaSkola"]
                    ),

                MestoSkole =
                    Convert.ToString(
                        red["MestoSkole"]
                    ),

                Zanimanje =
                    Convert.ToString(
                        red["Zanimanje"]
                    ),

                NazivKonkursa =
                    Convert.ToString(
                        red["NazivKonkursa"]
                    ),

                RadnoMesto =
                    Convert.ToString(
                        red["RadnoMesto"]
                    ),

                StepenObrazovanja =
                    Convert.ToString(
                        red["StepenObrazovanja"]
                    ),

                GodineIskustva =
                    red["GodineIskustva"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                            red["GodineIskustva"]
                        ),

                MotivacionoPismo =
                    red["MotivacionoPismo"] == DBNull.Value
                        ? string.Empty
                        : Convert.ToString(
                            red["MotivacionoPismo"]
                        ),

                DatumPodnosenja =
                    Convert.ToDateTime(
                        red["DatumPodnosenja"]
                    ),

                RokKonkursa =
                    Convert.ToDateTime(
                        red["RokKonkursa"]
                    ),

                StatusZahteva =
                    Convert.ToString(
                        red["StatusZahteva"]
                    ),

                IspunjavaOsnovneUslove =
                    red["IspunjavaOsnovneUslove"] != DBNull.Value
                    &&
                    Convert.ToBoolean(
                        red["IspunjavaOsnovneUslove"]
                    ),

                Dokumentacija =
                    new List<DokumentacijaViewModel>()
            };
        }
    }
}