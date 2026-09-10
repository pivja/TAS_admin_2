using System.Data.Entity;

namespace TAS_admin.Models
{
    // ตอนรันครั้งแรก ถ้ายังไม่มีฐานข้อมูล ระบบจะสร้างให้อัตโนมัติ พร้อมใส่ข้อมูลตัวอย่าง 3 รายการ
    public class TasDbInitializer : CreateDatabaseIfNotExists<TasDbContext>
    {
        protected override void Seed(TasDbContext context)
        {
            var d1 = new Driver { FullName = "ปริญญา นามสกุล", LicenseNo = "0320100", Address = "40 ถ.สุขุมวิท อ.บางละมุง จ.ชลบุรี", Phone = "0980460321" };
            var d2 = new Driver { FullName = "สมหมาย นามสกุล", LicenseNo = "0410200", Address = "12 ถ.สุขุมวิท อ.ศรีราชา จ.ชลบุรี", Phone = "0891234567" };
            var d3 = new Driver { FullName = "สมศักดิ์ นามสกุล", LicenseNo = "0550300", Address = "88 ถ.สุขาภิบาล อ.เมือง จ.ระยอง", Phone = "0812345678" };

            context.Drivers.Add(d1);
            context.Drivers.Add(d2);
            context.Drivers.Add(d3);

            context.Trucks.Add(new Truck { LicensePlate = "บพ 9999", Province = "ชลบุรี", WeightKg = 200, Driver = d1 });
            context.Trucks.Add(new Truck { LicensePlate = "อก 8989", Province = "ระยอง", WeightKg = 350, Driver = d2 });
            context.Trucks.Add(new Truck { LicensePlate = "ทบ 1302", Province = "ชลบุรี", WeightKg = 200, Driver = d3 });

            base.Seed(context);
        }
    }
}
