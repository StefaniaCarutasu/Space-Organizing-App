using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        public void GetAllNotifications()
        {
            string userId = User.Identity.GetUserId();

        }
    }
}