using System;
using System.Collections.Generic;
using System.IO;
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
                ViewBag.TotalResults = listAnnouncements.Count();
                return View(listAnnouncements);
            }
            else if (dateInf != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower()) && announcement.CategoryID == Categories && announcement.PublicationDate >= dateInf).ToList();
                ViewBag.TotalResults = listAnnouncements.Count();
                return View(listAnnouncements);
            }
            else if (dateSup != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower()) && announcement.CategoryID == Categories && announcement.PublicationDate <= dateSup).ToList();
                ViewBag.TotalResults = listAnnouncements.Count();
                return View(listAnnouncements);
            }
            else if (Levels != null)
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower())) && announcement.CategoryID == Categories && announcement.LevelID == Levels).ToList();
                ViewBag.TotalResults = listAnnouncements.Count();
                return View(listAnnouncements);
            }
            else
            {
                List<Announcement> listAnnouncements = db.Announcements.Where(announcement => (announcement.Title.ToLower().Contains(titleInput.ToLower()) && announcement.Location.ToLower().Contains(cityInput.ToLower())) && announcement.CategoryID == Categories && announcement.PublicationDate <= dateInf && announcement.PublicationDate <= dateSup && announcement.LevelID == Levels).ToList();
                ViewBag.TotalResults = listAnnouncements.Count();
                return View(listAnnouncements);
            }
        }

        // Action to display all the announcements done by the current company
        public ActionResult GetAllAnnouncementDoneBy(string idUser)
        {
            // Get the CompanyID deoending on the its hash code
            Company currentCompany = db.Companies.Where(c => c.CompanySecondID.Equals(idUser)).FirstOrDefault();

            // Get all announcement posted by the company that has the id idUser
            List<Announcement> ownAnnouncements = db.Announcements.Where(c => c.CompanyID.Equals(currentCompany.CompanyID)).ToList();
            ViewBag.TotalAnnouncements = ownAnnouncements.Count();
            return View(ownAnnouncements);
        }

        // Get all candidates applied for the announcement passed in the arg
        public ActionResult ApplicantsForAnnouncement(int id)
        {
            List<ApplicantsViewModel> applicants = new List<ApplicantsViewModel>();
            var appliedCandidates = (from ca in db.Candidates.ToList()
                                     join app in db.Applications.ToList() on ca.CandidateID equals app.CandidateID
                                     join ann in db.Announcements.ToList() on app.AnnouncementID equals ann.AnnouncementID
                                     where ann.AnnouncementID == id
                                     select new
                                     {
                                         ca.FirstName,
                                         ca.LastName,
                                         ca.PhoneNumber,
                                         ca.Cv,
                                         ca.CoverLetter,
                                         ca.Email
                                     }).ToList();



            foreach (var candidate in appliedCandidates)
            {
                applicants.Add(new ApplicantsViewModel
                {
                    FirstName = candidate.FirstName,
                    LastName = candidate.LastName,
                    PhoneNumber = candidate.PhoneNumber,
                    CvFileName = candidate.Cv,
                    CoverLetterFileName = candidate.CoverLetter,
                    Email = candidate.Email,
                });
            }

            ViewData["Applicants"] = applicants;
            return View();
        }

        // Download  file
        public FileResult DownloadFile(string fileName, string email)
        {
            //Build the File Path.
            string path = Server.MapPath(fileName);

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        // GET: Announcement/Details/5
        public ActionResult Details(int id)
        {
            Announcement editingAnnouncement = db.Announcements.Where(c => c.AnnouncementID.Equals(id)).FirstOrDefault();
            return View(editingAnnouncement);
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
                // Find Company which will create Annoncement
                // Todo : find ID of Authenticated User/Company
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

                return RedirectToAction("GetAllAnnouncementDoneBy", "Announcement", new { idUser = id });

            }
            catch
            {
                return View();
            }
        }

        // GET: Announcement/Edit/5
        public ActionResult Edit(int id)
        {
            ViewData["Levels"] = new SelectList(db.Levels.ToList(), "LevelID", "LevelName");
            ViewData["Categories"] = new SelectList(db.Categories.ToList(), "CategoryID", "CategoryName");

            Announcement editingAnnouncement = db.Announcements.Where(c => c.AnnouncementID.Equals(id)).FirstOrDefault();
            return View(editingAnnouncement);
        }

        // POST: Announcement/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Announcement announcement)
        {
            try
            {
                // TODO: Add update logic here
                Announcement editingAnnouncement = db.Announcements.Where(c => c.AnnouncementID.Equals(id)).FirstOrDefault();
                editingAnnouncement.Title = announcement.Title;
                editingAnnouncement.Description = announcement.Description;
                editingAnnouncement.Location = announcement.Location;
                editingAnnouncement.PublicationDate = editingAnnouncement.PublicationDate;
                editingAnnouncement.CompanyID = editingAnnouncement.CompanyID;
                editingAnnouncement.LevelID = announcement.LevelID;
                editingAnnouncement.CategoryID = announcement.CategoryID;
                db.SaveChanges();

                return RedirectToAction("GetAllAnnouncementDoneBy", new { idUser = User.Identity.GetUserId() });
            }
            catch
            {
                return View();
            }
        }

        // GET: Announcement/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    Announcement deletingAnnouncement = db.Announcements.Where(c => c.AnnouncementID.Equals(id)).FirstOrDefault();
        //    return View(deletingAnnouncement);
        //}

        // POST: Announcement/Delete/5
        [HttpPost]
        public ActionResult Delete(int? announcementID)
        {
            try
            {
                // TODO: Add delete logic here
                Announcement deletingAnnouncement = db.Announcements.Where(a => a.AnnouncementID == announcementID).FirstOrDefault();
                db.Announcements.Remove(deletingAnnouncement);
                db.SaveChanges();

                return this.Json(new
                {

                }, JsonRequestBehavior.AllowGet);

                //return RedirectToAction("GetAllAnnouncementDoneBy", new { idUser = User.Identity.GetUserId() }) ;
            }
            catch
            {
                return RedirectToAction("GetAllAnnouncementDoneBy");
            }
        }
    }
}
