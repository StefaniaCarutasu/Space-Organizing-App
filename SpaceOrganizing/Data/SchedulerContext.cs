using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SpaceOrganizing.Data
{
    public partial class DefaultConnection : DbContext
    {
        public DefaultConnection() : base("name=DefaultConnection") { }
        public virtual DbSet<Event> Events { get; set; }
    }
}
