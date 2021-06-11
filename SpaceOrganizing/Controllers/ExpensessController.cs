using Microsoft.AspNet.Identity;
using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class ExpensessController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        // verificare daca userul face parte din echipa
        [NonAction]
        private bool IsFromGroup(String userId, int groupId)
        {
            var registrations = from reg in db.Registrations
                                where reg.UserId == userId
                                where reg.GroupId == groupId
                                select reg;

            return registrations != null;
        }


        //GET: afisare formular adaugare cheltuiala
        [Authorize(Roles = "User,Administrator")]
        public ActionResult New(int Id)
        {
            if (IsFromGroup(User.Identity.GetUserId(), Id))
            {
                Expense Expense = new Expense();

                ViewBag.GroupId = Id;
                return View(Expense);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa creati cheltuieli la o echipa din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }

        //POST: adaugare cheltuiala noua in baza de date
        [Authorize(Roles = "User,Administrator")]
        [HttpPost]
        public ActionResult New(Expense newExpense)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user1 = db.Users.Find(userId);
            ViewBag.GroupId = newExpense.GroupId;

            if (IsFromGroup(userId, newExpense.GroupId))
            {
                newExpense.UserId = userId;
                newExpense.User = user1;
                newExpense.Paid = false;

                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Expenses.Add(newExpense);
                        db.SaveChanges();
                        TempData["message"] = "cheltuiala a fost adaugat cu success!";

                        return Redirect("/Groups/Show/" + newExpense.GroupId);
                    }

                    ViewBag.Message = "Nu s-a putut adauga cheltuiala!";
                    return View(newExpense);
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Nu s-a putut adauga cheltuiala!";
                    return View(newExpense);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa creati cheltuieli la o echipa din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }

        //EDIT
        //GET: afisare formular de editare cheltuiala
        [Authorize(Roles = "User,Administrator")]
        public ActionResult Edit(int id)
        {
            Expense Expense = db.Expenses.Find(id);

            if (IsFromGroup(User.Identity.GetUserId(), Expense.GroupId))
            {
                return View(Expense);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati cheltuielile de la aceasta echipa!";
                return Redirect("/Groups/Show/" + Expense.GroupId);
            }
        }

        //PUT: modificare cheltuiala
        [Authorize(Roles = "User, Administrator")]
        [HttpPut]
        public ActionResult Edit(int id, Expense editedExpense)
        {
            ApplicationUser user1 = db.Users.Find(editedExpense.UserId);
            editedExpense.User = user1;

            try
            {
                if (IsFromGroup(User.Identity.GetUserId(), editedExpense.GroupId))
                {
                    if (ModelState.IsValid)
                    {
                        Expense Expense = db.Expenses.Find(id);

                        if (TryUpdateModel(Expense))
                        {
                            Expense = editedExpense;
                            db.SaveChanges();
                            TempData["message"] = "Cheltuiala a fost modificat cu succes!";

                            return Redirect("/Groups/Show/" + Expense.GroupId);
                        }

                        ViewBag.Message = "Nu s-a putut edita cheltuiala!";
                        return View(editedExpense);
                    }

                    ViewBag.Message = "Nu s-a putut edita cheltuiala!";
                    return View(editedExpense);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa modificati o cheltuiala din aceasta echipa!";
                    return Redirect("/Groups/Show/" + editedExpense.GroupId);
                }
            }

            catch (Exception e)
            {
                ViewBag.Message = "Nu s-a putut edita cheltuiala!";
                return View(editedExpense);
            }
        }

        //DELETE
        //DELETE: stergerea unuei cheltuieli
        [Authorize(Roles = "User,Administrator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Expense Expense = db.Expenses.Find(id);
            ApplicationUser user1 = db.Users.Find(Expense.UserId);

            try
            {
                if (IsFromGroup(User.Identity.GetUserId(), Expense.GroupId))
                {
                    db.Expenses.Remove(Expense);
                    db.SaveChanges();
                    TempData["message"] = "Cheltuiala a fost stearsa cu success!";

                    return Redirect("/Groups/Show/" + Expense.GroupId);
                }

                else
                {
                    TempData["message"] = "Nu aveti dreptul sa stergeti o cheltuiala care nu va apartine!";
                    return Redirect("/Groups/Show/" + Expense.GroupId);
                }
            }
            catch (Exception e)
            {
                TempData["message"] = "Nu s-a putut sterge cheltuiala!";
                return Redirect("/Grups/Show/" + Expense.GroupId);
            }
        }
    }
}