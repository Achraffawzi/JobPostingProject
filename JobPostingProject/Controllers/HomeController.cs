using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models;

namespace JobPostingProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            string titleValue = form["titleInput"];
            string locationValue = form["locationInput"];
            return RedirectToAction("Index", "Announcement", new { title = titleValue , location = locationValue });
        }
    }
}