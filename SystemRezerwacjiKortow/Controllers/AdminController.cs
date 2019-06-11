using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.AuthData;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.Resources;
using SystemRezerwacjiKortow.ViewModels;
namespace SystemRezerwacjiKortow.Controllers
{
    [AdminAuth]
    public class AdminController : Controller
    {
        // GET: Admin
        public List<Reservation> ReservationList;
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult UserRegistration()
        {
            return RedirectToAction("Registration", "User");
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

        //[HttpPost]
        public ActionResult ConfirmReservation(int id, bool accept=false)
        {
            ViewBag.MyErrorMessage = false;
            try
            {
                SqlReservation.AcceptReservation(id, accept);
                ViewBag.MyErrorMessage = true;
                SendCourtReservationMail(SqlReservation.GetReservation(id));
                return RedirectToAction("WaitingReservations");
            }
            catch
            {
                return RedirectToAction("WaitingReservations");
            }
        }

        public ActionResult CancelReservation(int id)
        {
            SqlReservation.CancelReservation(id, SqlUser.GetUser(User.Identity.Name).UserID);
            return RedirectToAction("WaitingReservations");
        }

        public ActionResult RedeemReservation(int id)
        {
            SqlReservation.MakePayment(id);
            return RedirectToAction("WaitingReservations");
        }

        public void SendCourtReservationMail(Reservation reservation)
        {
            var subject = Resources.Texts.ReservationConfirmation +" "+ reservation.ReservationID;
            var body = "<p><img src=\"https://student.labranet.jamk.fi/~H3188/wp_harjoitustyo/wordpress/wp-content/gallery-bank/gallery-uploads/o_1a68kjem61o38eikheljsibvhi.jpg\" width=\"600\" height=\"100\" /></p>"+
            "<p><font size=\"4\">"+Resources.Texts.ThanksForReservation+"</font></p>" +
                     "<p> &nbsp; &nbsp;"+Resources.Texts.IdTransaction+": <strong>"+reservation.ReservationID+ "</strong></p>"+
                        "<p> &nbsp; &nbsp;"+Resources.Texts.CourtNumber+": "+SqlCourt.GetCourt(reservation.CourtID).CourtNumber+"</p>"+
                           "<p> &nbsp; &nbsp;"+Resources.Texts.DateStart+": <strong>"+reservation.DateFrom.ToString()+"</strong></p>"+
                              "<p> &nbsp; &nbsp;"+Resources.Texts.DateEnd+": <strong>" + reservation.DateTo.ToString()+ "</strong></p>" +
                                 "<p> &nbsp; &nbsp; &nbsp;<strong> ul.Zwierzyniecka 45A 15 - 333, Bialystok </strong></p>"+
                                       "<p><span style = \"color: #999999;\">"+Resources.Texts.Reminder+" </span> &nbsp;<a href = \"#\" >twojekorty@gmail.com </a></p> ";

            var user = SqlUser.GetUserByID(reservation.UserID);

            Email.SendEmail(subject, body,user.Email,user.FirstName) ;
        }

        public ActionResult WaitingReservations()
        {
            ReservationList = new List<Reservation>();
            ReservationList = SqlReservation.GetReservations(13);
            var list = new List<Reservation>();
            foreach (Reservation reservation in ReservationList)
            {
                //Nie pobiera rezerwacji które są anulowane i zaakceptowane
                if (reservation.DateOfCancel == null && reservation.IsExecuted == false)
                {
                    list.Add(reservation);
                }
            }
            
            return View(list);

        }

        public ActionResult WaitingCourts()
        {
            ReservationList = new List<Reservation>();
            ReservationList = SqlReservation.GetReservations(13);
            var list = new List<Reservation>();
            foreach (Reservation reservation in ReservationList)
            {
                //Nie pobiera rezerwacji które są anulowane i zaakceptowane
                if (reservation.DateOfCancel == null && reservation.IsExecuted == false && reservation.CourtID!=0 && reservation.ContestID==0)
                {
                    list.Add(reservation);
                }
            }

            return View(list);
        }

        public ActionResult WaitingGear()
        {
            ReservationList = new List<Reservation>();
            ReservationList = SqlReservation.GetReservations(13);
            var list = new List<Reservation>();
            foreach (Reservation reservation in ReservationList)
            {
                //Nie pobiera rezerwacji które są anulowane i zaakceptowane
                if (reservation.DateOfCancel == null && reservation.IsExecuted == false && reservation.CourtID == 0 && reservation.ContestID == 0 && reservation.GearID!=0)
                {
                    list.Add(reservation);
                }
            }

            return View(list);
        }

        public ActionResult WaitingContests()
        {
            ReservationList = new List<Reservation>();
            ReservationList = SqlReservation.GetReservations(13);
            var list = new List<Reservation>();
            foreach (Reservation reservation in ReservationList)
            {
                //Nie pobiera rezerwacji które są anulowane i zaakceptowane
                if (reservation.DateOfCancel == null && reservation.IsExecuted == false &&  reservation.ContestID != 0)
                {
                    list.Add(reservation);
                }
            }

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
        public ActionResult EditHours( [Bind(Include = "DayOfWeek, TimeFrom, TimeTo")] OpeningHours openinghours)
        {
            try
            {
                SqlCompany.AddModifyOpeningHours(openinghours);

                return RedirectToAction("Complex");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AddHours()
        {
            var model = new NewOpeningHours();
            var openingHours = SqlCompany.GetOpeningHours();
            List<DayOfWeek> freeDays= Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Where(x=>!openingHours.Select(y => y.DayOfWeek).Contains((int)x+1)).ToList();

            ResourceManager rm = Texts.ResourceManager;
            string someString = rm.GetString("Sunday");
            model.FreeDays = freeDays.Select(x => new SelectListItem
            {
                Value = ((int)x+1).ToString(),
                Text = rm.GetString(Enum.GetName(typeof(DayOfWeek), x))
            });
            ViewBag.isNewHours = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult AddHours(NewOpeningHours model)
        {
            OpeningHours openingHours = new OpeningHours();
            openingHours.DayOfWeek = model.SelectedDay;
            openingHours.TimeFrom = model.TimeFrom;
            openingHours.TimeTo = model.TimeTo;
            SqlCompany.AddModifyOpeningHours(openingHours);
            return RedirectToAction("Complex");
        }

        public ActionResult DeleteUser(int id)
        {
            User user=SqlUser.GetUserByID(id);
            if (SqlUser.DeleteUser(user))
                ViewBag.deleteMessage = "Usunięto pomyślnie";
            else
                ViewBag.deleteMessage = "Nie udało się usunąć użytkownika";
            List<User> list = SqlUser.GetUsers();
            return View("Users",list);
        }

        public ActionResult EditUser(int id)
        {
            Customer customer = SqlUser.GetCustomer(SqlUser.GetUserByID(id));
            User user = SqlUser.GetUserByID(id);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Customer, UserCustomerViewModel>());
            var config2=new MapperConfiguration(cfg => cfg.CreateMap<User, UserCustomerViewModel>());
            var mapper = config.CreateMapper();
            
            UserCustomerViewModel model = mapper.Map<Customer,UserCustomerViewModel>(customer);
            mapper = config2.CreateMapper();
            mapper.Map<User,UserCustomerViewModel>(user, model);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(int id,UserCustomerViewModel model)
        {
            User user = SqlUser.GetUserByID(id);
            Customer customer = SqlUser.GetCustomer(user);
            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.Surname = model.Surname;
                user.DateOfBirth = model.DateOfBirth;
                customer.CompanyName = model.CompanyName;
                customer.City = model.City;
                customer.Street = model.Street;
                customer.ZipCode = model.ZipCode;
                customer.DiscountValue = model.DiscountValue;
                customer.CanReserve = model.CanReserve;
                SqlUser.AddModyfyAddress(customer, user.Email);
                SqlUser.InsertUser(user);
            }
            return RedirectToAction("Users");
        }

        public ActionResult DeleteHours(OpeningHours openingHours)
        {
            openingHours.TimeFrom = openingHours.TimeTo;
            SqlCompany.AddModifyOpeningHours(openingHours);
            return RedirectToAction("Complex");
        }

    }
}