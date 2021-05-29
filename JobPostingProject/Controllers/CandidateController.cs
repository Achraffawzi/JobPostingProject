using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace JobPostingProject.Controllers
{
    public class CandidateController : ApplicationBaseController
    {
        JobPostingDBEntities1 db = new JobPostingDBEntities1();
        ApplicationDbContext appDb = new ApplicationDbContext();
        // GET: Candidate
        public ActionResult Index(string idUser)
        {
            // Search for the candidate that has the same Hash code as idUser param
            Candidate appliedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();

            return View(appliedCandidate);

        }

        // GET: Candidate/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Candidate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Candidate/Create
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

        // GET: Candidate/Edit/5
        public ActionResult Edit(string idUser)
        {
            // Get the candidateID with the hash code idUser
            Candidate editingCandidate = this.db.Candidates.Where(c => c.CandidateSecondID == idUser).FirstOrDefault();
            CandidateEditProfileViewModel editVM = new CandidateEditProfileViewModel
            {
                CandidateSecondID = editingCandidate.CandidateSecondID,
                FirstName = editingCandidate.FirstName,
                LastName = editingCandidate.LastName,
                DateOfBirth = editingCandidate.DateOfBirth,
                Address = editingCandidate.Address,
                PhoneNumber = editingCandidate.PhoneNumber,
                Bio = editingCandidate.Bio,
            };
            ViewBag.UniqueID = idUser;
            return View(editVM);
        }

        // POST: Candidate/Edit/5
        [HttpPost]
        public ActionResult Edit(CandidateEditProfileViewModel model) // Ealash CandidateSecondID tayji null men view?
        {
            if (ModelState.IsValid)
            {
                // Get the email of current candidate
                string userHashID = User.Identity.GetUserId();
                var currentCandidate = db.Candidates.Where(c => c.CandidateSecondID == userHashID).FirstOrDefault();
                // Update logic goes here
                var folderPath = Server.MapPath("~/Data/Candidate/" + currentCandidate.Email);
                byte[] imageData = null;
                string fullPathInServer = "~/Data/Candidate/" + currentCandidate.Email + "/";

                // Save Cv In Server Side 
                if (model.CvFileName != null)
                {
                    string cvFileName = Path.GetFileNameWithoutExtension(model.CvFileName.FileName);
                    string cvFileExtension = Path.GetExtension(model.CvFileName.FileName);
                    string _cvFileName = cvFileName + cvFileExtension;
                    model.Cv = fullPathInServer + _cvFileName;
                    _cvFileName = Path.Combine(Server.MapPath(fullPathInServer), _cvFileName);
                    model.CvFileName.SaveAs(_cvFileName);
                }

                // Save Photo In Server Side 
                if (model.PhotoFileName != null)
                {
                    string photoFileName = Path.GetFileNameWithoutExtension(model.PhotoFileName.FileName);
                    string photoFileExtension = Path.GetExtension(model.PhotoFileName.FileName);
                    string _photoFileName = photoFileName + photoFileExtension;
                    model.Photo = fullPathInServer + _photoFileName;
                    _photoFileName = Path.Combine(Server.MapPath(fullPathInServer), _photoFileName);
                    model.PhotoFileName.SaveAs(_photoFileName);

                    // Todo: convert the user uploaded Photo as Byte Array before save to DB/ ApplicationDbContext => AspNetUserTable

                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase fileBase = Request.Files["PhotoFileName"];
                        using (var binary = new BinaryReader(fileBase.InputStream))
                        {
                            imageData = binary.ReadBytes(fileBase.ContentLength);
                        }
                    }
                }

                // Save Cover Letter In Server Side 
                if (model.CoverLetterFileName != null)
                {
                    string coverLetterFileName = Path.GetFileNameWithoutExtension(model.CoverLetterFileName.FileName);
                    string coverLetterFileExtension = Path.GetExtension(model.CoverLetterFileName.FileName);
                    string _coverLetterFileName = coverLetterFileName + coverLetterFileExtension;
                    model.CoverLetter = fullPathInServer + _coverLetterFileName;
                    _coverLetterFileName = Path.Combine(Server.MapPath(fullPathInServer), _coverLetterFileName);
                    model.CoverLetterFileName.SaveAs(_coverLetterFileName);
                }


                // Updating candidate info in the identity DbContext
                var updatedUser = appDb.Users.Where(u => u.Id.Equals(userHashID)).FirstOrDefault();

                updatedUser.FirstName = model.FirstName;
                updatedUser.LastName = model.LastName;

                appDb.SaveChanges();

                if (model.PhotoFileName != null)
                {
                    updatedUser.UserPhoto = imageData;
                }

                // Updating candidate info in our DB
                Candidate updatedCandidate = db.Candidates.Where(c => c.CandidateSecondID.Equals(userHashID)).FirstOrDefault();
                updatedCandidate.FirstName = model.FirstName;
                updatedCandidate.LastName = model.LastName;
                updatedCandidate.Address = model.Address;
                updatedCandidate.Bio = model.Bio;
                updatedCandidate.DateOfBirth = model.DateOfBirth;
                updatedCandidate.PhoneNumber = model.PhoneNumber;
                updatedCandidate.Photo = model.Photo;
                updatedCandidate.Cv = model.Cv;
                updatedCandidate.CoverLetter = model.CoverLetter;

                db.SaveChanges();


                return RedirectToAction("Index", "Candidate", new { idUser = User.Identity.GetUserId() });
            }
            return View();
        }

        // GET: Candidate/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Candidate/Delete/5
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
