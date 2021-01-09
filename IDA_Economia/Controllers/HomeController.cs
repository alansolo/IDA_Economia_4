using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IDA_Economia.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Inversiones()
        {

            return View();
        }

        public ActionResult EconomiaEspacial()
        {

            return View();
        }

        public ActionResult Desarrollo()
        {

            return View();
        }

        public ActionResult Proyecciones()
        {

            return View();
        }

        public ActionResult EntornoMacro()
        {

            return View();
        }

        public ActionResult Prospectiva()
        {

            return View();
        }

        public ActionResult Estrategia()
        {

            return View();
        }

        public ActionResult Servicios()
        {

            return View();
        }

        public ActionResult Servicios2()
        {

            return View();
        }

        public ActionResult NotFound()
        {

            return View();
        }
    }
}