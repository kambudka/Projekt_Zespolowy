using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Database;
using SystemRezerwacjiKortow.Models;
using SystemRezerwacjiKortow.ViewModels;

namespace SystemRezerwacjiKortow.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult NewChart()
        {
            List<object> iData = new List<object>();

            List<Hire> HireList = new List<Hire>();
            HireList = SqlHire.GetHires();

            DataTable dt = new DataTable();
            DataRow dr;
            int liczba;
            //Dodanie kolumny miesiąc
            dt.Columns.Add("Month", typeof(string));
            //ustawienie kolumny miesiac jako PrimaryKey, bez tego nie idzie Rows.Find
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = dt.Columns["Month"];
            dt.PrimaryKey = keyColumns;
            //Dodanie kolumny z liczba rezerwacji.
            dt.Columns.Add("Reservations", typeof(int));
            //Opcjionalne, posortowanie listy
            HireList.Sort((x, y) => DateTime.Compare(x.DateTo, y.DateTo));
            dr = dt.NewRow();
            foreach (Hire hire in HireList)
            {
                //Dla listy Hire jest liczona ilośc rezerwacji w danych miesiącu
                string month;
                switch (hire.DateTo.Month)
                {
                    case 3:
                        month = "Marzec";
                        break;
                    case 4:
                        month = "Kwiecien";
                        break;
                    default:
                        month = "Chujwie";
                        break;
                }
                //Sprawdzamy czy w tabeli jest już taki miesiąc
                DataRow result = dt.Rows.Find(month);
                if (result != null)
                {
                    //Jak znaleziono miesiąc w tabeli odczytujemy ilosc rezerwacji i dodajemy 1.
                    liczba = result.Field<int>("Reservations");
                    liczba += 1;
                    dr["Reservations"] = liczba; 
                }
                else {
                    //Jeżeli nie znaleziono miesiąca w tabeli, jest dodawany nowy wiersz
                    dr = dt.NewRow();
                    dr["Month"] = month;
                    dr["Reservations"] = 1 ;
                    dt.Rows.Add(dr);
                }
            }

            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        // GET: Statistics/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Statistics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Statistics/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Statistics/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Statistics/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Statistics/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Statistics/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
