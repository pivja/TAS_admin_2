using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TAS_admin.Models;

namespace TAS_admin.Controllers
{
    public class Admin_manageController : Controller
    {
        private TasDbContext db = new TasDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Admin_manage
        public ActionResult Index()
        {
            return View();
        }

        // ===================== รถบรรทุกทั้งหมด =====================
        public ActionResult Truck_information()
        {
            var trucks = db.Trucks.Include("Driver").OrderBy(t => t.LicensePlate).ToList();
            return View(trucks);
        }

        public ActionResult Truck_information1()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Truck_information1_id()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Any_From_Person()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        // ===================== เพิ่ม/แก้ไข คนขับ+รถ =====================
        // id = TruckId ที่จะแก้ไข ถ้าไม่ส่งมา (null) = เพิ่มใหม่
        [HttpGet]
        public ActionResult Edit_Truck(int? id)
        {
            var vm = new TruckDriverViewModel();

            if (id.HasValue)
            {
                var truck = db.Trucks.Include("Driver").FirstOrDefault(t => t.TruckId == id.Value);
                if (truck == null)
                {
                    return HttpNotFound();
                }

                vm.TruckId = truck.TruckId;
                vm.LicensePlate = truck.LicensePlate;
                vm.Province = truck.Province;
                vm.WeightKg = truck.WeightKg;

                if (truck.Driver != null)
                {
                    vm.DriverId = truck.Driver.DriverId;
                    vm.FullName = truck.Driver.FullName;
                    vm.LicenseNo = truck.Driver.LicenseNo;
                    vm.Address = truck.Driver.Address;
                    vm.Phone = truck.Driver.Phone;
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Truck(TruckDriverViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // บันทึกข้อมูลคนขับ (เพิ่มใหม่ หรือแก้ไขของเดิม)
            Driver driver;
            if (vm.DriverId > 0)
            {
                driver = db.Drivers.Find(vm.DriverId);
            }
            else
            {
                driver = new Driver();
                db.Drivers.Add(driver);
            }
            driver.FullName = vm.FullName;
            driver.LicenseNo = vm.LicenseNo;
            driver.Address = vm.Address;
            driver.Phone = vm.Phone;

            // บันทึกข้อมูลรถ (เพิ่มใหม่ หรือแก้ไขของเดิม)
            Truck truck;
            if (vm.TruckId > 0)
            {
                truck = db.Trucks.Find(vm.TruckId);
            }
            else
            {
                truck = new Truck();
                db.Trucks.Add(truck);
            }
            truck.LicensePlate = vm.LicensePlate;
            truck.Province = vm.Province;
            truck.WeightKg = vm.WeightKg;
            truck.Driver = driver;

            db.SaveChanges();

            TempData["Message"] = "บันทึกข้อมูลรถและคนขับเรียบร้อยแล้ว";
            return RedirectToAction("Truck_information");
        }

        // ===================== มอบหมายงานขนส่ง =====================
        [HttpGet]
        public ActionResult Job_Management(int? shipmentId)
        {
            var vm = BuildJobManagementViewModel(shipmentId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Job_Management(Shipment shipment)
        {
            if (string.IsNullOrWhiteSpace(shipment.ShipmentNo))
            {
                ModelState.AddModelError("ShipmentNo", "กรุณากรอก Shipment No");
            }

            if (!ModelState.IsValid)
            {
                var vm = BuildJobManagementViewModel(shipment.ShipmentId > 0 ? shipment.ShipmentId : (int?)null);
                vm.Shipment = shipment;
                return View(vm);
            }

            if (shipment.ShipmentId > 0)
            {
                var existing = db.Shipments.Find(shipment.ShipmentId);
                existing.ShipmentNo = shipment.ShipmentNo;
                existing.Status = shipment.Status;
                existing.DeliveryDate = shipment.DeliveryDate;
                existing.DriverId = shipment.DriverId;
                existing.TruckId = shipment.TruckId;
            }
            else
            {
                db.Shipments.Add(shipment);
            }

            db.SaveChanges();

            TempData["Message"] = "บันทึกงานขนส่งเรียบร้อยแล้ว ตอนนี้เพิ่ม Delivery Order ได้เลย";
            return RedirectToAction("Job_Management", new { shipmentId = shipment.ShipmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDeliveryOrder(DeliveryOrder newDeliveryOrder)
        {
            if (newDeliveryOrder.ShipmentId > 0 && !string.IsNullOrWhiteSpace(newDeliveryOrder.DeliveryNo))
            {
                db.DeliveryOrders.Add(newDeliveryOrder);
                db.SaveChanges();
                TempData["Message"] = "เพิ่ม Delivery Order เรียบร้อยแล้ว";
            }

            return RedirectToAction("Job_Management", new { shipmentId = newDeliveryOrder.ShipmentId });
        }

        private JobManagementViewModel BuildJobManagementViewModel(int? shipmentId)
        {
            var vm = new JobManagementViewModel
            {
                DriverList = new SelectList(db.Drivers.OrderBy(d => d.FullName).ToList(), "DriverId", "FullName"),
                TruckList = new SelectList(db.Trucks.OrderBy(t => t.LicensePlate).ToList(), "TruckId", "LicensePlate")
            };

            if (shipmentId.HasValue)
            {
                var shipment = db.Shipments.Find(shipmentId.Value);
                if (shipment != null)
                {
                    vm.Shipment = shipment;
                    vm.DeliveryOrders = db.DeliveryOrders
                        .Where(o => o.ShipmentId == shipmentId.Value)
                        .OrderBy(o => o.DeliveryOrderId)
                        .ToList();
                    vm.NewDeliveryOrder = new DeliveryOrder { ShipmentId = shipmentId.Value };
                }
            }

            if (vm.Shipment == null)
            {
                vm.Shipment = new Shipment { DeliveryDate = DateTime.Today, Status = "รอดำเนินการ" };
            }

            return vm;
        }

        // ===================== ประวัติงานขนส่ง =====================
        public ActionResult History()
        {
            var shipments = db.Shipments
                .Include("Driver")
                .Include("Truck")
                .OrderByDescending(s => s.DeliveryDate)
                .ToList();
            return View(shipments);
        }

        public ActionResult History_id(int id)
        {
            var shipment = db.Shipments
                .Include("Driver")
                .Include("Truck")
                .FirstOrDefault(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return HttpNotFound();
            }

            ViewBag.DeliveryOrders = db.DeliveryOrders
                .Where(o => o.ShipmentId == id)
                .OrderBy(o => o.DeliveryOrderId)
                .ToList();

            return View(shipment);
        }

        // ===================== ติดตามตำแหน่ง GPS =====================

        // หน้าที่คนขับเปิดค้างไว้บนมือถือ ระบบจะขอตำแหน่งจากเบราว์เซอร์แล้วส่งเข้าระบบให้เองอัตโนมัติ
        // (คนขับไม่มีบัญชี login ในระบบ จึงต้องเปิดให้เข้าได้โดยไม่ต้อง login)
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Track(int truckId)
        {
            var truck = db.Trucks.Find(truckId);
            if (truck == null)
            {
                return HttpNotFound();
            }
            return View(truck);
        }

        // รับพิกัดจากหน้า Track แล้วบันทึกตำแหน่งล่าสุดของรถคันนั้น
        // + เก็บลงประวัติ (GpsPing) สำหรับ playback, สะสมเลขไมล์, และเช็คว่าออกนอก/เข้าเขตต้องห้ามไหม
        [AllowAnonymous]
        [HttpPost]
        public JsonResult UpdateLocation(int truckId, double lat, double lng)
        {
            var truck = db.Trucks.Find(truckId);
            if (truck == null)
            {
                return Json(new { ok = false, message = "ไม่พบรถคันนี้" });
            }

            // สะสมระยะทางจากจุดก่อนหน้า (ถ้ามี) เข้าเลขไมล์ของรถ
            if (truck.Latitude.HasValue && truck.Longitude.HasValue)
            {
                var deltaMeters = HaversineMeters(truck.Latitude.Value, truck.Longitude.Value, lat, lng);
                // กันค่าผิดปกติ (GPS กระโดดไกลเกินจริงในช่วงเวลาสั้นๆ) ไม่ให้บวกระยะที่เกิน 5 กม./ครั้ง
                if (deltaMeters < 5000)
                {
                    truck.OdometerKm += deltaMeters / 1000.0;
                }
            }

            truck.Latitude = lat;
            truck.Longitude = lng;
            truck.LocationUpdatedAt = DateTime.Now;

            db.GpsPings.Add(new GpsPing
            {
                TruckId = truckId,
                Latitude = lat,
                Longitude = lng,
                RecordedAt = DateTime.Now
            });

            CheckGeofences(truck, lat, lng);

            db.SaveChanges();

            return Json(new { ok = true });
        }

        // ระยะทางระหว่าง 2 พิกัด (เมตร) ด้วยสูตร Haversine
        private static double HaversineMeters(double lat1, double lng1, double lat2, double lng2)
        {
            const double earthRadiusMeters = 6371000;
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLng = (lng2 - lng1) * Math.PI / 180.0;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0)
                * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusMeters * c;
        }

        // เช็คว่าตำแหน่งใหม่ของรถ ผิดเงื่อนไขเขตพื้นที่ (geofence) ที่ตั้งไว้ไหม ถ้าใช่ให้สร้างการแจ้งเตือน
        // (กันแจ้งซ้ำถี่เกินไป: แจ้งใหม่ได้เมื่อผ่านไปแล้วอย่างน้อย 30 นาทีจากแจ้งครั้งก่อนของ รถ+เขตนั้น)
        private void CheckGeofences(Truck truck, double lat, double lng)
        {
            var activeGeofences = db.Geofences.Where(g => g.IsActive).ToList();
            foreach (var g in activeGeofences)
            {
                var distanceMeters = HaversineMeters(g.CenterLat, g.CenterLng, lat, lng);
                bool isInsideZone = distanceMeters <= g.RadiusMeters;

                bool violated = g.IsAllowedZone ? !isInsideZone : isInsideZone;
                if (!violated)
                {
                    continue;
                }

                var recentAlertExists = db.GeofenceAlerts.Any(a =>
                    a.TruckId == truck.TruckId &&
                    a.GeofenceId == g.GeofenceId &&
                    DbFunctions.DiffMinutes(a.CreatedAt, DateTime.Now) < 30);

                if (recentAlertExists)
                {
                    continue;
                }

                var message = g.IsAllowedZone
                    ? "รถ " + truck.LicensePlate + " ออกนอกเขต \"" + g.Name + "\""
                    : "รถ " + truck.LicensePlate + " เข้าไปในเขตต้องห้าม \"" + g.Name + "\"";

                db.GeofenceAlerts.Add(new GeofenceAlert
                {
                    TruckId = truck.TruckId,
                    GeofenceId = g.GeofenceId,
                    Message = message,
                    CreatedAt = DateTime.Now,
                    IsRead = false
                });
            }
        }

        // หน้าแผนที่รวมตำแหน่งรถทุกคัน (สำหรับแอดมิน)
        public ActionResult Map()
        {
            return View();
        }

        // ให้หน้าแผนที่ดึงตำแหน่งรถล่าสุดเป็น JSON ทุกๆ ไม่กี่วินาที
        [HttpGet]
        public JsonResult TruckLocations()
        {
            var data = db.Trucks
                .Where(t => t.Latitude != null && t.Longitude != null)
                .Select(t => new
                {
                    truckId = t.TruckId,
                    licensePlate = t.LicensePlate,
                    driverName = t.Driver != null ? t.Driver.FullName : null,
                    lat = t.Latitude,
                    lng = t.Longitude,
                    updatedAt = t.LocationUpdatedAt
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // ===================== ดูเส้นทางย้อนหลัง (Trip Playback) =====================

        public ActionResult Playback(int truckId, DateTime? date)
        {
            var truck = db.Trucks.Find(truckId);
            if (truck == null)
            {
                return HttpNotFound();
            }

            var day = date ?? DateTime.Today;
            var start = day.Date;
            var end = start.AddDays(1);

            ViewBag.SelectedDate = start;
            ViewBag.Pings = db.GpsPings
                .Where(p => p.TruckId == truckId && p.RecordedAt >= start && p.RecordedAt < end)
                .OrderBy(p => p.RecordedAt)
                .Select(p => new { lat = p.Latitude, lng = p.Longitude, t = p.RecordedAt })
                .ToList();

            return View(truck);
        }

        // ===================== เขตพื้นที่ (Geofence) =====================

        public ActionResult GeofenceList()
        {
            var list = db.Geofences.OrderBy(g => g.Name).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult GeofenceCreate()
        {
            return View(new Geofence { RadiusMeters = 5000, IsAllowedZone = true, IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeofenceCreate(Geofence geofence)
        {
            if (!ModelState.IsValid)
            {
                return View(geofence);
            }

            db.Geofences.Add(geofence);
            db.SaveChanges();
            TempData["Message"] = "เพิ่มเขตพื้นที่เรียบร้อยแล้ว";
            return RedirectToAction("GeofenceList");
        }

        // แจ้งเตือนที่ยังไม่ได้อ่าน (สำหรับ badge/รายการบนหน้าแผนที่)
        [HttpGet]
        public JsonResult GeofenceAlertsUnread()
        {
            var alerts = db.GeofenceAlerts
                .Where(a => !a.IsRead)
                .OrderByDescending(a => a.CreatedAt)
                .Take(20)
                .Select(a => new { a.GeofenceAlertId, a.Message, a.CreatedAt })
                .ToList();
            return Json(alerts, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MarkGeofenceAlertsRead()
        {
            var unread = db.GeofenceAlerts.Where(a => !a.IsRead).ToList();
            foreach (var a in unread)
            {
                a.IsRead = true;
            }
            db.SaveChanges();
            return Json(new { ok = true });
        }

        // ===================== ซ่อมบำรุงรถ (Maintenance) =====================

        public ActionResult Maintenance()
        {
            var trucks = db.Trucks.OrderBy(t => t.LicensePlate).ToList();
            return View(trucks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceDone(int truckId)
        {
            var truck = db.Trucks.Find(truckId);
            if (truck != null)
            {
                truck.LastServiceOdometerKm = truck.OdometerKm;
                truck.LastServiceDate = DateTime.Now;
                db.SaveChanges();
                TempData["Message"] = "บันทึกการซ่อมบำรุงของรถ " + truck.LicensePlate + " เรียบร้อยแล้ว";
            }
            return RedirectToAction("Maintenance");
        }

        // ===================== บันทึกการเติมน้ำมัน (Fuel Log) =====================

        public ActionResult FuelLogList(int truckId)
        {
            var truck = db.Trucks.Find(truckId);
            if (truck == null)
            {
                return HttpNotFound();
            }

            ViewBag.Truck = truck;
            var logs = db.FuelLogs
                .Where(f => f.TruckId == truckId)
                .OrderBy(f => f.OdometerKm)
                .ToList();
            return View(logs);
        }

        [HttpGet]
        public ActionResult FuelLogCreate(int truckId)
        {
            return View(new FuelLog { TruckId = truckId, FilledAt = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FuelLogCreate(FuelLog fuelLog)
        {
            if (!ModelState.IsValid)
            {
                return View(fuelLog);
            }

            db.FuelLogs.Add(fuelLog);
            db.SaveChanges();
            TempData["Message"] = "บันทึกการเติมน้ำมันเรียบร้อยแล้ว";
            return RedirectToAction("FuelLogList", new { truckId = fuelLog.TruckId });
        }

        // ===================== ลิงก์ติดตามพัสดุสำหรับลูกค้า (ไม่ต้อง login) =====================

        [AllowAnonymous]
        public ActionResult TrackShipment(Guid token)
        {
            var shipment = db.Shipments
                .Include("Truck")
                .Include("Driver")
                .FirstOrDefault(s => s.TrackingToken == token);

            if (shipment == null)
            {
                return HttpNotFound();
            }

            ViewBag.DeliveryOrder = db.DeliveryOrders.FirstOrDefault(o => o.ShipmentId == shipment.ShipmentId);
            return View(shipment);
        }

        // ตำแหน่งรถล่าสุดของ shipment นี้ - เข้าถึงได้เฉพาะคนที่มีลิงก์ (token) เท่านั้น ไม่ต้อง login
        // (จงใจไม่รับ truckId ตรงๆ เพื่อกันคนเดา id รถคันอื่นดูตำแหน่งได้)
        [AllowAnonymous]
        [HttpGet]
        public JsonResult TrackShipmentLocation(Guid token)
        {
            var shipment = db.Shipments.Include("Truck").FirstOrDefault(s => s.TrackingToken == token);
            if (shipment == null || shipment.Truck == null || !shipment.Truck.Latitude.HasValue)
            {
                return Json(new { ok = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                ok = true,
                lat = shipment.Truck.Latitude,
                lng = shipment.Truck.Longitude,
                updatedAt = shipment.Truck.LocationUpdatedAt
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
