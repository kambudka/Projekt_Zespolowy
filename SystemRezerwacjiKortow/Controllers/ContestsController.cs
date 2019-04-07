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
            return View(contest);
        }


    }
}