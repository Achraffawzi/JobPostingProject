using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using JobPostingProject.Models;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace JobPostingProject.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext appDbContext;
        private JobPostingDBEntities1 db;

        // Sending new password through Gmail
        public ActionResult SendNewPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendNewPassword(ForgotPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mail = new MailMessage();
                    var loginInfo = new NetworkCredential("achrafawzi2000@gmail.com", "Lufthansa224$");
                    mail.From = new MailAddress(model.Email);
                    mail.To.Add(new MailAddress("achrafawzi2000@gmail.com"));
                    mail.Subject = "Password reinitialisation";
                    Random r = new Random();
                    int code = r.Next(1000, 9999);
                    mail.IsBodyHtml = true;
                    mail.Body = "Your new password is : " + "<b>" + code + "</b>" + "<br>" + "Use it to sign in";

                    var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = loginInfo;
                    smtpClient.Send(mail);

                    // update Aspnetuser
                    var user = UserManager.Users.Where(u => u.Email == model.Email).FirstOrDefault();
                    var newHashedPassword = UserManager.PasswordHasher.HashPassword(code.ToString());
                    user.PasswordHash = newHashedPassword;
                    UserManager.Update(user);
                    appDbContext.SaveChanges();
                    // Change password to our DB
                    Company company = this.db.Companies.Where(c => c.Email == model.Email).FirstOrDefault();
                    if (company != null)
                    {
                        company.Password = code.ToString();
                        db.SaveChanges();
                    }
                    else
                    {
                        Candidate candidate = this.db.Candidates.Where(c => c.Email == model.Email).FirstOrDefault();
                        if (candidate != null)
                        {
                            candidate.Password = code.ToString();
                            db.SaveChanges();
                        }
                        else
                        {
                            return View();
                        }
                    }
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
            return View();
        }

        public AccountController()
        {
            appDbContext = new ApplicationDbContext();
            db = new JobPostingDBEntities1();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ceci ne comptabilise pas les échecs de connexion pour le verrouillage du compte
            // Pour que les échecs de mot de passe déclenchent le verrouillage du compte, utilisez shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Email or password incorrect.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Nécessiter que l'utilisateur soit déjà connecté via un nom d'utilisateur/mot de passe ou une connexte externe
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Le code suivant protège des attaques par force brute contre les codes à 2 facteurs. 
            // Si un utilisateur entre des codes incorrects pendant un certain intervalle, le compte de cet utilisateur 
            // est alors verrouillé pendant une durée spécifiée. 
            // Vous pouvez configurer les paramètres de verrouillage du compte dans IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalide code.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult RegisterCompany()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCompany(RegisterViewModelCompany model)
        {
            if (ModelState.IsValid)
            {
                // Todo: Create a folder for each Company

                var folderPath = Server.MapPath("~/Data/Companies/" + model.Email);
                // Chech if Folder is Exists
                Byte[] logoData = null;
                //string _logoFileName;

                if (!Directory.Exists(folderPath))
                {
                    // Create Folder
                    Directory.CreateDirectory(Server.MapPath("~/Data/Companies/" + model.Email));
                    string fullPathInServer = "~/Data/Companies/" + model.Email + "/";

                    // Get file info of the logo
                    if (model.LogoFileName != null)
                    {
                        string logoFileName = Path.GetFileNameWithoutExtension(model.LogoFileName.FileName);
                        string logoFileExtension = Path.GetExtension(model.LogoFileName.FileName);
                        string _logoFileName = logoFileName + logoFileExtension;
                        model.Logo = fullPathInServer + _logoFileName;
                        _logoFileName = Path.Combine(Server.MapPath(fullPathInServer), _logoFileName);
                        model.LogoFileName.SaveAs(_logoFileName);



                        // Todo: convert the Company uploaded Photo as Byte Array before save to DB/ ApplicationDbContext => AspNetUserTable
                        if (Request.Files.Count > 0)
                        {
                            HttpPostedFileBase fileBase = Request.Files["LogoFileName"];
                            using (var binary = new BinaryReader(fileBase.InputStream))
                            {
                                logoData = binary.ReadBytes(fileBase.ContentLength);
                            }
                        }
                    }
                }

                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.Name, LastName = model.Name, UserPhoto = logoData };



                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "company");


                    // Add new company to the database
                    Company newCompany = new Company
                    {
                        CompanySecondID = user.Id,
                        Name = model.Name,
                        Logo = model.Logo,
                        Address = model.Adresse,
                        City = model.City,
                        PhoneNumber = model.PhoneNumber,
                        Description = model.Description,
                        Email = model.Email,
                        Password = model.Password
                    };
                    try
                    {
                        db.Companies.Add(newCompany);
                        db.SaveChanges();

                    }
                    catch (Exception)
                    {

                    }

                    // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                    // Envoyer un message électronique avec ce lien
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        public ActionResult ChangePasswordCompany()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePasswordCompanyAJAX(ChangePasswordViewModelCompany passwordView)
        {
            var userID = User.Identity.GetUserId();
            var currentUser = appDbContext.Users.Where(u => u.Id == userID).FirstOrDefault();
            if (!UserManager.CheckPassword(currentUser, passwordView.CurrentPassword))
            {
                return this.Json(new
                {
                    isChanged = false,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Change password to AspNetUser
                var newHashedPassword = UserManager.PasswordHasher.HashPassword(passwordView.NewPassword);
                currentUser.PasswordHash = newHashedPassword;
                appDbContext.Entry(currentUser).State = System.Data.Entity.EntityState.Modified;
                appDbContext.SaveChanges();
                // Change password to our DB
                Company company = this.db.Companies.Where(c => c.CompanySecondID == userID).FirstOrDefault();
                company.Password = passwordView.NewPassword;
                db.SaveChanges();
                return this.Json(new
                {
                    isChanged = true,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public async Task<JsonResult> UserAlreadyExistsAsync(string Email)
        {
            var result =
                await UserManager.FindByNameAsync(Email) ??
                await UserManager.FindByEmailAsync(Email);
            return Json(result == null, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModelCandidate model)
        {

            if (ModelState.IsValid)
            {
                // Todo: Create a folder for each Candidate

                var folderPath = Server.MapPath("~/Data/Candidate/" + model.Email);
                // Chech if Folder is Exists
                byte[] imageData = null;
                if (!Directory.Exists(folderPath))
                {
                    // Create Folder
                    Directory.CreateDirectory(Server.MapPath("~/Data/Candidate/" + model.Email));
                    string fullPathInServer = "~/Data/Candidate/" + model.Email + "/";

                    // Save Cv In Server Side 
                    if (model.CvFileName != null)
                    {
                        string cvFileName = Path.GetFileNameWithoutExtension(model.CvFileName.FileName);
                        string cvFileExtension = Path.GetExtension(model.CvFileName.FileName);
                        string _cvFileName = cvFileName + cvFileExtension;
                        model.CV = fullPathInServer + _cvFileName;
                        _cvFileName = Path.Combine(Server.MapPath(fullPathInServer), _cvFileName);
                        model.CvFileName.SaveAs(_cvFileName);

                    }

                    // Save Photo In Server Side 
                    if (model.PhotoFileName != null)
                    {
                        string photoFileName = Path.GetFileNameWithoutExtension(model.PhotoFileName.FileName);
                        string photoFileExtension = Path.GetExtension(model.PhotoFileName.FileName);
                        string _photoFileName = photoFileName + photoFileExtension;
                        model.Photo = fullPathInServer + _photoFileName;
                        _photoFileName = Path.Combine(Server.MapPath(fullPathInServer), _photoFileName);
                        model.PhotoFileName.SaveAs(_photoFileName);

                        // Todo: convert the user uploaded Photo as Byte Array before save to DB/ ApplicationDbContext => AspNetUserTable

                        if (Request.Files.Count > 0)
                        {
                            HttpPostedFileBase fileBase = Request.Files["PhotoFileName"];
                            using (var binary = new BinaryReader(fileBase.InputStream))
                            {
                                imageData = binary.ReadBytes(fileBase.ContentLength);
                            }
                        }
                    }

                    // Save Cover Letter In Server Side 
                    if (model.CoverLetterFileName != null)
                    {

                        string coverLetterFileName = Path.GetFileNameWithoutExtension(model.CoverLetterFileName.FileName);
                        string coverLetterFileExtension = Path.GetExtension(model.CoverLetterFileName.FileName);
                        string _coverLetterFileName = coverLetterFileName + coverLetterFileExtension;
                        model.CoverLetter = fullPathInServer + _coverLetterFileName;
                        _coverLetterFileName = Path.Combine(Server.MapPath(fullPathInServer), _coverLetterFileName);
                        model.CoverLetterFileName.SaveAs(_coverLetterFileName);
                    }
                }


                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };

                if (model.PhotoFileName != null)
                {
                    user.UserPhoto = imageData;
                }

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var res = await UserManager.AddToRoleAsync(user.Id, "candidate");
                    using (JobPostingDBEntities1 db = new JobPostingDBEntities1())
                    {
                        var newCandidate = new Candidate
                        {
                            CandidateSecondID = user.Id,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Address = model.Adresse,
                            Bio = model.Bio,
                            DateOfBirth = model.DateOfBirth,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Password = model.Password,
                            Photo = model.Photo,
                            Cv = model.CV,
                            CoverLetter = model.CoverLetter
                        };

                        //db.Entry<Candidate>().State = System.Data.Entity.EntityState.Added;

                        db.Candidates.Add(newCandidate);
                        db.SaveChanges();
                    }

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);


                    // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                    // Envoyer un message électronique avec ce lien
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");


                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        public ActionResult ChangePasswordCandidate()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangePasswordCandidateAJAX(ChangePasswordViewModelCandidate passwordView)
        {
            var userID = User.Identity.GetUserId();
            var currentUser = appDbContext.Users.Where(u => u.Id == userID).FirstOrDefault();
            if (!UserManager.CheckPassword(currentUser, passwordView.CurrentPassword))
            {
                return Json(new
                {
                    isChanged = false,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Change password to AspNetUser
                var newHashedPassword = UserManager.PasswordHasher.HashPassword(passwordView.NewPassword);
                currentUser.PasswordHash = newHashedPassword;
                appDbContext.Entry(currentUser).State = System.Data.Entity.EntityState.Modified;
                appDbContext.SaveChanges();
                // Change password to our DB
                Candidate candidate = this.db.Candidates.Where(c => c.CandidateSecondID == userID).FirstOrDefault();
                candidate.Password = passwordView.NewPassword;
                db.SaveChanges();
                return Json(new
                {
                    isChanged = true,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Delete user account
        public ActionResult DeleteAccountCompany()
        {
            return View();
        }

        [HttpPost]
        async public Task<ActionResult> DeleteAccountCompany(string id)
        {
            try
            {
                // Signout first
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                /*
                TODO :
                Search for the user
                Get the role of the current user
                Remove the role from it
                delete the user from Aspnetuser
                delete all the user's files
                Delete the user from the DB
            */

                // Get the current logged in company
                string userID = User.Identity.GetUserId();
                // Get the role of the current user
                var affectedRoles = await UserManager.GetRolesAsync(userID);
                // Search for the user
                var user = await UserManager.FindByIdAsync(userID);
                // Remove the role from it
                foreach (var role in affectedRoles)
                {
                    var result = await UserManager.RemoveFromRolesAsync(userID, role);
                }
                // delete the user from Aspnetuser
                await UserManager.DeleteAsync(user);

                // Delete all the files associated with the current logged in user
                Company currentCompany = this.db.Companies.FirstOrDefault(c => c.CompanySecondID == userID);
                var folderPath = Server.MapPath("~/Data/Companies/" + currentCompany.Email);
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }

                // Delete the user from the DB
                // First we need to delete all the annoucements done by the current logged in company
                List<Announcement> deletingAnnouncements = this.db.Announcements.Where(a => a.CompanyID == currentCompany.CompanyID).ToList();
                foreach (var item in deletingAnnouncements)
                {
                    this.db.Announcements.Remove(item);
                    this.db.SaveChanges();
                }
                this.db.Companies.Remove(currentCompany);
                await this.db.SaveChangesAsync();


                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewData["ResultMessage"] = "Something went wrong!";
                return View();
            }
        }

        public ActionResult DeleteAccountCandidate()
        {
            return View();
        }

        [HttpPost]
        async public Task<ActionResult> DeleteAccountCandidate(string id)
        {
            try
            {
                // Signout first
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                /*
                TODO :
                Search for the user
                Get the role of the current user
                Remove the role from it
                delete the user from Aspnetuser
                delete all the user's files
                Delete the user from the DB
            */

                // Get the current logged in company
                string userID = User.Identity.GetUserId();
                // Get the role of the current user
                var affectedRoles = await UserManager.GetRolesAsync(userID);
                // Search for the user
                var user = await UserManager.FindByIdAsync(userID);
                // Remove the role from it
                foreach (var role in affectedRoles)
                {
                    var result = await UserManager.RemoveFromRolesAsync(userID, role);
                }
                // delete the user from Aspnetuser
                await UserManager.DeleteAsync(user);

                // Delete all the files associated with the current logged in user
                Candidate currentCandidate = this.db.Candidates.FirstOrDefault(c => c.CandidateSecondID == userID);
                var folderPath = Server.MapPath("~/Data/Candidate/" + currentCandidate.Email);
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                // Delete the user from the DB
                // First we need to delete all the annoucements done by the current logged in company
                List<Application> deletingApplications = this.db.Applications.Where(a => a.CandidateID == currentCandidate.CandidateID).ToList();
                foreach (var item in deletingApplications)
                {
                    this.db.Applications.Remove(item);
                    this.db.SaveChanges();
                }
                this.db.Candidates.Remove(currentCandidate);
                await this.db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewData["ResultMessage"] = "Something went wrong!";
                return View();
            }
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPasswordAsp()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordAsp(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Ne révélez pas que l'utilisateur n'existe pas ou qu'il n'est pas confirmé
                    return View("ForgotPasswordConfirmation");
                }

                // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                // Envoyer un message électronique avec ce lien
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Réinitialiser le mot de passe", "Réinitialisez votre mot de passe en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Ne révélez pas que l'utilisateur n'existe pas
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Demandez une redirection vers le fournisseur de connexions externe
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Générer le jeton et l'envoyer
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Connecter cet utilisateur à ce fournisseur de connexion externe si l'utilisateur possède déjà une connexion
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si l'utilisateur n'a pas de compte, invitez alors celui-ci à créer un compte
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtenez des informations sur l’utilisateur auprès du fournisseur de connexions externe
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Applications auxiliaires
        // Utilisé(e) pour la protection XSRF lors de l'ajout de connexions externes
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}