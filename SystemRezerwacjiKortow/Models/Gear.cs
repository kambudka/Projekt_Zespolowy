﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SystemRezerwacjiKortow.Models
{
    public class Gear
    {
        public int GearID { get; set; }

        [Display(Name = "Cena za godzine")]
        [DataType(DataType.Currency)]
        //[Required(ErrorMessage = "Cena jest wymagana")]
        public decimal PriceH { get; set; }

        [Display(Name = "Nazwa")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa jest wymagana")]
        public string Name { get; set; }

        [Display(Name = "Ilość")]
        public int Amount { get; set; }
    }
}