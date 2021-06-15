using Microsoft.AspNet.Identity;
using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class ProfilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private void searchedUsers()
        {
            int count = 0;
            ViewBag.Count = count;
            var users = from usr in db.Users
                        orderby usr.UserName
                        select usr;
            var search = "";
            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<string> userIds = db.Users.Where(
                    us => us.UserName.Contains(search)).Select(u => u.Id).ToList();
                users = (IOrderedQueryable<ApplicationUser>)db.Users.Where(usr => userIds.Contains(usr.Id));
                ViewBag.CountUsers = users.Count();
            }
            else
            {
                ViewBag.CountUsers = 0;
            }

            ViewBag.UsersList = users;
        }

        private void GetAllNotifications()
        {
            string id = User.Identity.GetUserId();
            ViewBag.Notifications = (from notif in db.Notifications
                                     where notif.receivingUser == id
                                     orderby notif.sentDate descending
                                     select notif).ToList();
            var unread = (from notif in db.Notifications
                          where notif.receivingUser == id && notif.seen == false
                          select notif).Count();
            ViewBag.Unread = 0;
            if (unread != null)
            {
                ViewBag.Unread = unread;
            }
        }

        // GET: Profiles
        //Afisarea profilului utilizatorului logat curent
        [Authorize(Roles = "Administrator,User")]
        public ActionResult Index()
        {
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            ViewBag.Groups = (from gr in db.Groups
                              join reg in db.Registrations on gr.GroupId equals reg.GroupId
                              where reg.UserId == id
                              select gr).ToList();
            ViewBag.ProfileDescription = user.ProfileDescription;
            ViewBag.UserId = id;
            ViewBag.User = user;
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            ViewBag.Picture = user.ProfilePicture;
            string currentId = User.Identity.GetUserId();

            System.DateTime moment = System.DateTime.Now;
            int month = moment.Month;
            int day = moment.Day;
            DateTime userBirtday = user.BirthDate;
            if (day == userBirtday.Day && month == userBirtday.Month)
            {
                ViewBag.Birthday = 1;
            }
            else
            {
                ViewBag.Birthday = 0;
            }

            searchedUsers();
            GetAllNotifications();

            return View();
        }

        //Daca user ul curent logat este sau nu administrator de grup
        private bool isGroupAdmin()
        {
            string id = User.Identity.GetUserId();
            var groups = (from gr in db.Groups
                          where gr.UserId == id
                          select gr).ToList();
            if(groups.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Vizualizeaza profilul unui utilizator
        [Authorize(Roles = "Administrator,User")]
        public ActionResult Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            ViewBag.Profile = from profile in db.Profiles
                              where profile.UserId == id
                              select profile;
            ViewBag.User = user;
            ViewBag.Picture = user.ProfilePicture;
            ViewBag.ProfileDescription = user.ProfileDescription;
            ViewBag.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            string currentId = User.Identity.GetUserId();
            ViewBag.UserId = currentId;
            System.DateTime moment = System.DateTime.Now;
            int month = moment.Month;
            int day = moment.Day;
            DateTime userBirtday = user.BirthDate;
            if (day == userBirtday.Day && month == userBirtday.Month)
            {
                ViewBag.Birthday = 1;
            }
            else
            {
                ViewBag.Birthday = 0;
            }

            ViewBag.GroupAdmin = isGroupAdmin();

            searchedUsers();
            GetAllNotifications();

            return View(user);
        }

        [Authorize(Roles = "Administrator,User")]
        public ActionResult Edit()
        {
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            return View(user);
        }

        [HttpPut]
        [Authorize(Roles = "Administrator,User")]
        public ActionResult Edit(ApplicationUser requestUser, FormCollection fc, HttpPostedFileBase file)
        {
            string id = User.Identity.GetUserId();
            try
            {
                ApplicationUser user = db.Users.Find(id);
                if (TryUpdateModel(user))
                {
                    user.UserName = requestUser.UserName;
                    user.BirthDate = requestUser.BirthDate;
                    user.PhoneNumber = requestUser.PhoneNumber;
                    user.ProfileDescription = requestUser.ProfileDescription;
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost editat cu succes";
                    return RedirectToAction("Index");

                }
                return View(requestUser);
            }
            catch (Exception e)
            {
                return View(requestUser);
            }
        }

        [Authorize(Roles = "Administrator,User")]
        public ActionResult NewProfilePicture()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,User")]
        public ActionResult NewProfilePicture(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string id = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Find(id);
                user.ProfilePicture = true;
                db.SaveChanges();
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/profilePictures"), id + ".jpeg");

                // file is uploaded
                file.SaveAs(path);

            }
           

            // after successfully uploading redirect the user
            return RedirectToAction("Index", "Profiles");
        }


        public ActionResult InviteToGroup(string userId, int groupId)
        {
            
            ApplicationUser groupAdmin = db.Users.Find(User.Identity.GetUserId());
            Notification invite = new Notification();

            invite.GroupId = groupId;
            invite.sendingUser = groupAdmin.Id;
            invite.receivingUser = userId;
            invite.sentDate = DateTime.Now;
            invite.seen = false;
            invite.Message = "Hei, ";



            return View();
        }

    }
}