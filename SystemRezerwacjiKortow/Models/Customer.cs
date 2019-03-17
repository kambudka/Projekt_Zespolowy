using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Resources;

namespace SystemRezerwacjiKortow.Models
{
    public class Customer
    {
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
        [DataType(DataType.Currency)]
        public decimal DiscountValue { get; set; }

        [Display(Name = "CanReserve", ResourceType = typeof(Texts))]
        public bool CanReserve { get; set; }
    }
}