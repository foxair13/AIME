using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCDIS.Startup))]
namespace MVCDIS
{

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
         
            //app.MapSignalR();

        }



    }
}