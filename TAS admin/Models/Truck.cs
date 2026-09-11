using System.ComponentModel.DataAnnotations;

namespace TAS_admin.Models
{
    public class Truck
    {
        public int TruckId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกทะเบียนรถ")]
        [Display(Name = "ทะเบียนรถ")]
        public string LicensePlate { get; set; }

        [Display(Name = "จังหวัด")]
        public string Province { get; set; }

        [Display(Name = "น้ำหนัก (กก.)")]
        public decimal? WeightKg { get; set; }

        [Display(Name = "คนขับ")]
        public int? DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        [Display(Name = "ละติจูด")]
        public double? Latitude { get; set; }

        [Display(Name = "ลองจิจูด")]
        public double? Longitude { get; set; }

        [Display(Name = "อัปเดตตำแหน่งล่าสุด")]
        public System.DateTime? LocationUpdatedAt { get; set; }

        // ===== ข้อมูลระยะทาง / ซ่อมบำรุง =====

        [Display(Name = "เลขไมล์สะสม (กม.)")]
        public double OdometerKm { get; set; }

        [Display(Name = "เลขไมล์ตอนเข้าซ่อมบำรุงล่าสุด (กม.)")]
        public double? LastServiceOdometerKm { get; set; }

        [Display(Name = "รอบระยะซ่อมบำรุง (กม.)")]
        public double ServiceIntervalKm { get; set; }

        [Display(Name = "วันที่เข้าซ่อมบำรุงล่าสุด")]
        public System.DateTime? LastServiceDate { get; set; }
    }
}
