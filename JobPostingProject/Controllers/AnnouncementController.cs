using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Classes;
using JobPostingProject.Models;
using Microsoft.AspNet.Identity;

namespace JobPostingProject.Controllers
{
    public class AnnouncementController : ApplicationBaseController
    {
        JobPostingDBEntities1 db = new JobPostingDBEntities1();
        // GET: Announcement
        public ActionResult Index(string titleInput, string cityInput, int? Categories, DateTime? dateInf, DateTime? dateSup, int? Levels)
        {
            ViewBag.Levels = new SelectList(db.Levels.ToList(), "LevelID", "LevelName");
            ViewData["Categories"] = new SelectList(db.Categories.ToList(), "CategoryID", "CategoryName");

            ViewBag.Job = titleInput;
            ViewBag.City = cityInput;

            if (dateInf == null && dateSup == null && Levels == null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower()) && announcement.CategoryID == Categories).ToList();
                return View(listAnnouncements);
            }
            else if (dateInf != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower()) && announcement.CategoryID == Categories && announcement.PublicationDate >= dateInf).ToList();
                return View(listAnnouncements);
            }
            else if (dateSup != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower()) && announcement.CategoryID == Categories && announcement.PublicationDate <= dateSup).ToList();
                return View(listAnnouncements);
            }
            else if (Levels != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower())) && announcement.CategoryID == Categories &&  announcement.LevelID == Levels).ToList();
                return View(listAnnouncements);
            }
            else
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower())) && announcement.CategoryID == Categories && announcement.PublicationDate <= dateInf && announcement.PublicationDate <= dateSup && announcement.LevelID == Levels).ToList();
                return View(listAnnouncements);
            }
        }

        // GET: Announcement/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [AuthorizeCreateAnnouncement(Roles = "Company")]
        // GET: Announcement/Create
        public ActionResult CreateAnnouncement()
        {
            ViewData["Levels"] = new SelectList(db.Levels.ToList(), "LevelID", "LevelName");
            ViewData["Categories"] = new SelectList(db.Categories.ToList(), "CategoryID", "CategoryName");

            return View();
        }

        // POST: Announcement/Create
        [HttpPost]
        public ActionResult CreateAnnouncement(Announcement announcementModel)
        {
            try
            {

                // Find Company whitch will create Annoncement
                // Todo : find ID of Authocated User/Company
                string id = HttpContext.User.Identity.GetUserId();
                var creator = db.Companies.Where(c => c.CompanySecondID.Equals(id)).FirstOrDefault();
                // TODO: Add insert logic here to Create Announcement
                string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
                var newAnnouncement = new Announcement
                {
                    Title = announcementModel.Title,
                    Description = announcementModel.Description,
                    PublicationDate = Convert.ToDateTime(currentDate),
                    Location = announcementModel.Location,
                    LevelID = announcementModel.LevelID,
                    CategoryID = announcementModel.CategoryID,
                    CompanyID = creator.CompanyID
                };
                db.Announcements.Add(newAnnouncement);
                db.SaveChanges();

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
