using System.Web.Mvc;

namespace KonkursiZaPosaoServis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title =
                "Konkursi za posao servis";

            return View();
        }
    }
}