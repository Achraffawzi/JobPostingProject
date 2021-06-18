using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobPostingProject.Models
{
    public class CompanyEditViewModel
    {

        public string CompanySecondID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string Logo { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        [NotMapped]
        public HttpPostedFileBase LogoFileName { get; set; }
    }
}