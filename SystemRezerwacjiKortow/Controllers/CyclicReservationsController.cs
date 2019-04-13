using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.Database;

namespace SystemRezerwacjiKortow.Controllers
{
    public class CyclicReservationsController : Controller
    {
        // GET: CyclicReservations
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "Description,DayInterval,DateStart,DateStop,UserID,CourtID")] CyclicReservation cr, string howmany,int YourDropDownValue,string hourStart, string minuteStart,string hourStop,string minuteStop)
        {
            cr.UserID = SqlUser.GetUser(User.Identity.Name).UserID;
            DateTime start = new DateTime(cr.DateStart.Year, cr.DateStart.Month, cr.DateStart.Day, int.Parse(hourStart), int.Parse(minuteStart),0);
            DateTime stop = new DateTime(cr.DateStop.Year, cr.DateStop.Month, cr.DateStop.Day, int.Parse(hourStop), int.Parse(minuteStop), 0);
            bool check = SqlCyclicReservation.SetReservationCourtCyclic(cr.CourtID, cr.Description, start, stop,cr.UserID , int.Parse(howmany), YourDropDownValue, cr.DayInterval);
            if(check==false)
            {
                ViewBag.Message = SystemRezerwacjiKortow.Resources.Texts.CyclicReservationError;
            }
            else
            {
                return RedirectToAction("Profile", "User");
            }

            return View();
        }
    }
}