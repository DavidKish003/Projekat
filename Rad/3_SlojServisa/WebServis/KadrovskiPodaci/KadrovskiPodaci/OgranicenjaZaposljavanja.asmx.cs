using System;
using System.Data;
using System.Web.Services;

namespace KadrovskiPodaci
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class OgranicenjaZaposljavanja : WebService
    {
        [WebMethod]
        public DataSet DajSvaOgranicenja()
        {
            DataSet skupUskladjenosti = new DataSet();

            skupUskladjenosti.ReadXml(
                Server.MapPath("~/") +
                "XML/OgranicenjaSistematizacije.XML"
            );

            return skupUskladjenosti;
        }

        [WebMethod]
        public bool DaLiZanimanjeOdgovaraRadnomMestu(
            string zanimanje,
            string radnoMesto)
        {
            if (string.IsNullOrEmpty(zanimanje) ||
                string.IsNullOrEmpty(radnoMesto))
            {
                return false;
            }

            DataSet skupUskladjenosti =
                DajSvaOgranicenja();

            if (!skupUskladjenosti.Tables.Contains(
                    "UskladjenostZanimanja"))
            {
                return false;
            }

            string trazenoZanimanje =
                zanimanje.Trim();

            string trazenoRadnoMesto =
                radnoMesto.Trim();

            foreach (DataRow red in
                     skupUskladjenosti.Tables[
                         "UskladjenostZanimanja"
                     ].Rows)
            {
                string evidentiranoZanimanje =
                    Convert.ToString(
                        red["Zanimanje"]
                    ).Trim();

                string evidentiranoRadnoMesto =
                    Convert.ToString(
                        red["RadnoMesto"]
                    ).Trim();

                bool istoZanimanje =
                    string.Equals(
                        trazenoZanimanje,
                        evidentiranoZanimanje,
                        StringComparison.OrdinalIgnoreCase
                    );

                bool istoRadnoMesto =
                    string.Equals(
                        trazenoRadnoMesto,
                        evidentiranoRadnoMesto,
                        StringComparison.OrdinalIgnoreCase
                    );

                if (istoZanimanje && istoRadnoMesto)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
