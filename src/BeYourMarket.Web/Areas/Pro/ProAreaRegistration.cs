using System.Web.Mvc;

namespace BeYourMarket.Web.Areas.Pro
{
    public class ProAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Pro";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            //context.MapRoute(
            //    "Pro_UserPro",
            //    "Pro/UserPro/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional } , 
            //     namespaces: new[] { "BeYourMarket.Web.Areas.Pro.Controllers" }
            //).DataTokens.Add("area", "Pro");

            context.MapRoute(
                "Pro_default",
                "UserPro/{action}/{id}",
               defaults: new { controller = "UserPro", action = "Index", id = UrlParameter.Optional }
               , namespaces: new[] { "BeYourMarket.Web.Areas.Pro.Controllers" }
            );

            context.MapRoute(
                "Pro_defaultExt",
                "Pro/{controller}/{action}/{id}",
               defaults: new { controller = "UserPro", action = "Index", id = UrlParameter.Optional }
               , namespaces: new[] { "BeYourMarket.Web.Areas.Pro.Controllers" }
            );


        }
    }
}