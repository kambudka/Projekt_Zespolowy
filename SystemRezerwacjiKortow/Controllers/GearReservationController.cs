using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.Database;

namespace SystemRezerwacjiKortow.Controllers
{
    public class GearReservationController : Controller
    {
        // GET: GearReservation
        public ActionResult Index()
        {
            List<Gear> allGear = new List<Gear>();
            allGear = SqlGear.GetGears();
           /* DateTime a = new DateTime(2019, 1, 1);
            DateTime dateTo = DateTime.Now;
            dateTo.AddDays(14);
            availableGear = SqlGear.GetAvailableGears(0,a,DateTime.Now);*/
            return View(allGear);
        }
        public ActionResult GetAvailableGears(int year, int month, int day)
        {
            List<Gear> availableGear = new List<Gear>();
            DateTime now = DateTime.Now;
            DateTime dateTo = new DateTime(year, month, day);
            dateTo.AddHours(12);
            availableGear = SqlGear.GetAvailableGears(0, now, dateTo);
            return View(availableGear);
        }
        public ActionResult Reserve(int id)
        {
            if(String.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                Gear gear = SqlGear.GetGear(id);
                return View(gear);
            } 
        }
        public ActionResult ReserveDone([Bind(Include = "GearID,DateFrom,DateTo")] Gear gear, int amount,int fromHour, int toHour)
        {
            User tmp = SqlUser.GetUser(User.Identity.Name);
            DateTime from = new DateTime(gear.DateFrom.Year,gear.DateFrom.Month,gear.DateFrom.Day,fromHour,0,0);
           
            DateTime to = new DateTime(gear.DateTo.Year, gear.DateTo.Month, gear.DateTo.Day, toHour, 0, 0);
            
            bool check = SqlReservation.SetReservationGear(gear.GearID, amount, from, to, tmp.UserID);
            //return RedirectToAction("Index");
            if(check)
            {
                return RedirectToAction("GetMyCourtsReservations","User");
            }
            else
            {
                ViewBag.mes = SystemRezerwacjiKortow.Resources.Texts.CyclicReservationError;
            }
            return View();
        }

    }
}