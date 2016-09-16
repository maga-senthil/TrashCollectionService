using System;
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
        //public ActionResult Create([Bind(Include = "Id,FirstName,LastName,StreetAddress,City,state,ZipCode,PickUpDay,Email")] Customer customer)
        //{
        //    List<Calender> calendar = new List<Calender>();
        //    foreach (DateTime day in GeneratePickupSchedule(customer.PickUpDay))
        //    {
        //        calendar.Add(new Calender() { Days = day });
        //    }
        //    customer.PickUpDates = calendar;

        //    if (ModelState.IsValid)
        //    {
        //        db.Customers.Add(customer);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");

        //    }

        //    ViewBag.Email = new SelectList(db.Users, "Id", "Email", customer.Email);
        //    return View(customer);
        //}
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,StreetAddress,City,state,ZipCode,PickUpDay,Email")] Customer customer)
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
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            ViewBag.Email = new SelectList(db.Users, "Id", "Email", customer.Email);
            return View(customer);
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
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //public ActionResult PickUpSchedule()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PickUpSchedule(int zipcode)
        //{
        //       var customers = db.Customers.Where(x => x.ZipCode == zipcode).ToList();
        //       return View("ResultView", customers);
        //}
        public ActionResult PickUpSchedule()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PickUpSchedule(int zipcode,DateTime pickupday)
        {
            var customers = db.Customers.Where(x => x.ZipCode == zipcode && x.PickUpDay == pickupday).ToList();
            return View("ResultView", customers);
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
                return RedirectToAction("Index");
            }
            return View();
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
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
