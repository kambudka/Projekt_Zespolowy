using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.Models
{
    public class Gear
    {
        public int GearID { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        //[Required(ErrorMessage = "Cena jest wymagana")]
        public decimal PriceH { get; set; }

        [Display(Name = "GearName", ResourceType = typeof(Texts))]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa jest wymagana")]
        public string Name { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Texts))]
        public int Amount { get; set; }  // ilosc posiadanego przez kompleks sprzetu

        // potrzebne do listy dostepnego sprzetu
        [Display(Name = "DateStart", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "DateEnd", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }

        [Display(Name = "AmountAvailable", ResourceType = typeof(Texts))]
        public int AmountAvailable { get; set; }  // ilosc dostepnego sprzetu w danej godzinie
    }
}