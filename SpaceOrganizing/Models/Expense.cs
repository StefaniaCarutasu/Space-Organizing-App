using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpaceOrganizing.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pretul este obligatirou.")]
        public int Price { get; set; }

        public bool Paid { get; set; }

        public int GroupId { get; set; }

        public string UserId { get; set; }

        // foreign key
        // grupul din care apartine
        public virtual Group Group { get; set; }

        // userul care a platit
        public virtual ApplicationUser User { get; set;  }
    }
}