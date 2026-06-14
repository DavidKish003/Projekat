using System;
using System.Web.Http;
using System.Xml;
using RegistracijaVozilaServis.Models;

namespace RegistracijaVozilaServis.Controllers
{
     public class ParametriController : ApiController
     {
          [HttpGet]
          public IHttpActionResult PoslovnoPravilo()
          {
               try
               {
                    XmlDocument xmlDoc = new XmlDocument();

                    xmlDoc.Load(@"C:\Users\Luka\Desktop\Seminarski rad - Razvoj višeslojnog softvera\Primer\1_SlojPodataka\XML\ParametriPoslovnogPravila.xml");

                    XmlNode root = xmlDoc.SelectSingleNode("ParametriPoslovnogPravila");

                    ParametriPravilaModel parametri = new ParametriPravilaModel
                    {
                         MaksimalnaStarostVozila = int.Parse(root["MaksimalnaStarostVozila"].InnerText),
                         PeriodTehnickogPregledaUMesecima = int.Parse(root["PeriodTehnickogPregledaUMesecima"].InnerText)
                    };

                    return Ok(parametri);
               }
               catch (Exception ex)
               {
                    return BadRequest(ex.Message);
               }
          }
     }
}