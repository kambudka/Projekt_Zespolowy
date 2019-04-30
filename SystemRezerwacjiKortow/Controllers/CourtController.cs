using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.ComponentModel;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Controllers
{
    public class CourtController : Controller
    {
        List<Court> list = SqlCourt.GetCourts();
        // GET: Court
        public ActionResult Index()
        {
            return View(list);
        }

        public ActionResult PriceList()
        {
            return View(list);
        }
        public ActionResult PriceListWinter()
        {
            return View(list);
        }


        // GET: Court/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
       
        // GET: Court/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Court/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "CourtNumber, SurfaceType, IsForDoubles, IsCovered, PriceH, Name")] Court court)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlCourt.AddModifyCourt(court);
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(court);
            }
        }

        // GET: Court/Edit/5
        public ActionResult Edit(int id)
        {
           Court court = SqlCourt.GetCourt(id);
            return View(court);
        }

        // POST: Court/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, [Bind(Include = "CourtNumber, SurfaceType, IsForDoubles, IsCovered, PriceH, Name, PriceWinterRatio, PriceWeekendRatio")] Court court)
        {
            try
            {
                court.CourtID = id;
                SqlCourt.AddModifyCourt(court);
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(court);
            }
        }

        // GET: Court/Delete/5
        public ActionResult Delete(int id)
        {
            Court court = SqlCourt.GetCourt(id);
            return View(court);
        }

        // POST: Court/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Court ToDelete = new Court();
                ToDelete = SqlCourt.GetCourt(id);
                SqlCourt.DeleteCourt(ToDelete);
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search()
        {           
            return View();
        }


        [HttpPost]
        public ActionResult Search( [Bind(Include = "CourtNumber, SurfaceType, IsForDoubles, IsCovered, PriceH, Name, PriceWinterRatio, PriceWeekendRatio,DateFrom, DateTo,CourtID")] Court court)
        {
            List<Court> searchedCourts = SqlCourt.GetAvailableCourts(court.CourtID, court.DateFrom, court.DateTo);
            return View("SearchResult", searchedCourts);         
        }

  


    }
}
