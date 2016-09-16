using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Trash_Collector.Models;

namespace Trash_Collector.Controllers
{
    public class CalendersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Calenders
        public ActionResult Index()
        {
            return View(db.calender.ToList());
        }

        // GET: Calenders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calender calender = db.calender.Find(id);
            if (calender == null)
            {
                return HttpNotFound();
            }
            return View(calender);
        }

        // GET: Calenders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Calenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Days")] Calender calender)
        {
            Calender tempCalender = new Calender();
            foreach (DateTime day in GenerateCalender(calender.Days))
            {
                tempCalender.Days = day;
                db.calender.Add(tempCalender);
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View(calender);
        }
        static List<DateTime> GenerateCalender(DateTime day)
        {
            List<DateTime> calenderList = new List<DateTime>();
            calenderList.Add(day);
            for (int i = 0; i < 729; i++)
            {
                day = day.AddDays(1);
                calenderList.Add(day);
            }
            return calenderList;
        }

        // GET: Calenders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calender calender = db.calender.Find(id);
            if (calender == null)
            {
                return HttpNotFound();
            }
            return View(calender);
        }

        // POST: Calenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Days")] Calender calender)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calender).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(calender);
        }

        // GET: Calenders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calender calender = db.calender.Find(id);
            if (calender == null)
            {
                return HttpNotFound();
            }
            return View(calender);
        }

        // POST: Calenders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Calender calender = db.calender.Find(id);
            db.calender.Remove(calender);
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
    }
}
