using JobPostingProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobPostingProject.Startup))]
namespace JobPostingProject
{
    public partial class Startup
    {
        ApplicationDbContext context = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            //CreateUsers();
        }

        // In this method we will create default User roles and Admin user for login    
        private async void CreateRoles()
        {
            

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            


            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Company"))
            {

                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Company";
                await roleManager.CreateAsync(role);
            }
            if (!roleManager.RoleExists("Candidate"))
            {

                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Candidate";
                await roleManager.CreateAsync(role);
            }
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool    
                var role = new IdentityRole();
                role.Name = "Admin";
                await roleManager.CreateAsync(role);
            }
        }

            // Creating users
        private void CreateUsers()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var jamal = new ApplicationUser
            {
                UserName = "IDJA",
                Email = "Jamal@Idaissa.com",
                FirstName = "Jamal",
                LastName = "IdAissa"
            };

            var achraf = new ApplicationUser
            {
                UserName = "ItsYeAlpha",
                Email = "Achraf@Fawzi.com",
                FirstName = "Achraf",
                LastName = "FAWZI"
            };

            var checkJamal = userManager.Create(jamal, "Aze123@");
            if (checkJamal.Succeeded)
            {
                userManager.AddToRole(jamal.Id, "Admin");
            }

            var checkAchraf = userManager.Create(achraf, "Aze123@");

            if (checkAchraf.Succeeded)
            {
                userManager.AddToRole(achraf.Id, "Admin");
            }
        }
    }
}
