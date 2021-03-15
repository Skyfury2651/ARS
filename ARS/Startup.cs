using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ARS.Startup))]
namespace ARS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
