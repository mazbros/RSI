using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RSI.Startup))]
namespace RSI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
