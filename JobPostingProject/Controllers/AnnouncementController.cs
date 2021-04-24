using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models;

namespace JobPostingProject.Controllers
{
    public class AnnouncementController : Controller
    {
        JobPostingDBEntities1 db = new JobPostingDBEntities1();
        // GET: Announcement
        public ActionResult Index(FormCollection collection)
        {
            //string title = Request.Unvalidated.Form["titleInput"].ToString();
            //string location = Request.Unvalidated.Form["locationInput"].ToString();
            string title = collection["titleInput"];
            string location = collection["locationInput"];
            ViewBag.Levels = new SelectList(db.Levels.ToList(), "LevelID", "LevelName");
            List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.Equals(title) && announcement.Location.Equals(location)).ToList();
            return View(listAnnouncements);
        }

        //[HttpGet]
        //public ActionResult Search(FormCollection form)
        //{
        //    ViewBag.Levels = new SelectList(db.Levels.ToList(), "LevelID", "LevelName");
        //    List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.Equals("Frontend with reactJs") && announcement.Location.Equals("Las Vegas")) as List<Announcement>;
        //    return View("Index", listAnnouncements);
        //    //return RedirectToAction("Index", "Announcement", searchedAnnouncements);
        //}

        //[HttpPost]
        ////[Route(Name = "Announcement/Index")]
        //[ActionName("Index")]
        //public ActionResult Search()
        //{
        //    List<Announcement> searchedAnnouncements = db.Announcements.Where(announcement => announcement.Title.Equals(form["titleInput"]) && announcement.Location.Equals(form["cityInput"])) as List<Announcement>;
        //    return View(searchedAnnouncements);
        //}

        // GET: Announcement/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Announcement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Announcement/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Announcement/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Announcement/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Announcement/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Announcement/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
