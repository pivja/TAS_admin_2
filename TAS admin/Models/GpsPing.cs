using System;

namespace TAS_admin.Models
{
    // เก็บพิกัดทุกครั้งที่รถส่งตำแหน่งเข้ามา ใช้สำหรับดูเส้นทางย้อนหลัง (trip playback)
    public class GpsPing
    {
        public int GpsPingId { get; set; }

        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
