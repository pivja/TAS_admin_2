using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TAS_admin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			return RedirectToAction("Delivery_Planing", "Admin_manage",  new { area = "Admin_manageController" });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
      
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}