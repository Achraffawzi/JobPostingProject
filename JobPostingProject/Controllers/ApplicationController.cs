using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models; 


namespace JobPostingProject.Controllers
{
    [Authorize]
    public class ApplicationController : ApplicationBaseController
    {
        JobPostingDBEntities1 db = new JobPostingDBEntities1();
        // GET: Application
        public ActionResult Index(string idUser)
        {
            // Search for the candidate that has the same Hash code as idUser param
            Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();

            List<Application> listeOfApplications = db.Applications.Where(a => a.CandidateID == appliedCandidate.CandidateID).ToList();
            return View(listeOfApplications);
        }
        
        // GET: Application/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // GET: Application/Apply
        public ActionResult AnnouncementDetails(int? idAnnouncement)
        {
            Announcement checkedAnnouncement = db.Announcements.Where(a => a.AnnouncementID == idAnnouncement).FirstOrDefault();
            return View(checkedAnnouncement);
        }


        // POST: Application/Apply
        [HttpPost]
        public ActionResult AnnouncementDetails(string idUser, int idAnnouncement)
        {
            try
            {
                // TODO: Add insert logic here

                // Search for the candidate that has the same Hash code as idUser param
                Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();
                
                
                Application newApp = new Application
                {
                    AnnouncementID = idAnnouncement,
                    CandidateID = appliedCandidate.CandidateID,
                    ApplicationDate = DateTime.Now,
                };
                db.Applications.Add(newApp);
                db.SaveChanges();

                return RedirectToAction("Index", "Application", new { idUser = idUser});
            }
            catch
            {
                return View();
            }
        }

        // GET: Application/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Application/Edit/5
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

        // GET: Application/Delete/5
        public ActionResult Delete(string idUser, int id)
        {
            // Search for the candidate that has the same Hash code as idUser param
            Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();

            Application searchedAnnouncement = db.Applications.Where(a => a.CandidateID == appliedCandidate.CandidateID && a.AnnouncementID == id).FirstOrDefault();
            return View(searchedAnnouncement);
        }

        // POST: Application/Delete/5
        [HttpPost]
        public ActionResult Delete(string idUser, int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                // Search for the candidate that has the same Hash code as idUser param
                Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();

                Application searchedAnnouncement = db.Applications.Where(a => a.CandidateID == appliedCandidate.CandidateID && a.AnnouncementID == id).FirstOrDefault();
                db.Applications.Remove(searchedAnnouncement);
                db.SaveChanges();

                return RedirectToAction("Index", new { idUser = idUser});
            }
            catch
            {
                return View();
            }
        }
    }
}
