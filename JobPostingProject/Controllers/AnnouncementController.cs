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


        public ActionResult Index(string titleInput, string locationInput, DateTime? dateInput, int? Levels)
        {
            ViewBag.Levels = new SelectList(db.Levels.ToList(), "LevelID", "LevelName");
            ViewBag.JobTitle = titleInput;
            ViewBag.Location = locationInput;

            if (dateInput == null && Levels == null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.Contains(titleInput) && announcement.Location.Contains(locationInput)).ToList();
                return View(listAnnouncements);
            }
            else if(dateInput != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.Contains(titleInput) && announcement.Location.Contains(locationInput)) && announcement.PublicationDate == dateInput).ToList();
                return View(listAnnouncements);
            }
            else if (Levels != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.Contains(titleInput) && announcement.Location.Contains(locationInput)) && announcement.LevelID == Levels).ToList();
                return View(listAnnouncements);
            }
            else
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.Contains(titleInput) && announcement.Location.Contains(locationInput)) && announcement.PublicationDate == dateInput || announcement.LevelID == Levels).ToList();
                return View(listAnnouncements);
            }
        }

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
