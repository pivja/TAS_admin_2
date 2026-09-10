using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TAS_admin.Models
{
    // ใช้กับหน้า Edit_Truck - แก้ไข/เพิ่ม "คนขับ" และ "รถ" พร้อมกันในฟอร์มเดียว (ตามหน้าตาเดิมของ mockup)
    public class TruckDriverViewModel
    {
        public int TruckId { get; set; }
        public int DriverId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกทะเบียนรถ")]
        [Display(Name = "ทะเบียนรถ")]
        public string LicensePlate { get; set; }

        [Display(Name = "จังหวัด")]
        public string Province { get; set; }

        [Display(Name = "น้ำหนัก (กก.)")]
        public decimal? WeightKg { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุลคนขับ")]
        [Display(Name = "ชื่อ-นามสกุล")]
        public string FullName { get; set; }

        [Display(Name = "ใบขับขี่เลขที่")]
        public string LicenseNo { get; set; }

        [Display(Name = "ที่อยู่")]
        public string Address { get; set; }

        [Display(Name = "เบอร์โทร")]
        public string Phone { get; set; }
    }

    // ใช้กับหน้า Job_Management - สร้างงานขนส่งใหม่ + แสดง/เพิ่ม Delivery Order ของงานนั้น
    public class JobManagementViewModel
    {
        public JobManagementViewModel()
        {
            DeliveryOrders = new List<DeliveryOrder>();
            NewDeliveryOrder = new DeliveryOrder();
        }

        public Shipment Shipment { get; set; }
        public List<DeliveryOrder> DeliveryOrders { get; set; }
        public DeliveryOrder NewDeliveryOrder { get; set; }

        public SelectList DriverList { get; set; }
        public SelectList TruckList { get; set; }
    }
}
