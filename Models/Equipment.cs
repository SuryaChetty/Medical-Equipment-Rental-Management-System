using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Medical_Equipment_Rental_Management_System.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string EquipmentName { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, 100000)]
        public decimal PricePerDay { get; set; }

        [Required]
        [Range(0, 10000)]
        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

    }
}