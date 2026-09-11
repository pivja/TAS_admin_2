using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TAS_admin.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        // ===== ข้อมูลระบบ Log On =====
        [Required(ErrorMessage = "กรุณากรอกชื่อในระบบ")]
        [Display(Name = "ชื่อในระบบ")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "ประเภทเจ้าของ Username")]
        public string UserType { get; set; }

        // เก็บเป็น string แล้วแปลงเป็นวันที่เองในคอนโทรลเลอร์ (กันปัญหารูปแบบวันที่ไม่ตรงกับ locale ของเซิร์ฟเวอร์)
        [Display(Name = "วันหมดอายุของ Username")]
        public string UsernameExpiryDateText { get; set; }

        // ===== ข้อมูลรถ =====
        [Display(Name = "ทะเบียนรถ")]
        public string TruckLicensePlate { get; set; }

        [Display(Name = "จังหวัด")]
        public string TruckProvince { get; set; }

        [Display(Name = "น้ำหนักตัวรถ (ตัน)")]
        public string TruckWeightTonText { get; set; }

        [Display(Name = "ลักษณะยานพาหนะ")]
        public string TruckCharacteristics { get; set; }

        // ===== ข้อมูลผู้ใช้ในระบบ ผู้ขับรถ =====
        [Display(Name = "ชื่อ")]
        public string FirstName { get; set; }

        [Display(Name = "นามสกุล")]
        public string LastName { get; set; }

        [Display(Name = "เบอร์โทรศัพท์")]
        public string Phone { get; set; }

        [Display(Name = "หมายเลขพนักงาน")]
        public string EmployeeNo { get; set; }

        [Display(Name = "ใบขับขี่เลขที่")]
        public string LicenseNo { get; set; }

        [Display(Name = "ที่อยู่")]
        public string Address { get; set; }

        [Display(Name = "รูปภาพ")]
        public HttpPostedFileBase Photo { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
