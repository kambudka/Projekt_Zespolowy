using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }

        [Display(Name = "UserID", ResourceType = typeof(Texts))]
        public int UserID { get; set; }

        [Display(Name = "DateStart", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "DateEnd", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateTo { get; set; }

        [Display(Name = "DuePayment", ResourceType = typeof(Texts))]
        [DataType(DataType.Currency)]
        public decimal Payment { get; set; }

        [Display(Name = "GearID", ResourceType = typeof(Texts))]
        public int GearID { get; set; }

        [Display(Name = "GearAmount", ResourceType = typeof(Texts))]
        public int GearAmount { get; set; }

        [Display(Name = "CourtID", ResourceType = typeof(Texts))]
        public int CourtID { get; set; }

        [Display(Name = "DateInsert", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOfInsert { get; set; }

        [Display(Name = "DateCancel", ResourceType = typeof(Texts))]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateOfCancel { get; set; }

        [Display(Name = "IsExecuted", ResourceType = typeof(Texts))]
        //[Range(0, 1)]
        public bool IsExecuted { get; set; }

        [Display(Name = "CyclicReservationID", ResourceType = typeof(Texts))]
        public int CyclicReservationID { get; set; }

        [Display(Name = "ContestID", ResourceType = typeof(Texts))]
        public int ContestID { get; set; }

        [Display(Name = "IsAccepted", ResourceType = typeof(Texts))]
        //[Range(0, 1)]
        public bool IsAccepted { get; set; }

        // // potrzebne do widoku
        [Display(Name = "ReservationName", ResourceType = typeof(Texts))]
        public string ReservationName { get; set; }  // odpowiednio albo nazwa rezerwowanego kortu albo sprzetu

        [Display(Name = "UserName", ResourceType = typeof(Texts))]
        public string UserName { get; set; }

        [Display(Name = "Amount", ResourceType = typeof(Texts))]
        public int Amount { get; set; }

        [Display(Name = "ContestName", ResourceType = typeof(Texts))]
        public string ContestName { get; set; }
    }
}