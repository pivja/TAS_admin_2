using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TAS_admin.Models
{
    public class Shipment
    {
        public Shipment()
        {
            DeliveryOrders = new List<DeliveryOrder>();
            TrackingToken = Guid.NewGuid();
        }

        public int ShipmentId { get; set; }

        // รหัสลับสำหรับสร้างลิงก์ติดตามพัสดุให้ลูกค้า (ไม่ต้อง login)
        public Guid TrackingToken { get; set; }

        [Required(ErrorMessage = "กรุณากรอก Shipment No")]
        [Display(Name = "Shipment No")]
        public string ShipmentNo { get; set; }

        [Display(Name = "สถานะ")]
        public string Status { get; set; }

        [Display(Name = "วันที่ส่ง")]
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "คนขับ")]
        public int? DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        [Display(Name = "ทะเบียนรถ")]
        public int? TruckId { get; set; }
        public virtual Truck Truck { get; set; }

        public virtual ICollection<DeliveryOrder> DeliveryOrders { get; set; }

        // ===== หลักฐานการส่งของ (POD) จากแอปคนขับ =====
        [Display(Name = "รูปหลักฐานการส่งของ")]
        public string PodPhotoPath { get; set; }

        [Display(Name = "ลายเซ็นผู้รับของ (base64)")]
        public string PodSignatureData { get; set; }

        [Display(Name = "เวลาที่ส่งของสำเร็จ")]
        public DateTime? PodDeliveredAt { get; set; }
    }
}
