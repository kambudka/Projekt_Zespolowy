using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

        public ActionResult MonthRevenue()
        {
            return View();
        }


        [HttpGet]
        public JsonResult NewChartMoney()
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
            dt.Columns.Add("Revenue", typeof(int));
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
                    liczba = result.Field<int>("Revenue");
                    liczba += Decimal.ToInt32( hire.Payment);
                    dr["Revenue"] = liczba;
                }
                else
                {
                    //Jeżeli nie znaleziono miesiąca w tabeli, jest dodawany nowy wiersz
                    dr = dt.NewRow();
                    dr["Month"] = month;
                    dr["Revenue"] = Decimal.ToInt32(hire.Payment);
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

        public ActionResult MonthGear()
        {
            return View();
        }

        [HttpGet]
        public JsonResult NewChartGear()
        {
            List<object> iData = new List<object>();

            List<Hire> HireList = new List<Hire>();
            HireList = SqlHire.GetHires();

            DataTable dt = new DataTable();
            DataRow dr;
            int? liczba;
            //Dodanie kolumny miesiąc
            dt.Columns.Add("Month", typeof(string));
            //ustawienie kolumny miesiac jako PrimaryKey, bez tego nie idzie Rows.Find
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = dt.Columns["Month"];
            dt.PrimaryKey = keyColumns;
            //Dodanie kolumny z liczba rezerwacji.
            dt.Columns.Add("Gear", typeof(int));
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
                    liczba = result.Field<int>("Gear");
                    liczba += hire.GearAmount;
                    dr["Gear"] = liczba;
                }
                else
                {
                    //Jeżeli nie znaleziono miesiąca w tabeli, jest dodawany nowy wiersz
                    dr = dt.NewRow();
                    dr["Month"] = month;
                    dr["Gear"] = hire.GearAmount;
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



        [HttpGet]
        public JsonResult DailyChart(string month)
        {
            List<object> iData = new List<object>();

            List<Hire> HireList = new List<Hire>();
            HireList = SqlHire.GetHires();

            DataTable dt = new DataTable();
            DataRow dr;
            int liczba;
            //Dodanie kolumny miesiąc
            dt.Columns.Add("Day", typeof(int));
            //ustawienie kolumny day jako PrimaryKey, bez tego nie idzie Rows.Find
            DataColumn[] keyColumns = new DataColumn[1];
            keyColumns[0] = dt.Columns["Day"];
            dt.PrimaryKey = keyColumns;
            //Dodanie kolumny z liczba rezerwacji.
            dt.Columns.Add("Reservation", typeof(int));
            //Opcjionalne, posortowanie listy
            HireList.Sort((x, y) => DateTime.Compare(x.DateTo, y.DateTo));
            //Wypelniamy tabele dniami
            dr = dt.NewRow();

            //for (int i = 1; i < 31; i++)
            //{
            //    dr = dt.NewRow();
            //    dr["Day"] = i;
            //    dr["Reservation"] = 1;
            //    dt.Rows.Add(dr);
            //}
            foreach (Hire hire in HireList)
            {
                //Dla listy Hire jest liczona ilośc rezerwacji w danych miesiącu
                string monthstring;
                switch (hire.DateTo.Month)
                {
                    case 1:
                        monthstring = "Styczen";
                        break;
                    case 2:
                        monthstring = "Luty";
                        break;
                    case 3:
                        monthstring = "Marzec";
                        break;
                    case 4:
                        monthstring = "Kwiecien";
                        break;
                    case 5:
                        monthstring = "Maj";
                        break;
                    case 6:
                        monthstring = "Czewiec";
                        break;
                    case 7:
                        monthstring = "Lipiec";
                        break;
                    case 8:
                        monthstring = "Sierpien";
                        break;
                    case 9:
                        monthstring = "Wrzesien";
                        break;
                    case 10:
                        monthstring = "Pazdziernik";
                        break;
                    case 11:
                        monthstring = "Listopad";
                        break;
                    case 12:
                        monthstring = "Grudzien";
                        break;
                    default:
                        monthstring = "Błąd";
                        break;
                }

                //Sprawdzamy miesiac 
                if (month.Remove(0,10) == monthstring)
                {
                    DataRow result = dt.Rows.Find(hire.DateFromAsDayOfMonth);
                    int day = hire.DateFromAsDayOfMonth;
                    if (result != null)
                    {
                        //Jak hire pasuje do miesiaca to dodajemy jedna rezerwację
                        liczba = result.Field<int>("Reservation");
                        liczba += 1;
                        dr["Reservation"] = liczba;
                    }
                    else
                    {
                        //Jeżeli nie znaleziono miesiąca w tabeli, jest dodawany nowy wiersz
                        dr = dt.NewRow();
                        dr["Day"] = hire.DateFromAsDayOfMonth;
                        dr["Reservation"] = 1;
                        dt.Rows.Add(dr);
                    }
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

        public ActionResult DownloadHighChartHtml()
        {
            string serverPath = Server.MapPath("~/phantomjs/");
            string filename = DateTime.Now.ToString("ddMMyyyy_hhmmss") + ".pdf";
            string Url = "http://stagebelweb.azurewebsites.net/race/16867";

            new Thread(new ParameterizedThreadStart(x =>
            {
                ExecuteCommand(string.Format("cd {0} & E: & phantomjs rasterize.js {1} {2} \"A4\"", serverPath, Url, filename));
                //E: is the drive for server.mappath
            })).Start();

            var filePath = Path.Combine(Server.MapPath("~/phantomjs/"), filename);

            var stream = new MemoryStream();
            byte[] bytes = DoWhile(filePath);

            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Image.pdf");
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.End();
            return RedirectToAction("PrintPDF");
        }

        private void ExecuteCommand(string Command)
        {
            try
            {
                ProcessStartInfo ProcessInfo;
                Process Process;

                ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + Command);

                ProcessInfo.CreateNoWindow = true;
                ProcessInfo.UseShellExecute = false;

                Process = Process.Start(ProcessInfo);
            }
            catch { }
        }

        private byte[] DoWhile(string filePath)
        {
            byte[] bytes = new byte[0];
            bool fail = true;

            while (fail)
            {
                try
                {
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                    }

                    fail = false;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            System.IO.File.Delete(filePath);
            return bytes;
        }




    }
}
