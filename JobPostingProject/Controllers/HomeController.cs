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

        public ActionResult Index()
        {
            return View();
        }
    }
}