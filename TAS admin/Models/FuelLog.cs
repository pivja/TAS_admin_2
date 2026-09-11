using System;
using System.ComponentModel.DataAnnotations;

namespace TAS_admin.Models
{
    public class FuelLog
    {
        public int FuelLogId { get; set; }

        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; }

        [Display(Name = "วันที่เติม")]
        [DataType(DataType.Date)]
        public DateTime FilledAt { get; set; }

        [Display(Name = "จำนวนน้ำมัน (ลิตร)")]
        public double Liters { get; set; }

        [Display(Name = "ราคารวม (บาท)")]
        public double Cost { get; set; }

        [Display(Name = "เลขไมล์ตอนเติม (กม.)")]
        public double OdometerKm { get; set; }
    }
}
