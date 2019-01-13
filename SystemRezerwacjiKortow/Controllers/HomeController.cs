using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            ViewBag.Status = true;
            SqlDatabase.init();  // ustawianie wartości początkowych zmiennych
            //SqlTesty.Testy();   // do testowania bazy danych

            // List<Customer> customers = SqlDatabase.GetCustomers();
            //List<User> users = SqlUser.GetUsers();
            // List<Court> courts = SqlCourt.GetCourts();
            // List<Gear> gears = SqlGear.GetGears();
           // DateTime startDate = DateTime.ParseExact("30.12.2018 10:00", "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
           // DateTime endDate = DateTime.ParseExact("31.12.2018 22:00", "dd.MM.yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
          //  decimal price = SqlCourt.GetCourtPrice(startDate, endDate, 2);
            List<OpeningHours> openingHours = SqlCompany.GetOpeningHours();
            return View(openingHours);
        }
      
      
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Complex = SqlCompany.GetComplex();
            ViewBag.ContactEmail = ConfigurationManager.AppSettings["EmailAddress"];
            ViewBag.OpeningHours = SqlCompany.GetOpeningHours();
            return View();
        }
      
    }
}