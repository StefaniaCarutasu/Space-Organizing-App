using Microsoft.AspNet.Identity;
using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class NotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Notifications
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.Notifications = (from notification in db.Notifications
                                     where notification.receivingUser == userId
                                     orderby notification.sentDate descending
                                     select notification).ToList();

            return View();

        }

        public ActionResult Show(int id)
        {
            Notification notification = db.Notifications.Find(id);
            notification.seen = true;
            db.SaveChanges();

            ApplicationUser sendingUser = db.Users.Find(notification.sendingUser);
            ViewBag.Name = sendingUser.FirstName + sendingUser.LastName;
            ViewBag.Message = notification.Message;
            ViewBag.Date = notification.sentDate;
            ViewBag.Notification = notification;

            return View();
        }
    }
}