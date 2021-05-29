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
                    // Get the user details
                    var user = context.Users.SingleOrDefault(u => u.UserName.Equals(userName));
                    bool isCompany = User.IsInRole("Company");
                    bool isCandidate = User.IsInRole("Candidate");

                    // Check if FirstName and LastName are equals => UserType = Company
                    if (user.FirstName.Equals(user.LastName) && isCompany)
                    {
                        ViewData["FullName"] = user.FirstName;
                    }
                    else if ((user.FirstName.Equals(user.LastName) && isCandidate) || (!user.FirstName.Equals(user.LastName) && isCandidate))
                    {
                        string fullName = String.Concat(new string[] { user.FirstName, " ", user.LastName });
                        ViewData["FullName"] = fullName;
                    }

                }

            }
            base.OnActionExecuted(filterContext);
        }
    }
}