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
        }

        public int ShipmentId { get; set; }

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
    }
}
