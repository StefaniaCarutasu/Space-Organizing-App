﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class Tasks
{
	public Tasks()
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
        public string PriorityLabel { get; set; }

        [Required(ErrorMessage = "Deadline-ul este obligatorie.")]
        public DateTime Deadline { get; set; }
        public int TeamId { get; set; }

        //statusul posibil al unui task
        public IEnumerable<SelectListItem> PriorityLabel { get; set; }

        //foreign key
        //un task apartine unei echipe
        public virtual Group Group { get; set; }

        internal static Task FromResult(int v)
        {
            throw new NotImplementedException();
        }

        //este asignat unui membru
        public virtual ApplicationUser User2 { get; set; }

        //un task este creat de catre un organizator
        public virtual ApplicationUser User { get; set; }
    }
}
