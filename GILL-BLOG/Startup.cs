using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GILL_BLOG.Startup))]
namespace GILL_BLOG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
