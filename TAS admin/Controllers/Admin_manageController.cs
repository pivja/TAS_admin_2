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
    }
}
