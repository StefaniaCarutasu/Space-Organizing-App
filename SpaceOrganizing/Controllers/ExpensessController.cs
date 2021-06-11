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


        //GET: afisare formular adaugare plata
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
                TempData["message"] = "Nu aveti dreptul sa creati plati la o echipa din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }

        //POST: adaugare plata noua in baza de date
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
                        TempData["message"] = "Plata a fost adaugat cu success!";

                        return Redirect("/Groups/Show/" + newExpense.GroupId);
                    }

                    ViewBag.Message = "Nu s-a putut adauga plata!";
                    return View(newExpense);
                }
                catch (Exception e)
                {
                    ViewBag.Message = "Nu s-a putut adauga plata!";
                    return View(newExpense);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa creati plati la o echipa din care nu faceti parte!";
                return Redirect("Groups/Index");
            }
        }
    }
}