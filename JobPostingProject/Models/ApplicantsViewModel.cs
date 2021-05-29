using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobPostingProject.Models
{
    public class ApplicantsViewModel
    {
        //[Required(ErrorMessage = "{0} is required")]
        //[Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        //[Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        //[Display(Name = "Phone Number")]
        //[RegularExpression("^[0-9]{10,}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string CvFileName { get; set; }
        public string CoverLetterFileName { get; set; }
    }
}