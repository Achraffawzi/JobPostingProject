using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace JobPostingProject.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Mémoriser ce navigateur ?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }


    public class RegisterViewModelCandidate
    {
        //-------------- Candidate ---------------
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
        public string Adresse { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]{10,}$",ErrorMessage ="Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "About You")]
        public string Bio { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Photo")]
        public string Photo { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "CV")]
        public string CV { get; set; }

        //[Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Cover Letter")]
        public string CoverLetter { get; set; }

        [Remote("UserAlreadyExistsAsync", "Account", ErrorMessage = "User with this Email already exists")]
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage ="Invalid Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "{0} must have at less {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password ")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }

        public HttpPostedFileBase CvFileName { get; set; }
        public HttpPostedFileBase CoverLetterFileName { get; set; }
        public HttpPostedFileBase PhotoFileName { get; set; }
    }

   

    public class RegisterViewModelCompany
    {

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Phone Number")]
        [RegularExpression("^[0-9]{10,15}$",ErrorMessage ="Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        //[Required(ErrorMessage = "Logo is required")]
        [Display(Name = "Logo")]
        public string Logo { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Remote("UserAlreadyExistsAsync", "Account", ErrorMessage = "Company with this Email already exists")]
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = " {0} must have at less {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password not match")]
        public string ConfirmPassword { get; set; }

        public HttpPostedFileBase LogoFileName { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Le nouveau mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
