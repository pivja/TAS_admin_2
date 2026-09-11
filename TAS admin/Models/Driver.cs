using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TAS_admin.Models
{
    public class Driver
    {
        public Driver()
        {
            Trucks = new List<Truck>();
        }

        public int DriverId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุล")]
        [Display(Name = "ชื่อ-นามสกุล")]
        public string FullName { get; set; }

        [Display(Name = "ใบขับขี่เลขที่")]
        public string LicenseNo { get; set; }

        [Display(Name = "ที่อยู่")]
        public string Address { get; set; }

        [Display(Name = "เบอร์โทร")]
        public string Phone { get; set; }

        [Display(Name = "หมายเลขพนักงาน")]
        public string EmployeeNo { get; set; }

        [Display(Name = "รูปภาพ")]
        public string PhotoPath { get; set; }

        // เชื่อมกับบัญชีผู้ใช้ login (ApplicationUser.Id อยู่คนละฐานข้อมูล จึงเก็บเป็น string เทียบเอง ไม่ใช้ FK ของ EF)
        public string ApplicationUserId { get; set; }

        public virtual ICollection<Truck> Trucks { get; set; }
    }
}
