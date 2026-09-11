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
            // .NET Framework รุ่นเก่าอาจไม่ใช้ TLS 1.2 เป็นค่าเริ่มต้น ทำให้เรียก API ภายนอกที่บังคับ TLS 1.2+
            // (เช่น Facebook Graph API) ไม่ได้ - บังคับใช้ TLS 1.2 ตรงนี้เผื่อไว้ทั้งแอป
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // ให้ระบบสร้างฐานข้อมูล TAS_admin_Data อัตโนมัติตอนรันครั้งแรก (พร้อมข้อมูลตัวอย่าง)
            Database.SetInitializer(new TasDbInitializer());
        }
    }
}
