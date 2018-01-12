using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IS7012.AST.ParentsBank.Startup))]
namespace IS7012.AST.ParentsBank
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
