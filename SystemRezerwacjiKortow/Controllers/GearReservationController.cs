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
            Dictionary<string, decimal> tmp=null;
            if (Session["Currencies"]!=null)
            {
                tmp= (Dictionary<string, decimal>)Session["Currencies"];
            }
            
            
            if(tmp!=null)
            {
                for (int i = 0; i < allGear.Count; i++)
                {
                    allGear[i].PriceH = allGear[i].PriceH / tmp[Session["Currency"].ToString()]; ;
                }
            }
            DateTime dt = DateTime.Now;
            ViewBag.Year = dt.Year;
            ViewBag.Month = dt.Month;
            ViewBag.Day = dt.Day;
            if(Session["Currency"]==null)
            {
                Session["Currency"] = "PLN";
            }
            ViewBag.Code = Session["Currency"];
            return View(allGear);
        }
        public ActionResult GetAvailableGears(int year, int month, int day)
        {
            List<Gear> availableGear = new List<Gear>();
            DateTime now = DateTime.Now;
            DateTime dateTo = new DateTime(year, month, day);
            dateTo.AddHours(12);
            availableGear = SqlGear.GetAvailableGears(0, now, dateTo);
            
            Dictionary<string, decimal> tmp = null;
            if (Session["Currencies"] != null)
            {
                tmp = (Dictionary<string, decimal>)Session["Currencies"];
            }


            if (tmp != null)
            {
                for (int i = 0; i < availableGear.Count; i++)
                {
                    availableGear[i].PriceH = availableGear[i].PriceH / tmp[Session["Currency"].ToString()]; ;
                }
            }
            DateTime dt = DateTime.Now;
            ViewBag.Year = dt.Year;
            ViewBag.Month = dt.Month;
            ViewBag.Day = dt.Day;
            if (Session["Currency"] == null)
            {
                Session["Currency"] = "PLN";
            }
            ViewBag.Code = Session["Currency"];
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
                Dictionary<string, decimal> tmp = null;
                if (Session["Currencies"] != null)
                {
                    tmp = (Dictionary<string, decimal>)Session["Currencies"];
                }


                if (tmp != null)
                {
                    
                        gear.PriceH = gear.PriceH / tmp[Session["Currency"].ToString()]; ;
                    
                }
                DateTime dt = DateTime.Now;
                ViewBag.Year = dt.Year;
                ViewBag.Month = dt.Month;
                ViewBag.Day = dt.Day;
                if (Session["Currency"] == null)
                {
                    Session["Currency"] = "PLN";
                }
                ViewBag.Code = Session["Currency"];
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