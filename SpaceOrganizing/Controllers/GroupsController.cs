using Microsoft.AspNet.Identity;
using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Group = SpaceOrganizing.Models.Group;

namespace SpaceOrganizing.Controllers
{
    public class GroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [NonAction]
        private IEnumerable<SelectListItem> GetAllUsers(int groupId)
        {
            var UsersList = new List<SelectListItem>();
            var users = from user in db.Users
                        join reg in db.Registrations on user.Id equals reg.UserId
                        where reg.GroupId == groupId
                        select user;

            UsersList.Add(new SelectListItem
            {
                Value = null,
                Text = "None"
            });

            foreach (var user in users)
            {
                UsersList.Add(new SelectListItem
                {
                    Value = user.Id,
                    Text = user.UserName
                });
            }

            return UsersList;
        }

        // GET: Groups
        [Authorize(Roles = "User, Administrator")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            IQueryable<Group> groups = null;
            ViewBag.User = user;
            if (User.IsInRole("Administrator"))
            {
                groups = from gr in db.Groups
                         select gr;
            }
            else
            {
                List<int> OwnGroups = db.Registrations.Where(r => r.UserId == user.Id).Select(reg => reg.GroupId).ToList();

                groups = from gr in db.Groups
                         where OwnGroups.Contains(gr.GroupId)
                         select gr;
            }
            var users = from usr in db.Users
                        orderby usr.UserName
                        select usr;
            

            var search = "";
            if (Request.Params["searchDashboard"] != null)
            {
                search = Request.Params["searchDashboard"].ToString().Trim();
                List<string> userIds = db.Users.Where(us => us.UserName.Contains(search)).Select(u => u.Id).ToList();
                users = (IOrderedQueryable<ApplicationUser>)db.Users.Where(usr => userIds.Contains(usr.Id));
                ViewBag.CountUsers = users.Count();

                List<int> groupIds = db.Groups.Where(gr => gr.GroupName.Contains(search) || gr.GroupDescription.Contains(search)).Select(g => g.GroupId).ToList();
                groups = (IOrderedQueryable<Group>)db.Groups.Where(gr => groupIds.Contains(gr.GroupId));
            }
            else
            {
                ViewBag.CountUsers = 0;
            }


            ViewBag.UsersList = users;
            ViewBag.Groups = groups;

            return View();
        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult Show(int id)
        {
            Group group = db.Groups.Find(id);
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.UsersCount = group.Registrations.Count();
            ViewBag.User = user;
            ViewBag.isAdmin = false;
            ViewBag.isGroupOwner = false;
            ViewBag.UserId = User.Identity.GetUserId();
           
            if (User.IsInRole("Administrator"))
            {
                ViewBag.isAdmin = true;
            }
            if (group.UserId == User.Identity.GetUserId())
            {
                ViewBag.isGroupOwner = true;
            }
            ViewBag.isInGroup = false;
            foreach(var reg in group.Registrations)
            {
                if(reg.UserId == User.Identity.GetUserId())
                {
                    ViewBag.isInGroup = true;
                }
            }
            ViewBag.Group = group;
            bool acc = true;
            List<Tasks> taskuriDone = (from task in db.Tasks
                             where task.Done == true && task.GroupId == id
                             select task).ToList();
            ViewBag.countDone = taskuriDone.Count;
            List<Tasks> lowP = new List<Tasks>();
            List<Tasks> highP = new List<Tasks>();
            List<Tasks> medP = new List<Tasks>();
            foreach (var task in taskuriDone)
            {
                if (task.Priority == "Urgent" && task.Done == false)
                {
                    highP.Add(task);
                }
                else if (task.Priority =="Medium" && task.Done==false)
                {
                    medP.Add(task);
                }
                else if (task.Priority == "Low" && task.Done == false)
                {
                    lowP.Add(task);
                }
            }
            ViewBag.lowP = lowP;
            ViewBag.highP = highP;
            ViewBag.medP = medP;
            ViewBag.UsersList = GetAllUsers(id);
            return View(group);
        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult New()
        {
            Group group = new Group();
            group.UserId = User.Identity.GetUserId();
            return View(group);
        }

        [HttpPost]
        [Authorize(Roles = "User, Administrator")]
        public ActionResult New(Group gr)
        {
            gr.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Groups.Add(gr);
                    Registration reg = new Registration();
                    reg.UserId = User.Identity.GetUserId();
                    reg.GroupId = gr.GroupId;
                    reg.Date = DateTime.Now;
                    ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                    user.Registrations.Add(reg);
                    db.SaveChanges();
                    gr.Registrations.Add(reg);
                    db.SaveChanges();
                    return Redirect("/Groups/Show/" + @gr.GroupId);
                }
                else
                {
                    return View(gr);
                }

            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult Edit(int id)
        {
            Group gr = db.Groups.Find(id);
            if (gr.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(gr);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui grup care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "User, Administrator")]
        public ActionResult Edit(int id, Group requestGroup)
        {
            try
            {
                Group gr = db.Groups.Find(id);
                if (TryUpdateModel(gr))
                {
                    gr.GroupName = requestGroup.GroupName;
                    gr.GroupDescription = requestGroup.GroupDescription;
                    db.SaveChanges();
                    return Redirect("/Groups/Show/" + @gr.GroupId); ;
                }
                return View(requestGroup);
            }
            catch (Exception e)
            {
                return View(requestGroup);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "User, Administrator")]
        public ActionResult Delete(int id)
        {
            Group gr = db.Groups.Find(id);
            if (gr.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                db.Groups.Remove(gr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un grup care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        public ActionResult GroupNotification(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            Group group = db.Groups.Find(id);
            ApplicationUser groupAdmin = db.Users.Find(group.UserId);
            Notification notification = new Notification();
            notification.GroupId = id;
            notification.receivingUser = groupAdmin;
            notification.sendingUser = user;
            notification.Message = user.UserName + " wants to join " + group.GroupName;
            groupAdmin.Notifications.Add(notification);
            db.SaveChanges();
            ViewBag.Accepted = false;
            return Redirect("/Groups/Show/" + @group.GroupId);

        }

        [Authorize(Roles = "User, Administrator")]
        public ActionResult NewMember(int id)
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            Registration reg = new Registration();
            reg.GroupId = id;
            reg.UserId = User.Identity.GetUserId();
            reg.Date = DateTime.Now;
            Group group = db.Groups.Find(id);
            group.Registrations.Add(reg);
            user.Registrations.Add(reg);
            db.SaveChanges();

            return Redirect("/Groups/Show/" + @group.GroupId);
        }

        [HttpDelete]
        [Authorize(Roles = "User, Administrator")]

        public ActionResult LeaveGroup(int id)
        {
            Registration reg = db.Registrations.Find(id);
            db.Registrations.Remove(reg);
            db.SaveChanges();

            return Redirect("/Groups/Index");
        }

    }
}