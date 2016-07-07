using System.Web.Mvc;

namespace GameWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Scores()
        {            
            return View();
        }
    }
}