using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

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
            ViewBag.TotalApplicationsCandidate = listeOfApplications.Count();
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
        public JsonResult Apply(string idUser, int idAnnouncement)
        {
            try
            {
                // TODO: Add insert logic here

                // Search for the candidate that has the same Hash code as idUser param
                Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();

                // The returning JSON Object
                object result = null;

                // todo: check if Candidate is already applyed to job
                var check = db.Applications.Where(a => a.CandidateID.Equals(appliedCandidate.CandidateID) && a.AnnouncementID.Equals(idAnnouncement)).FirstOrDefault();
                if (check == null)
                {
                    Application newApp = new Application
                    {
                        AnnouncementID = idAnnouncement,
                        CandidateID = appliedCandidate.CandidateID,
                        ApplicationDate = DateTime.Now,
                    };
                    db.Applications.Add(newApp);
                    db.SaveChanges();

                    result = new
                    {
                        EnableSuccess = true,
                        SuccessTitle = "Success",
                        SuccessMsg = "Your application has been sent successfully!"
                    };
                }
                else
                {
                    result = new
                    {
                        EnableSuccess = false,
                        ErrorTitle = "Warning",
                        ErrorMsg = "You are already applayed for this Job!"
                    };
                }
                return this.Json(result, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new
                {
                    EnableSuccess = false,
                    ErrorTitle = "Warning",
                    ErrorMsg = "Something went wrong!"
                }, JsonRequestBehavior.AllowGet);
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
        //public ActionResult Delete(string idUser, int id)
        //{
        //    // Search for the candidate that has the same Hash code as idUser param
        //    Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();

        //    Application searchedAnnouncement = db.Applications.Where(a => a.CandidateID == appliedCandidate.CandidateID && a.AnnouncementID == id).FirstOrDefault();
        //    return View(searchedAnnouncement);
        //}

        // POST: Application/Delete/5
        [HttpPost]
        public ActionResult Delete(int? announcementID)
        {
            try
            {
                // TODO: Add delete logic here
                // Get the current logged in candidate
                string uniqueID = User.Identity.GetUserId();
                Candidate currentCandidate = this.db.Candidates.FirstOrDefault(c => c.CandidateSecondID == uniqueID);
                // Get the Application
                Application deletingApplication = db.Applications.FirstOrDefault(a => a.AnnouncementID == announcementID && a.CandidateID == currentCandidate.CandidateID);
                db.Applications.Remove(deletingApplication);
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
