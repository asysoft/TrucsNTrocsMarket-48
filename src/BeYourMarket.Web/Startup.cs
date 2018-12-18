using BeYourMarket.Model.Enum;
using BeYourMarket.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(BeYourMarket.Web.Startup))]
namespace BeYourMarket.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            CreateRolesandUsers();

        }

        // In this method we will create default User roles and Admin user for login   
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Administrator"))
            {
                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = Enum_UserType.Administrator.ToString()
                };
                roleManager.Create(role);
            }

            // Creating All role    
            if (!roleManager.RoleExists(Enum_UserType.Professional.ToString()) )
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = Enum_UserType.Professional.ToString()
                };
                roleManager.Create(role);
            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists(Enum_UserType.BackOffAdmin.ToString()))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = Enum_UserType.BackOffAdmin.ToString()
                };
                roleManager.Create(role);

            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists(Enum_UserType.BackOffListings.ToString()))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = Enum_UserType.BackOffListings.ToString()
                };
                roleManager.Create(role);
            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists(Enum_UserType.BackOffPubs.ToString()))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                {
                    Name = Enum_UserType.BackOffPubs.ToString()
                };
                roleManager.Create(role);
            }

            //Here we create a Admin super user who will maintain the website                              
            string AdminASYEmail = "asysoft@yahoo.com";
            if (UserManager.FindByEmail(AdminASYEmail) == null)
            {
                var user = new ApplicationUser();
                user.UserName = "asy";
                user.Email = AdminASYEmail;
                string userPWD = "TnTtest2018";

                //////////////////////
                user.RegisterDate = DateTime.Now;
                user.LastAccessDate = DateTime.Now;
                user.AcceptEmail = true;
                user.LeadSourceID = 0; //?
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                user.TwoFactorEnabled = true;
                user.LockoutEnabled = false;
                user.AccessFailedCount = 5;
                user.Disabled = false;

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Administrator");

                }
            }

        }

    }
}
