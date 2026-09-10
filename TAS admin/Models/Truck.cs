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
    }
}
