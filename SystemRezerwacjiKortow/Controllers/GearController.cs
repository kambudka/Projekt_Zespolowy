using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Controllers
{
    public class GearController : Controller
    {
        List<Gear> list = SqlGear.GetGears();
        // GET: Gear
        public ActionResult Index()
        {
            return View(list);
        }
        public ActionResult PriceListGear()
        {
            ViewBag.Message = "Nasza oferta";
            List<Gear> listGear = SqlGear.GetGears();
            return View(listGear);
        }

        // GET: Gear/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Gear/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Gear/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "PriceH, Name, Amount")] Gear gear)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SqlGear.AddModifyGear(gear);
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View(gear);
            }
        }

        // GET: Gear/Edit/5
        public ActionResult Edit(int id)
        {
            Gear gear = SqlGear.GetGear(id);
            return View(gear);
        }

        // POST: Gear/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, [Bind(Include = "PriceH, Name, Amount")] Gear gear)
        {
            /*try
            {
                SqlGear.GetGear(id);

                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }*/
            try
            {
                gear.GearID = id;
                SqlGear.AddModifyGear(gear);
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View(gear);
            }
        }

        // GET: Gear/Delete/5
        public ActionResult Delete(int id)
        {
            Gear gear = SqlGear.GetGear(id);
            return View(gear);
        }

        // POST: Gear/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                Gear ToDelete = new Gear();
                ToDelete = SqlGear.GetGear(id);
                SqlGear.DeleteGear(ToDelete);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
