using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class ErrorPageController : Controller
    {
        public ActionResult Error(int id)
        {
            Response.StatusCode = id;

            return View();
        }
    }
}