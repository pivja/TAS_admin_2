using System.Web;
using System.Web.Mvc;

namespace TAS_admin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // บังคับต้อง login ก่อนถึงจะใช้งานเว็บไซต์ได้ทุกหน้า
            // (หน้า Login/Register ใน AccountController มี [AllowAnonymous] ของตัวเองอยู่แล้ว จึงยังเข้าได้ตามปกติ)
            filters.Add(new AuthorizeAttribute());
        }
    }
}
