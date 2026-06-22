using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Http;
using KlasePodataka;
using PoslovnaLogika;

namespace KonkursiZaPosaoServis.Controllers
{
    [RoutePrefix("api/zahtev")]
    public class ZahtevController : ApiController
    {
        [HttpGet]
        [Route("sve")]
        public IHttpActionResult DajSvePrijave()
        {
            try
            {
                string stringKonekcije =
                    ProcitajStringKonekcije();

                SPZahtevZaRegistracijuDBKlasa bzpdtk =
                    new SPZahtevZaRegistracijuDBKlasa(
                        stringKonekcije
                    );

                DataSet setPodataka =
                    bzpdtk.DajSveZahteveZaRegistraciju();

                if (setPodataka == null ||
                    setPodataka.Tables.Count == 0)
                {
                    return Ok(
                        new List<Dictionary<string, object>>()
                    );
                }

                DataTable tabela =
                    setPodataka.Tables[0];

                List<Dictionary<string, object>> prijave =
                    PretvoriDataTableUListu(tabela);

                return Ok(prijave);
            }
            catch (Exception ex)
            {
                string detaljnaGreska =
                    ex.InnerException != null
                        ? ex.InnerException.Message
                        : ex.Message;

                return BadRequest(detaljnaGreska);
            }
        }

        [HttpPost]
        [Route("proveriuslove")]
        public IHttpActionResult ProveriUslove(
            [FromBody] ZahtevZaRegistracijuKlasa zahtev)
        {
            if (zahtev == null)
            {
                return BadRequest(
                    "Podaci o prijavi nisu prosleđeni."
                );
            }

            try
            {
                if (zahtev.DatumPodnosenja == DateTime.MinValue)
                {
                    zahtev.DatumPodnosenja =
                        DateTime.Today;
                }

                ZahtevPoslovnaLogika poslovnaLogika =
                    new ZahtevPoslovnaLogika();

                zahtev.IspunjavaOsnovneUslove =
                    poslovnaLogika
                        .ProveriDaLiKandidatIspunjavaUsloveLokalno(
                            zahtev.DatumPodnosenja,
                            zahtev.RokKonkursa,
                            zahtev.Zanimanje,
                            zahtev.RadnoMesto
                        );

                zahtev.StatusZahteva =
                    poslovnaLogika
                        .OdrediPocetniStatusLokalno(
                            zahtev.DatumPodnosenja,
                            zahtev.RokKonkursa,
                            zahtev.Zanimanje,
                            zahtev.RadnoMesto
                        );

                return Ok(zahtev);
            }
            catch (Exception ex)
            {
                string detaljnaGreska =
                    ex.InnerException != null
                        ? ex.InnerException.Message
                        : ex.Message;

                return BadRequest(detaljnaGreska);
            }
        }

        private string ProcitajStringKonekcije()
        {
            ConnectionStringSettings konekcija =
                ConfigurationManager.ConnectionStrings["Konekcija"];

            if (konekcija == null ||
                string.IsNullOrWhiteSpace(konekcija.ConnectionString))
            {
                throw new Exception(
                    "Nije pronađen connection string 'Konekcija' u Web.config fajlu."
                );
            }

            return konekcija.ConnectionString;
        }

        private List<Dictionary<string, object>> PretvoriDataTableUListu(
            DataTable tabela)
        {
            List<Dictionary<string, object>> lista =
                new List<Dictionary<string, object>>();

            if (tabela == null)
            {
                return lista;
            }

            foreach (DataRow red in tabela.Rows)
            {
                Dictionary<string, object> objekat =
                    new Dictionary<string, object>();

                foreach (DataColumn kolona in tabela.Columns)
                {
                    object vrednost =
                        red[kolona];

                    if (vrednost == DBNull.Value)
                    {
                        vrednost = null;
                    }

                    objekat.Add(
                        kolona.ColumnName,
                        vrednost
                    );
                }

                lista.Add(objekat);
            }

            return lista;
        }
    }
}