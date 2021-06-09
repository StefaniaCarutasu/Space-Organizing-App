﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SpaceOrganizing.Models;

namespace SpaceOrganizing.Controllers
{
    public class TaskssController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        [NonAction]
        private void SetAccessRights(Tasks Task)
        {
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            ViewBag.esteAdmin = User.IsInRole("Administrator");

            ViewBag.esteOrganizator = false;
            if (Task.UserId == ViewBag.utilizatorCurent)
            {
                ViewBag.esteOrganizator = true;
            }

            ViewBag.esteUser = IsFromGroup(Task.UserId, Task.GroupId);
        }

        // verificare daca userul face parte din echipa
        private bool IsFromGroup(String userId, int groupId)
        {
            var registrations = from reg in db.Registrations
                                where reg.UserId == userId
                                where reg.GroupId == groupId
                                select reg;

            return registrations != null;
        }

        // obtinere prioritati
        [NonAction]
        private IEnumerable<SelectListItem> GetPriority()
        {
            var PriorityList = new List<SelectListItem>();
            PriorityList.Add(new SelectListItem
            {
                Value = "Urgent",
                Text = "Urgent"
            });
            PriorityList.Add(new SelectListItem
            {
                Value = "Medium",
                Text = "Medium"
            });
            PriorityList.Add(new SelectListItem
            {
                Value = "Low",
                Text = "Low"
            });

            return PriorityList;
        }

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

        //SHOW
        //GET: afisarea unui singur task
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Show(int id)
        {
            if (TempData.ContainsKey("message"))
                ViewBag.Message = TempData["message"];

            Tasks Task = db.Tasks.Find(id);
            SetAccessRights(Task);

            if (ViewBag.esteUser || ViewBag.esteOrganizator || ViewBag.esteAdmin)
            {
                return View(Task);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa vedeti task-urile unei echipe din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }


        //NEW
        //GET: afisare formular adaugare task
        [Authorize(Roles = "User,Administrator")]
        public ActionResult New(int Id)
        {
            if (IsFromGroup(User.Identity.GetUserId(), Id))
            {
                Tasks Task = new Tasks();
                Task.PriorityLabel = GetPriority();
                Task.UsersList = GetAllUsers(Id);

                ViewBag.GroupId = Id;
                return View(Task);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa creati task-uri la o echipa din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }

        //POST: adaugare task-ul nou in baza de date
        [Authorize(Roles = "User,Administrator")]
        [HttpPost]
        public ActionResult New(Tasks newTask)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user1 = db.Users.Find(userId);
            ApplicationUser user2 = db.Users.Find(newTask.UserId2);
            ViewBag.GroupId = newTask.GroupId;

            if (IsFromGroup(userId, newTask.GroupId))
            {
                newTask.UserId = userId;
                newTask.Done = false;
                if (user2 != null)
                {
                    newTask.User2 = user2;
                }
                newTask.UsersList = GetAllUsers(newTask.GroupId);
                newTask.PriorityLabel = GetPriority();

                try
                {
                    db.Tasks.Add(newTask);
                    user1.CreatedTasks.Add(newTask);
                    if (user2 != null)
                    {
                        user2.AsignedTasks.Add(newTask);
                    }
                    db.SaveChanges();
                    TempData["message"] = "Task-ul a fost adaugat cu success!";

                    return Redirect("/Groups/Show/" + newTask.GroupId);
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Nu s-a putut adauga task-ul!";
                    if (newTask.Deadline < new DateTime())
                    {
                        ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                    }
                    ViewBag.Message = e.Message;

                    return View(newTask);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa creati task-uri la o echipa din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }


        //EDIT
        //GET: afisare formular de editare task
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Tasks Task = db.Tasks.Find(id);
            SetAccessRights(Task);

            if (ViewBag.esteAdmin || ViewBag.esteOrganizator || ViewBag.esteUser)
            {
                Task.PriorityLabel = GetPriority();
                Task.UsersList = GetAllUsers(Task.GroupId);
                return View(Task);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati task-urile de la aceasta echipa!";
                return Redirect("/Groups/Show/" + Task.GroupId);
            }
        }

        //PUT: modificare task
        [Authorize(Roles = "User, Administrator")]
        [HttpPut]
        public ActionResult Edit(int id, Tasks editedTask)
        {
            SetAccessRights(editedTask);
            editedTask.PriorityLabel = GetPriority();
            editedTask.UsersList = GetAllUsers(editedTask.GroupId);
            ApplicationUser user2 = db.Users.Find(editedTask.UserId2);

            try
            {
                if (ViewBag.esteAdmin || ViewBag.esteOrganizator || ViewBag.esteUser)
                {
                    Tasks Task = db.Tasks.Find(id);
                    Task.PriorityLabel = GetPriority();
                    Task.UsersList = GetAllUsers(Task.GroupId);
                    ApplicationUser user2Initial = db.Users.Find(Task.UserId2);

                    try
                    {
                        Task = editedTask;
                        if (user2 != user2Initial)
                        {
                            user2.AsignedTasks.Add(Task);
                            user2Initial.AsignedTasks.Remove(Task);
                        }
                        db.SaveChanges();
                        TempData["message"] = "Task-ul a fost modificat cu succes!";

                        return Redirect("/Taskss/Show/" + id);
                    }
                    catch (Exception e)
                    {
                        ViewBag.Message = "Nu s-a putut edita task-ul!";
                        return View(editedTask);
                    }
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati un task-urile din aceasta echipa!";
                    if (editedTask.Deadline < new DateTime())
                    {
                        ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                    }

                    return Redirect("/Groups/Show/" + editedTask.GroupId);
                }
            }

            catch (Exception e)
            {
                ViewBag.Message = "Nu s-a putut edita task-ul!";
                if (editedTask.Deadline < new DateTime())
                {
                    ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                }

                return View(editedTask);
            }
        }


        //DELETE
        //DELETE: stergerea unui task
        [Authorize(Roles = "User,Administrator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Tasks Task = db.Tasks.Find(id);
            SetAccessRights(Task);
            ApplicationUser user1 = db.Users.Find(Task.UserId);
            ApplicationUser user2 = db.Users.Find(Task.UserId2);

            try
            {
                if (ViewBag.esteOrganizator || ViewBag.esteAdmin)
                {
                    db.Tasks.Remove(Task);
                    user1.CreatedTasks.Remove(Task);
                    if (user2 != null)
                    {
                        user2.AsignedTasks.Remove(Task);
                    }
                    db.SaveChanges();
                    TempData["message"] = "Task-ul a fost sters cu success!";

                    return Redirect("/Groups/Show/" + Task.GroupId);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti un task care nu va apartine!";
                    return Redirect("/Groups/Show/" + Task.GroupId);
                }
            }
            catch (Exception e)
            {
                TempData["message"] = "Nu s-a putut sterge task-ul!";
                return Redirect("/Taskss/Show/" + Task.TaskId);
            }
        }
    }
}