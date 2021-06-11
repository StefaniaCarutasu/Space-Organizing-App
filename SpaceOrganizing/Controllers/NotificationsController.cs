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
        public void GetAllNotifications()
        {
            string userId = User.Identity.GetUserId();
            ViewBag.Notifications = (from notification in db.Notifications
                                     where notification.receivingUser.Id == userId
                                     select notification).ToList();

        }

        public ActionResult Show(int id)
        {
            Notification notification = (Notification)(from notif in db.Notifications
                                        where notif.NotificationId == id
                                        select notif);
            Group group = (Group)(from gr in db.Groups
                                  where gr.GroupId == notification.GroupId
                                  select gr);
            notification.seen = true;
            db.SaveChanges();
            ViewBag.SendingUser = notification.sendingUser;
            ViewBag.NotificationMessage = notification.Message;
            ViewBag.Group = group;

            return View();
        }
    }
}