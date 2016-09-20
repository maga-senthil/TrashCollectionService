using System;
using GoogleMaps.LocationServices;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Trash_Collector.Models
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.ApplicationUsers);
            return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.Email = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,StreetAddress,City,state,ZipCode,PickUpDay,Email")] Customer customer)
        {
            string latlng = GetCoordinates(customer.StreetAddress, customer.City);
            List<string> splitLatLng = latlng.Split(',').ToList();
            string latitude = splitLatLng[0];
            string longitude = splitLatLng[1];
            customer.Latitude = latitude;
            customer.Longitude = longitude;

            List<Calender> calendar = new List<Calender>();
            foreach (DateTime day in GeneratePickupSchedule(customer.PickUpDay))
            {
                var tempPickupDay = db.calender.Where(x => x.Days == day).Single();
                calendar.Add(tempPickupDay);
            }
            customer.PickUpDates = calendar;
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Details", customer);

            }

            ViewBag.Email = new SelectList(db.Users, "Id", "Email", customer.Email);
            return View(customer);
        }
        public static string GetCoordinates(string streetAddress, string city)
        {
            var address = streetAddress + city;
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);

            var latitude = point.Latitude;
            var longitude = point.Longitude;
            string latlng = latitude.ToString().Trim() + "," + longitude.ToString().Trim();
            return latlng;
        }
        static List<DateTime> GeneratePickupSchedule(DateTime pickupSchedule)
        {
            List<DateTime> pickupScheduleList = new List<DateTime>();
            pickupScheduleList.Add(pickupSchedule);
            for (int i = 0; i < 51; i++)
            {
                pickupSchedule = pickupSchedule.AddDays(7);
                pickupScheduleList.Add(pickupSchedule);
            }
            return pickupScheduleList;
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = new SelectList(db.Users, "Id", "Email", customer.Email);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,StreetAddress,City,state,ZipCode,PickUpDay,Email")] Customer customer)
        {
            List<Calender> calendar = new List<Calender>();
            foreach (DateTime day in GeneratePickupSchedule(customer.PickUpDay))
            {
                var tempPickupDay = db.calender.Where(x => x.Days == day).Single();
                calendar.Add(tempPickupDay);
            }
            customer.PickUpDates = calendar;
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Email = new SelectList(db.Users, "Id", "Email", customer.Email);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult PickUpSchedule()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PickUpSchedule(int zipcode, DateTime pickupday)
        {
            var tempPickupSchedule = db.calender.Where(x => x.Days == pickupday).Single().PickUpSchedule.Where(x => x.ZipCode == zipcode).ToList();
            var tempLatitudeList = tempPickupSchedule.Select(x => x.Latitude).ToList();
            double latitudeAvg = 0;
            foreach (string lat in tempLatitudeList) { double latitude = double.Parse(lat); latitudeAvg += latitude; }
            string latAvg = (latitudeAvg / tempLatitudeList.Count).ToString();
            var tempLongitudeList = tempPickupSchedule.Select(x => x.Longitude).ToList();
            double longitudeAvg = 0;
            foreach (string lat in tempLongitudeList) { double longitude = double.Parse(lat); longitudeAvg += longitude; }
            string lngAvg = (longitudeAvg / tempLongitudeList.Count).ToString();
            var tempStreetAddressList = tempPickupSchedule.Select(x => x.StreetAddress).ToList();
            List<string> mapCoordinateList = new List<string>();
            var mapCoordinates = mapCoordinateList;
            for (int i = 0; i < (tempPickupSchedule.Count); i++)
            {
                string mapInfo = null;
                if (i < (tempPickupSchedule.Count - 1))
                {
                    if (tempPickupSchedule.Count > 1 && i == 0) { mapCoordinates.Add("["); }
                    mapInfo = "{ " + "\"Id\": " + (i + 1) + ", " + "\"PlaceName\": " + "\"" + tempPickupSchedule[i].City + "\"" + ", " + "\"GeoLat\": " + "\"" + tempPickupSchedule[i].Latitude + "\"" + ", " + "\"GeoLong\": " + "\"" + tempPickupSchedule[i].Longitude + "\"" + " }" + ", ";
                }
                if (i == (tempPickupSchedule.Count - 1))
                {
                    mapInfo = "{ " + "\"Id\": " + (i + 1) + ", " + "\"PlaceName\": " + "\"" + tempPickupSchedule[i].City + "\"" + ", " + "\"GeoLat\": " + "\"" + tempPickupSchedule[i].Latitude + "\"" + ", " + "\"GeoLong\": " + "\"" + tempPickupSchedule[i].Longitude + "\"" + " } ";
                }
                mapCoordinateList.Add(mapInfo.Trim());
                if (tempPickupSchedule.Count > 1 && i == (tempPickupSchedule.Count - 1)) { mapCoordinates.Add("]"); }
            }
            return View("ResultView", tempPickupSchedule);
        }
        public ActionResult RemovePickupDay(int? id)
        {
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        [HttpPost]
        public ActionResult RemovePickupDay(int? id, DateTime pickupday)
        {
            Customer customer = db.Customers.Find(id);
            var tempPickupDay = db.calender.Where(x => x.Days == pickupday).Single();
            customer.PickUpDates.Remove(tempPickupDay);
            db.SaveChanges();
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Details", customer);
            }
            return View(customer);
        }
        public ActionResult AddPickupDay(int? id)
        {
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        [HttpPost]
        public ActionResult AddPickupDay(int? id, DateTime pickupday)
        {
            Customer customer = db.Customers.Find(id);
            var tempPickupDay = db.calender.Where(x => x.Days == pickupday).Single();
            customer.PickUpDates.Add(tempPickupDay);
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Details", customer);
            }
            return View(customer);
        }
        public ActionResult ChangePickupSchedule(int? id)
        {
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        [HttpPost]
        public ActionResult ChangePickupSchedule(int? id, DateTime pickupDay)
        {
            Customer customer = db.Customers.Find(id);
            DateTime oldPickupDate = customer.PickUpDay;
            List<Calender> newPickupDatesList = new List<Calender>();
            var calendarDates = customer.PickUpDates.Where(x => x.Days.CompareTo(pickupDay.AddDays(-7)) < 0).ToList();
            foreach (Calender day in calendarDates)
            {
                newPickupDatesList.Add(day);
            }
            int newDatesCount = newPickupDatesList.Count;
            for (int i = 0; i < (52 - newDatesCount); i++)
            {
                var tempPickupDates = db.calender.Where(x => x.Days == pickupDay).Single();
                newPickupDatesList.Add(tempPickupDates);
                pickupDay = pickupDay.AddDays(7);
            }
            customer.PickUpDates = newPickupDatesList;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Details", customer);
            }
            return View(customer);
        }
        public ActionResult CalculateBill(int? id)
        {
            Customer customer = db.Customers.Find(id);
            string displayBill = "";
            customer.Bill = displayBill;
            db.SaveChanges();
            return View(customer);
        }

        [HttpPost]
        public ActionResult CalculateBill(int? id, string bill)
        {
            Customer customer = db.Customers.Find(id);
            //int billMonth = pickupDay.Month;
            var tempPickupDay = customer.PickUpDates.Where(x => x.Days.Month == MonthInt(bill)).ToList();
            int tempCount = tempPickupDay.Count;
            int tempYear = 1942;
            if (tempCount > 0) { tempYear = tempPickupDay[0].Days.Year; }
            string year = tempYear.ToString();
            if (year == "1942") { year = ""; }
            double billCalculate = (tempCount * 7.33);
            string displayBill = "Payment for " + MonthString(MonthInt(bill)) + " " + year + ": $" + billCalculate.ToString();
            customer.Bill = displayBill;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
            }
            return RedirectToAction("Details", customer);
        }
        static int MonthInt (string billMonth)
        {
            int billMonthNumber = 0;
            billMonth = billMonth.Trim();
            if (billMonth == null) { billMonth = "1"; }
            if (billMonth.ToLower() == "january" || billMonth.ToLower() == "jan" || billMonth == "1") { billMonthNumber = 1; }
            if (billMonth.ToLower() == "february" || billMonth.ToLower() == "feb" || billMonth == "2") { billMonthNumber = 2; }
            if (billMonth.ToLower() == "march" || billMonth.ToLower() == "mar" || billMonth == "3") { billMonthNumber = 3; }
            if (billMonth.ToLower() == "april" || billMonth.ToLower() == "apr" || billMonth == "4") { billMonthNumber = 4; }
            if (billMonth.ToLower() == "may" || billMonth == "5") { billMonthNumber = 5; }
            if (billMonth.ToLower() == "june" || billMonth.ToLower() == "jun" || billMonth == "6") { billMonthNumber = 6; }
            if (billMonth.ToLower() == "july" || billMonth.ToLower() == "jul" || billMonth == "7") { billMonthNumber = 7; }
            if (billMonth.ToLower() == "august" || billMonth.ToLower() == "aug" || billMonth == "8") { billMonthNumber = 8; }
            if (billMonth.ToLower() == "september" || billMonth.ToLower() == "sep" || billMonth == "9") { billMonthNumber = 9; }
            if (billMonth.ToLower() == "october" || billMonth.ToLower() == "oct" || billMonth == "10") { billMonthNumber = 10; }
            if (billMonth.ToLower() == "november" || billMonth.ToLower() == "nov" || billMonth == "11") { billMonthNumber = 11; }
            if (billMonth.ToLower() == "december" || billMonth.ToLower() == "dec" || billMonth == "12") { billMonthNumber = 12; }
            return billMonthNumber;
        }
        static string MonthString(int billMonthNumber)
        {
            string billMonth = " ";
            if (billMonthNumber == 1) { billMonth = "January"; }
            if (billMonthNumber == 2) { billMonth = "February"; }
            if (billMonthNumber == 3) { billMonth = "March"; }
            if (billMonthNumber == 4) { billMonth = "April"; }
            if (billMonthNumber == 5) { billMonth = "May"; }
            if (billMonthNumber == 6) { billMonth = "June"; }
            if (billMonthNumber == 7) { billMonth = "July"; }
            if (billMonthNumber == 8) { billMonth = "August"; }
            if (billMonthNumber == 9) { billMonth = "September"; }
            if (billMonthNumber == 10) { billMonth = "October"; }
            if (billMonthNumber == 11) { billMonth = "November"; }
            if (billMonthNumber == 12) { billMonth = "December"; }
            return billMonth;
        }
        public ActionResult MapDisplay(int? id)
        {
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        [HttpPost]
        public ActionResult MapDisplay(int? id, string coordinates)
        {
            Customer customer = db.Customers.Find(id);
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                return RedirectToAction("Details", customer);
            }
            return View(customer);
        }
        public ActionResult GetCustomerSchedule(int? id)
        {
            Customer customer = db.Customers.Find(id);
            var tempPickupSchedule = customer.PickUpDates.Select(x => x).ToList();
            return View(tempPickupSchedule);
        }
    }
}
