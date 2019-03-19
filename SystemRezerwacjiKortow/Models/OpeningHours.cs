using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.Models
{
    public class OpeningHours
    {
        [Display(Name = "DayOfWeekNumber", ResourceType = typeof(Texts))]
        public int DayOfWeek { get; set; }

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
    }
}