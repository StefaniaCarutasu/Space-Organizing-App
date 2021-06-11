using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpaceOrganizing.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int GroupId { get; set; }

        public ApplicationUser sendingUser { get; set; }
        public ApplicationUser receivingUser { get; set; }

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public Boolean seen { get; set; }
    }
}