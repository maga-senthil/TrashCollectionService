using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Trash_Collector.Models
{
    public class Calender
    {
        [Key]
        public int Id { get; set; }
        public DateTime Days { get; set; }
        public virtual ICollection <Customer> PickUpSchedule { get; set; }
    }
}