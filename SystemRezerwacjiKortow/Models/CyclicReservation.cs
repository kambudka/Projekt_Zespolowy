using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.Models
{
    public class CyclicReservation
    {
        public int CyclicReservationID { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Texts))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterDescription")]
        public string Description { get; set; }

        [Display(Name = "DayOfWeek", ResourceType = typeof(Texts))]  // numer dnia tygodnia, 1 - niedziela, 7 - sobota
        [Range(1, 7)]
        public int DayOfWeek { get; set; }

        [Display(Name = "DayOfMonth", ResourceType = typeof(Texts))]
        [Range(1, 31)]
        public int DayOfMonth { get; set; }

        [Display(Name = "DayInterval", ResourceType = typeof(Texts))]
        public int DayInterval { get; set; }

        [Display(Name = "Hour", ResourceType = typeof(Texts))]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public TimeSpan Time { get; set; }

        [Display(Name = "DateStart", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateStart { get; set; }

        [Display(Name = "DateEnd", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateStop { get; set; }

        [Display(Name = "UserID", ResourceType = typeof(Texts))]
        public int UserID { get; set; }

        [Display(Name = "GearID", ResourceType = typeof(Texts))]
        public int GearID { get; set; }

        [Display(Name = "GearAmount", ResourceType = typeof(Texts))]
        public int GearAmount { get; set; }

        [Display(Name = "CourtID", ResourceType = typeof(Texts))]
        public int CourtID { get; set; }

        [Display(Name = "DateCancel", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateCancel { get; set; }

        // potrzebne do widoku
        [Display(Name = "ReservationName", ResourceType = typeof(Texts))]
        public string ReservationName { get; set; }  // odpowiednio albo nazwa rezerwowanego kortu albo sprzetu

        [Display(Name = "UserName", ResourceType = typeof(Texts))]
        public string UserName { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Texts))]
        public int Amount { get; set; }
    }
}