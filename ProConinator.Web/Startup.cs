using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProConinator.Web.Startup))]
namespace ProConinator.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
