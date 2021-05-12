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
        private Models.AppContext db = new Models.AppContext();

        [NonAction]
        private void SetAccessRights()
        {
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.esteOrganizator = User.IsInRole("Organizator");
            ViewBag.esteMembru = User.IsInRole("Membru");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }

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

        //SHOW
        //GET: afisarea unui singur Tasks
        [Authorize(Roles = "Membru,Organizator,Admin")]
        public ActionResult Show(int id)
        {
            if (TempData.ContainsKey("message"))
                ViewBag.Message = TempData["message"];

            Tasks Task = db.Tasks.Find(id);
            ViewBag.seteazaStatus = false;
            if (User.IsInRole("Membru") || User.IsInRole("Admin"))
            {
                ViewBag.seteazaStatus = true;
            }

            SetAccessRights();

            return View(Task);
        }


        //NEW
        //GET: afisare formular adaugare Tasks
        [Authorize(Roles = "Organizator,Admin")]
        public ActionResult New(int Id)
        {
            ViewBag.TeamId = Id;
            return View();
        }

        //POST: adaugare Tasks-ul nou in baza de date
        [Authorize(Roles = "Organizator,Admin")]
        [HttpPost]
        public ActionResult New(Tasks newTask)
        {
            string userId = User.Identity.GetUserId();
            newTask.UserId = userId;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Tasks.Add(newTask);
                    db.SaveChanges();
                    TempData["message"] = "Tasksul a fost adaugat cu success!";

                    return Redirect("/Teams/Show/" + newTask.GroupId);

                }

                else
                {
                    ViewBag.Message = "Nu s-a putut adauga Tasks-ul!";
                    if (newTask.Deadline < new DateTime())
                    {
                        ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                    }

                    return View(newTask);
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = "Nu s-a putut adauga Tasks-ul!";
                if (newTask.Deadline < new DateTime())
                {
                    ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                }

                return View(newTask);
            }
        }


        //EDIT
        //GET: afisare formular de editare Tasks
        [Authorize(Roles = "Membru,Organizator,Admin")]
        public ActionResult Edit(int id)
        {
            Tasks Task = db.Tasks.Find(id);
            Task.PriorityLabel = getPriority();

            if (User.IsInRole("Organizator") || User.IsInRole("Admin") || User.IsInRole("Membru"))
            {
                SetAccessRights();

                return View(Task);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati Tasks-urile de la aceasta echipa!";
                return Redirect("/Teams/Show/" + Task.GroupId);
            }
        }

        //PUT: modificare Tasks
        [Authorize(Roles = "Membru,Organizator,Admin")]
        [HttpPut]
        public ActionResult Edit(int id, Tasks editedTask)
        {
            try
            {
                if (User.IsInRole("Organizator") || User.IsInRole("Admin") || User.IsInRole("Membru"))
                {
                    if (ModelState.IsValid)
                    {

                        Tasks Tasks = db.Tasks.Find(id);
                        Tasks.PriorityLabel = getPriority();

                        if (TryUpdateModel(Tasks))
                        {
                            Tasks = editedTask;
                            db.SaveChanges();
                            TempData["message"] = "Tasks-ul a fost modificat cu succes!";

                            return Redirect("/Taskss/Show/" + id);
                        }

                        SetAccessRights();
                        editedTask.PriorityLabel = getPriority();

                        ViewBag.Message = "Nu s-a putut edita Tasks-ul!";
                        return View(editedTask);
                    }
                    SetAccessRights();
                    editedTask.PriorityLabel = getPriority();

                    ViewBag.Message = "Nu s-a putut edita Tasks-ul!";
                    return View(editedTask);
                }

                else
                {

                    TempData["message"] = "Nu aveti dreptul sa modificati un Tasks-urile din aceasta echipa!";
                    if (editedTask.Deadline < new DateTime())
                    {
                        ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                    }

                    return Redirect("/Teams/Show/" + editedTask.GroupId);
                }
            }

            catch (Exception e)
            {
                SetAccessRights();
                editedTask.PriorityLabel = getPriority();

                ViewBag.Message = "Nu s-a putut edita Tasks-ul!";
                if (editedTask.Deadline < new DateTime())
                {
                    ViewBag.Message = "Deadline-ul nu poate sa fie inainte de data curenta!";
                }

                return View(editedTask);
            }
        }


        //DELETE
        //DELETE: stergerea unui Tasks
        [Authorize(Roles = "Organizator,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Tasks Task = db.Tasks.Find(id);

            try
            {
                if (User.Identity.GetUserId() == Task.UserId || User.IsInRole("Admin"))
                {
                    db.Tasks.Remove(Task);
                    db.SaveChanges();
                    TempData["message"] = "Tasks-ul a fost sters cu success!";

                    return Redirect("/Teams/Show/" + Task.GroupId);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti un Tasks care nu va apartine!";
                    return Redirect("/Teams/Show/" + Task.GroupId);
                }
            }
            catch (Exception e)
            {
                TempData["message"] = "Nu s-a putut sterge Tasks-ul!";
                return Redirect("/Taskss/Show/" + Task.TaskId);
            }
        }
    }
}