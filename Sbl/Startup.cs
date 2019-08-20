using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sbl.Startup))]
namespace Sbl
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
