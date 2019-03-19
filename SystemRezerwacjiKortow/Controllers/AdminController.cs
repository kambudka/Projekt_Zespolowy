using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.ViewModels;
namespace SystemRezerwacjiKortow.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
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