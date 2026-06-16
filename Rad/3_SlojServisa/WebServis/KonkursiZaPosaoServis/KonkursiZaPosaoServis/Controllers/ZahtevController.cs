using System;
using System.Web.Http;
using KlasePodataka;
using PoslovnaLogika;

namespace KonkursiZaPosaoServis.Controllers
{
    [RoutePrefix("api/zahtev")]
    public class ZahtevController : ApiController
    {
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
                        .ProveriDaLiKandidatIspunjavaUslove(
                            zahtev.DatumPodnosenja,
                            zahtev.RokKonkursa,
                            zahtev.Zanimanje,
                            zahtev.RadnoMesto
                        );

                zahtev.StatusZahteva =
                    poslovnaLogika
                        .OdrediPocetniStatus(
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

                return BadRequest(
                    detaljnaGreska
                );
            }
        }
    }
}