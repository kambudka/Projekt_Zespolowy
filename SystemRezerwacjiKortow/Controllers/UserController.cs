using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;
using Microsoft.AspNet;
using SystemRezerwacjiKortow.ViewModels;
using AutoMapper;

namespace SystemRezerwacjiKortow.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";

            // Walidacja modelu uzytkownika 
            if (ModelState.IsValid)
            {

                // czy email istnieje
                bool isExist = SqlUser.CheckUserExists(user.Email);
                if (isExist)
                {
                    ViewBag.Message = Resources.Texts.EmailExistsMessage;
                    ModelState.AddModelError("EmailExist", Resources.Texts.EmailExistsMessage);
                    return View(user);
                }

                // generowanie kodu aktywacji
                user.ActivationCode = Guid.NewGuid().ToString();

                // hashing hasla
                user.Password = Crypto.Hash(user.Password);

                user.IsEmailVeryfied = false;
                user.RoleID = SqlDatabase.UserRoleId;  // domyślnie dodawany użytkownik o roli zwykłego user'a
                user.CustomerID = SqlDatabase.CustomerAtr;  // dodanie adresu użytkownika - atrapa, żeby baza przyjęła dane

                //zapis do bazy danych
                SqlUser.InsertUser(user);

                // wyslanie emaila do uzytkownika
                SendVerificationLinkEmail(user.Email, user.ActivationCode, user.FirstName);
                message = Resources.Texts.EmailVerificationMessage + user.Email;
                Status = true;
       
            }
            else
            {
                message = Resources.Texts.InvalidRequest;
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string firstName)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            string subject = Resources.Texts.EmailVerificationSubject + "!";
            string body = Resources.Texts.EmailVerificationBody +
                " <br/><a href='" + link + "'>" + link + "</a> ";
            Email.SendEmail(subject, body, email, firstName);
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            bool find = SqlUser.FindActivationCode(id);

            if (find)
            {
                SqlUser.UpdateUserEmailVeryfied(id);
                Status = true;
            }
            else
            {
                ViewBag.Message = Resources.Texts.InvalidRequest;
            }

            ViewBag.Status = Status;
            return View();
        }
        //Przypomnienie hasła
        [NonAction]
        public void SendRemindPasswordLink(string email, string activationCode, string firstName)
        {
            var verifyUrl = "/User/VerifyRemindPassword/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            string subject = "Przypomnienie hasła";

            string body = "Kliknij w poniższy link, aby kontynuować procedurę zmiany hasła.<br/>" +
                "Jeśli to nie ty wysyłałeś prośbę o przypomnienie hasła, zignoruj tego e-maila lub skontaktuj się z administratorem" +
                " <br/><a href='" + link + "'>" + link + "</a> ";

            Email.SendEmail(subject, body, email, firstName);
        }
        [HttpGet]
        public ActionResult VerifyRemindPassword(string id)
        {
            bool Status = false;
           

            if (id!=null)
            {

                Status = true;
            }
            else
            {
                ViewBag.Message = "Nieprawidłowe żądanie";
            }
            ViewBag.Code = id;
            ViewBag.Status = Status;
            return View();
        }

        [HttpPost]
        public ActionResult VerifyRemindPassword(string newPassword, string repeatPassword)
        {
            string code=null;
            string url = null;
            url = Request.Url.ToString();
            string[] collection = url.Split('/');
            bool Status = false;
            
            if ((newPassword == repeatPassword))
            {
                code = collection[collection.Length - 1];
                    Status = SqlUser.RemindPassword(Crypto.Hash(newPassword), code);
                
                
            }
            else
            {
                ViewBag.Message = "Nieprawidłowe żądanie ";
            }


            return RedirectToAction("Login");  // użytkownik niezalogowany nie mół być wylogowany
        }

        
        public ActionResult Sent(bool mode)
        {
            ViewBag.Message = mode;
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword, string repeatPassword)
        {
            string password = null;
            User tmp = SqlUser.GetUser(User.Identity.Name);
            password = SqlUser.GetUserPassword(tmp);

            if (Crypto.Hash(oldPassword) != password)
            {
                ViewBag.Message = "Stare hasło nie jest prawidłowe! ";
                return View();
            }
            else if(newPassword != repeatPassword)
            {
                ViewBag.Message = "Hasła nie są takie same! ";
                return View();

            }
            else if (newPassword == repeatPassword)
            {
                
                bool status = SqlUser.ChangePassword(Crypto.Hash(oldPassword), Crypto.Hash(newPassword), tmp.Email);
                return RedirectToAction("Logout");
            }
            else
            {
                ViewBag.Message = "Nie wiem jak to zrobiłeś/aś ale no nie pykło ";
                return View();
            }
           

        }

        [HttpGet]
        public ActionResult RemindPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemindPassword(string emailToSend)
        {

            bool Status = false;
            User tmp = SqlUser.GetUser(emailToSend);

            //string pass = SqlUser.GetUserPassword(tmp);
            if (tmp != null)
            {
                string activationCode = Guid.NewGuid().ToString();
                if(SqlUser.SaveUserActivationCode(emailToSend,activationCode))
                {
                    SendRemindPasswordLink(emailToSend, activationCode, tmp.FirstName);
                    Status = true; //wyświetli się w widoku komunikat o udanej próbie wysłania powiadomienia o haśle
                }
                
            }
            else
            {
                Status = false; //wyświetli się w widoku komunikat o błędzie
            }
            ViewBag.Message = Status;
            return RedirectToAction("Sent", new { mode = Status });
        }









        // do logowania
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Profile", "User");
            else
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            bool Status = false;
            string message = "";
            if (SqlUser.CheckUserExists(login.Email))
            {
                if (!SqlUser.CheckEmailVeryfied(login))
                {
                    ViewBag.Message = Resources.Texts.VerifyYourEmail;
                    return View();
                }
                if (string.Compare(Crypto.Hash(login.Password), SqlUser.GetUserPassword(login)) == 0)
                {
                    int timeout = login.RememberMe ? 525600 : 30; // 525600 min = 1 rok, 30 dni czas zycia cookiem
                    var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
                    {
                        Expires = DateTime.Now.AddMinutes(timeout),
                        HttpOnly = true
                    };
                    Response.Cookies.Add(cookie);
                    Status = true;

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        switch (SqlUser.GetUserRole(login.Email))
                        {
                            case "administrator":
                            return RedirectToAction("Index", "Admin");
                            //case "worker":
                            //return RedirectToAction("Profile", "User");
                            case "user":
                                return RedirectToAction("Profile", "User");
                        }
                    }
                }
                else
                {
                    message = Resources.Texts.InvalidPassword;
                }
            }
            else
            {
                message = Resources.Texts.InvalidUser;
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        // do wylogowania 
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            TempData["Logout"] = Resources.Texts.LogoutSuccessful;
            return RedirectToAction("Login", "User");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            Customer customer = SqlUser.GetCustomer(SqlUser.GetUser(User.Identity.Name));
            //Console.WriteLine(customer.CompanyName);
            ViewBag.user = SqlUser.GetUser(User.Identity.Name);
            return View(customer);
        }
       

        [Authorize]
        [HttpGet]
        public ActionResult EditProfile()
        {
            
            Customer customer = SqlUser.GetCustomer(SqlUser.GetUser(User.Identity.Name));
            User user = SqlUser.GetUser(User.Identity.Name);
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Customer, UserCustomerViewModel>());
            var config2 = new MapperConfiguration(cfg => cfg.CreateMap<User, UserCustomerViewModel>());
            var mapper = config.CreateMapper();

            UserCustomerViewModel model = mapper.Map<Customer, UserCustomerViewModel>(customer);
            mapper = config2.CreateMapper();
            mapper.Map<User, UserCustomerViewModel>(user, model);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserCustomerViewModel model)
        {
            User user = SqlUser.GetUser(User.Identity.Name);
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
                SqlUser.AddModyfyAddress(customer, user.Email);
                SqlUser.InsertUser(user);

            }
            else
            {
                return RedirectToAction("EditProfile", "User");
                
            }
            return RedirectToAction("Profile", "User");
        }

        public ActionResult DeleteAccount()
        {
            SqlUser.DeleteUser(SqlUser.GetUser(User.Identity.Name));
            FormsAuthentication.SignOut();
            TempData["Logout"] = Resources.Texts.LogoutSuccessful;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult GetCourts()
        {
            List<Reservation> myAllReservations = SqlReservation.GetReservations(SqlUser.GetUser(User.Identity.Name).UserID);
            List<Reservation> myCourts = new List<Reservation>();
            foreach (Reservation r in myAllReservations)
            {
                if (r.DateOfCancel == null && r.CourtID!=0)
                {
                    myCourts.Add(r);
                }
            }
            return View(myCourts);
           
        }
        public ActionResult GetGear()
        {
            List<Reservation> myAllReservations = SqlReservation.GetReservations(SqlUser.GetUser(User.Identity.Name).UserID);
            List<Reservation> myGear = new List<Reservation>();
            foreach (Reservation r in myAllReservations)
            {
                if (r.DateOfCancel == null && r.GearID!=0)
                {
                    myGear.Add(r);
                }
            }
            return View(myGear);

        }
        public ActionResult GetMyCourtsReservations()
        {
            List<Reservation> myAllReservations = SqlReservation.GetReservations(SqlUser.GetUser(User.Identity.Name).UserID);
            List<Reservation> myReservations=new List<Reservation>();
            foreach(Reservation r in myAllReservations)
            {
                if(r.DateOfCancel==null)
                {
                    myReservations.Add(r);
                }
            }
            return View(myReservations);
        }

        public ActionResult CancelReservation(int id)
        {
            Reservation reservation = SqlReservation.GetReservation(id);
            return View(reservation);
        }

        [HttpPost, ActionName("CancelReservation")]
        public ActionResult CancelConfirmed(int id)
        {
            bool check = SqlReservation.CancelReservation(id, SqlUser.GetUser(User.Identity.Name).UserID);


            return RedirectToAction("GetMyCourtsReservations");
        }

    }
}