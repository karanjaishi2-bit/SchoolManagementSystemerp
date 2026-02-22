using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models
{
    public class BillingMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public Student? Student { get; set; }

        [Required]
        public DateTime BillDate { get; set; } = DateTime.Now;

        // Add this property
        [Required]
        public string BillingType { get; set; }

        public ICollection<BillingItem> BillingItems { get; set; } = new List<BillingItem>();
    }
}
