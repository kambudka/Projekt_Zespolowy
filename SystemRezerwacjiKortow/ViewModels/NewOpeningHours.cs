using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.ViewModels
{
    public class NewOpeningHours
    {
        [Display(Name = "TimeFrom", ResourceType = typeof(Texts))]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan TimeFrom { get; set; }

        [Display(Name = "TimeTo", ResourceType = typeof(Texts))]
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan TimeTo { get; set; }

        [Display(Name = "DayOfWeek", ResourceType = typeof(Texts))]
        public string DayName { get; set; }

        [Display(Name ="ChooseDay", ResourceType =typeof(Texts))]
        public IEnumerable<SelectListItem> FreeDays { get; set; }

        public int SelectedDay { get; set; }
    }
}