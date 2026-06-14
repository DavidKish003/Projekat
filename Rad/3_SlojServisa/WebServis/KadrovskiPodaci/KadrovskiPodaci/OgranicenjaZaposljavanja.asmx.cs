using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
//
using System.Data; 

namespace KadrovskiPodaci
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OgranicenjaZaposljavanja : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet DajSvaOgranicenja()
        {
            DataSet dsOgranicenja = new DataSet();
            dsOgranicenja.ReadXml(Server.MapPath("~/") + "XML/OgranicenjaSistematizacije.XML");

            return dsOgranicenja;
        }


        [WebMethod]
        public int DajMaxBrojNastavnika(string pomSifraZvanja)
        {
            int MaxBrojNastavnika = 0;
            DataSet dsOgranicenja = new DataSet();
            dsOgranicenja.ReadXml(Server.MapPath("~/") + "XML/OgranicenjaSistematizacije.XML");
            // filtriranje dataset-a
            DataRow[] result = dsOgranicenja.Tables[0].Select("SifraZvanja='" + pomSifraZvanja + "'");
            MaxBrojNastavnika = int.Parse (result[0].ItemArray[1].ToString());

            return MaxBrojNastavnika;
        }

       
    }
}