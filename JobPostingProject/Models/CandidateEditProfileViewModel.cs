using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPostingProject.Models
{
    public class CandidateEditProfileViewModel 
    {
        [Display(Name = "Candidate Unique ID")]
        public string CandidateSecondID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Adresse")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]{10,}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "About You")]
        [AllowHtml]
        public string Bio { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Photo")]
        public string Photo { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "CV")]
        public string Cv { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Cover Letter")]
        public string CoverLetter { get; set; }

        public HttpPostedFileBase CvFileName { get; set; }
        public HttpPostedFileBase CoverLetterFileName { get; set; }
        public HttpPostedFileBase PhotoFileName { get; set; }
    }
}