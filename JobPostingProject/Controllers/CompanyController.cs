using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models;
using Microsoft.AspNet.Identity;

namespace JobPostingProject.Controllers
{
    public class CompanyController : ApplicationBaseController
    {
        JobPostingDBEntities1 db = new JobPostingDBEntities1();
        ApplicationDbContext appDb = new ApplicationDbContext();
        // GET: Company
        public ActionResult Index(string idUser)
        {
            // Search for the candidate that has the same Hash code as idUser param
            Company appliedCompany = db.Companies.Where(c => c.CompanySecondID.Equals(idUser)).FirstOrDefault();

            return View(appliedCompany);
        }

        // GET: Company/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
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

        // GET: Company/Edit/5
        public ActionResult Edit(string idUser)
        {
            ViewBag.UniqueID = idUser;
            return View();
        }

        // POST: Company/Edit/5
        [HttpPost]
        public ActionResult Edit(Company p_company)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    // Get the email of the current logged in company
                    var uniqueID = User.Identity.GetUserId();
                    var email = db.Companies.FirstOrDefault(c => c.CompanySecondID == uniqueID).Email;

                    // The path of the current logged in company's folder
                    var folderPath = Server.MapPath("~/Data/Companies/" + email);

                    Byte[] logoData = null;

                    string fullPathInServer = "~/Data/Companies/" + email + "/";

                    // Get file info of the logo
                    if (p_company.LogoFileName != null)
                    {
                        string logoFileName = Path.GetFileNameWithoutExtension(p_company.LogoFileName.FileName);
                        string logoFileExtension = Path.GetExtension(p_company.LogoFileName.FileName);
                        string _logoFileName = logoFileName + logoFileExtension;
                        p_company.Logo = fullPathInServer + _logoFileName;
                        _logoFileName = Path.Combine(Server.MapPath(fullPathInServer), _logoFileName);
                        p_company.LogoFileName.SaveAs(_logoFileName);



                        // Todo: convert the Company uploaded Photo as Byte Array before save to DB/ ApplicationDbContext => AspNetUserTable
                        if (Request.Files.Count > 0)
                        {
                            HttpPostedFileBase fileBase = Request.Files["LogoFileName"];
                            using (var binary = new BinaryReader(fileBase.InputStream))
                            {
                                logoData = binary.ReadBytes(fileBase.ContentLength);
                            }
                        }
                    }

                    // Updating candidate info in the identity DbContext
                    var updatedUser = appDb.Users.Where(u => u.Id.Equals(uniqueID)).FirstOrDefault();

                    updatedUser.FirstName = p_company.Name;
                    updatedUser.LastName = p_company.Name;

                    appDb.SaveChanges();

                    // Updating candidate info in our DB
                    Company updatedCompany = db.Companies.Where(c => c.CompanySecondID.Equals(uniqueID)).FirstOrDefault();

                    updatedCompany.Name = p_company.Name;
                    updatedCompany.City = p_company.City;
                    updatedCompany.Address = p_company.Address;
                    updatedCompany.Description = p_company.Description;
                    updatedCompany.Logo = p_company.Logo;
                    updatedCompany.PhoneNumber = p_company.PhoneNumber;

                    db.SaveChanges();
                    return RedirectToAction("Index", "Home", new { idUser = uniqueID });
                }
                ViewBag.ErrorMessage = "Something went wrong!";
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Company/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Company/Delete/5
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
