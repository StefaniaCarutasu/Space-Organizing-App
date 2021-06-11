using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Titlul task-ului este obligatoriu.")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractre.")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string UserId { get; set; }
        public string UserId2 { get; set; }

        [Required(ErrorMessage = "Deadline-ul este obligatoriu.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        public int GroupId { get; set; }

        public bool Done { get; set; }

        public string Priority { get; set; }

        // prioritatea posibila a unui task
        public IEnumerable<SelectListItem> PriorityLabel { get; set; }

        // lista useri pentru dropdown
        public IEnumerable<SelectListItem> UsersList { get; set; }

        // foreign key
        // un task apartine unei echipe
        public virtual Group Group { get; set; }

        // este asignat unui utilizator 
        public virtual ApplicationUser User2 { get; set; }

        //un task este creat de catre un utilizator
        public virtual ApplicationUser User { get; set; }
    }
}