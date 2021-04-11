using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ARS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ARS.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult ManageTransactions()
        {
            return View(db.Transaction.ToList());
        }

        public ActionResult ManageUsers()
        {
            var users = db.Users.ToList();

            return View(users);
        }

        public ActionResult ManageFlights()
        {
            return View(db.Flights.ToList());
        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,price,type,status,createdAt,updatedAt")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transaction.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(transaction);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,price,type,status,createdAt,updatedAt")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transaction.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transaction.Find(id);
            db.Transaction.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DataChart()
        {
            return View();
        }

        public ActionResult Users()
        {
            var UsersContext = new ApplicationDbContext();
            var users = UsersContext.Users.ToList();

            return View(users);
        }

        public ActionResult CreateFlight(Flight flight)
        {
            var newFlight = db.Flights.Add(flight);
            var flightId = newFlight.id;
            var seatStatus = (int) SeatStatus.Available;
            for (int i = 1; i < 5; i++)
            {
                int classType = (int)Models.SeatType.FirstClass;
                if (i > 2)
                {
                    classType = (int)Models.SeatType.ClubClass;
                }

                for (int j = 1; j < 7; j++)
                {
                    string position = i.ToString() + Convert.ToChar(j + 64);
                    db.Seats.Add(new Models.Seat
                    {
                        status = seatStatus,
                        classType = classType,
                        position = position,
                        flightId = flightId
                    });
                }
            }
            for (int i = 5; i < 19; i++)
            {

                int classType = (int)Models.SeatType.NonSmoking;
                if (i > 7)
                {
                    classType = (int)Models.SeatType.Bussiness;
                }
                if (i > 13)
                {
                    classType = (int)Models.SeatType.Smoking;
                }
                for (int j = 1; j < 7; j++)
                {
                    string position = i.ToString() + Convert.ToChar(j + 64);
                    db.Seats.Add(new Models.Seat
                    {
                        status = seatStatus,
                        classType = classType,
                        position = position,
                        flightId = flightId
                    });
                }
            }

            return RedirectToAction("ManageFlights");
        }

        public ActionResult UpdateUser(string id , ApplicationUser user)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = userManager.FindById(id);
            currentUser.lastName = user.lastName;
            currentUser.firstName = user.firstName;
            currentUser.address = user.address;
            currentUser.userIdentityCode = user.userIdentityCode;   
            currentUser.Email = user.Email;
            currentUser.PhoneNumber = user.PhoneNumber;
            currentUser.preferedCreditCardNumber = user.preferedCreditCardNumber;
            userManager.Update(currentUser);
            context.SaveChanges();

            return Json(new {success = true });
        }

        public JsonResult DataChart1(DateTime start, DateTime end)
        {
            var newEnd = end.AddDays(1);
            var data = db.Transaction.Where(x => x.createdAt >= start && x.createdAt < newEnd).Select(z => new { 
            price = z.price,
            date = z.createdAt
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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
