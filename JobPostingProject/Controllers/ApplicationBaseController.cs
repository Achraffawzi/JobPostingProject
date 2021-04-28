using JobPostingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPostingProject.Controllers
{
    public class ApplicationBaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var userName = User.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    var user = context.Users.SingleOrDefault(u => u.UserName.Equals(userName));
                    string fullName = String.Concat(new string[] { user.FirstName, " ", user.LastName });
                    ViewData["FullName"] = fullName;
                }

            }
            base.OnActionExecuted(filterContext);
        }
    }
}