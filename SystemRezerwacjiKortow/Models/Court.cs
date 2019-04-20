using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.Models
{
    public class Court
    {
        [Display(Name = "CourtID", ResourceType = typeof(Texts))]
        public int CourtID { get; set; }

        [Display(Name = "CourtNumber", ResourceType = typeof(Texts))]
        public int CourtNumber { get; set; }

        [Display(Name = "SurfaceType", ResourceType = typeof(Texts))]
        [StringLength(50)]
        public string SurfaceType { get; set; }

        [Display(Name = "IsForDoubles", ResourceType = typeof(Texts))]
        [Range(0, 1)]
        public bool IsForDoubles { get; set; }

        [Display(Name = "IsCovered", ResourceType = typeof(Texts))]
        [Range(0, 1)]
        public bool IsCovered { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal PriceH { get; set; }

        [Display(Name = "CourtName", ResourceType = typeof(Texts))]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "PriceWinterRatio", ResourceType = typeof(Texts))]
       // [DataType(DataType.Currency)]
        public decimal PriceWinterRatio { get; set; }

        [Display(Name = "PriceWeekendRatio", ResourceType = typeof(Texts))]
       // [DataType(DataType.Currency)]
        public decimal PriceWeekendRatio { get; set; }

        //[Display(Name = "Price", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal PriceWinter { get; set; }

        //[Display(Name = "Price", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal PriceWinterWeekend { get; set; }

        //[Display(Name = "Price", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal PriceSummerWeekend { get; set; }

        //[Display(Name = "Price", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal PriceSummer { get; set; }

        // potrzebne do listy dostepnych kortow
        [Display(Name = "DateStart", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "DateEnd", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }
    }
}