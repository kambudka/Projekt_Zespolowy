using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult Users()
        {
            List<User> list = SqlUser.GetUsers();
            return View(list);

        }

        public ActionResult Contest()
        {
            var list = new List<Contest>();
            return View(list);

        }

        public ActionResult Advertisements()
        {

            var list = new List<Advertisement>();
            return View(list);

        }

        public ActionResult Complex()
        {
            List<OpeningHours> list = SqlCompany.GetOpeningHours();
            return View(list);

        }
        public ActionResult EditHours(int dayOfWeek)
        {
            OpeningHours op = SqlCompany.GetOpeningHour(dayOfWeek);
            return View(op);
        }

        // POST: Gear/Edit/5
        [HttpPost]
        public ActionResult EditHours(int id, [Bind(Include = "DayOfWeek, TimeFrom, TimeTo")] OpeningHours openinghours)
        {
            try
            {
                SqlCompany.AddModifyOpeningHours(openinghours);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}