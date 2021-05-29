using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobPostingProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace JobPostingProject.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        JobPostingDBEntities1 db = new JobPostingDBEntities1();

        // Load Candidate / Company Image
        public FileContentResult UserPhotos()
        {

            string userId = User.Identity.GetUserId();

            // todo: get the  authenticated User details to load his Image    
            var dbUsers = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var authenticatedUser = dbUsers.Users.Where(u => u.Id == userId).FirstOrDefault();
            var userPhoto = authenticatedUser.UserPhoto;
            if (userPhoto == null)
            {
                string fileName = HttpContext.Server.MapPath(@"~/Images/UserDefaultImage/defaultImage.jpg");

                // Todo: Convert Image to Byte
                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);

                // Todo: Update UserPhoto Column 
                authenticatedUser.UserPhoto = imageData;
                dbUsers.SaveChanges();
                return File(imageData, "image/jpg");

            }
            // If Already Have a photo
            return new FileContentResult(authenticatedUser.UserPhoto, "image/jpeg");


        }


        // AutoComplete Function
        public JsonResult GetJobsTitle(string search)
        {
            List<string> JobsTitles;
            JobsTitles = db.Announcements.Where(x => x.Title.Contains(search)).Select(y => y.Title).ToList();
            return new JsonResult { Data = JobsTitles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public void summ()
        {

        }
        public ActionResult Index()
        {
            ViewData["Categories"] = new SelectList(db.Categories.ToList(), "CategoryID", "CategoryName");
            ViewBag.CategoeyNames = this.db.Categories.Select(c => c.CategoryName).Distinct().ToArray();
            return View();
        }

        [HttpPost]
        public JsonResult GetJobPortionByCategory()
        {

            List<Category> categories = this.db.Categories.ToList();
            List<double> partitions = new List<double>();
            double totalAnnouncements = this.db.Announcements.Count();
            double count = 0.0;
            double value;
            foreach (var cat in categories)
            {
                count = this.db.Announcements.Where(a => a.CategoryID == cat.CategoryID).Count();
                value = (count / totalAnnouncements) * 100.00 ;
                partitions.Add(Math.Round(value));
            }

            List<object> iData = new List<object>();
            iData.Add(partitions);
            iData.Add(categories.Select(c => c.CategoryName));

            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetJobPortionByLevel()
        {

            List<Level> levels = this.db.Levels.ToList();
            List<double> partitions = new List<double>();
            double totalAnnouncements = this.db.Announcements.Count();
            double count = 0.0;
            double value;
            foreach (var lev in levels)
            {
                count = this.db.Announcements.Where(a => a.LevelID == lev.LevelID).Count();
                value = (count / totalAnnouncements) * 100.00;
                partitions.Add(Math.Round(value));
            }

            List<object> iData = new List<object>();
            iData.Add(partitions);
            iData.Add(levels.Select(l => l.LevelName));

            return Json(iData, JsonRequestBehavior.AllowGet);
        }
    }
}