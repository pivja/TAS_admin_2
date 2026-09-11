using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using TAS_admin.Models;

namespace TAS_admin
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // ===== ตัวช่วยดีบั๊กชั่วคราว: ถ้า middleware ตัวไหนใน pipeline นี้ throw exception
            // จะโชว์ stack trace เต็มๆ ออกมาที่หน้าเว็บแทนที่จะเป็น 500 เปล่าๆ =====
            // (ลบตัวนี้ออกได้เมื่อ deploy จริง หรือเมื่อหาสาเหตุ error เจอแล้ว)
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("OWIN PIPELINE ERROR:\r\n\r\n" + ex.ToString());
                }
            });

            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // หมายเหตุ: Facebook Login เดิมเคยใช้ app.UseFacebookAuthentication(...) ของ Katana/OWIN
            // แต่ไลบรารีตัวนี้เก่ามาก (ปี 2015) เรียก Facebook Graph API เวอร์ชันเก่าที่ Facebook เลิกรองรับไปแล้ว
            // ทำให้ login แล้วเงียบๆ เด้งกลับมาหน้า Login โดยไม่ error ให้เห็นเลย
            // จึงเปลี่ยนไปทำ Facebook Login แบบเรียก API ตรงเองแทน (แบบเดียวกับ LINE Login)
            // ดู AccountController.LoginWithFacebook / FacebookCallback

            var googleClientId = ConfigurationManager.AppSettings["GoogleClientId"];
            var googleClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"];
            if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
            {
                app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
                {
                    ClientId = googleClientId,
                    ClientSecret = googleClientSecret
                });
            }

            // หมายเหตุ: LINE Login ไม่มี OWIN middleware สำเร็จรูป จึงทำเป็นปุ่มแยกต่างหาก
            // ดู AccountController.LoginWithLine / LineCallback
        }
    }
}