﻿using Newtonsoft.Json;
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
            List<Currency> currenciesList = await service.GetCurrenciesAsync();

            List<SelectListItem> listItems = new List<SelectListItem>();
            for (int i = 0; i < currenciesList.Count; i++)
            {
                listItems.Add(new SelectListItem { Value = currenciesList[i].code, Text = $"1 {currenciesList[i].currency} ({currenciesList[i].code}) = {currenciesList[i].mid} PLN" });
            }

            ViewBag.Currencies = listItems;
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
                List<Currencies> currenciesTable = JsonConvert.DeserializeObject<List<Currencies>>(json);
                List<Currency> currencies = currenciesTable[0].rates;
                currencies.Insert(0, new Currency { currency = "złoty", code="PLN", mid=1.0000M });      // Uzupełnienie listy z API o złotówki
                return currencies;
            }
        }
    }
}