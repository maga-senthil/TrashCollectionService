using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Trash_Collector.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string state { get; set; }
        public int ZipCode { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PickUpDay { get; set; }
       
        //public string password { get; set; }

        public virtual ICollection <Calender> PickUpDates { get; set; }

        [ForeignKey("ApplicationUsers")]
        //public string Register_Id { get; set; }
        public string Email { get; set; }
        public ApplicationUser ApplicationUsers { get; set; }
      
    }
}