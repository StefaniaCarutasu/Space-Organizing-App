using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpaceOrganizing.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public virtual Group Group { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}