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
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
        }

        // In this method we will create default User roles and Admin user for login    
        private async void CreateRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

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
        }
    } 
}
