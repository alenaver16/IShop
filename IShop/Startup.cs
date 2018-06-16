using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IShop.Startup))]
namespace IShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
