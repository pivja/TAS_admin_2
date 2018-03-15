using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TAS_admin.Startup))]
namespace TAS_admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
