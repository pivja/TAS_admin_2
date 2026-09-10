using System;
using System.ComponentModel.DataAnnotations;

namespace TAS_admin.Models
{
    public class DeliveryOrder
    {
        public int DeliveryOrderId { get; set; }

        [Display(Name = "Delivery No")]
        public string DeliveryNo { get; set; }

        [Display(Name = "Order No")]
        public string OrderNo { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Display(Name = "จุดขึ้นของ")]
        public string LoadingStation { get; set; }

        [Display(Name = "จุดหมาย")]
        public string Destination { get; set; }

        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
    }
}
