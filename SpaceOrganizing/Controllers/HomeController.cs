using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var search = "";
            var groups = db.Groups.Include("GroupName").AsQueryable();

            if(Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();

                List<int> groupIds = db.Groups.Where(
                    gr => gr.GroupName.Contains(search) ||
                          gr.GroupDescription.Contains(search)
                    ).Select(g => g.GroupId).ToList();

                groups = db.Groups.Where(group => groupIds.Contains(group.GroupId)).Include("GroupName");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}