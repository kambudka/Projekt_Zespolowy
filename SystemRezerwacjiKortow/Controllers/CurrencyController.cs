using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Controllers
{
    public class CurrencyController : Controller
    {
        private CurrencyService service = new CurrencyService();

        // GET: Currency
        public async Task<ActionResult> Index()
        {
            ViewBag.Currencies = await service.GetCurrenciesAsync();
            return View();
        }
    }

    public class CurrencyService {
        string url = "http://api.nbp.pl/api/exchangerates/tables/a/?format=json";

        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync(url);
                List <Currency> currenciesList = JsonConvert.DeserializeObject<List<Currency>>(json);
                return currenciesList;
            }
        }
    }
}