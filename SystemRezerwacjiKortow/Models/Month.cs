using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;
using System.ComponentModel.DataAnnotations;

namespace SystemRezerwacjiKortow.Models
{
    public class Month
    {
        public int MonthID { get; set; }

        [Display(Name = "MonthName", ResourceType = typeof(Texts))]
        public string MonthName { get; set; }

        [Display(Name = "MonthYear", ResourceType = typeof(Texts))]
        public int MonthYear { get; set; }

        [Display(Name = "TotalHireCount", ResourceType = typeof(Texts))]
        public int TotalHireCount { get; set; }

        [Display(Name = "TotalHireRevenue", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal TotalHireRevenue { get; set; }

        [Display(Name = "TotalGearCount", ResourceType = typeof(Texts))]
        public int? TotalGearCount { get; set; }

        [Display(Name = "DifferentUsers", ResourceType = typeof(Texts))]
        public int DifferenUsers { get; set; }

    }
}