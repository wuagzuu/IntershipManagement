using Microsoft.Owin;
using OfficeOpenXml;
using Owin;

[assembly: OwinStartupAttribute(typeof(IntershipManagement.Startup))]
namespace IntershipManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }
    }
}
