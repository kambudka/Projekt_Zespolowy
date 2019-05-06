using Rotativa;
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
       public List<Month> Months = new List<Month>();
       public List<Hire> HireList = new List<Hire>();
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthList()
        {
            HireList = SqlHire.GetHires();
            Month newmonth;
            HireList.Sort((x, y) => DateTime.Compare(x.DateTo, y.DateTo));
            foreach (Hire hire in HireList)
            {
                //Dla listy Hire jest liczona ilośc rezerwacji w danych miesiącu
                string month;
                switch (hire.DateTo.Month)
                {
                    case 1:
                        month = "Styczen";
                        break;
                    case 2:
                        month = "Luty";
                        break;
                    case 3:
                        month = "Marzec";
                        break;
                    case 4:
                        month = "Kwiecien";
                        break;
                    case 5:
                        month = "Maj";
                        break;
                    case 6:
                        month = "Czewiec";
                        break;
                    case 7:
                        month = "Lipiec";
                        break;
                    case 8:
                        month = "Sierpien";
                        break;
                    case 9:
                        month = "Wrzesien";
                        break;
                    case 10:
                        month = "Pazdziernik";
                        break;
                    case 11:
                        month = "Listopad";
                        break;
                    case 12:
                        month = "Grudzien";
                        break;
                    default:
                        month = "Błąd";
                        break;
                }

                //Sprawdzamy czy w tabeli jest już taki miesiąc
                Month result = Months.Find(x => x.MonthName.Contains(month));
                if (result != null)
                {
                    //Jak znaleziono miesiąc w liscie odczytujemy wartosci i dodajemy
                    result.TotalHireCount++;
                    result.TotalGearCount += hire.GearAmount;
                    result.TotalHireRevenue += hire.Payment;

                }
                else
                {
                    //Jeżeli nie znaleziono miesiąca w tabeli, jest dodawany nowy wpis
                    newmonth = new Month();
                    newmonth.MonthYear = hire.DateFrom.Year;
                    newmonth.MonthName = month;
                    newmonth.TotalHireCount = 1;
                    newmonth.TotalHireRevenue = hire.Payment;
                    newmonth.TotalGearCount = hire.GearAmount;
                    //newmonth.DifferenUsers = 1;
                    Months.Add(newmonth);
                }
            
            }

            return View(Months);
        }

        public ActionResult MakePDF(Month month2)
        {
            Month PDFMonth = new Month();
            return View(month2);
        }

        public ActionResult PrintPDF(Month month)
        {
            var report = new ActionAsPdf("MakePDF", month);
            return report;
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
                    case 1:
                        month = "Styczen";
                        break;
                    case 2:
                        month = "Luty";
                        break;
                    case 3:
                        month = "Marzec";
                        break;
                    case 4:
                        month = "Kwiecien";
                        break;
                    case 5:
                        month = "Maj";
                        break;
                    case 6:
                        month = "Czewiec";
                        break;
                    case 7:
                        month = "Lipiec";
                        break;
                    case 8:
                        month = "Sierpien";
                        break;
                    case 9:
                        month = "Wrzesien";
                        break;
                    case 10:
                        month = "Pazdziernik";
                        break;
                    case 11:
                        month = "Listopad";
                        break;
                    case 12:
                        month = "Grudzien";
                        break;
                    default:
                        month = "Błąd";
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
        public ActionResult Details(string name, int year)
        {
            Month month = Months.Find(x => x.MonthName.Contains(name));
            return View(month);
        }

    }
}
