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
		public ActionResult Job_Management_Single()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Truck_information()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
		public ActionResult Truck_Insert()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Truck_Insert_Complete()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Truck_information_Single()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Person()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Person_Single()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Person_Edit()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Truck_Edit()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult History()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult History_Single()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}
		public ActionResult Tracking()
		{
			ViewBag.Message = "Your application description page.";

			return Redirect("http://119.59.122.157:8082/");
		}
		public ActionResult Delivery_Planing()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}



	}
}