using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace PoslovnaLogika
{
    public class ZahtevPoslovnaLogika
    {
        private const string PodrazumevaniRestServisUrl =
            "http://localhost:49677/api/zahtev/proveriuslove";

        public bool ProveriDaLiKandidatIspunjavaUslove(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            ProveraUslovaRestResponse rezultat =
                ProveriUslovePrekoRestServisa(
                    datumPodnosenja,
                    rokKonkursa,
                    zanimanje,
                    radnoMesto
                );

            if (rezultat == null)
            {
                return false;
            }

            return rezultat.IspunjavaOsnovneUslove;
        }

        public string OdrediPocetniStatus(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            ProveraUslovaRestResponse rezultat =
                ProveriUslovePrekoRestServisa(
                    datumPodnosenja,
                    rokKonkursa,
                    zanimanje,
                    radnoMesto
                );

            if (rezultat == null ||
                string.IsNullOrWhiteSpace(rezultat.StatusZahteva))
            {
                return "Odbijena";
            }

            return rezultat.StatusZahteva;
        }

        public bool ProveriDaLiKandidatIspunjavaUsloveLokalno(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            bool prijavaJeUroku =
                datumPodnosenja.Date <=
                rokKonkursa.Date;

            if (!prijavaJeUroku)
            {
                return false;
            }

            return DaLiZanimanjeOdgovaraRadnomMestuLokalno(
                zanimanje,
                radnoMesto
            );
        }

        public string OdrediPocetniStatusLokalno(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            if (datumPodnosenja.Date >
                rokKonkursa.Date)
            {
                return "Odbijena";
            }

            bool zanimanjeOdgovara =
                ProveriDaLiKandidatIspunjavaUsloveLokalno(
                    datumPodnosenja,
                    rokKonkursa,
                    zanimanje,
                    radnoMesto
                );

            if (zanimanjeOdgovara)
            {
                return "U razmatranju";
            }

            return "Primljena";
        }

        private ProveraUslovaRestResponse ProveriUslovePrekoRestServisa(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            string servisUrl =
                ConfigurationManager.AppSettings[
                    "ZahtevProveraUslovaRestUrl"
                ];

            if (string.IsNullOrWhiteSpace(servisUrl))
            {
                servisUrl = PodrazumevaniRestServisUrl;
            }

            ProveraUslovaRestRequest zahtev =
                new ProveraUslovaRestRequest
                {
                    DatumPodnosenja = datumPodnosenja,
                    RokKonkursa = rokKonkursa,
                    Zanimanje = zanimanje,
                    RadnoMesto = radnoMesto
                };

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            string jsonZahtev =
                serializer.Serialize(zahtev);

            try
            {
                using (HttpClient klijent = new HttpClient())
                {
                    klijent.Timeout = TimeSpan.FromSeconds(10);

                    StringContent sadrzaj =
                        new StringContent(
                            jsonZahtev,
                            Encoding.UTF8,
                            "application/json"
                        );

                    HttpResponseMessage odgovor =
                        klijent
                            .PostAsync(servisUrl, sadrzaj)
                            .GetAwaiter()
                            .GetResult();

                    if (!odgovor.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string jsonOdgovor =
                        odgovor.Content
                            .ReadAsStringAsync()
                            .GetAwaiter()
                            .GetResult();

                    ProveraUslovaRestResponse rezultat =
                        serializer.Deserialize<ProveraUslovaRestResponse>(
                            jsonOdgovor
                        );

                    return rezultat;
                }
            }
            catch
            {
                return null;
            }
        }

        private bool DaLiZanimanjeOdgovaraRadnomMestuLokalno(
            string zanimanje,
            string radnoMesto)
        {
            string zanimanjeNormalizovano =
                NormalizujTekst(zanimanje);

            string radnoMestoNormalizovano =
                NormalizujTekst(radnoMesto);

            if (zanimanjeNormalizovano == radnoMestoNormalizovano)
            {
                return true;
            }

            Dictionary<string, List<string>> dozvoljenaZanimanja =
                new Dictionary<string, List<string>>
                {
                    {
                        "programer",
                        new List<string>
                        {
                            "programer",
                            "softverski inzenjer",
                            "web developer",
                            "backend developer",
                            "frontend developer"
                        }
                    },
                    {
                        "administrator",
                        new List<string>
                        {
                            "administrator",
                            "it administrator",
                            "sistemski administrator"
                        }
                    },
                    {
                        "ekonomista",
                        new List<string>
                        {
                            "ekonomista",
                            "racunovodja",
                            "finansijski administrator"
                        }
                    },
                    {
                        "masinski tehnicar",
                        new List<string>
                        {
                            "masinski tehnicar",
                            "tehnicar odrzavanja",
                            "operater masine"
                        }
                    }
                };

            foreach (var grupa in dozvoljenaZanimanja)
            {
                bool zanimanjeJeUGrupi =
                    grupa.Value.Any(x =>
                        NormalizujTekst(x) == zanimanjeNormalizovano);

                bool radnoMestoJeUGrupi =
                    grupa.Value.Any(x =>
                        NormalizujTekst(x) == radnoMestoNormalizovano);

                if (zanimanjeJeUGrupi && radnoMestoJeUGrupi)
                {
                    return true;
                }
            }

            return false;
        }

        private string NormalizujTekst(string tekst)
        {
            if (tekst == null)
            {
                return "";
            }

            return tekst
                .Trim()
                .ToLower()
                .Replace("č", "c")
                .Replace("ć", "c")
                .Replace("š", "s")
                .Replace("đ", "dj")
                .Replace("ž", "z");
        }
    }

    public class ProveraUslovaRestRequest
    {
        public DateTime DatumPodnosenja { get; set; }

        public DateTime RokKonkursa { get; set; }

        public string Zanimanje { get; set; }

        public string RadnoMesto { get; set; }
    }

    public class ProveraUslovaRestResponse
    {
        public bool IspunjavaOsnovneUslove { get; set; }

        public string StatusZahteva { get; set; }
    }
}