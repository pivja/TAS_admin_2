using System.ComponentModel.DataAnnotations;

namespace TAS_admin.Models
{
    // เขตพื้นที่วงกลม ถ้ารถวิ่งออกนอกเขตนี้ (หรือเข้ามาในเขตต้องห้าม) ระบบจะแจ้งเตือน
    public class Geofence
    {
        public int GeofenceId { get; set; }

        [Required(ErrorMessage = "กรุณาตั้งชื่อพื้นที่")]
        [Display(Name = "ชื่อพื้นที่")]
        public string Name { get; set; }

        [Display(Name = "ละติจูดจุดศูนย์กลาง")]
        public double CenterLat { get; set; }

        [Display(Name = "ลองจิจูดจุดศูนย์กลาง")]
        public double CenterLng { get; set; }

        [Display(Name = "รัศมี (เมตร)")]
        public double RadiusMeters { get; set; }

        // true = เขตอนุญาต (แจ้งเตือนเมื่อรถ "ออกนอก" เขตนี้)
        // false = เขตต้องห้าม (แจ้งเตือนเมื่อรถ "เข้าไปใน" เขตนี้)
        [Display(Name = "ประเภทเขต")]
        public bool IsAllowedZone { get; set; }

        public bool IsActive { get; set; }
    }
}
