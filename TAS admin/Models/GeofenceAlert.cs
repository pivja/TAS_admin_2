using System;

namespace TAS_admin.Models
{
    public class GeofenceAlert
    {
        public int GeofenceAlertId { get; set; }

        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; }

        public int GeofenceId { get; set; }
        public virtual Geofence Geofence { get; set; }

        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
