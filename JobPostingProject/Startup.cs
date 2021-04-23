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
        }
    }
}
