using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gestion_Pizzas.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Description fonctionnelle de l'application";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Mes coordonnées";

            return View();
        }
    }
}