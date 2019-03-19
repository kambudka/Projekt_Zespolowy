using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.Database;
using System.Runtime;
using System.Globalization;

namespace SystemRezerwacjiKortow.Controllers
{
    public class CalendarController : Controller
    {
        public List<Reservation> ReservationList;
        // GET: Calendar
        public ActionResult Index()
        {

            return View();
        }

        public int GetUserID()
        {
            return 0;
        }

        public JsonResult GetEvents(string paramstart, string paramend)
        {
            ReservationList = new List<Reservation>();
            ReservationList = SqlReservation.GetReservations(13);
            List<Event> list = new List<Event>();
            Event newevent;
            DateTime datetime = DateTime.Now;

            foreach (Reservation reservation in ReservationList)
            {
                newevent = new Event();
                //newevent.id = reservation.ReservationID;
                newevent.title = reservation.CourtID.ToString();
                newevent.start = ConvertFromDateToString(reservation.DateFrom);
                newevent.end = ConvertFromDateToString(reservation.DateTo);
                list.Add(newevent);
            }

            var events = list.ToArray();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetDayColor(string datetoconvert)
        {
            int color = 4;
            DateTime day = new DateTime();
            string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'K";
            try
            {
                day = DateTime.ParseExact(datetoconvert, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            }
            try
            {
                color = SqlReservation.GetReservationStateCourt(1, day, 0);
            }
            catch
            {

            }

            return new JsonResult { Data = color, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            var status = false;
              /*  if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                }
                else
                {
                    dc.Events.Add(e);
                }
                dc.SaveChanges();*/
                status = true;
            return new JsonResult { Data = new { status = status } };
        }

        public static string ConvertFromDateToString(DateTime timestamp)
        {
            string result = timestamp.ToString("yyyy-MM-ddTHH:mm:ss");
            return result;
        }
    }
}