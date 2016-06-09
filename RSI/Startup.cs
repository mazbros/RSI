using Microsoft.Owin;
using Owin;
using RSI;

[assembly: OwinStartup(typeof(Startup))]

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