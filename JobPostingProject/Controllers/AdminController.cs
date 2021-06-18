using JobPostingProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPostingProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly JobPostingDBEntities1 dbContext = new JobPostingDBEntities1();
        private readonly ApplicationDbContext applicationDb = new ApplicationDbContext();

        // GET : All Companies
        public ActionResult ListCompanies()
        {
            var ListCompanies = dbContext.Companies.ToList();
            return View(ListCompanies);
        }

        // GET Company By ID
        public ActionResult Company(string idUser)
        {
            Company company = dbContext.Companies.Where(c => c.CompanySecondID.Equals(idUser)).FirstOrDefault();
            return View(company);
        }

        // DELETE: Copmany
        public ActionResult DeleteCompany(int id)
        {

            var companyToDelete = dbContext.Companies.Find(id);
            dbContext.Companies.Remove(companyToDelete);
            dbContext.SaveChanges();

          return  RedirectToAction("ListCompanies");
        }

      //--------------------------------------------------------------------------------\\

        public ActionResult ListCandidates()
        {
            var ListCandidates = dbContext.Candidates.ToList();

            return View(ListCandidates);
        }

        // GET Company By ID
        public ActionResult Candidate(string idUser)
        {
            var candidate = dbContext.Candidates.Where(c => c.CandidateSecondID.Equals(idUser)).FirstOrDefault();
            return View(candidate);
        }

        // DELETE: Copmany
        public ActionResult DeleteCandidate(int id)
        {
            var candidateToDelete = dbContext.Candidates.Find(id);
            dbContext.Candidates.Remove(candidateToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("ListCandidates");
        }

        



        //-------------------------------- Utilities -----------------------------------------\\
        public FileResult DownloadFile(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(path));
            var splitPath = path.Split('/');
            var fileName = splitPath[splitPath.Length - 1];
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);


        }

        public FileContentResult UserPhoto(string idUser)
        {
            var user = applicationDb.Users.Find(idUser);
            return new FileContentResult(user.UserPhoto, "image/*");
        }

        //-------------------------------------------------------------------------------------------\\
        public ActionResult ListCategories()
        {
            var ListCategories = dbContext.Categories.ToList();
            return View(ListCategories);
        }

        // Add New Catgory
        public ActionResult AddCategory(Category newCategory)
        {
            dbContext.Categories.Add(new Category
            {
                CategoryName = newCategory.CategoryName
            });
            dbContext.SaveChanges();

            return RedirectToAction("ListCategories");
        }

        public ActionResult DeleteCategory(int id)
        {
            var categoryToDelete = dbContext.Categories.Find(id);
            dbContext.Categories.Remove(categoryToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("ListCategories");
        }
    }
}
