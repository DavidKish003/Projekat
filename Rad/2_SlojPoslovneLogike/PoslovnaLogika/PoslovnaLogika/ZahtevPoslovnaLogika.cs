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

            if (rezultat != null)
            {
                return rezultat.IspunjavaOsnovneUslove;
            }

            return ProveriDaLiKandidatIspunjavaUsloveLokalno(
                datumPodnosenja,
                rokKonkursa,
                zanimanje,
                radnoMesto
            );
        }

        public string OdrediPocetniStatus(
            DateTime datumPodnosenja,
            DateTime rokKonkursa,
            string zanimanje,
            string radnoMesto)
        {
            if (datumPodnosenja.Date > rokKonkursa.Date)
            {
                return "Odbijena";
            }

            ProveraUslovaRestResponse rezultat =
                ProveriUslovePrekoRestServisa(
                    datumPodnosenja,
                    rokKonkursa,
                    zanimanje,
                    radnoMesto
                );

            bool ispunjavaOsnovneUslove;

            if (rezultat != null)
            {
                ispunjavaOsnovneUslove =
                    rezultat.IspunjavaOsnovneUslove;
            }
            else
            {
                ispunjavaOsnovneUslove =
                    ProveriDaLiKandidatIspunjavaUsloveLokalno(
                        datumPodnosenja,
                        rokKonkursa,
                        zanimanje,
                        radnoMesto
                    );
            }

            if (ispunjavaOsnovneUslove)
            {
                return "U razmatranju";
            }

            return "Primljena";
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

                    if (string.IsNullOrWhiteSpace(jsonOdgovor))
                    {
                        return null;
                    }

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

            if (string.IsNullOrWhiteSpace(zanimanjeNormalizovano) ||
                string.IsNullOrWhiteSpace(radnoMestoNormalizovano))
            {
                return false;
            }

            if (zanimanjeNormalizovano == radnoMestoNormalizovano)
            {
                return true;
            }

            List<DozvoljenaUskladjenost> dozvoljeneUskladjenosti =
                new List<DozvoljenaUskladjenost>
                {
                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "elektrotehnicar informacionih tehnologija",
                        RadnoMesto = "junior programer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "elektrotehnicar informacionih tehnologija",
                        RadnoMesto = "programer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "elektrotehnicar informacionih tehnologija",
                        RadnoMesto = "web developer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "elektrotehnicar racunara",
                        RadnoMesto = "junior programer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "programer",
                        RadnoMesto = "junior programer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "programer",
                        RadnoMesto = "web developer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "programer",
                        RadnoMesto = "backend developer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "programer",
                        RadnoMesto = "frontend developer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "softverski inzenjer",
                        RadnoMesto = "programer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "softverski inzenjer",
                        RadnoMesto = "junior programer"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "it administrator",
                        RadnoMesto = "administrator"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "it administrator",
                        RadnoMesto = "sistemski administrator"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "ekonomista",
                        RadnoMesto = "knjigovodja"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "ekonomista",
                        RadnoMesto = "racunovodja"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "masinski tehnicar",
                        RadnoMesto = "tehnicar odrzavanja"
                    },

                    new DozvoljenaUskladjenost
                    {
                        Zanimanje = "masinski tehnicar",
                        RadnoMesto = "operater masine"
                    }
                };

            foreach (DozvoljenaUskladjenost uskladjenost in dozvoljeneUskladjenosti)
            {
                string evidentiranoZanimanje =
                    NormalizujTekst(
                        uskladjenost.Zanimanje
                    );

                string evidentiranoRadnoMesto =
                    NormalizujTekst(
                        uskladjenost.RadnoMesto
                    );

                bool istoZanimanje =
                    zanimanjeNormalizovano == evidentiranoZanimanje;

                bool istoRadnoMesto =
                    radnoMestoNormalizovano == evidentiranoRadnoMesto;

                if (istoZanimanje && istoRadnoMesto)
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

    public class DozvoljenaUskladjenost
    {
        public string Zanimanje { get; set; }

        public string RadnoMesto { get; set; }
    }
}