using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(M183.Startup))]
namespace M183
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
