using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.ViewModels
{
    public class UserCustomerViewModel
    {

        public int UserID { get; set; }
        public int CustomerID { get; set; }

        [Display(Name = "CompanyName", ResourceType = typeof(Texts))]
        public string CompanyName { get; set; }

        [Display(Name = "City", ResourceType = typeof(Texts))]
        [StringLength(50)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterYourCity")]
        public string City { get; set; }

        [Display(Name = "Street", ResourceType = typeof(Texts))]
        [StringLength(50)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterYourStreet")]
        public string Street { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(Texts))]
        [StringLength(6)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterYourPostalCode")]
        public string ZipCode { get; set; }

        [Display(Name = "Discount", ResourceType = typeof(Texts))]
        //[DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        [Range(0, 100, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName="MaxDiscountValue")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "CanReserve", ResourceType = typeof(Texts))]
        public bool CanReserve { get; set; }


        [Display(Name = "Name", ResourceType = typeof(Texts))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterYourName")]
        public string FirstName { get; set; }

        [Display(Name = "Surname", ResourceType = typeof(Texts))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterYourSurname")]
        public string Surname { get; set; }

        [Display(Name = "DateOfBirth", ResourceType = typeof(Texts))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Texts), ErrorMessageResourceName = "EnterYourDateOfBirth")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DateOfBirth { get; set; }




    }
}