using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.Database;
using System.Net;

namespace SystemRezerwacjiKortow.Controllers
{
    public class ContestsController : Controller
    {

        List<Contest> list = SqlContest.GetContests(0); // UserID 0 zwraca wszystkie turnieje

        // GET: Contest
        public ActionResult Index()
        {
            return View(list);
        }

        // GET: Contest/Details/5

        public ActionResult Details(int id)
        {
            Contest contest = SqlContest.GetContest(id);
            Dictionary<string, decimal> tmp = null;
            if (Session["Currencies"] != null)
            {
                tmp = (Dictionary<string, decimal>)Session["Currencies"];
            }


            if (tmp != null)
            {
               
                    contest.PaymentToPay = contest.PaymentToPay / tmp[Session["Currency"].ToString()]; ;
                
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
            return View(contest);
        }


    }
}