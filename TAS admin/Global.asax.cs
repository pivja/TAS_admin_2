using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TAS_admin.Models;

namespace TAS_admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // ให้ระบบสร้างฐานข้อมูล TAS_admin_Data อัตโนมัติตอนรันครั้งแรก (พร้อมข้อมูลตัวอย่าง)
            Database.SetInitializer(new TasDbInitializer());
        }
    }
}
