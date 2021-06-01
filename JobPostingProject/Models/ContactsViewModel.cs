using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobPostingProject.Models
{
    public class ContactsViewModel
    {
        [Required(ErrorMessage = "{0} is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "{0} is required.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Gmail Password")]
        public string Password { get; set; }
    }
}