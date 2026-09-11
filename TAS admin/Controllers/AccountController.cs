using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TAS_admin.Models;

namespace TAS_admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private TasDbContext _tasDb = new TasDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ระบบยังต้องมีอีเมลไม่ซ้ำกันภายใน (RequireUniqueEmail = true) แต่ฟอร์มนี้เก็บแค่ "ชื่อในระบบ"
                // จึงสร้างอีเมลหลอกจาก username ไว้ใช้ภายในเท่านั้น (เหมือนที่ทำกับ LINE/Facebook login)
                var placeholderEmail = model.Username + "@tas.local";
                var user = new ApplicationUser { UserName = model.Username, Email = placeholderEmail, UserType = model.UserType };

                if (!string.IsNullOrWhiteSpace(model.UsernameExpiryDateText))
                {
                    DateTime parsedExpiry;
                    if (DateTime.TryParse(model.UsernameExpiryDateText, out parsedExpiry))
                    {
                        user.UsernameExpiryDate = parsedExpiry;
                    }
                }

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // ===== บันทึกข้อมูลรถ + คนขับ (ถ้ากรอกมา) ผูกกับบัญชีที่เพิ่งสร้าง =====
                    Truck truck = null;
                    if (!string.IsNullOrWhiteSpace(model.TruckLicensePlate))
                    {
                        truck = new Truck
                        {
                            LicensePlate = model.TruckLicensePlate,
                            Province = model.TruckProvince,
                            Characteristics = model.TruckCharacteristics,
                            ServiceIntervalKm = 10000
                        };

                        decimal weightTon;
                        if (!string.IsNullOrWhiteSpace(model.TruckWeightTonText) && decimal.TryParse(model.TruckWeightTonText, out weightTon))
                        {
                            truck.WeightKg = weightTon * 1000m;
                        }
                    }

                    var fullName = ((model.FirstName ?? "") + " " + (model.LastName ?? "")).Trim();
                    Driver driver = null;
                    if (!string.IsNullOrWhiteSpace(fullName) || !string.IsNullOrWhiteSpace(model.Phone)
                        || !string.IsNullOrWhiteSpace(model.LicenseNo) || !string.IsNullOrWhiteSpace(model.EmployeeNo))
                    {
                        driver = new Driver
                        {
                            FullName = string.IsNullOrWhiteSpace(fullName) ? model.Username : fullName,
                            Phone = model.Phone,
                            LicenseNo = model.LicenseNo,
                            Address = model.Address,
                            EmployeeNo = model.EmployeeNo,
                            ApplicationUserId = user.Id
                        };

                        if (model.Photo != null && model.Photo.ContentLength > 0)
                        {
                            var photoFolder = Server.MapPath("~/Content/driver-photos");
                            if (!System.IO.Directory.Exists(photoFolder))
                            {
                                System.IO.Directory.CreateDirectory(photoFolder);
                            }
                            var fileName = "driver_" + user.Id + "_" + DateTime.Now.Ticks + System.IO.Path.GetExtension(model.Photo.FileName);
                            model.Photo.SaveAs(System.IO.Path.Combine(photoFolder, fileName));
                            driver.PhotoPath = "/Content/driver-photos/" + fileName;
                        }

                        if (truck != null)
                        {
                            driver.Trucks.Add(truck);
                        }

                        _tasDb.Drivers.Add(driver);
                    }
                    else if (truck != null)
                    {
                        _tasDb.Trucks.Add(truck);
                    }

                    if (driver != null || truck != null)
                    {
                        _tasDb.SaveChanges();
                    }

                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            ViewBag.Message = TempData["Message"] as string;
            return View();
        }

        // ===================== LINE Login =====================
        // หมายเหตุ: LINE ไม่มี OWIN middleware สำเร็จรูปเหมือน Facebook/Google
        // เลยทำ OAuth2 flow เองตรงนี้แทน (เรียก LINE API โดยตรง)

        [AllowAnonymous]
        public ActionResult LoginWithLine(string returnUrl)
        {
            var channelId = ConfigurationManager.AppSettings["LineChannelId"];
            if (string.IsNullOrWhiteSpace(channelId))
            {
                TempData["Message"] = "ยังไม่ได้ตั้งค่า LINE Login (LineChannelId ใน Web.config)";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            var state = Guid.NewGuid().ToString("N");
            Session["LineLoginState"] = state;
            Session["LineLoginReturnUrl"] = returnUrl;

            var callbackUrl = Url.Action("LineCallback", "Account", null, Request.Url.Scheme);
            var authorizeUrl = "https://access.line.me/oauth2/v2.1/authorize"
                + "?response_type=code"
                + "&client_id=" + Uri.EscapeDataString(channelId)
                + "&redirect_uri=" + Uri.EscapeDataString(callbackUrl)
                + "&state=" + state
                + "&scope=" + Uri.EscapeDataString("profile openid");

            return Redirect(authorizeUrl);
        }

        [AllowAnonymous]
        public async Task<ActionResult> LineCallback(string code, string state)
        {
            var expectedState = Session["LineLoginState"] as string;
            var returnUrl = Session["LineLoginReturnUrl"] as string;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state) || state != expectedState)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            var channelId = ConfigurationManager.AppSettings["LineChannelId"];
            var channelSecret = ConfigurationManager.AppSettings["LineChannelSecret"];
            var callbackUrl = Url.Action("LineCallback", "Account", null, Request.Url.Scheme);

            string lineUserId;

            using (var http = new HttpClient())
            {
                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new System.Collections.Generic.KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new System.Collections.Generic.KeyValuePair<string, string>("code", code),
                    new System.Collections.Generic.KeyValuePair<string, string>("redirect_uri", callbackUrl),
                    new System.Collections.Generic.KeyValuePair<string, string>("client_id", channelId),
                    new System.Collections.Generic.KeyValuePair<string, string>("client_secret", channelSecret)
                });

                var tokenResponse = await http.PostAsync("https://api.line.me/oauth2/v2.1/token", tokenRequest);
                if (!tokenResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("ExternalLoginFailure");
                }

                var tokenJson = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync());
                var accessToken = (string)tokenJson["access_token"];

                var profileRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.line.me/v2/profile");
                profileRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var profileResponse = await http.SendAsync(profileRequest);
                if (!profileResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("ExternalLoginFailure");
                }

                var profileJson = JObject.Parse(await profileResponse.Content.ReadAsStringAsync());
                lineUserId = (string)profileJson["userId"];
            }

            if (string.IsNullOrEmpty(lineUserId))
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            var loginInfoLine = new UserLoginInfo("LINE", lineUserId);

            var existingUser = await UserManager.FindAsync(loginInfoLine);
            if (existingUser != null)
            {
                await SignInManager.SignInAsync(existingUser, isPersistent: false, rememberBrowser: false);
                return RedirectToLocal(returnUrl);
            }

            // ยังไม่เคยมีบัญชีในระบบ -> สร้างบัญชีใหม่ให้อัตโนมัติแล้วผูกกับ LINE ของคนนี้
            // หมายเหตุ: ระบบตั้งค่าให้ต้องมีอีเมลไม่ซ้ำกัน (RequireUniqueEmail = true) แต่ LINE ไม่ได้ให้อีเมลจริงมา
            // จึงสร้างอีเมลหลอกที่ไม่ซ้ำจาก userId ของ LINE ไว้แทน (ใช้ยืนยันตัวตนในระบบเท่านั้น ไม่ใช่อีเมลจริง)
            var placeholderEmail = "line_" + lineUserId + "@line.local";
            var newUser = new ApplicationUser { UserName = "line_" + lineUserId, Email = placeholderEmail };
            var createResult = await UserManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            var addLoginResult = await UserManager.AddLoginAsync(newUser.Id, loginInfoLine);
            if (!addLoginResult.Succeeded)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            await SignInManager.SignInAsync(newUser, isPersistent: false, rememberBrowser: false);
            return RedirectToLocal(returnUrl);
        }

        // ===== Facebook Login แบบเรียก API ตรงเอง (เหมือน LINE) =====
        // เดิมใช้ app.UseFacebookAuthentication(...) ของ OWIN แต่ไลบรารีเก่าเกินไป เรียก Facebook API
        // เวอร์ชันที่ถูกเลิกรองรับแล้ว ทำให้ login แล้วเงียบๆ เด้งกลับมาหน้า Login โดยไม่ error
        [AllowAnonymous]
        public ActionResult LoginWithFacebook(string returnUrl)
        {
            var appId = ConfigurationManager.AppSettings["FacebookAppId"];
            var appSecret = ConfigurationManager.AppSettings["FacebookAppSecret"];
            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret))
            {
                TempData["Message"] = "ยังไม่ได้ตั้งค่า Facebook Login (FacebookAppId/FacebookAppSecret ใน Web.config)";
                return RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }

            var state = Guid.NewGuid().ToString("N");
            Session["FacebookLoginState"] = state;
            Session["FacebookLoginReturnUrl"] = returnUrl;

            var callbackUrl = Url.Action("FacebookCallback", "Account", null, Request.Url.Scheme);
            var authorizeUrl = "https://www.facebook.com/v21.0/dialog/oauth"
                + "?response_type=code"
                + "&client_id=" + Uri.EscapeDataString(appId)
                + "&redirect_uri=" + Uri.EscapeDataString(callbackUrl)
                + "&state=" + state
                + "&scope=" + Uri.EscapeDataString("email");

            return Redirect(authorizeUrl);
        }

        [AllowAnonymous]
        public async Task<ActionResult> FacebookCallback(string code, string state)
        {
            var expectedState = Session["FacebookLoginState"] as string;
            var returnUrl = Session["FacebookLoginReturnUrl"] as string;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state) || state != expectedState)
            {
                TempData["Message"] = "code หรือ state ไม่ถูกต้อง/หมดอายุ (session อาจหลุดระหว่างไป Facebook)";
                return RedirectToAction("ExternalLoginFailure");
            }

            var appId = ConfigurationManager.AppSettings["FacebookAppId"];
            var appSecret = ConfigurationManager.AppSettings["FacebookAppSecret"];
            var callbackUrl = Url.Action("FacebookCallback", "Account", null, Request.Url.Scheme);

            string facebookUserId;
            string facebookEmail = null;

            using (var http = new HttpClient())
            {
                var tokenUrl = "https://graph.facebook.com/v21.0/oauth/access_token"
                    + "?client_id=" + Uri.EscapeDataString(appId)
                    + "&redirect_uri=" + Uri.EscapeDataString(callbackUrl)
                    + "&client_secret=" + Uri.EscapeDataString(appSecret)
                    + "&code=" + Uri.EscapeDataString(code);

                var tokenResponse = await http.GetAsync(tokenUrl);
                var tokenBody = await tokenResponse.Content.ReadAsStringAsync();
                if (!tokenResponse.IsSuccessStatusCode)
                {
                    TempData["Message"] = "แลก access token กับ Facebook ไม่สำเร็จ: " + tokenBody;
                    return RedirectToAction("ExternalLoginFailure");
                }

                var tokenJson = JObject.Parse(tokenBody);
                var accessToken = (string)tokenJson["access_token"];
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["Message"] = "Facebook ไม่ส่ง access_token กลับมา: " + tokenBody;
                    return RedirectToAction("ExternalLoginFailure");
                }

                var profileUrl = "https://graph.facebook.com/me?fields=id,name,email&access_token=" + Uri.EscapeDataString(accessToken);
                var profileResponse = await http.GetAsync(profileUrl);
                var profileBody = await profileResponse.Content.ReadAsStringAsync();
                if (!profileResponse.IsSuccessStatusCode)
                {
                    TempData["Message"] = "ดึงข้อมูลโปรไฟล์จาก Facebook ไม่สำเร็จ: " + profileBody;
                    return RedirectToAction("ExternalLoginFailure");
                }

                var profileJson = JObject.Parse(profileBody);
                facebookUserId = (string)profileJson["id"];
                facebookEmail = (string)profileJson["email"];
            }

            if (string.IsNullOrEmpty(facebookUserId))
            {
                TempData["Message"] = "ไม่พบ id ผู้ใช้จากโปรไฟล์ Facebook";
                return RedirectToAction("ExternalLoginFailure");
            }

            var loginInfoFb = new UserLoginInfo("Facebook", facebookUserId);

            var existingUser = await UserManager.FindAsync(loginInfoFb);
            if (existingUser != null)
            {
                await SignInManager.SignInAsync(existingUser, isPersistent: false, rememberBrowser: false);
                return RedirectToLocal(returnUrl);
            }

            // ยังไม่เคยมีบัญชีในระบบ -> สร้างบัญชีใหม่ให้อัตโนมัติแล้วผูกกับ Facebook ของคนนี้
            // ถ้า Facebook ไม่ได้ให้อีเมลมา (ผู้ใช้ไม่มีอีเมลผูกไว้ หรือไม่อนุญาต) จะสร้างอีเมลหลอกแทน
            var placeholderEmail = !string.IsNullOrEmpty(facebookEmail) ? facebookEmail : ("fb_" + facebookUserId + "@facebook.local");
            var newFbUser = new ApplicationUser { UserName = "fb_" + facebookUserId, Email = placeholderEmail };
            var createFbResult = await UserManager.CreateAsync(newFbUser);
            if (!createFbResult.Succeeded)
            {
                TempData["Message"] = "สร้างบัญชีผู้ใช้ไม่สำเร็จ: " + string.Join(", ", createFbResult.Errors);
                return RedirectToAction("ExternalLoginFailure");
            }

            var addFbLoginResult = await UserManager.AddLoginAsync(newFbUser.Id, loginInfoFb);
            if (!addFbLoginResult.Succeeded)
            {
                TempData["Message"] = "ผูกบัญชี Facebook เข้ากับผู้ใช้ไม่สำเร็จ: " + string.Join(", ", addFbLoginResult.Errors);
                return RedirectToAction("ExternalLoginFailure");
            }

            await SignInManager.SignInAsync(newFbUser, isPersistent: false, rememberBrowser: false);
            return RedirectToLocal(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }

                if (_tasDb != null)
                {
                    _tasDb.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}