using System;
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
        private bool IsFromGroup(String userId, int GroupId)
        {
            // TO BE DONE WHEN GROUPS
            return true;
        }

        // obtinere prioritati
        [NonAction]
        private IEnumerable<SelectListItem> getPriority()
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
                        //where user.GroupId = groupId
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
                    Value = user.Id.ToString(),
                    Text = user.UserName.ToString()
                });
            }

            return UsersList;
        }

        //SHOW
        //GET: afisarea unui singur Task
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
        //GET: afisare formular adaugare Tasks
        [Authorize(Roles = "User,Administrator")]
        public ActionResult New(int Id)
        {
            if (IsFromGroup(User.Identity.GetUserId(), Id))
            {
                Tasks Task = new Tasks();
                Task.PriorityLabel = getPriority();
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

        //POST: adaugare Tasks-ul nou in baza de date
        [Authorize(Roles = "User,Admininistrator")]
        [HttpPost]
        public ActionResult New(Tasks newTask)
        {
            string userId = User.Identity.GetUserId();
            if (IsFromGroup(userId, newTask.GroupId))
            {
                newTask.UserId = userId;
                newTask.Done = false;

                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Tasks.Add(newTask);
                        db.SaveChanges();
                        TempData["message"] = "Task-ul a fost adaugat cu success!";

                        return Redirect("/Group/Show/" + newTask.GroupId);

                    }

                    else
                    {
                        ViewBag.Message = "Nu s-a putut adauga Task-ul!";
                        if (newTask.Deadline < new DateTime())
                        {
                            ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                        }

                        return View(newTask);
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Nu s-a putut adauga task-ul!";
                    if (newTask.Deadline < new DateTime())
                    {
                        ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                    }

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
        //GET: afisare formular de editare Tasks
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Tasks Task = db.Tasks.Find(id);
            SetAccessRights(Task);

            if (ViewBag.esteAdmin || ViewBag.esteOrganizator || ViewBag.esteUser)
            {
                Task.PriorityLabel = getPriority();
                Task.UsersList = GetAllUsers(Task.GroupId);
                return View(Task);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati task-urile de la aceasta echipa!";
                return Redirect("/Teams/Show/" + Task.GroupId);
            }
        }

        //PUT: modificare Tasks
        [Authorize(Roles = "User, Administrator")]
        [HttpPut]
        public ActionResult Edit(int id, Tasks editedTask)
        {
            SetAccessRights(editedTask);

            try
            {
                if (ViewBag.esteAdmin || ViewBag.esteOrganizator || ViewBag.esteUser)
                {
                    if (ModelState.IsValid)
                    {

                        Tasks Task = db.Tasks.Find(id);
                        Task.PriorityLabel = getPriority();
                        Task.UsersList = GetAllUsers(Task.GroupId);

                        if (TryUpdateModel(Task))
                        {
                            Task = editedTask;
                            db.SaveChanges();
                            TempData["message"] = "Task-ul a fost modificat cu succes!";

                            return Redirect("/Taskss/Show/" + id);
                        }

                        editedTask.PriorityLabel = getPriority();
                        editedTask.UsersList = GetAllUsers(editedTask.GroupId);

                        ViewBag.Message = "Nu s-a putut edita task-ul!";
                        return View(editedTask);
                    }

                    editedTask.PriorityLabel = getPriority();
                    editedTask.UsersList = GetAllUsers(editedTask.GroupId);

                    ViewBag.Message = "Nu s-a putut edita task-ul!";
                    return View(editedTask);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati un task-urile din aceasta echipa!";
                    if (editedTask.Deadline < new DateTime())
                    {
                        ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                    }

                    return Redirect("/Teams/Show/" + editedTask.GroupId);
                }
            }

            catch (Exception e)
            {
                editedTask.PriorityLabel = getPriority();
                editedTask.UsersList = GetAllUsers(editedTask.GroupId);

                ViewBag.Message = "Nu s-a putut edita task-ul!";
                if (editedTask.Deadline < new DateTime())
                {
                    ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                }

                return View(editedTask);
            }
        }


        //DELETE
        //DELETE: stergerea unui Tasks
        [Authorize(Roles = "User,Administrator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Tasks Task = db.Tasks.Find(id);
            SetAccessRights(Task);

            try
            {
                if (ViewBag.esteOrganizator || ViewBag.esteAdmin)
                {
                    db.Tasks.Remove(Task);
                    db.SaveChanges();
                    TempData["message"] = "Task-ul a fost sters cu success!";

                    return Redirect("/Teams/Show/" + Task.GroupId);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti un task care nu va apartine!";
                    return Redirect("/Teams/Show/" + Task.GroupId);
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