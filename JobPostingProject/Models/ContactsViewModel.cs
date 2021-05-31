using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobPostingProject.Models
{
    public class ContactsViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }

        [Required]
        [Display(Name = "Gmail Password")]
        public string Password { get; set; }
    }
}