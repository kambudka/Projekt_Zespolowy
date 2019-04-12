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
        public List<OpeningHours> HoursList;
        public List<Event> list = new List<Event>();
        // GET: Calendar
        public ActionResult Index()
        {

            return View();
        }

        public int GetUserID()
        {
            return 0;
        }

        public JsonResult GetEvents(string paramstart, string paramend, int court)
        {
            ReservationList = new List<Reservation>();
            ReservationList = SqlReservation.GetReservations(13);
            //list = new List<Event>();
            Event newevent;
            DateTime datetime = DateTime.Now;
            int id = court;

            foreach (Reservation reservation in ReservationList)
            {
                newevent = new Event();
                //Nie pobiera rezerwacji które są anulowane
                if (reservation.DateOfCancel == null && reservation.CourtID == id) {
                newevent.id = reservation.ReservationID;
                newevent.title = reservation.CourtID.ToString();
                newevent.start = ConvertFromDateToString(reservation.DateFrom);
                newevent.end = ConvertFromDateToString(reservation.DateTo);
                newevent.description = reservation.ReservationID.ToString();
                newevent.payment = reservation.Payment.ToString();
                //newevent
                list.Add(newevent);
                }
            }
            var events = list.ToArray();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpGet]
        public JsonResult GetHours(string paramstart, string paramend)
        {
            HoursList = new List<OpeningHours>();
            HoursList = SqlCompany.GetOpeningHours();
            //list = new List<Event>();

            var hours = HoursList.ToArray();
            return new JsonResult { Data = hours, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetColor(int courtid, string datejs)
        {
            string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'K";
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(datejs, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            }
            int color = SqlReservation.GetReservationStateCourt(courtid, date, 0);
            return new JsonResult { Data = color, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e, int court)
        {
            var status = false;
                           //Wed Mar 19 2019 07:30:00 GMT+0000
            string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'K";
            DateTime start = new DateTime();
            DateTime end = new DateTime();
            DateTime startnow = DateTime.Now;
            DateTime endnow = DateTime.Now.AddHours(1);
            try
            {
                start = DateTime.ParseExact(e.start, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            }
            try
            {
                end = DateTime.ParseExact(e.end, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
            }

            string email = User.Identity.Name;

            int id = SqlUser.GetUser(email).UserID;
            //SqlReservation.SetReservationCourt(1, DateTime.Now.AddMinutes(5), DateTime.Now.AddHours(1), 4);
            //SqlReservation.SetReservationCourt(1, new DateTime(2019, 3, 22, 16, 00, 0), new DateTime(2019, 3, 22, 17, 00, 0), 13);
            if (SqlReservation.SetReservationCourt(court, start.AddHours(-2), end.AddHours(-2), id))
                status = true;
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            string email = User.Identity.Name;

            int id = SqlUser.GetUser(email).UserID;

            if (SqlReservation.CancelReservation(eventID, id))
                status = true;

            return new JsonResult { Data = new { status = status } };
        }



        public static string ConvertFromDateToString(DateTime timestamp)
        {
            string result = timestamp.ToString("yyyy-MM-ddTHH:mm:ss");
            return result;
        }
        public static string ConvertFromStringToDate(DateTime timestamp)
        {
            string result = timestamp.ToString("yyyy-MM-ddTHH:mm:ss");
            return result;
        }
    }
}