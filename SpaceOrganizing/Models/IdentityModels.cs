using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SpaceOrganizing.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public virtual ICollection<Registration> Registrations { get; set; }
        public IEnumerable<SelectListItem> AllRoles { get; set; }

        //un user poate avea mia multe taskuri asignate
        public virtual ICollection<Tasks> AsignedTasks { get; set; }

        //un user poate crea mai multe taskuri
        public virtual ICollection<Tasks> CreatedTasks { get; set; }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        // modele aplicatie 
        public DbSet<Tasks> Tasks { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Registration> Registrations { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}