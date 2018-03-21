using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TAS_admin.Controllers
{
    public class Admin_manageController : Controller
    {
        // GET: Admin_manage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Job_Management()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Truck_information()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
		public ActionResult Truck_information1()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Truck_information1_id()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Any_From_Person()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Edit_Truck()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Edit_personal()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult History()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult History_id()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		



	}
}