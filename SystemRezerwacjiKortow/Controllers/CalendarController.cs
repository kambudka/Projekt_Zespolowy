using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.Database;
using System.Runtime;
using System.Globalization;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                if (reservation.DateOfCancel == null && reservation.CourtID == id)
                {
                    newevent.id = reservation.ReservationID;
                    newevent.title = reservation.CourtID.ToString();
                    newevent.start = ConvertFromDateToString(reservation.DateFrom);

                    newevent.end = ConvertFromDateToString(reservation.DateTo);
                    newevent.description = reservation.ReservationID.ToString();
                    newevent.payment = reservation.Payment.ToString();
                    newevent.color = "#47f441";
                    if (reservation.ContestID != 0)
                    {
                        newevent.color = "#f4d142";
                    }
                    else if (reservation.CyclicReservationID != 0)
                    {
                        newevent.color = "#4265f4";
                    }

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
        public JsonResult SaveTournament(Event e, int court)
        {
            var status = false;
            //Wed Mar 19 2019 07:30:00 GMT+0000
            string format = "ddd MMM dd yyyy HH:mm:ss 'GMT'K";
            DateTime start = new DateTime();
            DateTime end = new DateTime();
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
            if (e.length > 1)
                end = start.AddDays(e.length - 1);
            else
                end = start;
            string email = User.Identity.Name;

            int id = SqlUser.GetUser(email).UserID;
            //SqlReservation.SetReservationCourt(1, DateTime.Now.AddMinutes(5), DateTime.Now.AddHours(1), 4);
            //SqlReservation.SetReservationCourt(1, new DateTime(2019, 3, 22, 16, 00, 0), new DateTime(2019, 3, 22, 17, 00, 0), 13);
            if (SqlContest.SetReservationContest(e.name, e.organizer, e.description, start, end, id))
                status = true;
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult SaveCyclicEvent(Event e, int court)
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

        public ActionResult GeneratePDF(string start, string end, int courtId)
        {
            string format = "ddd MMM dd yyyy HH:mm:ss";
            start = start.Replace(" GMT 0200 (czas środkowoeuropejski letni)", "");
            end = end.Replace(" GMT 0200 (czas środkowoeuropejski letni)", "");
            DateTime dateFrom = DateTime.ParseExact(start, format, CultureInfo.InvariantCulture);
            DateTime dateTo = DateTime.ParseExact(end, format, CultureInfo.InvariantCulture);
            List<Reservation> allReservations = SqlReservation.GetReservations(13).Where(r => r.DateFrom > dateFrom && r.DateTo < dateTo && r.CourtID == courtId).ToList();
            List<DateTime> datesToRaport = new List<DateTime>();
            DateTime startDate = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, dateFrom.Hour, dateFrom.Minute, 0);
            while(startDate.Month!=dateTo.Month || startDate.Day!=dateTo.Day)
            {
                datesToRaport.Add(startDate);
                startDate = startDate.AddDays(1);
            }
            string handle = Guid.NewGuid().ToString();
            return File(CreatePdf(allReservations, datesToRaport, dateFrom, dateTo, courtId).ToArray(), "application/pdf", "kort.pdf");
        }
       
        public MemoryStream CreatePdf(List<Reservation> reservations, List<DateTime>dates, DateTime dateFrom, DateTime dateTo, int courtId)
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("SamplePdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(PageSize.A4, 10, 10, 42, 35);
  


            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            PdfPTable tableLayout;
            for (int i = 0; i < dates.Count / 7; i++)
            {
                tableLayout = new PdfPTable(8);
                tableLayout.SpacingAfter = 52;
                doc.Add(Add_Content_To_PDF(tableLayout, reservations, dates.GetRange(i*7,7), dates[i*7], dates[i*7+6], courtId));
            }
 
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return workStream;

        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout,List<Reservation> reservations,List<DateTime>dates, DateTime dateFrom, DateTime dateTo, int courtId)
        {
            Random rnd = new Random();
            float[] headers = new float[8];
            headers[0] = 30;
            for (int i = 1; i < headers.Count(); i++)
                headers[i] = 40;
            tableLayout.SetWidths(headers); 
            tableLayout.WidthPercentage = 90; 
            tableLayout.HeaderRows = 1;
 
            tableLayout.AddCell(new PdfPCell(new Phrase("Zajetosc kortu "+SqlCourt.GetCourt(courtId).CourtNumber+" od "+dateFrom.ToString("dd.MM.yyyy")+" do "+dateTo.ToString("dd.MM.yyyy"), new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            AddCellToHeader(tableLayout, "Godzina");
            foreach(var date in dates)
            AddCellToHeader(tableLayout, date.ToString("dd.MM.yyyy"));
            List<DateTime> hours = new List<DateTime>();
            DateTime startDate = new DateTime(2019, 5, 21, 6,0,0);
            for (int i = 0; i < 17; i++)
            {
                hours.Add(startDate);
                startDate = startDate.AddHours(1);
            }
           List<OpeningHours> openningHours= SqlCompany.GetOpeningHours();
            List<List<int>> table = new List<List<int>>();
            Dictionary<int, int> colors = new Dictionary<int, int>() { };
            for (int i = 0; i < hours.Count() - 1; i++)
            {
                table.Add(new List<int>() { -1, -1, -1, -1, -1,-1,-1 });
                AddCellToBody(tableLayout, hours[i].ToString("hh:mmtt"),255,255,255);
                for (int j=0;j<dates.Count();j++)
                { var openTime = openningHours.Where(h => h.DayOfWeek == j + 1).ToList();
                    if (openTime.Count > 0)
                    {
                        if (openTime.First().TimeFrom.Hours > hours[i].Hour || openTime.First().TimeTo.Hours < hours[i].Hour)
                        {
                            AddCellToBody(tableLayout, "", 212, 212, 212);
                        }
                        else
                        {
                            var res = reservations.Where(r => r.DateFrom.Day == dates[j].Day && hours[i].Hour <= r.DateTo.Hour && r.DateFrom.Hour < hours[i + 1].Hour).ToList();
                            if (res.Count > 0)
                            {
                                table[i][j] = res.First().ReservationID;
                                if (!colors.ContainsKey(table[i][j]))
                                    colors.Add(table[i][j], rnd.Next(70, 255));
                                AddCellToBody(tableLayout, res.First().UserName, 187, colors[table[i][j]], 214);
                            }



                            else
                                AddCellToBody(tableLayout, "", 255, 255, 255);
                        }
                    }
                    else
                        AddCellToBody(tableLayout, "", 212, 212, 212);


                }
            }

            return tableLayout;
        }
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.YELLOW)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }

        private static void AddCellToBody(PdfPTable tableLayout, string cellText, int rColor, int gColor, int bColor)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
     {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(rColor, gColor, gColor)
    });
            
        }
    }
}