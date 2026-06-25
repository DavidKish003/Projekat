using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml.Linq;
using KlasePodataka;

namespace KonkursiZaPosaoServis.Controllers
{
    [RoutePrefix("api/parametri")]
    public class ParametriController : ApiController
    {
        [HttpGet]
        [Route("ogranicenja")]
        public IHttpActionResult Ogranicenja()
        {
            try
            {
                List<UskladjenostZanimanjaKlasa> uskladjenosti =
                    UcitajUskladjenostiIzXml();

                return Ok(
                    uskladjenosti
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    DajDetaljnuGresku(ex)
                );
            }
        }

        [HttpGet]
        [Route("zanimanja")]
        public IHttpActionResult Zanimanja()
        {
            try
            {
                List<string> zanimanja =
                    UcitajUskladjenostiIzXml()
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

                return Ok(
                    zanimanja
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    DajDetaljnuGresku(ex)
                );
            }
        }

        [HttpGet]
        [Route("radnamesta")]
        public IHttpActionResult RadnaMesta(
            string zanimanje = null)
        {
            try
            {
                List<UskladjenostZanimanjaKlasa> uskladjenosti =
                    UcitajUskladjenostiIzXml();

                IEnumerable<UskladjenostZanimanjaKlasa> rezultat =
                    uskladjenosti;

                if (!string.IsNullOrWhiteSpace(zanimanje))
                {
                    rezultat =
                        rezultat.Where(
                            stavka =>
                                string.Equals(
                                    stavka.Zanimanje.Trim(),
                                    zanimanje.Trim(),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        );
                }

                List<string> radnaMesta =
                    rezultat
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

                return Ok(
                    radnaMesta
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    DajDetaljnuGresku(ex)
                );
            }
        }

        [HttpGet]
        [Route("proveriuskladjenost")]
        public IHttpActionResult ProveriUskladjenost(
            string zanimanje,
            string radnoMesto)
        {
            if (string.IsNullOrWhiteSpace(zanimanje))
            {
                return BadRequest(
                    "Zanimanje nije prosleđeno."
                );
            }

            if (string.IsNullOrWhiteSpace(radnoMesto))
            {
                return BadRequest(
                    "Radno mesto nije prosleđeno."
                );
            }

            try
            {
                bool uskladjeno =
                    UcitajUskladjenostiIzXml()
                        .Any(
                            stavka =>
                                string.Equals(
                                    stavka.Zanimanje.Trim(),
                                    zanimanje.Trim(),
                                    StringComparison.OrdinalIgnoreCase
                                )
                                &&
                                string.Equals(
                                    stavka.RadnoMesto.Trim(),
                                    radnoMesto.Trim(),
                                    StringComparison.OrdinalIgnoreCase
                                )
                        );

                return Ok(
                    uskladjeno
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    DajDetaljnuGresku(ex)
                );
            }
        }

        private List<UskladjenostZanimanjaKlasa> UcitajUskladjenostiIzXml()
        {
            string putanjaDoXmlFajla =
                @"C:\Users\Korisnik\Desktop\seminarski\Rad\3_SlojServisa\WebServis\KadrovskiPodaci\KadrovskiPodaci\XML\OgranicenjaSistematizacije.XML";

            if (!System.IO.File.Exists(putanjaDoXmlFajla))
            {
                throw new System.IO.FileNotFoundException(
                    "XML fajl nije pronađen na putanji: " +
                    putanjaDoXmlFajla
                );
            }

            XDocument xmlDokument =
                XDocument.Load(
                    putanjaDoXmlFajla
                );

            IEnumerable<XElement> elementi =
                xmlDokument.Descendants(
                    "UskladjenostZanimanja"
                );

            List<UskladjenostZanimanjaKlasa> uskladjenosti =
                new List<UskladjenostZanimanjaKlasa>();

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
                    new UskladjenostZanimanjaKlasa
                    {
                        Zanimanje = zanimanje,
                        RadnoMesto = radnoMesto
                    }
                );
            }

            if (uskladjenosti.Count == 0)
            {
                throw new InvalidOperationException(
                    "XML fajl ne sadrži nijednu ispravnu kombinaciju " +
                    "zanimanja i radnog mesta."
                );
            }

            return uskladjenosti;
        }

        private string DajDetaljnuGresku(
            Exception izuzetak)
        {
            if (izuzetak.InnerException != null)
            {
                return izuzetak.InnerException.Message;
            }

            return izuzetak.Message;
        }
    }
}